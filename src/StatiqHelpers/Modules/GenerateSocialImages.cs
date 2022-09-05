using StatiqHelpers.CustomExtensions;
using StatiqHelpers.ImageHelpers;

namespace StatiqHelpers.Modules
{
    public class GenerateSocialImages : ParallelModule
    {
        private readonly IImageService _imageService;

        public GenerateSocialImages(IImageService imageService)
        {
            _imageService = imageService;
        }

        protected override async Task<IEnumerable<IDocument>> ExecuteInputAsync(IDocument input, IExecutionContext context)
        {
            var siteTitle = context.GetSiteTitle();
            var readingTimeData = input.GetReadingTime();
            var centerText = $"{input.GetTitle().ToUpper()}{Environment.NewLine}";

            centerText += readingTimeData.RoundedMinutes < 1
                ? $"{readingTimeData.Seconds} sec"
                : $"{readingTimeData.RoundedMinutes} min";

            var coverImagePath = input.GetCoverImagePath();

            if (coverImagePath != null)
            {
                coverImagePath = coverImagePath.StartsWith(Constants.ImagesDirectory)
                    ? $"{context.FileSystem.GetRootPath()}/input/{coverImagePath}"
                    : $"{input.Source.Parent.FullPath}/{coverImagePath}";
            }

            var stream = await _imageService.CreateImageDocument(1200, 630, coverImagePath, siteTitle, centerText);

            var facebookDoc = context.CreateDocument(
                input.Source,
                $"./{Constants.SocialImagesDirectory}/{input.Destination.FileNameWithoutExtension}-facebook.png",
                context.GetContentProvider(stream));

            stream = await _imageService.CreateImageDocument(440, 220, coverImagePath, siteTitle, centerText);

            var twitterDoc = context.CreateDocument(
                input.Source,
                $"./{Constants.SocialImagesDirectory}/{input.Destination.FileNameWithoutExtension}-twitter.png",
                context.GetContentProvider(stream));

            return new[]
            {
                facebookDoc,
                twitterDoc
            };
        }
    }
}
