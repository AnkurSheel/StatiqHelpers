using System.Linq;
using StatiqHelpers.ReadingTimeModule;
using Xunit;
using xUnitHelpers;

namespace StatiqHelpers.Unit.Tests.ReadingTimeModuleTests
{
    public class ReadingTimeServiceTests
    {
        private readonly IReadingTimeService _readingTimeService;

        public ReadingTimeServiceTests()
        {
            _readingTimeService = new ReadingTimeService();
        }

        [Theory]
        [InlineData(100, 200, 0, 30)]
        [InlineData(200, 200, 1, 0)]
        [InlineData(300, 200, 1, 30)]
        [InlineData(400, 200, 2, 0)]
        [InlineData(568, 200, 2, 50)]
        public void ReadingTime_is_calculated_correctly(int numberOfWords, int wordsPerMinute, int expectedMinutes, int expectedSeconds)
        {
            string input = string.Concat(Enumerable.Repeat("a ", numberOfWords));

            var readingTimeData = _readingTimeService.GetReadingTime(input, wordsPerMinute);

            AssertHelper.AssertMultiple(
                () => Assert.Equal(expectedMinutes, readingTimeData.Minutes),
                () => Assert.Equal(expectedSeconds, readingTimeData.Seconds),
                () => Assert.Equal(numberOfWords, readingTimeData.Words));
        }

        [Theory]
        [InlineData(29, 0)]
        [InlineData(30, 1)]
        [InlineData(31, 1)]
        public void RoundedMinutes_is_calculated_correctly(int numberOfWords, int expectedRoundedMinutes)
        {
            string input = string.Concat(Enumerable.Repeat("a ", numberOfWords));

            var readingTimeData = _readingTimeService.GetReadingTime(input, 60);

            Assert.Equal(expectedRoundedMinutes, readingTimeData.RoundedMinutes);
        }
    }
}
