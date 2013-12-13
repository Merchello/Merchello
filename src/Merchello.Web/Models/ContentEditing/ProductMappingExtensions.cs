using System.Linq;
using Merchello.Core;
using Merchello.Core.Models;
using System;

namespace Merchello.Web.Models.ContentEditing
{
    internal static class ProductMappingExtensions
    {

        #region IProduct

        internal static IProduct ToProduct(this ProductDisplay productDisplay, IProduct destination)
        {
            if (productDisplay.Key != Guid.Empty)
            {
                destination.Key = productDisplay.Key;
            }
            destination.Name = productDisplay.Name;
            destination.Sku = productDisplay.Sku;
            destination.Price = productDisplay.Price;
            destination.CostOfGoods = productDisplay.CostOfGoods;
            destination.SalePrice = productDisplay.SalePrice;
            destination.OnSale = productDisplay.OnSale;
            destination.Weight = productDisplay.Weight;
            destination.Length = productDisplay.Length;
            destination.Width = productDisplay.Width;
            destination.Height = productDisplay.Height;
            destination.Barcode = productDisplay.Barcode;
            destination.Available = productDisplay.Available;
            destination.TrackInventory = productDisplay.TrackInventory;
            destination.OutOfStockPurchase = productDisplay.OutOfStockPurchase;
            destination.Taxable = productDisplay.Taxable;
            destination.Shippable = productDisplay.Shippable;
            destination.Download = productDisplay.Download;
            destination.DownloadMediaId = productDisplay.DownloadMediaId;

            foreach (var option in productDisplay.ProductOptions)
            {
                IProductOption destinationProductOption;
                if (destination.ProductOptions.Contains(option.Name))
                {
                    destinationProductOption = destination.ProductOptions[option.Name];

                    destinationProductOption = option.ToProductOption(destinationProductOption);
                }
                else
                {
                    destinationProductOption = new ProductOption(option.Name, option.Required);

                    destinationProductOption = option.ToProductOption(destinationProductOption);
                }

                destination.ProductOptions.Add(destinationProductOption);
            }

            return destination;
        }

        internal static IProduct ToProduct(this ProductDisplay productDisplay, string name, string sku, decimal price)
        {
            var destination = MerchelloContext.Current.Services.ProductService.CreateProduct(name, sku, price);
            return ToProduct(productDisplay, destination);
        }

        #endregion

        #region ProductDisplay

        internal static ProductDisplay ToProductDisplay(this IProduct product)
        {
            AutoMapper.Mapper.CreateMap<IProductAttribute, ProductAttributeDisplay>();
            AutoMapper.Mapper.CreateMap<IProductOption, ProductOptionDisplay>();
            AutoMapper.Mapper.CreateMap<IProductVariant, ProductVariantDisplay>();
            AutoMapper.Mapper.CreateMap<IProduct, ProductDisplay>();

            return AutoMapper.Mapper.Map<ProductDisplay>(product);
        }
               
        #endregion


        #region IProductAttribute


        internal static IProductAttribute ToProductAttribute(this ProductAttributeDisplay productAttributeDisplay, IProductAttribute destinationProductAttribute)
        {
            if (productAttributeDisplay.Key != Guid.Empty)
            {
                destinationProductAttribute.Key = productAttributeDisplay.Key;
            }
            destinationProductAttribute.Name = productAttributeDisplay.Name;
            destinationProductAttribute.Sku = productAttributeDisplay.Sku;
            destinationProductAttribute.OptionKey = productAttributeDisplay.OptionKey;
            destinationProductAttribute.SortOrder = productAttributeDisplay.SortOrder;

            return destinationProductAttribute;
        }

        internal static ProductAttributeDisplay ToProductAttributeDisplay(this IProductAttribute productAttribute)
        {
            AutoMapper.Mapper.CreateMap<IProductAttribute, ProductAttributeDisplay>();

            return AutoMapper.Mapper.Map<ProductAttributeDisplay>(productAttribute);
        }


        #endregion

        #region IProductOption

        internal static IProductOption ToProductOption(this ProductOptionDisplay productOptionDisplay, IProductOption destinationProductOption)
        {
            if (productOptionDisplay.Key != Guid.Empty)
            {
                destinationProductOption.Key = productOptionDisplay.Key;
            }
            destinationProductOption.Required = productOptionDisplay.Required;
            destinationProductOption.SortOrder = productOptionDisplay.SortOrder;

            foreach (var choice in productOptionDisplay.Choices)
            {
                IProductAttribute destinationProductAttribute;
                if (destinationProductOption.Choices.Contains(choice.Sku))
                {
                    destinationProductAttribute = destinationProductOption.Choices[choice.Key];

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

        internal static ProductOptionDisplay ToProductOptionDisplay(this IProductOption productOption)
        {
            AutoMapper.Mapper.CreateMap<IProductAttribute, ProductAttributeDisplay>();
            AutoMapper.Mapper.CreateMap<IProductOption, ProductOptionDisplay>();

            return AutoMapper.Mapper.Map<ProductOptionDisplay>(productOption);
        }

        #endregion

        #region IProductVariant

        internal static ProductVariantDisplay ToProductVariantDisplay(this IProductVariant productVariant)
        {
            AutoMapper.Mapper.CreateMap<IProductAttribute, ProductAttributeDisplay>();
            AutoMapper.Mapper.CreateMap<IProductVariant, ProductVariantDisplay>();

            return AutoMapper.Mapper.Map<ProductVariantDisplay>(productVariant);
        }

        internal static IProductVariant ToProductVariant(this ProductVariantDisplay productVariantDisplay, IProductVariant destination)
        {
            if (productVariantDisplay.Key != Guid.Empty)
            {
                destination.Key = productVariantDisplay.Key;
            }
            destination.Name = productVariantDisplay.Name;
            destination.Sku = productVariantDisplay.Sku;
            destination.Price = productVariantDisplay.Price;
            destination.CostOfGoods = productVariantDisplay.CostOfGoods;
            destination.SalePrice = productVariantDisplay.SalePrice;
            destination.OnSale = productVariantDisplay.OnSale;
            destination.Weight = productVariantDisplay.Weight;
            destination.Length = productVariantDisplay.Length;
            destination.Width = productVariantDisplay.Width;
            destination.Height = productVariantDisplay.Height;
            destination.Barcode = productVariantDisplay.Barcode;
            destination.Available = productVariantDisplay.Available;
            destination.TrackInventory = productVariantDisplay.TrackInventory;
            destination.OutOfStockPurchase = productVariantDisplay.OutOfStockPurchase;
            destination.Taxable = productVariantDisplay.Taxable;
            destination.Shippable = productVariantDisplay.Shippable;
            destination.Download = productVariantDisplay.Download;
            destination.DownloadMediaId = productVariantDisplay.DownloadMediaId;

            destination.ProductKey = productVariantDisplay.ProductKey;

            // TODO: Warehouse Inventory


            // JASON: Not sure we event need to do this...
            //foreach (var attribute in productVariantDisplay.Attributes)
            //{
            //    IProductAttribute destinationProductAttribute;

            //    var attr = destination.Attributes.Where(x => x.Name == attribute.Name).First();
            //    if ( attr != null )
            //    {
            //        destinationProductAttribute = attr;

            //        destinationProductAttribute = attribute.ToProductAttribute(destinationProductAttribute);
            //    }
            //    else
            //    {
            //        destinationProductAttribute = new ProductOption(attribute.Name, attribute.Required);

            //        destinationProductAttribute = attribute.ToProductAttribute(destinationProductAttribute);
            //    }

            //    destination.Attributes.Add(destinationProductAttribute);
            //}

            return destination;
        }

        #endregion
    }
}
