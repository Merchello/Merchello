using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;
using Merchello.Core.Models.TypeFields;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines a customer registry
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    internal class CustomerRegistry : IdEntity, ICustomerRegistry
    {
        private Guid _consumerKey;
        private Guid _customerRegistryTfKey;
        private IList<IPurchaseLineItem> _items;
 
        public CustomerRegistry(Guid consumerKey, CustomerRegistryType customerRegistryType)
            : this(consumerKey, EnumTypeFieldConverter.CustomerRegistry().GetTypeField(customerRegistryType).TypeKey, new List<IPurchaseLineItem>())
        { }

        public CustomerRegistry(Guid consumerKey, Guid customerRegistryTfKey)
            : this(consumerKey, customerRegistryTfKey, new List<IPurchaseLineItem>())
        { }

        public CustomerRegistry(Guid consumerKey, CustomerRegistryType customerRegistryType, IList<IPurchaseLineItem> items)
            : this(consumerKey, EnumTypeFieldConverter.CustomerRegistry().GetTypeField(customerRegistryType).TypeKey, items)
        { }

        public CustomerRegistry(Guid consumerKey, Guid customerRegistryTfKey, IList<IPurchaseLineItem> items)
        {
            _consumerKey = consumerKey;
            _customerRegistryTfKey = customerRegistryTfKey;
            _items = items;
        }
        
        private static readonly PropertyInfo CustomerRegistrySelector = ExpressionHelper.GetPropertyInfo<CustomerRegistry, Guid>(x => x.ConsumerKey);
        private static readonly PropertyInfo CustomerRegistryTfKeySelector = ExpressionHelper.GetPropertyInfo<CustomerRegistry, Guid>(x => x.CustomerRegistryTfKey);
        

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
        public Guid CustomerRegistryTfKey
        {
            get { return _customerRegistryTfKey; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _customerRegistryTfKey = value;
                    return _customerRegistryTfKey;
                }, _customerRegistryTfKey, CustomerRegistryTfKeySelector);
            }
        }

        /// <summary>
        /// The <see cref="CustomerRegistryType"/> of the customer registry
        /// </summary>
        [DataMember]
        public CustomerRegistryType CustomerRegistryType
        {
            get { return EnumTypeFieldConverter.CustomerRegistry().GetTypeField(_customerRegistryTfKey); }
            set
            {
                var reference = EnumTypeFieldConverter.CustomerRegistry().GetTypeField(value);
                if (!ReferenceEquals(TypeFieldMapperBase.NotFound, reference))
                {
                    // call through the property to flag the dirty property
                    _customerRegistryTfKey = reference.TypeKey;
                }
            }
        }

        /// <summary>
        /// The <see cref="IPurchaseLineItem"/>s in the customer registry
        /// </summary>
        [DataMember]
        public IEnumerable<IPurchaseLineItem> Items {
            get { return _items; }
        }
    }
}