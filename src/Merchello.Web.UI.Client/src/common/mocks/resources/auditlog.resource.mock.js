angular.module('merchello.mocks')
    .factory('auditLogResourceMock', ['$httpBackend', 'dailyAuditLogMock', 'mocksUtils',
        function($httpBackend, dailyAuditLogMock, mocksUtils) {

            function getSalesHistoryByInvoiceKey() {
                var dailyLog = dailyAuditLogMock.dailyAuditLog();
                return dailyLog;
            }

            return {
                register: function() {
                    $httpBackend
                        .whenGET(mocksUtils.urlRegex('/umbraco/backoffice/Merchello/AuditLogApi/GetSalesHistoryByInvoiceKey'))
                        .respond(getSalesHistoryByInvoiceKey);
                }
            };

        }]);
