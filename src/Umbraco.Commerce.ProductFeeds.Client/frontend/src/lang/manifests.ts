const localizationManifests = [
    {
        type: 'localization',
        alias: 'UcProductFeeds.Localization.En',
        weight: -100,
        name: 'English',
        meta: {
            culture: 'en',
        },
        js: () => import('./en.js'),
    },
];
export const manifests = [...localizationManifests];
