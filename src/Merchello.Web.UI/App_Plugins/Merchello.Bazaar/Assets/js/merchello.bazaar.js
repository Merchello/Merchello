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
                
                $('#btn-profile-open').click(function() {
                    $('#profile-form').show();
                    $('#address-view').hide();
                    
                });
                $('#btn-profile-form-cancel').click(function() {
                    merchello.bazaar.account.resetViews();
                });
                $('#chk-change-password').click(function () {
                    console.info('clicked');
                    if (this.checked) {
                        $('#change-password-form').show();
                    } else {
                        $('#change-password-form').hide();
                    }
                });
            },
            toggleProfileForm: function() {
                $('#btn-profile-open').attr('disabled', 'disabled');
            },
            resetViews: function () {
                if (window.location.hash === '#success') {
                    $('#message').show();
                    $('#message').delay(2000).fadeOut(1000);
                } else {
                    $('#message').hide();
                }
                $('#btn-profile-open').removeAttr('disabled');
                $('#address-view').show();
                $('#btn-add-address-billing').removeAttr('disabled');
                $('#btn-add-address-shipping').removeAttr('disabled');
                $('#address-form').hide();
                $('#profile-form').hide();
                $('#btn-profile-form').attr('disabled', 'disabled');
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
                        merchello.bazaar.checkout.toggleBillingIsShipping();
                    }

                    // update ship rate quotes
                    if ($('#shipping-quotes-select')) {
                        $('#shipping-quotes-select').change(function () {
                            console.info('got here');
                            merchello.bazaar.checkout.updateShipRateQuote($('#customer-token').val(), $(this).val());
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
            }
        }
    }

    merchello.bazaar.init();

}(window.merchello = window.merchello || {}));