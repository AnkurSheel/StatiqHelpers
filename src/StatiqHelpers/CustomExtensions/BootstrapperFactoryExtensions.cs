using StatiqHelpers.Analyzers;
using StatiqHelpers.Commands;
using StatiqHelpers.ImageHelpers;

namespace StatiqHelpers.CustomExtensions;

public static class BootstrapperFactoryExtensions
{
    public static Bootstrapper CreateBootstrapper(this BootstrapperFactory factory, string[] args)
        => factory.CreateWeb(args).RemovePipelines().AddCommand<ResizeImage>().AddCommand<NewPost>().AddPipelines().AddServices().AddAnalyzer<TitleLengthAnalyzer>().AddAnalyzer<ExcerptLengthAnalyzer>();
}
