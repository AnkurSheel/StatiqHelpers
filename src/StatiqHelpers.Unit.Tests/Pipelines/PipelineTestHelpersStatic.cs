using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Statiq.Testing;
using StatiqHelpers.CustomExtensions;
using StatiqHelpers.ImageHelpers;

namespace StatiqHelpers.Unit.Tests.Pipelines
{
    public static class PipelineTestHelpersStatic
    {
        public static Bootstrapper GetBootstrapper()
        {
            var bootstrapper = Bootstrapper.Factory.CreateBootstrapper(Array.Empty<string>());
            bootstrapper.ConfigureAnalyzers(
                collection =>
                {
                    foreach (var (key, value) in collection)
                    {
                        value.LogLevel = LogLevel.None;
                    }
                });
            bootstrapper.ConfigureServices(services => services.AddSingleton(Mock.Of<IImageService>()));

            return bootstrapper;
        }

        public static TestFileProvider GetFileProvider(NormalizedPath path)
            => new TestFileProvider
            {
                path,
            };

        public static TestFileProvider GetFileProvider(NormalizedPath path, string content)
            => new TestFileProvider
            {
                { path, content },
            };
    }
}
