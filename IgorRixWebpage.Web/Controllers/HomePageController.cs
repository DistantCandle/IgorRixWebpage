using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Extensions;
using IgorRixWebpage.Web.Models.ViewModels;
using Org.BouncyCastle.Crypto.Engines;
using IgorRixWebpage.Web.Helpers;
using Umbraco.Cms.Core.Models.Blocks;

namespace IgorRixWebpage.Web.Controllers
{
    public class HomePageController : RenderController
    {
        // public HomeController(
        //     ILogger<HomeController> logger,
        //     ICompositeViewEngine compositeViewEngine,
        //     IUmbracoContextAccessor umbracoContextAccessor)
        //     : base(logger, compositeViewEngine, umbracoContextAccessor)
        // {
        // }

        // When dealing with nested elements we will call blockmapper and pagecontent composition model
        // into our view model and then map the blocks to our composition model in the controller before passing it to the view.
        // This way we can keep all of our mapping logic in one place and our controller will just be responsible for passing the data to the view.

        //eg:
        private readonly BlockMapper _blockMapper;

        public HomePageController(
            ILogger<HomePageController> logger,
            ICompositeViewEngine compositeViewEngine,
            IUmbracoContextAccessor umbracoContextAccessor,
            BlockMapper blockMapper)
            : base(logger, compositeViewEngine, umbracoContextAccessor)
        {
            _blockMapper = blockMapper;
        }

        public override IActionResult Index()
        {
            var contentBlocks = CurrentPage?.Value<BlockListModel>("content");

            var viewModel = new HomePageViewModel
            {
                PageContentCompositionModel = _blockMapper.MapPageContent(contentBlocks)
            };

            return View("~/Views/HomePage/Index.cshtml", viewModel);
        }
    }
}