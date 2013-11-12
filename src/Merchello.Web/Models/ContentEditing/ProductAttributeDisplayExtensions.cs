using System;
using System.Collections.Generic;
using Merchello.Core.Models;


namespace Merchello.Web.Models.ContentEditing
{
    public static class ProductAttributeDisplayExtensions
    {
        public static IProductAttribute ToProductAttribute(this ProductAttributeDisplay productAttributeDisplay, IProductAttribute destinationProductAttribute)
        {
            destinationProductAttribute.Name = productAttributeDisplay.Name;
            destinationProductAttribute.Sku = productAttributeDisplay.Sku;
            destinationProductAttribute.OptionId = productAttributeDisplay.OptionId;
            destinationProductAttribute.SortOrder = productAttributeDisplay.SortOrder;

            return destinationProductAttribute;
        }
    }
}
