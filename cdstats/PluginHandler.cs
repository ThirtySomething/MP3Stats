﻿namespace net.derpaul.cdstats
{
    /// <summary>
    /// Class to deal with collections of plugins of type ICDStatsPlugin
    /// </summary>
    internal class PluginHandler
    {
        /// <summary>
        /// The path to the plugins
        /// </summary>
        private string PluginPath { get; set; }

        /// <summary>
        /// DB Connection object
        /// </summary>
        private CdStats DBConnection { get; set; }

        /// <summary>
        /// List of statistic plugins
        /// </summary>
        private List<ICDStatsPlugin> StatisticPlugins { get; set; }

        /// <summary>
        /// Constructor of plugin handler
        /// </summary>
        /// <param name="pluginPath">Path to plugins</param>
        internal PluginHandler(string pluginPath, CdStats dbConnection)
        {
            PluginPath = pluginPath;
            DBConnection = dbConnection;
            StatisticPlugins = new List<ICDStatsPlugin>();
        }

        /// <summary>
        /// Initialize statistic plugins
        /// </summary>
        /// <returns>true on success, otherwise false</returns>
        private bool InitCDStatsPlugins()
        {
            StatisticPlugins = PluginLoader<ICDStatsPlugin>.PluginsLoad(PluginPath, CDStatsConfig.Instance.PluginProductName);
            if (StatisticPlugins.Count == 0)
            {
                System.Console.WriteLine($"{nameof(InitCDStatsPlugins)}: No sensor plugins found in [{PluginPath}].");
                return false;
            }

            foreach (var plugin in StatisticPlugins)
            {
                try
                {
                    plugin.IsInitialized = plugin.Init();
                    if (plugin.IsInitialized)
                    {
                        System.Console.WriteLine($"{nameof(InitCDStatsPlugins)}: Initialized [{plugin.Name}] plugin.");
                    }
                    else
                    {
                        System.Console.WriteLine($"{nameof(InitCDStatsPlugins)}: Failed to initialize [{plugin.Name}] plugin.");
                    }
                }
                catch (Exception e)
                {
                    System.Console.WriteLine($"{nameof(InitCDStatsPlugins)}: Cannot init plugin [{plugin.GetType()}] => [{e.Message}]");
                    System.Console.WriteLine($"{nameof(InitCDStatsPlugins)}: Inner exception => [{e.InnerException}]");
                    continue;
                }
                System.Console.WriteLine();
            }

            return true;
        }

        /// <summary>
        /// Initialize all plugins
        /// </summary>
        /// <returns>true on success, otherwise false</returns>
        internal bool Init()
        {
            bool InitDone = InitCDStatsPlugins();
            return InitDone;
        }

        /// <summary>
        /// Run CDStats plugins
        /// </summary>
        internal void Process()
        {
            foreach (var plugin in StatisticPlugins)
            {
                if (!(plugin is ICDStatsPlugin))
                {
                    // Skip invalid plugins
                    continue;
                }

                // Call main method
                plugin.CollectStatistic(DBConnection);
            }
        }
    }
}