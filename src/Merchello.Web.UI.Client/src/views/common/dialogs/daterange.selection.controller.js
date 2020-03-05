angular.module('merchello').controller('Merchello.Common.Dialogs.DateRangeSelectionController',
    ['$scope', '$q', '$log', '$filter', '$element', 'assetsService', 'angularHelper', 'notificationsService', 'settingsResource', 'settingDisplayBuilder',
    function($scope, $q, $log, $filter, $element,  assetsService, angularHelper, notificationsService, settingsResource, settingDisplayBuilder) {

        $scope.loaded = false;
        $scope.preValuesLoaded = false;

        $scope.changeDateFilters = changeDateFilters;
        $scope.preSelectDate = preSelectDate;
        $scope.preSelectDays = preSelectDays;
        $scope.preSelectMonths = preSelectMonths;

        $scope.dateFormat = 'YYYY-MM-DD';
        $scope.rangeStart = '';
        $scope.rangeEnd = '';
        $scope.save = save;


        function init() {
            var promises = loadAssets();
            promises.push(loadSettings());

            $q.all(promises).then(function() {
                // arg!!! js
                $scope.dateFormat = $scope.settings.dateFormat.toUpperCase();

                var start, end;
                if ($scope.dialogData.startDate !== '' && $scope.dialogData.endDate !== '') {
                    start = $scope.dialogData.startDate;
                    end = $scope.dialogData.endDate;
                } else {
                    end = new Date();
                    start = new Date();
                    start = start.setMonth(start.getMonth() - 1);
                }

                // initial settings use standard
                $scope.rangeStart = $filter('date')(start, $scope.settings.dateFormat);
                $scope.rangeEnd = $filter('date')(end, $scope.settings.dateFormat);
                if ($scope.dialogData.showPreDeterminedDates) {
                    $scope.showPreDeterminedDates = $scope.dialogData.showPreDeterminedDates;
                }


                setupDatePicker("#filterStartDate", $scope.rangeStart);
                $element.find("#filterStartDate").datetimepicker().on("changeDate", applyDateStart);

                setupDatePicker("#filterEndDate", $scope.rangeEnd);
                $element.find("#filterEndDate").datetimepicker().on("changeDate", applyDateEnd);

                $scope.preValuesLoaded = true;
            });
        }
        /**
         * @ngdoc method
         * @name loadAssets
         * @function
         *
         * @description - Loads needed and js stylesheets for the view.
         */
        function loadAssets() {
            var promises = [];
            var cssPromise = assetsService.loadCss('lib/datetimepicker/bootstrap-datetimepicker.min.css');
            var jsPromise = assetsService.load(['lib/moment/moment-with-locales.js', 'lib/datetimepicker/bootstrap-datetimepicker.js']);

            promises.push(cssPromise);
            promises.push(jsPromise);

            return promises;
        }

        function loadSettings() {
            var promise = settingsResource.getAllSettings();
            return promise.then(function(allSettings) {
                $scope.settings = settingDisplayBuilder.transform(allSettings);
            }, function(reason) {
                notificationsService.error('Failed to load settings', reason.message);
            });
        }

        /**
         * @ngdoc method
         * @name setupDatePicker
         * @function
         *
         * @description
         * Sets up the datepickers
         */
        function setupDatePicker(pickerId, defaultDate, isStart) {

            // Open the datepicker and add a changeDate eventlistener
            $element.find(pickerId).datetimepicker({
                defaultDate: defaultDate,
                format: $scope.dateFormat
            });

            //Ensure to remove the event handler when this instance is destroyted
            $scope.$on('$destroy', function () {
                $element.find(pickerId).datetimepicker("destroy");
            });
        }

        /**
         * @ngdoc method
         * @name changeDateFilters
         * @function
         *
         * @param {string} start - String representation of start date.
         * @param {string} end - String representation of end date.
         * @description - Change the date filters, then triggera new API call to load the reports.
         */
        function changeDateFilters(start, end) {
            $scope.rangeStart = start;
            $scope.rangeEnd = end;
        }

        function preSelectDays(days) {
            var end = new Date();
            var start = new Date().setDate(new Date().getDate() - days);
            preSelectDate(moment(start).format($scope.dateFormat), moment(end).format($scope.dateFormat));
        }

        function preSelectMonths(months) {
            var end = new Date();
            var start = new Date().setMonth(new Date().getMonth() - months);
            preSelectDate(moment(start).format($scope.dateFormat), moment(end).format($scope.dateFormat));
        }

        /**
         * @ngdoc method
         * @name preSelectDate
         * @function
         *
         * @param {string} start - String representation of start date.
         * @param {string} end - String representation of end date.
         * @description - Change the date filters, then triggera new API call to load the reports.
         */
        function preSelectDate(start, end) {
            changeDateFilters(start, end);
            save();
        }

        /*-------------------------------------------------------------------
         * Helper Methods
         * ------------------------------------------------------------------*/

        //handles the date changing via the api
        function applyDateStart(e) {
            angularHelper.safeApply($scope, function () {
                // when a date is changed, update the model
                if (e.localDate) {
                    $scope.rangeStart = moment(e.localDate).format($scope.dateFormat);
                }
            });
        }

        //handles the date changing via the api
        function applyDateEnd(e) {
            angularHelper.safeApply($scope, function () {
                // when a date is changed, update the model
                if (e.localDate) {
                    $scope.rangeEnd = moment(e.localDate).format($scope.dateFormat);
                }
            });
        }

        function save() {
            $scope.dialogData.startDate = $scope.rangeStart;
            $scope.dialogData.endDate = $scope.rangeEnd;
            $scope.submit($scope.dialogData);
        }

        // Initialize the controller
        init();

}]);
