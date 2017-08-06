namespace Merchello.Core.Data.Models
{
    using System;

    internal partial class AuditLogDto
    {
        public Guid Pk { get; set; }

        public Guid? EntityKey { get; set; }

        public Guid? EntityTfKey { get; set; }

        public string Message { get; set; }

        public int Verbosity { get; set; }

        public string ExtendedData { get; set; }

        public bool IsError { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime CreateDate { get; set; }
    }
}