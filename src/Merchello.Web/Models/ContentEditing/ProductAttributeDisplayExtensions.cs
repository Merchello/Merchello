using System;
using System.Collections.Generic;
using Merchello.Core.Models;


namespace Merchello.Web.Models.ContentEditing
{
    internal static class ProductAttributeDisplayExtensions
    {
        internal static IProductAttribute ToProductAttribute(this ProductAttributeDisplay productAttributeDisplay, IProductAttribute destinationProductAttribute)
        {
            destinationProductAttribute.Name = productAttributeDisplay.Name;
            destinationProductAttribute.Sku = productAttributeDisplay.Sku;
            destinationProductAttribute.OptionKey = productAttributeDisplay.OptionKey;
            destinationProductAttribute.SortOrder = productAttributeDisplay.SortOrder;

            return destinationProductAttribute;
        }
    }
}
