using System.Collections.Generic;

namespace Merchello.Core.Models
{
    using System;
    using System.Reflection;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Models.Interfaces;

    /// <summary>
    /// The virtual variant.
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    internal class VirtualVariant : Entity, IVirtualVariant
    {
        /// <summary>
        /// The property selectors.
        /// </summary>
        private static readonly Lazy<PropertySelectors> _ps = new Lazy<PropertySelectors>();

        #region Fields

        /// <summary>
        /// Product variant sku.
        /// </summary>
        private string _sku;

        /// <summary>
        /// Product key.
        /// </summary>
        private Guid _productKey;

        /// <summary>
        /// The product choices.
        /// </summary>
        private Dictionary<string, string> _choices;

        #endregion

        /// <inheritdoc/>
        [DataMember]
        public string Sku
        {
            get
            {
                return _sku;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _sku, _ps.Value.SkuSelector);
            }
        }


        /// <inheritdoc/>
        [DataMember]
        public Guid ProductKey
        {
            get
            {
                return _productKey;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _productKey, _ps.Value.ProductKeySelector);
            }
        }
        
        /// <inheritdoc/>
        [DataMember]
        public Dictionary<string, string> Choices
        {
            get
            {
                return _choices;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _choices, _ps.Value.ChoicesSelector);
            }
        }

        /// <summary>
        /// The property selectors.
        /// </summary>
        private class PropertySelectors
        {
            /// <summary>
            /// The product variant sku.
            /// </summary>
            public readonly PropertyInfo SkuSelector = ExpressionHelper.GetPropertyInfo<VirtualVariant, string>(x => x.Sku);

            /// <summary>
            /// The product key.
            /// </summary>
            public readonly PropertyInfo ProductKeySelector = ExpressionHelper.GetPropertyInfo<VirtualVariant, Guid>(x => x.ProductKey);

            /// <summary>
            /// The product choices.
            /// </summary>
            public readonly PropertyInfo ChoicesSelector = ExpressionHelper.GetPropertyInfo<VirtualVariant, Dictionary<string, string>>(x => x.Choices);
        }
    }
}