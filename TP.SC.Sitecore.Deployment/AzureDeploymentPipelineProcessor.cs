using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using Sitecore.Azure.Deployments.Farms;
using Sitecore.Azure.ExtentionMethods;
using Sitecore.Azure.Managers.AzureManagers;
using Sitecore.Azure.Sys.Collections.Generic;
using Sitecore.Azure.Sys.Logging;
using Sitecore.Configuration;
using Sitecore.Pipelines.HttpRequest;

namespace TP.SC.Sitecore.Deployment
{
    public class DeploymentPipelineProcessor : HttpRequestProcessor
    {
        private readonly string _activationUrl;
        private readonly string _deployCDFarm;
        private readonly string _deployCDRole;
        private readonly string _deployCEFarm;
        private readonly string _deployCERole;

        public DeploymentPipelineProcessor(string activationUrl, string deployCDFarm, string deployCDRole, string deployCEFarm, string deployCERole)
        {
            _activationUrl = activationUrl;
            _deployCDFarm = deployCDFarm;
            _deployCDRole = deployCDRole;
            _deployCEFarm = deployCEFarm;
            _deployCERole = deployCERole;
        }


        public override void Process(HttpRequestArgs args)
        {
            if (string.IsNullOrWhiteSpace(_activationUrl)) return;

            if (args.Context.Request.RawUrl.StartsWith(_activationUrl, StringComparison.OrdinalIgnoreCase))
            {
                ProcessRequest(args.Context);
                args.Context.Response.End();
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            context.Server.ScriptTimeout = 86400;
            var slot = context.Request.QueryString["slot"];
            var ff = context.Request.QueryString["fireandforget"];
            var accessToken = context.Request.QueryString["token"];
            if (!accessToken.Equals(Settings.GetSetting("AzureDeploymentHandlerToken")))
            {
                throw new DeployAccessDenied();
            }

            HttpContext.Current.Response.Write("\r\n" + DateTime.Now.ToString("R") + ": Starting Upgrade ");

            var scUpgradeSettings = new UpgradeSettings(slot);
            (new PrepareUpgrade(scUpgradeSettings)).SetEnvironment().SetLocation();
            var scCDDeployment = (new ProcessUpgrade(scUpgradeSettings)).DoDeployment(_deployCDFarm, _deployCDRole, DeploymentType.ContentDelivery);
            AzureDeploymentManager.Current.UpgradeDeploymentAsync(scCDDeployment);
            var scCEDeployment = (new ProcessUpgrade(scUpgradeSettings)).DoDeployment(_deployCEFarm, _deployCERole, DeploymentType.ContentEditing);;
            AzureDeploymentManager.Current.UpgradeDeploymentAsync(scCEDeployment);

            if (ff.ToLower().Equals("true"))
            {
                Thread.Sleep(2000);
                List<LogMessage> scLogs = new List<LogMessage>();
                while (scCDDeployment.IsBusy() || scCEDeployment.IsBusy())
                {
                    IEnumerable<LogMessage> tmpMsgs = null;
                    if (scLogs.IsEmpty())
                    {
                        tmpMsgs = Log.GetMessages(0);
                        scLogs.AddRange(tmpMsgs);
                    }
                    else
                    {
                        tmpMsgs = Log.GetMessages(scLogs.Last().MessageId);
                        scLogs.AddRange(tmpMsgs);
                    }

                    foreach (LogMessage msg in tmpMsgs)
                    {
                        HttpContext.Current.Response.Write("\r\n" + DateTime.Now.ToString("R") + ": " + string.Format("Sitecore Azure log: {0}", string.Format(msg.MessageText, msg.Parameters)));
                    }
                    Thread.Sleep(1000);
                }
            }
        }
    }
}