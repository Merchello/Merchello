/*! MUI
 * https://merchello.com
 * Copyright (c) 2017 Across the Pond, LLC.
 * Licensed 
 */


// JS Overrides for the FastTrack starter
// must be included AFTER merchello.ui.js
(function() {
    // Braintree Success (handle the redirect)
    MUI.on('Braintree.success', redirectBraintreeSuccess);
    MUI.on('AddItem.added', hideShowCheckoutButton);

    function redirectBraintreeSuccess() {
        var hidden = $('#braintree-successurl');
        if (hidden.length > 0) {
            var successUrl = $(hidden).val();
            window.location = successUrl;
        }
    }
    
    // membership
    function hideShowCheckoutButton(evt, args) {
        var target = $('[data-value="quickcheckout"]');
        if (target.length > 0) {
            if (args.BasketItemCount !== undefined) {
                if (args.BasketItemCount > 0) {
                    $(target).show();
                } else {
                    $(target).hide();
                }
            }
        }
    }
    
    

})();

