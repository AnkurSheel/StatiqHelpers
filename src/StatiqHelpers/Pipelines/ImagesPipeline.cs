using StatiqHelpers.CustomExtensions;

namespace StatiqHelpers.Pipelines
{
    public class ImagesPipeline : Pipeline
    {
        public ImagesPipeline()
        {
            InputModules = new ModuleList
            {
                new ReadFiles("**/*.{jpg,png,svg,ico,gif}")
            };

            ProcessModules = new ModuleList
            {
                new CacheDocuments
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
                                        var slug = postDetailsFromPath["slug"].ToString();
                                        slug = slug.Replace("/images", "");
                                        var path = new NormalizedPath(slug);
                                        path = path.OptimizeSlug();
                                        return new NormalizedPath($"{Constants.PostImagesDirectory}/{path}/{doc.Source.FileName}");
                                    })))
                        .ElseIf(
                            Config.FromDocument(doc => doc.Source.FullPath.Contains("pages")),
                            new SetDestination(
                                Config.FromDocument(
                                    doc =>
                                    {
                                        var relativeInputPath = doc.Source.GetRelativeInputPath().ToString();
                                        relativeInputPath = relativeInputPath.Replace("/images", "");
                                        return new NormalizedPath($"{Constants.ImagesDirectory}/{relativeInputPath}");
                                    })))
                        .Else(
                            new SetDestination(
                                Config.FromDocument(document => new NormalizedPath(Constants.ImagesDirectory).Combine(document.Source.FileName))))
                }
            };

            OutputModules = new ModuleList
            {
                new WriteFiles()
            };
        }
    }
}
