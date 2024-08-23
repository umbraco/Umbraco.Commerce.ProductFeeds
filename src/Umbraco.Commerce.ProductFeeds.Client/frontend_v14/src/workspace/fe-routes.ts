import { WORKSPACE_ENTITY_ALIAS } from '../constants';

const baseUrl = 'section/settings/workspace/uc:store-settings';

export const editRoute = (storeId: string, itemId: string) => `${baseUrl}/${storeId}/${WORKSPACE_ENTITY_ALIAS}/${itemId}`;

export const createRoute = (storeId: string) => editRoute(storeId, 'create');

export const listRoute = (storeId: string) => `${baseUrl}/${storeId}/${WORKSPACE_ENTITY_ALIAS}`;

export const storeRoute = (storeId: string) => `${baseUrl}/${storeId}`;