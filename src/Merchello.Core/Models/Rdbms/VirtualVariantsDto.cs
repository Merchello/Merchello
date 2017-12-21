using System.Collections.Generic;

namespace Merchello.Core.Models.Rdbms
{
    using System;

    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.DatabaseAnnotations;

    /// <summary>
    /// The Virtual Variants DTO.
    /// </summary>
    [TableName("merchVirtualVariants")]
    [PrimaryKey("pk", autoIncrement = false)]
    [ExplicitColumns]
    internal class VirtualVariantsDto
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        [Column("pk")]
        [PrimaryKeyColumn(AutoIncrement = false)]
        [Constraint(Default = "newid()")]
        public Guid Key { get; set; }
      
        /// <summary>
        /// Gets or sets the sku.
        /// </summary>
        [Column("sku")]
        [NullSetting(NullSetting = NullSettings.NotNull)]
        public string Sku { get; set; }

        /// <summary>
        /// Gets or sets the product key.
        /// </summary>
        [Column("productKey")]
        [NullSetting(NullSetting = NullSettings.NotNull)]
        public Guid ProductKey { get; set; }

        /// <summary>
        /// Gets or sets the choices.
        /// </summary>
        [Column("choices")]
        [NullSetting(NullSetting = NullSettings.NotNull)]
        [SpecialDbType(SpecialDbTypes.NTEXT)]
        public Dictionary<string, string> Choices { get; set; }
    }
}
