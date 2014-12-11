using System;
using System.Collections.Generic;
using Merchello.Core.Models;

namespace Merchello.Core.Strategies.Packaging
{
    using Umbraco.Core;

    public abstract class PackagingStrategyBase : IPackagingStrategy
    {

        protected readonly LineItemCollection LineItemCollection;
        protected readonly IAddress Destination;
        protected readonly IMerchelloContext MerchelloContext;
        protected Guid VersionKey { get; private set; }

        protected PackagingStrategyBase(LineItemCollection lineItemCollection, IAddress destination, Guid versionKey)
         : this(Core.MerchelloContext.Current, lineItemCollection, destination, versionKey)
        { }

        internal PackagingStrategyBase(IMerchelloContext merchelloContext, LineItemCollection lineItemCollection, IAddress destination, Guid versionKey)
        {
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");
            Mandate.ParameterNotNull(lineItemCollection, "lineItemCollection");
            Mandate.ParameterNotNull(destination, "destination");
            Mandate.ParameterCondition(!Guid.Empty.Equals(versionKey), "versionKey");

            MerchelloContext = merchelloContext;
            LineItemCollection = lineItemCollection;
            Destination = destination;
            VersionKey = versionKey;

        }

        public abstract IEnumerable<IShipment> PackageShipments();

    }
}