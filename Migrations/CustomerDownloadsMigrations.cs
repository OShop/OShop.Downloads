using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;
using Orchard.Environment.Extensions;

namespace OShop.Downloads.Migrations {
    [OrchardFeature("OShop.CustomerDownloads")]
    public class CustomerDownloadsMigrations : DataMigrationImpl {

        public int Create() {
            ContentDefinitionManager.AlterPartDefinition("CustomerDownloadsPart", part => part
                .Attachable()
                .WithDescription("Allows customer to view his purchased product downloads.")
            );

            ContentDefinitionManager.AlterTypeDefinition("Customer", type => type
                .WithPart("CustomerDownloadsPart")
                .Creatable(false)
            );

            return 1;
        }
    }
}