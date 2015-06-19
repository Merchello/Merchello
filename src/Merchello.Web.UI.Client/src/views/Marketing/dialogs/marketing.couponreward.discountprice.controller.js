/**
 * @ngdoc controller
 * @name Merchello.Marketing.Dialogs.OfferRewardCouponDiscountPriceController
 * @function
 *
 * @description
 * The controller to configure the discount for a coupon line item reward
 */
angular.module('merchello').controller('Merchello.Marketing.Dialogs.OfferRewardCouponDiscountPriceController',
    ['$scope', 'settingsResource', 'invoiceHelper',
        function($scope, settingsResource, invoiceHelper) {
            $scope.loaded = false;
            $scope.adjustmentType = 'flat';
            $scope.currencySymbol = '';
            $scope.amount = 0;

            // exposed methods
            $scope.save = save;

            function init() {
                loadSettings();
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
                    if ($scope.dialogData.component.isConfigured()) {
                        loadExistingConfigurations();
                    } else {
                        $scope.loaded = true;
                    }
                }, function (reason) {
                    notificationsService.error("Settings Load Failed", reason.message);
                });
            }

            function loadExistingConfigurations() {
                var amount = $scope.dialogData.getValue('amount');
                var adjustmentType = $scope.dialogData.getValue('adjustmentType');
                $scope.adjustmentType = adjustmentType === '' ? 'flat' : adjustmentType;
                $scope.amount = amount === '' ? 0 : invoiceHelper.round(amount, 2);
                $scope.loaded = true;
            }

            function save() {
                if ($scope.priceAdjustForm.$valid) {
                    $scope.dialogData.setValue('amount', Math.abs(invoiceHelper.round($scope.amount*1, 2)));
                    $scope.dialogData.setValue('adjustmentType', $scope.adjustmentType);
                    $scope.submit($scope.dialogData);
                }
            }

            // Initialize
            init();
        }]);

