using System;
using Merchello.Core.Gateways;
using Merchello.Core.Gateways.Notification;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Gateways.Taxation;
using Merchello.Core.ObjectResolution;
using Umbraco.Core;
using Umbraco.Core.Persistence.SqlSyntax;

namespace Merchello.Tests.Base.SqlSyntax
{
    internal class SqlSyntaxProviderTestHelper
    {
        public static void EstablishSqlSyntax(DbSyntax syntax = DbSyntax.SqlCe)
        {
            try
            {
                var syntaxtest = SqlSyntaxContext.SqlSyntaxProvider ;
            }
            catch (Exception)
            {

                SqlSyntaxContext.SqlSyntaxProvider = SqlSyntaxProvider(syntax);
            }
            //if (Resolution.IsFrozen) return;
           

            //PaymentGatewayProviderResolver.Current = new PaymentGatewayProviderResolver(() => PluginManager.Current.ResolveTypesWithAttribute<PaymentGatewayProviderBase, GatewayProviderActivationAttribute>());
            //NotificationGatewayProviderResolver.Current = new NotificationGatewayProviderResolver(() => PluginManager.Current.ResolveTypesWithAttribute<NotificationGatewayProviderBase, GatewayProviderActivationAttribute>());
            //TaxationGatewayProviderResolver.Current = new TaxationGatewayProviderResolver(() => PluginManager.Current.ResolveTypesWithAttribute<TaxationGatewayProviderBase, GatewayProviderActivationAttribute>());
            //ShippingGatewayProviderResolver.Current = new ShippingGatewayProviderResolver(() => PluginManager.Current.ResolveTypesWithAttribute<ShippingGatewayProviderBase, GatewayProviderActivationAttribute>());

            //if(!EventTriggerRegistry.IsInitialized)
            //EventTriggerRegistry.Current =
            //    new EventTriggerRegistry(() => PluginManager.Current.ResolveTypesWithAttribute<IEventTriggeredAction, EventTriggeredActionForAttribute>());

            //Resolution.Freeze();
        }

        public static ISqlSyntaxProvider SqlSyntaxProvider(DbSyntax syntax = DbSyntax.SqlCe)
        {
            return syntax == DbSyntax.SqlServer ? new SqlServerSyntaxProvider() : (ISqlSyntaxProvider)new SqlCeSyntaxProvider();
        }
    }
}