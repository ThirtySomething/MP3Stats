﻿using net.derpaul.mp3stats.model;

namespace net.derpaul.mp3stats.plugin
{
    /// <summary>
    /// Plugin to determine various duration statistics
    /// </summary>
    public class PluginTracksArtist : PluginBase
    {

        /// <summary>
        /// Get statistic name
        /// </summary>
        public override string Name { get; } = "Total tracks per artist";

        /// <summary>
        /// Major entry point of plugin
        /// </summary>
        /// <param name="dbConnection">Valid DB connection object</param>
        /// <param name="outputPath">Path to write own statistics file</param>
        /// <param name="logger">Passed logger to write infomration</param>
        public override void CollectStatistic(MP3Stats dbConnection, string outputPath, NLog.Logger logger)
        {
            var name_file = GetFilename(outputPath);
            var artists_total = dbConnection.MP3Import.Select(a => a.artist).Distinct().Count();
            var trk_tot = dbConnection.MP3Import.Count();
            var tracks_artists = dbConnection.MP3Import.GroupBy(a => a.artist).Select(a => new { artist = a.Key, tracks = a.Count() }).OrderByDescending(a => a.tracks).ThenBy(a => a.artist).ToList();
            var dur_tot = dbConnection.MP3Import.Sum(myimport => myimport.durationms);

            using (StreamWriter statistic_file = new StreamWriter(name_file))
            {
                MP3StatsUtil.WriteHeader(statistic_file, this.Name, this.GetType().Name);

                statistic_file.WriteLine("<b>Tracks:</b> {0} - <b>Artists:</b> {1} ({2})<br>",
                    trk_tot,
                    artists_total,
                    MP3StatsUtil.GetStringFromMs(dur_tot)
                );
                statistic_file.WriteLine("<p>");
                var tracks_mem = trk_tot;
                foreach (var record in tracks_artists)
                {
                    if (tracks_mem != record.tracks)
                    {
                        statistic_file.WriteLine("</p>");
                        statistic_file.WriteLine("<p>");
                        tracks_mem = record.tracks;
                    }
                    var artists_duration_total = dbConnection.MP3Import.Where(a => a.artist == record.artist).Sum(a => a.durationms);

                    statistic_file.WriteLine("<b>Tracks:</b> {0} - <b>Artist:</b> {1} ({2})<br>",
                        record.tracks,
                        record.artist,
                        MP3StatsUtil.GetStringFromMs(artists_duration_total)
                    );
                }
                statistic_file.WriteLine("</p>");
            }
        }
    }
}