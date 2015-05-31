/**
 * @ngdoc controller
 * @name Merchello.Marketing.Dialogs.OfferConstraintPriceController
 * @function
 *
 * @description
 * The controller to configure the price component constraint
 */
angular.module('merchello').controller('Merchello.Marketing.Dialogs.OfferConstraintPriceController',
    ['$scope', 'settingsResource',
        function($scope, settingsResource) {

            $scope.loaded = false;
            $scope.operator = 'gt';
            $scope.price = 0;
            $scope.currencySymbol = '';

            // exposed methods
            $scope.save = save;

            function init() {
                loadSettings();
                console.info($scope.dialogData);
            }

            /**
             * @ngdoc method
             * @name loadSettings
             * @function
             *
             * @description
             * Load the settings from the settings service to get the currency symbol
             */
            function loadSettings() {
                var currencySymbolPromise = settingsResource.getCurrencySymbol();
                currencySymbolPromise.then(function (currencySymbol) {
                    $scope.currencySymbol = currencySymbol;
                    $scope.loaded = true;
                }, function (reason) {
                    notificationsService.error("Settings Load Failed", reason.message);
                });
            }

            function save() {
                console.info('got here');
                //$scope.dialogData.component.extendedData.setValue('price', $scope.price);
                //$scope.dialogData.component.extendedData.setValue('operator', $scope.operator);
                //console.info($scope.dialogData.component.extendedData);
                //$scope.submit(dialogData);
            }

            // Initialize the controller
            init();
        }]);
