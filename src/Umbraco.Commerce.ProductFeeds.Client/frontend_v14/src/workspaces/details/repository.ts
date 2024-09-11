import { UmbRepositoryBase } from '@umbraco-cms/backoffice/repository';
import { UcpfListDataSource } from '../datasource';
import { UmbControllerHost } from '@umbraco-cms/backoffice/controller-api';

export class UcpfReadWriteRepository
    extends UmbRepositoryBase {

    #dataSource: UcpfListDataSource;

    constructor(host: UmbControllerHost) {
        super(host);
        this.#dataSource = new UcpfListDataSource(host);
    }

    async fetchSingleAsync(id: string) {
        return this.#dataSource.fetchFeedSettingDetailsAsync(id);
    }

    async fetchFeedTypesAsync() {
        return this.#dataSource.fetchFeedTypesAsync();
    }
}
