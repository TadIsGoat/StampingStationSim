using System;
using System.Collections.Generic;
using System.Text;

namespace StampingStationSim
{
    /// <summary>
    /// Represents a record in the production database
    /// </summary>
    /// <remarks>
    /// Only these "representers of records" use different property naming convention (with uppercase first letter). 
    /// Might be confusing and I should fix fit, but will I? probably not.
    /// </remarks>
    internal class ProductionRecord
    {
        public int Id {  get; set; }
        public DateTime TimeStamp { get; set; }
        public int CycleTimeMs { get; set; }
    }
}
