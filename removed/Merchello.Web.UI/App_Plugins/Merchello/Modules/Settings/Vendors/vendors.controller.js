(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Settings.VendorsController
     * @function
     * 
     * @description
     * The controller for the Vendors page
     */
    controllers.VendorsController = function ($scope) {

        $scope.vendors = [];
        $scope.sortProperty = "name";
        $scope.sortOrder = "asc";
        $scope.flyouts = {
            addVendor: false,
            deleteVendor: false
        };

        $scope.loadVendors = function () {

            // just a mock of getting the vendor objects
            var mockVendors = [
                {
                    key: 0,
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
                    key: 1,
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

        $scope.addVendorFlyout = new merchello.Models.Flyout(
            $scope.flyouts.addVendor,
            function (isOpen) {
                $scope.flyouts.addVendor = isOpen;
            },
            {
                clear: function () {
                    var self = $scope.addVendorFlyout;
                    self.model = new merchello.Models.Vendor();
                },
                confirm: function () {
                    var self = $scope.addVendorFlyout;
                    if ((typeof self.model.key) == "undefined") {
                        var newKey = $scope.vendors.length;
                        // Note From Kyle: This key-creation logic will need to be modified to fit whatever works for the database.
                        $scope.newVendor.key = newKey;
                        $scope.vendors.push(self.model);
                        // Note From Kyle: An API call will need to be wired in here to add the new Vendor to the database.
                    } else {
                        // Note From Kyle: An API call will need to be wired in here to edit the existing Vendor in the database.
                    }
                    self.clear();
                    self.close();
                },
                open: function (model) {
                    if (!model) {
                        $scope.addVendorFlyout.clear();
                    }
                }
            });

        $scope.deleteVendorFlyout = new merchello.Models.Flyout(
            $scope.flyouts.deleteVendor,
            function(isOpen) {
                $scope.flyouts.deleteVendor = isOpen;
            },
            {
                clear: function () {
                    var self = $scope.deleteVendorFlyout;
                    self.model = new merchello.Models.Vendor();
                },
                confirm: function () {
                    var self = $scope.deleteVendorFlyout;
                    var idx = -1;
                    for (i = 0; i < $scope.vendors.length; i++) {
                        if ($scope.vendors[i].key == self.model.key) {
                            idx = i;
                        }
                    }
                    if (idx > -1) {
                        $scope.vendors.splice(idx, 1);
                        // Note From Kyle: An API call will need to be wired in here to delete the Vendor in the database.
                    }
                    self.clear();
                    self.close();
                },
                open: function (model) {
                    if (!model) {
                        $scope.deleteVendorFlyout.clear();
                    }
                }
            });

        $scope.loadVendors();

    }


    angular.module("umbraco").controller("Merchello.Dashboards.Settings.VendorsController", ['$scope', merchello.Controllers.VendorsController]);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
