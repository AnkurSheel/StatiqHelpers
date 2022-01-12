using Statiq.App;
using Statiq.Web;
using StatiqHelpers.Commands;
using StatiqHelpers.ImageHelpers;

namespace StatiqHelpers.Extensions
{
    public static class BootstrapperFactoryExtensions
    {
        public static Bootstrapper InitStatiq(this BootstrapperFactory factory, string[] args)
            => factory.CreateWeb(args).RemovePipelines().AddCommand<ResizeImage>().AddCommand<NewPost>().AddPipelines().AddServices();
    }
}
