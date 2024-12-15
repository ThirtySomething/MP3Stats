﻿using ATL;
using Microsoft.EntityFrameworkCore;
using net.derpaul.cdstats.model;
using Newtonsoft.Json.Linq;

namespace net.derpaul.cdstats
{
    /// <summary>
    /// Extract meta data of MP3 file
    /// </summary>
    internal class HandleID3
    {
        /// <summary>
        /// List of MP3 files
        /// </summary>
        private List<MP3File> filenamesMP3;

        /// <summary>
        /// Connection to database (Entity Framework)
        /// </summary>
        private CdStats DBInstance;

        /// <summary>
        /// Pass list of MP3 filenames to handler in constructor
        /// </summary>
        /// <param name="filenamesMP3"></param>
        public HandleID3(List<MP3File> filenamesMP3)
        {
            this.filenamesMP3 = filenamesMP3;
        }

        /// <summary>
        /// Init of database
        /// </summary>
        /// <returns>true on success</returns>
        /// <returns>false on failure</returns>
        public bool Init()
        {
            bool ret = true;

            DBInstance = new CdStats(new DbContextOptions<CdStats>());
            DBInstance.Database.EnsureCreated();

            return ret;
        }

        /// <summary>
        /// Update ERM object with ID3 data
        /// </summary>
        /// <param name="dbObject"></param>
        /// <param name="metaData"></param>
        private void setMetaData(MP3Import dbObject, Track metaData)
        {
            dbObject.artist = metaData.Artist;
            dbObject.album = metaData.Album;
            dbObject.title = metaData.Title;
            dbObject.genre = metaData.Genre;
            dbObject.durationms = metaData.DurationMs;
            dbObject.tracknumber = metaData.TrackNumber ?? 0;
            dbObject.tracktotal = metaData.TrackTotal ?? 0;
            dbObject.year = metaData.Year ?? 0;
            dbObject.discnumber = metaData.DiscNumber ?? 0;
            dbObject.disctotal = metaData.DiscTotal ?? 0;
            dbObject.bitrate = metaData.Bitrate;
            dbObject.samplerate = metaData.SampleRate;

            if (dbObject.filename.StartsWith("ac_dc"))
            {
                var dataTranslation = JObject.Parse(DataCollectorConfig.Instance.DataTranslation);
                foreach (var item in dataTranslation)
                {
                    if (dbObject.artist.Equals(item.Key))
                    {
                        dbObject.artist = item.Value.ToString();
                    }
                }
            }
        }

        /// <summary>
        /// Major process of import
        /// </summary>
        /// <param name="pathprefix"></param>
        public void Process(string pathprefix)
        {
            foreach (MP3File ofile in filenamesMP3)
            {
                // Read ID3 tag
                var tagID3 = new Track(ofile.filename);

                // Strip prefix
                string pname = ofile.filename;
                pname = pname.Replace(pathprefix, "");

                // Handle MP3 import
                var OMP3Import = DBInstance.MP3Import.Where(a => a.filename == pname).FirstOrDefault();
                if (null == OMP3Import)
                {
                    // Record not in database, init with file data data
                    OMP3Import = new MP3Import();
                    OMP3Import.filename = pname;
                    // OMP3Import.file_hash = ofile.filehash;
                    OMP3Import.date_file_mod = ofile.date_file_mod;
                    DBInstance.Add(OMP3Import);

                    // Enlarge record with ID3 meta data
                    setMetaData(OMP3Import, tagID3);

                    // Save to database
                    DBInstance.SaveChanges();

                    System.Console.WriteLine($"Fresh import of track [{pname}]");

                    // Continue with next file
                    continue;
                }

                if ((OMP3Import.date_file_mod == ofile.date_file_mod)
                    //&& (OMP3Import.file_hash == file_hash)
                    )
                {
                    System.Console.WriteLine($"Skip unchanged track [{pname}]");

                    // No changes since last import, data already present
                    continue;
                }

                // Update file meta data
                OMP3Import.date_file_mod = ofile.date_file_mod;
                // OMP3Import.file_hash = ofile.filehash;

                // Update ID3 meta data in record
                setMetaData(OMP3Import, tagID3);

                // Save to database
                DBInstance.SaveChanges();

                System.Console.WriteLine($"Update of track [{pname}]");
            }
        }
    }
}
