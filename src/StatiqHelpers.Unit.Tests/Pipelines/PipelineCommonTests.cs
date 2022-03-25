using System.Linq;
using System.Text.Json;
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
            await Verifier.Verify(moduleList.Where(x => x is not GatherDocuments).Select(x => x.GetType().Name));
        }
    }
}
