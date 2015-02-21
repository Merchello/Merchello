// merchello.bazaar.js | requires jQuery 1.10.1+

(function(merchello, undefined) {

    merchello.bazaar = {
        init: function() {
            $(document).ready(function() {
                merchello.bazaar.products.bind();
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
                        $.each($(variants), function (idx, ddl) {
                            $(ddl).change(function () {
                                merchello.bazaar.products.getUpdatedVariants(elem, ddl, key);
                            });
                        });
                    }
                   
                });
            },

            updateAddToCartVariants: function(variants, changedElement) {
                var allVariants = [];

                $.each(variants, function(index, variant) {
                    $.each(variant.attributes, function(i, el) {
                        if (el.optionKey != changedElement.id) {
                            allVariants.push(el.key);
                        }
                    });
                });

                var uniqueVariants = $.unique(allVariants);

                $.each($(".ProductVariants"), function(i, productVariant) {
                    $.each(productVariant.options, function(j, option) {
                        if (productVariant.id != changedElement.id) {
                            if ($.inArray(option.value, uniqueVariants) >= 0) {
                                $(option).show();
                            } else {
                                $(option).hide();
                            }
                        }
                    });
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

            getUpdatedVariants: function(root, variant, key) {

                var options = variant.selectedOptions[0].value;

                var variantOptions = {};
                variantOptions.productKey = key;
                variantOptions.optionChoices = options;

                $.ajax({
                    type: "GET",
                    url: "/umbraco/Bazaar/BazaarSiteApi/FilterOptionsBySelectedChoices",
                    data: variantOptions,
                    success: function(variants) {
                        merchello.bazaar.products.updateAddToCartVariants(variants, variant);
                        merchello.bazaar.products.getVariantPrice(root, key);
                    },
                    dataType: "json",
                    traditional: true
                }).fail(function(ex) {
                    $.error(ex);
                });
            }
        }
    }

    merchello.bazaar.init();

}(window.merchello = window.merchello || {}));