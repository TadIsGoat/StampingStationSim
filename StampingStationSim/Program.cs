using StampingStationSim;
using System.Diagnostics;

Inputs inputs = new Inputs();
Outputs outputs = new Outputs();
Controller controller = new Controller();
PneumaticCylinder stamperSimulator = new PneumaticCylinder();
PneumaticCylinder clamperSimulator = new PneumaticCylinder();
AlarmManager alarmManager = new AlarmManager();

Stopwatch timer = new Stopwatch();
int loopTime = 100; //ms //in real world probably less but we dont care here
int cycleCounter = 0;

Console.CursorVisible = false; // Hides the blinking cursor for a cleaner look

while (true)
{
    timer.Restart();

    inputs.ReadInputs(stamperSimulator.isExtended, stamperSimulator.isRetracted, clamperSimulator.isExtended, clamperSimulator.isRetracted);
    controller.Update(inputs, outputs, alarmManager);
    outputs.InterlockSafety();
    stamperSimulator.Update(outputs.extendStamp, outputs.retractStamp);
    clamperSimulator.Update(outputs.extendClamp, outputs.retractClamp);

    PrintUI();

    timer.Stop();
    int remainingTime = loopTime - (int)timer.ElapsedMilliseconds;
    if (remainingTime > 0)
    {
        Thread.Sleep(remainingTime);
    }
    cycleCounter++;
}

/// <summary>
/// Completely vibecoded UI cuz idc about visuals here
/// </summary>
void PrintUI ()
{
    Console.SetCursorPosition(0, 0);

    Console.WriteLine("========================================");
    Console.WriteLine("       STAMPING STATION SIMULATOR       ");
    Console.WriteLine("========================================");

    // Status Header
    string mode = inputs.manualModeSwitch ? "MANUAL" : "AUTO  ";
    Console.WriteLine($" MODE:  {mode}                          ");
    Console.WriteLine($" STATE: {controller.currentState,-20}    "); // -20 pads it with spaces so text doesn't ghost
    Console.WriteLine($" LIGHT: {(outputs.activeLight ? "ON " : "OFF")}                            ");
    Console.WriteLine("----------------------------------------");

    // Sensor & Cylinder Status
    Console.WriteLine(" [ PART SENSOR ]: " + (inputs.partPresentSensor ? "DETECTED" : "EMPTY   "));
    Console.WriteLine();
    Console.WriteLine($" [ CLAMP ] Ext Valve: {(outputs.extendClamp ? "ON " : "OFF")} | Ret Valve: {(outputs.retractClamp ? "ON " : "OFF")}");
    Console.WriteLine($"           Sensor Ext: {(clamperSimulator.isExtended ? "[X]" : "[ ]")} | Sensor Ret: {(clamperSimulator.isRetracted ? "[X]" : "[ ]")}");
    Console.WriteLine();
    Console.WriteLine($" [ STAMP ] Ext Valve: {(outputs.extendStamp ? "ON " : "OFF")} | Ret Valve: {(outputs.retractStamp ? "ON " : "OFF")}");
    Console.WriteLine($"           Sensor Ext: {(stamperSimulator.isExtended ? "[X]" : "[ ]")} | Sensor Ret: {(stamperSimulator.isRetracted ? "[X]" : "[ ]")}");

    Console.WriteLine("----------------------------------------");

    // Controls Menu
    Console.WriteLine(" CONTROLS:");
    Console.WriteLine(" [P] Toggle Part Present");
    Console.WriteLine(" [S] Start Cycle (Auto Mode)");
    Console.WriteLine(" [R] Reset/Home Machine");
    Console.WriteLine(" [M] Toggle Auto/Manual Mode");
    Console.WriteLine();
    if (inputs.manualModeSwitch)
    {
        Console.WriteLine(" MANUAL JOG CONTROLS (Hold Key):       ");
        Console.WriteLine(" [1] Clamp Down   [2] Clamp Up         ");
        Console.WriteLine(" [3] Stamp Down   [4] Stamp Up         ");
    }
    else
    {
        // Print blank lines to overwrite manual controls when switching back to auto
        Console.WriteLine("                                       ");
        Console.WriteLine("                                       ");
        Console.WriteLine("                                       ");
    }
}