using System.Text.RegularExpressions;
using StatiqHelpers.CustomExtensions;

namespace StatiqHelpers.Modules
{
    public class ReplaceImageLinks : ParallelModule
    {
        private readonly string _imagesDirectory;

        public ReplaceImageLinks(string imagesDirectory)
        {
            _imagesDirectory = imagesDirectory;
        }

        protected override async Task<IEnumerable<IDocument>> ExecuteInputAsync(IDocument input,
            IExecutionContext context)
        {
            var currentDocumentContent = await input.GetContentStringAsync();
            var slug = input.GetString(MetaDataKeys.Slug);
            var optimizedSlug = new NormalizedPath(slug).OptimizeSlug();

            var replaced = Regex.Replace(currentDocumentContent, @"!\[(?<alt>.*)\]\(./images/(?<imagePath>.*)\)",
                "![$1](./$2)", RegexOptions.None);

            // !\[(?<alt>[^\]]+)\]\(\./(?<imagepath>[^)]*)\)

            replaced = Regex.Replace(replaced, @"!\[(?<alt>[^\]]+)\]\(\./(?<imagePath>[^)]*)\)",
                $"![$1](/{_imagesDirectory}/{optimizedSlug}/$2)", RegexOptions.None);
            return input.Clone(context.GetContentProvider(replaced, input.ContentProvider.MediaType)).Yield();
        }
    }
}
