using System.Threading.Tasks;
using StatiqHelpers.Pipelines;
using VerifyXunit;
using Xunit;

namespace StatiqHelpers.Unit.Tests.Pipelines;

[UsesVerify]
public class SocialImagePipelineTests : PipelineBaseFixture
{
    private const string PipelineName = nameof(SocialImagesPipeline);

    [Fact]
    public async Task Verify_dependencies()
    {
        await VerifyDependencies(PipelineName);
    }

    [Fact]
    public async Task Verify_input_modules()
    {
        await VerifyInputModules(PipelineName);
    }

    [Fact]
    public async Task Verify_process_modules()
    {
        await VerifyProcessModules(PipelineName);
    }

    [Fact]
    public async Task Verify_post_process_modules()
    {
        await VerifyPostProcessModules(PipelineName);
    }

    [Fact]
    public async Task Verify_output_modules()
    {
        await VerifyOutputModules(PipelineName);
    }
}
