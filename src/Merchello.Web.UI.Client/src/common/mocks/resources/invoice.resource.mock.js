
angular.module('merchello.mocks')
    .factory('invoiceResourceMock', ['$httpBackend', 'mocksUtils', 'invoiceMocks',
        function($httpBackend, mocksUtils, invoiceMocks) {

            function getByKey(id) {
                var invoices = invoiceMocks.invoicesArray();
                return invoices[0];
            }

            function searchInvoices ()
            {
                var invoices = invoiceMocks.invoicesArray();

                var mockResults = {
                    currentPage: 1,
                    itemsPerPage: 25,
                    items: invoices,
                    totalItems: invoices.length,
                    totalPages: 1
                };

                return mockResults;
            }

            return {
                register: function() {
                    $httpBackend
                        .whenPOST(mocksUtils.urlRegex('/umbraco/backoffice/Merchello/InvoiceApi/SearchInvoices'))
                        .respond(searchInvoices());

                    $httpBackend
                        .whenGET(mocksUtils.urlRegex('/umbraco/backoffice/Merchello/InvoiceApi/GetInvoice?'))
                        .respond(getByKey());
                }
            };
        }]);