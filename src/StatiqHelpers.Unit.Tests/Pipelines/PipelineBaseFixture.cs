using Statiq.App;
using Statiq.Common;
using Statiq.Core;
using Statiq.Testing;

namespace StatiqHelpers.Unit.Tests.Pipelines
{
    public abstract class PipelineBaseFixture : BaseFixture
    {
        protected readonly Bootstrapper Bootstrapper;

        protected PipelineBaseFixture()
        {
            Bootstrapper = PipelineTestHelpersStatic.GetBootstrapper();
            BaseSetUp();
        }

        protected async Task VerifyDependencies(string pipelineName)
        {
            var result = await Bootstrapper.RunTestAsync(new TestFileProvider());

            Assert.Equal((int)ExitCode.Normal, result.ExitCode);

            var pipeline = result.Engine.Pipelines[pipelineName];
            await Verify(pipeline.Dependencies);
        }

        protected async Task VerifyInputModules(string pipelineName)
        {
            var result = await Bootstrapper.RunTestAsync(new TestFileProvider());

            Assert.Equal((int)ExitCode.Normal, result.ExitCode);

            var pipeline = result.Engine.Pipelines[pipelineName];
            await VerifyModule(pipeline.InputModules);
        }

        protected async Task VerifyProcessModules(string pipelineName)
        {
            var result = await Bootstrapper.RunTestAsync(new TestFileProvider());

            Assert.Equal((int)ExitCode.Normal, result.ExitCode);

            var pipeline = result.Engine.Pipelines[pipelineName];
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

        protected async Task VerifyPostProcessModules(string pipelineName)
        {
            var result = await Bootstrapper.RunTestAsync(new TestFileProvider());

            Assert.Equal((int)ExitCode.Normal, result.ExitCode);

            var pipeline = result.Engine.Pipelines[pipelineName];
            await VerifyModule(pipeline.PostProcessModules);
        }

        protected async Task VerifyOutputModules(string pipelineName)
        {
            var result = await Bootstrapper.RunTestAsync(new TestFileProvider());

            Assert.Equal((int)ExitCode.Normal, result.ExitCode);

            var pipeline = result.Engine.Pipelines[pipelineName];
            await VerifyModule(pipeline.OutputModules);
        }

        private async Task VerifyModule(ModuleList moduleList)
        {
            await Verify(moduleList.Where(x => x is not GatherDocuments).Select(x => x.GetType().Name));
        }
    }
}
