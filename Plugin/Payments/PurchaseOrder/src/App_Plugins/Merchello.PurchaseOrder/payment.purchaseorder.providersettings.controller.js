/**
 * @ngdoc controller
 * @name Merchello.Plugin.GatewayProviders.Payments.Dialogs.PurchaseOrderGatewayProviderController
 * @function
 * 
 * @description
 * The controller for managing the Purchase Order Settings on the Gateway Providers page
 */
angular.module('merchello.plugins.purchaseorder').controller('Merchello.Plugin.GatewayProviders.Payments.Dialogs.PurchaseOrderGatewayProviderController',
    ['$scope', 'purchaseorderProviderSettingsBuilder',
        function ($scope, purchaseorderProviderSettingsBuilder) {

            $scope.purchaseOrderSettings = {};

            /**
           * @ngdoc method 
           * @name init
           * @function
           * 
           * @description
           * Method called on intial page load.  Loads in data from server and sets up scope.
           */
            function init() {
                var json = JSON.parse($scope.dialogData.provider.extendedData.getValue('purchaseOrderProcessorSettings'));
                $scope.purchaseOrderSettings = purchaseorderProviderSettingsBuilder.transform(json);

                $scope.$watch(function () {
                    return $scope.purchaseOrderSettings;
                }, function (newValue, oldValue) {
                    $scope.dialogData.provider.extendedData.setValue('purchaseOrderProcessorSettings', angular.toJson(newValue));
                }, true);
            }

            // initialize the controller
            init();

        }]);