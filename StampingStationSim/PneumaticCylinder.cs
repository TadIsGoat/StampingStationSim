using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace StampingStationSim
{
    internal class PneumaticCylinder
    {
        public bool isExtended { get; private set; } = false;
        public bool isRetracted { get; private set; } = true;

        private bool isExtending = false;
        private bool isRetracting = false;

        private Stopwatch travelTimer = new Stopwatch();
        int travelTime;
        const int shortestTime = 800;
        const int longestTime = 2100;
        private Random random = new Random();
        

        public void Update(bool extendCommand, bool retractCommand)
        {
            if (!extendCommand)
            {
                isExtending = false;
            }
            if (!retractCommand)
            {
                isRetracting = false;
            }

            if (extendCommand && !isExtended && !isExtending)
            {
                travelTimer.Restart();
                travelTime = random.Next(shortestTime, longestTime);
                isRetracted = false;
                isExtending = true;
            }
            else if (retractCommand && !isRetracted && !isRetracting)
            {
                travelTimer.Restart();
                travelTime = random.Next(shortestTime, longestTime);
                isExtended = false;
                isRetracting = true;
            }

            if (isExtending && travelTimer.ElapsedMilliseconds >= travelTime)
            {
                travelTimer.Stop();
                isExtending = false;
                isExtended = true;
            }
            else if (isRetracting && travelTimer.ElapsedMilliseconds >= travelTime)
            {
                travelTimer.Stop();
                isRetracting = false;
                isRetracted = true;
            }
        }
    }
}
