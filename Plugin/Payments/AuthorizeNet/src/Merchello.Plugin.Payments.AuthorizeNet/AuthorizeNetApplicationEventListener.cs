using System;
using System.Linq;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Plugin.Payments.AuthorizeNet.Models;
using Umbraco.Core;
using Umbraco.Core.Events;
using Umbraco.Core.Logging;

namespace Merchello.Plugin.Payments.AuthorizeNet
{
    public class AuthorizeNetEvents : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication,
                                                   ApplicationContext applicationContext)
        {
            base.ApplicationStarted(umbracoApplication, applicationContext);

            LogHelper.Info<AuthorizeNetEvents>("Initializing AuthorizeNet provider registration binding events");


            GatewayProviderService.Saving += delegate(IGatewayProviderService sender, SaveEventArgs<IGatewayProvider> args)
            {
                    var key = new Guid("C6BF6743-3565-401F-911A-33B68CACB11B");
                    var provider =  args.SavedEntities.FirstOrDefault(x => key == x.Key && !x.HasIdentity);
                    if(provider == null) return;
                    
                    provider.ExtendedData.SaveProcessorSettings(new AuthorizeNetProcessorSettings());
                    
            };
        }
    }
}