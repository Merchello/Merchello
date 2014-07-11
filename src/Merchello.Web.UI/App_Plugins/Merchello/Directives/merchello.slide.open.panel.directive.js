(function (directives, undefined) {

    /**
     * @ngdoc directive
     * @name merchello-slide-open-panel
     * @function
     * 
     * @description
     * Directive to allow a section of content to slide open/closed based on a boolean value
     */
    directives.MerchelloSlideOpenPanelDirective = function () {
        return {
            restrict: 'E',
            replace: true,
            transclude: 'true',
            scope: {
                isOpen: '=',
                classes: '='
            },
            templateUrl: '/App_Plugins/Merchello/Directives/merchello-slide-open-panel.html',
            link: function ($scope, $element, attrs) {
                
                if ($scope.classes == undefined) {
                    $scope.classes = 'control-group umb-control-group';
                }
            }

        };
    };

    angular.module("umbraco").directive('merchelloSlideOpenPanel', merchello.Directives.MerchelloSlideOpenPanelDirective);

}(window.merchello.Directives = window.merchello.Directives || {}));

