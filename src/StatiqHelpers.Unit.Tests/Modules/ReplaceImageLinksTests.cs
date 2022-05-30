using Statiq.Common;
using Statiq.Testing;
using StatiqHelpers.Modules;

namespace StatiqHelpers.Unit.Tests.Modules
{
    public class ReplaceImageLinksTests : BaseFixture
    {
        private readonly ReplaceImageLinks _module;

        public ReplaceImageLinksTests()
        {
            _module = new ReplaceImageLinks("imagesDir");
        }

        [Fact]
        public async Task Image_links_are_replaced_correctly_for_images_in_the_same_folder()
        {
            var content = "![Alt text 1](./image1.jpg)";

            var document = GetTestDocument(content);

            var result = await ExecuteAsync(document, _module).SingleAsync();

            await AssertContents(result, "![Alt text 1](/imagesDir/slug/image1.jpg)");
        }

        [Fact]
        public async Task Image_links_are_replaced_correctly_for_images_in_the_images_folder()
        {
            var content = "![Alt text 1](./images/image1.jpg)";

            var document = GetTestDocument(content);

            var result = await ExecuteAsync(document, _module).SingleAsync();

            await AssertContents(result, "![Alt text 1](/imagesDir/slug/image1.jpg)");
        }

        [Fact]
        public async Task Path_is_optimized_and_filler_words_are_removed()
        {
            var content = "![Alt text 1](./image1.jpg)";

            var document = GetTestDocument(content, "how to slug and new");

            var result = await ExecuteAsync(document, _module).SingleAsync();

            await AssertContents(result, "![Alt text 1](/imagesDir/slug-new/image1.jpg)");
        }

        [Fact]
        public async Task Image_links_are_not_replaced_for_images_with_absolute_path()
        {
            var content = "![Alt text 1](/folder/image1.jpg)";

            var document = GetTestDocument(content);

            var result = await ExecuteAsync(document, _module).SingleAsync();

            await AssertContents(result, "![Alt text 1](/folder/image1.jpg)");
        }

        private TestDocument GetTestDocument(string content, string slug = "slug")
        {
            var document = new TestDocument(new NormalizedPath("a.txt"), content)
            {
                { MetaDataKeys.Slug, slug }
            };
            return document;
        }

        private async Task AssertContents(TestDocument document, string expected)
        {
            var body = await document.GetContentStringAsync();
            Assert.Equal(expected, body);
        }
    }
}
