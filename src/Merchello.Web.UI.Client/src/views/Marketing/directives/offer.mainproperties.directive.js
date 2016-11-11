/**
 * @ngdoc directive
 * @name offerMainProperties
 *
 * @description
 * Common form elements for Merchello's OfferSettings
 */
angular.module('merchello.directives').directive('offerMainProperties', function(dialogService, localizationService) {

    return {
        restrict: 'E',
        replace: true,
        scope: {
            offer: '=',
            context: '=',
            settings: '=',
            toggleOfferExpires: '&'
        },
        templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/Directives/offer.mainproperties.tpl.html',
        link: function (scope, elm, attr) {

            scope.dateBtnText = '';
            scope.ready = false;
            var allDates = '';

            scope.openDateRangeDialog = function() {
                var dialogData = {
                    startDate: scope.offer.offerStartsDate,
                    endDate: scope.offer.offerEndsDate
                };

                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/daterange.selection.html',
                    show: true,
                    callback: processDateRange,
                    dialogData: dialogData
                });
            }

            scope.clearDates = function() {
                scope.toggleOfferExpires();
            }

            function init() {
                scope.$watch('offer', function(nv, ov) {

                    if (nv) {
                        if (nv.key !== undefined) {
                            localizationService.localize('merchelloGeneral_allDates').then(function(value) {
                                allDates = value;
                                setDateBtnText();
                                scope.ready = true;
                            });
                        }
                    }

                });

            }

            function processDateRange(dialogData) {
                scope.offer.offerStartsDate = dialogData.startDate;
                scope.offer.offerEndsDate = dialogData.endDate;
                setDateBtnText();

            }

            function setDateBtnText() {
                if (scope.offer.offerExpires) {
                    scope.dateBtnText = scope.offer.offerStartsDate + ' - ' + scope.offer.offerEndsDate;
                } else {
                    scope.dateBtnText = allDates;
                }
            }


            init();
        }
    };
})
