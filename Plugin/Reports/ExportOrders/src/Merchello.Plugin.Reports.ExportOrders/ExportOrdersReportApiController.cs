using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Xml.Linq;
using Merchello.Core.Models;
using Microsoft.Web.Infrastructure;
using Umbraco.Core.IO;

namespace Merchello.Plugin.Reports.ExportOrders
{
    using System.Collections.Generic;

    using Merchello.Core;
    using Merchello.Web.Models.Querying;
    using Merchello.Web.Trees;
    using Merchello.Web.Reporting;
    using Merchello.Web.WebApi;
    using Merchello.Core.Services;
    using Merchello.Web;
    

    /// <summary>
    /// The sales over time report controller.
    /// </summary>
    [BackOfficeTree("exportOrders", "reports", "Export Orders", "icon-download", "/merchello/merchello/ViewReport/Merchello.ExportOrders|ExportOrders", 10)]
    public class ExportOrdersReportApiController :  ReportController
    {
        /// <summary>
        /// The <see cref="IInvoiceService"/>.
        /// </summary>
        private readonly IInvoiceService _invoiceService;

        /// <summary>
        /// The <see cref="MerchelloHelper"/>
        /// </summary>
        private readonly MerchelloHelper _merchello;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportOrdersReportApiController"/> class.
        /// </summary>
        public ExportOrdersReportApiController()
            : this(Core.MerchelloContext.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportOrdersReportApiController"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        public ExportOrdersReportApiController(IMerchelloContext merchelloContext)
            : base(merchelloContext)
        {
            _invoiceService = merchelloContext.Services.InvoiceService;
            
            _merchello = new MerchelloHelper(merchelloContext.Services);
        }

        /// <summary>
        /// Gets the base url.
        /// GET /umbraco/Merchello/InvoiceApi/GetInvoice/{guid}

        /// </summary>
        public override KeyValuePair<string, object> BaseUrl
        {
            get
            {
                return GetBaseUrl<ExportOrdersReportApiController>("merchelloReportExportOrders");
            }
        }
         
         /// <summary> 
         /// The get default report data. 
         /// </summary> 
         /// <returns> 
         /// The <see cref="QueryResultDisplay"/>. 
         /// </returns> 
         public override QueryResultDisplay GetDefaultReportData() 
         { 
             throw new System.NotImplementedException(); 
         }

        private string FormatAddress(IAddress extendedData )
        {
            var sb = new StringBuilder();

            sb.Append(extendedData.Address1).AppendLine();
            if (extendedData.Address2.Length > 0)
            {
                sb.Append(extendedData.Address2).AppendLine();
            }
            sb.AppendFormat("{0},{1} {2}", extendedData.Locality, extendedData.Region, extendedData.PostalCode).AppendLine();
            sb.Append(extendedData.CountryCode);

            return sb.ToString();
        }

        /// <summary>
        /// The get default report data.
        /// 
        /// GET /umbraco/Merchello/ExportOrdersReportApi/GetOrderReportData/
        /// 
        /// </summary>
        /// <returns>
        /// The <see cref="QueryResultDisplay"/>.
        /// </returns>
        public QueryResultDisplay GetOrderReportData()
        {
            var dtStart = new DateTime(2014,1,1);
            var dtEnd = new DateTime(2014,12,31);
            var invoices = _invoiceService.GetInvoicesByDateRange(dtStart,dtEnd).ToArray();
            var queryResultDisplay = new QueryResultDisplay();

            try
            {
                var csvExport = new CvsExport();
                foreach (var invoice in invoices)
                {
                    csvExport.AddRow();

                    csvExport["Number"] = invoice.InvoiceNumber;
                    csvExport["Date"] = invoice.InvoiceDate;
                    csvExport["Bill To Name"] = invoice.BillToName;
                    csvExport["Bill To Company"] = invoice.BillToCompany;
                    csvExport["Bill To Address"] = invoice.BillToAddress1;
                    csvExport["Bill To Address2"] = invoice.BillToAddress2;
                    csvExport["Email"] = invoice.BillToEmail;
                    csvExport["Phone"] = invoice.BillToPhone;
                    csvExport["City"] = invoice.BillToLocality;
                    csvExport["State"] = invoice.BillToRegion;
                    csvExport["Postal Code"] = invoice.BillToPostalCode;
                    csvExport["Total"] = invoice.Total;
                    csvExport["Status"] = invoice.InvoiceStatus.Name;

                    foreach (var invoiceItems in invoice.Items)
                    {

                        foreach (var invoiceItem in invoice.Items)
                        {
                            if (invoiceItem.LineItemType == LineItemType.Product)
                            {
                                csvExport["Name"] = invoiceItem.Name;
                                csvExport["Sku"] = invoiceItem.Sku;
                                csvExport["Quantity"] = invoiceItem.Quantity;
                                csvExport["Price"] = invoiceItem.Price;
                            }
                            else if (invoiceItem.LineItemType == LineItemType.Shipping)
                            {
                                csvExport["Ship Method"] = invoiceItem.Name;
                                csvExport["Ship Quantity"] = invoiceItem.Quantity;
                                csvExport["Ship Price"] = invoiceItem.Price;

                                var origin =invoiceItem.ExtendedData.GetAddress(Constants.ExtendedDataKeys.ShippingOriginAddress);
                                var destination =invoiceItem.ExtendedData.GetAddress(Constants.ExtendedDataKeys.ShippingDestinationAddress);

                                csvExport["Ship Origin"] = FormatAddress(origin);
                                csvExport["Ship Destination"] = FormatAddress(destination);
                            }
                            else if (invoiceItem.LineItemType == LineItemType.Tax)
                            {
                                csvExport["Tax"] = invoiceItem.Name;
                                csvExport["Tax Quantity"] = invoiceItem.Quantity;
                                csvExport["Tax Price"] = invoiceItem.Price;
                            }
                            else if (invoiceItem.LineItemType == LineItemType.Discount)
                            {
                                csvExport["Coupon"] = invoiceItem.Name;
                                csvExport["Coupon Quantity"] = invoiceItem.Quantity;
                                csvExport["Coupon Price"] = invoiceItem.Price;
                            }
                        }
                    }
                }

                string path = HttpContext.Current.Server.MapPath("/orders.csv");

                csvExport.ExportToFile(path);

                queryResultDisplay = new QueryResultDisplay { Items = invoices, TotalItems = invoices.Count() };
            }
            catch (SystemException e)
            {
                string ex = e.Message;
            }

            return queryResultDisplay;
        }
    }
}