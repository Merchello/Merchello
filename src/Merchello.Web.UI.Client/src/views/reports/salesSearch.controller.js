
angular.module('merchello').controller('Merchello.Backoffice.Reports.SalesSearchController',
    ['$http', '$scope', '$q', 'umbRequestHelper', '$log', '$filter', 'assetsService', 'dialogService', 'queryDisplayBuilder',
        'settingsResource', 'invoiceHelper', 'merchelloTabsFactory', 'salesOverTimeResource',
        function ($http, $scope, $q, umbRequestHelper, $log, $filter, assetsService, dialogService, queryDisplayBuilder,
            settingsResource, invoiceHelper, merchelloTabsFactory, salesOverTimeResource) {

            var datesChangeEventName = 'merchello.reportsdashboard.datechange';

            $scope.loaded = false;
            $scope.preValuesLoaded = false;
            $scope.reportData = [];
            $scope.settings = {};
            $scope.dateBtnText = '';
            $scope.baseUrl = '';
            $scope.salesSearchSnapshot = {};
            $scope.selectedStatuses = [];
            $scope.toggle = [];

            // Load
            init();

            // Scope Methods
            $scope.reload = reload;
            $scope.toggleFilter = toggleFilter;          
            $scope.openDateRangeDialog = openDateRangeDialog;
            $scope.clearDates = clearDates;

            function init() {
                $scope.baseUrl = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloSalesSearchBaseUrl'];
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
                settingsResource.getAllCombined().then(function (combined) {
                    $scope.settings = combined.settings;
                    loadDefaultData();
                });
            }

            function toggleFilter(inx) {
                $scope.toggle[inx] = !$scope.toggle[inx];
            }

            function loadDefaultData() {
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: $scope.baseUrl + 'GetInitialData',
                        method: "GET"
                    }), 'Failed to retreive default report data').then(function (data) {
                        $scope.salesSearchSnapshot = data;
                        $scope.loaded = true;
                        $scope.preValuesLoaded = true;
                        setDateButtonText();
                        $scope.selectedStatuses = angular.copy($scope.salesSearchSnapshot.invoiceStatuses);
                    });
            }

            function openDateRangeDialog() {
                var dialogData = {
                    startDate: $scope.salesSearchSnapshot.startDate,
                    endDate: $scope.salesSearchSnapshot.endDate
                };

                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/daterange.selection.html',
                    show: true,
                    callback: processDateRange,
                    dialogData: dialogData
                });
            }


            function setDateButtonText() {
                $scope.dateBtnText = $scope.salesSearchSnapshot.startDate + ' - ' + $scope.salesSearchSnapshot.endDate;
            }

            function processDateRange(dialogData) {
                $scope.startDate = dialogData.startDate;
                $scope.endDate = dialogData.endDate;
                //loadCustomData(); TODO - Call method
            }

            function clearDates() {
                $scope.loaded = false;
                $scope.preValuesLoaded = false;
                loadDefaultData();
            }

            function reload(startDate, endDate) {
                $scope.salesSearchSnapshot.startDate = startDate;
                $scope.salesSearchSnapshot.endDate = endDate;                
            }

        }]);
