//// A class to manage braintree payments
MUI.Checkout.Payment.Braintree = {

    invoiceKey: '',

    token: '',
    
    events : [
        { alias: 'btsuccess', name: 'Braintree.success' }
    ],

    // initialize payment form
    init: function() {
        var btforms = MUI.Checkout.Payment.Braintree.getBraintreeForm();
        if ($(btforms).length > 0) {
            if (!MUI.Services.Braintree.initialized) {
                MUI.Services.Braintree.loadAssets(function() {
                    MUI.Checkout.Payment.Braintree.bind.allForms();
                });
            } else {
                MUI.Checkout.Payment.Braintree.bind.allForms();
            }
        }
    },

    bind: {

        allForms: function() {

            if($('.paypal-button').length > 0) {
                // binds the PayPal on time transaction
                MUI.Checkout.Payment.Braintree.bind.btpaypal.init();
            } else {
                // binds the standard transaction
                MUI.Checkout.Payment.Braintree.bind.btstandard.init();
            }
        },
        
        btstandard: {

            init: function() {
                MUI.debugConsole('Initializing Braintree CC form');

                var form = MUI.Checkout.Payment.Braintree.getBraintreeForm();

                if (form.length > 0) {


                    var token = MUI.Checkout.Payment.Braintree.getBraintreeToken();

                    braintree.client.create({
                        authorization: token
                    }, function (err, clientInstance) {
                        if (err) {
                            console.error(err);
                            return;
                        }
                        braintree.hostedFields.create({
                            client: clientInstance,
                            styles: {
                                'input': {
                                    'font-size': '14px',
                                    'font-family': 'helvetica, tahoma, calibri, sans-serif',
                                    'color': '#3a3a3a'
                                },
                                ':focus': {
                                    'color': 'black'
                                }
                            },
                            fields: {
                                number: {
                                    selector: '#card-number',
                                    placeholder: '4111 1111 1111 1111'
                                },
                                cvv: {
                                    selector: '#cvv',
                                    placeholder: '123'
                                },
                                expirationMonth: {
                                    selector: '#expiration-month',
                                    placeholder: 'MM'
                                },
                                expirationYear: {
                                    selector: '#expiration-year',
                                    placeholder: 'YY'
                                },
                                postalCode: {
                                    selector: '#postal-code',
                                    placeholder: '90210'
                                }
                            }
                        }, function (err, hostedFieldsInstance) {
                            if (err) {
                                console.error(err);
                                return;
                            }

                            hostedFieldsInstance.on('validityChange', function (event) {
                                var field = event.fields[event.emittedBy];

                                if (field.isValid) {
                                    if (event.emittedBy === 'expirationMonth' || event.emittedBy === 'expirationYear') {
                                        if (!event.fields.expirationMonth.isValid || !event.fields.expirationYear.isValid) {
                                            return;
                                        }
                                    } else if (event.emittedBy === 'number') {
                                        $('#card-number').next('span').text('');
                                    }

                                    // Apply styling for a valid field
                                    $(field.container).parents('.form-group').addClass('has-success');
                                } else if (field.isPotentiallyValid) {
                                    // Remove styling  from potentially valid fields
                                    $(field.container).parents('.form-group').removeClass('has-warning');
                                    $(field.container).parents('.form-group').removeClass('has-success');
                                    if (event.emittedBy === 'number') {
                                        $('#card-number').next('span').text('');
                                    }
                                } else {
                                    // Add styling to invalid fields
                                    $(field.container).parents('.form-group').addClass('has-warning');
                                    // Add helper text for an invalid card number
                                    if (event.emittedBy === 'number') {
                                        $('#card-number').next('span').text('Looks like this card number has an error.');
                                    }
                                }
                            });

                            hostedFieldsInstance.on('cardTypeChange', function (event) {
                                // Handle a field's change, such as a change in validity or credit card type
                                if (event.cards.length === 1) {
                                    $('#card-type').text(event.cards[0].niceType);
                                } else {
                                    $('#card-type').text('Card');
                                }
                            });

                            $('.panel-body').submit(function (event) {
                                event.preventDefault();
                                hostedFieldsInstance.tokenize(function (err, payload) {
                                    if (err) {
                                        console.error(err);
                                        return;
                                    }

                                    var data = { nonce: payload.nonce };
                                    var method = 'Process';

                                    if (MUI.Checkout.Payment.Braintree.invoiceKey !== '') {
                                        data.invoiceKey = MUI.Checkout.Payment.Braintree.invoiceKey;
                                        method = 'Retry';
                                    }

                                    var url = MUI.Settings.Endpoints.braintreeStandardCcSurface + method;

                                    MUI.Checkout.Payment.Braintree.postBraintreeForm(url, data);
                                });
                            });
                        });
                    });
                }
            }
        },

        // PayPal payments through Braintree
        btpaypal: {

            init: function() {
                MUI.debugConsole('initializing PayPal OneTime Transaction');

                var token = MUI.Checkout.Payment.Braintree.getBraintreeToken();

                var paypalButton = document.querySelector('.paypal-button');

                // Create a client.
                braintree.client.create({
                    authorization: token
                }, function (clientErr, clientInstance) {

                    // Stop if there was a problem creating the client.
                    // This could happen if there is a network error or if the authorization
                    // is invalid.
                    if (clientErr) {
                        console.error('Error creating client:', clientErr);
                        return;
                    }

                    // Create a PayPal component.
                    braintree.paypal.create({
                        client: clientInstance
                    }, function (paypalErr, paypalInstance) {

                        // Stop if there was a problem creating PayPal.
                        // This could happen if there was a network error or if it's incorrectly
                        // configured.
                        if (paypalErr) {
                            console.error('Error creating PayPal:', paypalErr);
                            return;
                        }

                        // Enable the button.
                        paypalButton.removeAttribute('disabled');

                        // When the button is clicked, attempt to tokenize.
                        paypalButton.addEventListener('click', function (event) {

                            // Because tokenization opens a popup, this has to be called as a result of
                            // customer action, like clicking a button—you cannot call this at any time.
                            paypalInstance.tokenize({
                                flow: 'vault'
                            }, function (tokenizeErr, payload) {

                                // Stop if there was an error.
                                if (tokenizeErr) {
                                    if (tokenizeErr.type !== 'CUSTOMER') {
                                        console.error('Error tokenizing:', tokenizeErr);
                                    }
                                    return;
                                }

                                // Tokenization succeeded!
                                paypalButton.setAttribute('disabled', true);

                                if (MUI.Settings.Endpoints.braintreePayPalSurface !== '')
                                {
                                    var method = 'Process';
                                    var data = { nonce: payload.nonce };
                                    if (MUI.Checkout.Payment.Braintree.invoiceKey !== '') {
                                        method = 'Retry';
                                        data.invoiceKey = MUI.Checkout.Payment.Braintree.invoiceKey;
                                    }

                                    var url = MUI.Settings.Endpoints.braintreePayPalSurface + method;
                                    MUI.Checkout.Payment.Braintree.postBraintreeForm(url, data);
                                }
                            });

                        }, false);
                    });

                });
            }

        }
    },

    postBraintreeForm: function(url, data) {
        MUI.Notify.toggleOverlay();
        $.ajax({
            url: url,
            type: 'POST',
            data: data
        }).then(function(result) {
            MUI.Checkout.Payment.Braintree.handlePaymentResult(result, 'Braintree.success');
        }, function(err) {
            MUI.Checkout.Payment.Braintree.handlePaymentException(err);
        });
    },

    handlePaymentResult: function(result, evt) {
        MUI.Notify.toggleOverlay();
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
        MUI.Notify.toggleOverlay();
        MUI.Notify.error('There was an error');
        MUI.Logger.captureError(err);
    },

    getBraintreeToken: function() {
        var frm = MUI.Checkout.Payment.Braintree.getBraintreeForm();
        var token = $(frm).find('#Token');
        return $(token).val();
    },

    getBraintreeForm: function() {
        return $('[data-muiscript="braintree"]');
    }
};
