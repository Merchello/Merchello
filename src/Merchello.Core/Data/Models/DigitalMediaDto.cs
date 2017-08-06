namespace Merchello.Core.Data.Models
{
    using System;

    internal partial class DigitalMediaDto
    {
        public Guid Pk { get; set; }

        public DateTime? FirstAccessed { get; set; }

        public Guid ProductVariantKey { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime CreateDate { get; set; }

        public string ExtendedData { get; set; }
    }
}