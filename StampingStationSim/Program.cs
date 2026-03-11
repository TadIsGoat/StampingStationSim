using Microsoft.VisualBasic.FileIO;
using StampingStationSim;
using System.Diagnostics;
using System.Runtime.CompilerServices;

Inputs inputs = new Inputs();
Outputs outputs = new Outputs();
Controller controller = new Controller();
PneumaticCylinder stamperSimulator = new PneumaticCylinder();
PneumaticCylinder clamperSimulator = new PneumaticCylinder();

Stopwatch timer = new Stopwatch();
int loopTime = 100;//ms //in real world probably less but we dont care here
int cycleCounter = 0;

while (true)
{
    timer.Restart();

    inputs.ReadInputs(stamperSimulator.isExtended, stamperSimulator.isRetracted, clamperSimulator.isExtended, clamperSimulator.isRetracted);
    controller.Update(inputs, outputs);
    outputs.InterlockSafety();
    stamperSimulator.Update(outputs.extendStamp, outputs.retractStamp);
    clamperSimulator.Update(outputs.extendClamp, outputs.retractClamp);

    timer.Stop();
    int remainingTime = loopTime - (int)timer.ElapsedMilliseconds;
    if (remainingTime > 0)
    {
        Thread.Sleep(remainingTime);
    }
    cycleCounter++;
}