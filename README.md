# StatiqHelpers

An opinionated wrapper around Statiq

## Features

- Images are copied to the "assets/images" folder.
- Javascript files should be present in the "assets/js" folder and are copied into the "assets/js" folder.
    - Service worker file (sw.js) is copied to the root if present.
- Stylesheets are copied to the assets folder.
    - Files starting with \_ are ignored.
- TTF and WOFF2 fonts are copied to assets/fonts.
- Binary files with extensions pdf, zip, rar or exe are copied to assets/downloads.
- Slugs are optimized
- Pages
    - All pages need to belong to a pages subfolder.
    - They can be markdown or razor pages.
    - Markdown files have to be in a subfolder.
        - Generates the slug from the folder name for markdown files.
    - The slug and filename are used for setting the destination for razor pages.
    - Generates the slug from the file name for razor files.
    - Nested folders can be used to generate a route.
    - Page Images are copied to assets/images/pages/slug/.
        - Images can be in the same folder as the markdown file or in a slug/images folder.
- Posts
    - All posts need to belong to a posts subfolder.
    - Only markdown files are supported.
    - Markdown files have to be in a subfolder.
        - the folder name has to be in the format : "yyyy-mm-dd-slug". "yyyy-mm-dd" is used to get the published date.
        - Optional. You can have a top level folder for year to organize the files. This is ignored.
    - Posts in the future are ignored. *They are shown if the environment is set as development*
    - Post Images are copied to assets/images/posts/slug/.
        - Images can be in the same folder as the markdown file or in a yyyy-mm-dd-slug/images folder.
    - Reading Time is generated for the articles.
    - Social share images for facebook and twitter are created.
    - Related Posts are created based on category, tags and relative date from the post.
- Index page is taken from Index.cshtml and has access to the list of posts.
- Blog page is taken from Blog.cshtml and has access to the list of posts.
    - The grouping and ordering of the posts can be configured.
- A rss.xml is created for the RSS Feed
    - Set title from Post title.
    - Sets description from Post description.
    - Sets image from post image or shared image in assets/images folder.
    - Sets published date from post directory date.
    - Sets updated date from updated date from post frontmatter. Defaults to published date.
- A sitemap.xml is created
    - Sets the path and last modified date from the document.

## Modules

1. Reading Time Module
    - Generates reading time for articles.
    - WPM can be set as a setting  *ReadingTimeWordsPerMinute*. Defaults to 238.
    - Reading time is returned as Minutes and seconds as well as rounded minutes where 30 or more is rounded up.
2. Responsive YouTube Shortcode
    - Makes you tube videos responsive with a 16:9 aspect ratio and full width.
    - pass in the id and the title.
3. Optimize Slug
    - Optimizes the destination slug by
        - converting reserved characters to -.
        - converting to lower case.
        - trimming and collapsing whitespaces.
        - turning spaces into dashes.
        - removing multiple dashes.
        - removing filler words.
4. Replace Image Links
    - Replaces relative Image Links in markdown file to point to the correct folder in the images directory.
    - Absolute Image Links in markdown file are not replaced.
5. GenerateRssMetaData
    - Sets the title, description, updatedDate and image for the RSS feed from the post metadata.
6. GenerateSitemapMetaData
    - Sets the path and last modified date from the document.
7. GeneratePageDetailsFromPath
    - Sets the slug from the folder path for pages.
8. GeneratePostDetailsFromPath
    - Sets the slug and published date from the path.
9. GenerateSocialImages
    - Creates images to share on facebook and twitter.
    - Can pass in a cover image to use as the background
10. Related Posts Module
     - Generates related posts based on category, tags and relative date from the post.
     - Number of posts to show can be set as a setting  *NumberOfRelatedPosts*. Defaults to 4.
     - Does not condier posts that do not have a matching category or tag.
