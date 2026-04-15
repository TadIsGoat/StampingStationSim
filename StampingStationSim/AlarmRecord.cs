using System;
using System.Collections.Generic;
using System.Text;

namespace StampingStationSim
{
    /// <summary>
    /// Represents a record in the alarm database
    /// </summary>
    internal class AlarmRecord
    {
        public int Id {  get; set; }
        public DateTime TimeStamp {  get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
