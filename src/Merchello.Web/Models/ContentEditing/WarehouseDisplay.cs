using System;

namespace Merchello.Web.Models.ContentEditing
{
    public class WarehouseDisplay
    {
        public Guid Key { get; set; }
        public string Name { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Locality { get; set; }
        public string Region { get; set; }
        public string PostalCode { get; set; }
        public string CountryCode { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool IsDefault { get; set; }
    }
}
