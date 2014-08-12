using Merchello.Web.Reporting;

namespace Merchello.Web
{
    using System;
    using System.Collections.Generic;
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
        internal static IEnumerable<Type> ResolveReportApiControllers(this PluginManager pluginManger)
        {
            return pluginManger.ResolveTypes<ReportApiControllerBase>();
        } 
    }
}