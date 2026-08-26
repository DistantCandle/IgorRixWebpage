using Umbraco.Cms.Core.Models.Blocks;

namespace IgorRixWebpage.Web.Models.CompositionModels
{
    public class PageContentCompositionModel
    {
        // Raw block list from Umbraco - used when passing unmapped blocks
        public List<BlockListItem>? Content { get; set; }

        // Mapped block view models - uncomment when ready to use BlockMapper
        public List<object>? Blocks { get; set; }
    }
}