using System.Collections.Generic;

namespace Merchello.Examine.DataServices
{
    public interface IDataService
    {
        IProductDataService ProductDataService { get; }
        IInvoiceDataService InvoiceDataService { get; }
        IOrderDataService OrderDataService { get; }
        ILogService LogService { get; }


        string MapPath(string virtualPath);
    }
}