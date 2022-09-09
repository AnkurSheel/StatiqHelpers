using Statiq.Common;
using Statiq.Testing;
using StatiqHelpers.Modules.RelatedPosts;

namespace StatiqHelpers.Unit.Tests.Modules.RelatedPosts;

public class RelatedPostsPointCalculatorTests
{
    private readonly IRelatedPostsPointCalculator _relatedPostsPointCalculator;

    public RelatedPostsPointCalculatorTests()
    {
        _relatedPostsPointCalculator = new RelatedPostsPointCalculator();
    }

    [Fact]
    public void Gets_2_points_for_a_category_match()
    {
        var document1 = GetTestDocument(
            "a.txt",
            "Category1",
            new List<string>
            {
                "Tag1"
            });
        var document2 = GetTestDocument(
            "b.txt",
            "Category1",
            new List<string>
            {
                "Tag2"
            });

        var points = _relatedPostsPointCalculator.GetPoints(document1, document2);

        Assert.Equal(2, points);
    }

    [Fact]
    public void Gets_1_point_for_each_tag_match()
    {
        var document1 = GetTestDocument(
            "a.txt",
            "Category1",
            new List<string>
            {
                "Tag1",
                "Tag2",
                "Tag3",
                "Tag4"
            });
        var document2 = GetTestDocument(
            "b.txt",
            "Category2",
            new List<string>
            {
                "Tag2",
                "Tag3",
                "Tag4",
                "Tag5",
            });

        var points = _relatedPostsPointCalculator.GetPoints(document1, document2);

        Assert.Equal(3, points);
    }

    private static TestDocument GetTestDocument(string path, string category, IReadOnlyList<string> tags)
        => new TestDocument(new NormalizedPath(path))
        {
            { MetaDataKeys.Category, category },
            { MetaDataKeys.Tags, tags }
        };
}
