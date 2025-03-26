using Umbraco.Cms.Infrastructure.Migrations;

namespace Umbraco.Commerce.ProductFeeds.Infrastructure.Migrations
{
    /// <summary>
    /// This is a dummy migration step to recover a missed state '13.1.0' of the migration plan.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "<Pending>")]
    internal class DummyMigrationStep_13_1_0 : MigrationBase
    {
        public DummyMigrationStep_13_1_0(IMigrationContext context)
            : base(context)
        {
        }

        protected override void Migrate()
        {
        }
    }
}
