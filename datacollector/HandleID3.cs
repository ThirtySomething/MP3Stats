﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace net.derpaul.cdstats
{
    internal class HandleID3
    {
        private List<string> filenamesMP3;
        private CdStats DBInstance;

        public HandleID3(List<string> filenamesMP3)
        {
            this.filenamesMP3 = filenamesMP3;
        }

        public bool Init()
        {
            bool ret = true;

            DBInstance = new CdStats(new DbContextOptions<CdStats>());
            DBInstance.Database.EnsureCreated();

            return ret;
        }

        public void Process()
        {
            foreach (string filename in filenamesMP3)
            {
                // Handle of filename
                var Ofilename = DBInstance.Filename.Where(a => a.name == filename).FirstOrDefault();
                if (null == Ofilename) {
                    Ofilename = new Filename();
                    Ofilename.name = filename;
                    Ofilename.import = DateTime.UtcNow;
                    DBInstance.Add(Ofilename);
                    DBInstance.SaveChanges();
                }

                // Read ID3 tag
                var tagID3 = TagLib.File.Create(filename);

                // Handle of album
                var album = tagID3.Tag.Album;
                var Oalbum = DBInstance.Album.Where(a => a.name == album).FirstOrDefault();
                if (Oalbum == null)
                {
                    Oalbum = new Album { name = album };
                    DBInstance.Add(Oalbum);
                    DBInstance.SaveChanges();
                }

                // Handle of artist
                var artist = tagID3.Tag.FirstPerformer;
                var Oartist = DBInstance.Artist.Where(a => a.name == artist).FirstOrDefault();
                if (Oartist == null)
                {
                    Oartist = new Artist { name = artist };
                    DBInstance.Add(Oartist);
                    DBInstance.SaveChanges();
                }

                // Handle of genres
                var genre = tagID3.Tag.FirstGenre;
                var Ogenre = DBInstance.Genre.Where(a => a.name == genre).FirstOrDefault();
                if (Ogenre == null)
                {
                    Ogenre = new Genre { name = genre };
                    DBInstance.Add(Ogenre);
                    DBInstance.SaveChanges();
                }

                // Handle of titles
                var title = tagID3.Tag.Title;
                var Otitle= DBInstance.Title.Where(a => a.name == title).FirstOrDefault();
                if (Otitle == null)
                {
                    Otitle = new Title { name = title };
                    DBInstance.Add(Otitle);
                    DBInstance.SaveChanges();
                }
            }
        }
    }
}
