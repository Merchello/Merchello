
angular.module('merchello.mocks')
    .factory('invoiceResourceMock', ['$httpBackend', 'invoiceMocks', 'queryResultDisplayBuilder',
        function($httpBackend, invoiceMocks) {

            function searchInvoices ()
            {
                var invoices = invoiceMocks.invoicesArray();

                var mockResults = {
                    currentPage: 1,
                    itemsPerPage: 25,
                    items: invoices,
                    totalItems: 16,
                    totalPages: 1
                };

                return [200, mockResults, null];
            }

            return {
                register: function() {
                    $httpBackend
                        .whenPOST('/umbraco/backoffice/Merchello/InvoiceApi/SearchInvoices')
                        .respond(searchInvoices());
                }
            };
        }]);