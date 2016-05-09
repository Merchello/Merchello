namespace Merchello.Implementation.Controllers.Api
{
    using Merchello.Implementation.Controllers.Base.Api;
    using Merchello.Implementation.Models;

    using Umbraco.Web.Mvc;

    /// <summary>
    /// An API controller responsible for handling <see cref="IProductDataTable"/> API data.
    /// </summary>
    /// <remarks>
    /// We only need to designate the types with this controller and there are no factory overrides.
    /// However, there is a constructor in <see cref="ProductDataTableApiControllerBase{TTable, TRow}"/> that you can specify
    /// and override the factory.
    /// </remarks>
    [PluginController("Merchello")]
    public class DefaultProductDataTableApiController : ProductDataTableApiControllerBase<ProductDataTable, ProductDataTableRow>
    {
    }
}