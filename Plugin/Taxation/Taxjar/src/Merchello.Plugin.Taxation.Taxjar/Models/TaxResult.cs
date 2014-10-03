namespace Merchello.Plugin.Taxation.Taxjar.Models
{
    using System;
    using System.Collections.Generic;

    public class TaxResult
    {
        /// <summary>
        /// Gets the extended data key.
        /// </summary>
        public static string ExtendedDataKey
        {
            get
            {
                return "merchTaxJarTaxResult";
            }
        }

        /*  "amount_to_collect" => 0.83,
                           "rate" => 0.083,
                      "has_nexus" => true,
                "freight_taxable" => false,
                     "tax_source" => "origin"
         * */

        public decimal TotalTax { get; set; }
        public decimal Rate { get; set; }
        public bool HasNexus { get; set; }
        public bool FreightTaxable { get; set; }
        public string TaxSource { get; set; }
        public bool Success { get; set; }
    }
}
