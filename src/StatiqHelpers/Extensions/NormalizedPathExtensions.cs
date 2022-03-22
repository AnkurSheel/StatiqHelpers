using System.Linq;
using System.Text.RegularExpressions;
using Statiq.Common;

namespace StatiqHelpers.Extensions
{
    public static class NormalizedPathExtensions
    {
        public static NormalizedPath OptimizeSlug(this NormalizedPath path)
        {
            var optimizedSegments = path.Segments.Select(
                    segment =>
                    {
                        var optimizedSegment = Constants.StopWords.Aggregate(
                            segment.ToString(),
                            (current, word) => Regex.Replace(current, @$"\b{word}\b", "", RegexOptions.IgnoreCase));

                        return NormalizedPath.OptimizeFileName(optimizedSegment);
                    })
                .ToList();

            return string.Join("/", optimizedSegments);
        }
    }
}
