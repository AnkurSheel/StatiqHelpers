using StatiqHelpers.Pipelines;

namespace StatiqHelpers.Unit.Tests.Pipelines;

[UsesVerify]
public class SocialImagePipelineTests : PipelineBaseFixture
{
    protected override string PipelineName => nameof(SocialImagesPipeline);
    protected override string[] DependentPipelineNames => new[] { nameof(PostPipeline) };

    [Fact]
    public async Task Verify_dependencies()
    {
        await VerifyDependencies();
    }

    [Fact]
    public async Task Verify_input_modules()
    {
        await VerifyInputModules();
    }

    [Fact]
    public async Task Verify_process_modules()
    {
        await VerifyProcessModules();
    }

    [Fact]
    public async Task Verify_post_process_modules()
    {
        await VerifyPostProcessModules();
    }

    [Fact]
    public async Task Verify_output_modules()
    {
        await VerifyOutputModules();
    }
}