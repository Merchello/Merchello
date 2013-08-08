using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchDBTypeField")]
    [PrimaryKey("pk", autoIncrement = false)]
    [ExplicitColumns]
    public class TypeFieldDto
    {
        [Column("pk")]
        [PrimaryKeyColumn]
        public Guid Pk { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("alias")]
        public string Alias { get; set; }
    }
}
