
angular.module('merchello').controller('Merchello.Backoffice.Reports.SalesSearchController',
    ['$http', '$scope', '$q', 'umbRequestHelper', '$log', '$filter', 'assetsService', 'dialogService', 'queryDisplayBuilder',
        'settingsResource', 'invoiceHelper', 'merchelloTabsFactory', 'salesOverTimeResource',
        function ($http, $scope, $q, umbRequestHelper, $log, $filter, assetsService, dialogService, queryDisplayBuilder,
            settingsResource, invoiceHelper, merchelloTabsFactory, salesOverTimeResource) {

            $scope.loaded = false;
            $scope.preValuesLoaded = false;
            $scope.settings = {};
            $scope.dateBtnText = '';
            $scope.baseUrl = '';
            $scope.salesSearchSnapshot = {};
            $scope.toggle = [];
            $scope.originalStartDate = '';
            $scope.originalEndDate = '';

            // Load
            init();

            // Scope Methods
            $scope.toggleFilter = toggleFilter;
            $scope.openDateRangeDialog = openDateRangeDialog;
            $scope.updateData = updateData;
            $scope.reload = reload;

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
                    }), 'Failed to retreive default report data')
                    .then(function (data) {
                        $scope.salesSearchSnapshot = data;
                        $scope.loaded = true;
                        $scope.preValuesLoaded = true;
                        $scope.originalStartDate = $scope.salesSearchSnapshot.startDate;
                        $scope.originalEndDate = $scope.salesSearchSnapshot.endDate;
                        setDateButtonText();
                    });
            }

            function updateData() {

                // Clear the products first
                // As don't want to post a ton of data we don't need to
                $scope.salesSearchSnapshot.products = [];

                return umbRequestHelper.resourcePromise(
                    $http.post($scope.baseUrl + 'UpdateData', $scope.salesSearchSnapshot), 'Failed to update the report')
                    .then(function (data) {
                        $scope.salesSearchSnapshot = data;
                    });
            }

            function reload(startDate, endDate) {
                if ($scope.preValuesLoaded) {
                    $scope.salesSearchSnapshot.startDate = startDate;
                    $scope.salesSearchSnapshot.endDate = endDate;
                    setDateButtonText();
                    updateData();
                }
            }

            function toggleFilter(inx) {
                $scope.toggle[inx] = !$scope.toggle[inx];
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
                // This never gets hit for som
            }
        }]);
