namespace Merchello.Core.Models
{
    using System;
    using System.Reflection;
    using System.Runtime.Serialization;
    using EntityBase;
    using Interfaces;

    /// <summary>
    /// Represents a warehouse catalog
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class WarehouseCatalog : Entity, IWarehouseCatalog
    {
        private readonly Guid _warehouseKey;
        private string _name;
        private string _description;

        public WarehouseCatalog(Guid warehouseKey)
        {
            Mandate.ParameterCondition(warehouseKey != Guid.Empty, "warehouseKey");
            _warehouseKey = warehouseKey;
        }

        private static readonly PropertyInfo NameSelector = ExpressionHelper.GetPropertyInfo<WarehouseCatalog, string>(x => x.Name);
        private static readonly PropertyInfo DescriptionSelector = ExpressionHelper.GetPropertyInfo<WarehouseCatalog, string>(x => x.Description);

        /// <summary>
        /// The warhouse key (identifier) 
        /// </summary>
        [DataMember]
        public Guid WarehouseKey {
            get { return _warehouseKey; }
        }

        /// <summary>
        /// The optional name or title of the catalog
        /// </summary>
        [DataMember]
        public string Name
        {
            get { return _name; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _name = value;
                    return _name;
                }, _name, NameSelector);
            }
        }

        /// <summary>
        /// The optional description of the catalog
        /// </summary>
        [DataMember]
        public string Description
        {
            get { return _description; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _description = value;
                    return _description;
                }, _description, DescriptionSelector);
            }
        }
    }
}