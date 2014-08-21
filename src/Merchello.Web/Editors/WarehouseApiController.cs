namespace Merchello.Web.Editors
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using Core;
    using Core.Models;
    using Core.Services;
    using Models.ContentEditing;    
    using Umbraco.Web;
    using Umbraco.Web.Mvc;
    using WebApi;

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
            : this(Core.MerchelloContext.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WarehouseApiController"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        public WarehouseApiController(IMerchelloContext merchelloContext)
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
        internal WarehouseApiController(IMerchelloContext merchelloContext, UmbracoContext umbracoContext)
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
        [AcceptVerbs("PUT", "POST")]
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
                response = Request.CreateResponse(HttpStatusCode.NotFound, string.Format("{0}", ex.Message));
            }

            return response;
        }

        /// <summary>
        /// Returns the collection warehouse catalogs for a given warehouse key
        /// 
        /// GET /umbraco/Merchello/WarehouseApi/GetWarehouseCatalogs/{key}
        /// </summary>
        /// <param name="id">
        /// Key of the warehouse return catalogs
        /// </param>
        /// <returns>
        /// A collection of <see cref="WarehouseCatalogDisplay"/>.
        /// </returns>
        [AcceptVerbs("GET")]
        public IEnumerable<WarehouseCatalogDisplay> GetWarehouseCatalogs(Guid id)
        {
            return _warehouseService.GetWarhouseCatalogByWarehouseKey(id).Select(x => x.ToWarehouseCatalogDisplay());
        }


        /// <summary>
        /// Adds warehouse catalog.
        /// POST /umbraco/Merchello/WarehouseApi/AddWarehouseCatalog
        /// </summary>
        /// <param name="catalog">
        /// The catalog.
        /// </param>
        /// <returns>
        /// The <see cref="WarehouseCatalogDisplay"/>.
        /// </returns>
        /// <exception cref="InvalidDataException">
        /// Throws an exception if the warehouse catalog key is null
        /// </exception>
        [AcceptVerbs("POST")]
        public WarehouseCatalogDisplay AddWarehouseCatalog(WarehouseCatalogDisplay catalog)
        {
            if (catalog.WarehouseKey.Equals(Guid.Empty)) throw new InvalidDataException("The warehouse key must be assigned");

            var warehouseCatalog = _warehouseService.CreateWarehouseCatalogWithKey(catalog.WarehouseKey, catalog.Name, catalog.Description);

            return warehouseCatalog.ToWarehouseCatalogDisplay();
        }

        /// <summary>
        /// Saves a warehouse catalog.
        /// PUT /umbraco/Merchello/WarehouseApi/PutWarehouseCatalog
        /// </summary>
        /// <param name="catalog">
        /// The catalog.
        /// </param>
        /// <returns>
        /// The <see cref="WarehouseCatalogDisplay"/>.
        /// </returns>
        [AcceptVerbs("POST", "PUT")]
        public WarehouseCatalogDisplay PutWarehouseCatalog(WarehouseCatalogDisplay catalog)
        {
            var warehouseCatalog = _warehouseService.GetWarehouseCatalogByKey(catalog.Key);

            warehouseCatalog = catalog.ToWarehouseCatalog(warehouseCatalog);

            _warehouseService.Save(warehouseCatalog);

            return warehouseCatalog.ToWarehouseCatalogDisplay();
        }

        /// <summary>
        /// GET /umbraco/Merchello/WarehouseApi/DeleteWarehouseCatalog/{id}
        /// Deletes a warehouse catalog.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        [AcceptVerbs("GET", "POST")]
        public HttpResponseMessage DeleteWarehouseCatalog(Guid id)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);

            if (Core.Constants.DefaultKeys.Warehouse.DefaultWarehouseCatalogKey.Equals(id))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, new InvalidOperationException("Cannot delete the default warehouse catalog."));
            }

            var catalog = _warehouseService.GetWarehouseCatalogByKey(id);

            _warehouseService.Delete(catalog);

            return response;
        }
    }
}
