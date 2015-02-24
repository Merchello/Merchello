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
            });
        },

        products: {
            bind: function () {
                $.each($('.add-to-cart.has-variants'), function(index, elem) {

                    var key = $(elem).children('.variant-pricing').attr('id');

                    // initialize the price
                    merchello.bazaar.products.getVariantPrice(elem, key);

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


            getVariantPrice: function (elem, key) {
                var options = "";
                $.each($(elem).find(".ProductVariants"), function (index, element) {
                    options = options + element.selectedOptions[0].value + ",";
                });

                var variantOptions = {};
                variantOptions.productKey = key;
                variantOptions.optionChoiceKeys = options;

                $.ajax({
                    type: "GET",
                    url: "/umbraco/Bazaar/BazaarSiteApi/GetProductVariantPrice",
                    data: variantOptions,
                    success: function(price) {
                        $("#" + key).text(price);
                    },
                    dataType: "json",
                    traditional: true
                }).fail(function(ex) {
                    $.error(ex);
                });
            },

            getUpdatedVariants: function(root, ddl, key) {

                var productAttributeKey = ddl.selectedOptions[0].value;

                var filter = {};
                filter.productKey = key;
                filter.productAttributeKey = productAttributeKey;

                $.ajax({
                    type: "GET",
                    url: "/umbraco/Bazaar/BazaarSiteApi/FilterOptionChoices",
                    data: filter,
                    success: function (filteredOptions) {
                        merchello.bazaar.products.updateOptionChoices(filteredOptions);
                        merchello.bazaar.products.getVariantPrice(root, key);
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
            copyBillingAddress: function(event) {
                console.info('will copy the address');
                event.preventDefault();
            }
        }
    }

    merchello.bazaar.init();

}(window.merchello = window.merchello || {}));