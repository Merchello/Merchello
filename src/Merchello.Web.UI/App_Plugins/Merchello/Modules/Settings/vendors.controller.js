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
        $scope.deleteVendor = new merchello.Models.Vendor();
        $scope.newVendor = new merchello.Models.Vendor();

        $scope.flyouts = {
            addVendor: false,
            deleteVendor: false
        };

        $scope.loadVendors = function () {

            // just a mock of getting the vendor objects
            var mockVendors = [
                {
                    key: 1,
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
                    key: 2,
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

        $scope.addVendorFlyout = {
            clear: function() {
                $scope.newVendor = new merchello.Models.Vendor();
            },
            close: function () {
                $scope.flyouts.addVendor = false;
            },
            open: function (vendor) {
                if (vendor) {
                    $scope.newVendor = vendor;
                }
                $scope.flyouts.addVendor = true;
            },
            save: function () {
                var idx = -1;
                for (i = 0; i < $scope.vendors.length; i++) {
                    if ($scope.vendors[i].key == $scope.newVendor.key) {
                        idx = i;
                    }
                }
                if (idx > -1) {
                    $scope.vendors[idx] = $scope.newVendor;
                    // Note From Kyle: An API call will need to be wired in here to edit the existing Vendor in the database.
                } else {
                    var newKey = $scope.vendors.length;
                    // Note From Kyle: This key-creation logic will need to be modified to fit whatever works for the database.
                    $scope.newVendor.key = newKey;
                    $scope.vendors.push($scope.newVendor);
                    // Note From Kyle: An API call will need to be wired in here to add the new Vendor to the database.
                }
                $scope.addVendorFlyout.clear();
                $scope.addVendorFlyout.close();
            },
            toggle: function () {
                $scope.flyouts.addVendor = !$scope.flyouts.addVendor;
            }
        };

        $scope.deleteVendorFlyout = {
            close: function () {
                $scope.flyouts.deleteVendor = false;
            },
            confirm: function() {
                var idx = -1;
                for (i = 0; i < $scope.vendors.length; i++) {
                    if ($scope.vendors[i].key == $scope.deleteVendor.key) {
                        idx = i;
                    }
                }
                if (idx > -1) {
                    $scope.vendors.splice(idx, 1);
                }
                // Note From Kyle: Some sort of logic to confirm there are no products currently associated with this vendor might be needed.
                $scope.deleteVendor = new merchello.Models.Vendor();
                // Note From Kyle: An API call will need to be wired in here to delete the Vendor in the database.
                $scope.deleteVendorFlyout.close();
            },
            open: function (vendor) {
                if (vendor) {
                    $scope.deleteVendor = vendor;
                }
                $scope.flyouts.deleteVendor = true;
            },
            toggle: function () {
                $scope.flyouts.deleteVendor = !$scope.flyouts.deleteVendor;
            }
        };

        $scope.loadVendors();


    }


    angular.module("umbraco").controller("Merchello.Dashboards.Settings.VendorsController", merchello.Controllers.VendorsController);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
