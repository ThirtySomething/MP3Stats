using System.Xml.Serialization;

namespace net.derpaul.cdstats
{
    /// <summary>
    /// Configuration settings of datacollector to read the bricklets
    /// </summary>
    public class DataCollectorConfig : ConfigLoader<DataCollectorConfig>, IConfigObject
    {
        /// <summary>
        /// To set default values
        /// </summary>
        public void SetDefaults()
        {
            MP3Path = "m:\\";
            MP3Pattern = "*.mp3";
        }

        /// <summary>
        /// Root path of MP3 collection
        /// </summary>
        public string MP3Path { get; set; }

        /// <summary>
        /// Pattern to search for
        /// </summary>
        public string MP3Pattern { get;set; }

        /// <summary>
        /// Product name of plugin set in AssemblyInfo.cs
        /// This is hardcoded and not configurable!
        /// </summary>
        [XmlIgnore]
        public string PluginProductName { get; } = "net.derpaul.cdstats.plugin";
    }
}