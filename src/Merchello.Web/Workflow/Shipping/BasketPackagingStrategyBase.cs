using System.Collections.Generic;
using Merchello.Core;
using Merchello.Core.Models;

namespace Merchello.Web.Workflow.Shipping
{
    public abstract class BasketPackagingStrategyBase : IBasketPackagingStrategy
    {

        protected readonly IBasket Basket;
        protected readonly IAddress Destination;
        protected readonly IMerchelloContext MerchelloContext;

        protected BasketPackagingStrategyBase(IBasket basket, IAddress destination)
         : this(Core.MerchelloContext.Current, basket, destination)
        { }

        internal BasketPackagingStrategyBase(IMerchelloContext merchelloContext, IBasket basket, IAddress destination)
        {
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");
            Mandate.ParameterNotNull(basket, "basket");
            Mandate.ParameterNotNull(destination, "destination");

            MerchelloContext = merchelloContext;
            Basket = basket;
            Destination = destination;

        }

        public abstract IEnumerable<IShipment> PackageShipments();

    }
}