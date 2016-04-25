namespace Merchello.Web.Mvc
{
    /// <summary>
    /// An UmbracoViewPage{T} that exposes a Merchello helper.
    /// </summary>
    /// <typeparam name="TModel">
    /// The type of view model
    /// </typeparam>
    public abstract class MerchelloHelperViewPage<TModel> : Umbraco.Web.Mvc.UmbracoViewPage<TModel>
    {
        /// <summary>
        /// The <see cref="MerchelloHelper"/>
        /// </summary>
        private MerchelloHelper _helper;

        /// <summary>
        /// Gets the <see cref="MerchelloHelper"/>.
        /// </summary>
        public MerchelloHelper Merchello
        {
            get
            {
                if (_helper == null)
                {
                    _helper = new MerchelloHelper();
                }
                return _helper;
            }
        }
    }
}