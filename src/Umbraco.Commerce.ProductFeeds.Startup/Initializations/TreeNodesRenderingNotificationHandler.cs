using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Trees;
using Umbraco.Commerce.ProductFeeds.Core.Constants;

namespace Umbraco.Commerce.ProductFeeds.Startup.Initializations
{
    /// <summary>
    /// Inject custom tree on menu rendering event.
    /// </summary>
    public class TreeNodesRenderingNotificationHandler : INotificationHandler<TreeNodesRenderingNotification>
    {
        public void Handle(TreeNodesRenderingNotification notification)
        {
            ArgumentNullException.ThrowIfNull(notification);

            string storeId = notification.QueryString["id"].ToString();
            if (notification.TreeAlias.Equals(Cms.Constants.Trees.Settings.Alias, StringComparison.Ordinal)
                && notification.QueryString["nodeType"].ToString().Equals(Cms.Constants.Trees.Settings.NodeType.Store.ToString(), StringComparison.Ordinal)
                && !string.IsNullOrEmpty(storeId))
            {
                TreeNode menuItem = new("7428", storeId, null, null)
                {
                    Name = "Product Feeds",
                    Icon = "icon-rss",
                    NodeType = "ProductFeedsList",
                    RoutePath = $"{notification.QueryString["application"]}/{RouteParams.AreaName}/productfeedsetting-list/{storeId}",
                    MenuUrl = $"/umbraco/backoffice/{RouteParams.AreaName}/productfeedstreenode/getmenu",
                    ChildNodesUrl = null,
                };

                menuItem.AdditionalData.Add("storeId", storeId);
                notification.Nodes.Add(menuItem);
            }
        }
    }
}
