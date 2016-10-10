namespace Merchello.Core.Models
{
    using System;
    using System.Reflection;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;

    /// <inheritdoc/>
    [Serializable]
    [DataContract(IsReference = true)]
    public class StoreSetting : Entity, IStoreSetting
    {
        /// <summary>
        /// The property selectors.
        /// </summary>
        private static readonly Lazy<PropertySelectors> _ps = new Lazy<PropertySelectors>();

        /// <summary>
        /// The name.
        /// </summary>
        private string _name;

        /// <summary>
        /// The value.
        /// </summary>
        private string _value;

        /// <summary>
        /// The type name.
        /// </summary>
        private string _typeName;

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
        public string Value
        {
            get
            {
                return _value;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _value, _ps.Value.ValueSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string TypeName
        {
            get
            {
                return _typeName;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _typeName, _ps.Value.TypeNameSelector);
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
            public readonly PropertyInfo NameSelector = ExpressionHelper.GetPropertyInfo<StoreSetting, string>(x => x.Name);

            /// <summary>
            /// The value selector.
            /// </summary>
            public readonly PropertyInfo ValueSelector = ExpressionHelper.GetPropertyInfo<StoreSetting, string>(x => x.Value);

            /// <summary>
            /// The type name selector.
            /// </summary>
            public readonly PropertyInfo TypeNameSelector = ExpressionHelper.GetPropertyInfo<StoreSetting, string>(x => x.TypeName);
        }
    }
}