using SkiaSharp;

namespace StatiqHelpers.ImageHelpers
{
    public class ImageService : IImageService
    {
        private readonly IImageLayoutService _layoutService;
        private readonly IImageProcessingService _imageProcessingService;
        private readonly IImageDrawingService _imageDrawingService;

        public ImageService(
            IImageLayoutService layoutService,
            IImageProcessingService imageProcessingService,
            IImageDrawingService imageDrawingService
        )
        {
            _layoutService = layoutService;
            _imageProcessingService = imageProcessingService;
            _imageDrawingService = imageDrawingService;
        }

        public async Task<Stream> CreateImageDocument(
            int width,
            int height,
            string? coverImagePath,
            string siteTitle,
            string centerText,
            string secondaryText,
            string fontPath
        )
        {
            var info = new SKImageInfo(width, height, SKColorType.Rgba8888, SKAlphaType.Premul);
            using var surface = SKSurface.Create(info);
            var canvas = surface.Canvas;

            canvas.Clear(SKColors.Transparent);

            var layout = _layoutService.CalculateLayouts(width, height, centerText, secondaryText, siteTitle, fontPath);

            if (!string.IsNullOrEmpty(coverImagePath) && File.Exists(coverImagePath))
            {
                _imageDrawingService.DrawBackgroundImage(coverImagePath, canvas);
            }
            else
            {
                _imageDrawingService.DrawFallbackGradient(canvas);
            }

            _imageDrawingService.DrawMainTextSection(canvas, layout.TextLayout);
            _imageDrawingService.DrawMetadataBadge(canvas, layout.BadgeLayout);
            _imageDrawingService.AddFloatingBrand(canvas, layout.BrandLayout);

            return await _imageProcessingService.EncodeToStream(surface);
        }

        public async Task ResizeImages(
            IReadOnlyList<string> imagePaths,
            int newWidth,
            int newHeight,
            bool increaseImageSizes
        )
        {
            await _imageProcessingService.ResizeImages(imagePaths, newWidth, newHeight, increaseImageSizes);
        }
    }
}
