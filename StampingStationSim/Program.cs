using StampingStationSim;
using System.Diagnostics;
using System.Text;
using System.Timers;

Inputs inputs = new Inputs();
Outputs outputs = new Outputs();
Controller controller = new Controller();
PneumaticCylinder stamperSimulator = new PneumaticCylinder();
PneumaticCylinder clamperSimulator = new PneumaticCylinder();
AlarmManager alarmManager = new AlarmManager();
ProductionManager productionManager = new ProductionManager();

Console.CursorVisible = false; // hides the blinking cursor for a cleaner look


/// <summary>
/// The main loop controlling physics running on a separate thread to avoid being held up by other stuff to stay safe.
/// </summary>
Task physics = Task.Run(() =>
{
    Stopwatch timer = new Stopwatch();
    int loopTime = 100; //ms //in real world probably less but we dont care here

    while(true)
    {
        timer.Restart();

        //machine
        controller.Update(inputs, outputs, alarmManager, productionManager);
        outputs.InterlockSafety();

        //simulation
        stamperSimulator.Update(outputs.extendStamp, outputs.retractStamp);
        clamperSimulator.Update(outputs.extendClamp, outputs.retractClamp);

        //loop control
        timer.Stop();
        int remainingTime = loopTime - (int)timer.ElapsedMilliseconds;
        if (remainingTime > 0)
        {
            Thread.Sleep(remainingTime);
        }
    }
});

/// <summary>
/// The secondary loop running on the main thread, isn't so crucial so it's running less often.
/// </summary>
Stopwatch uiTimer = new Stopwatch();
int uiLoopTime = 250; //refreshes 4 times a sec
while (true)
{
    uiTimer.Restart();

    //input & ui print
    inputs.ReadInputs(stamperSimulator.isExtended, stamperSimulator.isRetracted, clamperSimulator.isExtended, clamperSimulator.isRetracted);
    PrintUI();

    //loop control
    uiTimer.Stop();
    int remainingTime = uiLoopTime - (int)uiTimer.ElapsedMilliseconds;
    if (remainingTime > 0)
    {
        Thread.Sleep(remainingTime);
    }
}

/// <summary>
/// Completely vibecoded UI cuz idc about visuals here
/// </summary>
void PrintUI()
{
    // 1. Build the entire screen in memory first
    StringBuilder ui = new StringBuilder();

    ui.AppendLine("==================================================");
    ui.AppendLine("            STAMPING STATION SIMULATOR            ");
    ui.AppendLine("==================================================");

    // --- DASHBOARD ---
    string mode = inputs.manualModeSwitch ? "MANUAL" : "AUTO  ";
    ui.AppendLine(" [ SYSTEM STATUS ]");
    ui.AppendLine($"   Mode:  [{mode}]           Good Parts: [ {productionManager.goodPartsCount} ]   "); // <-- Reserved for the database!
    ui.AppendLine($"   State: [{controller.currentState,-15}]  Light:      [{(outputs.activeLight ? "ON " : "OFF")}]   ");
    ui.AppendLine("--------------------------------------------------");

    // --- SENSORS & HARDWARE ---
    ui.AppendLine(" [ HARDWARE DIAGNOSTICS ]");
    ui.AppendLine($"   Part Nest: {(inputs.partPresentSensor ? "[DETECTED]" : "[ EMPTY  ]")}");
    ui.AppendLine();
    ui.AppendLine($"   CLAMP: Valve {(outputs.extendClamp ? "[ON ]" : "[OFF]")} | Ext Sensor {(clamperSimulator.isExtended ? "[X]" : "[ ]")} | Ret Sensor {(clamperSimulator.isRetracted ? "[X]" : "[ ]")}");
    ui.AppendLine($"   STAMP: Valve {(outputs.extendStamp ? "[ON ]" : "[OFF]")} | Ext Sensor {(stamperSimulator.isExtended ? "[X]" : "[ ]")} | Ret Sensor {(stamperSimulator.isRetracted ? "[X]" : "[ ]")}");
    ui.AppendLine("--------------------------------------------------");

    // --- CONTROLS ---
    ui.AppendLine(" [ OPERATOR PANEL ]");
    if (inputs.manualModeSwitch)
    {
        ui.AppendLine("   [1/2] Clamp Jog (Down/Up)   [3/4] Stamp Jog    ");
        ui.AppendLine("   [M] Switch to AUTO          [R] Reset/Home     ");
    }
    else
    {
        ui.AppendLine("   [S] Start Auto Cycle        [P] Toggle Part    ");
        ui.AppendLine("   [M] Switch to MANUAL        [R] Reset/Home     ");
    }
    ui.AppendLine("==================================================");

    // --- ALARM HISTORIAN ---
    ui.AppendLine(" [ ALARM HISTORIAN ]");
    var recentAlarms = alarmManager.GetAlarms();
    for (int i = 0; i < 5; i++)
    {
        if (i < recentAlarms.Count)
        {
            // Pad right to exactly 48 chars to overwrite ghost text
            ui.AppendLine($" {recentAlarms[i].PadRight(48)} ");
        }
        else
        {
            // Empty space if no alarms exist yet
            ui.AppendLine("                                                  ");
        }
    }

    // 2. Slap the finished string onto the screen in ONE frame!
    Console.SetCursorPosition(0, 0);
    Console.Write(ui.ToString());
}

/*

//old PrintUI()

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


*/