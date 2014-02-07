using System;
using Merchello.Core.Models;

namespace Merchello.Core.Gateways.Shipping
{

    internal class WarehouseCatalogValidationVisitor : ILineItemVisitor
    {
        
        private Guid _warehouseCatalogKey;
        private CatalogValidationStatus _catalogValidationStatus = CatalogValidationStatus.ErrorNoCatalogFound;

        public void Visit(ILineItem lineItem)
        {
            if (!lineItem.ExtendedData.ContainsWarehouseCatalogKey()) return;

            var key = lineItem.ExtendedData.GetWarehouseCatalogKey();

            if (_catalogValidationStatus == CatalogValidationStatus.ErrorNoCatalogFound)
            {
                _catalogValidationStatus = CatalogValidationStatus.Ok;
                _warehouseCatalogKey = key;
            }
            else if (_catalogValidationStatus == CatalogValidationStatus.Ok && !_warehouseCatalogKey.Equals(key))
            {
                _catalogValidationStatus = CatalogValidationStatus.ErrorMultipleCatalogs;
            }
        }

        public enum CatalogValidationStatus
        {            
            Ok,
            ErrorNoCatalogFound,
            ErrorMultipleCatalogs
        }

        public CatalogValidationStatus CatalogCatalogValidationStatus
        {
            get { return _catalogValidationStatus; }
        }

        public Guid WarehouseCatalogKey {
            get { return _warehouseCatalogKey; }
        }
       
    }
}