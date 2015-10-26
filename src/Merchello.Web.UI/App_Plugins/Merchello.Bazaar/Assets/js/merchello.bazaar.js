// merchello.bazaar.js | requires jQuery 1.10.1+

(function(merchello, undefined) {

    merchello.bazaar = {
        settings: {
            defaultBillingCountryCode: 'US',
            defaultShippingCountryCode: 'US'
        },
        init: function() {
            $(document).ready(function() {
                merchello.bazaar.products.bind();
                merchello.bazaar.checkout.bind();
                merchello.bazaar.account.bind();
            });
        },
        account: {
            bind: function() {
                merchello.bazaar.account.resetViews();

                // profile forms
                $('#btn-profile-open').click(function() {
                    $('#profile-form').show();
                    $('#address-view').hide();
                    
                });
                $('#btn-profile-form-cancel').click(function() {
                    merchello.bazaar.account.resetViews();
                });
                $('#chk-change-password').click(function () {
                    if (this.checked) {
                        $('#change-password-form').show();
                    } else {
                        $('#change-password-form').hide();
                    }
                });

                // address edit buttons
                $('#btn-add-address-billing').click(function() {
                    merchello.bazaar.account.customerAddressForm.init('billing');
                });
                $('#btn-add-address-shipping').click(function () {
                    merchello.bazaar.account.customerAddressForm.init('shipping');
                });
                $.each($('.address-edit-link'), function(index, btn) {
                    $(btn).click(function() {
                        merchello.bazaar.account.customerAddressForm.init($(this).attr('data-adddresstype'), $(this).attr('data-addresskey'));
                    });
                });
                $('#btn-address-form-cancel').click(function () {
                    merchello.bazaar.account.resetViews();
                });

                // adddress country drop downs
                $.each($('.country-selection'), function(index, ddl) {
                    $(ddl).change(function() {
                        merchello.bazaar.account.setProvinces(this);
                    });
                });

                $('#address-province-select').change(function() {
                    $('#address-province-entry').val($(this).val());
                });
                $('#address-province-entry').val($('#address-province-select').val());

            },
            toggleProfileForm: function() {
                $('#btn-profile-open').attr('disabled', 'disabled');
            },
            customerAddressForm: {
                init: function (type, addressKey) {
                    $('#address-form').show();
                    $('#address-view').hide();
                    var label = (type === 'shipping' ? ' Shipping ' : ' Billing ') + 'Address';
                    var countryDdl = ('#address-countrycode-' + type);
                    $(countryDdl).show();
                    merchello.bazaar.account.setProvinces(countryDdl);
                    $('#address-countrycode-' + (type === 'shipping' ? 'billing' : 'shipping')).hide();
                    $('#address-addresstype').val(type);
                    if (addressKey) {
                        // this is an update
                        var data = {};
                        data.customerAddressKey = addressKey;
                        $.ajax({
                            type: "GET",
                            url: "/umbraco/Bazaar/BazaarSiteApi/GetCustomerAddress",
                            data: data,
                            success: function (adr) {
                                $('#address-caption').text('Edit Your ' + label);
                                $('#address-label').val(adr.label);
                                $('#address-fullname').val(adr.fullName);
                                $('#address-address1').val(adr.address1);
                                $('#address-address2').val(adr.address2);
                                $('#address-locality').val(adr.locality);
                                $('#address-region-entry').val(adr.region);
                                $('#address-province-select').val(adr.region);
                                if (adr.addressType === 'shipping') {
                                    $('#address-countrycode-shipping').val(adr.countryCode);
                                } else {
                                    $('#address-countrycode-billing').val(adr.countryCode);
                                }
                                $('#address-postalcode').val(adr.postalCode);
                                $('#address-phone').val(adr.phone);
                                $('#address-key').val(adr.key);
                            },
                            dataType: "json",
                            traditional: true
                        }).fail(function (ex) {
                            $.error(ex);
                        });

                    } else {
                        $('#address-caption').text('Add a New ' + label);
                    }
                }  
            },
            setProvinces: function (elem) {
                var countryCode = $(elem).val();
                var data = {};
                data.countryCode = countryCode;
                $.ajax({
                    type: "GET",
                    url: "/umbraco/Bazaar/BazaarSiteApi/GetProvincesForCountry",
                    data: data,
                    success: function (provinces) {
                        if (provinces.length > 0) {
                            $('#address-province-select').show();
                            $('#address-province-select').find('option').remove();
                            $.each(provinces, function (idx, province) {
                                $('#address-province-select').append('<option value=\'' + province.code + '\'>' + province.name + '</option>')
                            });
                            $('#address-province-entry').hide();
                        } else {
                            $('#address-province-select').hide();
                            $('#address-province-entry').show();
                        }
                    },
                    dataType: "json",
                    traditional: true
                }).fail(function (ex) {
                    $.error(ex);
                });

            },
            resetViews: function () {
                if (window.location.hash === '#success') {
                    $('#message').show();
                    $('#message').delay(2000).fadeOut(1000);
                    window.location.hash = '';
                } else {
                    $('#message').hide();
                }
                $('#btn-profile-open').removeAttr('disabled');
                $('#address-view').show();
                $('#btn-add-address-billing').removeAttr('disabled');
                $('#btn-add-address-shipping').removeAttr('disabled');
                $('#address-countrycode-billing').val('US');
                $('#address-form').hide();
                $('#profile-form').hide();
                $('#btn-profile-form').attr('disabled', 'disabled');
                // address
                $('#address-label').val('');
                $('#address-fullname').val('');
                $('#address-address1').val('');
                $('#address-address2').val('');
                $('#address-locality').val('');
                $('#address-region-entry').val('');
                $('#address-province-select').val('');
                $('#address-postalcode').val('');
                $('#address-phone').val('');
                $('#address-key').val('');

                // reset form validations
                $('.field-validation-error')
                    .empty()
                    .removeClass('field-validation-error')
                    .addClass('field-validation-valid');
                $('.input-validation-error')
                    .removeClass('input-validation-error')
                    .addClass('valid');
            }
        },

        products: {
            bind: function () {
                
                $.each($('.add-to-cart.has-variants'), function(index, elem) {

                    var key = $(elem).find('.variant-pricing').attr('id');

                    // initialize the price
                    merchello.bazaar.products.getVariantPriceAndInventory(elem, key);

                    var variants = $(elem).find('.ProductVariants');
                    if (variants.length > 0) {
                        merchello.bazaar.products.getUpdatedVariants(elem, variants[0], key);
                        // bind to the select element
                        $.each($(variants), function (idx, ddl) {
                            $(ddl).change(function () {
                                merchello.bazaar.products.getUpdatedVariants(elem, ddl, key);
                            });
                        });
                    }
                });

            },
            updateOptionChoices: function (filteredOptions) {
                $.each(filteredOptions, function (index, option) {
                    var ddl = $('#' + option.key);
                    var currentSelection = $(ddl).val();
                    if (currentSelection === '' || currentSelection === null) {
                        $(ddl).val($('#' + $(ddl).attr('id') + ' option:first').val());
                    }
                    $(ddl).find('option')
                        .remove();
                        $.each(option.choices, function (i, opt) {
                            $(ddl).append('<option value=\'' + opt.key + '\'>' + opt.name + '</option>');
                        });
                        $(ddl).val(currentSelection);
                        if ($(ddl).val() === '' || $(ddl).val() === null) {
                            $(ddl).val($('#' + $(ddl).attr('id') + ' option:first').val());
                        }
                });

            },
            getVariantPriceAndInventory: function (elem, key) {
                var options = "";
                $.each($(elem).find(".ProductVariants"), function (index, element) {
                    options = options + $(element).val() + ",";
                });

                var variantOptions = {};
                variantOptions.productKey = key;
                variantOptions.optionChoiceKeys = options;

                $.ajax({
                    type: "GET",
                    url: "/umbraco/Bazaar/BazaarSiteApi/GetProductVariantPriceAndInventory",
                    data: variantOptions,
                    success: function (data) {
                        var price = $("#" + key);
                        $(price).empty();
                        if (data.onSale) {
                            $(price).append('<span class="sale-price">' + data.salePrice + '</span><span class="original-price">' + data.price + '</span>');
                        } else {
                            $(price).append('<span>' + data.price + '</span>');
                        }
                        var inv = $('#inv-' + key);
                        var btn = $('#btn-' + key);
                        if (data.tracksInventory) {
                            $(inv).show();
                            $(inv).empty();
                            if (data.totalInventoryCount > 0) {
                                $(inv).append('<span class=\'inventory\'>In stock (' + data.totalInventoryCount + ')');
                                $(btn).removeAttr('disabled');
                                $(btn).show();
                            } else {
                                $(inv).append('<span class=\'inventory\'>Out of stock');
                                $(btn).attr('disabled', 'disabled');
                                $(btn).hide();
                            }
                        } else {
                            $(btn).show();
                            $(btn).removeAttr('disabled');
                        } 
                    },
                    dataType: "json",
                    traditional: true
                }).fail(function(ex) {
                    $.error(ex);
                });
            },
            getUpdatedVariants: function(root, ddl, key) {

                //var productAttributeKey = ddl.selectedOptions[0].value;
                var productAttributeKey = $(ddl).val();

                var filter = {};
                filter.productKey = key;
                filter.productAttributeKey = productAttributeKey;

                $.ajax({
                    type: "GET",
                    url: "/umbraco/Bazaar/BazaarSiteApi/FilterOptionChoices",
                    data: filter,
                    success: function (filteredOptions) {
                        merchello.bazaar.products.updateOptionChoices(filteredOptions);
                        merchello.bazaar.products.getVariantPriceAndInventory(root, key);
                    },
                    dataType: "json",
                    traditional: true
                }).fail(function(ex) {
                    $.error(ex);
                });
            }
        },

        checkout: {
            bind: function () {
                  $('#shipping-address').hide();
                if ($('#addresses-form').length) {
                    // copy the billing address if needed
                    $('#addresses-form').submit(function(event) {
                        merchello.bazaar.checkout.copyBillingAddress(event);
                    });
                
                    // bind the country dropdowns
                    if ($('.country-selection').length > 0) {

                        $('#billing-country-select').val(merchello.bazaar.settings.defaultBillingCountryCode);

                        $('.country-selection').each(function (idx, ddl) {
                            $(ddl).change(function () {
                                merchello.bazaar.checkout.setProvinces(this);
                                if ($(this).attr('data-addresstype') === 'billing') {
                                    merchello.bazaar.checkout.validateShippingCountry(this);
                                }
                            });
                            merchello.bazaar.checkout.setProvinces(ddl);
                            
                        });
                        // set the province ddl to update the region textbox
                        $('#billing-province-select').change(function () {
                            $('#billing-province-entry').val($(this).val());
                        });
                    }

                    // use billing checkbox
                    if ($('#billing-is-shipping').length > 0) {
                        $('#billing-is-shipping').click(function () {
                            merchello.bazaar.checkout.toggleBillingIsShipping();
                        });
                    }


                    // bind the customer address drop downs
                    if ($('#billing-address-select').length > 0) {
                        // TODO this should not be necessary
                        $('#billing-address-select').removeAttr('data-val').removeAttr('data-val-required');

                        $('#billing-address-select').change(function() {
                            if ($(this).val() !== '00000000-0000-0000-0000-000000000000') {
                                $('#billing-address').hide();
                                $('#billing-vcard').show();
                                merchello.bazaar.checkout.setCustomerAddress('billing', $(this).val());
                                
                            } else {
                                $('#billing-address').show();
                                $('#billing-vcard').hide();
                            }
                            merchello.bazaar.checkout.refreshCustomerAddressViewSettings();
                        });
                    }
                    if ($('shipping-address-select').length > 0) {
                        $('#shipping-address-select').removeAttr('data-val').removeAttr('data-val-required');
                        $('#shipping-address-select').change(function() {
                            if ($(this).val() !== '00000000-0000-0000-0000-000000000000') {
                                $('#billing-is-shipping-check').hide();
                                $('#shipping-vcard').show();
                                merchello.bazaar.checkout.setCustomerAddress('shipping', $(this).val());
                                $('#shipping-address').hide();
                            } else {
                                $('#billing-is-shipping-check').show();
                                if (!$('#billing-is-shipping').is(':checked')) {
                                    $('#shipping-address').show();
                                }
                                $('#shipping-vcard').hide();

                            }
                            merchello.bazaar.checkout.refreshCustomerAddressViewSettings();
                        });
                    }

                    if ($('#save-addresses-check').length > 0) {
                        $('#save-addresses-check').click(function() {
                            merchello.bazaar.checkout.refreshCustomerAddressViewSettings();
                        });
                    }
                }

                // update ship rate quotes
                if ($('#shipping-quotes-select').length > 0) {
                    $('#shipping-quotes-select').change(function () {
                        merchello.bazaar.checkout.updateShipRateQuote($('#customer-token').val(), $(this).val());
                        merchello.bazaar.checkout.setShipMethod();
                    });
                }

                if ($('#payment-method-select').length > 0) {
                    $('#payment-method-select').change(function () {
                        merchello.bazaar.checkout.setPaymentMethod();
                    });
                    merchello.bazaar.checkout.setPaymentMethod();
                }
                
            },
            validateShippingCountry: function(elem) {
                if (0 === $('#shipping-country-select option[value=' + $(elem).val() + ']').length && $('#billing-is-shipping:checked').length > 0) {
                    merchello.bazaar.checkout.toggleBillingIsShipping();
                    $('#billing-is-shipping').attr('checked', false);
                    $('#billing-is-shipping').attr('disabled', true);
                } else {
                    $('#billing-is-shipping').removeAttr('disabled');
                }
            },
            toggleBillingIsShipping: function () {
                if ($('#billing-is-shipping').is(':checked')) {
                    $('#shipping-address').hide();
                } else
                {
                    $('#shipping-address').show();
                }
                merchello.bazaar.checkout.refreshCustomerAddressViewSettings();
            },
            refreshCustomerAddressViewSettings: function () {
                // drop downs
                if ($('#shipping-address-select')) {
                    if ($('#billing-is-shipping').is(':checked')) {
                        if ($('#billing-address-select')) {
                            if ($('#billing-address-select').val() !== '00000000-0000-0000-0000-000000000000') {
                                $('#shipping-address-select option:nth(0)').text('Use a copy of \'' + $("#billing-address-select option:selected").text() + '\'');
                            } else {
                                $('#shipping-address-select option:nth(0)').text('Enter a new address');
                            }
                        }
                    } else {
                        $('#shipping-address-select option:nth(0)').text('Enter a new address');
                    }
                }
                // save info options
                if ($('#IsAnonymous').val().toLowerCase() === "false") {
                    // if either of the drop downs has an empty key we need to show one or both label
                    // boxes.
                    var showBillingLabel = false;
                    var showShippingLabel = false;
                    var showCheckbox = false;
                    if ($('#shipping-address-select').is(':visible')) {
                        if ($('#shipping-address-select').val() === '00000000-0000-0000-0000-000000000000') {
                            showShippingLabel = true;
                        }
                    } else {
                        showShippingLabel = true;
                    }
                    if ($('#billing-address-select').is(':visible')) {
                        if ($('#billing-address-select').val() === '00000000-0000-0000-0000-000000000000') {
                            showBillingLabel = true;
                        }
                    } else {
                        showBillingLabel = true;
                    }
                    showCheckbox = showBillingLabel || showShippingLabel;
                    if (showCheckbox) {
                        $('#save-addresses-options').show();
                        if ($('#save-addresses-check').is(':checked')) {
                            $('#customer-address-labels').show();
                            if (showBillingLabel) {
                                $('#billing-address-label-save').show();
                            } else {
                                $('#billing-address-label-save').hide();
                            }
                            if (showShippingLabel) {
                                if ($('#billing-is-shipping').is(':checked') && $('#billing-is-shipping').is(':visible') && ($('#billing-address-select').val() === '00000000-0000-0000-0000-000000000000') || $('#billing-address-select').length === 0) {
                                    $('#shipping-address-label-save').hide();
                                } else {
                                    $('#shipping-address-label-save').show();
                                }
                            } else {
                                $('#shipping-address-label-save').hide();
                            }
                        } else {
                            $('#customer-address-labels').hide();
                        }
                        
                    } else {
                        $('#save-addresses-options').hide();
                    }
                
                } else {
                    $('#save-addresses-options').hide();
                }
            },
            setProvinces: function (elem) {
                var countryCode = $(elem).val();
                var data = {};
                data.countryCode = countryCode;
                $.ajax({
                    type: "GET",
                    url: "/umbraco/Bazaar/BazaarSiteApi/GetProvincesForCountry",
                    data: data,
                    success: function (provinces) {
                        if ($(elem).attr('data-addresstype') === 'billing') {
                            
                            if (provinces.length > 0) {
                                $('#billing-province-select').show();
                                $('#billing-province-select').find('option').remove();
                                $.each(provinces, function(indexBilling, billingProvince) {
                                    $('#billing-province-select').append('<option value=\'' + billingProvince.code + '\'>' + billingProvince.name + '</option>')
                                });
                                $('#billing-province-entry').hide();
                            } else {
                                $('#billing-province-select').hide();
                                $('#billing-province-entry').show();
                            }
                        } else {

                            if (provinces.length > 0) {
                                $('#shipping-province-select').show();
                                $('#shipping-province-select').find('option').remove();
                                $.each(provinces, function (shippingBilling, shippingProvince) {
                                    $('#shipping-province-select').append('<option value=\'' + shippingProvince.code + '\'>' + shippingProvince.name + '</option>')
                                });
                                $('#shipping-province-entry').hide();
                            } else {
                                $('#shipping-province-select').hide();
                                $('#shipping-province-entry').show();
                            }
                        }
                    },
                    dataType: "json",
                    traditional: true
                }).fail(function (ex) {
                    $.error(ex);
                });
                
            },
            updateShipRateQuote: function (customerToken, methodKey) {
                var data = {};
                data.customerToken = customerToken;
                data.methodKey = methodKey;
                $.ajax({
                    type: "GET",
                    url: "/umbraco/Bazaar/BazaarSiteApi/UpdateShipRateQuote",
                    data: data,
                    success: function (summary) {
                        $('#shipping-total').text(summary.shippingTotal);
                        $('#tax-total').text(summary.taxTotal);
                        $('#invoice-total').text(summary.invoiceTotal);
                    },
                    dataType: "json",
                    traditional: true
                }).fail(function (ex) {
                    $.error(ex);
                });
            },
            copyBillingAddress: function (event) {
                if ($('#billing-is-shipping').is(':checked')) {
                    $('#shipping-name').val($('#billing-name').val());
                    $('#shipping-email').val($('#billing-email').val());
                    $('#shipping-address1').val($('#billing-address1').val());
                    $('#shipping-address2').val($('#billing-address2').val());
                    $('#shipping-locality').val($('#billing-locality').val());
                    $('#shipping-province-entry').val($('#billing-province-entry').val());
                    $('#shipping-province-select').val($('#billing-province-select').val());
                    $('#shipping-country-select').val($('#billing-country-select').val());
                    $('#shipping-postalcode').val($('#billing-postalcode').val());
                    $('#shipping-phone').val($('#billing-phone').val());
                }
            },
            setCustomerAddress : function(type, key) {
                var data = {};
                data.customerAddressKey = key;
                $.ajax({
                    type: "GET",
                    url: "/umbraco/Bazaar/BazaarSiteApi/GetCustomerAddress",
                    data: data,
                    success: function (adr) {
                        $('#' + type + '-name').val(adr.fullName);
                        $('#' + type + '-address1').val(adr.address1);
                        $('#' + type + '-address2').val(adr.address2);
                        $('#' + type + '-locality').val(adr.locality);
                        $('#' + type + '-province-entry').val(adr.region);
                        $('#' + type + '-province-select').val(adr.region);
                        $('#' + type + '-country-select').val(adr.countryCode);
                        $('#' + type + '-postalcode').val(adr.postalCode);
                        $('#' + type + '-phone').val(adr.phone);
                        if (type === 'billing') {
                            $('#shipping-address-label').val(adr.label);
                            $('#billing-address-label').val(adr.label);
                        }
                        
                        $('#' + type + '-vcard-fn').text(adr.fullName);
                        $('#' + type + '-vcard-address1').text(adr.address1);
                        $('#' + type + '-vcard-address2').text(adr.address2);
                        $('#' + type + '-vcard-locality').text(adr.locality);
                        $('#' + type + '-vcard-region').text(adr.region);
                        $('#' + type + '-vcard-country').text(adr.countryCode);
                        $('#' + type + '-vcard-postalcode').text(adr.postalCode);
                        if (type === 'billing') {
                            merchello.bazaar.checkout.copyBillingAddress();
                        }
                    },
                    dataType: "json",
                    traditional: true
                }).fail(function (ex) {
                    $.error(ex);
                });
            },
            setShipMethod: function() {
                $.each($('.selected-shipmethod-key'), function(idx, elem) {
                    $(elem).val($('#shipping-quotes-select').val());
                });
            },
            setPaymentMethod: function () {
                var forms = $('.payment-method-form');
                $.each(forms, function(index, frm) {
                    $(frm).hide();
                });
                var paymentMethodKey = $('#payment-method-select').val();

                var data = {};
                data.paymentMethodKey = paymentMethodKey;
                $.ajax({
                    type: "GET",
                    url: "/umbraco/Bazaar/BazaarSiteApi/GetPaymentMethodUi",
                    data: data,
                    success: function (info) {
                        $('#' + info.alias).show();
                        $('#' + info.alias).find('.selected-paymentmethod-key').val(paymentMethodKey);
                    },
                    dataType: "json",
                    traditional: true
                }).fail(function (ex) {
                    $.error(ex);
                });
            }
        }
    }

    merchello.bazaar.init();

}(window.merchello = window.merchello || {}));
