using System;
using System.Reflection;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Represents a payment method
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    internal class PaymentMethod : Entity, IPaymentMethod
    {
        private readonly Guid _providerKey;
        private string _name;
        private string _description;
        private string _paymentCode;

        private static readonly PropertyInfo NameSelector = ExpressionHelper.GetPropertyInfo<PaymentMethod, string>(x => x.Name);
        private static readonly PropertyInfo DescriptionSelector = ExpressionHelper.GetPropertyInfo<PaymentMethod, string>(x => x.Description);
        private static readonly PropertyInfo PaymentCodeSelector = ExpressionHelper.GetPropertyInfo<PaymentMethod, string>(x => x.PaymentCode);

        internal PaymentMethod(Guid providerKey)
        {
            Mandate.ParameterCondition(!Guid.Empty.Equals(providerKey), "providerKey");
            _providerKey = providerKey;
        }


        /// <summary>
        /// The key associated with the gateway provider for the payment method
        /// </summary>
        [DataMember]
        public Guid ProviderKey
        {
            get { return _providerKey; }         
        }

        /// <summary>
        /// The name assoicated with the payment method
        /// </summary>
        [DataMember]
        public string Name
        {
            get { return _name; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _name = value;
                    return _name;
                }, _name, NameSelector);
            }
        }

        /// <summary>
        /// The description of the payment method
        /// </summary>
        [DataMember]
        public string Description
        {
            get { return _description; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _description = value;
                    return _description;
                }, _description, DescriptionSelector);
            }
        }

        /// <summary>
        /// The payment code of the payment method
        /// </summary>
        [DataMember]
        public string PaymentCode
        {
            get { return _paymentCode; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _paymentCode = value;
                    return _paymentCode;
                }, _paymentCode, PaymentCodeSelector);
            }
        }
    }
}