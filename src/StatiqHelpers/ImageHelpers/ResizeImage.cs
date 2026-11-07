using System.ComponentModel;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Spectre.Console.Cli;

using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace StatiqHelpers.ImageHelpers
{
    [Description(
        "Resizes the jpegs. Passing zero for one of height or width within the resize options will automatically preserve the aspect ratio of the original image or the nearest possible ratio")]
    public class ResizeImage : EngineCommand<ResizeImageSettings>
    {
        private IImageService? _imageService;

        public ResizeImage(
            IConfiguratorCollection configurators,
            Settings settings,
            IServiceCollection serviceCollection,
            IFileSystem fileSystem,
            Bootstrapper bootstrapper) : base(configurators, settings, serviceCollection, fileSystem, bootstrapper)
        {
        }

        protected override async Task<int> ExecuteEngineAsync(CommandContext commandContext, ResizeImageSettings commandSettings, IEngineManager engineManager)
        {
            _imageService = engineManager.Engine.Services.GetRequiredService<IImageService>();
            var gitService = engineManager.Engine.Services.GetRequiredService<IGitService>();

            var engine = engineManager.Engine;
            var images = GetImages(!commandSettings.AllFiles, engine.FileSystem, gitService);

            var message = commandSettings.AllFiles
                ? "all files"
                : "checked out files";
            engineManager.Engine.Logger.Log(LogLevel.Information, "Beginning resizing of images on {message} : {count} : {increase}", message, images.Count, commandSettings.IncreaseImageSizes);

            await _imageService.ResizeImages(images, commandSettings.Width, commandSettings.Height, commandSettings.IncreaseImageSizes);

            return 0;
        }

        private IReadOnlyList<string> GetImages(
            bool onlyCheckedOutFiles,
            IReadOnlyFileSystem fileSystem,
            IGitService gitService
        )
        {
            var images = fileSystem.GetInputFiles("**/*.{jpg,jpeg,png,webp}").Select(x => x.Path).ToList();

            if (onlyCheckedOutFiles)
            {
                var rootPath = fileSystem.RootPath.Parent.Parent.FullPath;
                var modifiedFiles = gitService.GetModifiedFiles(rootPath);

                var modifiedImages = modifiedFiles.Where(
                        x =>
                        {
                            var extension = Path.GetExtension(x);
                            return extension is ".jpg" or ".jpeg" or ".png" or ".webp";
                        })
                    .Select(x => new NormalizedPath(Path.Combine(rootPath, x)))
                    .ToList();

                images = images.Where(x => modifiedImages.Contains(x)).ToList();
            }

            return images.Select(x => x.FullPath).ToList();
        }
    }
}
