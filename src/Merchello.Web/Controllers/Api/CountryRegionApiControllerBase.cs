namespace Merchello.Web.Controllers.Api
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web.Http;

	using Merchello.Core.Services;
	using Merchello.Web;
	using Merchello.Web.Factories;
	using Merchello.Web.Models.Ui;

	using Umbraco.Core;
	using Umbraco.Web.WebApi;
	using Core.Models;

	/// <summary>
	/// An API controller for handling country regions.
	/// </summary>
	[Merchello.Web.WebApi.JsonCamelCaseFormatter]
    public abstract class CountryRegionApiControllerBase : UmbracoApiController
    {
        /// <summary>
        /// The <see cref="MerchelloHelper"/>.
        /// </summary>
        private readonly MerchelloHelper _merchelloHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="CountryRegionApiControllerBase"/> class.
        /// </summary>
        protected CountryRegionApiControllerBase()
            : this(new MerchelloHelper())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CountryRegionApiControllerBase"/> class.
        /// </summary>
        /// <param name="merchelloHelper">
        /// The <see cref="MerchelloHelper"/>.
        /// </param>
        protected CountryRegionApiControllerBase(
            MerchelloHelper merchelloHelper)
        {
            Mandate.ParameterNotNull(merchelloHelper, "merchell");

            this._merchelloHelper = merchelloHelper;
        }

		/// <summary>
		/// Gets a collection of <see cref="IProvince"/>.
		/// </summary>
		/// <param name="countryCode">
		/// The country code.
		/// </param>
		/// <returns>
		/// The <see cref="IEnumerable{IProvince}"/>.
		/// </returns>
		[HttpPost]
        public virtual IEnumerable<IProvince> PostGetRegionsForCountry(string countryCode)
        {	
			return StoreSettingService.GetProvincesByCountryCode(countryCode);
		}
    }
}