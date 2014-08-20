(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Settings.Shipping.Dialogs.ShippingProviderController
     * @function
     * 
     * @description
     * The controller for the adding / editing shipping providers on the Shipping page
     */
    controllers.ShippingProviderController = function ($scope) {

        /**
        * @ngdoc method
        * @name init
        * @function
        * 
        * @description
        * Performs any needed functions when the $scope is initialized.
        */
        $scope.init = function() {
            $scope.prepareDropdown('providers');
        };

        /**
        * @ngdoc method
        * @name isProviderSelected
        * @function
        * 
        * @description
        * If the provider has been selected, return true. Otherwise, return false.
        */
        $scope.isProviderSelected = function() {
            var result = false;
            if ($scope.dialogData.provider.key !== '-1') {
                result = true;
            }
            return result;
        };

        /**
        * @ngdoc method
        * @name prepareDropdown
        * @function
        * 
        * @description
        * Configure the dropdown of the selected type to have a non-blank default option indicating the need to select an option.
        * If no type is provided, use 'providers' as the type.
        */
        $scope.prepareDropdown = function (type) {
            if (!type) {
                type = 'providers';
            }
            if (type === 'providers') {
                if ($scope.dialogData.availableProviders[0].key !== '-1') {
                    $scope.dialogData.availableProviders.unshift({
                        key: '-1',
                        name: '------'
                    });
                }
                $scope.dialogData.provider = $scope.dialogData.availableProviders[0];
            } else if (type === 'resources') {
                if ($scope.dialogData.provider.resources[0].serviceCode !== '-1')
                {
                    $scope.dialogData.provider.resources.unshift({
                        serviceCode: '-1',
                        name: '------'
                    });
                }
                $scope.dialogData.resource = $scope.dialogData.provider.resources[0];
            }
        };

        /**
        * @ngdoc method
        * @name prepareDropdown
        * @function
        * 
        * @description
        * Submit the form to save the change if the provider and resource are both selected.
        */
        $scope.save = function () {
            if ($scope.dialogData.provider.key !== '-1') {
                if ($scope.dialogData.resource.serviceCode !== '-1') {
                    $scope.dialogData.provider.resources.splice(0, 1);
                    $scope.submit($scope.dialogData);
                }
            }
        };

        $scope.init();

    };

    angular.module("umbraco").controller("Merchello.Dashboards.Settings.Shipping.Dialogs.ShippingProviderController", ['$scope', merchello.Controllers.ShippingProviderController]);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
