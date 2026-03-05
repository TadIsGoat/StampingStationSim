using Microsoft.VisualBasic.FileIO;
using StampingStationSim;
using System.Diagnostics;
using System.Runtime.CompilerServices;

Inputs inputs = new Inputs();
Outputs outputs = new Outputs();
Controller controller = new Controller();
StamperSimulator stamperSimulator= new StamperSimulator();

Stopwatch timer = new Stopwatch();
int loopTime = 100;//ms //in real world probably less but we dont care here
int cycleCounter = 0;

while (true)
{
    timer.Restart();

    inputs.ReadInputs(stamperSimulator.stampExtendedSimulator, stamperSimulator.stampRetractedSimulator);
    controller.Update(inputs, outputs);
    outputs.InterlockSafety();
    stamperSimulator.Update(outputs);

    timer.Stop();
    Thread.Sleep(loopTime - (int)timer.ElapsedMilliseconds);
    cycleCounter++;
}