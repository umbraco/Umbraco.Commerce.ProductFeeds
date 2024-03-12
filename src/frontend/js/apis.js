import axios from 'axios';
import { editRoute } from './fe-routes';

const BASE_BACKOFFICE_ROUTE = '/umbraco/backoffice/umbracocommerceproductfeeds';


/**
 * Trim umbraco api prefix similar to using $httpService directive
 * @param {*} response 
 */
const trimUmbracoPrefix = (response) => {
    const umbApiPrefix = ")]}',\n";
    if (response && response.data && typeof (response.data) === 'string' && response.data.startsWith(umbApiPrefix)) {
        response.data = JSON.parse(response.data.trimStart(umbApiPrefix));
    }
};

axios.interceptors.response.use(function (response) {
    trimUmbracoPrefix(response);
    return response;
}, function (error) {
    const { response } = error;
    trimUmbracoPrefix(response);
    return Promise.reject(error);
});

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
    return data;
};

/**
 * 
 * @returns {Promise<string[]>}
 */
export const getFeedTypesAsync = async () => {
    const { data } = await axios.get(BASE_BACKOFFICE_ROUTE + '/ProductFeedSetting/GetFeedTypes');
    return data;
};

/**
 * 
 * @returns {Promise<string[]>}
 */
export const getDocumentTypesAsync = async () => {
    const { data } = await axios.get(BASE_BACKOFFICE_ROUTE + '/ProductFeedSetting/GetDocumentTypes');
    return data;
};

/**
 * 
 * @param {string} documentTypeAlias 
 * @returns {Promise<string[]>}
 */
export const getCustomPropertyAliasesAsync = async (documentTypeAlias) => {
    const { data } = await axios.get(BASE_BACKOFFICE_ROUTE + `/ProductFeedSetting/GetPropertyAliases?documentTypeAliases=${documentTypeAlias}`);
    return data;
};

/**
 * 
 * @param {object} model 
 * @returns {Promise<string>} Saved record id.
 */
export const saveSettingAsync = async (model) => {
    const { data } = await axios.post(BASE_BACKOFFICE_ROUTE + '/ProductFeedSetting/Save', model);
    return data;
};

/**
 * 
 * @param {string} id 
 * @returns {Promise<boolean>}
 */
export const deleteAsync = async (id) => {
    const formData = new FormData();
    formData.append('id', id);
    const { data } = await axios.post(BASE_BACKOFFICE_ROUTE + '/ProductFeedSetting/Delete', formData);
    return data;
};

/**
 * 
 * @returns {Promise<string[]>}
 */
export const getPropertyValueExtractorsAsync = async () => {
    const { data } = await axios.get(BASE_BACKOFFICE_ROUTE + '/ProductFeedSetting/GetPropertyValueExtractors');
    return data;
};