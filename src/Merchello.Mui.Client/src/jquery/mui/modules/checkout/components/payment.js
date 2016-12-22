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
            // binds the standard transaction
            MUI.Checkout.Payment.Braintree.bind.btstandard.init();

            if($('#paypal-container').length > 0) {
                // binds the PayPal on time transaction
                MUI.Checkout.Payment.Braintree.bind.btpaypal.init();
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
                /*

                if (frm.length > 0) {
                    var cn = $(frm).find('[data-muivalue="cardnumber"]');
                    if (cn.length > 0) {
                        var icon = $(cn).next('[data-muivalue="cardtype"]');
                        if ($(icon.length === 0)) {
                            var span = MUI.Settings.Payments.cardtemplate;
                            $(cn).after(span);
                        }
                        $(cn).keyup(function() {
                            MUI.Services.Braintree.setCardLabel(cn);
                        });
                    }

                    // disable the form
                    $(frm).submit(function(e) {
                        e.preventDefault();

                        var cc = MUI.Checkout.Payment.Braintree.getBrainTreeCreditCard();
                        if (MUI.Services.Braintree.validateCard(cc)) {
                            var token = MUI.Checkout.Payment.Braintree.getBraintreeToken();
                            var setup = {
                                clientToken: token
                            };
                            var client = new braintree.api.Client(setup);
                            client.tokenizeCard(cc, function(err, nonce) {
                                if (err !== null) {
                                    MUI.Logger.captureError(err);
                                    //CO.Checkout.renderErrorMessages([ err ]);
                                    return false;
                                }

                                var data = { nonce: nonce };
                                var method = 'Process';

                                if (MUI.Checkout.Payment.Braintree.invoiceKey !== '') {
                                    data.invoiceKey = MUI.Checkout.Payment.Braintree.invoiceKey;
                                    method = 'Retry';
                                }

                                var url = MUI.Settings.Endpoints.braintreeStandardCcSurface + method;

                                MUI.Checkout.Payment.Braintree.postBraintreeForm(url, data);
                            });
                        }
                    });
                }

                */
            }
        },

        // PayPal payments through Braintree
        btpaypal: {

            init: function() {
                MUI.debugConsole('initializing PayPal OneTime Transaction');

                var token = MUI.Checkout.Payment.Braintree.getBraintreeToken();

                // TODO update to V3
                braintree.setup(token, "custom", {
                    paypal: {
                        container: "paypal-container"
                    },
                    onPaymentMethodReceived: function (obj) {


                        var frm = MUI.Checkout.Payment.Braintree.getBraintreeForm();
                        $(frm).submit(function(e) {
                           e.preventDefault();
                        });
                        var hidden = $(frm).find('[data-muivalue="btpaypalnonce"]');
                        MUI.Checkout.Payment.Braintree.token = hidden.val();
                        $(hidden).val(obj.nonce);

                        var btn = $(frm).find('.mui-requirejs');
                        if (MUI.Settings.Endpoints.braintreePayPalSurface !== '')
                        {
                            var method = 'Process';
                            var data = { nonce: $(hidden).val() };
                            if (MUI.Checkout.Payment.Braintree.invoiceKey !== '') {
                                method = 'Retry';
                                data.invoiceKey = MUI.Checkout.Payment.Braintree.invoiceKey;
                            }

                            var url = MUI.Settings.Endpoints.braintreePayPalSurface + method;
                            
                            // determine whether or not to show the submit button
                            if (MUI.Settings.Payments.braintreePayPalRequiresBtn !== undefined
                                && MUI.Settings.Payments.braintreePayPalRequiresBtn)
                            {
                                $(btn).show();
                                $(btn).click(function(e) {
                                    e.preventDefault();
                                    MUI.Checkout.Payment.Braintree.postBraintreeForm(url, data);
                                });
                            } else {
                                MUI.Checkout.Payment.Braintree.postBraintreeForm(url, data);
                            }
                        }
                    }
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
    },

    // OBSOLETE
    getBrainTreeCreditCard: function() {

        try {
            var cc = new MUI.Services.Braintree.BraintreeCreditCard();
            cc.cardholderName = $('#CardHolder').val();
            cc.number = $('#CardNumber').val();
            cc.expirationDate = $('#ExpiresMonthYear').val();
            cc.cvv = $('#Cvv').val();
            cc.billingAddress.postalCode = $('#PostalCode').val();

            return cc;
        }
        catch(err) {
            MUI.Logger.captureError(err);
            throw err;
        }
    }
};
