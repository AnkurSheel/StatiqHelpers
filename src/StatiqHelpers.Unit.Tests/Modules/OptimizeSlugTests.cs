using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Statiq.Common;
using Statiq.Testing;
using StatiqHelpers.Modules;
using Xunit;

namespace StatiqHelpers.Unit.Tests.Modules
{
    public class OptimizeSlugTests : BaseFixture
    {
        [Fact]
        public async Task Slug_is_converted_to_lower_case()
        {
            const string slug = "FolDeR MiXeD CapS/FileName MiXeD CapS";
            var document = ModuleTestHelpersStatic.GetTestDocument(GetMetadata(slug));

            var optimizeSlug = new OptimizeSlug();

            var result = await ExecuteAsync(document, optimizeSlug).SingleAsync();

            Assert.Equal("folder-mixed-caps/filename-mixed-caps", result[MetaDataKeys.Slug].ToString());
        }

        [Theory]
        [MemberData(nameof(ReservedChars))]
        public async Task Reserved_characters_in_slug_are_converted_to_dash(char character)
        {
            var reservedCharacterString = new string(character, 10);
            var slug = $"/prefix {reservedCharacterString} folder/c/ prefixFile {reservedCharacterString} filename";
            var document = ModuleTestHelpersStatic.GetTestDocument(GetMetadata(slug));

            var optimizeSlug = new OptimizeSlug();

            IDocument result = await ExecuteAsync(document, optimizeSlug).SingleAsync();

            Assert.Equal("prefix-folder/c/prefixfile-filename", result[MetaDataKeys.Slug].ToString());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task Throws_an_exception_if_slug_is_null_or_empty(string slug)
        {
            var context = new TestExecutionContext();
            context.TestLoggerProvider.ThrowLogLevel = LogLevel.None;

            var document = ModuleTestHelpersStatic.GetTestDocument(GetMetadata(slug));
            var optimizeSlug = new OptimizeSlug();

            var exception = await Assert.ThrowsAsync<LoggedException>(() => ExecuteAsync(document, context, optimizeSlug).SingleAsync());

            Assert.IsType<ArgumentNullException>(exception.InnerException);
        }

        [Fact]
        public async Task Whitespaces_are_trimmed()
        {
            const string slug = "      folder   name   /  file    name     ";
            var document = ModuleTestHelpersStatic.GetTestDocument(GetMetadata(slug));

            var optimizeSlug = new OptimizeSlug();

            var result = await ExecuteAsync(document, optimizeSlug).SingleAsync();

            Assert.Equal("folder-name/file-name", result[MetaDataKeys.Slug].ToString());
        }

        [Theory]
        [MemberData(nameof(FillerWords))]
        public async Task Filler_words_are_removed(string slug)
        {
            var document = ModuleTestHelpersStatic.GetTestDocument(GetMetadata($"{slug} file name"));

            var optimizeSlug = new OptimizeSlug();

            var result = await ExecuteAsync(document, optimizeSlug).SingleAsync();

            Assert.Equal("file-name", result[MetaDataKeys.Slug].ToString());
        }

        [Theory]
        [MemberData(nameof(FillerWords))]
        public async Task Filler_words_that_are_part_of_another_word_are_not_removed(string slug)
        {
            var document = ModuleTestHelpersStatic.GetTestDocument(GetMetadata($"{slug}file name"));

            var optimizeSlug = new OptimizeSlug();

            var result = await ExecuteAsync(document, optimizeSlug).SingleAsync();

            Assert.Equal($"{slug}file-name", result[MetaDataKeys.Slug].ToString(), true);
        }

        private KeyValuePair<string, object>[] GetMetadata(string slug)
        {
            return new[] { new KeyValuePair<string, object>(MetaDataKeys.Slug, slug) };
        }

        public static readonly IEnumerable<object[]> FillerWords = Constants.StopWords.Select(x => new object[] { x }).ToArray();

        public static readonly IEnumerable<object[]> ReservedChars =
            NormalizedPath.OptimizeFileNameReservedChars.Where(x => x != '\\' && x != '/').Select(x => new object[] { x }).ToArray();
    }
}
