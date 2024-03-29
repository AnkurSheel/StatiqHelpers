﻿using StatiqHelpers.Modules;

namespace StatiqHelpers.Pipelines
{
    public class SitemapPipeline : Pipeline
    {
        public SitemapPipeline()
        {
            Dependencies.AddRange(nameof(HomePipeline), nameof(PagesPipeline), nameof(PostPipeline));
            ProcessModules = new ModuleList
            {
                new ConcatDocuments(Dependencies.ToArray()),
                new GenerateSitemapMetaData(),
                new GenerateSitemap()
            };

            OutputModules = new ModuleList
            {
                new WriteFiles()
            };
        }
    }
}
