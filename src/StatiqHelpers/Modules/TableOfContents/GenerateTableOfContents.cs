using Markdig;
using Markdig.Extensions.AutoIdentifiers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

using StatiqHelpers.Models;

namespace StatiqHelpers.Modules.TableOfContents;

public class GenerateTableOfContents : ParallelModule
{
    protected override async Task<IEnumerable<IDocument>> ExecuteInputAsync(IDocument input, IExecutionContext context)
    {
        var content = await input.GetContentStringAsync();
        var pipeline = new MarkdownPipelineBuilder()
            .UseAutoIdentifiers(AutoIdentifierOptions.GitHub)
            .Build();
        var markdownDocument = Markdown.Parse(content, pipeline);

        var toc = new List<TocEntry>();

        foreach (var heading in markdownDocument.Descendants<HeadingBlock>())
        {
            var title = GetHeaderText(heading);
            var id = heading.GetAttributes().Id;

            if (!string.IsNullOrEmpty(id))
            {
                toc.Add(new TocEntry(title, id, heading.Level));
            }
        }

        return input.Clone(new MetadataItems
        {
            { Keys.TableOfContents, toc }
        }).Yield();
    }

    private string GetHeaderText(HeadingBlock heading)
    {
        var inline = heading.Inline;
        if (inline == null)
        {
            return string.Empty;
        }

        return string.Concat(inline.Descendants<LiteralInline>().Select(x => x.Content));
    }
}