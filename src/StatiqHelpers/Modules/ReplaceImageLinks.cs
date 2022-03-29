using System.Text.RegularExpressions;
using Statiq.Common;

namespace StatiqHelpers.Modules
{
    public class ReplaceImageLinks : ParallelModule
    {
        private readonly string _imagesDirectory;

        public ReplaceImageLinks(string imagesDirectory)
        {
            _imagesDirectory = imagesDirectory;
        }

        protected override async Task<IEnumerable<IDocument>> ExecuteInputAsync(IDocument input, IExecutionContext context)
        {
            var currentDocumentContent = await input.GetContentStringAsync();
            var replaced = Regex.Replace(currentDocumentContent, @"!\[(?<alt>.*)\]\(./images/(?<imagePath>.*)\)", "![$1](./$2)", RegexOptions.None);

            replaced = Regex.Replace(
                replaced,
                @"!\[(?<alt>.*)\]\(./(?<imagePath>.*)\)",
                $"![$1](/{_imagesDirectory}/{input.GetString(MetaDataKeys.Slug)}/$2)",
                RegexOptions.None);
            return input.Clone(context.GetContentProvider(replaced, input.ContentProvider.MediaType)).Yield();
        }
    }
}
