using SkiaSharp;

namespace StatiqHelpers.ImageHelpers;

public interface IImageLayoutService
{
    SocialImageLayout CalculateLayouts(
        int width,
        int height,
        string centerText,
        string secondaryText,
        string siteTitle,
        string fontPath
    );
}
