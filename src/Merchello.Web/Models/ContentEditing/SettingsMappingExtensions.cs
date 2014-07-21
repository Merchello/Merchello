using System.Collections.Generic;
using Merchello.Core.Models;

namespace Merchello.Web.Models.ContentEditing
{
    internal static class SettingsMappingExtensions
    {

        #region CountryDisplay

        internal static CountryDisplay ToCountryDisplay(this ICountry country)
        {            
            return AutoMapper.Mapper.Map<CountryDisplay>(country);
        }

        #endregion

        #region ProvinceDisplay

        internal static ProvinceDisplay ToProvinceDisplay(this IProvince province)
        {            
            return AutoMapper.Mapper.Map<ProvinceDisplay>(province);
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
						settingDisplay.CurrencyCode = setting.Value;
						break;
					case "nextOrderNumber":
						if (!int.TryParse(setting.Value, out intValue))
						{
							intValue = 0;
						}	 
						settingDisplay.NextOrderNumber = intValue;
						break;
					case "nextInvoiceNumber":
						if (!int.TryParse(setting.Value, out intValue))
						{
							intValue = 0;
						}
						settingDisplay.NextInvoiceNumber = intValue;
						break;
					case "dateFormat":
						settingDisplay.DateFormat = setting.Value;
						break;
					case "timeFormat":
						settingDisplay.TimeFormat = setting.Value;
						break;
                    case "unitSystem":
                        settingDisplay.UnitSystem = setting.Value;
                        break;
					case "globalShippable":
						if (!bool.TryParse(setting.Value, out boolValue))
						{
							boolValue = false;
						}
						settingDisplay.GlobalShippable = boolValue;
						break;
					case "globalTaxable":
						if (!bool.TryParse(setting.Value, out boolValue))
						{
							boolValue = false;
						}
						settingDisplay.GlobalTaxable = boolValue;
						break;
					case "globalTrackInventory":
						if (!bool.TryParse(setting.Value, out boolValue))
						{
							boolValue = false;
						}
						settingDisplay.GlobalTrackInventory = boolValue;
                        break;
                    case "globalShippingIsTaxable":
                        if (!bool.TryParse(setting.Value, out boolValue))
                        {
                            boolValue = false;
                        }
                        settingDisplay.GlobalShippingIsTaxable = boolValue;
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
						setting.Value = settingDisplay.CurrencyCode;			  
						break;
					case "nextOrderNumber":
						setting.Value = settingDisplay.NextOrderNumber.ToString();	  
						break;
					case "nextInvoiceNumber":
						setting.Value = settingDisplay.NextInvoiceNumber.ToString();	 
						break;
					case "dateFormat":
						setting.Value = settingDisplay.DateFormat;					 
						break;
					case "timeFormat":
						setting.Value = settingDisplay.TimeFormat;
                        break;
                    case "unitSystem":
                        setting.Value = settingDisplay.UnitSystem;
                        break;
					case "globalShippable":
						setting.Value = settingDisplay.GlobalShippable.ToString();
						break;
					case "globalTaxable":
						setting.Value = settingDisplay.GlobalTaxable.ToString();	 
						break;
					case "globalTrackInventory":
						setting.Value = settingDisplay.GlobalTrackInventory.ToString();	
						break;
                    case "globalShippingIsTaxable":
						setting.Value = settingDisplay.GlobalShippingIsTaxable.ToString();	
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
