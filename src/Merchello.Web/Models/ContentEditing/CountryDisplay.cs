using System;
using System.Collections.Generic;

namespace Merchello.Web.Models.ContentEditing
{
    public class CountryDisplay
    {
        public Guid Key { get; set; }
        public string CountryCode { get; set; }
        public string Name { get; set; }
        public string ProvinceLabel { get; set; }
        public IEnumerable<ProvinceDisplay> Provinces { get; set; }
    }
}
