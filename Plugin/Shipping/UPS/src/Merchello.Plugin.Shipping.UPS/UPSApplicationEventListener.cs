﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Umbraco.Core;
using Umbraco.Core.Events;
using Umbraco.Core.Logging;

namespace Merchello.Plugin.Shipping.UPS
{
    public class UPSEvents : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication,
                                              ApplicationContext applicationContext)
        {
            base.ApplicationStarted(umbracoApplication, applicationContext);

            LogHelper.Info<UPSEvents>("Initializing AuthorizeNet provider registration binding events");


            GatewayProviderService.Saving += delegate(IGatewayProviderService sender, SaveEventArgs<IGatewayProvider> args)
            {
                var key = new Guid("AEB14625-B9DE-4DE8-9C92-99204D340342");
                var provider = args.SavedEntities.FirstOrDefault(x => key == x.Key && !x.HasIdentity);
                if (provider == null) return;

                //provider.ExtendedData.SaveProcessorSettings(new AuthorizeNetProcessorSettings());

            };
        }
    }
}
