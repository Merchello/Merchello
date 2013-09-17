using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchProduct")]
    [PrimaryKey("pk", autoIncrement = false)]
    [ExplicitColumns]
    internal class ProductDto
    {
        [Column("pk")]
        [PrimaryKeyColumn(AutoIncrement = false)]
        public Guid Key { get; set; }

        [Column("sku")]
        public string Sku { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("price")]
        public decimal Price { get; set; }

        [Column("costOfGoods")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public decimal? CostOfGoods { get; set; }

        [Column("salePrice")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public decimal? SalePrice { get; set; }

        [Column("weight")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public decimal? Weight { get; set; }

        [Column("length")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public decimal? Length { get; set; }

        [Column("width")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public decimal? Width { get; set; }

        [Column("height")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public decimal? Height { get; set; }
       
        [Column("taxable")]
        [Constraint(Default = "1")]
        public bool Taxable { get; set; }

        [Column("shippable")]
        [Constraint(Default = "1")]
        public bool Shippable { get; set; }

        [Column("download")]
        [Constraint(Default = "0")]
        public bool Download { get; set; }

        [Column("downloadUrl")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string DownloadUrl { get; set; }

        [Column("template")]
        [Constraint(Default = "0")]
        public bool Template { get; set; }

        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }
    }
}
