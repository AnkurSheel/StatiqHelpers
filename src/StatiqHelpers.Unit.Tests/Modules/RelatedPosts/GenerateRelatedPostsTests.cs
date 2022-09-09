using System.Collections.Immutable;
using Moq;
using Statiq.Common;
using Statiq.Testing;
using StatiqHelpers.Modules.RelatedPosts;
using StatiqHelpers.Pipelines;
using Keys = StatiqHelpers.Modules.RelatedPosts.Keys;

namespace StatiqHelpers.Unit.Tests.Modules.RelatedPosts
{
    public class GenerateRelatedPostsTests : BaseFixture
    {
        private readonly List<IDocument> _documents;
        private readonly GenerateRelatedPosts _module;

        private readonly TestExecutionContext _context;

        private readonly Mock<IRelatedPostsService> _relatedPostService = new();

        public GenerateRelatedPostsTests()
        {
            _context = new TestExecutionContext();
            _documents = new List<IDocument>
            {
                RelatedPostTestHelpersStatic.GetTestDocument("a.txt", "Title 1", "slug-1"),
                RelatedPostTestHelpersStatic.GetTestDocument("b.txt", "Title 2", "slug-2"),
                RelatedPostTestHelpersStatic.GetTestDocument("c.txt", "Title 3", "slug-3")
            };
            _module = new GenerateRelatedPosts(_relatedPostService.Object);
        }

        [Fact]
        public async Task Sets_RelatedPosts_Key()
        {
            var dictionary = new Dictionary<string, ImmutableArray<IDocument>>
            {
                { nameof(PostPipeline), _documents.ToImmutableArray() }
            };

            _context.Outputs = new TestPipelineOutputs(dictionary);

            var expectedDocuments = _documents.Take(2).Select(RelatedPostTestHelpersStatic.GetRelatedPostInformation).ToList();

            _relatedPostService.Setup(x => x.GetRelatedPosts(It.IsAny<IDocument>(), It.IsAny<IReadOnlyList<IDocument>>(), It.IsAny<int>()))
                .Returns(expectedDocuments);

            var document = _documents.First() as TestDocument;
            var result = await ExecuteAsync(document, _context, _module).SingleAsync();

            Assert.Equal(expectedDocuments, result[Keys.RelatedPosts]);
        }
    }
}
