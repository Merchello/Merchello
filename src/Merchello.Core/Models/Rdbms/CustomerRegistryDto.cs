using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchCustomerItemRegister")]
    [PrimaryKey("id")]
    [ExplicitColumns]
    internal class CustomerItemRegisterDto
    {
        [Column("id")]
        [PrimaryKeyColumn]
        public int Id { get; set; }

        [Column("consumerKey")]
        [IndexAttribute(IndexTypes.NonClustered, Name = "IX_merchCustomerItemRegisterConsumerKey")]
        public Guid ConsumerKey { get; set; }

        [Column("registerTfKey")]
        public Guid RegisterTfKey { get; set; }

        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }

    }
}
