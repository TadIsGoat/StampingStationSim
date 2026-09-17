using StampingStationSim;
using System.Diagnostics;
using System.Text;

Inputs inputs = new Inputs();
Outputs outputs = new Outputs();
Controller controller = new Controller();
PneumaticCylinder stamperSimulator = new PneumaticCylinder();
PneumaticCylinder clamperSimulator = new PneumaticCylinder();
AlarmManager alarmManager = new AlarmManager();
ProductionManager productionManager = new ProductionManager();

//helpers that "push the buttons instead of the operator"
bool webStartRequest = false;
bool webResetRequest = false;
bool webStopRequest = false;
bool webClearAlarmsRequest = false;

#region THREAD C (API stuff)
var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders(); //silents all the logs that the API is sending to the console
var app = builder.Build();

//emergency stop button
app.MapPost("/api/control/estop", () =>
{
    webStopRequest = true;
    return Results.Ok(new { Message = "Remote emergency stop command received" });
});

//reset button
app.MapPost("/api/control/reset", () =>
{
    webResetRequest = true;
    return Results.Ok(new { Message = "Remote reset command received" });
});

//start button
app.MapPost("/api/control/start", () =>
{
    webStartRequest = true;
    return Results.Ok(new { Message = "Remote start command received" });
});

//clear alarms button
app.MapPost("/api/control/clearalarms", () =>
{
    webClearAlarmsRequest = true;
    return Results.Ok(new { Message = "Clear alarms command received" });
});


//parts done count
app.MapGet("/api/production", () =>
{
    using (var db = new AppDbContext())
    {
        int totalParts = db.productionHistory.Count(); //counts the good part logs
        var newestRecord = db.productionHistory.OrderByDescending(p => p.TimeStamp).FirstOrDefault();
        int lastTime = newestRecord != null ? newestRecord.CycleTimeMs : 0;

        return Results.Ok(new
        {
            TotalGoodParts = totalParts,
            LastTime = lastTime,
            Message = "Data pulled live from stamping station"
        });
    }
});

//alarm history
app.MapGet("/api/alarms", () =>
{
    using (var db = new AppDbContext())
    {
        var recentAlarms = alarmManager.GetAlarms();
        return Results.Ok(recentAlarms);
    }
});

//current state
app.MapGet("/api/status", () =>
{
    using (var db = new AppDbContext())
    {
        return Results.Ok(new
        {
            State = controller.currentState.ToString(),
            Mode = inputs.manualModeSwitch ? "MANUAL" : "AUTO",
            PartPresent = inputs.partPresentSensor,
            WarningLight = outputs.activeLight
        });
    }
});

//HMI panel
app.MapGet("/", () =>
{
    string htmlContent = File.ReadAllText("dashboard.html");
    return Results.Content(htmlContent, "text/html");
});

app.RunAsync("http://localhost:5000"); //run async without awaiting anything to avoid holding back the machine

#endregion

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
    if (webStopRequest || webStartRequest || webResetRequest || webClearAlarmsRequest) //check what the web requested and do it instead of the operator
    {
        inputs.emergencyStopButton = webStopRequest;
        webStopRequest = false;
        inputs.startButton = webStartRequest;
        webStartRequest = false;
        inputs.resetButton = webResetRequest;
        webResetRequest = false;
        inputs.clearAlarmsButton = webClearAlarmsRequest;
        webClearAlarmsRequest = false;
    }

    if (inputs.clearAlarmsButton) //this really doesn't belong into the controller, not sure if it belongs here, but didn't know where else to put it; also the button is only  wired up in the webapp and not in the console dashboard
    {
        alarmManager.ClearAlarms();
    }

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
        ui.AppendLine("   [E] Emergency Stop                             ");
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