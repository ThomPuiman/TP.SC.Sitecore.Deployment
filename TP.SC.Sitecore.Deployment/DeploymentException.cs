using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Exceptions;

namespace TP.SC.Sitecore.Deployment
{
    public class EnvironmentException : SitecoreException
    {
        public EnvironmentException(string message)
        : base(message)
        {
        }
    }

    public class LocationException : SitecoreException
    {
    }

    public class DeployAccessDenied : SitecoreException
    {
    }

    public class AzureModuleException : SitecoreException
    {
        public AzureModuleException(string message)
        : base(message)
        {
        }
    }
}