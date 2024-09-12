
import { defineConfig } from 'vite';
import { viteStaticCopy } from 'vite-plugin-static-copy';
import tsconfigPaths from 'vite-tsconfig-paths';

const outputDir = '../../Umbraco.Commerce.ProductFeeds.Web/wwwroot/';
export default defineConfig({
    build: {
        minify: false,
        lib: {
            entry: {
                'ucproductfeeds.backoffice': './src/index.ts',
            },
            formats: ['es'],
        },
        outDir: outputDir,
        emptyOutDir: true,
        sourcemap: true,
        rollupOptions: {
            external: [/^@umbraco/],
            onwarn: () => { },
            output: {
                assetFileNames: (assetInfo) => {
                    if (assetInfo.name === 'uccheckout.css') {
                        return 'uccheckout.surface.css';
                    }

                    return '[name].[ext]';
                },
            },
        },
        cssCodeSplit: true,
    },
    plugins: [
        tsconfigPaths(),
        viteStaticCopy({
            targets: [
                {
                    src: './src/umbraco-package.json',
                    dest: outputDir,
                },
            ],
        }),
    ],
});