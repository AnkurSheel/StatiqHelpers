using SkiaSharp;

namespace StatiqHelpers.ImageHelpers;

public class ImageDrawingService : IImageDrawingService
{
    private readonly IImageProcessingService _imageProcessingService;

    private readonly SKColor _colorStart = SKColor.Parse("#00d2ff");
    private readonly SKColor _colorEnd = SKColor.Parse("#9200ff");
    private readonly SKColor _accentNeon = SKColor.Parse("#ff007f");

    public ImageDrawingService(IImageProcessingService imageProcessingService)
    {
        _imageProcessingService = imageProcessingService;
    }

    public void DrawBackgroundImage(
        string coverImagePath,
        SKCanvas canvas
    )
    {
        var width = canvas.DeviceClipBounds.Width;
        var height = canvas.DeviceClipBounds.Height;
        using var bgBitmap = SKBitmap.Decode(coverImagePath);
        if (bgBitmap != null)
        {
            using var paint = new SKPaint { FilterQuality = SKFilterQuality.High, IsAntialias = true };
            using var thumbnail = _imageProcessingService.ResizeBitmap(width, height, bgBitmap);
            canvas.DrawBitmap(thumbnail, 0, 0, paint);
        }
    }

    public void DrawFallbackGradient(SKCanvas canvas)
    {
        var width = canvas.DeviceClipBounds.Width;
        var height = canvas.DeviceClipBounds.Height;
        using var paint = new SKPaint
        {
            IsAntialias = true,
            FilterQuality = SKFilterQuality.High,
            Shader = SKShader.CreateLinearGradient(
                new SKPoint(0, 0),
                new SKPoint(width, height),
                [_colorStart, _colorEnd],
                null,
                SKShaderTileMode.Clamp)
        };
        canvas.DrawRect(0, 0, width, height, paint);
    }

    public void DrawMainTextSection(
        SKCanvas canvas,
        TextLayoutDetails layout
    )
    {
        DrawFrostedGlass(canvas, layout);

        DrawCenterText(canvas, layout);
    }

    private void DrawFrostedGlass(SKCanvas canvas, TextLayoutDetails layout)
    {
        canvas.Save();

        using var clipPath = new SKPath();
        clipPath.AddRoundRect(layout.GlassRect, 20, 20);
        canvas.ClipPath(clipPath);

        ApplyBackgroundBlur(canvas);
        ApplyGlassTint(canvas, clipPath);
        DrawGlassBorder(canvas, clipPath);

        canvas.Restore();
    }

    private void ApplyBackgroundBlur(SKCanvas canvas)
    {
        using var blurPaint = new SKPaint
        {
            ImageFilter = SKImageFilter.CreateBlur(15, 15),
            BlendMode = SKBlendMode.SrcOver,
            IsAntialias = true,
            FilterQuality = SKFilterQuality.High
        };
        canvas.SaveLayer(blurPaint);
        canvas.Restore();
    }

    private void ApplyGlassTint(SKCanvas canvas, SKPath path)
    {
        using var tintPaint = new SKPaint
        {
            Color = SKColors.Black.WithAlpha(140),
            IsAntialias = true,
            Style = SKPaintStyle.Fill,
            FilterQuality = SKFilterQuality.High
        };
        canvas.DrawPath(path, tintPaint);
    }

    private void DrawGlassBorder(SKCanvas canvas, SKPath path)
    {
        using var borderPaint = new SKPaint
        {
            Color = SKColors.White.WithAlpha(50),
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 2.0f,
            IsAntialias = true
        };
        canvas.DrawPath(path, borderPaint);
    }
    
    private void DrawCenterText(SKCanvas canvas, TextLayoutDetails layout)
    {
        var width = canvas.DeviceClipBounds.Width;
        var y = (canvas.DeviceClipBounds.Height - layout.ContentHeight) / 2f + layout.FontSize;

        using var textPaint = CreateMainTextPaint(layout.Typeface, layout.FontSize);
        using var shadowPaint = CreateTextShadowPaint(layout.Typeface, layout.FontSize);

        foreach (var line in layout.Lines)
        {
            canvas.DrawText(line, width / 2f, y, shadowPaint);
            canvas.DrawText(line, width / 2f, y, textPaint);
            y += layout.LineHeight;
        }
    }


    private SKPaint CreateMainTextPaint(SKTypeface typeface, float fontSize)
        => new()
        {
            Typeface = typeface,
            TextSize = fontSize,
            Color = SKColors.White,
            IsAntialias = true,
            SubpixelText = true,
            LcdRenderText = true,
            TextAlign = SKTextAlign.Center,
            FakeBoldText = true,
            FilterQuality = SKFilterQuality.High
        };

    private SKPaint CreateTextShadowPaint(SKTypeface typeface, float fontSize)
        => new()
        {
            Typeface = typeface,
            TextSize = fontSize,
            Color = SKColors.Black.WithAlpha(160),
            IsAntialias = true,
            SubpixelText = true,
            TextAlign = SKTextAlign.Center,
            MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 4)
        };

    public void DrawMetadataBadge(
        SKCanvas canvas,
        BadgeLayoutDetails layout
    )
    {
        using var paint = new SKPaint
        {
            Typeface = layout.Typeface,
            TextSize = layout.FontSize,
            Color = _colorStart,
            IsAntialias = true,
            SubpixelText = true,
            FakeBoldText = true,
            TextAlign = SKTextAlign.Center
        };

        using var bgPaint = new SKPaint { Color = SKColor.Parse("#1A1A1A"), IsAntialias = true };
        canvas.DrawRoundRect(layout.BadgeRect, 10, 10, bgPaint);

        using var borderPaint = new SKPaint
        {
            Color = _colorStart.WithAlpha(100),
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 2,
            IsAntialias = true
        };
        canvas.DrawRoundRect(layout.BadgeRect, 10, 10, borderPaint);

        canvas.DrawText(layout.Text, layout.BadgeRect.MidX, layout.BadgeRect.MidY + layout.FontSize / 3f, paint);
    }

    public void AddFloatingBrand(
        SKCanvas canvas,
        BrandLayoutDetails layout
    )
    {
        var width = canvas.DeviceClipBounds.Width;
        var height = canvas.DeviceClipBounds.Height;

        using var paint = new SKPaint
        {
            Typeface = layout.Typeface,
            TextSize = layout.FontSize,
            Color = SKColors.White,
            IsAntialias = true,
            SubpixelText = true,
            TextAlign = SKTextAlign.Right
        };

        DrawBrandPill(canvas, layout.PillRect);

        canvas.DrawCircle(
            width - layout.Margin - layout.TextWidth - 20,
            height - layout.Margin - layout.FontSize / 3.5f,
            5,
            new SKPaint { Color = _accentNeon, IsAntialias = true });
        canvas.DrawText(layout.Text, width - layout.Margin, height - layout.Margin, paint);
    }

    private void DrawBrandPill(SKCanvas canvas, SKRect pillRect)
    {
        using var pillPaint = new SKPaint { Color = SKColors.Black.WithAlpha(180), IsAntialias = true };
        canvas.DrawRoundRect(pillRect, 12, 12, pillPaint);
    }
}