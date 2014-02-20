using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchItemCacheItem")]
    [PrimaryKey("pk", autoIncrement = false)]
    [ExplicitColumns]
    internal class ItemCacheItemDto : ILineItemDto
    {
        [Column("pk")]
        [PrimaryKeyColumn(AutoIncrement = false)]
        [Constraint(Default = "newid()")]
        public Guid Key { get; set; }
        
        [Column("itemCacheKey")]
        [ForeignKey(typeof(ItemCacheDto), Name = "FK_merchItemCacheItem_merchItemCache", Column = "pk")]
        public Guid ContainerKey { get; set; }

        [Column("lineItemTfKey")] 
        public Guid LineItemTfKey { get; set; }

        [Column("sku")]
        public string Sku { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }        

        [Column("price")]
        public decimal Price { get; set; }

        [Column("extendedData")]
        [NullSetting(NullSetting = NullSettings.Null)]
        [SpecialDbType(SpecialDbTypes.NTEXT)]
        public string ExtendedData { get; set; }

        [Column("exported")]
        public bool Exported { get; set; }

        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }
    }
}
