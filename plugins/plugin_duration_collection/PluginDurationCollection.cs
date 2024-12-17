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
        public override string Name { get; } = "Collection Durations";

        /// <summary>
        /// Collect statistics, main method of plugin
        /// </summary>
        /// <param name="dbConnection"></param>
        /// <param name="outputPath">Path to write own statistics file</param>
        public override void CollectStatistic(CdStats dbConnection, string outputPath)
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

                statistic_file.WriteLine("<b>Shortest track length:</b> " + GetStringFromMs(dur_min) + " - " + track_short.title + " (" + track_short.artist + ")<br>");
                statistic_file.WriteLine("<b>Average track length:</b> " + GetStringFromMs(dur_avg) + " - " + trk_tot + " tracks<br>");
                statistic_file.WriteLine("<b>Longest track length:</b> " + GetStringFromMs(dur_max) + " - " + track_long.title + " (" + track_long.artist + ")<br>");
                statistic_file.WriteLine("<b>Playtime overall:</b> " + GetStringFromMs(dur_tot) + "<br>");
            }
        }
    }
}