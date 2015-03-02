// merch-functions.js | requires jQuery 1.10.1+, masonry.js, imagesloaded.pkgd.js | created by mindfly
// Created on May 28, 2014 | last updated May 28, 2014

var merchello = {

    flag: [],
	init: function() {
		$(document).ready(function() {

		    merchello.Products.init();
		});
	},
			
	Products: {
	    init: function () {
	        merchello.Products.getVariantPrice();
	        if ($('.ProductVariants').length > 0) {
	            merchello.Products.getUpdatedVariants($('.ProductVariants')[0]);
	        }
	    },

	    updateAddToCartVariants: function (variants, changedElement) {
	        var allVariants = [];

	        $.each(variants, function (index, variant) {
	            $.each(variant.attributes, function (i, el) {
	                if (el.optionKey != changedElement.id) {
	                    allVariants.push(el.key);
	                }
	            });
	        });

	        var uniqueVariants = $.unique(allVariants);

	        $.each($(".ProductVariants"), function (i, productVariant) {
	            $.each(productVariant.options, function (j, option) {
	                if (productVariant.id != changedElement.id) {
	                    if ($.inArray(option.value, uniqueVariants) >= 0) {
	                        $(option).show();
	                    }
	                    else {
	                        $(option).hide();
	                    }
	                }
	            });
	        });
	    },
        getVariantPrice: function () {
            var options = "";
            $.each($(".ProductVariants"), function(index, element) {
                options = options + element.selectedOptions[0].value + ",";
            });

            var variantOptions = {};
            variantOptions.productKey = $("#ProductKey")[0].value;
            variantOptions.optionChoiceKeys = options;
            
            $.ajax({
                type: "GET",
                url: "/umbraco/RosettaStone/SiteApi/GetProductVariantPrice",
                data: variantOptions,
                success: function(price) {
                    $("#productPrice").text(price);
                },
                dataType: "json",
                traditional: true
            }).fail(function (ex) {
                $.error(ex);
            });
        },
        getUpdatedVariants: function (variant) {

            var options = variant.selectedOptions[0].value;

            var variantOptions = {};
            variantOptions.productKey = $("#ProductKey")[0].value;
            variantOptions.optionChoices = options;

            $.ajax({
                type: "GET",
                url: "/umbraco/RosettaStone/SiteApi/FilterOptionsBySelectedChoices",
                data: variantOptions,
                success: function(variants){
                    merchello.Products.updateAddToCartVariants(variants, variant);
                    merchello.Products.getVariantPrice();
                },
                dataType: "json",
                traditional: true
            }).fail(function (ex) {
                $.error(ex);
            });
        }
    }
};
merchello.init();