    /**
     * @ngdoc controller
     * @name Merchello.Backoffice.ReportsViewReportController
     * @function
     *
     * @description
     * The controller for the ViewReport page
     *
     * This is a bootstrapper to allow reports that are plugins to be loaded using the merchello application route.
     */
    angular.module('merchello').controller('Merchello.Backoffice.ReportsViewReportController',
        ['$scope', '$routeParams',
         function($scope, $routeParams) {

             $scope.loaded = true;
             $scope.preValuesLoaded = true;

             // Property to control the report to show
             $scope.reportParam = $routeParams.id;

             var re = /(\\)/g;
             var subst = '/';

             var result = $scope.reportParam.replace(re, subst);

             //$scope.reportPath = "/App_Plugins/Merchello.ExportOrders|ExportOrders.html";
             $scope.reportPath = "/App_Plugins/" + result + ".html";

    }]);
