using Merchello.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merchello.Tests.Base.DataMakers
{
	public class MockSettingDataMaker : MockDataMakerBase												
	{																								   
		private Guid nextInvoiceNumberKey = new Guid("10BF357E-2E91-4888-9AE5-5B9D7E897052");
		private Guid globalShippableKey = new Guid("43116355-FC53-497F-965B-6227B57A38E6");
		private Guid nextOrderNumberKey = new Guid("FFC51FA0-2AFF-4707-876D-79E6FD726022");
		private Guid globalTaxable = new Guid("02F008D4-6003-4E4A-9F82-B0027D6A6208");
		private Guid globalTrackInventory = new Guid("11D2CBE6-1057-423B-A7C4-B0EF6D07D9A0");
		private Guid currencyCodeKey = new Guid("7E62B7AB-E633-4CC1-9C3B-C3C54BF10BF6");
		private Guid dateFormat = new Guid("4693279F-85DC-4EEF-AADF-D47DB0CDE974");
		private Guid timeFormat = new Guid("CBE0472F-3F72-439D-9C9D-FC8F840C1A9D");

		public static IStoreSetting MockSettingForInserting(string value)
		{
			var setting = MockSettingForInserting(nextInvoiceNumberKey, "nextInvoiceNumber", value);
			return setting;
		}

		/// <summary>
		/// Represents a product as if it was returned from the database
		/// </summary>
		/// <param name="key">The key you want to use as the key for the product</param>
		/// <returns><see cref="IStoreSetting"/></returns>
		public static IEnumerable<IStoreSetting> MockSettingComplete(Guid key, string name, string value)
		{
			var setting = MockSettingForInserting(key, name, value);
			((StoreSetting)setting).AddingEntity();
			setting.ResetDirtyProperties();
			return setting;
		}

		/// <summary>
		/// Makes a list of products for inserting
		/// </summary>
		/// <param name="count">The number of products to create</param>
		/// <returns>A collection of <see cref="IStoreSetting"/></returns>			   
		public static IEnumerable<IStoreSetting> MockSettingForInserting(Guid key, string name, string value)
		{				
			return new StoreSetting() { Key = key, Name = name, Value = value };
		}
	}
}
