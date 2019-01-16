//// A class to deal with AddItem box JQuery functions
//// This looks for a form with data attribute "data-muifrm='additem'"
MUI.AddItem = {

    events : [
        { alias: 'added', name: 'AddItem.added' },
        { alias: 'tableCreated', name: 'AddItem.tableCreated' }
    ],

    addItemSuccess: 'Successfully added item to basket',

    postUrl: '',

    forms: [],

    // Initializes the AddItem object
    init: function() {

        if (MUI.Settings.Endpoints.basketSurface !== undefined && MUI.Settings.Endpoints.basketSurface !== '') {
            MUI.AddItem.postUrl = MUI.Settings.Endpoints.basketSurface + 'AddBasketItem';
        }

        // find all the product keys to load the data tables
        var containers = $('[data-muivalue="product"]');

        if (containers.length > 0) {
            $.each(containers, function(idx, c) {
                var key = '';
                key = $(c).data('muikey');
                if (key !== undefined && key !== '') {
                    // verify the key does not already exist
                    // this can happen if the product exists on the page more than once
                    var found = _.find(MUI.AddItem.bind.keys, function(k) { return k === key });
                    if (found === undefined) {
                        MUI.AddItem.bind.keys.push(key);
                    }
                }
            });
        }

        // keys are all loaded now get the data tables
        loadData();

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
                            MUI.emit('AddItem.tableCreated', pdt);
                            MUI.AddItem.dataTables.push(pdt);
                            //MUI.AddItem.bind.controls(pdt);
                        });
                    }


                    // find all of the AddItem forms
                    MUI.AddItem.bind.forms();

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

        forms: function() {
             var frms = $('[data-muifrm="additem"]');
             if (frms.length > 0) {
                MUI.AddItem.Forms = frms;
                $.each(frms, function(idx, frm) {
                    MUI.AddItem.bind.form(frm);
                });
             }
        },

        // Bind the form;
        form: function(frm) {

            // If the endpoint is set, override the form to do AJAX posts
            if (MUI.AddItem.postUrl !== '') {

                $(frm).submit(function () {
                    $.ajax({
                        type: 'POST',
                        url: MUI.AddItem.postUrl,
                        data: $(this).serialize()
                    }).then(function(result) {
                        MUI.emit('AddItem.added', result);
                        MUI.Notify.info(MUI.AddItem.addItemSuccess);
                    }, function(err) {
                       MUI.Logger.captureError(err); 
                    });
                    return false;
                });
            }

            var key = $(frm).closest('[data-muivalue="product"]').data('muikey');

            if (key !== undefined) {
                MUI.AddItem.bind.controls(frm, key);
            }
        },

        // Bind the form
        // This also requires the API promise to be completed.
        // parameter: pdt - Product Data Table object
        // TODO - this will need to change in 2.1.0
        // TODO - when we can designate which sort of form element to use for option choice selction
        controls: function(frm, key) {
            var options = MUI.AddItem.getOptionsForProduct(frm, key);

            if (options.length > 0) {
                $.each(options, function(idx, o) {
                   $(o).change(function() {
                       // TODO filter lists to ensure all choices are available
                       MUI.AddItem.updateVariantPricing(frm, key);
                       MUI.AddItem.ensureInventorySettings(frm, key);
                   });
                });
            }
            // initial pricing (on load)
            MUI.AddItem.updateVariantPricing(frm, key);
            MUI.AddItem.ensureInventorySettings(frm, key);
        }
    },


    // Product data tables
    // This is essentially a cache of product variant values so that we do not have to hit the API
    // on every option value change
    dataTables : [],

    // updates the variant pricing from the product data table data
    updateVariantPricing: function(frm, key) {
        // find the append point
        var well = $(frm).closest('[data-muivalue="product"]');
        if (well.length === 0) return;
        var appendTo = $(well).find('[data-muivalue="variantprice"]');
        if (appendTo.length > 0) {
            appendTo = appendTo[0];
        } else {
            // There is not reason to continue if the append point cannot be found
            return;
        }

        var row = MUI.AddItem.findProductDataTableRow(frm, key);

        if (row !== undefined) {

            // update the price
            var html = '';
            if (row.onSale) {
                html = "<span class='sale-price'>" + row.formattedSalePrice + "</span> <span class='original-price'>" + row.formattedPrice + "</span>";
            } else {
                html = "<span>" + row.formattedPrice + "</span>";
            }

            $(appendTo).html(html);
        }

    },

    ensureInventorySettings: function(frm, key) {
        var row = MUI.AddItem.findProductDataTableRow(frm, key);
        if (row !== undefined) {
            var appendTo = $(frm).find('[data-muiinv="' + row.productKey + '"]');
            if (appendTo.length > 0) {

                var showBtn = row.outOfStockPurchase || row.inventoryCount > 0;

                var html = '';
                if (row.inventoryCount > 0) {
                    html = MUI.Settings.Labels.InStock.replace('@0', row.inventoryCount);
                } else {

                    if (row.outOfStockPurchase) {
                        html = MUI.Settings.Labels.OutOfStockAllowPurchase;
                    } else {
                        html = MUI.Settings.Labels.OutOfStock;
                    }
                    html = "<span>" + html + "</span>";
                }

                $(appendTo).html(html);
                var btn = $(frm).find(':submit');

                if (showBtn) {
                    $(btn).show();
                } else {
                    $(btn).hide();
                }
            }
        }
    },

    getOptionsForProduct: function(frm, key) {
        var container = $('[data-muikey="' + key + '"]');
        if (container.length > 0) {
            var parents = $(frm).find('[data-muioption]');
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

    findProductDataTableRow: function(frm, key) {
        // get all of the options associated with this product so we can
        // find the matching data row in the product data table
        var options = MUI.AddItem.getOptionsForProduct(frm, key);

        var row = undefined;
        if (options.length > 0) {
            // get the current selections
            var keys = [];
            $.each(options, function (idx, o) {
                keys.push($(o).val());
            });

            var pdt = MUI.AddItem.getProductDataTable(key);
            if (pdt !== undefined) {
                if (keys.length > 0) {
                    row = pdt.getRowByMatchKeys(keys);
                }
            }
        }

        return row;
    },

    // Gets the data table
    // uses underscore
    getProductDataTable: function (pkey) {
        return _.find(MUI.AddItem.dataTables, function (t) { return t.productKey === pkey; });
    }
};
