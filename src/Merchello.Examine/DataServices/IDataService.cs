using System.Collections.Generic;

namespace Merchello.Examine.DataServices
{
    public interface IDataService
    {
        IProductDataService ProductDataService { get; }
        IInvoiceDataService InvoiceDataService { get; }
        ILogService LogService { get; }


        string MapPath(string virtualPath);
    }
}