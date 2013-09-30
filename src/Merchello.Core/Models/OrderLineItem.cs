using System;
using System.Reflection;
using System.Runtime.Serialization;
using Merchello.Core.Models.TypeFields;

namespace Merchello.Core.Models
{

    /// <summary>
    /// Defines a purchase list item
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    internal class OrderLineItem : LineItemBase, IOrderLineItem
    {
        private int _unitOfMeasureMultiplier;

        //public OrderLineItem(int containerId, LineItemType lineItemType)
        //    : this(containerId, EnumTypeFieldConverter)
        //{ }

        internal OrderLineItem(int containerId, Guid lineItemTfKey)
            : this(containerId, lineItemTfKey, new LineItemCollection())
        { }

        internal OrderLineItem(int containerId, Guid lineItemTfKey, LineItemCollection itemization)
            : base(containerId, lineItemTfKey, itemization)
        { }

        private static readonly PropertyInfo UnitOfMeasureMultiplierSelector = ExpressionHelper.GetPropertyInfo<OrderLineItem, int>(x => x.UnitOfMeasureMultiplier);

        /// <summary>
        /// The unit of measure associated with the item
        /// </summary>
        [DataMember]
        public int UnitOfMeasureMultiplier
        {
            get { return _unitOfMeasureMultiplier; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _unitOfMeasureMultiplier = value;
                    return _unitOfMeasureMultiplier;
                }, _unitOfMeasureMultiplier, UnitOfMeasureMultiplierSelector);
            }
        }
    }

}