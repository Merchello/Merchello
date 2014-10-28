using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merchello.Plugin.Taxation.Taxjar.Models
{
    public class TaxRequest
    {

        /*
         * from http://www.taxjar.com/api/docs/#smart-sales-tax-api
         * 
         * Amount 	Amount of the order, excluding shipping.
         * Shipping 	Amount charged for shipping.
         * From Country 	2 Character Country code (US or CA) order when business has nexus.
         * From State 	Two letter postal abbreviation for state/province where the order is shipped from. Also if your business has nexus in the ship to state even if shipped from outside of state.
         * From City 	City order was shipped from or when business has nexus.
         * From Zip 	Zip code order was shipped from or when business has nexus.
         * To Country 	2 Character Country code (US or CA) order was shipped to. (required)
         * To State 	State or Province order was shipped to. (required)
         * To City 	City order was shipped to.
         * To Zip 	Zip code order was shipped to. (required)
         * 
         */

        public decimal Amount { get; set; }
        public decimal Shipping { get; set; }
        //public string FromCountry { get; set; }
        //public string FromState { get; set; }
        //public string FromCity { get; set; }
        //public string FromZip { get; set; }
        public string ToCountry { get; set; }
        public string ToState { get; set; }
        public string ToCity { get; set; }
        public string ToZip { get; set; }
    }
}
