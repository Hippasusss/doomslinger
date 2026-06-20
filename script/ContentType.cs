using System;
using Godot;

namespace DoomSlinger;

[Flags]
public enum ContentType
{
    Violence = 1 << 0,
    Sport    = 1 << 1,
    Kids     = 1 << 2,
    Cooking  = 1 << 3,
    Porn     = 1 << 4,
}

public enum ContentCategory
{
    Harmful,
    Wholesome,
    Family,
}

public static class ContentTypeInfo
{
    public static ContentCategory GetCategory(ContentType type) => type switch
    {
        ContentType.Violence => ContentCategory.Harmful,
        ContentType.Porn     => ContentCategory.Harmful,
        ContentType.Sport    => ContentCategory.Wholesome,
        ContentType.Cooking  => ContentCategory.Wholesome,
        ContentType.Kids     => ContentCategory.Family,
        _                    => ContentCategory.Wholesome,
    };

    public static Color GetCategoryColor(ContentCategory category) => category switch
    {
        ContentCategory.Harmful   => new Color(0.9f, 0.2f, 0.2f),
        ContentCategory.Wholesome => new Color(0.2f, 0.8f, 0.3f),
        ContentCategory.Family    => new Color(0.95f, 0.85f, 0.2f),
        _                         => Colors.Gray,
    };

    public static Color GetColor(ContentType type) => GetCategoryColor(GetCategory(type));
}
