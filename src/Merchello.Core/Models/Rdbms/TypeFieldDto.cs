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
    [PrimaryKey("key", autoIncrement = false)]
    [ExplicitColumns]
    internal class TypeFieldDto
    {
        [Column("key")]
        [PrimaryKeyColumn(AutoIncrement = false)]
        public Guid Key { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("alias")]
        public string Alias { get; set; }
    }
}
