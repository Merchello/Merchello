using System.Web.Mvc;
using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Web;
using Umbraco.Web.Mvc;

namespace Controllers
{
    /// <summary>
    /// SurfaceController responsible for shipment related interactions
    /// </summary>
    [PluginController("RosettaStone")]
    public class ShipmentController : MerchelloSurfaceContoller
    {
        public ShipmentController()
            : this(MerchelloContext.Current)
         {}
        
        public ShipmentController(IMerchelloContext merchelloContext) 
            : base(merchelloContext)
        { }

        /// <summary>
        /// Renders the ShipmentSummary Partial View
        /// </summary>
        /// <param name="shipment">The <see cref="IShipment"/> to be displayed</param>
        [ChildActionOnly]
        public ActionResult RenderShipmentSummary(IShipment shipment)
        {            
            return PartialView("ShipmentSummary", shipment);
        }
    }
}