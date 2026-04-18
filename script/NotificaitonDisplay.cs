using Godot;
using System.Collections.Generic;
using Utils;

public partial class NotificaitonDisplay : Control
{
    [Export] private RichTextLabel displayText;

    public enum WarningType
    {
        Error,
        Info,
        Warning
    }

    private static readonly Dictionary<WarningType, string> WarningTypeNames = new ()
    {
        { WarningType.Info, ">: " },
        { WarningType.Warning, "> ⚠: " },
        { WarningType.Error, "> ❌: " }
    };

    public void AddNotification(string notification, WarningType warningType = WarningType.Info)
    {
        string prepend = WarningTypeNames[warningType];
        displayText.Text += prepend + notification +"\n";
    }
     
    readonly DeltaTimer testTimer = new (1);
    public override void _Process(double delta)
    {
        if(testTimer.Delta(delta))
        {
            AddNotification("hello", WarningType.Error);
        }
    }
}
