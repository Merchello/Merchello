using umbraco.businesslogic;
using umbraco.interfaces;
using Umbraco.Web.Mvc;

namespace Merchello.Web.Trees
{
    [Application("merchello", "Merchello", "Merchello-Icon.png", 10)] // coin
    [PluginController("Merchello")]
    public class MerchelloApp : IApplication
    {
    }
}