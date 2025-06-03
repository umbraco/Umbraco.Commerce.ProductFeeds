using AutoMapper;
using Umbraco.Commerce.ProductFeeds.Infrastructure.DtoMappings;

namespace Umbraco.Commerce.ProductFeeds.Infrastructure.UnitTests
{
#pragma warning disable CA1515 // Consider making public types internal
    public class AutoMapperTests
#pragma warning restore CA1515 // Consider making public types internal
    {
        [Fact]
        public void Mapping_Configuration_Should_Be_Valid()
        {
            MapperConfiguration config = new(cfg => cfg.AddMaps(typeof(InfrastructureMappingProfile)));
            config.AssertConfigurationIsValid();
        }
    }
}
