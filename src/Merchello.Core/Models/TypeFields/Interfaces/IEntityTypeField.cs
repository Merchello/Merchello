namespace Merchello.Core.Models.TypeFields
{
    public interface IEntityTypeField : ITypeFieldMapper<EntityType>
    {
        /// <summary>
        /// The customer entity type
        /// </summary>
        ITypeField Customer { get; }

        /// <summary>
        /// The GatewayProvider entity type
        /// </summary>
        ITypeField GatewayProvider { get; }

        /// <summary>
        /// The Invoice entity type
        /// </summary>
        ITypeField Invoice { get; }

        /// <summary>
        /// The ItemCache entity type
        /// </summary>
        ITypeField ItemCache { get; }

        /// <summary>
        /// The Order entity type
        /// </summary>
        ITypeField Order { get; }

        /// <summary>
        /// The Payemnt entity type
        /// </summary>
        ITypeField Payment { get; }

        /// <summary>
        /// The Product entity type
        /// </summary>
        ITypeField Product { get;  }

        /// <summary>
        /// The Shipment Entity type
        /// </summary>
        ITypeField Shipment { get; }

        /// <summary>
        /// The Warehouse Entity type
        /// </summary>
        ITypeField Warehouse { get; }

        /// <summary>
        /// The WarehouseCatalog entity tyep
        /// </summary>
        ITypeField WarehouseCatalog { get; }
    }
}