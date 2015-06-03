namespace Merchello.Core.Marketing.Constraints
{
    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// A discount validation constraint to restrict this offer to line item price related rules.
    /// </summary>
    [OfferComponent("66957C56-8A5E-4ECD-BDEB-565F8777A38F", "Restrict by price", "This discount is only offered to line item collections matching configured price rules.",
        "~/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/marketing.offerconstraint.price.html")]
    public class OfferPriceConstraint : OfferConstraintComponentBase<ILineItemContainer>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OfferPriceConstraint"/> class.
        /// </summary>
        /// <param name="definition">
        /// The definition.
        /// </param>
        public OfferPriceConstraint(OfferComponentDefinition definition)
            : base(definition)
        {
        }


        /// <summary>
        /// Gets the display configuration format.
        /// This text is used by the back office UI to display configured values
        /// </summary>
        public override string DisplayConfigurationFormat
        {
            get
            {
                var price = this.GetConfigurationValue("price");
                var op = this.GetConfigurationValue("operator");

                var operatorText = StringOperatorHelper.TextForOperatorString(op);

                // price and operator
                if (string.IsNullOrEmpty(price) || string.IsNullOrEmpty(operatorText)) return string.Empty;

                return string.Format("'Price is {0} ' +  $filter('currency')({1}, $scope.currencySymbol)", operatorText, price);
            }
        }

        public override Attempt<ILineItemContainer> Apply(ILineItemContainer value, ICustomerBase customer)
        {
            throw new System.NotImplementedException();
        }
    }
}