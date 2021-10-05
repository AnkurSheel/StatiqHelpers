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
        private readonly PostListOptions _options;

        public PostListPipeline(PostListOptions options)
        {
            _options = options;
            Dependencies.Add(nameof(PostPipeline));

            InputModules = new ModuleList
            {
                new ReadFiles("Blog.cshtml")
            };

            ProcessModules = new ModuleList
            {
                new SetDestination("blog.html"),
                new SetMetadata("Title", _options.Title)
            };

            PostProcessModules = new ModuleList
            {
                new RenderRazor().WithModel(
                    Config.FromDocument(
                        (document, context) =>
                        {
                            var documentList = context.Outputs.FromPipeline(nameof(PostPipeline));

                            documentList = _options.Descending
                                ? documentList.OrderByDescending(_options.OrderFunction).ToDocumentList()
                                : documentList.OrderBy(_options.OrderFunction).ToDocumentList();

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
