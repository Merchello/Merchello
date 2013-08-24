using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Events;
using Merchello.Core.Models;
using Merchello.Core.Persistence;
using Merchello.Core.Services;
using Umbraco.Core.Persistence.UnitOfWork;

namespace Merchello.Core.CustomerConversion
{
    /// <summary>
    /// Strategy to convert an anonymous customer into a new customer
    /// </summary>
    public class AnonymousToNewCustomerStrategy : BaseAnonymousConversionStrategy
    {

        private readonly IAnonymousCustomer _anonymous;
        private readonly string _firstName;
        private readonly string _lastName;
        private readonly string _email;
        
        
        /// <summary>
        /// 
        /// </summary>
        internal AnonymousToNewCustomerStrategy(IAnonymousCustomer anonymous, string firstName, string lastName, string email)
            : this(anonymous,firstName,lastName,email, new CustomerService(new PetaPocoUnitOfWorkProvider(), new RepositoryFactory()))
        {}

        public AnonymousToNewCustomerStrategy(IAnonymousCustomer anonymous, string firstName, string lastName, string email, ICustomerService customerService)
            : base(customerService)
        {
            _anonymous = anonymous;
            _firstName = firstName;
            _lastName = lastName;
            _email = email;
          
        }

        public override ICustomer ConvertToCustomer()
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
        public static event TypedEventHandler<AnonymousToNewCustomerStrategy, ConvertEventArgs<IAnonymousCustomer>> Converting;

        /// <summary>
        /// Occurs after Converting anonymous users to customer
        /// </summary>
        public static event TypedEventHandler<AnonymousToNewCustomerStrategy, ConvertEventArgs<ICustomer>> Converted;

        #endregion
    }
}
