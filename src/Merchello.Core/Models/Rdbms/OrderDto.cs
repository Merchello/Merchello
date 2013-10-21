using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchOrder")]
    [PrimaryKey("id")]
    [ExplicitColumns]
    internal class OrderDto
    {
        [Column("id")]
        [PrimaryKeyColumn]
        public int Id { get; set; }

        [Column("customerId")]
        [ForeignKey(typeof(CustomerDto), Name = "FK_merchOrder_merchCustomer",Column = "id")]
        [IndexAttribute(IndexTypes.NonClustered, Name = "IX_merchOrderCustomer")]
        public int CustomerId { get; set; }

        [Column("orderNumber")]
        [IndexAttribute(IndexTypes.UniqueNonClustered, Name = "IX_merchOrderNumber")]
        public string OrderNumber { get; set; }

        [Column("orderDate")]
        [IndexAttribute(IndexTypes.NonClustered, Name = "IX_merchOrderDate")]
        public DateTime OrdereDate { get; set; }

        [Column("orderStatusId")]
        [ForeignKey(typeof(OrderStatusDto), Name = "FK_merchOrder_merchOrderStatus", Column = "id")]
        public int OrderStatusId { get; set; }
        
        [Column("exported")]
        public bool Exported { get; set; }
      
        [Column("amount")]
        public decimal Amount { get; set; }

        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }

        [ResultColumn]
        public OrderStatusDto OrderStatusDto { get; set; }

        [ResultColumn]
        public CustomerDto CustomerDto { get; set; }

    }
}