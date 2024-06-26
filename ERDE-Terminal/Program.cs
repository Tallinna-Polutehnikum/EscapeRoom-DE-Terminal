﻿// See https://aka.ms/new-console-template for more information
using ERDE_Terminal;
using System.Management;
using System.Reflection;
using WMPLib;

Random random = new Random();

// Read config
var parser = new IniFileParser();
var config = parser.ReadIniFile("x86dist.dll");

var usbKeys = new List<string>();
foreach (var value in config["usb"].Values)
{
    usbKeys.Add(value);
}
var currentUsbKey = usbKeys[random.Next(usbKeys.Count)];

WindowsMediaPlayer player = new WindowsMediaPlayer();
player.settings.autoStart = false;
player.URL = "x64dist.dll";



int sessionId = random.Next(100000, 1000000);

Console.ForegroundColor = ConsoleColor.Red;
Console.WriteLine(@"

███    ██  ██████       ██████  ██████  ███    ██ ███    ██ ███████  ██████ ████████ ██  ██████  ███    ██ 
████   ██ ██    ██     ██      ██    ██ ████   ██ ████   ██ ██      ██         ██    ██ ██    ██ ████   ██ 
██ ██  ██ ██    ██     ██      ██    ██ ██ ██  ██ ██ ██  ██ █████   ██         ██    ██ ██    ██ ██ ██  ██ 
██  ██ ██ ██    ██     ██      ██    ██ ██  ██ ██ ██  ██ ██ ██      ██         ██    ██ ██    ██ ██  ██ ██ 
██   ████  ██████       ██████  ██████  ██   ████ ██   ████ ███████  ██████    ██    ██  ██████  ██   ████ 

");


using (HttpClient client = new HttpClient())
{
    bool network = true;
    while(network)
    try
    {
        HttpResponseMessage response = await client.GetAsync(config["game"]["backend"] + String.Format("?rid={0}&sid={1}", config["game"]["roomId"], sessionId));
        if (response.IsSuccessStatusCode)
        {
            network = false;
        }
        Thread.Sleep(2000);
    }
    catch (HttpRequestException e)
    {
        //Console.WriteLine($"Request exception: {e.Message}");
    }
}



Thread.Sleep(2125);
Console.Clear();
Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine(@"

 ██████  ██████  ███    ██ ███    ██ ███████  ██████ ████████ ███████ ██████  
██      ██    ██ ████   ██ ████   ██ ██      ██         ██    ██      ██   ██ 
██      ██    ██ ██ ██  ██ ██ ██  ██ █████   ██         ██    █████   ██   ██ 
██      ██    ██ ██  ██ ██ ██  ██ ██ ██      ██         ██    ██      ██   ██ 
 ██████  ██████  ██   ████ ██   ████ ███████  ██████    ██    ███████ ██████  

");
Thread.Sleep(1125);
Console.WriteLine("Connection established: Uplink successful.");
Thread.Sleep(600);
Console.WriteLine("Session ID for two-step authentication: "+sessionId);
Thread.Sleep(800);
Console.WriteLine("Payload downloaded");
Thread.Sleep(800);
Console.ForegroundColor = ConsoleColor.Yellow;
Console.WriteLine("Provide USB decryption key...");

using var watcher = new ManagementEventWatcher();
var query = new WqlEventQuery("SELECT * FROM __InstanceCreationEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_USBHub'");
watcher.EventArrived += DeviceInsertedEvent;
watcher.Query = query;
watcher.Start();

bool running = true;
bool encrypted = true;
bool remoteAccess = false;
while(running && encrypted)
{
    if (Console.ReadLine() == "exitprogram")
        running = false;
}

while(running && !encrypted)
{
    Console.ForegroundColor = ConsoleColor.White;
    Console.Write("Enter command: ");
    string command = Console.ReadLine();
    //Console.WriteLine(command);

    if (command.StartsWith("exitprogram"))
        running = false;

    if (command.StartsWith("replay"))
    {
        player.controls.currentPosition = 0;
        player.controls.play();
    }
    else if (command.StartsWith("net_access"))
    {
        if (command.Contains("/target:SatUplink") && command.Contains("/method:quantum_decrypt") && command.Contains("/p:22"))
        {
            remoteAccess = true;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Access granted");
        }

        Console.ForegroundColor = ConsoleColor.Yellow;
        bool firstError = false;
        if (!command.Contains("/target:"))
        {
            Console.WriteLine("Missing target - check your syntax /target:<targetName>");
            firstError = true;
        }
        if (!command.Contains("/method:"))
        {
            Console.WriteLine("Missing method - check your syntax /method:<methodName>");
            firstError = true;

        }
        if (!command.Contains("/p:"))
        {
            Console.WriteLine("Missing port - check your syntax /p:<number>");
            firstError = true;
        }

        if (!firstError) { 
            if (!command.Contains("/target:SatUplink")) { 
                Console.WriteLine("Server not responding. Is your target correct?");
            }
            else if (!command.Contains("/method:quantum_decrypt"))
            {
                Console.WriteLine("Method ineffective. Are you using correct method?");
            }
            else if (!command.Contains("/p:22"))
            {
                Console.WriteLine("Server not responding. Is the port number correct?");
            }
        }
    }
    else if (command.StartsWith("exploit_deploy"))
    {
        if (!remoteAccess)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Server not responding. Did you managed to connect to the remote server using \"net_access\" command?");
        }
        else if (command.Contains("/package:ZeroDay_StuxRev") && command.Contains("/target:C2_Server"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Injection successful. Searching for secret code...");
            Thread.Sleep(1500);
            Console.WriteLine("Found half of the code: "+ config["game"]["code"]);
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            if (!command.Contains("/package:"))
            {
                Console.WriteLine("Missing package - check your syntax /package:<packageName>");
            }
            else if (!command.Contains("/package:ZeroDay_StuxRev"))
            {
                Console.WriteLine("Cannot find package, check your file name");
            }

            if (!command.Contains("/target:"))
            {
                Console.WriteLine("Missing target - check your syntax /target:<targetName>");
            }
            else if (!command.Contains("/target:C2_Server"))
            {
                Console.WriteLine("Failed to connect, invalid target");
            }
        }
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Unknown command");
    }
}

WiFiManager.DisconnectAllWiFi();
WiFiManager.ClearSavedWiFiProfiles();

// To keep the application running, preventing it from exiting immediately due to the top-level statements finishing execution.
await Task.Run(() => Console.ReadLine());
watcher.Stop();

void DeviceInsertedEvent(object sender, EventArrivedEventArgs e)
{
    ManagementBaseObject instance = (ManagementBaseObject)e.NewEvent["TargetInstance"];

    //DeviceID: USB\VID_0951&PID_1666\60A44C4250F7BE815B416C96
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine(instance.GetPropertyValue("DeviceID"));
    if (instance.GetPropertyValue("DeviceID").ToString().Trim() == currentUsbKey)
    {
        watcher.Stop();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Matching signature");
        Console.Write("Decrypting.");
        int numberOfDots = 20;
        int sleepTime = 900;
        for (int i = 0; i < numberOfDots; i++)
        {
            Thread.Sleep(sleepTime);
            if (sleepTime > 75)
                sleepTime -= 55;
            Console.Write(".");
        }
        Thread.Sleep(1500);
        Console.WriteLine("\nDone!");
        Thread.Sleep(1125);
        Console.WriteLine("Starting playback (use command 'replay' to play again)");
        Console.WriteLine("Press Enter to continue...");
        player.controls.play();
        //Thread.Sleep(2125);
        //Console.WriteLine("Press Enter to continue");
        encrypted = false;
    } else
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Invalid signature! Possibly wrong decryption key");
        Console.ForegroundColor = ConsoleColor.Green;
        Thread.Sleep(1125);
    }

    /*foreach (var property in instance.Properties)
    {
        Console.WriteLine($"{property.Name}: {property.Value}");
    }/*

    // Attempt to retrieve the serial number from the device instance
    try
    {
        var deviceID = instance["DeviceID"].ToString();
        var searcher = new ManagementObjectSearcher($"SELECT * FROM Win32_PnPEntity WHERE DeviceID = '{deviceID}'");

        foreach (var device in searcher.Get())
        {
            var serialNumber = device["SerialNumber"];
            if (serialNumber != null)
            {
                Console.WriteLine($"Serial Number: {serialNumber}");
            }
            else
            {
                Console.WriteLine("Serial Number not found.");
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while trying to retrieve the serial number: {ex.Message}");
        Console.WriteLine("Invalid signature! Possibly wrong decryption key");
    }*/
}
