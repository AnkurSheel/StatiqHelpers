﻿using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using Spectre.Console.Cli;
using Statiq.App;
using Statiq.Common;

namespace StatiqHelpers.Commands
{
    public class NewPostSettings : BaseCommandSettings
    {
    }

    [Description("Creates the structure and frontmatter for a new post")]
    public class NewPost : EngineCommand<NewPostSettings>
    {
        public NewPost(
            IConfiguratorCollection configurators,
            Settings settings,
            IServiceCollection serviceCollection,
            IFileSystem fileSystem,
            Bootstrapper bootstrapper) : base(configurators, settings, serviceCollection, fileSystem, bootstrapper)
        {
        }

        protected override async Task<int> ExecuteEngineAsync(CommandContext commandContext, NewPostSettings commandSettings, IEngineManager engineManager)
        {
            var frontMatter = new StringBuilder();
            frontMatter.AppendLine("---");

            var title = AnsiConsole.Ask<string>("Enter the [green]title[/]?");
            frontMatter.AppendLine(@$"title: ""{title}""");

            var excerpt = AnsiConsole.Prompt(new TextPrompt<string>("[grey][[Optional]][/] Enter the [green]excerpt/description[/]").AllowEmpty());
            frontMatter.AppendLine(@$"excerpt: ""{excerpt}""");

            var date = GetDate();

            AddCoverImageToFrontMatter(frontMatter);
            AddTagsToFrontmatter(frontMatter);

            frontMatter.AppendLine();
            frontMatter.AppendLine("---");

            frontMatter.AppendLine();

            var file = GetFile(engineManager, title, date);

            await file.WriteAllTextAsync(frontMatter.ToString());

            engineManager.Engine.Logger.Log(LogLevel.Information, "Wrote new markdown file at {File}", file.Path);

            return 0;
        }

        private static IFile GetFile(IEngineManager engineManager, string title, DateTime date)
        {
            var fileSystem = engineManager.Engine.FileSystem;
            var rootPath = fileSystem.GetRootPath();

            var optimizedTitle = NormalizedPath.OptimizeFileName(title);
            var filePath = $"{rootPath}/input/posts/{date.Year}/{date:yyyy-MM-dd}-{optimizedTitle}/{optimizedTitle}.md";

            return fileSystem.GetFile(filePath);
        }

        private void AddCoverImageToFrontMatter(StringBuilder frontMatter)
        {
            var coverImage = AnsiConsole.Prompt(new TextPrompt<string>("[grey][[Optional]][/] Enter coverImage").AllowEmpty());

            if (!string.IsNullOrWhiteSpace(coverImage))
            {
                frontMatter.AppendLine(@$"coverImage: ""{coverImage}""");
            }
        }

        private void AddTagsToFrontmatter(StringBuilder frontMatter)
        {
            var tagsAsString = AnsiConsole.Ask<string>("Enter comma seperated [green]tags[/]");
            var tags = tagsAsString.Split(
                    new[]
                    {
                        ',',
                        ';',
                        '|'
                    },
                    StringSplitOptions.RemoveEmptyEntries)
                .Select(i => i.Trim())
                .ToArray();
            frontMatter.AppendLine("tags:");

            foreach (var tag in tags)
            {
                frontMatter.AppendLine(@$"- ""{tag}""");
            }
        }

        private static DateTime GetDate()
        {
            var dateAsString = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter [green]date[/] in the format (yyyy-MM-dd). Leave blank for today date").AllowEmpty());

            var date = DateTime.Now;

            if (!string.IsNullOrWhiteSpace(dateAsString))
            {
                date = DateTime.ParseExact(dateAsString, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }

            return date;
        }
    }
}