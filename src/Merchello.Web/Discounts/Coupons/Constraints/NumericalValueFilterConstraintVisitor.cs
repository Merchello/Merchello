namespace Merchello.Web.Discounts.Coupons.Constraints
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using Merchello.Core;
    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// The numerical value constraint visitor.
    /// </summary>
    internal class NumericalValueFilterConstraintVisitor : ILineItemVisitor
    {
        /// <summary>
        /// The target value configured in the back office editor.
        /// </summary>
        private readonly decimal _target;

        /// <summary>
        /// The string based operator.
        /// </summary>
        /// <remarks>
        /// eg. eq, lt, lte, gt, gte
        /// </remarks>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        private readonly string _operator;

        /// <summary>
        /// The property.
        /// </summary>
        private readonly string _property;

        /// <summary>
        /// Initializes a new instance of the <see cref="NumericalValueFilterConstraintVisitor"/> class.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="op">
        /// The op.
        /// </param>
        /// <param name="property">
        /// The property - either quantity or price
        /// </param>
        public NumericalValueFilterConstraintVisitor(decimal target, string op, string property)
        {            
            Ensure.ParameterNotNullOrEmpty(property, "property");
            Ensure.ParameterCondition(property.Equals("quantity") || property.Equals("price"), "property must be 'quantity' or 'price'");
            _property = property;
            this._operator = op;
            _target = target;
            FilteredLineItems = new List<ILineItem>();
        }

        /// <summary>
        /// Gets the filtered line items.
        /// </summary>
        public List<ILineItem> FilteredLineItems { get; private set; } 

        /// <summary>
        /// Visits the line items to apply expression.
        /// </summary>
        /// <param name="lineItem">
        /// The line item.
        /// </param>        
        public void Visit(ILineItem lineItem)
        {          
            if (lineItem.LineItemType == LineItemType.Product)
            {
                if (StringOperatorHelper.Evaluate(
                    _property == "price" ? lineItem.TotalPrice : lineItem.Quantity,
                    _target,
                    this._operator)) FilteredLineItems.Add(lineItem);
            }
            else
            {
                FilteredLineItems.Add(lineItem);
            }
        }
    }
}