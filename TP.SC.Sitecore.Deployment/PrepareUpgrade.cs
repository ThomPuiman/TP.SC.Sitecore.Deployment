using System.Linq;
using Sitecore.Azure.Configuration;
using Sitecore.Azure.Deployments.Environments;
using Sitecore.Common;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace TP.SC.Sitecore.Deployment
{
    public class PrepareUpgrade
    {
        private Item AzureRootItem { get; set; }
        public UpgradeSettings PrepareUpgradeSettings { get; set; }

        public PrepareUpgrade(UpgradeSettings settings)
        {
            PrepareUpgradeSettings = settings;
            var db = Database.GetDatabase("master");
            AzureRootItem = db.GetItem("/sitecore/system/Modules/Azure");

            if (AzureRootItem == null)
            {
                throw new AzureModuleException("Root item of Sitecore Azure module not found.");
            }
        }

        public PrepareUpgrade SetEnvironment()
        {
            PrepareUpgradeSettings.AzureEnvironmentItem = AzureRootItem.Children.FirstOrDefault(o => !string.IsNullOrWhiteSpace(o["Environment Id"]));

            if (PrepareUpgradeSettings.AzureEnvironmentItem == null)
            {
                throw new EnvironmentException("The environment item within the Sitecore Azure module isn't found.");
            }

            var environmentDefinition = Settings.EnvironmentDefinitions.FirstOrDefault(o => o.EnvironmentId.ToID().Equals(ID.Parse(PrepareUpgradeSettings.AzureEnvironmentItem["Environment Id"])));
            PrepareUpgradeSettings.AzureEnvironment = Environment.GetEnvironment(environmentDefinition);

            return this;
        }

        public PrepareUpgrade SetLocation()
        {
            var locationItem = PrepareUpgradeSettings.AzureEnvironmentItem.Children.FirstOrDefault(o => o.TemplateName == "Location");

            if (locationItem == null)
            {
                throw new LocationException();
            }

            PrepareUpgradeSettings.AzureLocation = PrepareUpgradeSettings.AzureEnvironment.Locations.FirstOrDefault(o => o.Name == locationItem.DisplayName);
            return this;
        }
    }
}