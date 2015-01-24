
    angular.module('merchello.directives').directive('notificationMethods', function() {
        return {
            restrict: 'E',
            replace: true,
            templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/Directives/notificationmethods.tpl.html',
            controller: function($scope) {

                $scope.getMonitorName = function(key) {
                    var monitor = _.find($scope.notificationMonitors, function(monitor) {
                        return monitor.key === key;
                    });
                    if(monitor !== null || monitory !== undefined) {
                        return monitor.name;
                    } else {
                        return 'Not found';
                    }
                }

            }
        };
    });
