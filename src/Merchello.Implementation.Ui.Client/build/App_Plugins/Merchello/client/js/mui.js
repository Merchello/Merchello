/*! MUI
 * https://merchello.com
 * Copyright (c) 2016 Accross the Pond, LLC.
 * Licensed 
 */


// Generic Merchello UI scripts.
// Requires JQuery version 1.10.2 or higher
//          JQuery unobrusive
//          underscore.js
var MUI = (function() {

    // If DEBUG_MODE is true allows messages to be written to the console
    // THESE SHOULD be set to false before deploying to production!!!!!
    var DEBUG_MODE = {
        events: false,
        console: true
    };

    // Private members
    var eventHandlers = [];

    // Initialization
    function init() {
        $(document).ready(function() {
            // initialize the logger module
            MUI.Logger.init();
            // initialize the add item module
            MUI.AddItem.init();
            // initialize the basket module
            MUI.Basket.init();
            // initialize the labels
            MUI.Labels.init();
        });
    }
    
    function hasLogger() {
        return MUI.Logger.hasLogger;
    }

    //// Registers an MUI event
    function registerEvent(evt, callback) {
        try
        {
            var existing = _.find(eventHandlers, function(ev) { return ev.event === evt; });
            if (existing !== undefined && existing !== null) {
                if (callback !== undefined && typeof callback === "function") {
                    existing.callbacks.push(callback);
                }
            } else {
                var callbacks = [];
                if (callback !== undefined && typeof callback === "function") {
                    callbacks.push(callback);
                }
                eventHandlers.push({ event: evt, callbacks: callbacks });
            }
        }
        catch(err) {
            MUI.Logger.captureError(err);
        }
    }

    /// emit the event
    function trigger(name, args) {
        var existing = _.find(eventHandlers, function(ev) { return ev.event === name; });
        if (existing === undefined || existing === null) return;

        // execute each of the registered callbacks
        _.each(existing.callbacks, function(cb) {
            try {
                cb(name, args);
            }
            catch(err) {
                MUI.Logger.captureError(err, {
                    extra: {
                        eventName: name,
                        args: args
                    }
                });
            }
        });
    }

    // create a generic cache of functions, where fn is the function to retrieve and execute for a value.
    // also ensures, the function is executed once and a single value is returned.
    function createCache(fn) {
        var cache = {};
        return function( key, callback ) {
            if ( !cache[ key ] ) {
                cache[ key ] = $.Deferred(function( defer ) {
                    fn( defer, key );
                }).promise();
            }
            return cache[ key ].done( callback );
        };
    }

    // utility method to map the event name from an alias so they can be
    // more easily referenced in other modules
    function getEventNameByAlias(events, alias) {
        var evt = _.find(events, function(e) { return e.alias === alias});
        return evt === undefined ? '' : evt.name;
    }

    // writes events to debug console
    function debugConsoleEvents(events) {
        if (DEBUG_MODE.events) {
            _.each(events, function(ev) {
                MUI.on(ev.name, function(name, args) {
                    console.info(ev);
                    console.info(args === undefined ? 'No args' : args);
                });
            });
        }
    }

    // write to debug console if in debug mode
    function debugConsole(obj) {
        if (DEBUG_MODE.console) {
            console.info(obj);
        }
    }

    // exposed members
    return {
        init: init,
        hasLogger: hasLogger,
        createCache: createCache,
        on: registerEvent,
        emit: trigger,
        getEventNameByAlias: getEventNameByAlias,
        debugConsoleEvents : debugConsoleEvents,
        debugConsole: debugConsole
    }

})();

//// A class to deal with AddItem box JQuery functions
//// This looks for a form with data attribute "data-muifrm='additem'"
MUI.AddItem = {

    events : [
        { alias: 'added', name: 'AddItem.added' }
    ],

    postUrl: '',

    // Initializes the AddItem object
    init: function() {

        if (MUI.Settings.basketSurfaceEndpoint !== '') {
            MUI.AddItem.postUrl = MUI.Settings.basketSurfaceEndpoint + 'AddBasketItem';
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
            if (MUI.Settings.productTableApiEndpoint === '') return;
            if (MUI.AddItem.bind.keys.length > 0) {
                var url = MUI.Settings.productTableApiEndpoint + 'PostGetProductDataTables';

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

                        // TODO Add some sort of success message (notification panel?)

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

//// A class to deal with basket JQuery functions
//// This looks for a form with data attribute "data-muifrm='basket'"
MUI.Basket = {

    // initialize the basket
    init: function() {
        if (MUI.Settings === undefined) return;
        if (MUI.Settings.basketSurfaceEndpoint === '') return;

        var frm = $('[data-muifrm="basket"]');
        if (frm.length > 0) {
            MUI.Basket.bind.form(frm[0]);
        }
    },

    // binds the form
    bind: {
        form: function(frm) {

            // Watch for changes in the input fields
            $(frm).find(':input').change(function() {
                var frmRef = $(this).closest('form');

                // post the form to update the basket quantities
                var url = MUI.Settings.basketSurfaceEndpoint + 'UpdateBasket';
                $.ajax({
                    type: 'POST',
                    url: url,
                    data: $(frmRef).serialize()
                }).then(function(results) {

                    // update the line items sub totals
                    $.each(results.UpdatedItems, function(idx, item) {
                        var hid = $('input:hidden[value="' + item.Key + '"]')
                        if (hid.length > 0) {
                            var subtotal = $(hid).closest('tr').find('[data-muivalue="linetotal"]');
                            if (subtotal.length > 0) {
                                $(subtotal).html(item.FormattedTotal);
                            }
                        }
                    });

                    // set the new basket total
                    var total = $(frmRef).find('[data-muivalue="total"]');
                    if (total.length > 0) {
                        $(total).html(results.FormattedTotal);
                    }

                    // Emit the event so that labels can update if handled
                    MUI.emit('AddItem.added', results);

                }, function(err) {
                   MUI.Logger.captureError(err);
                });
            });


            var btn = $(frm).find('[data-muibtn="update"]');
            if (btn.length > 0) {
                $(btn).hide();
            }

        }
    }
};

MUI.Forms = {
    init: function() {

        // add the custom validator for MVC unobtrusive validation
        $.validator.addMethod('validateexpiresdate', function(value, element, params) {

            // Make sure the value has a length of 5
            if (value.length != 5) {
                return false;
            }

            var today = new Date();
            var thisYear = today.getFullYear() - 2000;
            var expMonth = +value.substr(0, 2);
            var expYear = +value.substr(3, 4);

            return (expMonth >= 1
            && expMonth <= 12
            && (expYear >= thisYear && expYear < thisYear + 20)
            && (expYear == thisYear ? expMonth >= (today.getMonth() + 1) : true));

        });

        $.validator.unobtrusive.adapters.addBool('validateexpiresdate')

    },

    // Post a form and return the promise
    post: function(frm, url) {
        return MUI.resourcePromise(url, $(frm).serialize());
    },
    
    // rebinds a form unobtrusive validation
    rebind: function(frm) {
        $.validator.unobtrusive.parse(frm);
    },


    // validates the form
    validate: function(frm) {

        // obtain validator
        var validator = $(frm).validate();

        var isValid = true;
        $(frm).find("input").each(function () {

            // validate every input element inside this step
            if (!validator.element(this)) {
                isValid = false;
            }
        });

        return isValid;
    },

    // tests a string to see if it is in a valid email format
    isValidEmail: function(email) {
        var re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        return re.test(email);
    }
};

//// A class to assist in updating labels in the UI
//// This looks for a form with data attribute "data-muilabel='VALUE'"
MUI.Labels = {
    
    init: function() {
        
        // Event listener
        MUI.on('AddItem.added', MUI.Labels.update.basketItemCount);
        
    },
    
    update: {
        
        basketItemCount: function(evt, args) {
            if (args.Success) {
                var label = $('[data-muilabel="basketcount"]');
                if (label.length > 0) {
                    $(label).html(args.BasketItemCount);
                }
            }
        }
        
    }
};

//// The logger interface
// If you have a remove logger you can wire it in here.
MUI.Logger = {

    hasLogger: false,
    
    // Initializes the Sentry (Raven) logger
    init: function() {
        return;
    },

    // Sets the module context
    setSiteContext: function(siteAlias) {
        return;
    },

    setUserContext: function(email) {
        return;
    },

    // Captures an error
    captureError: function(e, args) {
        var consoleLog = args === undefined ? e : { error: e, args: args };
        MUI.debugConsole(consoleLog);
        return;
    },

    captureMessage: function(msg, args) {
        var consoleLog = args === undefined ? message : { message: msg, args: args };
        MUI.debugConsole(consoleLog);
        return;
    },

    isReady: function() {
        return MUI.Logger.hasLogger;
        // this can be used to test a logger is available ex. getsentry.com -> Raven.isSetup();
    }
};

//// Cart model
MUI.AddItem.ProductDataTable = function() {
    var self = this;
    self.productKey = '';
    self.rows = [];
};

/// Cart Model Prototypes
//  ProductDataTable
MUI.AddItem.ProductDataTable.prototype = (function() {

    // used to match a pricing row with the drop down selection
    function getRowByMatchKeys(keys) {
        var row = _.find(this.rows, function (r) {
            var test = _.intersection(r.matchKeys, keys);
            return test.length === keys.length;
        });
        return row;
    }

    return {
        getRowByMatchKeys: getRowByMatchKeys
    };

}());

MUI.AddItem.ProductDataTableRow = function() {
    var self = this;
    self.productKey = '',
    self.productVariantKey = '',
    self.sku = '';
    self.isForVariant = false;
    self.onSale = false;
    self.salePrice = 0;
    self.salePriceIncVat = 0;
    self.price = 0;
    self.priceIncVat = 0;
    self.isAvailable = true;
    self.matchKeys = [];
    self.inventoryCount = 0;
    self.outOfStockPurchase = false;
};

// put prototype methods there

// initialize the MUI object
MUI.init();


