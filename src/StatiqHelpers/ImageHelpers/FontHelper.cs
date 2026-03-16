using SkiaSharp;

namespace StatiqHelpers.ImageHelpers;

internal class FontHelper : IFontHelper
{
    public SKTypeface InstallFont(string fontPath)
        => SKTypeface.FromFile(fontPath);
}
