using Godot;
using System;
using System.Collections.Generic;

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
        { WarningType.Info, ">: Warning: " },
        { WarningType.Warning, "> Error: " },
        { WarningType.Error, ">:" }
    };

    public void AddNotification(string notification, WarningType warningType = WarningType.Info)
    {
        string prepend = WarningTypeNames[warningType];
        displayText.Text += prepend + notification;
    }
}
