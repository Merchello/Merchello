using Merchello.Web.Reporting;

namespace Merchello.Web
{
    using System;
    using System.Collections.Generic;

    using Merchello.Web.Mvc;
    using Merchello.Web.Trees;

    using Umbraco.Core;

    /// <summary>
    /// Extension methods for the Plugin Manager
    /// </summary>
    public static class PluginManagerExtensions
    {
        /// <summary>
        /// Returns all the resolved objects that sub-class <see cref="IPaymentMethodUiController"/>
        /// </summary>
        /// <param name="pluginManager">
        /// The <see cref="PluginManager"/>.
        /// </param>
        /// <returns>
        /// The collection of checkout operation controllers
        /// </returns>
        internal static IEnumerable<Type> ResolveCheckoutOperationControllers(this PluginManager pluginManager)
        {
            return pluginManager.ResolveTypes<IPaymentMethodUiController>();
        }

        /// <summary>
        /// Returns all available report data aggregators.
        /// </summary>
        /// <param name="pluginManger">
        /// The <see cref="PluginManager"/>.
        /// </param>
        /// <returns>
        /// The collection of the report data aggregators resolved.
        /// </returns>
        internal static IEnumerable<Type> ResolveReportApiControllers(this PluginManager pluginManger)
        {
            return pluginManger.ResolveTypesWithAttribute<ReportController, BackOfficeTreeAttribute>();
        } 
    }
}