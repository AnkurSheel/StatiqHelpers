using Microsoft.Extensions.DependencyInjection;
using Statiq.App;
using Statiq.Common;
using Statiq.Core;
using Statiq.Web.Pipelines;
using StatiqHelpers.ImageHelpers;
using StatiqHelpers.Pipelines;
using StatiqHelpers.ReadingTimeModule;

namespace StatiqHelpers.Extensions
{
    public static class BootstrapperExtensions
    {
        public static Bootstrapper AddPipelines(this Bootstrapper bootstrapper)
        {
            bootstrapper.AddPipeline<PostPipeline>()
                .AddPipeline<PagesPipeline>()
                .AddPipeline<ImagesPipeline>()
                .AddPipeline<CssPipeline>()
                .AddPipeline<FontsPipeline>()
                .AddPipeline<ScriptsPipeline>()
                .AddPipeline<RssPipeline>()
                .AddPipeline<HomePipeline>()
                .AddPipeline<SocialImagesPipeline>()
                .AddPipeline<PostListPipeline>()
                .AddPipeline<TagsListPipeline>()
                .AddPipeline<TagsPipeline>()
                .AddPipeline<SitemapPipeline>();
            return bootstrapper;
        }

        public static Bootstrapper AddServices(this Bootstrapper bootstrapper)
        {
            bootstrapper.ConfigureServices(
                services =>
                {
                    services.AddTransient<IImageService, ImageService>();
                    services.AddTransient<IReadingTimeService, ReadingTimeService>();
                });
            return bootstrapper;
        }

        public static Bootstrapper RemovePipelines(this Bootstrapper bootstrapper)
        {
            bootstrapper.ConfigureEngine(
                engine =>
                {
                    engine.Pipelines.Remove(nameof(Inputs));
                    engine.Pipelines.Remove(nameof(Assets));
                    engine.Pipelines.Remove(nameof(Content));
                    engine.Pipelines.Remove(nameof(Sitemap));
                    engine.Pipelines.Remove(nameof(Archives));
                    engine.Pipelines.Remove(nameof(Feeds));
                    engine.Pipelines.Remove(nameof(Data));
                    engine.Pipelines.Remove(nameof(Redirects));
                    engine.Pipelines.Remove(nameof(SearchIndex));
                    engine.Pipelines.Remove(nameof(AnalyzeContent));
                    engine.Pipelines.Add(
                        nameof(AnalyzeContent),
                        new Pipeline
                        {
                            Deployment = true,
                            ExecutionPolicy = ExecutionPolicy.Normal,
                            InputModules =
                            {
                                new ReplaceDocuments(
                                    nameof(ScriptsPipeline),
                                    nameof(HomePipeline),
                                    nameof(PagesPipeline),
                                    nameof(PostListPipeline),
                                    nameof(PostPipeline),
                                    nameof(TagsListPipeline),
                                    nameof(TagsPipeline)),
                            }
                        });
                });

            return bootstrapper;
        }
    }
}
