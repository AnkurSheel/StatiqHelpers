using SkiaSharp;

namespace StatiqHelpers.ImageHelpers;

public class ImageLayoutService : IImageLayoutService
{
    private readonly IFontHelper _fontHelper;

    public ImageLayoutService(IFontHelper fontHelper)
    {
        _fontHelper = fontHelper;
    }

    public SocialImageLayout CalculateLayouts(
        int width,
        int height,
        string centerText,
        string secondaryText,
        string siteTitle,
        string fontPath
    )
    {
        var textLayout = CalculateTextLayout(width, height, centerText, fontPath);
        var badgeLayout = CalculateBadgeLayout(textLayout.GlassRect, secondaryText, fontPath);
        var brandLayout = CalculateBrandLayout(width, height, siteTitle, fontPath);

        return new SocialImageLayout(textLayout, badgeLayout, brandLayout);
    }

    private TextLayoutDetails CalculateTextLayout(
        int width,
        int height,
        string text,
        string fontPath
    )

    {
        var fontSize = text.Length > 45 ? height / 13f : height / 10f;
        var xPadding = width * 0.12f;

        var typeface = _fontHelper.InstallFont(fontPath);
        using var paint = new SKPaint { Typeface = typeface, TextSize = fontSize, IsAntialias = true };
        List<string> lines = WrapText(text, width - xPadding * 2, paint);

        var lineHeight = fontSize * 1.25f;
        var contentHeight = lines.Count * lineHeight;

        var rectWidth = width * 0.90f;
        var rectHeight = contentHeight + height * 0.15f;

        var glassRect = new SKRect(
            (width - rectWidth) / 2f,
            (height - rectHeight) / 2f,
            (width - rectWidth) / 2f + rectWidth,
            (height - rectHeight) / 2f + rectHeight);

        return new TextLayoutDetails(
            lines,
            fontSize,
            lineHeight,
            contentHeight,
            glassRect,
            typeface);
    }

    private BadgeLayoutDetails CalculateBadgeLayout(SKRect glassRect, string metadata, string fontPath)
    {
        var typeface = _fontHelper.InstallFont(fontPath);
        var badgeFontSize = glassRect.Height * 0.1f;
        using var paint = new SKPaint
        {
            Typeface = typeface,
            TextSize = badgeFontSize,
            IsAntialias = true,
        };

        var textWidth = paint.MeasureText(metadata);
        var badgeRect = new SKRect(
            glassRect.MidX - textWidth / 2f - 15,
            glassRect.Top - badgeFontSize / 2f - 10,
            glassRect.MidX + textWidth / 2f + 15,
            glassRect.Top + badgeFontSize / 2f + 10);

        return new BadgeLayoutDetails(
            badgeRect,
            badgeFontSize,
            metadata,
            typeface);
    }

    private BrandLayoutDetails CalculateBrandLayout(int width, int height, string siteTitle, string fontPath)
    {
        var brandFontSize = height / 24f;
        var margin = width / 25f;

        var typeface = _fontHelper.InstallFont(fontPath);
        using var paint = new SKPaint
        {
            Typeface = typeface,
            TextSize = brandFontSize,
            IsAntialias = true,
        };

        var textWidth = paint.MeasureText(siteTitle);
        var pillRect = new SKRect(
            width - margin - textWidth - 40,
            height - margin - brandFontSize - 10,
            width - margin + 10,
            height - margin + 10);

        return new BrandLayoutDetails(
            pillRect,
            brandFontSize,
            margin,
            textWidth,
            siteTitle,
            typeface);
    }

    private List<string> WrapText(string text, float maxWidth, SKPaint paint)
    {
        var resultLines = new List<string>();
        var currentLine = string.Empty;
        foreach (var word in text.Split(' '))
        {
            var testLine = string.IsNullOrWhiteSpace(currentLine) ? word : $"{currentLine} {word}";
            if (paint.MeasureText(testLine) <= maxWidth)
            {
                currentLine = testLine;
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(currentLine))
                {
                    resultLines.Add(currentLine);
                }
                currentLine = word;
            }
        }
        if (!string.IsNullOrWhiteSpace(currentLine))
        {
            resultLines.Add(currentLine);
        }

        return resultLines;
    }
}
