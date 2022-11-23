using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BabyFeedingRecordWebApplication.Models;

namespace BabyFeedingRecordWebApplication.Data
{
    public class BabyFeedingRecordWebApplicationContext : DbContext
    {
        public BabyFeedingRecordWebApplicationContext (DbContextOptions<BabyFeedingRecordWebApplicationContext> options)
            : base(options)
        {
        }

        public DbSet<BabyFeedingRecordWebApplication.Models.FeedingRecord> FeedingRecord { get; set; } = default!;

        public DbSet<BabyFeedingRecordWebApplication.Models.FeedingStatistics> FeedingStatistics { get; set; }

        public DbSet<BabyFeedingRecordWebApplication.Models.MovingAvg> MovingAvg { get; set; }

        public DbSet<BabyFeedingRecordWebApplication.Models.BabyFood> BabyFood { get; set; }
    }
}
