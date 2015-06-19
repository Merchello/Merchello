using System.Collections.Specialized;

namespace Merchello.Core.Models
{
    using System;
    using System.Reflection;
    using System.Runtime.Serialization;

    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Models.Interfaces;

    ///// TODO SR - This is just a scaffold for example.  Do whatever you need to do =)


    /// <summary>
    /// The digital media.
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    internal class DigitalMedia : Entity, IDigitalMedia
    {
        #region Fields
        
        /// <summary>
        /// The name selector.
        /// </summary>
        /// <remarks>
        /// </remarks>
        private static readonly PropertyInfo FirstAccessedSelector = ExpressionHelper.GetPropertyInfo<DigitalMedia, DateTime?>(x => x.FirstAccessed);

        private static readonly PropertyInfo ProductVariantSelector = ExpressionHelper.GetPropertyInfo<DigitalMedia, Guid>(x => x.ProductVariantKey);

        private static readonly PropertyInfo ExtendedDataChangedSelector = ExpressionHelper.GetPropertyInfo<LineItemBase, ExtendedDataCollection>(x => x.ExtendedData);

        private void ExtendedDataChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(ExtendedDataChangedSelector);
        }
        /// <summary>
        /// This is immutable
        /// </summary>
        
        private DateTime? _firstAccessed;

        private Guid _productVariantKey;


        private ExtendedDataCollection _extendedData;
        #endregion

        [DataMember]
        public Guid ProductVariantKey
        {
            get
            {
                return _productVariantKey;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                    {
                        _productVariantKey = value;
                        return _productVariantKey;
                    },
                    _productVariantKey,
                    ProductVariantSelector);
            }
        }

        [DataMember]
        public DateTime? FirstAccessed
        {
            get
            {
                return _firstAccessed;
            }

            set
            {
                SetPropertyValueAndDetectChanges(
                    o =>
                    {
                        _firstAccessed = value;
                        return _firstAccessed;
                    },
                    _firstAccessed,
                    FirstAccessedSelector);
            }
        }


        /// <summary>
        /// A collection for storing custom/extended line item data
        /// </summary>
        [DataMember]
        public ExtendedDataCollection ExtendedData
        {
            get { return _extendedData; }
            internal set
            {
                _extendedData = value;
                _extendedData.CollectionChanged += ExtendedDataChanged;
            }
        }
    }
}