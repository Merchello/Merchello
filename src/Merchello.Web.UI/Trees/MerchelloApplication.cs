using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Web.Mvc;
using umbraco.businesslogic;
using umbraco.interfaces;

namespace Merchello.Web.UI.Trees
{
    [Application("merchello", "Merchello", "Merchello-Icon.png", 10)] // coin
    [PluginController("Merchello")]
    public class MerchelloApp : IApplication
    {
    }
}