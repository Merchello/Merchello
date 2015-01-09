angular.module('merchello.mocks')
    .factory('paymentResourceMock', ['$httpBackend', 'paymentMocks', 'mocksUtils', function($httpBackend, paymentMocks, mocksUtils) {

        function getPaymentsByInvoice() {
            return paymentMocks.paymentsArray();
        }

        return {
            register: function() {

                $httpBackend
                    .whenGET(mocksUtils.urlRegex('/umbraco/backoffice/Merchello/PaymentApi/GetPaymentsByInvoice?'))
                    .respond(getPaymentsByInvoice);
            }
        };

    }]);
