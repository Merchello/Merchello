using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchItemCacheItem")]
    [PrimaryKey("id")]
    [ExplicitColumns]
    internal class ItemCacheItemDto : ILineItemDto
    {
        [Column("id")]
        [PrimaryKeyColumn]
        public int Id { get; set; }
        
        [Column("itemCacheId")]
        [ForeignKey(typeof(ItemCacheDto), Name = "FK_merchItemCacheItem_merchItemCache", Column = "id")]
        public int ContainerId { get; set; }

        [Column("lineItemTfKey")] 
        public Guid LineItemTfKey { get; set; }

        [Column("sku")]
        public string Sku { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }        

        [Column("amount")]
        public decimal Amount { get; set; }

        [Column("extendedData")]
        [NullSetting(NullSetting = NullSettings.Null)]
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
