using System;
using System.Collections.Generic;
using System.Text;

namespace StampingStationSim
{
    internal class Inputs
    {
        public bool startButton { get; private set; }
        public bool resetButton { get; private set; }
        public bool partPresentSensor { get; private set; }
        public bool stampExtendedSensor {  get; private set; }
        public bool stampRetractedSensor { get; private set; }
        public bool clampExtendedSensor { get; private set; }
        public bool clampRetractedSensor { get; private set; }

        public void ReadInputs(bool stampExtended, bool stampRetracted, bool clampExtended, bool clampRetracted)
        {
            ResetAll();
            stampExtendedSensor = stampExtended;
            stampRetractedSensor = stampRetracted;
            clampExtendedSensor = clampExtended;
            clampRetractedSensor = clampRetracted;
            if (Console.KeyAvailable)
            {
                ConsoleKey key = Console.ReadKey().Key;
                if (key == ConsoleKey.S)
                {
                    startButton = true;
                }
                else if (key == ConsoleKey.R)
                {
                    resetButton = true;
                }
                else if (key == ConsoleKey.P)
                {
                    partPresentSensor = !partPresentSensor;
                }
            }
        }

        public void ResetAll()
        {
            startButton = false;
            resetButton = false;
            stampExtendedSensor = false;
            stampRetractedSensor = false;
            clampExtendedSensor = false;
            clampRetractedSensor = false;
        }
    }
}
