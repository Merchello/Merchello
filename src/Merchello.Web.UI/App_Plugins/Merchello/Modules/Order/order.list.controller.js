(function (controllers, undefined) {
    
    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Order.ListController
     * @function
     * 
     * @description
     * The controller for the orders list page
     */
    controllers.OrderListController = function ($scope, assetsService, merchelloOrderService, merchelloInvoiceService) {

        $scope.orderIssues = [];
        $scope.invoices = [];
        $scope.sortProperty = "name";
        $scope.sortOrder = "asc";
        $scope.limitAmount = 10;
        $scope.currentPage = 0;

        assetsService.loadCss("/App_Plugins/Merchello/Common/Css/merchello.css");

        $scope.numberOfPages = function () {
            return Math.ceil($scope.invoices.length / $scope.limitAmount);
        };

        $scope.limitChanged = function (newVal) {
            $scope.limitAmount = newVal;
        };

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


        $scope.loaded = true;
        $scope.preValuesLoaded = true;

    };


    angular.module("umbraco").controller("Merchello.Dashboards.Order.ListController", merchello.Controllers.OrderListController);

}(window.merchello.Controllers = window.merchello.Controllers || {}));
