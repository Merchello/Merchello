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
        ['$scope', '$routeParams', 'settingsResource',
         function($scope, $routeParams, settingsResource) {

             $scope.loaded = false;

             $scope.reportPath = '';

             // Property to control the report to show
             var reportParam  = $routeParams.id;

             settingsResource.getReportBackofficeTrees().then(function(trees) {

                 if(trees.length > 0) {
                     var tree = _.find(trees, function (t) {
                         if (t.routeId == reportParam) {
                             return t;
                         }
                     });

                     if (tree !== undefined) {
                        $scope.reportPath = tree.routePath;
                     }
                 }

                 if($scope.routePath === '') {
                     var re = /(\\)/g;
                     var subst = '/';

                     var result = reportParam.replace(re, subst);

                     //$scope.reportPath = "/App_Plugins/Merchello.ExportOrders|ExportOrders.html";
                     $scope.reportPath = "/App_Plugins/" + result + ".html";
                 }

                 $scope.loaded = true;
             });



    }]);
