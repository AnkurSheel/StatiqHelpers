using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LibGit2Sharp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spectre.Console.Cli;
using Statiq.App;
using Statiq.Common;
using Statiq.Core;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace StatiqHelpers.ImageHelpers
{
    [Description(
        "Resizes the jpegs. Passing zero for one of height or width within the resize options will automatically preserve the aspect ratio of the original image or the nearest possible ratio")]
    public class ResizeImage : EngineCommand<ResizeImageSettings>
    {
        private IImageService? _imageService;

        public ResizeImage(IConfiguratorCollection configurators, Settings settings, IServiceCollection serviceCollection, Bootstrapper bootstrapper) : base(
            configurators,
            settings,
            serviceCollection,
            bootstrapper)
        {
        }

        protected override async Task<int> ExecuteEngineAsync(CommandContext commandContext, ResizeImageSettings commandSettings, IEngineManager engineManager)
        {
            _imageService = engineManager.Engine.Services.GetRequiredService<IImageService>();

            Engine engine = engineManager.Engine;
            var images = GetImages(!commandSettings.AllFiles, engine.FileSystem);

            var message = commandSettings.AllFiles
                ? "all files"
                : "checked out files";
            engineManager.Engine.Logger.Log(LogLevel.Information, "Beginning resizing of images on {message} : {count}", message, images.Count);

            await _imageService.ResizeImages(images, commandSettings.Width, commandSettings.Height);

            return 0;
        }

        private IReadOnlyList<string> GetImages(bool onlyCheckedOutFiles, IFileSystem fileSystem)
        {
            var images = fileSystem.GetInputFiles("**/*.{jpg,jpeg,png}").Select(x => x.Path).ToList();

            if (onlyCheckedOutFiles)
            {
                var rootPath = fileSystem.RootPath.Parent.Parent.FullPath;
                using var repo = new Repository(rootPath);
                var status = repo.RetrieveStatus();

                var modifiedImages = status.Where(x =>
                    {
                        var extension = Path.GetExtension((string?)x.FilePath);
                        return (extension is ".jpg" or "jpeg" || extension == "png") && x.State != FileStatus.Ignored;
                    })
                    .Select(x => new NormalizedPath(Path.Combine(rootPath, x.FilePath))).ToList();

                images = images.Where(x => modifiedImages.Contains(x)).ToList();
            }

            return images.Select(x => x.FullPath).ToList();
        }
    }
}
