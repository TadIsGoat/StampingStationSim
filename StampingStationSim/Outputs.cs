using System;
using System.Collections.Generic;
using System.Text;

namespace StampingStationSim
{
    internal class Outputs
    {
        public bool activeLight;
        public bool extendValve;
        public bool retractValve;

        public void InterlockSafety()
        {
            if (extendValve && retractValve)
            {
                extendValve = false;
                retractValve = false;
            }
        }

        public void ResetAll()
        {
            activeLight = false;
            extendValve = false;
            retractValve = false;
        }
    }
}
