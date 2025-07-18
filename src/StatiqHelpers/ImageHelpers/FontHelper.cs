using SixLabors.Fonts;

namespace StatiqHelpers.ImageHelpers;

internal class FontHelper : IFontHelper
{
    public FontFamily InstallFont(string fontPath)
        => new FontCollection().Install(fontPath);
}
