using AutoMapper;
using Umbraco.Commerce.ProductFeeds.Infrastructure.DtoMappings;

namespace Umbraco.Commerce.ProductFeeds.Infrastructure.UnitTests
{
    public class AutoMapperTests
    {
        [Fact]
        public void Mapping_Configuration_Should_Be_Valid()
        {
            MapperConfiguration config = new(cfg => cfg.AddMaps(typeof(InfrastructureMappingProfile)));
            config.AssertConfigurationIsValid();
        }
    }
}
