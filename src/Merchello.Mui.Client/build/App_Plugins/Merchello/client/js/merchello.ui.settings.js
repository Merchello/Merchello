/*! MUI
 * https://merchello.com
 * Copyright (c) 2017 Across the Pond, LLC.
 * Licensed 
 */


if (MUI !== undefined) {

    // MUI Settings
    // Allows for overriding MUI defaults
    MUI.Settings = {

        Defaults: {
            BillingCountryCode: 'US',
            ShippingCountryCode: 'US'
        },

        Labels: {
            OutOfStock: 'Out of stock.',

            OutOfStockAllowPurchase: 'Out of stock - Back order available.',

            InStock: 'In stock (@0)'
        },

        // API endpoints
        // These can be adjusted to match custom controllers which implement base classes
        Endpoints: {
            // the basket surface controller end point
            basketSurface: '/umbraco/Merchello/StoreBasket/',

            // the wish list surface controller end point
            wishListSurface: '/umbraco/Merchello/StoreWishList/',

            // the product table api controller end point
            productTableApi: '/umbraco/Merchello/ProductDataTableApi/',

            // the braintree surface controller
            braintreePayPalSurface:   '/umbraco/fasttrack/BraintreePayPal/',
            braintreeStandardCcSurface: '/umbraco/fasttrack/BraintreeStandardCc/',

            // the checkout address surface controller end point
            countryRegionApi: '/umbraco/Merchello/CountryRegionApi/'
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

            overlay: '<div class="mui-overlay" data-muinotify="overlay"></div>'
        },

        // Payment handlers
        Payments: {

            cardtemplate: '<span class="pull-right" data-muivalue="cardtype"></span>',
            okcardtemplate: '[CT] <span class="glyphicon glyphicon-ok"></span>',
            invalidcardtemplate: '[CT] <span class="glyphicon glyphicon-remove"></span>',

            // if true a button to post the payment (nonce) back to the server to complete the payment.
            // if false, the payment will be submitted as soon as the nonce is received from braintree
            braintreePayPalRequiresBtn: false

        }
    }
};



