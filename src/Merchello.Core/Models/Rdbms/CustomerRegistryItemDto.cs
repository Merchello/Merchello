using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchCustomerItemRegisterItem")]
    [PrimaryKey("id")]
    [ExplicitColumns]
    internal class CustomerItemRegisterItemDto
    {
        [Column("id")]
        [PrimaryKeyColumn]
        public int Id { get; set; }

        [Column("parentId")]
        [ForeignKey(typeof(CustomerItemRegisterItemDto), Name = "FK_merchCustomerItemRegisterItem_merchCustomerItemRegisterItem", Column = "id")]
        [IndexAttribute(IndexTypes.NonClustered, Name = "IX_merchCustomerItemRegisterItemParent")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public int? ParentId { get; set; }

        [Column("customerItemRegisterId")]
        [ForeignKey(typeof(CustomerItemRegisterDto), Name = "FK_merchCustomerItemRegistryItem_merchCustomerItemRegistry", Column = "id")]
        public int CustomerItemRegisterId { get; set; }

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
        [NullSetting(NullSetting = NullSettings.Null)]
        public string ExtendedData { get; set; }

        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }
    }
}
