using System;
using System.Collections.Generic;
using System.Text;

namespace StampingStationSim
{
    /// <summary>
    /// Represents the pneumatic valves on a stamping station
    /// </summary>
    internal class Outputs
    {
        public bool activeLight;

        public bool extendStamp;
        public bool retractStamp;

        public bool extendClamp;
        public bool retractClamp;

        /// <summary>
        /// Firewall which prevents conflicting signals reaching the cylinders
        /// </summary>
        /// <remarks>
        /// If operator in manual mode accidentally trigers both EXTEND && RETRACT at the same time,
        /// this method forces both outputs to false to prevent the pneumatics to tear themselves apart
        /// </remarks>
        public void InterlockSafety()
        {
            if (extendStamp && retractStamp)
            {
                extendStamp = false;
                retractStamp = false;
            }

            if (extendClamp && retractClamp)
            {
                extendClamp = false;
                retractClamp = false;
            }
        }

        /// <summary>
        /// Resets actuators to their default states
        /// </summary>
        /// <remarks>
        /// Used heavily during <see cref="State.Fault"/> recovery
        /// </remarks>
        public void ResetAll()
        {
            activeLight = false;
            extendStamp = false;
            retractStamp = false;
            extendClamp = false;
            retractClamp = false;
        }
    }
}
