using SkiaSharp;

namespace StatiqHelpers.ImageHelpers;

public class BadgeLayoutDetails
{
    public BadgeLayoutDetails(SKRect badgeRect, float fontSize, string text, SKTypeface typeface)
    {
        BadgeRect = badgeRect;
        FontSize = fontSize;
        Text = text;
        Typeface = typeface;
    }

    public SKRect BadgeRect { get; }

    public float FontSize { get; }

    public string Text { get; }

    public SKTypeface Typeface { get; }
}
