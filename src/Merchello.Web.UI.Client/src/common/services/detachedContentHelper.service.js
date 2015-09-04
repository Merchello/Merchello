angular.module('merchello.services').factory('detachedContentHelper',
    ['$q', 'formHelper', 'notificationsService',
    function($q, formHelper, notificationsService) {

        return {

            detachedContentPerformSave: function(args) {

                if (!angular.isObject(args)) {
                    throw "args must be an object";
                }
                if (!args.scope) {
                    throw "args.scope is not defined";
                }
                if (!args.content) {
                    throw "args.content is not defined";
                }
                if (!args.statusMessage) {
                    throw "args.statusMessage is not defined";
                }
                if (!args.saveMethod) {
                    throw "args.saveMethod is not defined";
                }

                var deferred = $q.defer();
                if (args.scope.preValuesLoaded && formHelper.submitForm({ scope: args.scope, statusMessage: args.statusMessage })) {
                    args.scope.preValuesLoaded = false;

                    // save the current language only
                    angular.forEach(args.scope.contentTabs, function(ct) {
                        angular.forEach(ct.properties, function (p) {
                            if (typeof p.value !== "function") {
                                args.scope.detachedContent.detachedDataValues.setValue(p.alias, angular.toJson(p.value));
                            }
                        });
                    });

                    args.saveMethod(args.content).then(function(data) {
                        formHelper.resetForm({ scope: args.scope, notifications: data.notifications });
                        args.scope.preValuesLoaded = true;
                        deferred.resolve(data);
                    }, function (err) {

                        //show any notifications
                        if (angular.isArray(err.data.notifications)) {
                            for (var i = 0; i < err.data.notifications.length; i++) {
                                notificationsService.showNotification(err.data.notifications[i]);
                            }
                        }
                        args.scope.preValuesLoaded = true;
                        deferred.reject(err);
                    });
                } else {
                    deferred.reject();
                }

                return deferred.promise;
            }

        }

}]);
