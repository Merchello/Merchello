namespace Merchello.Web
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Web;

    using Merchello.Core;
    using Merchello.Core.Models.Interfaces;

    using Umbraco.Core.Models;
    using Umbraco.Web;

    /// <summary>
    /// 
    /// </summary>
    public class DigitalDownloadHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            string requestKeyAsStr = null;
            //Get the key from the qs
            if (context.Request.QueryString.AllKeys.Any(x => x.ToLower() == "key"))
            {
                requestKeyAsStr = context.Request.QueryString["Key"];
            }
            else
            {
                var parts = context.Request.Url.AbsolutePath.Split('/');
                if (parts.Length > 2)
                {
                    requestKeyAsStr = parts[2];
                }
            }
            
            Guid key;
            if (string.IsNullOrWhiteSpace(requestKeyAsStr) || !Guid.TryParse(requestKeyAsStr, out key))
            {
                //parsing of the key failed
                ThrowError(context, "Non valid key in querystring");
            }
            else
            {
                //continue processing
                ProcessKey(context, key);
            }
        }

        private void ProcessKey(HttpContext context, Guid key)
        {
            var model = MerchelloContext.Current.Services.DigitalMediaService.GetByKey(key);
            
            //if Key is found in DB
            if (model == null)
            {
                ThrowError(context, "Non valid key in querystring");
                return;
            }

            IPublishedContent mediaItem = null;


            var productVariant = MerchelloContext.Current.Services.ProductVariantService.GetByKey(model.ProductVariantKey);
      
            if(productVariant != null) 
            { 
                if (!productVariant.Download || !productVariant.DownloadMediaId.HasValue)
                {
                    ThrowError(context, "Product Variant isn't available for download!");
                    return;
                }

                mediaItem = UmbracoContext.Current.MediaCache.GetById(productVariant.DownloadMediaId.Value);
            }
            

            var product = MerchelloContext.Current.Services.ProductService.GetByKey(model.ProductVariantKey);

            if (product != null) 
            {
                if (!product.Download || !product.DownloadMediaId.HasValue) 
                {
                    ThrowError(context, "Product isn't available for download!");
                    return;
                }

                mediaItem = UmbracoContext.Current.MediaCache.GetById(product.DownloadMediaId.Value);
            }
            

            //TODO: move this to config somewhere
            if(!model.FirstAccessed.HasValue && model.CreateDate.AddDays(30) > DateTime.Now)
            {
                //key is valid
                SendDownload(context, model, mediaItem);
            }
            else
            {
                if (model.FirstAccessed.HasValue && model.FirstAccessed.Value.AddDays(2) > DateTime.Now)
                {
                    // download has been used, but within secondary download period. i.e. download was started and stopped. or it failed!
                    SendDownload(context, model, mediaItem);
                }
                else
                {
                    //Key is expired
                    ThrowError(context, "key has expired");
                    return;
                }
            }
        }

        private void SendDownload(HttpContext context, IDigitalMedia model, IPublishedContent mediaItem)
        {
            
            
            //TODO: pull this from model
            string file = mediaItem.GetPropertyValue<string>("umbracoFile");
            
            var fullFilename = context.Server.MapPath(file);

            var bin = System.IO.File.ReadAllBytes(fullFilename);

            context.Response.ClearHeaders();
            context.Response.ClearContent();
            context.Response.ContentType = "application/octet-stream"; //set file type
            context.Response.AddHeader("Content-Disposition", string.Format("attachment; filename=\"{0}\"", Path.GetFileName(fullFilename)));
            context.Response.BinaryWrite(bin);
            context.Response.Flush();
            context.Response.End();

            //update database to mark it as downloaded, after the stream completes if possible
            model.FirstAccessed = DateTime.Now;
            MerchelloContext.Current.Services.DigitalMediaService.Save(model);
        }

        private void ThrowError(HttpContext ctx, string message)
        {
            ctx.Response.Clear();
            ctx.Response.Write(message);
            ctx.Response.End();
        }

        public bool IsReusable { get; private set; }
    }
}
