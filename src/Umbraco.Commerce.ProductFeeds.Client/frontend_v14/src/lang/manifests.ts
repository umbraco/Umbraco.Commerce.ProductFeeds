const localizationManifests = [
    {
        type: 'localization',
        alias: 'UcPproductFeeds.Localization.En',
        weight: -100,
        name: 'English',
        meta: {
            culture: 'en',
        },
        js: () => import('./en.js'),
    },
];
export const manifests = [...localizationManifests];
