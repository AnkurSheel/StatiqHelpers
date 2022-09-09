using Moq;
using Statiq.Common;
using Statiq.Testing;
using StatiqHelpers.Modules.RelatedPosts;
using xUnitHelpers;

namespace StatiqHelpers.Unit.Tests.Modules.RelatedPosts
{
    public class RelatedPostsServiceTests
    {
        private readonly IRelatedPostsService _relatedPostsService;
        private readonly Mock<IRelatedPostsPointCalculator> _relatedPostsPointCalculator = new();
        private readonly TestDocument _document1;
        private readonly TestDocument _document2;
        private readonly TestDocument _document3;
        private readonly List<IDocument> _documents;

        public RelatedPostsServiceTests()
        {
            _relatedPostsService = new RelatedPostsService(_relatedPostsPointCalculator.Object);
            _document1 = RelatedPostTestHelpersStatic.GetTestDocument("a.txt", "Title 1", "slug-1");
            _document2 = RelatedPostTestHelpersStatic.GetTestDocument("b.txt", "Title 2", "slug-2");
            _document3 = RelatedPostTestHelpersStatic.GetTestDocument("c.txt", "Title 3", "slug-3");
            _documents = new List<IDocument>
            {
                _document1,
                _document2,
                _document3,
            };
        }

        [Fact]
        public void Does_not_get_the_same_post_as_a_related_post()
        {
            var documents = new List<IDocument>
            {
                _document1,
                _document1
            };

            var relatedPosts = GetRelatedPosts(documents: documents);

            AssertHelper.AssertMultiple(
                () => Assert.Equal(0, relatedPosts.Count),
                () => _relatedPostsPointCalculator.Verify(x => x.GetPoints(It.IsAny<IDocument>(), It.IsAny<IDocument>()), Times.Never));
        }

        [Fact]
        public void Does_not_get_posts_which_are_not_related()
        {
            _relatedPostsPointCalculator.Setup(x => x.GetPoints(It.IsAny<IDocument>(), It.IsAny<IDocument>())).Returns(0);

            var relatedPosts = GetRelatedPosts();

            AssertHelper.AssertMultiple(
                () => Assert.Equal(0, relatedPosts.Count),
                () => _relatedPostsPointCalculator.Verify(x => x.GetPoints(It.IsAny<IDocument>(), It.IsAny<IDocument>()), Times.Exactly(2)));
        }

        [Fact]
        public void Related_Posts_are_ordered_by_points()
        {
            var expectedRelatedPosts = new List<RelatedPostInformation>
            {
                RelatedPostTestHelpersStatic.GetRelatedPostInformation(_document3),
                RelatedPostTestHelpersStatic.GetRelatedPostInformation(_document2),
            };

            _relatedPostsPointCalculator.SetupSequence(x => x.GetPoints(It.IsAny<IDocument>(), It.IsAny<IDocument>())).Returns(5).Returns(6);

            var relatedPosts = GetRelatedPosts();

            AssertHelper.AssertOrderedCollection(expectedRelatedPosts, relatedPosts);
        }

        [Fact]
        public void Related_Posts_are_ordered_by_relative_date_if_points_are_equal()
        {
            _document1.Add(MetaDataKeys.PublishedDate, DateTime.UtcNow);
            _document2.Add(MetaDataKeys.PublishedDate, DateTime.UtcNow.AddDays(3));
            _document3.Add(MetaDataKeys.PublishedDate, DateTime.UtcNow.AddDays(-1));

            var expectedRelatedPosts = new List<RelatedPostInformation>
            {
                RelatedPostTestHelpersStatic.GetRelatedPostInformation(_document3),
                RelatedPostTestHelpersStatic.GetRelatedPostInformation(_document2)
            };

            _relatedPostsPointCalculator.Setup(x => x.GetPoints(It.IsAny<IDocument>(), It.IsAny<IDocument>())).Returns(1);

            var relatedPosts = GetRelatedPosts();

            AssertHelper.AssertOrderedCollection(expectedRelatedPosts, relatedPosts);
        }

        [Fact]
        public void Does_not_get_more_than_the_required_number_of_related_posts()
        {
            _relatedPostsPointCalculator.Setup(x => x.GetPoints(It.IsAny<IDocument>(), It.IsAny<IDocument>())).Returns(1);

            var relatedPosts = GetRelatedPosts(1);

            Assert.Equal(RelatedPostTestHelpersStatic.GetRelatedPostInformation(_document2), relatedPosts.Single());
        }

        [Fact]
        public void Gets_all_posts_if_there_are_less_posts_then_the_required_number()
        {
            var documents = new List<IDocument>
            {
                _document1,
                _document2
            };

            _relatedPostsPointCalculator.Setup(x => x.GetPoints(It.IsAny<IDocument>(), It.IsAny<IDocument>())).Returns(1);

            var relatedPosts = GetRelatedPosts(documents: documents);

            Assert.Equal(RelatedPostTestHelpersStatic.GetRelatedPostInformation(_document2), relatedPosts.Single());
        }

        private IReadOnlyList<RelatedPostInformation> GetRelatedPosts(int numberOfRelatedPosts = 2, IReadOnlyList<IDocument>? documents = null)
            => _relatedPostsService.GetRelatedPosts(_document1, documents ?? _documents, numberOfRelatedPosts);

        
    }
}
