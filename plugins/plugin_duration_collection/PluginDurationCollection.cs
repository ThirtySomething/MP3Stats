﻿using net.derpaul.cdstats.model;

namespace net.derpaul.cdstats.plugin
{
    /// <summary>
    /// Plugin to determine various duration statistics
    /// </summary>
    public class PluginDurationCollection : PluginBase
    {

        /// <summary>
        /// Get statistic name
        /// </summary>
        public override string Name { get; } = "Collection durations";

        /// <summary>
        /// Major entry point of plugin
        /// </summary>
        /// <param name="dbConnection">Valid DB connection object</param>
        /// <param name="outputPath">Path to write own statistics file</param>
        /// <param name="logger">Passed logger to write infomration</param>
        public override void CollectStatistic(CdStats dbConnection, string outputPath, NLog.Logger logger)
        {
            var dur_min = (from myimport in dbConnection.MP3Import select myimport).Min(myimport => myimport.durationms);
            var dur_avg = (from myimport in dbConnection.MP3Import select myimport).Average(myimport => myimport.durationms);
            var dur_max = (from myimport in dbConnection.MP3Import select myimport).Max(myimport => myimport.durationms);
            var dur_tot = (from myimport in dbConnection.MP3Import select myimport).Sum(myimport => myimport.durationms);
            var trk_tot = (from myimport in dbConnection.MP3Import select myimport).Count();

            var track_short = (from myimport in dbConnection.MP3Import select myimport).Where(track => track.durationms == dur_min).FirstOrDefault();
            var track_long = (from myimport in dbConnection.MP3Import select myimport).Where(track => track.durationms == dur_max).FirstOrDefault();

            var name_file = GetFilename(outputPath);
            using (StreamWriter statistic_file = new StreamWriter(name_file))
            {
                WriteHeader(statistic_file);

                TimeSpan time = TimeSpan.FromMilliseconds(dur_min);
                DateTime startdate = new DateTime() + time;

                statistic_file.WriteLine("<b>Shortest track length:</b> {0} - {1} ({2})<br>",
                    GetStringFromMs(dur_min),
                    track_short.title ?? "",
                    track_short.artist ?? ""
                );
                statistic_file.WriteLine("<b>Average track length:</b> {0} - {1} tracks<br>",
                    GetStringFromMs(dur_avg),
                    trk_tot
                );
                statistic_file.WriteLine("<b>Longest track length:</b> {0} - {1} ({2})<br>",
                    GetStringFromMs(dur_max),
                    track_long.title ?? "",
                    track_long.artist ?? ""
                );
                statistic_file.WriteLine("<b>Playtime overall:</b> {0} - {1} tracks<br>",
                    GetStringFromMs(dur_tot),
                    trk_tot
                );
            }
        }
    }
}