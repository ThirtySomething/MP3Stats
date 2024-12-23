namespace net.derpaul.mp3stats
{
    /// <summary>
    /// Interface for configuration saving
    /// </summary>
    public interface IConfigObject
    {
        /// <summary>
        /// Save current config
        /// </summary>
        void Save();

        /// <summary>
        /// To set default values
        /// </summary>
        void SetDefaults();
    }
}