using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.PropertyEditors;

namespace Merchello.Plugin.Shipping.UPS
{
    public class Constants
    {
        public static class ExtendedDataKeys
        {
            public static string ProcessorSettings = "upsProcessorSettings";
            public static string UpsAccessKey = "upsAccessKey";
            public static string UpsUserName = "upsUserName";
            public static string UpsPassword = "upsPassword";

            //public static string NextDayAirEarlyAmServiceCode = "1DM";
            //public static string NextDayAirEarlyAmServiceType = "Next Day Air Early AM";
            public static string NextDayAirServiceCode = "01";
            public static string NextDayAirServiceType = "UPS Next Day Air";
            public static string SecondDayAirServiceCode = "02";
            public static string SecondDayAirServiceType = "UPS 2nd Day Air";
            public static string GroundServiceCode = "03";
            public static string GroundServiceType = "UPS Ground";
            public static string WorldwideExpressServiceCode = "07";
            public static string WorldwideExpressServiceType = "UPS Worldwide Express";
            public static string WorldwideExpeditedServiceCode = "08";
            public static string WorldwideExpeditedServiceType = "UPS Worldwide Expedited";
            public static string StandardServiceCode = "11";
            public static string StandardServiceType = "UPS Standard";
            public static string ThirdDaySelectServiceCode = "12";
            public static string ThirdDaySelectServiceType = "UPS 3 Day Select";
            public static string NextDayAirSaverServiceCode = "13";
            public static string NextDayAirSaverServiceType = "UPS Next Day Air Saver";
            public static string NextDayAirEarlyAmServiceCode = "14";
            public static string NextDayAirEarlyAmServiceType = "UPS Next Day Air Early A.M.";
            public static string WorldwideExpressPlusServiceCode = "54";
            public static string WorldwideExpressPlusServiceType = "UPS Worldwide Express Plus";
            public static string SecondDayAirAmServiceCode = "59";
            public static string SecondDayAirAmServiceType = "UPS 2nd Day Air A..M.";
            //public static string WorldwideSaverExpressServiceCode = "SV";
            //public static string WorldwideSaverExpressServiceType = "Worldwide Saver (Express)";
            //public static string UPSExpressServiceCode = "ND";
            //public static string UPSExpressServiceType = "UPS Express (NA1)";



        }
    }
}

