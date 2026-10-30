import { defineConfig } from '@hey-api/openapi-ts';

export default defineConfig({
    input: 'http://localhost:44321/umbraco/openapi/ucproductfeeds.json',
    output: {
        path: 'src/generated/apis',
    },
    plugins: [
        {
            name: '@hey-api/client-fetch',
            exportFromIndex: true,
        },
    ],
});
