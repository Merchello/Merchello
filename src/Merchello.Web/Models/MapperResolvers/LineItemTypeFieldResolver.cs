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
            return (TypeField)source.GetTypeField();
        }
    }
}