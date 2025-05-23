using System.Text.Json;
using AutoMapper;
using Umbraco.Commerce.ProductFeeds.Core.Features.FeedSettings.Application;
using Umbraco.Commerce.ProductFeeds.Infrastructure.DbModels;

namespace Umbraco.Commerce.ProductFeeds.Infrastructure.DtoMappings
{
    public class InfrastructureMappingProfile : Profile
    {
        public InfrastructureMappingProfile()
        {
            CreateMap<ProductFeedSettingWriteModel, UmbracoCommerceProductFeedSetting>()
                .ForSourceMember(src => src.PropertyNameMappings, opt => opt.DoNotValidate())
                .ForMember(dest => dest.FeedType, opt => opt.MapFrom((src, dest) => src.FeedGeneratorId)) // TODO: Rename FeedType to FeedGeneratorId or similar
                .ForMember(dest => dest.ProductPropertyNameMappings, opt => opt.MapFrom((src, dest) => JsonSerializer.Serialize(src.PropertyNameMappings)))
                .ForMember(dest => dest.ProductChildVariantTypeIds, opt => opt.MapFrom((src, dest) => string.Join(';', src.ProductChildVariantTypeIds)))
                .ForMember(dest => dest.ProductDocumentTypeIds, opt => opt.MapFrom((src, dest) => string.Join(';', src.ProductDocumentTypeIds)));

            CreateMap<UmbracoCommerceProductFeedSetting, ProductFeedSettingReadModel>()
                .ForMember(dest => dest.FeedGeneratorId, opt => opt.MapFrom((src, dest) => src.FeedType)) // TODO: Rename FeedType to FeedGeneratorId or similar
                .ForMember(dest => dest.PropertyNameMappings, opt => opt.MapFrom((src, dest) => JsonSerializer.Deserialize<ICollection<PropertyAndNodeMapItem>>(src.ProductPropertyNameMappings)))
                .ForMember(dest => dest.ProductChildVariantTypeIds, opt => opt.MapFrom((src, dest) => !string.IsNullOrEmpty(src.ProductChildVariantTypeIds) ? src.ProductChildVariantTypeIds?.Split(';') : []))
                .ForMember(dest => dest.ProductDocumentTypeIds, opt => opt.MapFrom((src, dest) => !string.IsNullOrEmpty(src.ProductDocumentTypeIds) ? src.ProductDocumentTypeIds.Split(';') : []));
        }
    }
}
