namespace Merchello.Core.Data.Models
{
    using System;

    internal class NoteDto
    {
        public Guid Pk { get; set; }

        public Guid EntityKey { get; set; }

        public Guid EntityTfKey { get; set; }

        public string Author { get; set; }

        public string Message { get; set; }

        public bool InternalOnly { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime CreateDate { get; set; }
    }
}