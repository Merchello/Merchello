using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merchello.Web.Models.ContentEditing
{
	public class SettingDisplay
	{
		public string currencyCode { get; set; }											 
		public int nextOrderNumber { get; set; }
		public int nextInvoiceNumber { get; set; }
		public string dateFormat { get; set; }
		public string timeFormat { get; set; }
		public bool globalShippable { get; set; }
		public bool globalTaxable { get; set; }
        public bool globalTrackInventory { get; set; }
        public bool globalShippingIsTaxable { get; set; }
	}
}
