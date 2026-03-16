using SkiaSharp;

namespace StatiqHelpers.ImageHelpers;

public class BrandLayoutDetails
{
    public BrandLayoutDetails(SKRect pillRect, float fontSize, float margin, float textWidth, string text, SKTypeface typeface)
    {
        PillRect = pillRect;
        FontSize = fontSize;
        Margin = margin;
        TextWidth = textWidth;
        Text = text;
        Typeface = typeface;
    }

    public SKRect PillRect { get; }

    public float FontSize { get; }

    public float Margin { get; }

    public float TextWidth { get; }

    public string Text { get; }

    public SKTypeface Typeface { get; }
}
