angular.module('merchello.directives').directive('merchelloDateRangeButton',
    function($filter, settingsResource, dialogService, merchDateHelper) {

        return {
            restrict: 'E',
            replace: true,
            scope: {
                reload: '&?',
                startDate: '=',
                endDate: '='
            },
            template: '<div class="btn-group pull-right" data-ng-show="loaded">' +
            '<a href="#" class="btn btn-small" data-ng-click="openDateRangeDialog()" prevent-default>{{ dateBtnText }}</a>' +
            '<a href="#" class="btn btn-small" prevent-default data-ng-click="clearDates()">X</a>' +
            '</div>',
            link: function(scope, elm, attr) {

                scope.loaded = false;
                scope.settings = {};

                scope.openDateRangeDialog = openDateRangeDialog;
                scope.clearDates = clearDates;

                function init() {
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
                        scope.settings = combined.settings;
                        setDefaultDates();
                        scope.loaded = true;
                    });
                };

                function setDefaultDates() {
                    var date = new Date(), y = date.getFullYear(), m = date.getMonth();
                    var firstOfMonth = new Date(date.setMonth(date.getMonth() - 1));
                    var endOfMonth = new Date();
                    scope.startDate = $filter('date')(firstOfMonth, scope.settings.dateFormat);
                    scope.endDate = $filter('date')(endOfMonth, scope.settings.dateFormat);
                    setDateBtnText();
                    reload();
                }

                function setDateBtnText() {
                    scope.dateBtnText = scope.startDate + ' - ' + scope.endDate;
                    scope.preValuesLoaded = true;
                }

                function clearDates() {
                    setDefaultDates();
                }

                function openDateRangeDialog() {
                    var dialogData = {
                        startDate: scope.startDate,
                        endDate: scope.endDate,
                        showPreDeterminedDates: true
                    };

                    dialogService.open({
                        template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/daterange.selection.html',
                        show: true,
                        callback: processDateRange,
                        dialogData: dialogData
                    });
                }

                function processDateRange(dialogData) {
                    scope.preValuesLoaded = false;

                    scope.startDate = dialogData.startDate;
                    scope.endDate =  dialogData.endDate;
                    //eventsService.emit(datesChangeEventName, { startDate : $scope.startDate, endDate : $scope.endDate });
                    setDateBtnText();
                    reload();
                }

                function reload() {

                    scope.reload()(
                        merchDateHelper.convertToJsDate(scope.startDate, scope.settings.dateFormat),
                        merchDateHelper.convertToJsDate(scope.endDate, scope.settings.dateFormat));
                }

                init();
            }
        }

});
