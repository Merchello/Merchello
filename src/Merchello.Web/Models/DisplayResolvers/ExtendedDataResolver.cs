using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Merchello.Core.Models;

namespace Merchello.Web.Models.DisplayResolvers
{
    /// <summary>
    /// Custom AutoMapper Resolver - Maps <see cref="ExtendedDataCollection"/> to an Enumerable
    /// </summary>
    public class ExtendedDataResolver : ValueResolver<IGatewayProvider, IEnumerable<KeyValuePair<string, string>>>
    {
        protected override IEnumerable<KeyValuePair<string, string>> ResolveCore(IGatewayProvider source)
        {
            return source.ExtendedData.AsEnumerable();
        }
    }
}