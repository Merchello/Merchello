using System;
using System.Reflection;
using System.Runtime.Serialization;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models
{

    /// <summary>
    /// Defines a purchase list item
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    internal class PurchaseLineItemContainer : LineItemContainerBase, IPurchaseLineItem
    {
        private int _unitOfMeasureMultiplier;

        public PurchaseLineItemContainer(int containerId, LineItemCollection lineItems)
            : base(containerId, lineItems)
        { }

        private static readonly PropertyInfo UnitOfMeasureMultiplierSelector = ExpressionHelper.GetPropertyInfo<PurchaseLineItemContainer, int>(x => x.UnitOfMeasureMultiplier);

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