using Merchello.Core.Events;
using Merchello.Core.Models;
using Merchello.Core.Services;

namespace Merchello.Core.ConversionStrategies
{
    public class AnonymousCustomerToExistingCustomerStrategy : BaseAnonymousCustomerConversionStrategy, IAnonymousCustomerConversionStrategy
    {
        private IAnonymousCustomer _anonymous;
        private int _memberId;

        public AnonymousCustomerToExistingCustomerStrategy(IAnonymousCustomer anonymous, int memberId, ICustomerService customerService) 
            : base(customerService)
        {
            Mandate.ParameterNotNull(anonymous, "anonymous");
         
            _anonymous = anonymous;
            _memberId = memberId;
        }

        public ICustomer ConvertToCustomer()
        {
            Converting.RaiseEvent(new ConvertEventArgs<IAnonymousCustomer>(_anonymous), this);

            var customer = CustomerService.GetByMemberId(_memberId);
            
            // TODO : RSS convert the basket

            Converted.RaiseEvent(new ConvertEventArgs<ICustomer>(customer), this);

            return customer;
        }

        #region Events

        /// <summary>
        /// Occurs before Converting anonymous users to customer
        /// </summary>
        public static event TypedEventHandler<AnonymousCustomerToExistingCustomerStrategy, ConvertEventArgs<IAnonymousCustomer>> Converting;

        /// <summary>
        /// Occurs after Converting anonymous users to customer
        /// </summary>
        public static event TypedEventHandler<AnonymousCustomerToExistingCustomerStrategy, ConvertEventArgs<ICustomer>> Converted;

        #endregion
    }
}
