namespace Merchello.Core.Marketing.Coupons
{
    using Merchello.Core.Models;

    /// <summary>
    /// A base class used for defining coupon campaign activities.
    /// </summary>
    [CampaignActivity("05F735B6-C01E-4B61-863D-EFE7DF8136BF", "Coupon", "A discount associated with a coupon offer", "~/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/marketing.coupon.addedit.html")]
    public class Coupon : CampaignActivityBase, IDiscountActivity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Coupon"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public Coupon(CampaignActivitySettings settings)
            : base(settings)
        {
        }
    }
}