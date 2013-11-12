using System;
using System.Collections.Generic;
using Merchello.Core.Models;


namespace Merchello.Web.Models.ContentEditing
{
    public static class ProductOptionDisplayExtensions
    {
        public static IProductOption ToProductOption(this ProductOptionDisplay productOptionDisplay, IProductOption destinationProductOption)
        {
            destinationProductOption.Required = productOptionDisplay.Required;
            destinationProductOption.SortOrder = productOptionDisplay.SortOrder;

            foreach (var choice in productOptionDisplay.Choices)
            {
                IProductAttribute destinationProductAttribute;
                if (destinationProductOption.Choices.Contains(choice.Sku))
                {
                    destinationProductAttribute = destinationProductOption.Choices[choice.Sku];

                    destinationProductAttribute = choice.ToProductAttribute(destinationProductAttribute);
                }
                else
                {
                    destinationProductAttribute = new ProductAttribute(choice.Name, choice.Sku);
                        
                    destinationProductAttribute = choice.ToProductAttribute(destinationProductAttribute);
                }

                destinationProductOption.Choices.Add(destinationProductAttribute);
            }

            return destinationProductOption;
        }
    }
}
