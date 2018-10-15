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
            MUI.Assets.cachedGetScript('/App_Plugins/Merchello/client/lib/card-validator.min.js'), // Verify we don't need this anymore
            MUI.Assets.cachedGetScript('//js.braintreegateway.com/web/3.38.1/js/client.min.js'),
            MUI.Assets.cachedGetScript('//js.braintreegateway.com/web/3.38.1/js/hosted-fields.min.js'),
            MUI.Assets.cachedGetScript('//js.braintreegateway.com/web/3.38.1/js/paypal.min.js')
            // MUI.Assets.cachedGetScript('//js.braintreegateway.com/v2/braintree.js')
        ).then(function() {
                MUI.Services.Braintree.initialized = true;
                if (callback !== undefined) callback();
            },
            function(err) { MUI.Logger.captureError(err); });

    }
};
