using Sitecore.Azure.Deployments.AzureDeployments;
using Sitecore.Azure.Deployments.Farms;

namespace TP.SC.Sitecore.Deployment
{
    public class ProcessUpgrade
    {
        private UpgradeSettings _upgradeSettings { get; set; }

        public ProcessUpgrade(UpgradeSettings settings)
        {
            _upgradeSettings = settings;
        }

        public AzureDeployment DoDeployment(string farm, string role, DeploymentType type)
        {
            var scFarm = _upgradeSettings.AzureLocation.GetFarm(farm, type);
            var scRole = scFarm.GetWebRole(role);
            var deployment = scRole.GetDeployment(_upgradeSettings.AzureSlot);
            deployment.Update();
            return deployment;
        }
    }
}