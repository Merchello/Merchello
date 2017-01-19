namespace Merchello.Core.Strategies.Packaging
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// Represents a base packaging strategy.
    /// </summary>
    public abstract class PackagingStrategyBase : IPackagingStrategy
    {
        /// <summary>
        /// The line item collection.
        /// </summary>
        protected readonly LineItemCollection LineItemCollection;

        /// <summary>
        /// The destination.
        /// </summary>
        protected readonly IAddress Destination;

        /// <summary>
        /// The <see cref="IMerchelloContext"/>.
        /// </summary>
        protected readonly IMerchelloContext MerchelloContext;

        ///// <summary>
        ///// Initializes a new instance of the <see cref="PackagingStrategyBase"/> class.
        ///// </summary>
        ///// <param name="lineItemCollection">
        ///// The line item collection.
        ///// </param>
        ///// <param name="destination">
        ///// The destination.
        ///// </param>
        ///// <param name="versionKey">
        ///// The version key.
        ///// </param>
        //protected PackagingStrategyBase(LineItemCollection lineItemCollection, IAddress destination, Guid versionKey)
        //    : this(Core.MerchelloContext.Current, lineItemCollection, destination, versionKey)
        //{
        //}

        /// <summary>
        /// Initializes a new instance of the <see cref="PackagingStrategyBase"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="lineItemCollection">
        /// The line item collection.
        /// </param>
        /// <param name="destination">
        /// The destination.
        /// </param>
        /// <param name="versionKey">
        /// The version key.
        /// </param>
        protected PackagingStrategyBase(IMerchelloContext merchelloContext, LineItemCollection lineItemCollection, IAddress destination, Guid versionKey)
        {
            Ensure.ParameterNotNull(merchelloContext, "merchelloContext");
            Ensure.ParameterNotNull(lineItemCollection, "lineItemCollection");
            Ensure.ParameterNotNull(destination, "destination");
            Ensure.ParameterCondition(!Guid.Empty.Equals(versionKey), "versionKey");

            MerchelloContext = merchelloContext;
            LineItemCollection = lineItemCollection;
            Destination = destination;
            VersionKey = versionKey;

        }

        /// <summary>
        /// Gets the version key.
        /// </summary>
        protected Guid VersionKey { get; private set; }

        /// <summary>
        /// Packages a line item collection into shipments.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{IShipment}"/>.
        /// </returns>
        public abstract IEnumerable<IShipment> PackageShipments();
    }
}