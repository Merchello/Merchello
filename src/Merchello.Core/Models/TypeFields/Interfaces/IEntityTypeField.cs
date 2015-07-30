namespace Merchello.Core.Models.TypeFields
{
    /// <summary>
    /// Defines the EntityTypeField.
    /// </summary>
    public interface IEntityTypeField : ITypeFieldMapper<EntityType>
    {
        /// <summary>
        /// Gets the customer entity type
        /// </summary>
        ITypeField Customer { get; }

        /// <summary>
        /// Gets the entity collection entity type.
        /// </summary>
        ITypeField EntityCollection { get; }

        /// <summary>
        /// Gets the GatewayProvider entity type
        /// </summary>
        ITypeField GatewayProvider { get; }

        /// <summary>
        /// Gets the Invoice entity type
        /// </summary>
        ITypeField Invoice { get; }

        /// <summary>
        /// Gets the ItemCache entity type
        /// </summary>
        ITypeField ItemCache { get; }

        /// <summary>
        /// Gets the Order entity type
        /// </summary>
        ITypeField Order { get; }

        /// <summary>
        /// Gets the Payment entity type
        /// </summary>
        ITypeField Payment { get; }

        /// <summary>
        /// Gets the Product entity type
        /// </summary>
        ITypeField Product { get;  }

        /// <summary>
        /// Gets the Shipment Entity type
        /// </summary>
        ITypeField Shipment { get; }

        /// <summary>
        /// Gets the Warehouse Entity type
        /// </summary>
        ITypeField Warehouse { get; }

        /// <summary>
        /// Gets the WarehouseCatalog entity type
        /// </summary>
        ITypeField WarehouseCatalog { get; }
    }
}