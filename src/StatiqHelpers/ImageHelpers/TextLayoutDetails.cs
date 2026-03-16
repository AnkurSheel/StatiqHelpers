using SkiaSharp;

namespace StatiqHelpers.ImageHelpers;

public class TextLayoutDetails
{
    public TextLayoutDetails(
        IReadOnlyList<string> lines,
        float fontSize,
        float lineHeight,
        float contentHeight,
        SKRect glassRect,
        SKTypeface typeface)
    {
        Lines = lines;
        FontSize = fontSize;
        LineHeight = lineHeight;
        ContentHeight = contentHeight;
        GlassRect = glassRect;
        Typeface = typeface;
    }

    public IReadOnlyList<string> Lines { get; }

    public float FontSize { get; }

    public float LineHeight { get; }

    public float ContentHeight { get; }

    public SKRect GlassRect { get; }

    public SKTypeface Typeface { get; }
}
