using System.Linq;
using System.Threading.Tasks;
using Statiq.App;
using Statiq.Common;
using Statiq.Core;
using Statiq.Testing;
using VerifyXunit;
using Xunit;

namespace StatiqHelpers.Unit.Tests.Pipelines
{
    public static class PipelineCommonTests
    {
        public static async Task Verify_dependencies(Bootstrapper bootstrapper, string pipelineName)
        {
            var result = await bootstrapper.RunTestAsync(new TestFileProvider());

            Assert.Equal((int)ExitCode.Normal, result.ExitCode);

            var pipeline = result.Engine.Pipelines[pipelineName];
            await Verifier.Verify(pipeline.Dependencies);
        }

        public static async Task Verify_input_modules(Bootstrapper bootstrapper, string pipelineName)
        {
            var result = await bootstrapper.RunTestAsync(new TestFileProvider());

            Assert.Equal((int)ExitCode.Normal, result.ExitCode);

            var pipeline = result.Engine.Pipelines[pipelineName];
            await VerifyModule(pipeline.InputModules);
        }

        public static async Task Verify_process_modules(Bootstrapper bootstrapper, string pipelineName)
        {
            var result = await bootstrapper.RunTestAsync(new TestFileProvider());

            Assert.Equal((int)ExitCode.Normal, result.ExitCode);

            var pipeline = result.Engine.Pipelines[pipelineName];
            await VerifyModule(pipeline.ProcessModules);
        }

        public static async Task Verify_process_modules_cache(Bootstrapper bootstrapper, string pipelineName)
        {
            var result = await bootstrapper.RunTestAsync(new TestFileProvider());

            Assert.Equal((int)ExitCode.Normal, result.ExitCode);

            var pipeline = result.Engine.Pipelines[pipelineName];
            var cacheDocumentsModule = (pipeline.ProcessModules.Single(x => x is CacheDocuments) as CacheDocuments)!;
            await VerifyModule(pipeline.ProcessModules.Append(cacheDocumentsModule.Children.ToArray()));
        }

        public static async Task Verify_post_process_modules(Bootstrapper bootstrapper, string pipelineName)
        {
            var result = await bootstrapper.RunTestAsync(new TestFileProvider());

            Assert.Equal((int)ExitCode.Normal, result.ExitCode);

            var pipeline = result.Engine.Pipelines[pipelineName];
            await VerifyModule(pipeline.PostProcessModules);
        }

        public static async Task Verify_output_modules(Bootstrapper bootstrapper, string pipelineName)
        {
            var result = await bootstrapper.RunTestAsync(new TestFileProvider());

            Assert.Equal((int)ExitCode.Normal, result.ExitCode);

            var pipeline = result.Engine.Pipelines[pipelineName];
            await VerifyModule(pipeline.OutputModules);
        }

        private static async Task VerifyModule(ModuleList moduleList)
        {
            await Verifier.Verify(moduleList.Select(x => x.GetType().Name));
        }
    }
}
