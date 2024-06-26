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
            CreateMap<ProductFeedSettingWriteModel, UmbracoCommerceProductFeedSetting>(MemberList.Source)
                .ForSourceMember(src => src.PropertyNameMappings, opt => opt.DoNotValidate())
                .ForMember(dest => dest.ProductPropertyNameMappings, opt => opt.MapFrom((src, dest) => JsonSerializer.Serialize(src.PropertyNameMappings)))
                .ForSourceMember(src => src.ProductDocumentTypeAliases, opt => opt.DoNotValidate())
                .ForMember(dest => dest.ProductDocumentTypeAliases, opt => opt.MapFrom((src, dest) => string.Join(';', src.ProductDocumentTypeAliases)));

            CreateMap<UmbracoCommerceProductFeedSetting, ProductFeedSettingReadModel>()
                .ForMember(dest => dest.PropertyNameMappings, opt => opt.MapFrom((src, dest) => JsonSerializer.Deserialize<ICollection<PropertyValueMapping>>(src.ProductPropertyNameMappings)))
                .ForMember(dest => dest.ProductDocumentTypeAliases, opt => opt.MapFrom((src, dest) => src.ProductDocumentTypeAliases.Split(';')));
        }
    }
}
