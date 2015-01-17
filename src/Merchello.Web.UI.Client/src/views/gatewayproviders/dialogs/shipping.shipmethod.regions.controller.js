    /**
     * @ngdoc controller
     * @name Merchello.GatewayProviders.Dialogs.ShipMethodRegionsController
     * @function
     *
     * @description
     * The controller for the adding / editing ship methods regions
     */
    angular.module('merchello').controller('Merchello.GatewayProviders.Dialogs.ShipMethodRegionsController',
        ['$scope', function($scope) {

            $scope.allProvinces = false;

            // exposed methods
            $scope.toggleAllProvinces = toggleAllProvinces;


            /**
             * @ngdoc method
             * @name toggleAllProvinces
             * @function
             *
             * @description
             * Toggle the provinces.
             */
            function toggleAllProvinces() {
                _.each($scope.dialogData.shippingGatewayMethod.shipMethod.provinces, function (province)
                {
                    province.allowShipping = $scope.allProvinces;
                });
            }
    }]);
