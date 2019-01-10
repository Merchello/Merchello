
angular.module('merchello').controller('Merchello.Backoffice.Reports.SalesSearchController',
    ['$http', '$scope', '$q', 'umbRequestHelper', '$log', '$filter', 'assetsService', 'dialogService', 'queryDisplayBuilder',
        'settingsResource', 'invoiceHelper', 'merchelloTabsFactory', 'salesOverTimeResource',
        function ($http, $scope, $q, umbRequestHelper, $log, $filter, assetsService, dialogService, queryDisplayBuilder,
            settingsResource, invoiceHelper, merchelloTabsFactory, salesOverTimeResource) {

            $scope.loaded = false;
            $scope.preValuesLoaded = false;
            $scope.reportData = [];
            $scope.startDate = '';
            $scope.endDate = '';
            $scope.settings = {};
            $scope.dateBtnText = '';
            $scope.baseUrl = '';
            $scope.salesSearchSnapshot = {};
            

            $scope.openDateRangeDialog = openDateRangeDialog;
            $scope.clearDates = clearDates;

            init();

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

            function loadDefaultData() {
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: $scope.baseUrl + 'GetInitialData',
                        method: "GET"
                    }), 'Failed to retreive default report data').then(function (data) {
                        $scope.salesSearchSnapshot = data;
                        $scope.loaded = true;
                        $scope.preValuesLoaded = true;
                    });
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


            function setDateButtonText() {
                $scope.dateBtnText = $scope.startDate + ' - ' + $scope.endDate;
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

        }]);
