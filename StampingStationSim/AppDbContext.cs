using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace StampingStationSim
{
    /// <summary>
    /// Bridge between C# and SQLite DB
    /// </summary>
    internal class AppDbContext : DbContext
    {
        public DbSet<AlarmRecord> alarmHistory { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=StationHistorian.db");
        }
    }
}
