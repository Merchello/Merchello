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