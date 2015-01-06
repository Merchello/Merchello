(function (directives, undefined) {

    /**
     * @ngdoc directive
     * @name ShippingHistoryDirective
     * @function
     * 
     * @description
     * directive to display shipping history
     */
    directives.ShippingHistoryDirective = function(dialogService) {
        return {
            restrict: 'E',
            replace: true,
            templateUrl: '/App_Plugins/Merchello/Modules/Order/Directives/shipping-history.html',
            scope: {
                shipments: '=',
                shipmentStatuses: '='
            },
            link: function($scope, $element) {

                $scope.dialogData = {};

                $scope.editToAddress = function () {
                    console.info($scope.dialogData.address);
                };

                $scope.editFromAddress = function () {
                    console.info($scope.dialogData.address);
                };

                $scope.editAddress = function (shipment, ref) {
                    $scope.dialogData.ref = ref;
                    $scope.dialogData.address = $scope.buildAddress(shipment, ref);

                    dialogService.open({
                        template: '/App_Plugins/Merchello/Common/Js/Dialogs/edit.address.html',
                        show: true,
                        callback: ref == 'to' ? $scope.editToAddress : $scope.editFromAddress,
                        dialogData: $scope.dialogData
                    });
                }

                $scope.buildAddress = function (shipment, ref) {
                    var address = new merchello.Models.Address();
                    address.addressType = 'shipping';
                    console.info(shipment);
                    if (ref == 'from') {
                        address.name = shipment.fromName;
                        address.organization = shipment.fromOrganization;
                        address.address1 = shipment.fromAddress1;
                        address.address2 = shipment.fromAddress2;
                        address.locality = shipment.fromLocality;
                        address.region = shipment.fromRegion;
                        address.postalCode = shipment.fromPostalCode;
                        address.countryCode = shipment.fromCountryCode;
                        address.isCommercial = shipment.fromIsCommercial;
                    } else {
                        address.name = shipment.toName;
                        address.organization = shipment.toOrganization;
                        address.address1 = shipment.toAddress1;
                        address.address2 = shipment.toAddress2;
                        address.locality = shipment.toLocality;
                        address.region = shipment.toRegion;
                        address.postalCode = shipment.toPostalCode;
                        address.countryCode = shipment.toCountryCode;
                        address.isCommercial = shipment.toIsCommercial;
                        address.phone = shipment.phone;
                        address.email = shipment.email;
                    }

                    return address;
                }
            }

            
        }
    };

    angular.module("umbraco").directive('merchelloShippingHistory', merchello.Directives.ShippingHistoryDirective);

    
}(window.merchello.Directives = window.merchello.Directives || {}));

