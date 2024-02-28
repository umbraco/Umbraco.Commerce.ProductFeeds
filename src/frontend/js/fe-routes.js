import ucUtils from './utils';

/**
 * 
 * @param {string} storeId 
 * @param {string} itemId 
 * @returns 
 */
export const editRoute = (storeId, itemId) => `/settings/umbracocommerceproductfeeds/productfeedsetting-edit/${ucUtils.createCompositeId([storeId, itemId])}`;

/**
 * 
 * @param {string} storeId 
 * @returns 
 */
export const createRoute = (storeId) => editRoute(storeId, null);

/**
 * 
 * @param {string} storeId 
 * @returns 
 */
export const listRoute = (storeId) => `/settings/umbracocommerceproductfeeds/productfeedsetting-list/${ucUtils.createCompositeId([storeId])}`;