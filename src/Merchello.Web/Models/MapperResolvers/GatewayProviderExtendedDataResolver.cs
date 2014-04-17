using System.Collections.Generic;
using AutoMapper;
using Merchello.Core.Models;

namespace Merchello.Web.Models.MapperResolvers
{
    /// <summary>
    /// Custom AutoMapper Resolver - Maps <see cref="ExtendedDataCollection"/> to an Enumerable
    /// </summary>
    public class GatewayProviderExtendedDataResolver : ValueResolver<IGatewayProvider, IEnumerable<KeyValuePair<string, string>>>
    {
        protected override IEnumerable<KeyValuePair<string, string>> ResolveCore(IGatewayProvider source)
        {
            return source.ExtendedData.AsEnumerable();
        }
    }
}