using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchCustomerItemCacheItem")]
    [PrimaryKey("id")]
    [ExplicitColumns]
    internal class CustomerItemCacheItemDto
    {
        [Column("id")]
        [PrimaryKeyColumn]
        public int Id { get; set; }
        
        [Column("itemCacheId")]
        [ForeignKey(typeof(CustomerItemCacheDto), Name = "FK_merchCustomerItemCacheItem_merchCustomerItemCache", Column = "id")]
        public int ItemCacheId { get; set; }

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

        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }
    }
}
