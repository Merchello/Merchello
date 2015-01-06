(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Settings.Shipping.Dialogs.ShippingEditWarehouseController
     * @function
     * 
     * @description
     * The controller for adding / editing a warehouse
     */
	controllers.ShippingEditWarehouseController = function ($scope) {

	    /**
         * @ngdoc method
         * @name buildRegionOptions
         * @function
         * 
         * @description
         * Build the region dropdown dialog options, and map the currently selected one to the current region for the warehouse if one exists.
         */
	    $scope.buildRegionOptions = function () {
	        var currentRegion = false;
	        if ($scope.dialogData.warehouse.region != "" && $scope.dialogData.warehouse.region != null) {
	            currentRegion = $scope.dialogData.warehouse.region;
	        }
	        for (var i = 0; i < $scope.regions.length; i++) {
	            var newOption = { id: i, name: $scope.regions[i].name };
                $scope.options.regions.push(newOption);
                if (currentRegion === $scope.regions[i].abbr) {
                    $scope.filters.region = $scope.options.regions[i];
                }
	        }
            if ($scope.filters.region === false) {
                $scope.filters.region = $scope.options.regions[0];
            }
	    };

	    /**
         * @ngdoc method
         * @name init
         * @function
         * 
         * @description
         * Toggles the isUsOrCanada scope variable to true or false depending on the currently selected country code.
         */
	    $scope.checkIfCountryIsUsOrCanada = function() {
	        var countryCode = $scope.dialogData.warehouse.countryCode;
            if (countryCode === "US" || countryCode === "CA") {
                $scope.isUsOrCanada = true;
            } else {
                $scope.isUsOrCanada = false;
            }
	    }

	    /**
         * @ngdoc method
         * @name init
         * @function
         * 
         * @description
         * Fires when the scope is loaded.
         */
	    $scope.init = function () {
	        $scope.resetVariables();
	        $scope.buildRegionOptions();
	        $scope.checkIfCountryIsUsOrCanada();
	    };

	    /**
         * @ngdoc method
         * @name resetVariables
         * @function
         * 
         * @description
         * Set or reset scope variables.
         */
	    $scope.resetVariables = function () {
	        $scope.isUsOrCanada = false;
            $scope.regions = [
                { name: 'State/Province', abbr: '--' },
                { name: "Alabama", abbr: "AL" },
                { name: "Alaska", abbr: "AK" },
                { name: "Alberta", abbr: "AB" },
                { name: "Arizona", abbr: "AZ" },
                { name: "Arkansas", abbr: "AR" },
                { name: "American Samoa", abbr: "AS" },
                { name: "British Columbia", abbr: "BC" },
                { name: "California", abbr: "CA" },
                { name: "Colorado", abbr: "CO" },
                { name: "Connecticut", abbr: "CT" },
                { name: "Delaware", abbr: "DE" },
                { name: "District of Columbia", abbr: "DC" },
                { name: "Fed. States of Micronesia", abbr: "FM" },
                { name: "Florida", abbr: "FL" },
                { name: "Georgia", abbr: "GA" },
                { name: "Guam", abbr: "GU" },
                { name: "Hawaii", abbr: "HI" },
                { name: "Idaho", abbr: "ID" },
                { name: "Illinois", abbr: "IL" },
                { name: "Indiana", abbr: "IN" },
                { name: "Iowa", abbr: "IA" },
                { name: "Kansas", abbr: "KS" },
                { name: "Kentucky", abbr: "KY" },
                { name: "Louisiana", abbr: "LA" },
                { name: "Maine", abbr: "ME" },
                { name: "Manitoba", abbr: "MB" },
                { name: "Maryland", abbr: "MD" },
                { name: "Marshall Islands", abbr: "MH" },
                { name: "Massachusetts", abbr: "MA" },
                { name: "Michigan", abbr: "MI" },
                { name: "Minnesota", abbr: "MN" },
                { name: "Mississippi", abbr: "MS" },
                { name: "Missouri", abbr: "MO" },
                { name: "Montana", abbr: "MT" },
                { name: "Nebraska", abbr: "NE" },
                { name: "Nevada", abbr: "NV" },
                { name: "New Brunswick", abbr: "NB" },
                { name: "New Hampshire", abbr: "NH" },
                { name: "New Jersey", abbr: "NJ" },
                { name: "New Mexico", abbr: "NM" },
                { name: "New York", abbr: "NY" },
                { name: "Newfoundland and Labrador", abbr: "NL" },
                { name: "North Carolina", abbr: "NC" },
                { name: "North Dakota", abbr: "ND" },
                { name: "Northern Marianas", abbr: "MP" },
                { name: "Northwest Territories", abbr: "NT" },
                { name: "Nova Scotia", abbr: "NS" },
                { name: "Nunavut", abbr: "NU" },
                { name: "Ohio", abbr: "OH" },
                { name: "Oklahoma", abbr: "OK" },
                { name: "Ontario", abbr: "ON" },
                { name: "Oregon", abbr: "OR" },
                { name: "Pennsylvania", abbr: "PA" },
                { name: "Prince Edward Island", abbr: "PE" },
                { name: "Puerto Rico", abbr: "PR" },
                { name: "Québec", abbr: "QC" },
                { name: "Rhode Island", abbr: "RI" },
                { name: "Saskatchewan", abbr: "SK" },
                { name: "South Carolina", abbr: "SC" },
                { name: "South Dakota", abbr: "SD" },
                { name: "Tennessee", abbr: "TN" },
                { name: "Texas", abbr: "TX" },
                { name: "Utah", abbr: "UT" },
                { name: "Vermont", abbr: "VT" },
                { name: "Virginia", abbr: "VA" },
                { name: "(U.S.) Virgin Islands", abbr: "VI" },
                { name: "Washington", abbr: "WA" },
                { name: "West Virginia", abbr: "WV" },
                { name: "Wisconsin", abbr: "WI" },
                { name: "Wyoming", abbr: "WY" },
                { name: "Yukon Territory", abbr: "YT" },
                { name: "Armed Forces, the Americas", abbr: "AA" },
                { name: "Armed Forces, Europe", abbr: "AE" },
                { name: "Armed Forces, Pacific", abbr: "AP" }
            ];
            $scope.filters = {
                region: false
            };
            $scope.options = {
                regions: []
            };
	    }

	    /**
         * @ngdoc method
         * @name updateRegion
         * @function
         * 
         * @description
         * Update the region in the dialogData.warehouse object based on the currently selected item in the region dropdown.
         */
        $scope.updateRegion = function () {
            if ($scope.filters.region.id !== 0) {
                var abbr = $scope.regions[$scope.filters.region.id].abbr;
                $scope.dialogData.warehouse.region = abbr;
            }
        };

	    $scope.init();
	};

	angular.module("umbraco").controller("Merchello.Dashboards.Settings.Shipping.Dialogs.ShippingEditWarehouseController", ['$scope', merchello.Controllers.ShippingEditWarehouseController]);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
