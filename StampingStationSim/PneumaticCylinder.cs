using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace StampingStationSim
{
    /// <summary>
    /// Simulates a pneumatic cylinder that can be extended or retracted based on control commands. Also simulates failures via random movement times.
    /// </summary>
    /// <remarks>This class models the state and control logic for a pneumatic cylinder, tracking whether it
    /// is extended or retracted. It is intended for internal use within the application and is not
    /// thread-safe.</remarks>
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
        
        /// <summary>
        /// Updates the state of the cylinder based on the commands it gets.
        /// </summary>
        /// <param name="extendCommand">The command to extend. Only one of extend/retract can be true</param>
        /// <param name="retractCommand">The command to retract. Only one of extend/retract can be true</param>
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
