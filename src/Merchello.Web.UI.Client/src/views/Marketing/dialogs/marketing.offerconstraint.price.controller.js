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

            /**
             * @ngdoc method
             * @name save
             * @function
             *
             * @description
             * Saves the configuration
             */
            function save() {
                $scope.dialogData.setValue('price', $scope.price);
                $scope.dialogData.setValue('operator', $scope.operator);

                $scope.submit($scope.dialogData);
            }

            // Initialize the controller
            init();
        }]);
