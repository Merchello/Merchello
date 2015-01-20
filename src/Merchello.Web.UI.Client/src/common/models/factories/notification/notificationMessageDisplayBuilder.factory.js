    /**
     * @ngdoc service
     * @name notificationMessageDisplayBuilder
     *
     * @description
     * A utility service that builds NotificationMessageDisplay models
     */
    angular.module('merchello.models').factory('notificationMessageDisplayBuilder',
        ['genericModelBuilder', 'NotificationMessageDisplay',
            function(genericModelBuilder, NotificationMessageDisplay) {

                var Constructor = NotificationMessageDisplay;
                return {
                    createDefault: function() {
                        return new Constructor();
                    },
                    transform: function(jsonResult) {
                        return genericModelBuilder.transform(jsonResult, Constructor);
                    }
                };
    }]);

