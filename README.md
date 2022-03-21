# StatiqHelpers

An opinionated wrapper around Statiq

## Features

- Images are copied to the "assets/images" folder
- Stylesheets are copied to the assets folder.
    - Files starting with _ are ignored.
- TTF and WOFF2 fonts are copied to assets/fonts
- Binary files with extensions pdf, zip, rar or exe are copied to assets/downloads
- Pages
    - All pages need to belong to a pages subfolder.
    - Markdown files have to be in a subfolder.
        - Generates the slug from the folder name for markdown files.
    - Generates the slug from the file name for razor files.
    - nested folders can be used to generate a route.
    - Page Images are copied to assets/images/pages/slug/
        - Images can be in the same folder as the markdown file or in a slug/images folder
- Posts
    - Markdown files have to be in a subfolder.
        - the folder name has to be in the format : "yyyy-mm-dd-slug". "yyyy-mm-dd" is used to get the published date
        - Optional. You can have a top level folder for year to organize the files. This is ignored.
    - Posts in the future are ignored. *They are shown if the environment is set as development*
    - Post Images are copied to assets/images/posts/slug/
        - Images can be in the same folder as the markdown file or in a yyyy-mm-dd-slug/images folder
    - Reading Time is generated for the articles
- RSS Feed
    - Set title from Post title
    - Sets description from Post description
    - Sets image from post image or shared image in assets/images folder
    - Sets published date from post directory date
    - Sets updated date from updated date from post frontmatter. Defaults to published date

## Modules

4. Reading Time Module
    - Generates reading time for articles.
    - WPM can be set as a setting  *ReadingTimeWordsPerMinute*. Deafults to 238.
    - Reading time is returned as Minutes and seconds as well rounded minutes where 30 or more is rounded up
5. Responsive YouTube Shortcode
    - Makes you tube videos responsive with a 16:9 aspect ratio and full width
    - pass in the id and the title
