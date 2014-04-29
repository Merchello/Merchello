(function (controllers, undefined) {
    
    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Product.ListController
     * @function
     * 
     * @description
     * The controller for the product editor
     */
	controllers.ProductListController = function ($scope, $routeParams, $location, assetsService, notificationsService, angularHelper, serverValidationManager, merchelloProductService, merchelloSettingsService) {

        $scope.filtertext = "";
        $scope.products = [];
        $scope.filteredproducts = [];
        $scope.watchCount = 0;
        $scope.sortProperty = "name";
        $scope.sortOrder = "asc";
        $scope.limitAmount = 10;
        $scope.currentPage = 0;

        assetsService.loadCss("/App_Plugins/Merchello/Common/Css/merchello.css");

        //--------------------------------------------------------------------------------------
        // Initialization methods
        //--------------------------------------------------------------------------------------

        /**
         * @ngdoc method
         * @name loadProducts
         * @function
         * 
         * @description
         * Load the products from the product service, then wrap the results
         * in Merchello models and add to the scope via the products collection.
         */
        $scope.loadProducts = function () {

            var promise = merchelloProductService.getAllProducts();

            promise.then(function(products) {

                $scope.products = _.map(products, function(productFromServer) {
                    return new merchello.Models.Product(productFromServer, true);
                });

                $scope.loaded = true;
                $scope.preValuesLoaded = true;

            }, function(reason) {

                notificationsService.success("Products Load Failed:", reason.message);

            });

        };

		$scope.loadSettings = function () {


			var currencySymbolPromise = merchelloSettingsService.getCurrencySymbol();
			currencySymbolPromise.then(function(currencySymbol) {
				$scope.currencySymbol = currencySymbol;

			}, function (reason) {
				alert('Failed: ' + reason.message);
			});
		};

        /**
         * @ngdoc method
         * @name init
         * @function
         * 
         * @description
         * Method called on intial page load.  Loads in data from server and sets up scope.
         */
        $scope.init = function () {

        	$scope.loadProducts();
        	$scope.loadSettings();

        };

        $scope.init();



        //--------------------------------------------------------------------------------------
        // Events methods
        //--------------------------------------------------------------------------------------

        /**
         * @ngdoc method
         * @name limitChanged
         * @function
         * 
         * @description
         * Helper function to set the amount of items to show per page for the paging filters and calculations
         */
        $scope.limitChanged = function (newVal) {
            $scope.limitAmount = newVal;
        };

        /**
         * @ngdoc method
         * @name changeSortOrder
         * @function
         * 
         * @description
         * Helper function to set the current sort on the table and switch the 
         * direction if the property is already the current sort column.
         */
        $scope.changeSortOrder = function (propertyToSort) {

            if ($scope.sortProperty == propertyToSort) {
                if ($scope.sortOrder == "asc") {
                    $scope.sortProperty = "-" + propertyToSort;
                    $scope.sortOrder = "desc";
                } else {
                    $scope.sortProperty = propertyToSort;
                    $scope.sortOrder = "asc";
                }
            } else {
                $scope.sortProperty = propertyToSort;
                $scope.sortOrder = "asc";
            }

        };

        /**
         * @ngdoc method
         * @name getFilteredProducts
         * @function
         * 
         * @description
         * Calls the product service to search for products via a string search 
         * param.  This searches the Examine index in the core.
         */
        $scope.getFilteredProducts = function (filter) {
            notificationsService.info("Filtering...", "");

            if (merchello.Helpers.Strings.isNullOrEmpty(filter)) {
                $scope.loadProducts();
                notificationsService.success("Filtered Products Loaded", "");
            } else {
                var promise = merchelloProductService.filterProducts(filter);

                promise.then(function(products) {

                    $scope.products = _.map(products, function(productFromServer) {
                        return new merchello.Models.Product(productFromServer, true);
                    });

                    notificationsService.success("Filtered Products Loaded", "");

                }, function(reason) {

                    notificationsService.success("Filtered Products Load Failed:", reason.message);

                });
            }
        };


        //--------------------------------------------------------------------------------------
        // Calculations
        //--------------------------------------------------------------------------------------

        /**
         * @ngdoc method
         * @name numberOfPages
         * @function
         * 
         * @description
         * Helper function to get the amount of items to show per page for the paging
         */
        $scope.numberOfPages = function () {
            return Math.ceil($scope.products.length / $scope.limitAmount);
        };


    };

	angular.module("umbraco").controller("Merchello.Dashboards.Product.ListController", ['$scope', '$routeParams', '$location', 'assetsService', 'notificationsService', 'angularHelper', 'serverValidationManager', 'merchelloProductService', 'merchelloSettingsService', merchello.Controllers.ProductListController]);

}(window.merchello.Controllers = window.merchello.Controllers || {}));

