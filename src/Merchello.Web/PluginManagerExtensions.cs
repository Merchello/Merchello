using Merchello.Web.Reporting;

namespace Merchello.Web
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Marketing.Offer;
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
        internal static IEnumerable<Type> ResolvePaymentMethodUiControllers(this PluginManager pluginManager)
        {
            return pluginManager.ResolveTypes<IPaymentMethodUiController>();
        }

        /// <summary>
        /// Returns all <see cref="OfferComponentBase"/> types.
        /// </summary>
        /// <param name="pluginManager">
        /// The plugin manager.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{Type}"/>.
        /// </returns>
        internal static IEnumerable<Type> ResolveOfferComponents(this PluginManager pluginManager)
        {
            return pluginManager.ResolveTypesWithAttribute<OfferComponentBase, OfferComponentAttribute>();
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
            return pluginManger.ResolveTypes<ReportController>();
        }


        /// <summary>
        /// Resolves any <see cref="OfferManagerBase{TOffer}"/> types.
        /// </summary>
        /// <param name="pluginManager">
        /// The <see cref="PluginManager"/>.
        /// </param>
        /// <returns>
        /// The collection of <see cref="OfferManagerBase{TOffer}"/>.
        /// </returns>
        internal static IEnumerable<Type> ResolveOfferProviders(this PluginManager pluginManager)
        {
            return pluginManager.ResolveTypesWithAttribute<IOfferProvider, BackOfficeTreeAttribute>();
        }
    }
}