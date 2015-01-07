angular.module('merchello.mocks')
    .factory('auditLogResourceMock', ['$httpBackend', 'dailyAuditLogMock', 'salesHistoryDisplayBuilder',
        function($httpBackend, dailyAuditLogMock, salesHistoryDisplayBuilder) {

            function getSalesHistoryByInvoiceKey(key) {
                return dailyAuditLogMock.dailyAuditLog();
            }

            return {
                register: function() {
                    $httpBackend
                        .whenGET('/umbraco/backoffice/Merchello/AuditLogApi/GetSalesHistoryByInvoiceKey')
                        .respond(getSalesHistoryByInvoiceKey());
                }
            };

        }]);
