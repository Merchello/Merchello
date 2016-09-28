namespace Merchello.Core.Models
{
    using System;
    using System.Reflection;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Models.TypeFields;

    /// <summary>
    /// The applied payment.
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    internal class AppliedPayment : Entity, IAppliedPayment
    {
        /// <summary>
        /// The property selectors.
        /// </summary>
        private static readonly Lazy<PropertySelectors> _ps = new Lazy<PropertySelectors>();

        /// <summary>
        /// The invoice key.
        /// </summary>
        private readonly Guid _invoiceKey;

        /// <summary>
        /// The payment key.
        /// </summary>
        private readonly Guid _paymentKey;

        /// <summary>
        /// The applied payment type field key.
        /// </summary>
        private Guid _appliedPaymentTfKey;

        /// <summary>
        /// The description.
        /// </summary>
        private string _description;

        /// <summary>
        /// The amount.
        /// </summary>
        private decimal _amount;

        /// <summary>
        /// The exported
        /// </summary>
        private bool _exported;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppliedPayment"/> class.
        /// </summary>
        /// <param name="paymentKey">
        /// The payment key.
        /// </param>
        /// <param name="invoiceKey">
        /// The invoice key.
        /// </param>
        /// <param name="appliedPaymentType">
        /// The applied payment type.
        /// </param>
        public AppliedPayment(Guid paymentKey, Guid invoiceKey, AppliedPaymentType appliedPaymentType)
            : this(
                paymentKey,
                invoiceKey,
                EnumTypeFieldConverter.AppliedPayment.GetTypeField(appliedPaymentType).TypeKey)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppliedPayment"/> class.
        /// </summary>
        /// <param name="paymentKey">
        /// The payment key.
        /// </param>
        /// <param name="invoiceKey">
        /// The invoice key.
        /// </param>
        /// <param name="appliedPaymentTfKey">
        /// The applied payment type field key.
        /// </param>
        internal AppliedPayment(Guid paymentKey, Guid invoiceKey, Guid appliedPaymentTfKey)
        {
            Ensure.ParameterCondition(!Guid.Empty.Equals(paymentKey), "paymentKey");
            Ensure.ParameterCondition(!Guid.Empty.Equals(invoiceKey), "invoiceKey");
            Ensure.ParameterCondition(!Guid.Empty.Equals(appliedPaymentTfKey), "appliedPaymentTfKey");

            _paymentKey = paymentKey;
            _invoiceKey = invoiceKey;
            _appliedPaymentTfKey = appliedPaymentTfKey;
        }

        /// <inheritdoc/>
        [DataMember]
        public Guid PaymentKey
        {
            get
            {
                return _paymentKey;
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public Guid InvoiceKey
        {
            get
            {
                return _invoiceKey;
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public Guid AppliedPaymentTfKey
        {
            get
            {
                return _appliedPaymentTfKey;
            }

            internal set
            {
                SetPropertyValueAndDetectChanges(
                    value,
                    ref _appliedPaymentTfKey,
                    _ps.Value.AppliedPaymentTypeFieldSelector);
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
        public decimal Amount
        {
            get
            {
                return _amount;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _amount, _ps.Value.AmountSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public bool Exported
        {
            get
            {
                return _exported;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _exported, _ps.Value.ExportedSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public AppliedPaymentType TransactionType
        {
            get
            {
                return EnumTypeFieldConverter.AppliedPayment.GetTypeField(_appliedPaymentTfKey);
            }

            set
            {
                var reference = EnumTypeFieldConverter.AppliedPayment.GetTypeField(value);
                if (!ReferenceEquals(TypeFieldMapperBase.NotFound, reference))
                {
                    // call through the property to flag the dirty property
                    AppliedPaymentTfKey = reference.TypeKey;
                }
            }
        }

        /// <summary>
        /// Property selectors.
        /// </summary>
        private class PropertySelectors
        {
            /// <summary>
            /// The applied payment type field selector.
            /// </summary>
            public readonly PropertyInfo AppliedPaymentTypeFieldSelector =
                ExpressionHelper.GetPropertyInfo<AppliedPayment, Guid>(x => x.AppliedPaymentTfKey);

            /// <summary>
            /// The description selector.
            /// </summary>
            public readonly PropertyInfo DescriptionSelector =
                ExpressionHelper.GetPropertyInfo<AppliedPayment, string>(x => x.Description);

            /// <summary>
            /// The amount selector.
            /// </summary>
            public readonly PropertyInfo AmountSelector =
                ExpressionHelper.GetPropertyInfo<AppliedPayment, decimal>(x => x.Amount);

            /// <summary>
            /// The exported selector.
            /// </summary>
            public readonly PropertyInfo ExportedSelector =
                ExpressionHelper.GetPropertyInfo<AppliedPayment, bool>(x => x.Exported);
        }
    }
}