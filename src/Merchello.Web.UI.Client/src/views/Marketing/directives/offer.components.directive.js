/**
 * @ngdoc directive
 * @name offerComponents
 *
 * @description
 * Common form elements for Merchello's OfferComponents
 */
angular.module('merchello.directives').directive('offerComponents', function() {

    return {
        restrict: 'E',
        replace: true,
        scope: {
            offerSettings: '=',
            settings: '=',
            componentType: '@'
        },
        templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/Directives/offer.components.tpl.html',
        controller:  'Merchello.Directives.OfferComponentsDirectiveController'
    }
})

