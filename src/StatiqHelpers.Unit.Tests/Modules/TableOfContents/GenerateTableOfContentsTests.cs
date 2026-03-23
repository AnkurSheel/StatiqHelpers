using Statiq.Testing;

using StatiqHelpers.Models;
using StatiqHelpers.Modules.TableOfContents;

using TocKeys = StatiqHelpers.Modules.TableOfContents.Keys;

namespace StatiqHelpers.Unit.Tests.Modules.TableOfContents
{
    public class GenerateTableOfContentsTests : BaseFixture
    {
        private readonly GenerateTableOfContents _module;
        private readonly TestExecutionContext _context;

        public GenerateTableOfContentsTests()
        {
            _context = new TestExecutionContext();
            _module = new GenerateTableOfContents();
        }

        [Fact]
        public async Task TableOfContents_is_generated_correctly_for_basic_headings()
        {
            var content = @"
# Heading 1
## Heading 2
### Heading 3
";
            var input = new TestDocument(content);

            var result = await ExecuteAsync(input, _context, _module).SingleAsync();

            var toc = result.Get<List<TocEntry>>(TocKeys.TableOfContents);
            Assert.NotNull(toc);
            Assert.Equal(3, toc.Count);

            Assert.Equal("Heading 1", toc[0].Title);
            Assert.Equal("heading-1", toc[0].Id);
            Assert.Equal(1, toc[0].Level);

            Assert.Equal("Heading 2", toc[1].Title);
            Assert.Equal("heading-2", toc[1].Id);
            Assert.Equal(2, toc[1].Level);

            Assert.Equal("Heading 3", toc[2].Title);
            Assert.Equal("heading-3", toc[2].Id);
            Assert.Equal(3, toc[2].Level);
        }

        [Fact]
        public async Task TableOfContents_is_generated_correctly_for_headings_with_special_characters()
        {
            var content = @"
## Heading with Space
## Heading-with-Dash
## Heading with !@#$%^&*()_+ special characters
";
            var input = new TestDocument(content);

            var result = await ExecuteAsync(input, _context, _module).SingleAsync();

            var toc = result.Get<List<TocEntry>>(TocKeys.TableOfContents);
            Assert.NotNull(toc);
            Assert.Equal(3, toc.Count);

            Assert.Equal("Heading with Space", toc[0].Title);
            Assert.Equal("heading-with-space", toc[0].Id);

            Assert.Equal("Heading-with-Dash", toc[1].Title);
            Assert.Equal("heading-with-dash", toc[1].Id);

            Assert.Equal("Heading with !@#$%^&*()_+ special characters", toc[2].Title);
            Assert.Equal("heading-with-_-special-characters", toc[2].Id);
        }

        [Fact]
        public async Task TableOfContents_handles_nested_inline_elements()
        {
            var content = @"
## Heading with **Bold** and *Italic*
## Heading with [Link](http://example.com)
";
            var input = new TestDocument(content);

            var result = await ExecuteAsync(input, _context, _module).SingleAsync();

            var toc = result.Get<List<TocEntry>>(TocKeys.TableOfContents);
            Assert.NotNull(toc);
            Assert.Equal(2, toc.Count);

            // Based on GetHeaderText implementation: inline.Descendants<LiteralInline>().Select(x => x.Content)
            // Bold/Italic/Link wrap the text in other inline types, but the text itself should be LiteralInline.
            Assert.Equal("Heading with Bold and Italic", toc[0].Title);
            Assert.Equal("heading-with-bold-and-italic", toc[0].Id);

            Assert.Equal("Heading with Link", toc[1].Title);
            Assert.Equal("heading-with-link", toc[1].Id);
        }

        [Fact]
        public async Task TableOfContents_is_empty_when_no_headings()
        {
            var content = @"
This is some content without any headings.
";
            var input = new TestDocument(content);

            var result = await ExecuteAsync(input, _context, _module).SingleAsync();

            var toc = result.Get<List<TocEntry>>(TocKeys.TableOfContents);
            Assert.NotNull(toc);
            Assert.Empty(toc);
        }
    }
}
