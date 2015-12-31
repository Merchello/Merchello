
angular.module('merchello').controller('Merchello.Backoffice.Reports.SalesOverTimeController',
    ['$scope', '$q', '$log', 'settingsResource', 'invoiceHelper', 'merchelloTabsFactory', 'salesOverTimeResource',
        function($scope, $q, $log, settingsResource, invoiceHelper, merchelloTabsFactory, salesOverTimeResource) {

            $scope.loaded = false;
            $scope.preValuesLoaded = false;
            $scope.tabs = [];

            function init() {
                $scope.tabs = merchelloTabsFactory.createReportsTabs();
                $scope.tabs.addTab("salesOverTime", "merchelloTree_salesOverTime", '#/merchello/merchello/salesOverTime/manage');
                $scope.tabs.setActive("salesOverTime");

                loadDefaultData();
            }

            function loadDefaultData() {

                salesOverTimeResource.getDefaultReportData().then(function(result) {
                    console.info(result);
                    $scope.preValuesLoaded = true;
                    $scope.loaded = true;

                });

            }

            init();
        }]);
