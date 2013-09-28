using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchProductBase")]
    [PrimaryKey("pk", autoIncrement = false)]
    [ExplicitColumns]
    internal class ProductBaseDto
    {
        [Column("pk")]
        [PrimaryKeyColumn(AutoIncrement = false)]
        [Constraint(Default = "newid()")]
        public Guid Key { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("baseSku")]
        public string BaseSku { get; set; }

        [Column("price")]
        public decimal Price { get; set; }

        [Column("canHaveOptions")]
        public bool CanHaveOptions { get; set; }

        [Column("outOfStockPurchase")]
        public bool OutOfStockPurchase { get; set; }

        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }

    }
}
