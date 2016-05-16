MUI.Services.Braintree = {

    initialized: false,

    acceptCards: [],
    
    currentCard: {},

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
        return MUI.Services.Braintree.validateCardNumber(creditCard.number).isValid &&
            MUI.Services.Braintree.validateExpires(creditCard.expirationDate).isValid &&
            MUI.Services.Braintree.validateCvv(creditCard.cvv).isValid &&
            MUI.Services.Braintree.validatePostalCode(creditCard.billingAddress.postalCode);
    },

    // Validates card number
    validateCardNumber: function(number) {
        var valid = cardValidator.number(number);
        if (!valid.isValid) MUI.Notify.warn('Invalid credit card number');
        return valid;
    },

    // validates the expires date
    validateExpires: function(expires) {
        var valid = cardValidator.expirationDate(expires);
        if (!valid.isValid) MUI.Notify.warn('Invalid credit card expiration date');
        return valid;
    },

    // validates the cvv (matches the length to the card type)
    validateCvv: function(cvv) {
        var valid = cardValidator.cvv(cvv);
        if (!valid.isValid) MUI.Notify.warn('Invalid credit card cvv');
        return valid;
    },

    // validates the postal code (at least 3 digits)
    validatePostalCode: function(postalCode) {
        var valid = cardValidator.postalCode(postalCode)
        return valid;
    },

    // Sets the card icon
    setCardLabel: function (el) {

        if (!el.length > 0) return;

        var valid = cardValidator.number($(el).val());

        // remove the previous icon
        var icon = $(el).next('[data-muivalue="cardtype"]');

        // if the card is not defined in the event from Braintree clear the saved card type
        if (!valid.card) {
            MUI.Services.Braintree.currentCard = {};
            $(icon).empty();
            return;
        }

        if (valid.card && valid.isPotentiallyValid) {
            MUI.Services.Braintree.currentCard = MUI.Utilities.CardTypes.getCardByType(valid.card.type);
            var rpl = MUI.Services.Braintree.currentCard.niceType;
            var template = valid.isValid ?
                MUI.Settings.Payments.okcardtemplate :
                MUI.Settings.Payments.invalidcardtemplate;

            var template = template.replace('[CT]', rpl);
            $(icon).html(template);

        }
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
