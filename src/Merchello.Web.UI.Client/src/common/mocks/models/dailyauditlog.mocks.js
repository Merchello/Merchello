
angular.module('merchello.mocks').factory('dailyAuditLogMock', ['mockHelper', function(mockHelper) {

    function dailyAuditLog() {
        return { "dailyLogs": [
                {   "day" : "2014-12-12T00:00:00",
                    "logs": [
                        {
                            "key": "31cd00b8-c089-4d3f-8364-e2cf81a54e2d",
                            "entityKey": "2f87b975-df08-4201-b576-d4da6757ffea",
                            "entityTfKey": "6263d568-dee1-41bb-8100-2333ecb4cf08",
                            "entityType": "Payment",
                            "message": "{\"area\":\"merchelloAuditLogs\",\"key\":\"paymentAuthorize\",\"invoiceTotal\":568.200000,\"currencyCode\":\"USD\"}",
                            "verbosity":0,
                            "isError":false,
                            "recordDate":"2014-12-12T10:53:28.01Z",
                            "extendedData":[]
                        },
                        {
                            "key": "988090a4-aef4-48eb-bfc9-db79381ef131",
                            "entityKey": "349ea387-849e-4e91-9d7c-13620dd2ee45",
                            "entityTfKey": "454539b9-d753-4c16-8ed5-5eb659e56665",
                            "entityType": "Invoice",
                            "message": "{\"area\":\"merchelloAuditLogs\",\"key\":\"invoiceCreated\",\"invoiceNumber\":\"9\"}",
                            "verbosity": 0,
                            "isError": false,
                            "recordDate": "2014-12-12T10:53:27.987Z",
                            "extendedData":[]
                        }
                    ]}
            ]};

    }

    return {
        dailyAuditLog: dailyAuditLog
    };

}]);