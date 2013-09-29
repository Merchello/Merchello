using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchCustomerRegistryItem")]
    [PrimaryKey("id")]
    [ExplicitColumns]
    internal class CustomerRegistryItemDto
    {
        [Column("id")]
        [PrimaryKeyColumn]
        public int Id { get; set; }

        [Column("parentId")]
        [ForeignKey(typeof(CustomerRegistryItemDto), Name = "FK_merchCustomerRegistryItem_merchCustomerRegistryItemItem", Column = "id")]
        [IndexAttribute(IndexTypes.NonClustered, Name = "IX_merchCustomerRegistryItemParent")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public int? ParentId { get; set; }

        [Column("customerRegistryId")]
        [ForeignKey(typeof(CustomerRegistryDto), Name = "FK_merchCustomerRegistryItem_merchCustomerRegistry", Column = "id")]
        public int CustomerRegistryId { get; set; }

        [Column("lineItemTfKey")] 
        public Guid LineItemTfKey { get; set; }

        [Column("sku")]
        public string Sku { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("baseQuantity")]
        public int BaseQuantity { get; set; }

        [Column("unitOfMeasureMultiplier")]
        public int UnitOfMeasureMultiplier { get; set; }

        [Column("amount")]
        public decimal Amount { get; set; }

        [Column("extendedData")]
        public string ExtendedData { get; set; }

        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }
    }
}
