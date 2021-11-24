using Statiq.App;
using Statiq.Web;
using StatiqHelpers.ImageHelpers;

namespace StatiqHelpers.Extensions
{
    public static class BootstrapperFactoryExtensions
    {
        public static Bootstrapper InitStatiq(this BootstrapperFactory factory, string[] args)
            => factory.CreateWeb(args).RemovePipelines().AddCommand<ResizeImage>().AddPipelines().AddServices();
    }
}
