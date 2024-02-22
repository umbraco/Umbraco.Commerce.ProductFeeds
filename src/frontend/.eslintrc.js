module.exports = {
  'env': {
    'browser': true,
    'commonjs': true,
    'es2021': true,
    'node': true,
  },
  'overrides': [
    {
      'files': [
        '.eslintrc.{js,cjs}',
      ],
    },
  ],
  'parserOptions': {
    'sourceType': 'module',
  },
  'rules': {
    'comma-dangle': ['error', 'always-multiline'],
    'semi': ['error', 'always'],
    'no-var': 'error',
    'no-undef': 'error',
    'quotes': ['error', 'single'],
    'no-console': 'warn',
    'no-debugger': 'warn',
    'no-use-before-define': 'error',
  },
  'globals': {
    'angular': 'readonly',
    'Umbraco': 'readonly',
  },
};
