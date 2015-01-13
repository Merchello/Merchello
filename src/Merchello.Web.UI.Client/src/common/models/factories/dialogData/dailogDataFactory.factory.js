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

        return {
            createCapturePaymentDialogData: createCapturePaymentDialogData,
            createCreateShipmentDialogData: createCreateShipmentDialogData,
            createEditShipmentDialogData: createEditShipmentDialogData,
            createEditAddressDialogData: createEditAddressDialogData
        };
}]);
