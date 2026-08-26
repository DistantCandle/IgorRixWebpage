using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Routing;
using Umbraco.Extensions;
using IgorRixWebpage.Web.Models.NestedElements;
using IgorRixWebpage.Web.Models;
using IgorRixWebpage.Web;
using System.Text.RegularExpressions;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Services;
using Microsoft.AspNetCore.Http;
using IgorRixWebpage.Web.Models.CompositionModels;

namespace IgorRixWebpage.Web.Helpers
{
    public class BlockMapper
    {
        private readonly IPublishedValueFallback _publishedValueFallback;
        private readonly IPublishedUrlProvider _publishedUrlProvider;
        private readonly Umbraco.Cms.Core.Web.IUmbracoContextAccessor _umbracoContextAccessor;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IContentTypeService _contentTypeService;
        private readonly IDataTypeService _dataTypeService;

        public BlockMapper(
            IPublishedValueFallback publishedValueFallback,
            IPublishedUrlProvider publishedUrlProvider,
            Umbraco.Cms.Core.Web.IUmbracoContextAccessor umbracoContextAccessor,
            IHttpContextAccessor httpContextAccessor,
            IContentTypeService contentTypeService,
            IDataTypeService dataTypeService)
        {
            _publishedValueFallback = publishedValueFallback;
            _publishedUrlProvider = publishedUrlProvider;
            _umbracoContextAccessor = umbracoContextAccessor;
            _httpContextAccessor = httpContextAccessor;
            _contentTypeService = contentTypeService;
            _dataTypeService = dataTypeService;
        }

        // Page-level mapper - uncomment when ready to wire up PageContentCompositionModel
        public PageContentCompositionModel MapPageContent(BlockListModel? blockList)
        {
            var model = new PageContentCompositionModel();
            if (blockList == null) return model;

            model.Blocks = blockList.Select(block => block.Content.ContentType.Alias switch
            {
                "headerComponent" => (object?)MapHeaderComponentBlock(block),
                _ => null
            })
            .Where(b => b != null)
            .ToList()!;

            return model;
        }

        // Header Component Block Method
        public HeaderComponent? MapHeaderComponentBlock(BlockListItem block)
        {
            if (block?.Content == null)
            {
                return null;
            }

            var content = block.Content;


            // ==============================
            // LOGO
            // ==============================

            var media = content.Value<MediaWithCrops>(
                _publishedValueFallback,
                "image"
            );


            // ==============================
            // NAVIGATION
            // ==============================

            var navigation =
                content.Value<IEnumerable<IPublishedContent>>(
                    _publishedValueFallback,
                    "navigation"
                ) ?? Enumerable.Empty<IPublishedContent>();


            // ==============================
            // CTA
            // ==============================

            var ctaLink = content.Value<Link>(
                _publishedValueFallback,
                "ctaLink"
            );


            // ==============================
            // HEADER
            // ==============================

            var header = new HeaderComponent
            {
                Logo = media?.MediaUrl(
                    _publishedUrlProvider
                ),

                Navigation = navigation
                    .Take(4)
                    .Where(x => x != null && x.IsVisible())
                    .Select(x => new HeaderNavigationItem
                    {
                        Name = x.Name,
                        Url = x.Url(),
                        Active = IsCurrentPage(x)
                    })
                    .ToList(),

                CtaText = content.Value<string>(
                    _publishedValueFallback,
                    "ctaText"
                ),

                CtaLink = ctaLink?.Url,

                ShowCTA = content.Value<bool>(
                    _publishedValueFallback,
                    "showCTA"
                ),

                StickyHeader = content.Value<bool>(
                    _publishedValueFallback,
                    "stickyHeader"
                )
            };

            return header;
        }
        private HeaderNavigationItem MapNavigationItem(IPublishedContent item)
        {
            return new HeaderNavigationItem
            {
                Name = item.Name,
                Url = item.Url(),
                Active = IsCurrentPage(item),

                Children = item.Children()
                    .Where(x => x.IsVisible())
                    .Select(MapNavigationItem)
                    .ToList()
            };
        }
        private bool IsCurrentPage(
            IPublishedContent item)
        {
            var currentPage = _umbracoContextAccessor
                .GetRequiredUmbracoContext()
                .PublishedRequest?
                .PublishedContent;

            if (currentPage == null)
            {
                return false;
            }

            return item.Id == currentPage.Id ||
                   item.IsAncestor(currentPage);
        }

    }
}


//Declarations:

// Image = media?.MediaUrl(_publishedUrlProvider),
// ImageAltText = content.Value<string>(_publishedValueFallback, "imageAltText") ?? string.Empty,
// Title = content.Value<string>(_publishedValueFallback, "title") ?? string.Empty,
// SubTitle = content.Value<string>(_publishedValueFallback, "subTitle") ?? string.Empty,
// Button = content.Value<IEnumerable<Link>>(_publishedValueFallback, "button")?
// Body = content.Value<string>(_publishedValueFallback, "body") ?? string.Empty,
//    .Select(link => new Link
//    {
//        Url = link.Url,
//        Name = link.Name,
//        Target = link.Target
//    })
//    .ToList(),