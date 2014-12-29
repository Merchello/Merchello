namespace Merchello.Web.Models.MapperResolvers
{
    using System.Collections.Generic;
    using AutoMapper;
    using Core.Models;

    /// <summary>
    /// Custom AutoMapper Resolver - Maps <see cref="ExtendedDataCollection"/> to an Enumerable
    /// </summary>
    /// <remarks>
    /// 
    /// Fixes issue M-211 http://issues.merchello.com/youtrack/issue/M-211
    /// 
    /// </remarks>
    public class ExtendedDataResolver : ValueResolver<IHasExtendedData, IEnumerable<KeyValuePair<string, string>>>
    {
        /// <summary>
        /// The resolve core.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// A collection of key value pairs that represent an <see cref="ExtendedDataCollection"/>
        /// </returns>
        protected override IEnumerable<KeyValuePair<string, string>> ResolveCore(IHasExtendedData source)
        {
            return source.ExtendedData.AsEnumerable();
        }
    }
}