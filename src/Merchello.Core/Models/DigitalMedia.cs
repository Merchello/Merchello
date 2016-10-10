namespace Merchello.Core.Models
{
    using System;
    using System.Collections.Specialized;
    using System.Reflection;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Models.Interfaces;

    /// <summary>
    /// The digital media.
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    internal class DigitalMedia : Entity, IDigitalMedia
    {
        /// <summary>
        /// The property selectors.
        /// </summary>
        private static readonly Lazy<PropertySelectors> _ps = new Lazy<PropertySelectors>();

        #region Fields

        /// <summary>
        /// First accessed data.
        /// </summary>
        private DateTime? _firstAccessed;

        /// <summary>
        /// Product variant key.
        /// </summary>
        private Guid _productVariantKey;

        /// <summary>
        /// The extended data collection.
        /// </summary>
        private ExtendedDataCollection _extendedData;

        #endregion

        /// <inheritdoc/>
        [DataMember]
        public Guid ProductVariantKey
        {
            get
            {
                return _productVariantKey;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _productVariantKey, _ps.Value.ProductVariantSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public DateTime? FirstAccessed
        {
            get
            {
                return _firstAccessed;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _firstAccessed, _ps.Value.FirstAccessedSelector);
            }
        }


        /// <inheritdoc/>
        [DataMember]
        public ExtendedDataCollection ExtendedData
        {
            get
            {
                return _extendedData;
            }

            internal set
            {
                _extendedData = value;
                _extendedData.CollectionChanged += ExtendedDataChanged;
            }
        }

        /// <summary>
        /// Handles the ExtendDataCollection changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ExtendedDataChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(_ps.Value.ExtendedDataChangedSelector);
        }

        /// <summary>
        /// The property selectors.
        /// </summary>
        private class PropertySelectors
        {
            /// <summary>
            /// The first accessed selector.
            /// </summary>
            public readonly PropertyInfo FirstAccessedSelector = ExpressionHelper.GetPropertyInfo<DigitalMedia, DateTime?>(x => x.FirstAccessed);

            /// <summary>
            /// The product variant selector.
            /// </summary>
            public readonly PropertyInfo ProductVariantSelector = ExpressionHelper.GetPropertyInfo<DigitalMedia, Guid>(x => x.ProductVariantKey);

            /// <summary>
            /// The extended data changed selector.
            /// </summary>
            public readonly PropertyInfo ExtendedDataChangedSelector = ExpressionHelper.GetPropertyInfo<LineItemBase, ExtendedDataCollection>(x => x.ExtendedData);
        }
    }
}