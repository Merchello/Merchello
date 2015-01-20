    /**
     * @ngdoc service
     * @name notificationMonitorDisplayBuilder
     *
     * @description
     * A utility service that builds NotificationMonitorDisplayDisplay models
     */
    angular.module('merchello.models').factory('notificationMonitorDisplayBuilder',
        ['genericModelBuilder', 'NotificationMonitorDisplay',
        function(genericModelBuilder, NotificationMonitorDisplay) {
            var Constructor = NotificationMonitorDisplay;
            return {
                createDefault: function() {
                    return new Constructor();
                },
                transform: function(jsonResult) {
                    return genericModelBuilder.transform(jsonResult, Constructor);
                }
            };
        }]);
