/**
 * @ngdoc service
 * @name merchello.models.dialogDataFactory
 *
 * @description
 * A utility service that builds dialogData models
 */
angular.module('merchello.models').factory('dialogDataFactory',
    ['CapturePaymentDialogData',
    function(CapturePaymentDialogData) {

        function getCapturePaymentDialogData() {
            return new CapturePaymentDialogData();
        }

        return {
            getCapturePaymentDialogData: getCapturePaymentDialogData
        };
}]);
