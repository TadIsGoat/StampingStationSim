using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace StampingStationSim
{
    enum State
    {
        Idle,
        Extending,
        Extended,
        Retracting,
        Retracted,
        Fault,
    }

    internal class Controller
    {
        private State currentState;
        private Stopwatch dwellTimer = new Stopwatch();
        private int dwellTime = 500; //ms
        private Stopwatch movementTimer = new Stopwatch();
        private int maxMovementTime = 2000;
        public void Update()
        {
            switch (currentState)
            {
                case State.Idle:
                    if (inputs.startButton && inputs.partPresentSensor)
                    {
                        outputs.activeLight = true;
                        outputs.extendValve = true;
                        currentState = State.Extending;
                        movementTimer.Restart();
                    }
                    break;
                case State.Extending:
                    if (movementTimer.ElapsedMilliseconds >= maxMovementTime)
                    {
                        currentState = State.Fault;
                    }
                    else if (inputs.StampExtendedSensor)
                    {
                        outputs.extendValve = false;
                        currentState = State.Extended;
                        movementTimer.Stop();
                        dwellTimer.Restart();
                    }
                    break;
                case State.Extended:
                    if (dwellTimer.ElapsedMilliseconds >= dwellTime)
                    {
                        outputs.retractValve = true;
                        currentState = State.Retracting;
                        movementTimer.Restart();
                    }
                    break;
                case State.Retracting:
                    if (movementTimer.ElapsedMilliseconds >= maxMovementTime)
                    {
                        currentState = State.Fault; 
                    }
                    else if (inputs.StampRetractedSensor)
                    {
                        outputs.retractValve = false;
                        currentState = State.Retracted;
                        movementTimer.Stop();
                    }
                    break;
                case State.Retracted:
                    outputs.activeLight = false;
                    outputs.ResetAll();
                    currentState = State.Idle;
                    break;
                case State.Fault:
                    // TODO
                    break;
            }
        }
    }
}
