using System.Text.RegularExpressions;

namespace StatiqHelpers.CustomExtensions
{
    public static class NormalizedPathExtensions
    {
        public static NormalizedPath OptimizeSlug(this NormalizedPath path)
        {
            var optimizedSegments = path.Segments.Select(
                    segment =>
                    {
                        var optimizedSegment = Constants.StopWords.Aggregate(segment.ToString(), (current, word) => ReplaceWholeWord(current, $"{word}", ""));

                        optimizedSegment = NormalizedPath.OptimizeFileName(optimizedSegment);

                        if (optimizedSegment[0] == '-')
                        {
                            optimizedSegment = optimizedSegment.Substring(1);
                        }

                        return optimizedSegment;
                    })
                .ToList();

            return string.Join("/", optimizedSegments);
        }

        private static string ReplaceWholeWord(string input, string wordToReplace, string replacementWord)
        {
            var pattern = $"\\b{wordToReplace}\\b";
            return Regex.Replace(input, pattern, replacementWord, RegexOptions.IgnoreCase);
        }
    }
}
