/**
 * @ngdoc directive
 * @name offerMainProperties
 *
 * @description
 * Common form elements for Merchello's OfferSettings
 */
angular.module('merchello.directives').directive('offerMainProperties', function() {

    return {
        restrict: 'E',
        replace: true,
        scope: {
            offer: '=',
            context: '=',
            settings: '=',
            toggleOfferExpires: '&'
        },
        templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/Directives/offer.mainproperties.tpl.html'
    };
})
