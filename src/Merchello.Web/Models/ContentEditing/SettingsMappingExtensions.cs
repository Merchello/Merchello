using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core;
using Merchello.Core.Models;

namespace Merchello.Web.Models.ContentEditing
{
    internal static class SettingsMappingExtensions
    {

        #region CountryDisplay

        internal static CountryDisplay ToCountryDisplay(this ICountry country)
        {
            AutoMapper.Mapper.CreateMap<ICountry, CountryDisplay>();
            AutoMapper.Mapper.CreateMap<IProvince, ProvinceDisplay>();

            return AutoMapper.Mapper.Map<CountryDisplay>(country);
        }

        #endregion

		internal static SettingDisplay ToStoreSettingDisplay(this SettingDisplay settingDisplay, IEnumerable<IStoreSetting> storeSettings)
		{
			int intValue;
			bool boolValue;
			foreach (var setting in storeSettings)
			{
				switch (setting.Name)
				{
					case "currencyCode":
						settingDisplay.currencyCode = setting.Value;
						break;
					case "nextOrderNumber":
						if (!int.TryParse(setting.Value, out intValue))
						{
							intValue = 0;
						}	 
						settingDisplay.nextOrderNumber = intValue;
						break;
					case "nextInvoiceNumber":
						if (!int.TryParse(setting.Value, out intValue))
						{
							intValue = 0;
						}
						settingDisplay.nextInvoiceNumber = intValue;
						break;
					case "dateFormat":
						settingDisplay.dateFormat = setting.Value;
						break;
					case "timeFormat":
						settingDisplay.timeFormat = setting.Value;
						break;
					case "globalShippable":
						if (!bool.TryParse(setting.Value, out boolValue))
						{
							boolValue = false;
						}
						settingDisplay.globalShippable = boolValue;
						break;
					case "globalTaxable":
						if (!bool.TryParse(setting.Value, out boolValue))
						{
							boolValue = false;
						}
						settingDisplay.globalTaxable = boolValue;
						break;
					case "globalTrackInventory":
						if (!bool.TryParse(setting.Value, out boolValue))
						{
							boolValue = false;
						}
						settingDisplay.globalTrackInventory = boolValue;
						break;
					default:
						setting.Value = string.Empty;
						break;
				}
			}
			return settingDisplay;
		}

		internal static IEnumerable<IStoreSetting> ToStoreSetting(this SettingDisplay settingDisplay, IEnumerable<IStoreSetting> destination)
		{
			foreach(var setting in destination)
			{
				switch (setting.Name)
				{
					case "currencyCode":
						setting.Value = settingDisplay.currencyCode;			  
						break;
					case "nextOrderNumber":
						setting.Value = settingDisplay.nextOrderNumber.ToString();	  
						break;
					case "nextInvoiceNumber":
						setting.Value = settingDisplay.nextInvoiceNumber.ToString();	 
						break;
					case "dateFormat":
						setting.Value = settingDisplay.dateFormat;					 
						break;
					case "timeFormat":
						setting.Value = settingDisplay.timeFormat;
						break;
					case "globalShippable":
						setting.Value = settingDisplay.globalShippable.ToString();
						break;
					case "globalTaxable":
						setting.Value = settingDisplay.globalTaxable.ToString();	 
						break;
					case "globalTrackInventory":
						setting.Value = settingDisplay.globalTrackInventory.ToString();	
						break;
					default:
						setting.Value = string.Empty;		 
						break;
				}												
			}
			return destination;
		}
    }
}
