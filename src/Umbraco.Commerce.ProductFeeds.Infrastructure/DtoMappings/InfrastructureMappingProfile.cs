using System.Text.Json;
using AutoMapper;
using Umbraco.Commerce.ProductFeeds.Core.Features.FeedSettings.Application;
using Umbraco.Commerce.ProductFeeds.Core.FeedSettings.Application;
using Umbraco.Commerce.ProductFeeds.Infrastructure.DbModels;

namespace Umbraco.Commerce.ProductFeeds.Infrastructure.DtoMappings
{
    public class InfrastructureMappingProfile : Profile
    {
        public InfrastructureMappingProfile()
        {
            CreateMap<ProductFeedSettingWriteModel, UmbracoCommerceProductFeedSetting>()
                .ForSourceMember(src => src.PropertyNameMappings, opt => opt.DoNotValidate())
                .ForMember(dest => dest.ProductPropertyNameMappings, opt => opt.MapFrom((src, dest) => JsonSerializer.Serialize(src.PropertyNameMappings)))
                .ForMember(dest => dest.ProductChildVariantTypeIds, opt => opt.MapFrom((src, dest) => string.Join(';', src.ProductChildVariantTypeIds)))
                .ForMember(dest => dest.ProductDocumentTypeIds, opt => opt.MapFrom((src, dest) => string.Join(';', src.ProductDocumentTypeIds)))

                // legacy mapping
                .ForMember(dest => dest.ProductDocumentTypeAliases, opt => opt.MapFrom((src, dest) => string.Join(';', src.ProductDocumentTypeAliases)));

            CreateMap<UmbracoCommerceProductFeedSetting, ProductFeedSettingReadModel>()
                .ForMember(dest => dest.PropertyNameMappings, opt => opt.MapFrom((src, dest) => JsonSerializer.Deserialize<ICollection<PropertyAndNodeMapItem>>(src.ProductPropertyNameMappings)))
                .ForMember(dest => dest.ProductChildVariantTypeIds, opt => opt.MapFrom((src, dest) => !string.IsNullOrEmpty(src.ProductChildVariantTypeIds) ? src.ProductChildVariantTypeIds?.Split(';') : []))
                .ForMember(dest => dest.ProductDocumentTypeIds, opt => opt.MapFrom((src, dest) => !string.IsNullOrEmpty(src.ProductDocumentTypeIds) ? src.ProductDocumentTypeIds.Split(';') : []))

                // legacy mapping
                .ForMember(dest => dest.ProductDocumentTypeAliases, opt => opt.MapFrom((src, dest) => !string.IsNullOrEmpty(src.ProductDocumentTypeAliases) ? src.ProductDocumentTypeAliases.Split(';') : []));
        }
    }
}
