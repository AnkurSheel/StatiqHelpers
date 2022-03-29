using Statiq.Common;
using Statiq.Core;
using Statiq.Testing;
using StatiqHelpers.Modules;
using xUnitHelpers;

namespace StatiqHelpers.Unit.Tests.Modules
{
    public class GenerateSitemapMetaDataTests : BaseFixture
    {
        private const string Host = "www.example.com";
        private readonly TestDocument _input;
        private readonly GenerateSitemapMetaData _module;
        private readonly TestExecutionContext _context;
        private readonly NormalizedPath _path;

        public GenerateSitemapMetaDataTests()
        {
            _context = new TestExecutionContext();
            _context.Settings.Add(Keys.Host, Host);

            _path = new NormalizedPath("a.txt");
            _input = new TestDocument(_path);
            _module = new GenerateSitemapMetaData();
        }

        [Fact]
        public async Task Properties_are_set_correctly()
        {
            var result = await ExecuteAsync(_input, _context, _module).SingleAsync();

            AssertSitemapItem(result);
        }

        [Fact]
        public async Task Last_modified_is_set_correctly_when_there_is_a_published_date()
        {
            var publishedDate = DateTime.SpecifyKind(new DateTime(2022, 03, 27), DateTimeKind.Utc);
            _input.Add(MetaDataKeys.PublishedDate, publishedDate);

            var result = await ExecuteAsync(_input, _context, _module).SingleAsync();

            AssertSitemapItem(result, publishedDate);
        }

        [Fact]
        public async Task Last_modified_is_set_correctly_when_there_is_a_published_and_updated_date()
        {
            var publishedDate = DateTime.SpecifyKind(new DateTime(2022, 03, 27), DateTimeKind.Utc);
            var updatedDate = DateTime.SpecifyKind(new DateTime(2022, 03, 28), DateTimeKind.Utc);
            _input.Add(MetaDataKeys.PublishedDate, publishedDate);
            _input.Add(MetaDataKeys.UpdatedOnDate, updatedDate);

            var result = await ExecuteAsync(_input, _context, _module).SingleAsync();

            AssertSitemapItem(result, updatedDate);
        }

        private void AssertSitemapItem(TestDocument result, DateTime? expectedLastModifiedDate = null)
        {
            var sitemapItem = result[Keys.SitemapItem] as SitemapItem;

            AssertHelper.AssertMultiple(
                () => Assert.Equal($"http://{Host}/{_path}", sitemapItem?.Location),
                () => Assert.Equal(expectedLastModifiedDate, sitemapItem?.LastModUtc),
                () => Assert.Null(sitemapItem?.ChangeFrequency),
                () => Assert.Null(sitemapItem?.Priority));
        }
    }
}
