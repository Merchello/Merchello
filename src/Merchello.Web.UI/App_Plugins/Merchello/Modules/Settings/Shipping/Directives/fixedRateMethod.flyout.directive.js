(function (directives, undefined) {

    /**
     * @ngdoc directive
     * @name FixedRateMethodFlyoutDirective
     * @function
     * 
     * @description
     * directive to help with the method add/edit flyout per provider
     */
    directives.FixedRateMethodFlyoutDirective = function () {
        return {
            require: '^shippingCountry',
            restrict: 'EA',
            scope: {
                country: '=',
                provider: '=',
                isVisible: '=',
                confirm: '&onConfirm'
            },
            templateUrl: '/App_Plugins/Merchello/Modules/Settings/Shipping/Directives/fixed-rate-method-flyout.html',
            controller: function ($scope) {
                $scope.flyoutModel = new merchello.Models.Flyout(
                    $scope.isVisible,
                    function (isOpen) {
                        $scope.isVisible = isOpen;
                    },
                    {
                        confirm: function () {
                            var self = $scope.flyoutModel;

                            var selectedMethod = self.model;

                            $scope.confirm(selectedMethod);

                            self.clear();
                            self.close();
                        }
                    });

                //$scope.$on('methodFlyoutOpen', function (event, model) {
                //    $scope.flyoutModel.open(model);
                //});
            },
            link: function ($scope, $element, $attrs, shippingMethodController) {

                shippingMethodController.flyoutModel = $scope.flyoutModel;

                $scope.$on('methodFlyoutOpen', function (event, model) {
                    $scope.flyoutModel.open(model);
                });
            }

        };
    }

    angular.module("umbraco").directive('fixedRateMethodFlyout', merchello.Directives.FixedRateMethodFlyoutDirective);


    /**
     * @ngdoc directive
     * @name FlyoutButton
     * @function
     * 
     * @description
     * directive to help open of a flyout and setting the model properly
     */
    directives.FlyoutButton = function () {
        return {
            require: '^shippingCountry',
            restrict: 'A',
            scope: {
                flyoutModel: '=',
                openFlyout: '&',
            },
            link: function ($scope, $element, $attrs, shippingMethodController) {

                $element.on('click', function (event) {
                    shippingMethodController.open($scope.flyoutModel);
                });
            }
        };
    }

    angular.module("umbraco").directive('flyoutButton', merchello.Directives.FlyoutButton);

}(window.merchello.Directives = window.merchello.Directives || {}));

