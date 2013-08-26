using Merchello.Core.Events;
using Merchello.Core.Models;
using Merchello.Core.Services;

namespace Merchello.Core.ConversionStrategies
{
    /// <summary>
    /// Strategy to convert an anonymous customer into a new customer
    /// </summary>
    public class AnonymousCustomerToNewCustomerStrategy : BaseAnonymousCustomerConversionStrategy, IAnonymousCustomerConversionStrategy
    {

        private readonly IAnonymousCustomer _anonymous;
        private readonly string _firstName;
        private readonly string _lastName;
        private readonly string _email;
        
        
        public AnonymousCustomerToNewCustomerStrategy(IAnonymousCustomer anonymous, string firstName, string lastName, string email, ICustomerService customerService)
            : base(customerService)
        {
            _anonymous = anonymous;
            _firstName = firstName;
            _lastName = lastName;
            _email = email;
          
        }

        public ICustomer ConvertToCustomer()
        {

            Converting.RaiseEvent(new ConvertEventArgs<IAnonymousCustomer>(_anonymous), this);

            var customer = CustomerService.CreateCustomer(_firstName, _lastName, _email);
            customer.Key = _anonymous.Key;
            CustomerService.Save(customer);

            Converted.RaiseEvent(new ConvertEventArgs<ICustomer>(customer), this);

            return customer;
        }

        
        #region Events

        /// <summary>
        /// Occurs before Converting anonymous users to customer
        /// </summary>
        public static event TypedEventHandler<AnonymousCustomerToNewCustomerStrategy, ConvertEventArgs<IAnonymousCustomer>> Converting;

        /// <summary>
        /// Occurs after Converting anonymous users to customer
        /// </summary>
        public static event TypedEventHandler<AnonymousCustomerToNewCustomerStrategy, ConvertEventArgs<ICustomer>> Converted;

        #endregion
    }
}
