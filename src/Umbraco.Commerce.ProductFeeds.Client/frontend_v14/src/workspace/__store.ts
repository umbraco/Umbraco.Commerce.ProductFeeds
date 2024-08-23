// import type { UmbControllerHost } from '@umbraco-cms/backoffice/controller-api';
// import { UmbArrayState } from '@umbraco-cms/backoffice/observable-api';
// import { UmbContextToken } from '@umbraco-cms/backoffice/context-api';
// import { WORKSPACE_STORE_ALIAS } from '../constants.js';
// import { UmbStoreBase } from '@umbraco-cms/backoffice/store';
// import { ProductFeedSettingReadModel } from '../generated/apis/types.gen.js';

// export const UC_EMAIL_TEMPLATE_STORE_CONTEXT = new UmbContextToken<UcProductFeedStore>(WORKSPACE_STORE_ALIAS);

// export class UcProductFeedStore extends UmbStoreBase<ProductFeedSettingReadModel> {
// 	constructor(host: UmbControllerHost) {
// 		super(host, UC_EMAIL_TEMPLATE_STORE_CONTEXT.toString(), new UmbArrayState<ProductFeedSettingReadModel>([], (x) => x.id));
// 	}
// }

// export { UcProductFeedStore as api };