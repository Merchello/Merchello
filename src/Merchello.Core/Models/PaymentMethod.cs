namespace Merchello.Core.Models
{
    using System;
    using System.Reflection;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Represents a payment method
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    internal class PaymentMethod : Entity, IPaymentMethod
    {
        /// <summary>
        /// The property selectors.
        /// </summary>
        private static readonly Lazy<PropertySelectors> _ps = new Lazy<PropertySelectors>();

        /// <summary>
        /// The provider key.
        /// </summary>
        private readonly Guid _providerKey;

        /// <summary>
        /// The name.
        /// </summary>
        private string _name;

        /// <summary>
        /// The description.
        /// </summary>
        private string _description;

        /// <summary>
        /// The payment code.
        /// </summary>
        private string _paymentCode;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentMethod"/> class.
        /// </summary>
        /// <param name="providerKey">
        /// The provider key.
        /// </param>
        internal PaymentMethod(Guid providerKey)
        {
            Ensure.ParameterCondition(!Guid.Empty.Equals(providerKey), "providerKey");
            _providerKey = providerKey;
        }


        /// <inheritdoc/>
        [DataMember]
        public Guid ProviderKey
        {
            get
            {
                return _providerKey;
            }         
        }

        /// <inheritdoc/>
        [DataMember]
        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _name, _ps.Value.NameSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string Description
        {
            get
            {
                return _description;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _description, _ps.Value.DescriptionSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string PaymentCode
        {
            get
            {
                return _paymentCode;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _paymentCode, _ps.Value.PaymentCodeSelector);
            }
        }

        /// <summary>
        /// The property selectors.
        /// </summary>
        private class PropertySelectors
        {
            /// <summary>
            /// The name selector.
            /// </summary>
            public readonly PropertyInfo NameSelector = ExpressionHelper.GetPropertyInfo<PaymentMethod, string>(x => x.Name);

            /// <summary>
            /// The description selector.
            /// </summary>
            public readonly PropertyInfo DescriptionSelector = ExpressionHelper.GetPropertyInfo<PaymentMethod, string>(x => x.Description);

            /// <summary>
            /// The payment code selector.
            /// </summary>
            public readonly PropertyInfo PaymentCodeSelector = ExpressionHelper.GetPropertyInfo<PaymentMethod, string>(x => x.PaymentCode);
        }
    }
}