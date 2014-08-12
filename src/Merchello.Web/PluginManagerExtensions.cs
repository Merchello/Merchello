namespace Merchello.Web
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Reporting;

    using Umbraco.Core;

    public static class PluginManagerExtensions
    {
        /// <summary>
        /// Returns all available report data aggregators.
        /// </summary>
        /// <param name="pluginManger">
        /// The plugin manger.
        /// </param>
        /// <returns>
        /// The collection of the report data aggregators resolved.
        /// </returns>
        internal static IEnumerable<Type> ResolveReportDataAggregators(this PluginManager pluginManger)
        {
            return pluginManger.ResolveAttributedTypes<ReportDataAggregatorAttribute>();
        } 
    }
}