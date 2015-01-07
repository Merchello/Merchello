angular.module('merchello.mocks')
    .factory('paymentResourceMock', ['$httpBackend', 'paymentMocks', function($httpBackend, paymentMocks) {

        function getPaymentsByInvoice(invoiceKey) {
            return paymentMocks.paymentsArray();
        }

        return {
            register: function() {

                $httpBackend
                    .whenGET('/umbraco/backoffice/Merchello/PaymentApi/GetPaymentsByInvoice?')
                    .respond(getPaymentsByInvoice);
            }
        };

    }]);
