using System;
using System.Collections.Generic;

namespace Merchello.Web.Models.ContentEditing
{
    public class CountryDisplay
    {
        public string CountryCode { get; set; }
        public string Name { get; set; }
        public string ProvinceLabel { get; set; }
        IEnumerable<ProvinceDisplay> Provinces { get; set;  }
    }
}
