using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchProductAttribute")]
    [PrimaryKey("id")]
    [ExplicitColumns]
    internal class ProductAttributeDto
    {
        [Column("id")]
        [PrimaryKeyColumn]
        public int Id { get; set; }

        [Column("optionId")]
        [ForeignKey(typeof(ProductOptionDto), Name = "FK_merchProductAttribute_merchOption", Column = "id")]
        public int OptionId { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("sku")]
        public string Sku { get; set; }

        [Column("sortOrder")]
        public int SortOrder { get; set; }

        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }
    }
}
