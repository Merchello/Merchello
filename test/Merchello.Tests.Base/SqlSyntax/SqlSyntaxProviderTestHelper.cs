using Merchello.Core.Gateways.Notification;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Gateways.Taxation;
using Merchello.Core.Triggers;
using Umbraco.Core;
using Umbraco.Core.Persistence.SqlSyntax;

namespace Merchello.Tests.Base.SqlSyntax
{
    internal class SqlSyntaxProviderTestHelper
    {
        public static void EstablishSqlSyntax(DbSyntax syntax = DbSyntax.SqlCe)
        {
            if (Resolution.IsFrozen) return;
            SqlSyntaxContext.SqlSyntaxProvider = SqlSyntaxProvider(syntax);

            PaymentGatewayProviderResolver.Current = new PaymentGatewayProviderResolver(() => PluginManager.Current.ResolveTypes<PaymentGatewayProviderBase>());
            NotificationGatewayProviderResolver.Current = new NotificationGatewayProviderResolver(() => PluginManager.Current.ResolveTypes<NotificationGatewayProviderBase>());
            TaxationGatewayProviderResolver.Current = new TaxationGatewayProviderResolver(() => PluginManager.Current.ResolveTypes<TaxationGatewayProviderBase>());
            ShippingGatewayProviderResolver.Current = new ShippingGatewayProviderResolver(() => PluginManager.Current.ResolveTypes<ShippingGatewayProviderBase>());

            EventTriggerResolver.Current = new EventTriggerResolver(() => PluginManager.Current.ResolveTypes<IEventTrigger>());

            Resolution.Freeze();
        }

        public static ISqlSyntaxProvider SqlSyntaxProvider(DbSyntax syntax = DbSyntax.SqlCe)
        {
            return syntax == DbSyntax.SqlServer ? new SqlServerSyntaxProvider() : (ISqlSyntaxProvider)new SqlCeSyntaxProvider();
        }
    }
}