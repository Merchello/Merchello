(function (directives, undefined) {

    /**
     * @ngdoc directive
     * @name NotificationMethodsDirective
     * @function
     * 
     * @description
     * directive to display notifications methods, message and to provide a parent for the directives and flyouts
     */
    directives.NotificationMethodsDirective = function () {
        return {
            restrict: 'E',
            replace: true,
            templateUrl: '/App_Plugins/Merchello/Modules/Settings/Notifications/Directives/notification-methods.html'
        };
    };

    angular.module("umbraco").directive('notificationMethods', merchello.Directives.NotificationMethodsDirective);


}(window.merchello.Directives = window.merchello.Directives || {}));

