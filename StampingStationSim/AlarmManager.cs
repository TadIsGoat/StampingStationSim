using System;
using System.Collections.Generic;
using System.Text;

namespace StampingStationSim
{
    internal class AlarmManager
    {
        private List<string> alarms = new List<string>();

        public void AddAlarm(string msg)
        {
            alarms.Insert(0, $"({DateTime.Now.ToString("HH:mm:ss")})");

            if (alarms.Count > 5)
            {
                alarms.RemoveAt(alarms.Count - 1);
            }
        }

        public List<string> GetAlarms()
        {
            return alarms;
        }

        public void ClearAlarms()
        {
            alarms.Clear();
        }
    }
}
