using System;
using System.Threading.Tasks;
using Statiq.Common;
using Statiq.Testing;
using Xunit;
using xUnitHelpers;

namespace StatiqHelpers.Modules.MetadataFromPath
{
    public class GeneratePostDetailsFromPathTests : BaseFixture
    {
        [Theory]
        [MemberData(nameof(Data))]
        public async Task Sets_the_metadata_from_the_folder_path_for_markdown_files(string inputPath, DateTime expectedPublishedDate, string expectedSlug)
        {
            var input = new TestDocument(new NormalizedPath($"/input/{inputPath}"));
            var generatePostDetailsFromPath = new GeneratePostDetailsFromPath();

            var result = await ExecuteAsync(input, generatePostDetailsFromPath).SingleAsync();

            AssertHelper.AssertMultiple(() => Assert.Equal(expectedSlug, result[MetaDataKeys.Slug]), () => Assert.Equal(expectedPublishedDate, result[MetaDataKeys.PublishedDate]));
        }

        [Theory]
        [InlineData("folder1/filename.md")]
        [InlineData("202-10-21-folder1/filename.md")]
        [InlineData("2021-13-21-folder1/filename.md")]
        [InlineData("2021-10-35-folder1/filename.md")]
        [InlineData("2021-21-folder1/filename.md")]
        [InlineData("2021-10-21-folder1/filename.md")]
        [InlineData("2021-10-21/filename.md")]
        [InlineData("2021-10-21-/filename.md")]
        public void If_metadata_cannot_be_parsed_throw_exception(string inputPath)
        {
            var input = new TestDocument(new NormalizedPath($"/input/{inputPath}"));
            var generatePostDetailsFromPath = new GeneratePostDetailsFromPath();

            Assert.ThrowsAsync<Exception>(() => ExecuteAsync(input, generatePostDetailsFromPath));

            // Assert.Equal("Could not parse date and slug from folder", ex.Message);
        }

        public static readonly object[][] Data =
        {
            new object[] { "2021/2021-10-21-folder1/filename.md", new DateTime(2021, 10, 21), "folder1" }, new object[] { "2021-10-21-folder1/filename.md", new DateTime(2021, 10, 21), "folder1" },
        };
    }
}
