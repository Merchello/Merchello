/*! MUI
 * https://merchello.com
 * Copyright (c) 2016 Accross the Pond, LLC.
 * Licensed 
 */


if (MUI !== undefined) {

    // MUI Settings
    // Allows for overriding MUI defaults
    MUI.Settings = {

        eventHandlers: function() {
            // Braintree
            MUI.on('BraintreePayPal.success', MUI.Settings.Payments.braintreePayPalSuccess);
        },

        // Notifications - the notification bar
        Notifications: {
            // If true, the notification bar will be appended before the </body> tag and notfication
            // messages will be inserted into the bar
            enabled: true,

            infoCss: 'alert-info',
            successCss: 'alert-success',
            warnCss: 'alert-warning',
            errorCss: 'alert-danger',

            // the template for the notification bar
            template: '<div class="alert mui-notify-bar" data-muinotify="notifybar"><div class="container" data-muivalue="nofity">Success</div></div>',
        },

        // API endpoints
        // These can be adjusted to match custom controllers which implement base classes
        Endpoints: {
            // the basket surface controller end point
            basketSurface: '/umbraco/Merchello/Basket/',

            // the product table api controller end point
            productTableApi: '/umbraco/Merchello/ProductDataTableApi/',

            // the braintree surface controller
            brainTreeSurface:   '/umbraco/fasttrack/BraintreePayPal/'
        },

        // Payment handlers
        Payments: {

            // if true a button to post the payment (nonce) back to the server to complete the payment.
            // if false, the payment will be submitted as soon as the nonce is received from braintree
            braintreePayPalRequiresBtn: false,
            
            // redirection after a successful braintree paypal payment
            braintreePayPalSuccess: function() {
                var successUrl = $('#braintree-pay-pal-successurl').val();
                window.location = successUrl;
            }

        }
    }

    // bind the event handlers
    MUI.Settings.eventHandlers();
};



