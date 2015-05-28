angular.module('merchello.directives').directive('offerMainProperties', function() {

    return {
        restrict: 'E',
        replace: true,
        scope: {
            offer: '=',
            context: '=',
            settings: '='
        },
        templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/Directives/offer.mainproperties.tpl.html',
        controller: function() {

        }
    }
})
