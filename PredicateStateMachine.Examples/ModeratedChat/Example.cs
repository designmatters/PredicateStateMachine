using Microsoft.Extensions.Logging;
using PredicateStateMachine;

namespace ModeratedChat;

public static class Example
{
    public static PredicateStateMachine<ModerationEvent> CreateStateMachine()
    {
        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder
                .SetMinimumLevel(LogLevel.Information)
                .AddConsole(options => { options.TimestampFormat = "HH:mm:ss "; });
        });

        var logger = loggerFactory.CreateLogger<PredicateStateMachine<ModerationEvent>>();

        
        var machine = new PredicateStateMachine<ModerationEvent>(logger);

        var normal = new ModerationState("Normal");
        var warned = new ModerationState("Warned");
        var muted = new ModerationState("Muted");
        var banned = new ModerationState("Banned");
        var underReview = new ModerationState("UnderReview");
        var approved = new ModerationState("Approved");
        var rejected = new ModerationState("Rejected");
        
        // note that this needs more edge case configuration 
        machine.AddPath(normal, new Transition<ModerationEvent>(e => e.Identifier == "ViolationDetected" && e.Severe == false), warned);
        machine.AddPath(normal, new Transition<ModerationEvent>(e => e.Identifier == "ViolationDetected" && e.Severe == true), muted);
        machine.AddPath(warned, new Transition<ModerationEvent>(e => e.Identifier == "ViolationDetected"), muted);
        machine.AddPath(muted, new Transition<ModerationEvent>(e => e.Identifier == "ViolationDetected"), banned);
        machine.AddTimeout(muted, new TimeoutConfiguration<ModerationEvent>(10000, new ModerationEvent("TimoutExpired")));
        machine.AddPath(muted, new Transition<ModerationEvent>(e => e.Identifier == "TimeoutExpired"), normal);
        machine.AddPath(banned, new Transition<ModerationEvent>(e => e.Identifier == "AppealSubmitted"), underReview);
        machine.AddPath(underReview, new Transition<ModerationEvent>(e => e.Identifier == "ModeratorApproved"), approved);
        machine.AddPath(underReview, new Transition<ModerationEvent>(e => e.Identifier == "ModeratorRejected"), rejected);
        machine.AddPath(approved, new Transition<ModerationEvent>(e => true), normal);

        machine.AddStates([normal, warned, muted, banned, underReview, approved, rejected]);
        machine.Configure(new StateMachineConfig<ModerationEvent>(normal));
        return machine;
    }

    public static void Run()
    {
        var app = new ChatApplication();

        Console.WriteLine("Chat started. Type messages as '<username>: <message>'");
        Console.WriteLine("User Commands: /appeal, exit");
        Console.WriteLine("Admin Commands: /approve <user>, /reject <user>, exit");

        while (true)
        {
            Console.Write("> ");
            var input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input)) continue;
            if (input.Equals("exit", StringComparison.OrdinalIgnoreCase)) break;

            var split = input.Split(":", 2);
            if (split.Length != 2)
            {
                Console.WriteLine("Invalid format. Use '<username>: <message>'");
                continue;
            }

            var user = split[0].Trim();
            var message = split[1].Trim();

            app.ProcessMessage(user, message);
        }
    }
}