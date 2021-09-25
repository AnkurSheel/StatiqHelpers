using System.Text.RegularExpressions;

namespace StatiqHelpers.ReadingTime
{
    public class ReadingTimeService : IReadingTimeService
    {
        private static readonly Regex SpacesRegex = new Regex(@"\S+", RegexOptions.Multiline);

        public ReadingTimeData GetReadingTime(string content, int wordsPerMinute)
        {
            var words = SpacesRegex.Matches(content).Count;

            var minutes = words / wordsPerMinute;
            var remainingWords = words % wordsPerMinute;
            var seconds = remainingWords * 60 / wordsPerMinute;

            return new ReadingTimeData(minutes, seconds, words);
        }
    }
}
