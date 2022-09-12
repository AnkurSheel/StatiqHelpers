using Moq;
using Statiq.Testing;
using StatiqHelpers.ImageHelpers;
using StatiqHelpers.Modules;
using StatiqHelpers.Modules.ReadingTime;
using xUnitHelpers;
using Keys = Statiq.Common.Keys;

namespace StatiqHelpers.Unit.Tests.Modules;

public class GenerateSocialImagesTests : BaseFixture
{
    private const string SiteTitle = "SiteTitle";
    private readonly GenerateSocialImages _module;
    private readonly Mock<IImageService> _imageService;

    public GenerateSocialImagesTests()
    {
        _imageService = new Mock<IImageService>();
        _module = new GenerateSocialImages(_imageService.Object);
    }

    [Fact]
    public async Task Social_images_are_created_for_facebook_twitter()
    {
        var input = GetTestDocument();

        var result = await ExecuteAsync(input, GetTestContext(), _module);

        AssertHelper.AssertUnorderedCollection(
            new[]
            {
                "assets/images/social/FileName-facebook.png",
                "assets/images/social/FileName-twitter.png",
            },
            result.Select(x => x.Destination.ToString()).ToList());
    }

    [Fact]
    public async Task Image_documents_with_the_correct_dimensions_are_created_for_facebook_twitter()
    {
        var input = GetTestDocument();

        var result = await ExecuteAsync(input, GetTestContext(), _module);

        AssertHelper.AssertMultiple(
            () => _imageService.Verify(x => x.CreateImageDocument(1200, 630, It.IsAny<string?>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once),
            () => _imageService.Verify(x => x.CreateImageDocument(440, 220, It.IsAny<string?>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once),
            () => _imageService.VerifyNoOtherCalls());
    }

    [Theory]
    [InlineData("Filename 1")]
    [InlineData("Filename 2")]
    public async Task Center_text_contains_the_title(string filename)
    {
        var input = GetTestDocument(filename);

        var result = await ExecuteAsync(input, GetTestContext(), _module);

        var expectedCenterText = $"{filename.ToUpper()}{Environment.NewLine}1 min";
        AssertHelper.AssertMultiple(
            () => _imageService.Verify(
                x => x.CreateImageDocument(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>(), It.IsAny<string>(), expectedCenterText),
                Times.Exactly(2)));
    }

    [Theory]
    [MemberData(nameof(ReadingTimeData))]
    public async Task Center_text_contains_the_reading_time(ReadingTimeData readingTimeData, string expectedCenterText)
    {
        var input = GetTestDocument(readingTimeData: readingTimeData);

        var result = await ExecuteAsync(input, GetTestContext(), _module);

        AssertHelper.AssertMultiple(
            () => _imageService.Verify(
                x => x.CreateImageDocument(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>(), It.IsAny<string>(), expectedCenterText),
                Times.Exactly(2)));
    }

    [Fact]
    public async Task If_site_title_exists_image_contains_the_site_title()
    {
        var input = GetTestDocument();

        var siteTitle = "SiteTitle";
        var result = await ExecuteAsync(input, GetTestContext(siteTitle), _module);

        AssertHelper.AssertMultiple(
            () => _imageService.Verify(
                x => x.CreateImageDocument(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>(), siteTitle, It.IsAny<string>()),
                Times.Exactly(2)));
    }

    [Fact]
    public async Task If_site_title_does_not_exist_the_image_does_not_contain_the_site_title()
    {
        var input = GetTestDocument();

        var result = await ExecuteAsync(input, GetTestContext(), _module);

        AssertHelper.AssertMultiple(
            () => _imageService.Verify(
                x => x.CreateImageDocument(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>(), "", It.IsAny<string>()),
                Times.Exactly(2)));
    }

    [Fact]
    public async Task If_cover_image_does_not_exist_the_image_does_not_have_a_cover_image()
    {
        var input = GetTestDocument();

        var result = await ExecuteAsync(input, GetTestContext(), _module);

        AssertHelper.AssertMultiple(
            () => _imageService.Verify(
                x => x.CreateImageDocument(It.IsAny<int>(), It.IsAny<int>(), null, It.IsAny<string>(), It.IsAny<string>()),
                Times.Exactly(2)));
    }

    [Fact]
    public async Task If_cover_image_exist_the_image_has_a_cover_image()
    {
        var coverImage = "cover.jpg";
        var input = GetTestDocument("./folder/slug/filename.txt", coverImage: $"{coverImage}");

        var result = await ExecuteAsync(input, GetTestContext(), _module);

        AssertHelper.AssertMultiple(
            () => _imageService.Verify(
                x => x.CreateImageDocument(It.IsAny<int>(), It.IsAny<int>(), $"/input/folder/slug/{coverImage}", It.IsAny<string>(), It.IsAny<string>()),
                Times.Exactly(2)));
    }

    [Fact]
    public async Task If_cover_image_exists_in_the_images_folder_the_image_has_a_cover_image()
    {
        var coverImage = "assets/images/cover.jpg";
        var context = GetTestContext();
        var input = GetTestDocument("./folder/slug/filename.txt", coverImage: $"{coverImage}");

        var result = await ExecuteAsync(input, context, _module);

        AssertHelper.AssertMultiple(
            () => _imageService.Verify(
                x => x.CreateImageDocument(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    $"{context.FileSystem.RootPath}/input/{coverImage}",
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Exactly(2)));
    }

    private TestExecutionContext GetTestContext(string? siteTitle = null, NormalizedPath? rootPath = null)
    {
        var context = new TestExecutionContext();

        rootPath ??= new NormalizedPath("root");

        context.FileSystem.RootPath = (NormalizedPath)rootPath;

        if (siteTitle != null)
        {
            context.Settings.Add(Keys.Title, SiteTitle);
        }

        return context;
    }

    private TestDocument GetTestDocument(NormalizedPath? path = null, ReadingTimeData? readingTimeData = null, string? coverImage = null)
    {
        path = path ?? new NormalizedPath("./FileName.txt");

        var input = new TestDocument((NormalizedPath)path)
        {
            { StatiqHelpers.Modules.ReadingTime.Keys.ReadingTime, readingTimeData ?? new ReadingTimeData(1, 0, 0) }
        };

        if (coverImage != null)
        {
            input.Add(MetaDataKeys.CoverImage, coverImage);
        }

        return input;
    }

    public static readonly object[][] ReadingTimeData =
    {
        new object[]
        {
            new ReadingTimeData(0, 10, 0),
            $"FILENAME{Environment.NewLine}10 sec"
        },
        new object[]
        {
            new ReadingTimeData(5, 10, 0),
            $"FILENAME{Environment.NewLine}5 min"
        },
        new object[]
        {
            new ReadingTimeData(5, 40, 0),
            $"FILENAME{Environment.NewLine}6 min"
        },
    };
}
