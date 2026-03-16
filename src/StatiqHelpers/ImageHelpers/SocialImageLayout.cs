namespace StatiqHelpers.ImageHelpers;

public class SocialImageLayout
{
    public SocialImageLayout(
        TextLayoutDetails textLayout,
        BadgeLayoutDetails badgeLayout,
        BrandLayoutDetails brandLayout)
    {
        TextLayout = textLayout;
        BadgeLayout = badgeLayout;
        BrandLayout = brandLayout;
    }

    public TextLayoutDetails TextLayout { get; }

    public BadgeLayoutDetails BadgeLayout { get; }

    public BrandLayoutDetails BrandLayout { get; }
}
