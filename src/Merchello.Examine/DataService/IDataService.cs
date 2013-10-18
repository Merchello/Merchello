using Merchello.Core.Services;

namespace Merchello.Examine.DataService
{
    public interface IDataService
    {
        IProductVariantService ProductVariantService { get; }

        string MapPath(string virtualPath);
    }
}