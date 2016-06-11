﻿namespace Merchello.Web
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

            var productVariant = MerchelloContext.Current.Services.ProductVariantService.GetByKey(model.ProductVariantKey);

            if (!productVariant.Download && productVariant.DownloadMediaId.HasValue)
            {
                ThrowError(context, "Product Variant isnt availablt for download!");
                return;
            }

            var mediaItem = UmbracoContext.Current.MediaCache.GetById(productVariant.DownloadMediaId.Value);


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

            context.Response.Clear();
            context.Response.ContentType = "application/octet-stream";//set file type
            context.Response.Buffer = false;
            context.Response.BufferOutput = false;
            //set download filename + ensure download widget (stops pdf's opening in browser!)
            context.Response.AddHeader("Content-Disposition", string.Format("attachment; filename=\"{0}\"", Path.GetFileName(fullFilename)));

            //send file down stream
            //context.Response.TransmitFile();
            context.Response.WriteFile(fullFilename);

            context.Response.Flush();
            context.Response.Close();

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
