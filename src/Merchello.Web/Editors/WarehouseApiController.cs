namespace Merchello.Web.Editors
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;

    using Merchello.Core;
    using Merchello.Core.Models;
    using Merchello.Core.Services;
    using Merchello.Web.Models.ContentEditing;
    using Merchello.Web.WebApi;

    using Umbraco.Web;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// The warehouse API controller.
    /// </summary>
    [PluginController("Merchello")]
    public class WarehouseApiController : MerchelloApiController
    {
        /// <summary>
        /// The warehouse service.
        /// </summary>
        private readonly IWarehouseService _warehouseService;

        /// <summary>
        /// Initializes a new instance of the <see cref="WarehouseApiController"/> class.
        /// </summary>
        public WarehouseApiController()
            : this(MerchelloContext.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WarehouseApiController"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        public WarehouseApiController(MerchelloContext merchelloContext)
            : base(merchelloContext)
        {
            _warehouseService = MerchelloContext.Services.WarehouseService;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WarehouseApiController"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="umbracoContext">
        /// The umbraco context.
        /// </param>
        internal WarehouseApiController(MerchelloContext merchelloContext, UmbracoContext umbracoContext)
            : base(merchelloContext, umbracoContext)
        {
            _warehouseService = MerchelloContext.Services.WarehouseService;
        }

        /// <summary>
        /// Returns default warehouse for the store
        /// 
        /// GET /umbraco/Merchello/WarehouseApi/GetDefaultWarehouse
        /// </summary>
        /// <returns>
        /// The <see cref="WarehouseDisplay"/>.
        /// </returns>
        public WarehouseDisplay GetDefaultWarehouse()
        {
            IWarehouse warehouse = _warehouseService.GetDefaultWarehouse();
            if (warehouse == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return warehouse.ToWarehouseDisplay();
        }

        /// <summary>
        /// Returns warehouse from a key
        /// 
        /// GET /umbraco/Merchello/WarehouseApi/GetWarehouse/{key}
        /// </summary>
        /// <param name="id">
        /// Key of the warehouse to retrieve
        /// </param>
        /// <returns>
        /// The <see cref="WarehouseDisplay"/>.
        /// </returns>
        public WarehouseDisplay GetWarehouse(Guid id)
        {
            var warehouse = _warehouseService.GetByKey(id);

            if (warehouse == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return warehouse.ToWarehouseDisplay();
        }

        /// <summary>
        /// Returns Warehouses by keys separated by a comma
        /// 
        /// GET /umbraco/Merchello/WarehouseApi/GetWarehouses?keys={guid}&amp;keys={guid}
        /// </summary>
        /// <param name="keys">
        /// Warehouse keys to retrieve
        /// </param>
        /// <returns>
        /// The collection of warehouses.
        /// </returns>
        internal IEnumerable<WarehouseDisplay> GetWarehouses([FromUri]IEnumerable<Guid> keys)
        {
            if (keys != null)
            {
                var warehouses = _warehouseService.GetByKeys(keys);
                if (warehouses == null)
                {
                    //throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
                }

                foreach (IWarehouse warehouse in warehouses)
                {
                    yield return warehouse.ToWarehouseDisplay();
                }
            }
            else
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Parameter keys is null")),
                    ReasonPhrase = "Invalid Parameter"
                };
                throw new HttpResponseException(resp);
            }
        }

        /// <summary>
        /// Updates an existing warehouse
        /// 
        /// PUT /umbraco/Merchello/WarehouseApi/PutWarehouse
        /// </summary>
        /// <param name="warehouse">
        /// WarehouseDisplay object serialized from Web API
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        [AcceptVerbs("PUT","POST")]
        public HttpResponseMessage PutWarehouse(WarehouseDisplay warehouse)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);

            try
            {
                IWarehouse merchWarehouse = _warehouseService.GetByKey(warehouse.Key);
                merchWarehouse = warehouse.ToWarehouse(merchWarehouse);

                _warehouseService.Save(merchWarehouse);
            }
            catch (Exception ex)
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound, String.Format("{0}", ex.Message));
            }

            return response;
        }

        /// <summary>
        /// Updates an existing warehouse
        ///
        /// PUT /umbraco/Merchello/WarehouseApi/PutWarehouses
        /// </summary>
        /// <param name="warehouses">IEnumerable<WarehouseDisplay> object serialized from WebApi</param>
        [AcceptVerbs("PUT")]
        internal HttpResponseMessage PutWarehouses(IEnumerable<WarehouseDisplay> warehouses)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);

            if (warehouses != null)
            {
                try
                {
                    foreach (var warehouse in warehouses)
                    {
                        IWarehouse merchWarehouse = _warehouseService.GetByKey(warehouse.Key);
                        merchWarehouse = warehouse.ToWarehouse(merchWarehouse);

                        _warehouseService.Save(merchWarehouse);
                    }
                }
                catch (Exception ex)
                {
                    response = Request.CreateResponse(HttpStatusCode.NotFound, String.Format("{0}", ex.Message));
                }
            }
            else
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound, String.Format("Parameter warehouses in null"));
            }

            return response;
        }
    }
}
