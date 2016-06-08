MUI.Checkout = {

    Payment: {},

    init: function() {
        // Initialize the address module
        MUI.Checkout.Address.init();
        // Initialize the payment module
        MUI.Checkout.Payment.Braintree.init();
    }
    
};
