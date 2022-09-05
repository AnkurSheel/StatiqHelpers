using Statiq.Razor;
using StatiqHelpers.Models;

namespace StatiqHelpers.CustomExtensions
{
    public static class RazorExtensions
    {
        public static RenderRazor WithBaseModel(this RenderRazor renderRazor, string? title = null)
        {
            return renderRazor.WithModel(Config.FromDocument((document, context) => new BaseModel(document, context)));
        }
    }
}
