using SkiaSharp;

namespace StatiqHelpers.ImageHelpers;

public interface IImageDrawingService
{
    void DrawMainTextSection(
        SKCanvas canvas,
        TextLayoutDetails layout
    );

    void DrawMetadataBadge(
        SKCanvas canvas,
        BadgeLayoutDetails layout
    );

    void AddFloatingBrand(
        SKCanvas canvas,
        BrandLayoutDetails layout
    );

    void DrawBackgroundImage(
        string coverImagePath,
        SKCanvas canvas
    );

    void DrawFallbackGradient(SKCanvas canvas);
}
