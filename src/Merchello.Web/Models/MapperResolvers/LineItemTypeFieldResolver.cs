namespace Merchello.Web.Models.MapperResolvers
{
    using System.Collections.Generic;
    using System.Linq;

    using AutoMapper;

    using Merchello.Core;
    using Merchello.Core.Models;
    using Merchello.Core.Models.TypeFields;
    using Merchello.Web.Models.ContentEditing;

    /// <summary>
    /// The line item type field resolver.
    /// </summary>
    internal class LineItemTypeFieldResolver : ValueResolver<ILineItem, TypeField>
    {
        /// <summary>
        /// Resolves the line item type field.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="TypeField"/>.
        /// </returns>
        protected override TypeField ResolveCore(ILineItem source)
        {
            var type = EnumTypeFieldConverter.LineItemType.GetTypeField(source.LineItemTfKey);
            ITypeField typeField = EnumTypeFieldConverter.LineItemType.Product;
            switch (type)
            {
                case LineItemType.Custom:
                    typeField =
                        EnumTypeFieldConverter.LineItemType.CustomTypeFields.FirstOrDefault(
                            x => x.TypeKey.Equals(source.LineItemTfKey));
                    break;
                case LineItemType.Discount:
                    typeField = EnumTypeFieldConverter.LineItemType.Discount;
                    break;
                case LineItemType.Product:
                    typeField = EnumTypeFieldConverter.LineItemType.Product;
                    break;
                case LineItemType.Tax:
                    typeField = EnumTypeFieldConverter.LineItemType.Tax;
                    break;
                case LineItemType.Shipping:
                    typeField = EnumTypeFieldConverter.LineItemType.Shipping;
                    break;
                    
            }

            return (TypeField)typeField;
        }
    }
}