﻿using System;
using Statiq.Common;
using Statiq.Core;
using Statiq.Highlight;
using Statiq.Markdown;
using Statiq.Razor;
using Statiq.Yaml;
using StatiqHelpers.Extensions;
using StatiqHelpers.Modules;
using StatiqHelpers.Modules.MetadataFromPath;
using StatiqHelpers.Modules.ReadingTime;
using StatiqHelpers.Modules.Rss;

namespace StatiqHelpers.Pipelines
{
    public class PostPipeline : Pipeline
    {
        public PostPipeline(IReadingTimeService readingTimeService)
        {
            InputModules = new ModuleList
            {
                new ReadFiles("posts/**/*.{md,mdx}")
            };

            ProcessModules = new ModuleList
            {
                new CacheDocuments
                {
                    new ExtractFrontMatter(new ParseYaml()),
                    new GeneratePostDetailsFromPath(),
                    new FilterDocuments(Config.FromDocument((document, context) => context.IsDevelopment() || document.GetPublishedDate() <= DateTime.UtcNow.Date)),
                    new GenerateRssMetaData(),
                    new ReplaceInContent(@"!\[(?<alt>.*)\]\(./images/(?<imagePath>.*)\)", Config.FromDocument((document, context) => "![$1](./$2)")).IsRegex(),
                    new ReplaceInContent(
                            @"!\[(?<alt>.*)\]\(./(?<imagePath>.*)\)",
                            Config.FromDocument((document, context) => $"![$1](/{Constants.PostImagesDirectory}/{document.GetString(MetaDataKeys.Slug)}/$2)"))
                        .IsRegex(),
                    new GenerateReadingTime(readingTimeService),
                    new RenderMarkdown().UseExtensions(),
                    new ProcessShortcodes(),
                    new HighlightCode().WithAutoHighlightUnspecifiedLanguage(true),
                    new OptimizeSlug(),
                    new SetDestination(
                        Config.FromDocument((doc, ctx) => new NormalizedPath(Constants.BlogPath).Combine($"{doc.GetString(MetaDataKeys.Slug)}.html"))),
                }
            };

            PostProcessModules = new ModuleList
            {
                new RenderRazor().WithBaseModel()
            };

            OutputModules = new ModuleList
            {
                new WriteFiles()
            };
        }
    }
}
