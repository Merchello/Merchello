
    angular.module('merchello.directives').directive('notificationMethods', function() {
        return {
            restrict: 'E',
            replace: true,
            templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/Directives/notificationmethods.tpl.html',
            controller: function($scope) {

                // Exposed monitors
                $scope.getMonitorName = getMonitorName;

                function getMonitorName(key) {
                    var monitor = _.find($scope.notificationMonitors, function(monitor) {
                        return monitor.monitorKey === key;
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
