import { UmbWorkspaceActionArgs, UmbWorkspaceActionBase } from '@umbraco-cms/backoffice/workspace';
import { DETAILS_WORKSPACE_CONTEXT, UcpfDetailsWorkspaceContext } from '../context';
import { UmbControllerHost } from '@umbraco-cms/backoffice/controller-api';

export class UcpfWorkspaceActionOpenFeed extends UmbWorkspaceActionBase<UcpfDetailsWorkspaceContext> {
    #workspaceContext?: UcpfDetailsWorkspaceContext;

    #feedRelativePath?: string;

    constructor(host: UmbControllerHost, args: UmbWorkspaceActionArgs<UcpfDetailsWorkspaceContext>) {
        super(host, args);

        this.consumeContext(DETAILS_WORKSPACE_CONTEXT, (context) => {
            this.#workspaceContext = context;
            this.observe(
                this.#workspaceContext?.model,
                (model) => {
                    this.#feedRelativePath = model?.feedRelativePath;
                    if (model && model.id && model.feedRelativePath) {
                        this.enable();
                    } else {
                        this.disable();
                    }
                },
            );
        });
    }

    override async execute() {
        if (this.#feedRelativePath) {
            window.open(`/umbraco/commerce/productfeed/${this.#feedRelativePath}`, '_blank');
        }
    }
}
