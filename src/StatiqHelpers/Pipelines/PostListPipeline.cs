using System.Linq;
using Statiq.Common;
using Statiq.Core;
using Statiq.Razor;
using Statiq.Yaml;
using StatiqHelpers.Extensions;
using StatiqHelpers.Models;

namespace StatiqHelpers.Pipelines
{
    public class PostListPipeline : Pipeline
    {
        public PostListPipeline(PostListOptions options)
        {
            Dependencies.Add(nameof(PostPipeline));

            InputModules = new ModuleList
            {
                new ReadFiles("Blog.cshtml")
            };

            ProcessModules = new ModuleList
            {
                new ExtractFrontMatter(new ParseYaml()),
                new SetDestination("blog.html"),
            };

            PostProcessModules = new ModuleList
            {
                new RenderRazor().WithModel(
                    Config.FromDocument(
                        (document, context) =>
                        {
                            var documentList = context.Outputs.FromPipeline(nameof(PostPipeline));

                            documentList = options.Descending
                                ? documentList.OrderByDescending(options.OrderFunction).ToDocumentList()
                                : documentList.OrderBy(options.OrderFunction).ToDocumentList();

                            var allPosts = documentList.Select(x => x.AsBaseModel(context)).ToList();
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
