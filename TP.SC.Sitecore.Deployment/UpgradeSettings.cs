using Sitecore.Azure.Deployments;
using Sitecore.Azure.Deployments.AzureDeployments;
using Sitecore.Azure.Deployments.Environments;
using Sitecore.Azure.Deployments.Locations;
using Sitecore.Azure.Deployments.Roles;
using Sitecore.Azure.Deployments.Farms;
using Sitecore.Data.Items;

namespace TP.SC.Sitecore.Deployment
{
    public class UpgradeSettings
    {
        public UpgradeSettings(string deploymentSlotParam)
        {
            if (deploymentSlotParam.ToLower().Equals("production"))
            {
                AzureSlot = DeploymentSlot.Production;
            }
            AzureSlot = DeploymentSlot.Staging;
        }

        public Environment AzureEnvironment { get; set; }
        public Location AzureLocation { get; set; }
        public Farm AzureFarm { get; set; }
        public WebRole AzureRole { get; set; }
        public AzureDeployment AzureDeployment { get; set; }
        public DeploymentSlot AzureSlot { get; set; }
        public Item AzureEnvironmentItem { get; set; }
    }
}