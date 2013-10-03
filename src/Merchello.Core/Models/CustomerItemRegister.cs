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
        private Guid _registerTfKey;
        private readonly LineItemCollection _items;
 
        public CustomerItemRegister(Guid consumerKey, CustomerItemRegisterType customerItemRegisterType)
            : this(consumerKey, EnumTypeFieldConverter.CustomerItemRegistry.GetTypeField(customerItemRegisterType).TypeKey, new LineItemCollection())
        { }

        internal CustomerItemRegister(Guid consumerKey, Guid registerTfKey)
            : this(consumerKey, registerTfKey, new LineItemCollection())
        { }

        public CustomerItemRegister(Guid consumerKey, CustomerItemRegisterType customerItemRegisterType, LineItemCollection items)
            : this(consumerKey, EnumTypeFieldConverter.CustomerItemRegistry.GetTypeField(customerItemRegisterType).TypeKey, items)
        { }

        internal CustomerItemRegister(Guid consumerKey, Guid registerTfKey, LineItemCollection items)
        {
            Mandate.ParameterCondition(consumerKey != Guid.Empty, "consumerKey");
            Mandate.ParameterCondition(registerTfKey != Guid.Empty, "customerRegisterTfKey");
            Mandate.ParameterNotNull(items, "items");

            _consumerKey = consumerKey;
            _registerTfKey = registerTfKey;
            _items = items;
        }
        
        private static readonly PropertyInfo CustomerRegistrySelector = ExpressionHelper.GetPropertyInfo<CustomerItemRegister, Guid>(x => x.ConsumerKey);
        private static readonly PropertyInfo RegistryTfKeySelector = ExpressionHelper.GetPropertyInfo<CustomerItemRegister, Guid>(x => x.RegisterTfKey);
        
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
        public Guid RegisterTfKey
        {
            get { return _registerTfKey; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _registerTfKey = value;
                    return _registerTfKey;
                }, _registerTfKey, RegistryTfKeySelector);
            }
        }

        /// <summary>
        /// The <see cref="CustomerItemRegisterType"/> of the customer registry
        /// </summary>
        [DataMember]
        public CustomerItemRegisterType CustomerItemRegisterType
        {
            get { return EnumTypeFieldConverter.CustomerItemRegistry.GetTypeField(_registerTfKey); }
            set
            {
                var reference = EnumTypeFieldConverter.CustomerItemRegistry.GetTypeField(value);
                if (!ReferenceEquals(TypeFieldMapperBase.NotFound, reference))
                {
                    // call through the property to flag the dirty property
                    _registerTfKey = reference.TypeKey;
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