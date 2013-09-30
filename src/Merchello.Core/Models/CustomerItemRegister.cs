using System;
using System.Reflection;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;
using Merchello.Core.Models.TypeFields;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines a customer item register
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    internal class CustomerItemRegister : IdEntity, ICustomerItemRegister
    {
        private Guid _consumerKey;
        private Guid _customerRegisterTfKey;
        private readonly LineItemCollection _items;
 
        public CustomerItemRegister(Guid consumerKey, CustomerItemRegisterType customerItemRegisterType)
            : this(consumerKey, EnumTypeFieldConverter.CustomerItemRegistry.GetTypeField(customerItemRegisterType).TypeKey, new LineItemCollection())
        { }

        internal CustomerItemRegister(Guid consumerKey, Guid customerRegisterTfKey)
            : this(consumerKey, customerRegisterTfKey, new LineItemCollection())
        { }

        public CustomerItemRegister(Guid consumerKey, CustomerItemRegisterType customerItemRegisterType, LineItemCollection items)
            : this(consumerKey, EnumTypeFieldConverter.CustomerItemRegistry.GetTypeField(customerItemRegisterType).TypeKey, items)
        { }

        internal CustomerItemRegister(Guid consumerKey, Guid customerRegisterTfKey, LineItemCollection items)
        {
            Mandate.ParameterCondition(consumerKey != Guid.Empty, "consumerKey");
            Mandate.ParameterCondition(customerRegisterTfKey != Guid.Empty, "customerRegisterTfKey");
            Mandate.ParameterNotNull(items, "items");

            _consumerKey = consumerKey;
            _customerRegisterTfKey = customerRegisterTfKey;
            _items = items;
        }
        
        private static readonly PropertyInfo CustomerRegistrySelector = ExpressionHelper.GetPropertyInfo<CustomerItemRegister, Guid>(x => x.ConsumerKey);
        private static readonly PropertyInfo CustomerRegistryTfKeySelector = ExpressionHelper.GetPropertyInfo<CustomerItemRegister, Guid>(x => x.CustomerRegisterTfKey);
        
        /// <summary>
        /// The <see cref="IConsumer"/> key
        /// </summary>
        [DataMember]
        public Guid ConsumerKey
        {
            get { return _consumerKey; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _consumerKey = value;
                    return _consumerKey;
                }, _consumerKey, CustomerRegistrySelector);
            }
        }

        /// <summary>
        /// The registry type field <see cref="ITypeField"/> guid typeKey
        /// </summary>
        [DataMember]
        public Guid CustomerRegisterTfKey
        {
            get { return _customerRegisterTfKey; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _customerRegisterTfKey = value;
                    return _customerRegisterTfKey;
                }, _customerRegisterTfKey, CustomerRegistryTfKeySelector);
            }
        }

        /// <summary>
        /// The <see cref="CustomerItemRegisterType"/> of the customer registry
        /// </summary>
        [DataMember]
        public CustomerItemRegisterType CustomerItemRegisterType
        {
            get { return EnumTypeFieldConverter.CustomerItemRegistry.GetTypeField(_customerRegisterTfKey); }
            set
            {
                var reference = EnumTypeFieldConverter.CustomerItemRegistry.GetTypeField(value);
                if (!ReferenceEquals(TypeFieldMapperBase.NotFound, reference))
                {
                    // call through the property to flag the dirty property
                    _customerRegisterTfKey = reference.TypeKey;
                }
            }
        }

        /// <summary>
        /// The <see cref="IOrderLineItem"/>s in the customer registry
        /// </summary>
        [DataMember]
        public LineItemCollection Items {
            get { return _items; }
        }
    }
}