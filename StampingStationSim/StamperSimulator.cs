using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace StampingStationSim
{
    internal class StamperSimulator
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
        

        public void Update(Outputs outputs)
        {
            if (!outputs.extendValve)
            {
                isExtending = false;
            }
            if (!outputs.retractValve)
            {
                isRetracting = false;
            }

            if (outputs.extendValve && !isExtended && !isExtending)
            {
                travelTimer.Restart();
                travelTime = random.Next(shortestTime, longestTime);
                isRetracted = false;
                isExtending = true;
            }
            else if (outputs.retractValve && !isRetracted && !isRetracting)
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
