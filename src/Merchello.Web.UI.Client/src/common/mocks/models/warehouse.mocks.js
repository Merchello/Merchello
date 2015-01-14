angular.module('merchello.mocks')
    .factory('warehouseMocks', [function() {

        function defaultWarehouse() {
            return {"key":"268d4007-8853-455a-89f7-a28398843e5f","name":"Default Warehouse","address1":"411 W. Magnolia","address2":null,"locality":"Bellingham","region":"WA","postalCode":"98225","countryCode":"US","phone":null,"email":null,"isDefault":true,"warehouseCatalogs":[{"key":"b25c2b00-578e-49b9-bea2-bf3712053c63","warehouseKey":"268d4007-8853-455a-89f7-a28398843e5f","name":"Default Catalog","description":null,"isDefault":true}]};
        }

        return {
            defaultWarehouse: defaultWarehouse
        };
    }]);
