// @ts-check

import eslint from '@eslint/js';
import tseslint from 'typescript-eslint';
import globals from 'globals';

export default tseslint.config(
    eslint.configs.recommended,
    ...tseslint.configs.recommended,
    {
        languageOptions: {
            globals: {
                ...globals.node,
                ...globals.browser,
            },
        },
        files: ['**/*.ts', '**/*.js'],
        rules: {
            'comma-dangle': ['error', 'always-multiline'],
            'quotes': ['error', 'single'],
            'object-property-newline': 'error',
            'semi': ['error'],
            'no-console': 'warn',
        },
    },
);