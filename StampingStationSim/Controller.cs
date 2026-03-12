using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace StampingStationSim
{
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

    internal class Controller
    {
        private State currentState;
        private Stopwatch dwellTimer = new Stopwatch();
        private int dwellTime = 500; //ms
        private Stopwatch clampTimer = new Stopwatch();
        private int clampTime = 200; //ms
        private Stopwatch movementTimer = new Stopwatch();
        private int maxMovementTime = 2000;
        public void Update(Inputs inputs, Outputs outputs)
        {
            if (inputs.manualModeSwitch)
            {
                outputs.extendClamp = inputs.clampExtendButton && !inputs.clampRetractButton;
                outputs.retractClamp = inputs.clampRetractButton && !inputs.clampExtendButton;
                outputs.extendStamp = inputs.stampExtendButton && !inputs.stampRetractButton;
                outputs.retractStamp = inputs.stampRetractButton && !inputs.stampExtendButton;

                currentState = State.Fault; // to force the operator to press reset after exiting manual mode
            }
            else
            {
                switch (currentState)
                {
                    case State.Idle:
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
                    case State.Fault:
                        outputs.extendStamp = false;
                        outputs.retractStamp = false;
                        outputs.extendClamp = false;
                        outputs.retractClamp = false;

                        if (inputs.resetButton)
                        {
                            outputs.retractStamp = true;
                            currentState = State.StampRetracting;
                            movementTimer.Restart();
                        }
                        break;
                }
            }
        }
    }
}
