angular.module('merchello').controller('Merchello.Backoffice.MerchelloReportsDashboardController',
    ['$scope', '$element', '$filter', 'assetsService', 'settingsResource', 'merchelloTabsFactory',
        function($scope, $element, $filter, assetsService, settingsResource, merchelloTabsFactory) {

            $scope.loaded = false;
            $scope.preValuesLoaded = false;
            $scope.tabs = [];
            $scope.settings = {};
            $scope.startDate = '';
            $scope.endDate = '';

            assetsService.loadCss('/App_Plugins/Merchello/lib/charts/angular-chart.min.css').then(function() {
                init();
            });


            function init() {
                $scope.tabs = merchelloTabsFactory.createReportsTabs();
                $scope.tabs.setActive("reportsdashboard");
                loadSettings();
            }


            /**
             * @ngdoc method
             * @name loadSettings
             * @function
             *
             * @description - Load the Merchello settings.
             */
            function loadSettings() {
                settingsResource.getAllCombined().then(function(combined) {
                    $scope.settings = combined.settings;
                    setDefaultDates();
                });
            };

            function setDefaultDates() {
                var date = new Date(), y = date.getFullYear(), m = date.getMonth();
                var firstOfMonth = new Date(y, m, 1);
                var endOfMonth = new Date(y, m + 1, 0);
                $scope.startDate = $filter('date')(firstOfMonth, $scope.settings.dateFormat);
                $scope.endDate = $filter('date')(endOfMonth, $scope.settings.dateFormat);
                $scope.loaded = true;
                $scope.preValuesLoaded = true;
            }


        }]);
