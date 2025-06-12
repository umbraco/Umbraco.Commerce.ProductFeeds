import { defineConfig } from '@hey-api/openapi-ts';

export default defineConfig({
    input: 'http://localhost:44321/umbraco/swagger/ucproductfeeds/swagger.json',
    output: {
        path: 'src/generated/apis',
        lint: 'eslint',
    },
    plugins: ['@hey-api/client-axios'],
});