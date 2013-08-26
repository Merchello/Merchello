using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Events;
using Merchello.Core.Models;
using Merchello.Core.Services;

namespace Merchello.Core.ConversionStrategies
{
    internal class AnonymousCustomerBasketConversionStrategy : BaseAnonymousCustomerConversionStrategy, IAnonymousBasketConversionStrategy
    {
        private IAnonymousCustomer _anonymous;
        private ICustomer _customer;
        private ICustomerService _customerService;
        private IBasketService _basketService;

        public AnonymousCustomerBasketConversionStrategy(IAnonymousCustomer anonymous, ICustomer customer, ICustomerService customerService, IBasketService basketService) : base(customerService)
        {
            Mandate.ParameterNotNull(anonymous, "anonymous");
            Mandate.ParameterNotNull(customer, "customer");
            Mandate.ParameterNotNull(customerService, "customerService");
            Mandate.ParameterNotNull(basketService, "basketService");
            _anonymous = anonymous;
            _customer = customer;
            _customerService = customerService;
            _basketService = basketService;
        }

        public IBasket ConvertBasket()
        {
            var anonymousBasket = _basketService.GetByConsumer(_anonymous, BasketType.Basket);
            var customerBasket = _basketService.GetByConsumer(_customer, BasketType.Basket);

            Converting.RaiseEvent(new ConvertEventArgs<IBasket>(anonymousBasket), this);
            
            // empty the customer basket 
            if(customerBasket.IsEmpty() == false) _basketService.Empty(customerBasket);

            // 
            customerBasket.BasketItems = anonymousBasket.BasketItems;
            _basketService.Save(customerBasket);
            
            _basketService.Delete(anonymousBasket);
            Converted.RaiseEvent(new ConvertEventArgs<IBasket>(customerBasket), this);

            return customerBasket;
        }

        #region Events

        /// <summary>
        /// Occurs before Converting the anonymous customer's baskets to a customer basket
        /// </summary>
        public static event TypedEventHandler<AnonymousCustomerBasketConversionStrategy, ConvertEventArgs<IBasket>> Converting;

        /// <summary>
        /// Occurs after Converting the anonymous customer's basket to a customer basket
        /// </summary>
        public static event TypedEventHandler<AnonymousCustomerBasketConversionStrategy, ConvertEventArgs<IBasket>> Converted;

        #endregion
    }
}
