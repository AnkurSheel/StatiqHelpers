using System.Linq;
using System.Threading.Tasks;
using Moq;
using Statiq.Common;
using Statiq.Testing;
using StatiqHelpers.Modules.ReadingTime;
using Xunit;

namespace StatiqHelpers.Unit.Tests.ReadingTimeModuleTests
{
    public class GenerateReadingTimeTests : BaseFixture
    {
        [Fact]
        public async Task Sets_ReadingTime_Key()
        {
            var readingTimeService = new Mock<IReadingTimeService>();
            const int numberOfWords = 250;
            var readingTimeData = new ReadingTimeData(5, 10, numberOfWords);
            readingTimeService.Setup(x => x.GetReadingTime(It.IsAny<string>(), It.IsAny<int>())).Returns(readingTimeData);

            var content = string.Concat(Enumerable.Repeat("a ", numberOfWords));
            var input = new TestDocument(content);
            var generateReadingTime = new GenerateReadingTime(readingTimeService.Object);

            var result = await ExecuteAsync(input, generateReadingTime).SingleAsync();

            Assert.Equal(readingTimeData, result["ReadingTime"]);
        }
    }
}
