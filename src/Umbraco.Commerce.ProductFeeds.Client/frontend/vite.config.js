
import { defineConfig } from 'vite';
import tsconfigPaths from 'vite-tsconfig-paths';

const outputDir = '../../Umbraco.Commerce.ProductFeeds.Web/wwwroot';
export default defineConfig(({command, mode}) => {
    return {
        build: {
            minify: false,
            lib: {
                entry: {
                    'ucproductfeeds.backoffice': './src/index.ts',
                },
                formats: ['es'],
            },
            outDir: outputDir,
            emptyOutDir: mode !== 'development',
            sourcemap: true,
            rollupOptions: {
                external: [/^@umbraco/],
                onwarn: () => { },
            },
            cssCodeSplit: true,
        },
        plugins: [
            tsconfigPaths(),
        ],
    };
});
