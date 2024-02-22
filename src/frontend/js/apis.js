import axios from 'axios';
import { editRoute } from './fe-routes';

const BASE_BACKOFFICE_ROUTE = '/umbraco/backoffice/umbracocommerceproductfeeds';

export const getFeedSetingsAsync = async function (storeId) {
    const { data } = await axios.get(BASE_BACKOFFICE_ROUTE + `/productfeedsettinglist/get?storeId=${storeId}`);
    return data
        .map(((item) => {
            item.icon = 'icon-rss';
            item.name = item.feedName;
            item.routePath = editRoute(storeId, item.id);
            return item;
        }));
};

export const getFeedSettingAsync = async function (feedId) {
    const { data } = await axios.get(BASE_BACKOFFICE_ROUTE + `/ProductFeedSetting/get?id=${feedId}`);
    console.log(data);
    return data;
};