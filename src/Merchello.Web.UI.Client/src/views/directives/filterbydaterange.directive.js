    /**
     * @ngdoc directive
     * @name filter-by-date-range
     * @function
     *
     * @description
     * Directive to wrap all Merchello Mark up.
     */
    angular.module('merchello.directives').directive('filterByDateRange', function() {
        return {
            restrict: 'E',
            replace: true,
            scope: {
                filterStartDate: '=',
                filterEndDate: '=',
                filterWithDates: '&'
            },
            templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/directives/filterbydaterange.tpl.html',
            controller: function($scope, $element, assetsService, angularHelper, notificationsService, settingsResource, settingDisplayBuilder) {

                $scope.settings = {};

                // exposed methods
                $scope.changeDateFilters = changeDateFilters;

                function init() {
                    loadCssAssets();
                    loadSettings();
                }

                /**
                 * @ngdoc method
                 * @name loadCssAssets
                 * @function
                 *
                 * @description - Loads needed stylesheets for the view.
                 */
                function loadCssAssets() {

                    assetsService.loadCss('lib/datetimepicker/bootstrap-datetimepicker.min.css').then(function () {
                        var filesToLoad = ["lib/datetimepicker/bootstrap-datetimepicker.min.js"];
                        assetsService.load(filesToLoad).then(
                            function () {
                                //The Datepicker js and css files are available and all components are ready to use.

                                setupDatePicker("#filterStartDate");
                                $element.find("#filterStartDate").datetimepicker().on("changeDate", applyDateStart);

                                setupDatePicker("#filterEndDate");
                                $element.find("#filterEndDate").datetimepicker().on("changeDate", applyDateEnd);
                            });
                    });
                }

                function loadSettings() {
                    var promise = settingsResource.getAllSettings();
                    promise.then(function(allSettings) {
                        console.info(allSettings);
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
                function setupDatePicker(pickerId) {

                    // Open the datepicker and add a changeDate eventlistener
                    $element.find(pickerId).datetimepicker();

                    //Ensure to remove the event handler when this instance is destroyted
                    $scope.$on('$destroy', function () {
                        $element.find(pickerId).datetimepicker("destroy");
                    });
                }

                /*-------------------------------------------------------------------
                 * Event Handler Methods
                 *-------------------------------------------------------------------*/

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
                    $scope.filterStartDate = start;
                    $scope.filterEndDate = end;
                    $scope.currentPage = 0;
                    $scope.filterWithDates();
                }

                /*-------------------------------------------------------------------
                 * Helper Methods
                 * ------------------------------------------------------------------*/

                //handles the date changing via the api
                function applyDateStart(e) {
                    angularHelper.safeApply($scope, function () {
                        // when a date is changed, update the model
                        if (e.localDate) {
                            $scope.filterStartDate = e.localDate.toIsoDateString();
                        }
                    });
                }

                //handles the date changing via the api
                function applyDateEnd(e) {
                    angularHelper.safeApply($scope, function () {
                        // when a date is changed, update the model
                        if (e.localDate) {
                            $scope.filterEndDate = e.localDate.toIsoDateString();
                        }
                    });
                }

                // Initialize the controller
                init();
            }
        };
    });
