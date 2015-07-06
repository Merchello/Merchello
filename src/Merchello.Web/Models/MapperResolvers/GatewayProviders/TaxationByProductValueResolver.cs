namespace Merchello.Web.Models.MapperResolvers.GatewayProviders
{
    using System.Collections.Generic;

    using AutoMapper;

    using Merchello.Core.Gateways;
    using Merchello.Core.Gateways.Taxation;
    using Merchello.Core.Models;
    using Merchello.Web.Models.ContentEditing;

    public class TaxationByProductValueResolver : ValueResolver<GatewayProviderBase, bool>
    {
        protected override bool ResolveCore(GatewayProviderBase source)
        {
            return source is ITaxationByProductProvider;
        }
    }
}