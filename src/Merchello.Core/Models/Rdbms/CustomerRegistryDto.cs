using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchCustomerRegistry")]
    [PrimaryKey("id")]
    [ExplicitColumns]
    internal class CustomerRegistryDto
    {
        [Column("id")]
        [PrimaryKeyColumn]
        public int Id { get; set; }

        [Column("consumerKey")]
        [IndexAttribute(IndexTypes.NonClustered, Name = "IX_merchCustomerRegistryConsumerKey")]
        public Guid ConsumerKey { get; set; }

        [Column("registryTfKey")]
        public Guid RegistryTfKey { get; set; }

        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }

    }
}
