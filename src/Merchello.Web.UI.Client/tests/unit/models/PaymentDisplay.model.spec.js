'use strict';

describe('PaymentDisplay prototype tests', function() {

    beforeEach(module('umbraco'));

    it('getStatus should return Authorized/Captured',
        inject(function (paymentDisplayBuilder, paymentMocks) {

            //// Arrange
            var payments = paymentMocks.paymentsArray();
            var payment = paymentDisplayBuilder.transform(payments[0]);

            //// Act
            var status = payment.getStatus();
            expect (status).toBe('Authorized/Captured');
        }));

    it('hasAmount should return true for a payment amount greater than zero',
        inject(function(paymentDisplayBuilder, paymentMocks) {

            //// Arrange
            var mock = paymentMocks.randomPayment();
            var payment = paymentDisplayBuilder.transform(mock);

            //// Act
            var hasAmount = payment.hasAmount();

            //// Assert
            expect (hasAmount).toBeTruthy();

    }));
});
