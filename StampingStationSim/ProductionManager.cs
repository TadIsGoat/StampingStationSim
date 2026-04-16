using System;
using System.Collections.Generic;
using System.Text;

namespace StampingStationSim
{
    /// <summary>
    /// Manages the production database and the local good parts count
    /// </summary>
    internal class ProductionManager
    {
        public int goodPartsCount { get; private set; } = 0;

        /// <summary>
        /// Initializes a new instance of the ProductionManager class and ensures the database is created.
        /// </summary>
        public ProductionManager()
        {
            using (var db = new AppDbContext())
            {
                db.Database.EnsureCreated();
                goodPartsCount = db.productionHistory.Count();
            }
        }

        /// <summary>
        /// Adds a new part to the database and to the local good parts count.
        /// </summary>
        /// <param name="cycleTime">Says how long it took to create exact part.</param>
        public void AddGoodPart(int cycleTime)
        {
            goodPartsCount++;

            using (var db = new AppDbContext())
            {
                var newPart = new ProductionRecord
                {
                    TimeStamp = DateTime.Now,
                    CycleTimeMs = cycleTime
                };

                db.productionHistory.Add(newPart);
                db.SaveChanges();
            }
        }
    }
}
