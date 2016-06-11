namespace Merchello.Web.Validation.Tasks
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Models;
    using Merchello.Web.Models.ContentEditing;
    using Merchello.Web.Workflow.CustomerItemCache;

    using Umbraco.Core;
    using Umbraco.Core.Logging;

    /// <summary>
    /// Validates that product pricing in Merchello has not been changed.
    /// </summary>
    /// <remarks>
    /// Note: for multi-currency sites, you should place the extended data key (merchLineItemAllowsValidation = false) 
    /// into the line item extended data so that this task
    /// does not reset the price to that set in the back office
    /// </remarks>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    internal class ValidateProductPriceTask : CustomerItemCacheValidatationTaskBase<ValidationResult<CustomerItemCacheBase>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidateProductPriceTask"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="enableDataModifiers">
        /// The enable data modifiers.
        /// </param>
        public ValidateProductPriceTask(IMerchelloContext merchelloContext, bool enableDataModifiers)
            : base(merchelloContext, enableDataModifiers)
        {
        }

        /// <summary>
        /// Performs the task of validating the pricing information.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>
        public override Attempt<ValidationResult<CustomerItemCacheBase>> PerformTask(ValidationResult<CustomerItemCacheBase> value)
        {
            var visitor = new ProductPricingValidationVisitor(Merchello);

            value.Validated.Accept(visitor);

            if (visitor.InvalidPrices.Any())
            {
                foreach (var result in visitor.InvalidPrices.ToArray())
                {                    
                    var lineItem = result.Key;                                        
                    var quantity = lineItem.Quantity;
                    var name = lineItem.Name;
                    var removedEd = lineItem.ExtendedData.AsEnumerable();
                    value.Validated.RemoveItem(lineItem.Sku);
                    
                    var extendedData = new ExtendedDataCollection();
                    ProductVariantDisplay variant;
                    if (result.Value is ProductDisplay)
                    {
                        var product = result.Value as ProductDisplay;
                        variant = product.AsMasterVariantDisplay();
                    }
                    else
                    {
                        variant = result.Value as ProductVariantDisplay;
                    }

                    if (variant == null)
                    {
                        var nullReference = new NullReferenceException("ProductVariantDisplay cannot be null");
                        LogHelper.Error<ValidateProductPriceTask>("Exception occurred when attempting to adjust pricing information", nullReference);
                        throw nullReference;
                    }

                    extendedData.AddProductVariantValues(variant);
                    extendedData.MergeDataModifierLogs(variant);
                    extendedData.MergeDataModifierLogs(variant);

                    // preserve any custom extended data values
                    foreach (var val in removedEd.Where(val => !extendedData.ContainsKey(val.Key)))
                    {
                        extendedData.SetValue(val.Key, val.Value);
                    }

                    var price = variant.OnSale ? extendedData.GetSalePriceValue() : extendedData.GetPriceValue();

                    var keys = lineItem.ExtendedData.Keys.Where(x => extendedData.Keys.Any(y => y != x));
                    foreach (var k in keys)
                    {
                        extendedData.SetValue(k, lineItem.ExtendedData.GetValue(k));
                    }

                    value.Validated.AddItem(string.IsNullOrEmpty(name) ? variant.Name : name, variant.Sku, quantity, price, extendedData);
                    value.Messages.Add("Price updated for " + variant.Sku + " to " + price);
                }

                value.Validated.Save();
            }

            return Attempt<ValidationResult<CustomerItemCacheBase>>.Succeed(value);
        }
    }
}