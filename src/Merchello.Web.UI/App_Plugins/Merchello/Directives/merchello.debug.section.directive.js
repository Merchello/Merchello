(function (directives, undefined) {

    /**
     * @ngdoc directive
     * @name merchello-debug-section
     * @function
     * 
     * @description
     * Directive to show a debug section with a button to show the debug information in an Umbraco dialog
     */
    directives.MerchelloDebugSectionDirective = function (dialogService) {
        return {
            restrict: 'E',
            replace: true,
            templateUrl: '/App_Plugins/Merchello/Directives/merchello-debug-section.html',
            link: function ($scope, $element, attrs) {
                /**
                 * @ngdoc method
                 * @name showDebugInfo
                 * @function
                 * 
                 * @description
                 * Shows a dialog with debugging info
                 */
                $scope.showDebugInfo = function () {

                    var dialogData = _.pick($scope, attrs.propsToDebug.split(","));

                    dialogService.open({
                        template: '/App_Plugins/Merchello/Common/Js/Dialogs/debug.dialog.html',
                        show: true,
                        callback: function () { },
                        dialogData: dialogData
                    });
                };

                $scope.debugOn = Umbraco.Sys.ServerVariables.isDebuggingEnabled;
            }
        };
    };

    angular.module("umbraco").directive('merchelloDebugSection', ["dialogService", merchello.Directives.MerchelloDebugSectionDirective]);

}(window.merchello.Directives = window.merchello.Directives || {}));

