/*! MUI
 * https://merchello.com
 * Copyright (c) 2016 Across the Pond, LLC.
 * Licensed 
 */


// JS Overrides for the FastTrack starter
// must be included AFTER merchello.ui.js
(function() {
    // Braintree Success (handle the redirect)
    MUI.on('Braintree.success', redirectBraintreeSuccess);

    function redirectBraintreeSuccess() {
        var hidden = $('#braintree-successurl');
        if (hidden.length > 0) {
            var successUrl = $(hidden).val();
            window.location = successUrl;
        }
    }
})();

