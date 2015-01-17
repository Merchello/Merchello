/**
 * @ngdoc service
 * @name merchello.models.dialogDataFactory
 *
 * @description
 * A utility service that builds dialogData models
 */
angular.module('merchello.models').factory('dialogDataFactory',
    ['CapturePaymentDialogData', 'CreateShipmentDialogData', 'EditAddressDialogData',
    function(CapturePaymentDialogData, CreateShipmentDialogData, EditAddressDialogData) {

        // creates dialogData object for capturing a payment
        function createCapturePaymentDialogData() {
            return new CapturePaymentDialogData();
        }

        // creates dialogData for creating a shipment
        function createCreateShipmentDialogData() {
            return new CreateShipmentDialogData();
        }

        // creates dialogData for editing ShipmentDisplay
        function createEditShipmentDialogData() {
            return new EditShipmentDialogData();
        }

        // creates dialogData for editing AddressDisplay
        function createEditAddressDialogData() {
            return new EditAddressDialogData();
        }

        // creates dialogData for adding Ship Countries
        function createAddShipCountryDialogData() {
            return new AddShipCountryDialogData();
        }

        // creates dialogData for deleting ship countries
        function createDeleteShipCountryDialogData() {
            return new DeleteShipCountryDialogData();
        }

        // creates dialogData for adding providers to ship countries
        function createAddShipCountryProviderDialogData() {
            return new AddShipCountryProviderDialogData();
        }

        // creates a dialogData for deleting ship country ship methods
        function createDeleteShipCountryShipMethodDialogData() {
            return new DeleteShipCountryShipMethodDialogData();
        }

        // creates a dialogData for editing shipping gateway methods
        function createEditShippingGatewayMethodDialogData() {
            return new EditShippingGatewayMethodDialogData();
        }

        // creates a dialogData for adding or editing warehouses
        function createAddEditWarehouseDialogData() {
            return new AddEditWarehouseDialogData();
        }

        // creates a dialogData for adding or editing warehouse catalogs
        function createAddEditWarehouseCatalogDialogData() {
            return new AddEditWarehouseCatalogDialogData();
        }

        function createDeleteWarehouseCatalogDialogData() {
            return new DeleteWarehouseCatalogDialogData();
        }

        function createChangeWarehouseCatalogDialogData() {
            return new ChangeWarehouseCatalogDialogData();
        }

        return {
            createAddShipCountryDialogData: createAddShipCountryDialogData,
            createDeleteShipCountryDialogData: createDeleteShipCountryDialogData,
            createAddShipCountryProviderDialogData: createAddShipCountryProviderDialogData,
            createChangeWarehouseCatalogDialogData: createChangeWarehouseCatalogDialogData,
            createDeleteWarehouseCatalogDialogData: createDeleteWarehouseCatalogDialogData,
            createEditShippingGatewayMethodDialogData: createEditShippingGatewayMethodDialogData,
            createAddEditWarehouseCatalogDialogData: createAddEditWarehouseCatalogDialogData,
            createCapturePaymentDialogData: createCapturePaymentDialogData,
            createCreateShipmentDialogData: createCreateShipmentDialogData,
            createEditShipmentDialogData: createEditShipmentDialogData,
            createEditAddressDialogData: createEditAddressDialogData,
            createAddEditWarehouseDialogData: createAddEditWarehouseDialogData,
            createDeleteShipCountryShipMethodDialogData: createDeleteShipCountryShipMethodDialogData
        };
}]);
