using Statiq.Common;
using Statiq.Core;
using StatiqHelpers.Extensions;

namespace StatiqHelpers.Pipelines
{
    public class ImagesPipeline : Pipeline
    {
        public ImagesPipeline()
        {
            InputModules = new ModuleList
            {
                new ReadFiles("**/*.{jpg,png,svg,ico}")
            };

            ProcessModules = new ModuleList
            {
                new ExecuteIf(Config.FromDocument(doc => doc.Source.Name.Contains("favicon.ico")))
                    {
                        new SetDestination("favicon.ico")
                    }.ElseIf(
                        Config.FromDocument(doc => doc.Source.FullPath.Contains("posts")),
                        new SetDestination(
                            Config.FromDocument(
                                doc =>
                                {
                                    var postDetailsFromPath = doc.GetPostDetailsFromPath();
                                    var slug = postDetailsFromPath["slug"];
                                    return new NormalizedPath($"{Constants.PostImagesDirectory}/{slug}/{doc.Source.FileName}");
                                })))
                    .Else(new SetDestination(Config.FromDocument(document => new NormalizedPath(Constants.ImagesDirectory).Combine(document.Source.FileName))))
            };

            OutputModules = new ModuleList
            {
                new WriteFiles()
            };
        }
    }
}
