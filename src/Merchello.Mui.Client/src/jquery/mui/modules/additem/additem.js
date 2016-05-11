//// A class to deal with AddItem box JQuery functions
//// This looks for a form with data attribute "data-muifrm='additem'"
MUI.AddItem = {

    events : [
        { alias: 'added', name: 'AddItem.added' }
    ],

    addItemSuccess: 'Successfully added item to basket',

    postUrl: '',

    // Initializes the AddItem object
    init: function() {

        if (MUI.Settings.Endpoints.basketSurface !== undefined && MUI.Settings.Endpoints.basketSurface !== '') {
            MUI.AddItem.postUrl = MUI.Settings.Endpoints.basketSurface + 'AddBasketItem';
        }

        // find all of the AddItem forms
        var frms = $('[data-muifrm="additem"]');
        if (frms.length > 0) {
            $.each(frms, function(idx, frm) {
               MUI.AddItem.bind.form(frm);
            });
            loadData();
        }

        // loads the product data tables after the keys have been acquired
        function loadData() {
            if (MUI.Settings.Endpoints.productTableApi !== undefined && MUI.Settings.Endpoints.productTableApi === '') return;
            if (MUI.AddItem.bind.keys.length > 0) {
                var url = MUI.Settings.Endpoints.productTableApi + 'PostGetProductDataTables';

                $.ajax({
                    type: 'POST',
                    url: url,
                    dataType: 'json',
                    contentType: "application/json; charset=utf-8",
                    data: JSON.stringify(MUI.AddItem.bind.keys)
                }).then(function (tables) {

                    // build the product data tables as JS objects so we can use prototypes
                    if (tables.length > 0) {
                        $.each(tables, function(tindx, tbl) {
                           var pdt = new MUI.AddItem.ProductDataTable();
                            pdt.productKey = tbl.productKey;
                            $.each(tbl.rows, function(rindx, dataRow) {
                                pdt.rows.push($.extend(new MUI.AddItem.ProductDataTableRow(), dataRow));
                            });
                            MUI.AddItem.dataTables.push(pdt);
                            MUI.AddItem.bind.controls(pdt);
                        });
                    }

                }, function (err) {
                    MUI.Notify.error(err);
                    MUI.Logger.captureError(err);
                });
            }
        }

        // Debug
        MUI.debugConsoleEvents(MUI.AddItem.events);
    },

    bind: {

        // An array of product keys that should be bound
        keys : [],

        // Bind the form;
        form: function(frm) {
            var container = $(frm).closest('[data-muiproduct]');
            var key = '';
            if (container.length > 0) {
                key = $(container).data('muiproduct');
                if (key !== undefined && key !== '') {
                    MUI.AddItem.bind.keys.push(key);
                }
            }

            // If the endpoint is set, override the form to do AJAX posts
            if (MUI.AddItem.postUrl !== '') {
                $(frm).submit(function () {
                    $.ajax({
                        type: 'POST',
                        url: MUI.AddItem.postUrl,
                        data: $(this).serialize()
                    }).then(function(result) {

                        MUI.emit('AddItem.added', result);
                        
                        MUI.Notify.info('Successfully added item to basket');

                    }, function(err) {
                       MUI.Logger.captureError(err); 
                    });
                    return false;
                });
            }
        },

        // Bind the form
        // This also requires the API promise to be completed.
        // parameter: pdt - Product Data Table object
        // TODO - this will need to change in 2.1.0
        // TODO - when we can designate which sort of form element to use for option choice selction
        controls: function(pdt) {

            var options = MUI.AddItem.getOptionsForProduct(pdt.productKey);
            if (options.length > 0) {
                $.each(options, function(idx, o) {
                   $(o).change(function() {
                       // TODO filter lists to ensure all choices are available
                       MUI.AddItem.updateVariantPricing(pdt);
                   });
                });

                // initial pricing (on load)
                MUI.AddItem.updateVariantPricing(pdt);
            }
        }
    },


    // Product data tables
    // This is essentially a cache of product variant values so that we do not have to hit the API
    // on every option value change
    dataTables : [],

    // updates the variant pricing from the product data table data
    updateVariantPricing: function(pdt) {
        // find the append point

        var appendTo = undefined;
        var container = $('[data-muiproduct="' + pdt.productKey + '"]');
        if (container.length > 0) {
            appendTo = $(container).find('[data-muivalue="variantprice"]');
            if (appendTo.length > 0) {
                appendTo = appendTo[0];
            } else {
                // There is not reason to continue if the append point cannot be found
                return;
            }
        }

        // get all of the options associated with this product so we can
        // find the matching data row in the product data table
        var options = MUI.AddItem.getOptionsForProduct(pdt.productKey);
        if (options.length > 0) {
            // get the current selections
            var keys = [];
            $.each(options, function(idx, o) {
               keys.push($(o).val());
            });

            if (keys.length > 0) {
                var row = pdt.getRowByMatchKeys(keys);
                if (row !== undefined) {
                    // update the price
                    var html = '';
                    if (row.onSale) {
                        html = "<span class='sale-price'>" + row.formattedSalePrice + "</span><span class='original-price'>" + row.formattedPrice + "</span>";
                    } else {
                        html = "<span>" + row.formattedPrice + "</span>";
                    }

                    $(appendTo).html(html);
                }
            }
        }
    },

    getOptionsForProduct: function(key) {
        var container = $('[data-muiproduct="' + key + '"]');
        if (container.length > 0) {
            var parents = $(container).find('[data-muioption]');
            var options = [];
            $.each(parents, function(idx, p) {
                // find the select
                options.push($(p).find('select'));
            });
            return options;
        } else {
            return [];
        }
    },

    // Gets the data table
    // uses underscore
    getProductDataTable: function (pkey) {
        return _.find(MUI.AddItem.dataTables, function (t) { return t.productKey === pkey; });
    }
};
