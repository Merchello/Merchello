using System;
using Merchello.Core.Models;

namespace Merchello.Web.Models.ContentEditing
{
    public class InvoiceStatusDisplay
    {
        public Guid Key { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public bool Reportable { get; set; }
        public bool Active { get; set; }
        public int SortOrder { get; set; }
    }
}