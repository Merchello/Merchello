(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Settings.VendorsController
     * @function
     * 
     * @description
     * The controller for the Vendors page
     */
    controllers.VendorsController = function ($scope, $routeParams, $location, notificationsService, angularHelper, serverValidationManager, merchelloProductService) {

        $scope.vendors = [];
        $scope.sortProperty = "name";
        $scope.sortOrder = "asc";

        $scope.loadVendors = function () {

            // just a mock of getting the vendor objects
            var mockVendors = [
                {
                    name: "Geeky Soap",
                    contact: "Anne-Marie",
                    phone: "360-647-7470",
                    address1: "114 W Magnolia St",
                    address2: "Suite 504",
                    locality: "Bellingham",
                    region: "WA",
                    postalCode: "98225",
                    country: "United States of America"
                },
                {
                    name: "Nerdy Soap",
                    contact: "Heather",
                    phone: "360-647-7470",
                    address1: "105 W Holly St H22",
                    address2: "",
                    locality: "Bellingham",
                    region: "WA",
                    postalCode: "98225",
                    country: "United States of America"
                }
            ];
            $scope.vendors = _.map(mockVendors, function (vendorFromServer) {
                return new merchello.Models.Vendor(vendorFromServer);
            });
            // end of mocks


            $scope.loaded = true;
            $scope.preValuesLoaded = true;

        };

        $scope.loadVendors();


    }


    angular.module("umbraco").controller("Merchello.Dashboards.Settings.VendorsController", merchello.Controllers.VendorsController);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
