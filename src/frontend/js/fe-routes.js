import ucUtils from './utils';

/**
 * 
 * @param {string} storeId 
 * @param {number} itemId 
 * @returns 
 */
export const editRoute = (storeId, itemId) => `/settings/umbracocommerceproductfeeds/productfeedsetting-edit/${ucUtils.createCompositeId([storeId, itemId])}`;

export const listRoute = (storeId) => `/settings/umbracocommerceproductfeeds/productfeedsetting-list/${ucUtils.createCompositeId([storeId])}`;