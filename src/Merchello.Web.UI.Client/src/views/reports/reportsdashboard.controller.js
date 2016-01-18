angular.module('merchello').controller('Merchello.Backoffice.MerchelloReportsDashboardController',
    ['$scope', '$element', '$filter', 'assetsService', 'dialogService', 'settingsResource', 'merchelloTabsFactory',
        function($scope, $element, $filter, assetsService, dialogService, settingsResource, merchelloTabsFactory) {

            $scope.loaded = false;
            $scope.preValuesLoaded = false;
            $scope.tabs = [];
            $scope.settings = {};
            $scope.startDate = '';
            $scope.endDate = '';
            $scope.dateBtnText = '';
            $scope.openDateRangeDialog = openDateRangeDialog;
            $scope.clearDates = clearDates;

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
                    $scope.loaded = true;
                });
            };

            function setDefaultDates() {
                var date = new Date(), y = date.getFullYear(), m = date.getMonth();
                var firstOfMonth = new Date(y, m, 1);
                var endOfMonth = new Date(y, m + 1, 0);
                $scope.startDate = $filter('date')(firstOfMonth, $scope.settings.dateFormat);
                $scope.endDate = $filter('date')(endOfMonth, $scope.settings.dateFormat);
                setDateBtnText();

            }

            function setDateBtnText() {
                $scope.dateBtnText = $scope.startDate + ' - ' + $scope.endDate;
                $scope.preValuesLoaded = true;
            }

            function clearDates() {
                $scope.preValuesLoaded = false;
                setDefaultDates();
            }

            function openDateRangeDialog() {
                var dialogData = {
                    startDate: $scope.startDate,
                    endDate: $scope.endDate
                };

                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/daterange.selection.html',
                    show: true,
                    callback: processDateRange,
                    dialogData: dialogData
                });
            }

            function processDateRange(dialogData) {
                $scope.preValuesLoaded = false;
                $scope.startDate = dialogData.startDate;
                $scope.endDate = dialogData.endDate;
                setDateBtnText();
            }
        }]);
