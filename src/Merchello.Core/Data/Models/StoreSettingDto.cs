namespace Merchello.Core.Data.Models
{
    using System;

    internal class StoreSettingDto
    {
        public Guid Pk { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public string TypeName { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime CreateDate { get; set; }
    }
}