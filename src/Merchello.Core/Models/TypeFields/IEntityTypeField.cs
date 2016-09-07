namespace Merchello.Core.Models.TypeFields
{
    /// <summary>
    /// Represents an EntityTypeField
    /// </summary>
    public interface IEntityTypeField : ITypeFieldMapper<EntityType>
    {
        /// <summary>
        /// Gets the customer <see cref="ITypeField"/>.
        /// </summary>
        ITypeField Customer { get; }

        /// <summary>
        /// Gets the entity collection <see cref="ITypeField"/>.
        /// </summary>
        ITypeField EntityCollection { get; }

        /// <summary>
        /// Gets the GatewayProvider <see cref="ITypeField"/>.
        /// </summary>
        ITypeField GatewayProvider { get; }

        /// <summary>
        /// Gets the Invoice <see cref="ITypeField"/>.
        /// </summary>
        ITypeField Invoice { get; }

        /// <summary>
        /// Gets the ItemCache <see cref="ITypeField"/>.
        /// </summary>
        ITypeField ItemCache { get; }

        /// <summary>
        /// Gets the Order <see cref="ITypeField"/>.
        /// </summary>
        ITypeField Order { get; }

        /// <summary>
        /// Gets the Payment <see cref="ITypeField"/>.
        /// </summary>
        ITypeField Payment { get; }

        /// <summary>
        /// Gets the Product <see cref="ITypeField"/>.
        /// </summary>
        ITypeField Product { get;  }

        /// <summary>
        /// Gets the Shipment <see cref="ITypeField"/>.
        /// </summary>
        ITypeField Shipment { get; }

        /// <summary>
        /// Gets the Warehouse <see cref="ITypeField"/>.
        /// </summary>
        ITypeField Warehouse { get; }

        /// <summary>
        /// Gets the WarehouseCatalog <see cref="ITypeField"/>.
        /// </summary>
        ITypeField WarehouseCatalog { get; }
    }
}