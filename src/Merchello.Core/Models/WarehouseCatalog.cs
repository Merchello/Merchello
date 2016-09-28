namespace Merchello.Core.Models
{
    using System;
    using System.Reflection;
    using System.Runtime.Serialization;
    using EntityBase;

    /// <inheritdoc/>
    [Serializable]
    [DataContract(IsReference = true)]
    public class WarehouseCatalog : Entity, IWarehouseCatalog
    {
        /// <summary>
        /// The property selectors.
        /// </summary>
        private static readonly Lazy<PropertySelectors> _ps = new Lazy<PropertySelectors>();

        /// <summary>
        /// The warehouse key.
        /// </summary>
        private readonly Guid _warehouseKey;

        /// <summary>
        /// The name.
        /// </summary>
        private string _name;

        /// <summary>
        /// The description.
        /// </summary>
        private string _description;

        /// <summary>
        /// Initializes a new instance of the <see cref="WarehouseCatalog"/> class.
        /// </summary>
        /// <param name="warehouseKey">
        /// The warehouse key.
        /// </param>
        public WarehouseCatalog(Guid warehouseKey)
        {
            Ensure.ParameterCondition(warehouseKey != Guid.Empty, "warehouseKey");
            _warehouseKey = warehouseKey;
        }

        /// <inheritdoc/>
        [DataMember]
        public Guid WarehouseKey
        {
            get
            {
                return _warehouseKey;
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _name, _ps.Value.NameSelector);
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public string Description
        {
            get
            {
                return _description;
            }

            set
            {
                SetPropertyValueAndDetectChanges(value, ref _description, _ps.Value.DescriptionSelector);
            }
        }

        /// <summary>
        /// The property selectors.
        /// </summary>
        private class PropertySelectors
        {
            /// <summary>
            /// The name selector.
            /// </summary>
            public readonly PropertyInfo NameSelector = ExpressionHelper.GetPropertyInfo<WarehouseCatalog, string>(x => x.Name);

            /// <summary>
            /// The description selector.
            /// </summary>
            public readonly PropertyInfo DescriptionSelector = ExpressionHelper.GetPropertyInfo<WarehouseCatalog, string>(x => x.Description);
        }
    }
}