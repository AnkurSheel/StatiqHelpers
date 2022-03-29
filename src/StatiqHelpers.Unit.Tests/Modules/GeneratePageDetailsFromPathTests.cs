using Statiq.Common;
using Statiq.Testing;
using StatiqHelpers.Modules;

namespace StatiqHelpers.Unit.Tests.Modules
{
    public class GeneratePageDetailsFromPathTests : BaseFixture
    {
        [Theory]
        [InlineData("folder1/filename.md", "folder1")]
        [InlineData("folder1/folder2/filename.md", "folder1/folder2")]
        public async Task Sets_the_slug_from_the_folder_path_for_markdown_files(string inputPath, string expectedSlug)
        {
            var input = new TestDocument(new NormalizedPath($"/input/pages/{inputPath}"));
            var generatePageDetailsFromPath = new GeneratePageDetailsFromPath();

            var result = await ExecuteAsync(input, generatePageDetailsFromPath).SingleAsync();

            Assert.Equal(expectedSlug, result[MetaDataKeys.Slug]);
        }

        [Fact]
        public void If_markdown_files_is_not_in_a_subfolder_throw_exception()
        {
            var input = new TestDocument(new NormalizedPath("/input/pages/filename.md"));
            var generatePageDetailsFromPath = new GeneratePageDetailsFromPath();

            Assert.ThrowsAsync<NotSupportedException>(() => ExecuteAsync(input, generatePageDetailsFromPath).SingleAsync());
        }

        [Theory]
        [InlineData("filename.cshtml", "filename")]
        [InlineData("folder1/filename.cshtml", "folder1/filename")]
        public async Task Sets_the_slug_from_the_folder_path_and_filename_for_razor_files(string inputPath, string expectedSlug)
        {
            var input = new TestDocument(new NormalizedPath($"/input/pages/{inputPath}"));
            var generatePageDetailsFromPath = new GeneratePageDetailsFromPath();

            var result = await ExecuteAsync(input, generatePageDetailsFromPath).SingleAsync();

            Assert.Equal(expectedSlug, result[MetaDataKeys.Slug]);
        }
    }
}
