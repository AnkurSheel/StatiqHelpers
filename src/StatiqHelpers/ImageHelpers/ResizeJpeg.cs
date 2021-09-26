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
    public class ResizeJpeg : EngineCommand<ResizeJpegSettings>
    {
        private IImageService? _imageService;

        public ResizeJpeg(IConfiguratorCollection configurators, Settings settings, IServiceCollection serviceCollection, Bootstrapper bootstrapper) : base(
            configurators,
            settings,
            serviceCollection,
            bootstrapper)
        {
        }

        protected override async Task<int> ExecuteEngineAsync(CommandContext commandContext, ResizeJpegSettings commandSettings, IEngineManager engineManager)
        {
            _imageService = engineManager.Engine.Services.GetRequiredService<IImageService>();

            Engine engine = engineManager.Engine;
            var jpegs = GetImages(!commandSettings.AllFiles, engine.FileSystem);

            var message = commandSettings.AllFiles
                ? "all files"
                : "checked out files";
            engineManager.Engine.Logger.Log(LogLevel.Information, "Beginning resizing of images on {message} : {count}", message, jpegs.Count);

            await _imageService.ResizeImages(jpegs, commandSettings.Width, commandSettings.Height);

            return 0;
        }

        private IReadOnlyList<string> GetImages(bool onlyCheckedOutFiles, IFileSystem fileSystem)
        {
            var jpegs = fileSystem.GetInputFiles("**/*.{jpg, jpeg}").Select(x => x.Path).ToList();

            if (onlyCheckedOutFiles)
            {
                var rootPath = fileSystem.RootPath.Parent.Parent.FullPath;
                using var repo = new Repository(rootPath);
                var status = repo.RetrieveStatus();

                var modifiedJpegs = status.Where(x => Path.GetExtension((string?)x.FilePath) == ".jpg" && x.State != FileStatus.Ignored)
                    .Select(x => new NormalizedPath(Path.Combine(rootPath, x.FilePath))).ToList();

                jpegs = jpegs.Where(x => modifiedJpegs.Contains(x)).ToList();
            }

            return jpegs.Select(x => x.FullPath).ToList();
        }
    }
}
