namespace Merchello.Web.Store.Controllers.Api
{
    using Merchello.Web.Controllers.Api;
    using Merchello.Web.Models.Ui;
    using Merchello.Web.Store.Models;

    using Umbraco.Web.Mvc;

    /// <summary>
    /// An API controller responsible for handling <see cref="ProductDataTableApiControllerBase{TTable,TRow}"/> API data.
    /// </summary>
    /// <remarks>
    /// We only need to designate the types with this controller and there are no factory overrides.
    /// However, there is a constructor in <see cref="IProductDataTable{TProductDataTableRow}"/> that you can specify
    /// and override the factory.
    /// </remarks>
    [PluginController("Merchello")]
    public class ProductDataTableApiController : ProductDataTableApiControllerBase<ProductDataTable, ProductDataTableRow>
    {
    }
}