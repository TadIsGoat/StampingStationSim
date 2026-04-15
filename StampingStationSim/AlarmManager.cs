using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace StampingStationSim
{
    /// <summary>
    /// Manages the alarm database table and the local alarm list with 5 most recent alarms.
    /// </summary>
    internal class AlarmManager
    {
        private List<string> alarms = new List<string>();

        /// <summary>
        /// Initializes a new instance of the AlarmManager class, ensures the database is created and loads the most recent alarm history entries.
        /// </summary>
        public AlarmManager()
        {
            using (var db = new AppDbContext())
            {
                db.Database.EnsureCreated();

                var last5Alarms = db.alarmHistory.OrderByDescending(a=>a.TimeStamp).Take(5).ToList();

                foreach (var alarm in last5Alarms) 
                {
                    alarms.Add($"[{alarm.TimeStamp.ToString("HH:mm:ss")}] {alarm.Message}");
                }

            }
        }

        /// <summary>
        /// Adds a new alarm to the local list and also adds it to database.
        /// </summary>
        /// <param name="msg">The alarm message, usually the reason for the alarm.</param>
        public void AddAlarm(string msg)
        {
            alarms.Insert(0, $"({DateTime.Now.ToString("HH:mm:ss")} - {msg})");

            if (alarms.Count > 5)
            {
                alarms.RemoveAt(alarms.Count - 1);
            }

            using (var db = new AppDbContext())
            {
                var newFault = new AlarmRecord
                {
                    TimeStamp = DateTime.Now,
                    Message = msg
                };

                db.alarmHistory.Add(newFault);
                db.SaveChanges();
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
