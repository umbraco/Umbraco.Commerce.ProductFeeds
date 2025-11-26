using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using Umbraco.Commerce.ProductFeeds.Infrastructure.DtoMappings;

namespace Umbraco.Commerce.ProductFeeds.Infrastructure.UnitTests
{
#pragma warning disable CA1515 // Consider making public types internal
    public class AutoMapperTests
#pragma warning restore CA1515 // Consider making public types internal
    {
        public AutoMapperTests()
        {
        }

        [Fact]
        public void Mapping_Configuration_Should_Be_Valid()
        {
            var config = new MapperConfiguration(cfg => cfg.AddMaps(typeof(InfrastructureMappingProfile)), NullLoggerFactory.Instance);
            config.AssertConfigurationIsValid();
        }
    }
}
