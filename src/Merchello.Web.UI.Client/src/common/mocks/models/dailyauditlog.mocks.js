
angular.module('merchello.mocks').factory('dailyAuditLogMock', ['mockHelper', function(mockHelper) {

    function dailyAuditLog() {

        return mockHelper.downCasePropertiesInObjectArray(
        {"DailyLogs":[{"Day":"2014-12-19T00:00:00","Logs":[{"Key":"fe63fd33-2c64-4da1-81fd-b93d741c2701","EntityKey":"e4a30ea3-b8a5-43f9-9b00-9b39fd1ef758","EntityTfKey":"6263d568-dee1-41bb-8100-2333ecb4cf08","EntityType":"Payment","Message":"{\"area\":\"merchelloAuditLogs\",\"key\":\"paymentCaptured\",\"invoiceTotal\":1104.0,\"currencyCode\":\"\"}","Verbosity":0,"IsError":false,"RecordDate":"2014-12-19T17:43:32.863Z","ExtendedData":[]}]}]}
        );
    }

    return {
        dailyAuditLog: dailyAuditLog
    };

}]);