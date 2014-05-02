using System;
using System.Reflection;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;
using Merchello.Core.Models.Interfaces;

namespace Merchello.Core.Models
{
    [Serializable]
    [DataContract(IsReference = true)]
    internal class NotificationTrigger : Entity, INotificationTrigger
    {
        private readonly string _name;
        private readonly string _binding;
        private Guid? _entityKey;

        internal NotificationTrigger(string name, string binding)
        {
            Mandate.ParameterNotNullOrEmpty(name, "name");
            Mandate.ParameterNotNullOrEmpty(binding, "binding");

            _name = name;
            _binding = binding;
        }

        private static readonly PropertyInfo EntityKeySelector = ExpressionHelper.GetPropertyInfo<NotificationTrigger, Guid?>(x => x.EntityKey);

        /// <summary>
        /// The name of the trigger
        /// </summary>
        [DataMember]
        public string Name 
        {
            get { return _name; }
        }

        /// <summary>
        /// A unique string "binding" constructed from the ServiceName and Event to Listen For
        /// </summary>
        [DataMember]
        public string Binding
        {
            get { return _binding; }
        }

        /// <summary>
        /// An optional reference key to further constrain the trigger
        /// </summary>
        /// <remarks>
        /// 
        /// eg.  InvoiceStatus.Key or OrderStatus.Key
        /// 
        /// </remarks>
        [DataMember]
        public Guid? EntityKey
        {
            get { return _entityKey; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _entityKey = value;
                    return _entityKey;
                }, _entityKey, EntityKeySelector);
            }
        }
    }
}