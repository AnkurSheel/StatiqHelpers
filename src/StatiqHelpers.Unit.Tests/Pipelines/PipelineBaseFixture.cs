using Statiq.Testing;

namespace StatiqHelpers.Unit.Tests.Pipelines;

public abstract class PipelineBaseFixture : BaseFixture
{
    protected readonly Bootstrapper Bootstrapper;

    protected PipelineBaseFixture()
    {
        Bootstrapper = PipelineTestHelpersStatic.GetBootstrapper();
        Bootstrapper.ConfigureEngine(engine =>
        {
            engine.Pipelines.RemoveAll(x => x.Key != PipelineName && !DependentPipelineNames.Contains(x.Key));
        });
        BaseSetUp();
    }

    protected abstract string PipelineName { get; }

    protected virtual string[] DependentPipelineNames => Array.Empty<string>();

    protected async Task VerifyDependencies()
    {
        var result = await Bootstrapper.RunTestAsync(new TestFileProvider());

        Assert.Equal((int)ExitCode.Normal, result.ExitCode);

        var pipeline = result.Engine.Pipelines[PipelineName];
        await Verify(pipeline.Dependencies);
    }

    protected async Task VerifyInputModules()
    {
        var result = await Bootstrapper.RunTestAsync(new TestFileProvider());

        Assert.Equal((int)ExitCode.Normal, result.ExitCode);

        var pipeline = result.Engine.Pipelines[PipelineName];
        await VerifyModule(pipeline.InputModules);
    }

    protected async Task VerifyProcessModules()
    {
        var result = await Bootstrapper.RunTestAsync(new TestFileProvider());

        Assert.Equal((int)ExitCode.Normal, result.ExitCode);

        var pipeline = result.Engine.Pipelines[PipelineName];
        var modules = pipeline.ProcessModules;

        if (modules.SingleOrDefault(x => x is CacheDocuments) is CacheDocuments cacheDocumentsModule)
        {
            modules.Append(cacheDocumentsModule.Children.ToArray());
        }

        if (modules.SingleOrDefault(x => x is ExecuteIf) is ExecuteIf executeIfModule)
        {
            foreach (var condition in executeIfModule)
            {
                modules.Append(condition.ToArray());
            }
        }

        await VerifyModule(modules);
    }

    protected async Task VerifyPostProcessModules()
    {
        var result = await Bootstrapper.RunTestAsync(new TestFileProvider());

        Assert.Equal((int)ExitCode.Normal, result.ExitCode);

        var pipeline = result.Engine.Pipelines[PipelineName];
        await VerifyModule(pipeline.PostProcessModules);
    }

    protected async Task VerifyOutputModules()
    {
        var result = await Bootstrapper.RunTestAsync(new TestFileProvider());

        Assert.Equal((int)ExitCode.Normal, result.ExitCode);

        var pipeline = result.Engine.Pipelines[PipelineName];
        await VerifyModule(pipeline.OutputModules);
    }

    private async Task VerifyModule(ModuleList moduleList)
    {
        await Verify(moduleList.Where(x => x is not GatherDocuments).Select(x => x.GetType().Name));
    }
}