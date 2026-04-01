using System;
using System.Collections.Generic;
using System.Text;

namespace StampingStationSim
{
    /// <summary>
    /// Represents the buttons, switches and sensors of a stamping station
    /// </summary>
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

        /// <summary>
        /// Handles all the inputs. Sensors of all simulated valves are requiered and cannot be null.
        /// </summary>
        /// <param name="stampExtended">Sensor checking if the stamp is extended</param>
        /// <param name="stampRetracted">Sensor checking if the stamp is retracted</param>
        /// <param name="clampExtended">Sensor checking if the clamp is extended</param>
        /// <param name="clampRetracted">Sensor checking if the clamp is retracted</param>
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

        /// <summary>
        /// Resets (almost) all buttons and sensors to their default state. The switches are left out.
        /// </summary>
        public void ResetAll()
        {
            startButton = false;
            resetButton = false;

            //partPresentSensor not here cuz its a switch

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
