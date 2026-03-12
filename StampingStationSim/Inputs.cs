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

        public bool manualModeSwitch { get; private set; }

        public bool clampExtendButton { get; private set; }
        public bool clampRetractButton { get; private set; }
        public bool stampExtendButton { get; private set; }
        public bool stampRetractButton { get; private set; }

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
                else if (key == ConsoleKey.M)
                {
                    manualModeSwitch = !manualModeSwitch;
                }

                if (manualModeSwitch)
                {
                    if (key == ConsoleKey.NumPad1)
                    {
                        clampExtendButton = true;
                    }
                    if (key == ConsoleKey.NumPad2)
                    {
                        clampRetractButton = true;
                    }
                    if (key == ConsoleKey.NumPad3)
                    {
                        stampExtendButton = true;
                    }
                    if (key == ConsoleKey.NumPad4)
                    {
                        stampRetractButton = true;
                    }
                }
            }
        }

        public void ResetAll()
        {
            startButton = false;
            resetButton = false;

            //partPresentSensor not here cuy its a switch

            stampExtendedSensor = false;
            stampRetractedSensor = false;
            clampExtendedSensor = false;
            clampRetractedSensor = false;

            //manualModeButton is a switch too

            clampExtendButton = false;
            clampRetractButton = false;
            stampExtendButton = false;
            stampRetractButton = false;
        }
    }
}
