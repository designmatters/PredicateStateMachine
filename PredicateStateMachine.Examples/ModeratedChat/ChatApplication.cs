using PredicateStateMachine;

namespace ModeratedChat;

public class ChatApplication
{
    private readonly Dictionary<string, PredicateStateMachine<ModerationEvent>> _moderations = new();

    public void ProcessMessage(string userName, string message)
    {
        if (message.StartsWith("/"))
        {
            HandleCommand(userName, message);
            return;
        }

        var moderationEvent = ModerateMessage(message);

        // Check if user is already in moderation flow
        if (_moderations.TryGetValue(userName, out var machine))
        {
            if (moderationEvent != null)
                machine.HandleEvent(moderationEvent);

            if (IsUserBannedAndNotify(userName, machine))
                return;

            AppendToChatBox(userName, message);
            return;
        }

        // If there is a first violation. Start the moderation flow.
        if (moderationEvent != null)
        {
            machine = Example.CreateStateMachine();
            machine.Start();
            _moderations[userName] = machine;
            machine.HandleEvent(moderationEvent);

            if (IsUserBannedAndNotify(userName, machine))
                return;

            AppendToChatBox(userName, message);
            return;
        }

        AppendToChatBox(userName, message);
    }

    private void HandleCommand(string userName, string command)
    {
        if (command.Equals("/appeal", StringComparison.OrdinalIgnoreCase))
        {
            if (_moderations.TryGetValue(userName, out var machine))
            {
                var state = machine.GetCurrentState().Name;
                if (state == "Banned")
                {
                    machine.HandleEvent(new ModerationEvent("AppealSubmitted"));
                    SendToUser(userName, "Appeal submitted.");
                    SendToAdmin(userName, "submitted an appeal.");
                }
                else
                {
                    SendToUser(userName, "You can only appeal if you're banned.");
                }
            }
            else
            {
                SendToUser(userName, "No violations on record.");
            }

            return;
        }

        if (userName != "admin") return;

        // TODO also check the user's actual state
        if (command.StartsWith("/approve "))
        {
            var target = command.Substring(9).Trim();
            if (_moderations.TryGetValue(target, out var targetMachine))
            {
                targetMachine.HandleEvent(new ModerationEvent("ModeratorApproved"));
                SendToUser(target, "Your appeal has been approved.");
            }
            else Console.WriteLine($"No moderation found for '{target}'");

            return;
        }

        // TODO also check the user's actual state
        if (command.StartsWith("/reject "))
        {
            var target = command.Substring(8).Trim();
            if (_moderations.TryGetValue(target, out var targetMachine))
            {
                targetMachine.HandleEvent(new ModerationEvent("ModeratorRejected"));
                SendToUser(target, "Your appeal has been rejected.");
            }
            else Console.WriteLine($"No moderation found for '{target}'");
        }
    }

    private bool IsUserBannedAndNotify(string userName, PredicateStateMachine<ModerationEvent> machine)
    {
        switch (machine.GetCurrentState().Name)
        {
            case "Banned":
                SendToUser(userName, "You are banned!");
                return true;
            case "UnderReview":
                SendToUser(userName, "Appeal under review. You're still banned.");
                return true;
            case "Rejected":
                SendToUser(userName, "You are permanently banned.");
                return true;
            default:
                return false;
        }
    }

    private void AppendToChatBox(string userName, string message)
    {
        Console.WriteLine($"→ ({userName}): {message}");
    }

    private void SendToUser(string userName, string message)
    {
        Console.WriteLine($"→ [@@{userName}]: {message}");
    }

    private void SendToAdmin(string userName, string message)
    {
        Console.WriteLine($"[ADMIN NOTICE] @@{userName} {message}");
    }

    private ModerationEvent? ModerateMessage(string message)
    {
        if (message.Contains("F")) return new ModerationEvent("ViolationDetected") { Severe = false };
        if (message.Contains("P")) return new ModerationEvent("ViolationDetected") { Severe = true };
        return null;
    }
}