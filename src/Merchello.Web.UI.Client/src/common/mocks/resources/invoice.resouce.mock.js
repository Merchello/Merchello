
angular.module('merchello.mocks')
    .factory('invoiceResouceMock', ['$httpBackend', 'invoiceMocks',
        function($httpBackend, invoiceMocks) {

            function searchInvoices (query)
            {
                return invoiceMocks.invoicesArray();
            }

            return {
                register: function() {
                    $httpBackend
                        .whenPOST('/umbraco/backoffice/Merchello/InvoiceApi/SearchInvoices')
                        .respond(searchInvoices());
                }
            };
        }]);