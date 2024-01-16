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
            CreateMap<ProductFeedSettingAddModel, UmbracoCommerceProductFeedSetting>(MemberList.Source)
                .ForMember(dest => dest.ProductPropertyNameMappings, opt => opt.MapFrom((src, dest) => JsonSerializer.Serialize(src.PropertyNameMappings)));
            CreateMap<UmbracoCommerceProductFeedSetting, ProductFeedSettingReadModel>()
                .ForMember(dest => dest.PropertyNameMappings, opt => opt.MapFrom((src, dest) => JsonSerializer.Deserialize<ICollection<PropertyValueMapping>>(src.ProductPropertyNameMappings)));
        }
    }
}
