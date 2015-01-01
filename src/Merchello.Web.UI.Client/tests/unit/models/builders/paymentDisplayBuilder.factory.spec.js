'use strict';

describe('paymentDisplayBuilder', function() {

    beforeEach(module('umbraco'));

    it('should be able to create a default PaymentDisplay', inject(function(paymentDisplayBuilder, PaymentDisplay){

        var payment = paymentDisplayBuilder.createDefault();

        expect(payment).toBeDefined();

    }));

    it('should be able to create an array of PaymentDisplay for an api jsonResult',
        inject(function(paymentDisplayBuilder, paymentMocks, PaymentDisplay) {

            //// Arrange
            var jsonResult = paymentMocks.paymentsArray();
            var count = jsonResult.length;

            //// Act
            var payments = paymentDisplayBuilder.transform(jsonResult);

            //// Assert
            expect (payments).toBeDefined();
            expect (payments.length).toBe(count);
            expect(jsonResult[0].key).toEqual(payments[0].key)
        }));

});
