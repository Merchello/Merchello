/**
 * @ngdoc directive
 * @name offerMainProperties
 *
 * @description
 * Common form elements for Merchello's OfferSettings
 */
angular.module('merchello.directives').directive('offerMainProperties', function(dialogService, localizationService, eventsService) {

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
            var eventOfferExpiresOpen = 'merchello.offercouponexpires.open';

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

                eventsService.on(eventOfferExpiresOpen, scope.openDateRangeDialog);

                scope.$watch('offer', function(nv, ov) {

                    if (nv) {
                        if (nv.key !== undefined) {
                            localizationService.localize('merchelloGeneral_allDates').then(function(value) {
                                allDates = value;
                                scope.ready = true;
                            });
                        }
                    }

                });

            }

            function processDateRange(dialogData) {
                scope.offer.offerStartsDate = dialogData.startDate;
                scope.offer.offerEndsDate = dialogData.endDate;
            }

            init();
        }
    };
})
