using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Actions;
using Umbraco.Cms.Core.Models.Trees;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Trees;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.BackOffice.Filters;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Commerce.ProductFeeds.Constants;

namespace Umbraco.Commerce.ProductFeeds.Apis.Backoffice.Controllers
{
    [JsonCamelCaseFormatter]
    [PluginController(RouteParams.AreaName)]
    public class ProductFeedsTreeNodeController : UmbracoAuthorizedApiController
    {
        private readonly IMenuItemCollectionFactory _menuItemCollectionFactory;
        private readonly ILocalizedTextService _localizedTextService;

        public ProductFeedsTreeNodeController(
            IMenuItemCollectionFactory menuItemCollectionFactory,
            ILocalizedTextService localizationService)
        {
            _menuItemCollectionFactory = menuItemCollectionFactory;
            _localizedTextService = localizationService;
        }

        [HttpGet]
        public IActionResult GetMenu()
        {
            MenuItemCollection menu = _menuItemCollectionFactory.Create();
            MenuItem actionDelete = menu.Items.Add<ActionDelete>(_localizedTextService, false, false, false)!;
            actionDelete.Icon = "icon-delete";
            actionDelete.AdditionalData["actionView"] = $"/App_Plugins/{RouteParams.AreaName}/angularjs/productfeedsetting-delete-dialog.html";
            actionDelete.AdditionalData["dialogTitle"] = "Delete";

            return Ok(menu);
        }
    }
}
