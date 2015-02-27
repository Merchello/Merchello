    /**
     * @ngdoc controller
     * @name Merchello.GatewayProviders.Dialogs.ShippingFixedRateShipMethodController
     * @function
     *
     * @description
     * The controller for configuring a fixed rate ship method
     */
    angular.module('merchello').controller('Merchello.GatewayProviders.Dialogs.ShippingFixedRateShipMethodController',
        ['$scope', 'notificationsService',
        'shippingFixedRateProviderResource', 'shippingGatewayProviderResource',
        'shipFixedRateTableDisplayBuilder', 'shipRateTierDisplayBuilder',
        function($scope, notificationsService, shippingFixedRateProviderResource, shippingGatewayProviderResource,
                 shipFixedRateTableDisplayBuilder, shipRateTierDisplayBuilder) {

            $scope.loaded = false;
            $scope.isAddNewTier = false;
            $scope.newTier = {};
            $scope.filters = {};
            $scope.rateTable = {}; //shipFixedRateTableDisplayBuilder.createDefault();
            $scope.rateTable.shipMethodKey = ''; //$scope.dialogData.method.key;


            // exposed methods
            $scope.addRateTier = addRateTier;
            $scope.insertRateTier = insertRateTier;
            $scope.cancelRateTier = cancelRateTier;
            $scope.removeRateTier = removeRateTier;
            $scope.save = save;

            /**
             * @ngdoc method
             * @name init
             * @function
             *
             * @description
             * Runs when the scope is initialized.
             */
            function init() {
                getRateTable();
            }

            /**
             * @ngdoc method
             * @name getRateTableIfRequired
             * @function
             *
             * @description
             * Get the rate table if it exists.
             */
            function getRateTable() {
                var promise = shippingFixedRateProviderResource.getRateTable($scope.dialogData.shippingGatewayMethod.shipMethod);
                promise.then(function(rateTable) {
                    $scope.rateTable = shipFixedRateTableDisplayBuilder.transform(rateTable);
                    $scope.rateTable.shipMethodKey = $scope.dialogData.shippingGatewayMethod.getKey();
                    $scope.loaded = true;
                }, function(reason) {
                    notificationsService.error('Could not retrieve rate table', reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name addRateTier
             * @function
             *
             * @description
             * Adds the edited, new rate tier to the method.
             */
            function addRateTier() {
                $scope.rateTable.addRow($scope.newTier);
                $scope.isAddNewTier = false;
            }

            /**
             * @ngdoc method
             * @name insertRateTier
             * @function
             *
             * @description
             * Inserts a new, blank row in the rate table.
             */
            function insertRateTier() {
                $scope.isAddNewTier = true;
                $scope.newTier = shipRateTierDisplayBuilder.createDefault();
            }

            /**
             * @ngdoc method
             * @name cancelRateTier
             * @function
             *
             * @description
             * Cancels the insert of a new blank row in the rate table.
             */
            function cancelRateTier() {
                $scope.isAddNewTier = false;
                $scope.newTier = {};
            }


            /**
             * @ngdoc method
             * @name removeRateTier
             * @function
             *
             * @description
             * Remove a rate tier from the method.
             */
            function removeRateTier(tier) {
                $scope.rateTable.removeRow(tier);
            }

            /**
             * @ngdoc method
             * @name save
             * @function
             *
             * @description
             * Saves the rate table and then submits.
             */
            function save() {
                var promiseSaveRateTable = shippingFixedRateProviderResource.saveRateTable($scope.rateTable);
                promiseSaveRateTable.then(function() {
                    $scope.submit($scope.dialogData);
                }, function(reason) {
                    notificationsService.error('Rate Table Save Failed', reason.message);
                });
            }

            // Initializes the controller
            init();
    }]);
