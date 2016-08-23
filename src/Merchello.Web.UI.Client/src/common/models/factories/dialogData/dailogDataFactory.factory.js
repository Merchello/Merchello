/**
 * @ngdoc service
 * @name merchello.models.dialogDataFactory
 *
 * @description
 * A utility service that builds dialogData models
 */
angular.module('merchello.models').factory('dialogDataFactory',
    [
    function() {

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

        function createEditTaxCountryDialogData() {
            return new EditTaxCountryDialogData();
        }

        function createDeletePaymentMethodDialogData() {
            return new DeletePaymentMethodDialogData();
        }

        // creates a dialog data model for adding or editing a notification method
        function createAddEditNotificationMethodDialogData() {
            return new AddEditNotificationMethodDialogData();
        }

        // creates a dialog data model for deleting a notification method
        function createDeleteNotificationMethodDialogData() {
            return new DeleteNotificationMethodDialogData();
        }

        // creates a dialog data model for adding and editing a notification message
        function createAddEditNotificationMessageDialogData() {
            return new AddEditNotificationMessageDialogData();
        }

        // creates a dialog data model for deleting a notification message
        function createDeleteNotificationMessageDialogData() {
            return new DeleteNotificationMessageDialogData();
        }

        // creates a dialog data model for adding or updating a customer
        function createAddEditCustomerDialogData() {
            return new AddEditCustomerDialogData();
        }

        // creates a dialog data model for deleting a customer
        function createDeleteCustomerDialogData() {
            return new DeleteCustomerDialogData();
        }

        // creates a dialog data model for adding or updating customer addresses
        function createAddEditCustomerAddressDialogData() {
            return new AddEditCustomerAddressDialogData();
        }

        // creates a dialog data model for deleting a customer address
        function createDeleteCustomerAddressDialogData() {
            return new DeleteCustomerAddressDialogData();
        }

        // creates a dialog data model for deleting a product dialog
        function createDeleteProductDialogData() {
            return new DeleteProductDialogData();
        }

        // Product Bulk Actions

        // creates a dialog data model for bulk action update product variant pricing
        function createBulkVariantChangePricesDialogData() {
            return new BulkVariantChangePricesDialogData();
        }

        // creates a dialog data module for bulk action update of product inventories
        function createBulkEditInventoryCountsDialogData() {
            return new BulkEditInventoryCountsDialogData();
        }

        // creates a dialog data for voiding payments
        function createProcessVoidPaymentDialogData() {
            return new ProcessVoidPaymentDialogData();
        }

        // creates a dialog data for refunding payments
        function createProcessRefundPaymentDialogData() {
            return new ProcessRefundPaymentDialogData();
        }

        // creates a dialog data for adding new payments
        function createAddPaymentDialogData() {
            return new AddPaymentDialogData();
        }

        // Marketing
        function createSelectOfferProviderDialogData() {
            return new SelectOfferProviderDialogData();
        }

        // offer components
        function createConfigureOfferComponentDialogData() {
            return new ConfigureOfferComponentDialogData();
        }

        function createAddEditEntityStaticCollectionDialog() {
            return new AddEditEntityStaticCollectionDialog();
        }

        // detached content
        function createEditDetachedContentTypeDialogData() {
            return new EditDetachedContentTypeDialogData();
        }

        function createAddDetachedContentTypeDialogData() {
            return new AddDetachedContentTypeDialogData();
        }

        /*----------------------------------------------------------------------------------------
        Property Editors
        -------------------------------------------------------------------------------------------*/

        function createProductSelectorDialogData() {
            return new ProductSelectorDialogData();
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
            createDeleteShipCountryShipMethodDialogData: createDeleteShipCountryShipMethodDialogData,
            createEditTaxCountryDialogData: createEditTaxCountryDialogData,
            createDeletePaymentMethodDialogData: createDeletePaymentMethodDialogData,
            createAddEditNotificationMethodDialogData: createAddEditNotificationMethodDialogData,
            createDeleteNotificationMethodDialogData: createDeleteNotificationMethodDialogData,
            createAddEditNotificationMessageDialogData: createAddEditNotificationMessageDialogData,
            createDeleteNotificationMessageDialogData: createDeleteNotificationMessageDialogData,
            createAddEditCustomerDialogData: createAddEditCustomerDialogData,
            createDeleteCustomerDialogData: createDeleteCustomerDialogData,
            createAddEditCustomerAddressDialogData: createAddEditCustomerAddressDialogData,
            createDeleteCustomerAddressDialogData: createDeleteCustomerAddressDialogData,
            createDeleteProductDialogData: createDeleteProductDialogData,
            createBulkVariantChangePricesDialogData: createBulkVariantChangePricesDialogData,
            createBulkEditInventoryCountsDialogData: createBulkEditInventoryCountsDialogData,
            createProductSelectorDialogData: createProductSelectorDialogData,
            createProcessVoidPaymentDialogData: createProcessVoidPaymentDialogData,
            createProcessRefundPaymentDialogData: createProcessRefundPaymentDialogData,
            createAddPaymentDialogData: createAddPaymentDialogData,
            createSelectOfferProviderDialogData: createSelectOfferProviderDialogData,
            createConfigureOfferComponentDialogData: createConfigureOfferComponentDialogData,
            createAddEditEntityStaticCollectionDialog: createAddEditEntityStaticCollectionDialog,
            createEditDetachedContentTypeDialogData: createEditDetachedContentTypeDialogData,
            createAddDetachedContentTypeDialogData: createAddDetachedContentTypeDialogData
        };
}]);
