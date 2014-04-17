using System.Collections.Generic;
using AutoMapper;
using Merchello.Core.Models;

namespace Merchello.Web.Models.MapperResolvers
{
    /// <summary>
    /// Custom AutoMapper Resolver - Maps <see cref="ExtendedDataCollection"/> to an Enumerable
    /// </summary>
    public class ExtendedDataResolver : ValueResolver<IHasExtendedData, IEnumerable<KeyValuePair<string, string>>>
    {
        protected override IEnumerable<KeyValuePair<string, string>> ResolveCore(IHasExtendedData source)
        {
            return source.ExtendedData.AsEnumerable();
        }
    }
}