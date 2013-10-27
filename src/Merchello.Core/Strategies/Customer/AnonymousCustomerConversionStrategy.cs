using Merchello.Core.ConversionStrategies;
using Merchello.Core.Events;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Umbraco.Core.Events;

namespace Merchello.Core.Strategies.Customer
{
    /// <summary>
    /// Strategy to convert an anonymous customer into a new customer
    /// </summary>
    public class AnonymousCustomerConversionStrategy : BaseAnonymousCustomerConversionStrategy, IAnonymousCustomerConversionStrategy
    {

        private readonly IAnonymousCustomer _anonymous;
        private readonly string _firstName;
        private readonly string _lastName;
        private readonly string _email;
        
        
        public AnonymousCustomerConversionStrategy(IAnonymousCustomer anonymous, string firstName, string lastName, string email, ICustomerService customerService)
            : base(customerService)
        {
            _anonymous = anonymous;
            _firstName = firstName;
            _lastName = lastName;
            _email = email;
          
        }

        public ICustomer ConvertToCustomer()
        {

            Converting.RaiseEvent(new ConvertEventArgs<IAnonymousCustomer>(_anonymous), this) ;

            var customer = CustomerService.CreateCustomer(_firstName, _lastName, _email);
            customer.EntityKey = _anonymous.Key;
            CustomerService.Save(customer);

            Converted.RaiseEvent(new ConvertEventArgs<ICustomer>(customer), this);

            return customer;
        }

        public IItemCache ConvertBasket()
        {
            throw new System.NotImplementedException();
        }

        #region Events

        /// <summary>
        /// Occurs before Converting anonymous users to customer
        /// </summary>
        public static event TypedEventHandler<AnonymousCustomerConversionStrategy, ConvertEventArgs<IAnonymousCustomer>> Converting;

        /// <summary>
        /// Occurs after Converting anonymous users to customer
        /// </summary>
        public static event TypedEventHandler<AnonymousCustomerConversionStrategy, ConvertEventArgs<ICustomer>> Converted;

        #endregion
    }
}
