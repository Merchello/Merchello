    /**
     * @ngdoc service
     * @name notificationMethodDisplayBuilder
     *
     * @description
     * A utility service that builds NotificationMethodDisplay models
     */
    angular.module('merchello.models').factory('notificationMethodDisplayBuilder',
    ['genericModelBuilder', 'notificationMessageDisplayBuilder', 'NotificationMethodDisplay',
        function(genericModelBuilder, notificationMessageDisplayBuilder, NotificationMethodDisplay) {

            var Constructor = NotificationMethodDisplay;

            return {
                createDefault: function() {
                    return new Constructor();
                },
                transform: function(jsonResult) {
                    var methods = [];
                    if(angular.isArray(jsonResult)) {
                        for(var i = 0; i < jsonResult.length; i++) {
                            var method = genericModelBuilder.transform(jsonResult[ i ], Constructor);
                            method.notificationMessages = notificationMessageDisplayBuilder.transform(jsonResult[ i ].notificationMessages);
                            methods.push(method);
                        }
                    } else {
                        methods = genericModelBuilder.transform(jsonResult, Constructor);
                        methods.notificationMessages = notificationMessageDisplayBuilder.transform(jsonResult.notificationMessages);
                    }
                    return methods;
                }
            };
    }]);
