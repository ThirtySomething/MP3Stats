﻿using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Emit;

namespace net.derpaul.cdstats
{
    /// <summary>
    /// ERM model for CD statistics
    /// </summary>
    public class CdStats : DbContext
    {
        /// <summary>
        /// Entity for MP3 imports
        /// </summary>
        public DbSet<MP3Import> MP3Import { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="options"></param>
        public CdStats(DbContextOptions<CdStats> options) : base(options)
        {
        }

        /// <summary>
        /// Initialize connection string and driver version
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = $"server={DataCollectorConfig.Instance.DBServer};port={DataCollectorConfig.Instance.DBPort};database={DataCollectorConfig.Instance.DBDatabase};user={DataCollectorConfig.Instance.DBUserId};password={DataCollectorConfig.Instance.DBPassword}";
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 21));
            optionsBuilder.UseMySql(connectionString, serverVersion, options =>
            {
                options.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
            });

            base.OnConfiguring(optionsBuilder);
        }

        /// <summary>
        /// ERM as code
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Entity for MP3 import
            modelBuilder.Entity<MP3Import>().ToTable("mp3import");
            modelBuilder.Entity<MP3Import>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.HasIndex(e => e.filename).IsUnique();
            });

            // Create the model
            base.OnModelCreating(modelBuilder);
        }
    }
}
