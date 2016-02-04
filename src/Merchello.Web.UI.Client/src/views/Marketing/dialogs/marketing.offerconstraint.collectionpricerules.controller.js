/**
 * @ngdoc controller
 * @name Merchello.Marketing.Dialogs.OfferConstraintCollectionPriceRulesController
 * @function
 *
 * @description
 * The controller to configure the collection price component
 */
angular.module('merchello').controller('Merchello.Marketing.Dialogs.OfferConstraintCollectionPriceRulesController',
    ['$scope', 'notificationsService', 'settingsResource', 'invoiceHelper',
        function($scope, notificationsService, settingsResource, invoiceHelper) {

            $scope.loaded = false;
            $scope.operator = 'gt';
            $scope.price = 0;
            $scope.currencySymbol = '';

            // exposed methods
            $scope.save = save;

            function init() {
                loadSettings();
                loadExistingConfigurations();
            }

            function loadExistingConfigurations() {
                var operator = $scope.dialogData.getValue('operator');
                var price = $scope.dialogData.getValue('price');
                $scope.operator = operator === '' ? 'gt' : operator;
                $scope.price = price === '' ? 0 : invoiceHelper.round(price, 2);
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
                $scope.dialogData.setValue('price', Math.abs(invoiceHelper.round($scope.price*1, 2)));
                $scope.dialogData.setValue('operator', $scope.operator);
                $scope.submit($scope.dialogData);
            }

            // Initialize the controller
            init();
        }]);
