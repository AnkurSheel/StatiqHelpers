using System.Linq;
using Statiq.Common;
using Statiq.Core;
using Statiq.Razor;
using StatiqHelpers.Extensions;
using StatiqHelpers.Models;

namespace StatiqHelpers.Pipelines
{
    public class PostListPipeline : Pipeline
    {
        public PostListPipeline()
        {
            Dependencies.Add(nameof(PostPipeline));

            InputModules = new ModuleList
            {
                new ReadFiles("Blog.cshtml")
            };

            ProcessModules = new ModuleList
            {
                new SetDestination("blog.html"),
                new SetMetadata("Title", "All Summaries")
            };

            PostProcessModules = new ModuleList
            {
                new RenderRazor().WithModel(
                    Config.FromDocument(
                        (document, context) =>
                        {
                            var allPosts = context.Outputs.FromPipeline(nameof(PostPipeline)).OrderBy(x => x.GetTitle()).Select(x => x.AsBaseModel(context)).ToList();
                            return new Posts(allPosts, document, context);
                        })),
            };

            OutputModules = new ModuleList
            {
                new WriteFiles()
            };
        }
    }
}
