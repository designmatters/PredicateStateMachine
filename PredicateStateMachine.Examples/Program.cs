Console.Clear();
Console.WriteLine("Select an example to run:");
Console.WriteLine("1. Sensor");
Console.WriteLine("2. Lock");
Console.WriteLine("3. Emergency Traffic Light");
Console.WriteLine("4. Moderated Chat");
Console.WriteLine("0. Exit");
Console.Write("\nEnter choice: ");

var key = Console.ReadKey(intercept: true).KeyChar;
Console.WriteLine("\n");

switch (key)
{
    case '1':
        Console.WriteLine("Running Sensor Example");
        Sensor.Example.Run();
        break;
    case '2':
        Console.WriteLine("Running Lock Example");
        Lock.Example.Run();
        break;
    case '3':
        Console.WriteLine("Running Emergency Traffic Light Example");
        await EmergencyTrafficLight.Example.Run();
        break;
    case '4':
        Console.WriteLine("Running Moderated Chat Example");
        ModeratedChat.Example.Run();
        break;
}

Console.ReadKey();