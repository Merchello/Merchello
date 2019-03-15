namespace Merchello.FastTrack.Controllers.Payment
{
    using System.Web.Mvc;

    using Merchello.FastTrack.Models.Payment;
    using Merchello.Web.Store.Controllers.Payment;

    using Umbraco.Core;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// The FastTrack cash payment controller.
    /// </summary>
    [PluginController("FastTrack")]
    public class BankTransferController : BankTransferControllerBase<FastTrackPaymentModel>
    {

    }
}