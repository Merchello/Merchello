'use strict';

angular.module('umbraco.resources').factory('diploTraceLogResources', function ($q, $http, umbRequestHelper) {
    return {
        getLogData: function (file) {
            return umbRequestHelper.resourcePromise(
                $http.get("Backoffice/TraceLogViewer/TraceLog/GetLogData", { params: { logfileName: file } })
            );
        },
        getLogFiles: function () {
            return umbRequestHelper.resourcePromise(
                $http.get("Backoffice/TraceLogViewer/TraceLog/GetLogFilesList")
            );
        },
        getLogDataResponse: function (file) {
            return umbRequestHelper.resourcePromise(
                $http.get("Backoffice/TraceLogViewer/TraceLog/GetLogDataResponse", { params: { logfileName: file } })
            );
        },
        getLastModifiedTime: function (file) {
            return umbRequestHelper.resourcePromise(
                $http.get("Backoffice/TraceLogViewer/TraceLog/GetLastModifiedTime", { params: { logfileName: file } })
            );
        }
    }
});