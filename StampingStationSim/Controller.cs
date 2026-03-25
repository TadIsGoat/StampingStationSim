using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace StampingStationSim
{
    /// <summary>
    /// All the states of the stamping machine.
    /// Note: Fault state is used as a safety layer when exiting the manual mode, the operator needs to press "reset" to get the machine to work in auto mode
    /// </summary>
    enum State
    {
        Idle,
        ClampExtending,
        ClampExtended,
        StampExtending,
        StampExtended,
        StampRetracting,
        StampRetracted,
        ClampRetracting,
        ClampRetracted,
        Fault,
    }

    /// <summary>
    /// Provides the control logic for the station based on inputs and outputs, for both manual and auto mode.
    /// Follows the A+ B+ A- B- standard
    /// </summary>
    internal class Controller
    {
        public State currentState {  get; private set; }
        private Stopwatch dwellTimer = new Stopwatch();
        private int dwellTime = 500; //ms
        private Stopwatch clampTimer = new Stopwatch();
        private int clampTime = 200; //ms
        private Stopwatch movementTimer = new Stopwatch();
        private int maxMovementTime = 2000;

        /// <summary>
        /// Updates the states of the machine based on the inputs and sensors.
        /// </summary>
        /// <param name="inputs">Operators buttons and sensors. Cannot be null</param>
        /// <param name="outputs">Pneumatics of the station. Cannot be null.</param>
        /// <param name="alarmManager">Used to log errors and warning. Cannot be null.</param>
        public void Update(Inputs inputs, Outputs outputs, AlarmManager alarmManager)
        {
            if (inputs.manualModeSwitch) //manual mode
            {
                outputs.extendClamp = inputs.clampExtendButton && !inputs.clampRetractButton;
                outputs.retractClamp = inputs.clampRetractButton && !inputs.clampExtendButton;
                outputs.extendStamp = inputs.stampExtendButton && !inputs.stampRetractButton;
                outputs.retractStamp = inputs.stampRetractButton && !inputs.stampExtendButton;

                currentState = State.Fault; //to force the operator to press reset after exiting manual mode
                alarmManager.AddAlarm("WARNING: manual mode exited");
            }
            else
            {
                bool isWorking = //if a part falls out during these we are fucked
                    (currentState == State.ClampExtending ||
                    currentState == State.ClampExtended ||
                    currentState == State.StampExtending ||
                    currentState == State.StampExtended);

                if (isWorking && inputs.partPresentSensor == false) //checks if a part hasn't fallen out at a dangerous point
                {
                    currentState = State.Fault;
                }

                switch (currentState)
                {
                    case State.Idle: //if we are here, all cylinders are retracted and we are waiting for the operator to press the start button (auto mode)
                        if (inputs.startButton && inputs.partPresentSensor)
                        {
                            outputs.activeLight = true;
                            outputs.extendClamp = true;
                            currentState = State.ClampExtending;
                            movementTimer.Restart();
                        }
                        break;
                    case State.ClampExtending:
                        if (movementTimer.ElapsedMilliseconds >= maxMovementTime)
                        {
                            currentState = State.Fault;
                            alarmManager.AddAlarm("FAULT: clamp extend timeout");
                        }
                        else if (inputs.clampExtendedSensor)
                        {
                            outputs.extendClamp = false;
                            currentState = State.ClampExtended;
                            movementTimer.Stop();
                            clampTimer.Restart();
                        }
                        break;
                    case State.ClampExtended:
                        if (clampTimer.ElapsedMilliseconds >= clampTime)
                        {
                            outputs.extendStamp = true;
                            currentState = State.StampExtending;
                            movementTimer.Restart();
                        }
                        break;
                    case State.StampExtending:
                        if (movementTimer.ElapsedMilliseconds >= maxMovementTime)
                        {
                            currentState = State.Fault;
                            alarmManager.AddAlarm("FAULT: stamp extend timeout");
                        }
                        else if (inputs.stampExtendedSensor)
                        {
                            outputs.extendStamp = false;
                            currentState = State.StampExtended;
                            movementTimer.Stop();
                            dwellTimer.Restart();
                        }
                        break;
                    case State.StampExtended:
                        if (dwellTimer.ElapsedMilliseconds >= dwellTime)
                        {
                            outputs.retractStamp = true;
                            currentState = State.StampRetracting;
                            movementTimer.Restart();
                        }
                        break;
                    case State.StampRetracting:
                        if (movementTimer.ElapsedMilliseconds >= maxMovementTime)
                        {
                            currentState = State.Fault;
                            alarmManager.AddAlarm("FAULT: stamp retract timeout");
                        }
                        else if (inputs.stampRetractedSensor)
                        {
                            outputs.retractStamp = false;
                            currentState = State.StampRetracted;
                            movementTimer.Stop();
                            clampTimer.Restart();
                        }
                        break;
                    case State.StampRetracted:
                        if (clampTimer.ElapsedMilliseconds >= clampTime)
                        {
                            outputs.retractClamp = true;
                            currentState = State.ClampRetracting;
                            movementTimer.Restart();
                        }
                        break;
                    case State.ClampRetracting:
                        if (movementTimer.ElapsedMilliseconds >= maxMovementTime)
                        {
                            currentState = State.Fault;
                            alarmManager.AddAlarm("FAULT: clamp retract timeout");
                        }
                        else if (inputs.clampRetractedSensor)
                        {
                            outputs.retractClamp = false;
                            currentState = State.ClampRetracted;
                            movementTimer.Stop();
                        }
                        break;
                    case State.ClampRetracted:
                        outputs.activeLight = false;
                        outputs.ResetAll();
                        currentState = State.Idle;
                        break;
                    case State.Fault: //if something stops working properly OR the operator wants to exit the manual mode we get here. Stops all movement, waits for the reset button.
                        outputs.extendStamp = false;
                        outputs.retractStamp = false;
                        outputs.extendClamp = false;
                        outputs.retractClamp = false;

                        if (inputs.resetButton)
                        {
                            outputs.retractStamp = true;
                            alarmManager.ClearAlarms();

                            currentState = State.StampRetracting;
                            movementTimer.Restart();
                        }
                        break;
                }
            }
        }
    }
}
