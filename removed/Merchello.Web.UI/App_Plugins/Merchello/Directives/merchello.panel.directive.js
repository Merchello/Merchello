(function (directives, undefined) {

    /**
     * @ngdoc directive
     * @name merchello-panel
     * @function
     * 
     * @description
     * Directive to wrap all Merchello Mark up and provide common classes.
     */
    directives.MerchelloPanelDirective = function () {
        return {
            restrict: 'E',
            replace: true,
            transclude: 'true',
            templateUrl: '/App_Plugins/Merchello/Directives/merchello-panel.html'
        };
    };

    angular.module("umbraco").directive('merchelloPanel', merchello.Directives.MerchelloPanelDirective);

}(window.merchello.Directives = window.merchello.Directives || {}));

