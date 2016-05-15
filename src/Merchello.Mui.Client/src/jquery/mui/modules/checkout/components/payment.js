//// A class to manage payments
MUI.Checkout.Payment = {

    invoiceKey: '',

    token: '',

    events : [
        { alias: 'btpaypalsuccess', name: 'BraintreePayPal.success' }
    ],

    // initialize payment form
    init: function() {
        var btforms = MUI.Checkout.Payment.getBraintreeForm();
        if ($(btforms).length > 0) {
            if (!MUI.Services.Braintree.initialized) {
                MUI.Services.Braintree.loadAssets(function() {
                    MUI.Checkout.Payment.bind.allForms();
                });
            } else {
                MUI.Checkout.Payment.bind.allForms();
            }
        }
    },

    bind: {

        allForms: function() {
            // binds the standard transaction
            MUI.Checkout.Payment.bind.btstandard.init();
            // binds the PayPal on time transaction
            MUI.Checkout.Payment.bind.btpaypal.init();
        },
        
        btstandard: {

            init: function() {

            }
        },

        // PayPal payments through Braintree
        btpaypal: {

            init: function() {
                console.info('initializing PayPal OneTime Transaction');

                var token = MUI.Checkout.Payment.getBraintreeToken();

                braintree.setup(token, "custom", {
                    paypal: {
                        container: "paypal-container"
                    },
                    onPaymentMethodReceived: function (obj) {


                        var frm = MUI.Checkout.Payment.getBraintreeForm();
                        $(frm).submit(function(e) {
                           e.preventDefault();
                        });
                        var hidden = $(frm).find('[data-muivalue="btpaypalnonce"]');
                        MUI.Checkout.Payment.token = hidden.val();
                        $(hidden).val(obj.nonce);

                        var btn = $(frm).find('.mui-requirejs');
                        if (MUI.Settings.Endpoints.brainTreeSurface !== '')
                        {
                            var method = 'Process';
                            var data = { nonce: $(hidden).val() };
                            if (MUI.Checkout.Payment.invoiceKey !== '') {
                                method = 'Retry';
                                data.invoiceKey = MUI.Checkout.Payment.invoiceKey;
                            }

                            // determine whether or not to show the submit button
                            if (MUI.Settings.Payments.braintreePayPalRequiresBtn !== undefined
                                && MUI.Settings.Payments.braintreePayPalRequiresBtn)
                            {
                                $(btn).show();
                                $(btn).click(function(e) {
                                    e.preventDefault();
                                    MUI.Checkout.Payment.postPayPalForm(method, data);
                                });
                            } else {
                                MUI.Checkout.Payment.postPayPalForm(method, data);
                            }
                        }
                    }
                });
            }

        }
    },

    postPayPalForm: function(method, data) {
        $.ajax({
            url: MUI.Settings.Endpoints.brainTreeSurface + method,
            type: 'POST',
            data: data
        }).then(function(result) {
            MUI.Checkout.Payment.handlePaymentResult(result, 'BraintreePayPal.success');
        }, function(err) {
            MUI.Checkout.Payment.handlePaymentException(err);
        });
    },

    handlePaymentResult: function(result, evt) {
        if (result.Success) {
            MUI.emit('AddItem.added', result);
            MUI.emit(evt, result);
            MUI.Notify.success(result.PaymentMethodName + ' Success!');
        } else {
            console.info(result);
            MUI.Notify.warn(result.PaymentMethodName + ' Error!')
        }
    },

    handlePaymentException: function(err) {
        MUI.Notify.error('There was an error');
        MUI.Logger.captureError(err);
    },

    getBraintreeToken: function() {
        var frm = MUI.Checkout.Payment.getBraintreeForm();
        var token = $(frm).find('#Token');
        return $(token).val();
    },

    getBraintreeForm: function() {
        return $('[data-muiscript="braintree"]');
    }
};
