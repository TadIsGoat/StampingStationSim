using System;
using System.Collections.Generic;
using System.Text;

namespace StampingStationSim
{
    internal class Outputs
    {
        public bool activeLight;

        public bool extendStamp;
        public bool retractStamp;

        public bool extendClamp;
        public bool retractClamp;

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
