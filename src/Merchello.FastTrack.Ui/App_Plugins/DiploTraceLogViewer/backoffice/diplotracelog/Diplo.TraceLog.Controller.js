'use strict';
app.requires.push('smart-table');

angular.module("umbraco").controller("DiploTraceLogEditController",
    function ($scope, $http, $routeParams, $route, $filter, $q, $templateCache, $timeout, $window, dialogService, notificationsService, navigationService, eventsService, stConfig, diploTraceLogResources) {

        var timer;
        var lastModified = 0;
        var persistKey = "diploTraceLogPersist";
        var pollingOnText = "Polling ";
        var pollingOffText = "Polling Off";
        var pollingIndicatorChar = "▪";

        $scope.isLoading = true;
        $scope.isValid = false;
        $scope.pageSize = {};
        $scope.persist = localStorage.getItem(persistKey) === "true" || false;

        $scope.polling = {
            enabled: false,
            interval: 5,
            buttonText: pollingOffText,
            indicator: ""
        };

        $scope.id = $routeParams.id;

        $scope.levelOptions = [
            { label: "Info", value: "INFO" },
            { label: "Warn", value: "WARN" },
            { label: "Error", value: "ERROR" },
            { label: "Debug", value: "DEBUG" },
        ];

        $scope.pollOptions = [
            { label: "Disabled", value: 0 },
            { label: "1 Second", value: 1 },
            { label: "5 Seconds", value: 5 },
            { label: "10 Seconds", value: 10 },
            { label: "60 Seconds", value: 60 },
        ];

        $scope.itemsPerPage = [20, 50, 100, 200, 500, 1000];

        $scope.isCurrentLog = $routeParams.id.endsWith('.txt');

        // Sync tree
        var routePath = ($routeParams.id === "Date" || $routeParams.id === "Filename" || $scope.isCurrentLog) ? [$routeParams.id] : ["Date", $routeParams.id];

        navigationService.syncTree({
            tree: $routeParams.tree,
            path: routePath,
            forceReload: false
        });

        // Gets the log data and populates the row collection
        var getLogData = function () {

            if ($routeParams.id == "Date" || $routeParams.id == "Filename") {
                getLogFiles();
                return;
            };

            $scope.isValid = true;

            diploTraceLogResources.getLogDataResponse($routeParams.id).then(function (data) {
                $scope.rowCollection = data.LogDataItems;
                lastModified = data.LastModifiedTicks;
                $scope.logFileDate = data.Date;
                $scope.isLoading = false;
            });
        }

        // gets the log file list
        var getLogFiles = function () {
            diploTraceLogResources.getLogFiles().then(function (data) {
                $scope.filesCollection = data;
                $scope.isLoading = false;
            });
        }

        // Open detail modal
        $scope.openDetail = function (logItem, data) {

            var dialog = dialogService.open({
                template: '/App_Plugins/DiploTraceLogViewer/backoffice/diplotracelog/detail.html',
                dialogData: { logItem: logItem, items: data }, show: true, width: 800
            });
        }

        // Used for continous polling
        var tick = function () {

            $scope.polling.indicator += pollingIndicatorChar;

            if ($scope.polling.indicator.length > 9) {
                $scope.polling.indicator = "";
            }

            if ($scope.polling.interval > 0) {

                diploTraceLogResources.getLastModifiedTime($routeParams.id).then(function (data) {
                    var modified = parseInt(data);

                    if (modified > lastModified) {
                        lastModified = modified;
                        notificationsService.success("Log Updated", "New entries where added to the log file.");
                        getLogData();
                    }

                    timer = $timeout(tick, parseInt($scope.polling.interval) * 1000);
                });
            }
        }

        // Calls the get log data service
        getLogData();

        // Cancels the timer when polling is enabled
        var cancelTimer = function () {
            if (timer) {
                $timeout.cancel(timer);
                timer = null;
                notificationsService.warning("Polling Stopped", "Log file polling has been stopped.");
            }
        }

        // Starts the timer when polling is enabled based on polling interval
        var startTimer = function () {

            if ($scope.isCurrentLog) {
                cancelTimer();
                $scope.polling.enabled = true;

                timer = $timeout(tick, parseInt($scope.polling.interval) * 1000);
                console.log("Polling started...");
                $scope.polling.indicator = pollingIndicatorChar;
            }
        }

        // Checks whether polling is enabled and starts or stops the timer
        $scope.checkTimer = function () {

            if ($scope.polling.enabled) {
                notificationsService.info("Polling Enabled", "Your log file will now be checked every " + $scope.polling.interval + " seconds for changes.");
                startTimer();
            }
            else {
                cancelTimer();
                $scope.polling.indicator = "";
            }

            $scope.polling.buttonText = $scope.polling.enabled ? pollingOnText + " " + $scope.polling.interval + "s" : pollingOffText;
        }

        // Sets the polling interval
        $scope.setPollInterval = function (seconds) {
            $scope.polling.interval = seconds;
            $scope.polling.enabled = true;
            $scope.checkTimer();
        }

        // Toggles whether polling is enabled or not
        $scope.togglePolling = function () {
            $scope.polling.enabled = !$scope.polling.enabled;
            $scope.checkTimer();
        }

        // Reloads the log file
        $scope.reload = function (clear) {
            $route.reload();
            if (clear === true) {
                localStorage.removeItem("diploTraceLogTable");
                $scope.changePersist(false);
            }
        }

        // Cancels timer when focus is lost
        $window.onblur = function () {
            cancelTimer();
        };

        // Checks whether to enable timer when window focused
        $window.onfocus = function () {
            $scope.checkTimer();
        };

        // Stores the perist status
        $scope.changePersist = function (persist) {
            localStorage.setItem(persistKey, persist);
        }

        // Destroys the timer when the app is killed
        $scope.$on("$destroy", function (event) {
            cancelTimer();
        });

    });

// Directive to persist smart table state to local storage
app.directive('stPersist', function () {
    return {
        require: '^stTable',
        link: function (scope, element, attr, ctrl) {
            var nameSpace = attr.stPersist;

            //save the table state every time it changes
            scope.$watch(function () {
                return ctrl.tableState();
            }, function (newValue, oldValue) {
                if (newValue !== oldValue) {
                    localStorage.setItem(nameSpace, JSON.stringify(newValue));
                }
            }, true);

            //fetch the table state when the directive is loaded
            if (scope.persist && localStorage.getItem(nameSpace)) {
                var savedState = JSON.parse(localStorage.getItem(nameSpace));
                var tableState = ctrl.tableState();

                scope.pageSize = savedState.pagination.number;

                angular.extend(tableState, savedState);
                ctrl.pipe();
            }
            else if (!scope.persist) {
                ctrl.tableState().sort = { "predicate": "Date", "reverse": true };
                scope.pageSize = 100;
            }
        }
    };
});

// Directive to reset all smart table filters
app.directive("stResetSearch", function () {
    return {
        restrict: 'EA',
        require: '^stTable',
        link: function (scope, element, attrs, ctrl) {
            return element.bind('click', function () {
                return scope.$apply(function () {
                    var tableState;
                    tableState = ctrl.tableState();
                    tableState.search.predicateObject = {};
                    tableState.pagination.start = 0;
                    return ctrl.pipe();
                });
            });
        }
    };
})

// Directive to make the adjacent input clearable with a [x]
app.directive('diploClearable', function () {
    return {
        restrict: 'E',
        require: '^stTable',
        template: '<i class="icon icon-delete"></i>',
        link: function (scope, element, attrs, ctrl) {
            return element.bind('click', function () {
                return scope.$apply(function () {

                    var name = element.next().attr('st-search');
                    var params = ctrl.tableState().search.predicateObject;

                    if (params && params[name] !== undefined) {
                        params[name] = '';
                    }

                    return ctrl.pipe();
                });

            });
        }
    }
});

// Directive to highlight last word in a class name
app.filter('diploLastWordHighlight', function () {
    return function (input) {
        var items = input.split(".");
        var last = items.pop();
        return "<small>" + items.join(".") + "</small>." + last;
    }
});