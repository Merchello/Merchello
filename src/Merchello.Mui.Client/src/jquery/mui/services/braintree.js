MUI.Services.Braintree = {

    initialized: false,

    acceptCards: [],
    
    currentCardType: '',

    events : [
        { attempt: 'unbindValidation', name: 'Braintree.UnbindValidation' },
        { attempt: 'cardTypeChanged',  name: 'Braintree.CardTypeChange' },
        { attempt: 'verified', name: 'Braintree.CardVerified' }
    ],

    // Load the required assets
    loadAssets: function(callback) {
        // load the braintree script and validation for cc with a promise
        // this also asserts if the customer goes back and changes the method to another
        // braintree method, that these are only loaded once
        $.when(
            MUI.Assets.cachedGetScript('/App_Plugins/Merchello/client/lib/card-validator.min.js'),
            MUI.Assets.cachedGetScript('//js.braintreegateway.com/v2/braintree.js')
        ).then(function() {
                MUI.Services.Braintree.initialized = true;
                if (callback !== undefined) callback();
            },
            function(err) { MUI.Logger.captureError(err); });

    },

    // Validates the entire card
    validateCard: function(creditCard) {
        if (typeof creditCard !== 'object') {
            return false;
        }

        return MUI.Providers.Braintree.validateCardNumber(creditCard.number) &&
            MUI.Providers.Braintree.validateExpires(creditCard.expirationDate) &&
            MUI.Providers.Braintree.validateCvv(creditCard.cvv) &&
            MUI.Providers.Braintree.validatePostalCode(creditCard.billingAddress.postalCode);
    },

    // Validates card number
    validateCardNumber: function(number) {
        return cardValidator.number(number);
    },

    // validates the expires date
    validateExpires: function(expires) {
        return cardValidator.expirationDate(expires);
    },

    // validates the cvv (matches the length to the card type)
    validateCvv: function(cvv) {
        return cardValidator.cvv(cvv);
    },

    // validates the postal code (at least 3 digits)
    validatePostalCode: function(postalCode) {
        return cardValidator.cvv(postalCode);
    },

    BraintreeCreditCard: function() {
        var self = this;
        self.cardholderName = '';
        self.number = '';
        self.cvv = '';
        self.expirationDate = '';
        self.billingAddress = {
            postalCode: ''
        };
    }
    
};
