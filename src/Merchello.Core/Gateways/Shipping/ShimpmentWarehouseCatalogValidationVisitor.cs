using System;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Models;
using Merchello.Core.Models.Interfaces;

namespace Merchello.Core.Gateways.Shipping
{

    internal class ShimpmentWarehouseCatalogValidationVisitor : ILineItemVisitor
    {
        
        private Guid _warehouseCatalogKey;
        private ShipmentCatalogValidationStatus _validationStatus = ShipmentCatalogValidationStatus.ErrorNoCatalogFound;

        public void Visit(ILineItem lineItem)
        {
            if (!lineItem.ExtendedData.ContainsWarehouseCatalogKey()) return;

            var key = lineItem.ExtendedData.GetWarehouseCatalogKey();

            if (_validationStatus == ShipmentCatalogValidationStatus.ErrorNoCatalogFound)
            {
                _validationStatus = ShipmentCatalogValidationStatus.Ok;
                _warehouseCatalogKey = key;
            }
            else if(_validationStatus == ShipmentCatalogValidationStatus.Ok &&  !_warehouseCatalogKey.Equals(key))
            {
                _validationStatus = ShipmentCatalogValidationStatus.ErrorMultipleCatalogs;
            }
        }

        public enum ShipmentCatalogValidationStatus
        {            
            Ok,
            ErrorNoCatalogFound,
            ErrorMultipleCatalogs
        }

        public ShipmentCatalogValidationStatus CatalogValidationStatus
        {
            get { return _validationStatus; }
        }

        public Guid WarehouseCatalogKey {
            get { return _warehouseCatalogKey; }
        }
       
    }
}