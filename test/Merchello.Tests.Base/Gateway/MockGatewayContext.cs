using System;
using Merchello.Core;
using Merchello.Core.Gateway;
using Merchello.Core.Models;
using Merchello.Core.Models.GatewayProviders;
using Merchello.Core.Models.TypeFields;
using Merchello.Core.Services;

namespace Merchello.Tests.Base.Gateway
{
    public class MockGatewayContext
    {
        private readonly IRegisteredGatewayProviderService _registeredGatewayProviderService;
        private readonly IPaymentGatewayProviderRegistry _paymentGatewayProviderRegistry;
        //private readonly IShippingGatewayProvider _shippingGatewayProvider;
        //private readonly ITaxationGatewayProvider _taxationGatewayProvider;

        public MockGatewayContext(IRegisteredGatewayProviderService registeredGatewayProviderService)
            : this(registeredGatewayProviderService, null)
        { }

        public MockGatewayContext(IRegisteredGatewayProviderService registeredGatewayProviderService,
                                  IPaymentGatewayProviderRegistry paymentGatewayProviderRegistry)
        {
            _registeredGatewayProviderService = registeredGatewayProviderService;
            _paymentGatewayProviderRegistry = paymentGatewayProviderRegistry;
        }

        public IPaymentGatewayProviderRegistry PaymentProvider
        {
            get { return _paymentGatewayProviderRegistry; }
        }

        //public IShippingGatewayProvider ShippingProvider
        //{
        //    get { return _shippingGatewayProvider; }
        //}

        //public ITaxationGatewayProvider TaxationGatewayProvider
        //{
        //    get { return _taxationGatewayProvider; }
        //}

        #region scrap
        

        //public IPaymentGatewayProvider Instantiate(Guid providerKey)
        //{
        //    return null;
        //}

        //// TODO : TryGetInstance as an Attempt
        //private IGatewayProvider GetInstance(Guid providerKey)
        //{
        //    var registered = _registeredGatewayProviderService.GetByKey(providerKey);
        //    var registeredType = Type.GetType(registered.TypeFullName);

        //    if(registeredType == null) throw new InvalidOperationException("registeredType");

        //    var ctrArgs = new[] {typeof (IGatewayProvider)};

        //    IGatewayProvider providerValue;
        //    switch (registered.GatewayProviderType)
        //    {
        //        case GatewayProviderType.Payment:
        //            providerValue = _paymentGatewayProvider;
        //            break;
        //        default:
        //            throw new NotImplementedException("Custom providers are not yet implemented");
        //    }

        //    var ctrValue = new object[] {providerValue};

        //    var constructor = registeredType.GetConstructor(ctrArgs);
        //    return constructor.Invoke(ctrValue) as IGatewayProvider;

        //}



        //// this will require a certain known set of methods between all gateway provider types (or one method)
        //private IGatewayProvider GetInstance(Guid providerKey, IGatewayProviderStrategy gatewayProviderStrategy)
        //{
        //    return null;
        //}

        #endregion
    }

    public interface IPaymentGatewayProviderRegistry
    {
        ReceivePaymentStrategyBase GetInstance(Guid key);
    }



    #region cash payment


    // this is a registered gateway provider
    public class CashPayment : ReceiveCashPaymentStrategyBase
    {
        public CashPayment(IRegisteredGatewayProvider registration, ICustomer customer, decimal amount) 
            : base(registration, customer, amount)
        { }


        public override IPaymentGatewayResponse ReceivePayment()
        {
            return null;
            //return new PaymentGatewayResponse();
        }
    }

    public interface IPaymentGatewayProvider
    {

    }


    public class PaymentGatewayResponse : IPaymentGatewayResponse
    {
        public PaymentGatewayResponse(Guid providerKey, PaymentGatewayResponseType paymentGatewayResponseType)
        {
            ProviderKey = providerKey;
            ResponseType = paymentGatewayResponseType;
        }
        public PaymentGatewayResponseType ResponseType { get;  private set; }
        public Guid ProviderKey { get; private set; }
        public string ReferenceNumber { get; set; }
        public string PaymentMethodName { get; set; }
        public bool Authorized { get; set; }
        public bool Captured { get; set; }
    }

    public interface IPaymentGatewayResponse
    {

        PaymentGatewayResponseType ResponseType { get; }

        /// <summary>
        /// The Gateway Provider Key
        /// </summary>
        Guid ProviderKey { get; }

        /// <summary>
        /// The reference number 
        /// </summary>
        string ReferenceNumber { get; set; }

        /// <summary>
        /// The optional name of the payment method used by the provider
        /// </summary>
        string PaymentMethodName { get; set; }

        /// <summary>
        /// True/false indicating whether or not the payment is authorized
        /// </summary>
        bool Authorized { get; set; }

        /// <summary>
        /// True/false indicating whether or not the payment has been captured
        /// </summary>
        bool Captured { get; set; }
    }


    public enum PaymentGatewayResponseType
    {
        Approved,
        Declined,
        Error,
        Fraud
    }

    #endregion

#region ReceivePaymentBase


    public abstract class ReceiveCashPaymentStrategyBase : ReceivePaymentStrategyBase
    {
        protected ReceiveCashPaymentStrategyBase(IRegisteredGatewayProvider registration, ICustomer customer, decimal amount) 
            : base(registration, customer, PaymentMethodType.Cash, amount)
        {
        }
    }


    public abstract class ReceivePaymentStrategyBase : IReceivePaymentStrategy
    {
        private readonly IRegisteredGatewayProvider _registration;
        private readonly Guid _paymentTypeFieldKey;
        private readonly ICustomer _customer;
        private readonly decimal _amount;

        protected ReceivePaymentStrategyBase(IRegisteredGatewayProvider registration, ICustomer customer, PaymentMethodType paymentMethodType, decimal amount)
            : this(registration, customer, EnumTypeFieldConverter.PaymentMethod().GetTypeField(paymentMethodType).TypeKey, amount)
        { }


        internal ReceivePaymentStrategyBase(IRegisteredGatewayProvider registration, ICustomer customer, Guid paymentTypeFieldKey, decimal amount)
        {
            Mandate.ParameterNotNull(registration, "registration");
            Mandate.ParameterNotNull(customer, "customer");

            _registration = registration;
            _paymentTypeFieldKey = paymentTypeFieldKey;
            _customer = customer;
            _amount = amount;
        }

        /// <summary>
        /// Abstract method to invoke the CreatePaymentStrategy
        /// </summary>
        /// <returns></returns>
        public abstract IPaymentGatewayResponse ReceivePayment();

        /// <summary>
        /// The provider key
        /// </summary>
        public Guid ProviderKey
        {
            get { return _registration.Key; }
        }

        /// <summary>
        /// The customer
        /// </summary>
        public ICustomer Customer
        {
            get { return _customer; }
        }

        /// <summary>
        /// The amount
        /// </summary>
        public decimal Amount
        {
            get { return _amount; }
        }

        /// <summary>
        /// The typeFieldKey for the payment
        /// </summary>
        public Guid PaymentTypeFieldKey
        {
            get { return _paymentTypeFieldKey; }
        }

    }

    public interface IReceivePaymentStrategy
    {
        IPaymentGatewayResponse ReceivePayment();
        //IPaymentGatewayResponse RefundPayment();
    }

#endregion
}



