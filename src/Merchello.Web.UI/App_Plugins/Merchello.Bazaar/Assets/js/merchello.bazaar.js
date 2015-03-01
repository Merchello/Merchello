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
                    $('address-province-entry').text($(this).val());
                });
                $('address-province-entry').text($('#address-province-select').val());

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

                    var key = $(elem).children('.variant-pricing').attr('id');

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

        checkout : {
            bind: function () {
                if ($('#addresses-form')) {
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
                    if ($('#billing-is-shipping')) {
                        $('#billing-is-shipping').click(function () {
                            merchello.bazaar.checkout.toggleBillingIsShipping();
                        });
                    }

                    // update ship rate quotes
                    if ($('#shipping-quotes-select')) {
                        $('#shipping-quotes-select').change(function () {
                            merchello.bazaar.checkout.updateShipRateQuote($('#customer-token').val(), $(this).val());
                        });
                    }

                    // bind the customer address drop downs
                    if ($('#billing-address-select')) {
                        $('#billing-address-select').change(function() {
                            if ($(this).val() !== '') {
                                $('#billing-address').hide();
                                $('#billing-vcard').show();
                                merchello.bazaar.checkout.setCustomerAddress('billing', $(this).val(), $('#billing-vcard'));
                            } else {
                                $('#billing-address').show();
                                $('#billing-vcard').hide();
                            }
                            merchello.bazaar.checkout.refreshCustomerAddressViewSettings();
                        });
                    }
                    if ($('shipping-address-select')) {
                        $('#shipping-address-select').change(function() {
                            if ($(this).val() !== '') {
                                $('#billing-is-shipping-check').hide();
                                $('#shipping-address').hide();
                            } else {
                                $('#billing-is-shipping-check').show();
                                if (!$('#billing-is-shipping').is(':checked')) {
                                    $('#shipping-address').show();
                                }
                            }
                            merchello.bazaar.checkout.refreshCustomerAddressViewSettings();
                        });
                    }

                    if ($('#save-addresses-check')) {
                        $('#save-addresses-check').click(function() {
                            merchello.bazaar.checkout.refreshCustomerAddressViewSettings();
                        });
                    }
                }
            },
            validateShippingCountry: function(elem) {
                if (0 === $('#shipping-country-select option[value=' + $(elem).val() + ']').length && $('#billing-is-shipping:checked').length > 0) {
                    console.info('open it');
                    merchello.bazaar.checkout.toggleBillingIsShipping();
                    $('#billing-is-shipping').attr('checked', false);
                    $('#billing-is-shipping').attr('disabled', true);
                } else {
                    $('#billing-is-shipping').removeAttr('disabled');
                }
            },
            toggleBillingIsShipping: function () {
                $('#shipping-address').toggle($('#billing-is-shipping').checked);
                merchello.bazaar.checkout.refreshCustomerAddressViewSettings();
            },
            refreshCustomerAddressViewSettings: function () {
                // drop downs
                if ($('#shipping-address-select')) {
                    if ($('#billing-is-shipping').is(':checked')) {
                        if ($('#billing-address-select')) {
                            if ($('#billing-address-select').val() !== '') {
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
                if ($('#shipping-address-select') || $('#billing-address-select')) {
                    // if either of the drop downs has an empty key we need to show one or both label
                    // boxes.
                    var showBillingLabel = false;
                    var showShippingLabel = false;
                    var showCheckbox = false;
                    if ($('#shipping-address-select')) {
                        if ($('#shipping-address-select').val() === '') {
                            showShippingLabel = true;
                        }
                    }
                    if ($('#billing-address-select')) {
                        if ($('#billing-address-select').val() === '') {
                            showBillingLabel = true;
                        }
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
                                $('#shipping-address-label-save').show();
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
                        console.info(summary);
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
            copyBillingAddress: function(event) {
                if ($('#billing-is-shipping').attr('checked') === 'checked') {
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
            setCustomerAddress : function(type, key, vcard) {
                
            }
        }
    }

    merchello.bazaar.init();

}(window.merchello = window.merchello || {}));