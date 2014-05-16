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
        }
    }
}
