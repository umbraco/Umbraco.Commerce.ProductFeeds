namespace Umbraco.Commerce.ProductFeeds.Core.PropertyValueExtractors.Implementations
{
    public class PropertyExtractorNameTypeMapping
    {
        private readonly Dictionary<string, Type> _nameTypeDict = new()
        {
            { nameof(DefaultSingleValuePropertyExtractor), typeof(DefaultSingleValuePropertyExtractor) },
            { nameof(DefaultMultipleMediaPickerPropertyValueExtractor), typeof(DefaultMultipleMediaPickerPropertyValueExtractor) },
        };

        public void AddNameTypeMap(string extractorName, Type type)
        {
            _nameTypeDict.Add(extractorName, type);
        }

        public Type GetExtractorType(string extractorName)
        {
            return _nameTypeDict[extractorName];
        }
    }
}
