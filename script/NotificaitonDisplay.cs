using System.Collections.Generic;
using Godot;

namespace DoomSlinger;

public partial class NotificaitonDisplay : Control, IDisplay
{
    [Export] private RichTextLabel displayText;
    private Human currentHuman;
    private const int maxNotifications = 10;
    private readonly Dictionary<Human, Queue<string>> notificationMap = [];

    public bool Enabled { get; set; } = true;

    public enum WarningType
    {
        Error,
        Info,
        Warning
    }

    private static readonly Dictionary<WarningType, string> WarningTypeNames = new ()
    {
        { WarningType.Info, ">: " },
        { WarningType.Warning, "> !: " },
        { WarningType.Error, "> X: " }
    };


    public void AddNotificationToHuman(Human human, string notification, WarningType warningType = WarningType.Info)
    {
        if (!notificationMap.TryGetValue(human, out var queue))
            notificationMap[human] = queue = new();

        if (queue.Count >= maxNotifications)
            queue.Dequeue();

        string prepend = WarningTypeNames[warningType];
        string final = prepend + notification +"\n";
        queue.Enqueue(final);
        if(human == currentHuman)
        {
            displayText.AppendText(final);
        }
    }

    private void RedrawNotifications(Human human)
    {
        displayText.Text = "";
        if (!notificationMap.TryGetValue(human, out var queue)) return;
        foreach(string line in queue)
        {
            displayText.AppendText(line);
        }
    }

    public void ToggleOnOff(bool onOff)
    {
        Enabled = onOff;
        if(!onOff)
        {
            displayText.Text = "none";
        }
        else
        {
            RedrawNotifications(currentHuman);
        }
    }

    public void UpdateDisplay(Human human)
    {
        if(currentHuman != human) 
        {
            currentHuman = human;
            RedrawNotifications(human);
        }
    }

}
