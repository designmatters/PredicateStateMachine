using PredicateStateMachine;

namespace ModeratedChat;

public static class Example
{
    public static PredicateStateMachine<ModerationEvent> CreateStateMachine()
    {
        var machine = new PredicateStateMachine<ModerationEvent>();

        var normal = new ModerationState(machine, "Normal");
        var warned = new ModerationState(machine, "Warned");
        var muted = new ModerationState(machine, "Muted");
        var banned = new ModerationState(machine, "Banned");
        var underReview = new ModerationState(machine, "UnderReview");
        var approved = new ModerationState(machine, "Approved");
        var rejected = new ModerationState(machine, "Rejected");

        normal.AddPath(new Trigger<ModerationEvent>(e => e is { Identifier: "ViolationDetected", Severe: false }), warned);
        normal.AddPath(new Trigger<ModerationEvent>(e => e is { Identifier: "ViolationDetected", Severe: true }), muted);
        warned.AddPath(new Trigger<ModerationEvent>(e => e.Identifier == "ViolationDetected"), muted);
        muted.AddPath(new Trigger<ModerationEvent>(e => e.Identifier == "ViolationDetected"), banned);
        muted.AddTimeout(new StateTimeoutConfiguration<ModerationEvent>(10000, new ModerationEvent("TimeoutExpired")));
        muted.AddPath(new Trigger<ModerationEvent>(e => e.Identifier == "TimeoutExpired"), normal);
        banned.AddPath(new Trigger<ModerationEvent>(e => e.Identifier == "AppealSubmitted"), underReview);
        underReview.AddPath(new Trigger<ModerationEvent>(e => e.Identifier == "ModeratorApproved"), approved);
        underReview.AddPath(new Trigger<ModerationEvent>(e => e.Identifier == "ModeratorRejected"), rejected);
        approved.AddPath(new Trigger<ModerationEvent>(e => true), normal);

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