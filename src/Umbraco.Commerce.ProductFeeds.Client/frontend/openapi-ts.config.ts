import { defineConfig } from '@hey-api/openapi-ts';

export default defineConfig({
    client: '@hey-api/client-fetch',
    input: 'http://localhost:43252/umbraco/swagger/ucproductfeeds/swagger.json',
    output: {
        path: 'src/generated/apis',
        lint: 'eslint',
    },
    schemas: false,
    // types: {
    //     enums: 'typescript',
    // },
});