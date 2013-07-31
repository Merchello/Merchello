using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchBasketItem")]
    [PrimaryKey("id")]
    [ExplicitColumns]
    public class BasketItemDto
    {
        [Column("id")]
        [PrimaryKeyColumn]
        public int Id { get; set; }

        [Column("parentId")]
        [ForeignKey(typeof(BasketItemDto), Name = "FK_merchBasketItem_merchBasketItem", Column = "id")]
        [IndexAttribute(IndexTypes.NonClustered, Name = "IX_merchBasketItemParent")]
        public int ParentId { get; set; }

        [Column("basketId")]
        [ForeignKey(typeof(BasketDto), Name = "FK_merchBasketItem_merchBasket", Column = "id")]
        public int BasketId { get; set; }

        [Column("typeKey")] // This is an InvoiceItemType
        public Guid TypeKey { get; set; }

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

        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }
    }
}
