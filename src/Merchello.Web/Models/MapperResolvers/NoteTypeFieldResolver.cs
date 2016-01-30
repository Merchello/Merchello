namespace Merchello.Web.Models.MapperResolvers
{
    using AutoMapper;

    using Merchello.Core.Models;
    using Merchello.Core.Models.TypeFields;

    /// <summary>
    /// The note type field resolver.
    /// </summary>
    public class NoteTypeFieldResolver : ValueResolver<INote, ITypeField>
    {
        /// <summary>
        /// The resolve core.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="ITypeField"/>.
        /// </returns>
        protected override ITypeField ResolveCore(INote source)
        {
            var entityType = EnumTypeFieldConverter.EntityType.GetTypeField(source.EntityTfKey);

            switch (entityType)
            {
                case Core.EntityType.Invoice:
                    return EnumTypeFieldConverter.EntityType.Invoice;
                case Core.EntityType.Customer:
                    return EnumTypeFieldConverter.EntityType.Customer;
                case Core.EntityType.Product:
                    return EnumTypeFieldConverter.EntityType.Product;
                case Core.EntityType.Order:
                    return EnumTypeFieldConverter.EntityType.Order;
                case Core.EntityType.Shipment:
                    return EnumTypeFieldConverter.EntityType.Shipment;
                case Core.EntityType.GatewayProvider:
                    return EnumTypeFieldConverter.EntityType.GatewayProvider;
                case Core.EntityType.ItemCache:
                    return EnumTypeFieldConverter.EntityType.ItemCache;
                case Core.EntityType.Payment:
                    return EnumTypeFieldConverter.EntityType.Payment;
                default:
                    return EnumTypeFieldConverter.EntityType.Invoice;
            }
        }
    }
}