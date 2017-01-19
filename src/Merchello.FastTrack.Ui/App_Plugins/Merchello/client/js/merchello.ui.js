/*! MUI
 * https://merchello.com
 * Copyright (c) 2017 Across the Pond, LLC.
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
        console: false
    };

    // Private members
    var eventHandlers = [];

    // Initialization
    function init() {
        $(document).ready(function() {
            // initialize the logger module
            MUI.Logger.init();
            // intialize the notifications
            MUI.Notify.init()
            // initialize the forms
            MUI.Forms.init();
            // initialize the add item module
            MUI.AddItem.init();
            // initialize the basket module
            MUI.Basket.init();
            // initialize the wish list module
            MUI.WishList.init();
            // initialize the checkout module
            MUI.Checkout.init();
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
        // ensures the settings object is created
        Settings: {
            Notifications: {},
            Endpoints: {}
        },
        // ensures the services object is created
        Services: {},
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

//// The logger
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

MUI.Services.Braintree = {

    initialized: false,

    acceptCards: [],
    
    currentCard: {},

    events : [
        { attempt: 'unbindValidation', name: 'Braintree.UnbindValidation' },
        { attempt: 'cardTypeChanged',  name: 'Braintree.CardTypeChange' },
        { attempt: 'verified', name: 'Braintree.CardVerified' }
    ],

    // Load the required assets
    loadAssets: function(callback) {
        // load the braintree script and validation for cc with a promise
        // this also asserts if the customer goes back and changes the method to another
        // braintree method, that these are only loaded once
        $.when(
            MUI.Assets.cachedGetScript('/App_Plugins/Merchello/client/lib/card-validator.min.js'), // Verify we don't need this anymore
            MUI.Assets.cachedGetScript('//js.braintreegateway.com/web/3.6.3/js/client.min.js'),
            MUI.Assets.cachedGetScript('//js.braintreegateway.com/web/3.6.3/js/hosted-fields.min.js'),
            MUI.Assets.cachedGetScript('//js.braintreegateway.com/web/3.6.3/js/paypal.min.js')
            // MUI.Assets.cachedGetScript('//js.braintreegateway.com/v2/braintree.js')
        ).then(function() {
                MUI.Services.Braintree.initialized = true;
                if (callback !== undefined) callback();
            },
            function(err) { MUI.Logger.captureError(err); });

    }
};

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
                        MUI.Notify.info('Successfully added item to basket');
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

MUI.Assets = {

    // gets a script ensuring it is only ever loaded once.
    // currently used in loading Braintree scripts
    // https://learn.jquery.com/code-organization/deferreds/examples/
    cachedGetScript: MUI.createCache(function(defer, url) {
        $.getScript( url ).then( defer.resolve, defer.reject );
    })
    
};

//// A class to deal with basket JQuery functions
//// This looks for a form with data attribute "data-muifrm='basket'"
MUI.Basket = {

    // initialize the basket
    init: function() {
        if (MUI.Settings.Endpoints.basketSurface === undefined || MUI.Settings.Endpoints.basketSurface === '') return;
        
        var frm = $('[data-muifrm="basket"]');
        if (frm.length > 0) {
            MUI.Basket.bind.form(frm[0]);
        }
    },

    // binds the form
    bind: {
        form: function(frm) {

            // Watch for changes in the input fields
            $(frm).find(':input[data-muiaction="updatequantity"]').change(function() {
                var frmRef = $(this).closest('form');

                // post the form to update the basket quantities
                var url = MUI.Settings.Endpoints.basketSurface + 'UpdateBasket';
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

MUI.Checkout = {

    Payment: {},

    init: function() {
        // Initialize the address module
        MUI.Checkout.Address.init();
        // Initialize the payment module
        MUI.Checkout.Payment.Braintree.init();
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

        $.validator.unobtrusive.adapters.addBool('validateexpiresdate');
        
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
                    $(label).html(args.ItemCount);
                }
            }
        }
    }
};

MUI.Notify = {

    types: [
        'success',
        'info',
        'error',
        'warn'
    ],

    // Value to check to see if notifications are enabled and the bar has been appended to the page
    enabled: false,
    
    bar: undefined,

    overlay: undefined,

    hasOverlay: false,

    // initializes the Notify class
    init: function() {
        MUI.Notify.appendNotifyBar();
    },

    toggleOverlay: function() {
      if (MUI.Notify.hasOverlay) {
          var overlay = MUI.Notify.overlay;
          $(overlay).toggle();
      }
    },


    // renders an info message
    info: function(msg) {
      MUI.Notify.message(msg, 'info', 1000);
    },

    // renders a success message
    success: function(msg) {
      MUI.Notify.message(msg, 'success', 500);
    },

    // renders an error message
    error: function(msg) {
        MUI.Notify.message(msg, 'error', 1000);
    },

    // renders a warning message
    warn: function(msg) {
        MUI.Notify.message(msg, 'warn', 750);
    },

    // renders a message
    message: function(msg, type, delay) {
        if (MUI.Notify.bar !== undefined && MUI.Notify.enabled) {

            if(delay === undefined) delay = 500;

            var bar = MUI.Notify.bar;
            var container = $(bar).find('[data-muivalue="nofity"]');
            if (container.length > 0) {

                // removes previous class from notify bar
                reset();
                // get the current css class
                if (type === undefined) type = 'info';
                var ref = MUI.Notify.getClassRef(type);
                var css = MUI.Notify.getCssClass(ref);
                $(bar).addClass(css);
                $(container).html(msg);
                $(bar).fadeIn().delay(delay).fadeOut();
            }
        }

        // removes all the css classes
        function reset() {
            var bar = MUI.Notify.bar;
            var refs = MUI.Notify.getClassRef('all');
            $.each(refs, function(ref) {
                var css = MUI.Notify.getCssClass(ref);
                $(bar).removeClass(css);
            });
        }
    },


    getCssClass: function(ref) {
        try {
            var css = MUI.Settings.Notifications[ref];
            if (css === undefined) {
                MUI.Logger.captureMessage('Failed to find CSS class for ' + ref + '. Returning alert-info');
                return 'alert-info';
            } else {
                return css;
            }
        } catch(err) {
            MUI.Logger.captureError(err);
        }
    },

    getClassRef: function (type) {
        if (type === 'all') {
            var refs = [];
            $.each(MUI.Notify.types, function(t) {
                refs.push(t + 'Css');
            });
            return refs;
        } else {
            var found = _.find(MUI.Notify.types, function(t) { return t === type });
            return found === undefined ? 'infoCss' : type + 'Css';
        }
    },

    // Appends the notify bar to the bottom of the current page
    appendNotifyBar: function() {
        if (MUI.Settings.Notifications.enabled !== undefined &&
            MUI.Settings.Notifications.enabled === true &&
            MUI.Settings.Notifications.template !== undefined &&
            MUI.Settings.Notifications.template !== '') {

            // ensure not exists
            if($('[data-muinotify="notifybar"]').length === 0)
            {
                var div = MUI.Settings.Notifications.template;

                $('body').append(div);
                MUI.Notify.bar = $('[data-muinotify="notifybar"]');
                $(MUI.Notify.bar).hide();
                MUI.Notify.enabled = true;
            }
        }


        if (MUI.Settings.Notifications.enabled !== undefined &&
            MUI.Settings.Notifications.enabled === true &&
            MUI.Settings.Notifications.overlay !== undefined &&
            MUI.Settings.Notifications.overlay !== '') {

            if($('[data-muinotify="overaly"]').length == 0) {
                var div = MUI.Settings.Notifications.overlay;

                $('body').append(div);
                MUI.Notify.overlay = $('[data-muinotify="overlay"]');
                $(MUI.Notify.overlay).hide();
                MUI.Notify.hasOverlay = true;
            }
        }
    }
};

MUI.Utilities = {
    // Ensures a null or undefined value has either a value or a default value
    // defaultValue itself defaults to an empty string.
    EnsureNullAsValue: function(value, defaultValue) {
        if (defaultValue === undefined) {
            defaultValue = '';
        }

        return value === null || value === undefined ? defaultValue : value;
    },

    // Gets a query string parameter value
    // http://stackoverflow.com/questions/901115/how-can-i-get-query-string-values-in-javascript
    getQueryStringValue: function(name, url) {
        if (!url) url = window.location.href;
        url = url.toLowerCase(); // This is just to avoid case sensitiveness
        name = name.replace(/[\[\]]/g, "\\$&").toLowerCase();// This is just to avoid case sensitiveness for query parameter name
        var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
            results = regex.exec(url);
        if (!results) return undefined;
        if (!results[2]) return '';
        return decodeURIComponent(results[2].replace(/\+/g, " "));
    },


    // Credit card types
    // Adapted from https://github.com/braintree/credit-card-type
    CardTypes : {

        types: [
            {
                niceType: 'Visa',
                type: 'visa',
                pattern: '^4\\d*$',
                gaps: [4, 8, 12],
                lengths: [16],
                code: {
                    name: 'CVV',
                    size: 3
                }
            },
            {
                niceType: 'MasterCard',
                type: 'master-card',
                pattern: '^5[1-5]\\d*$',
                gaps: [4, 8, 12],
                lengths: [16],
                code: {
                    name: 'CVC',
                    size: 3
                }
            },
            {
                niceType: 'American Express',
                type: 'american-express',
                pattern: '^3[47]\\d*$',
                isAmex: true,
                gaps: [4, 10],
                lengths: [15],
                code: {
                    name: 'CID',
                    size: 4
                }
            },
            {
                niceType: 'DinersClub',
                type: 'diners-club',
                pattern: '^3(0[0-5]|[689])\\d*$',
                gaps: [4, 10],
                lengths: [14],
                code: {
                    name: 'CVV',
                    size: 3
                }
            },
            {
                niceType: 'Discover',
                type: 'discover',
                pattern: '^6(011|5|4[4-9])\\d*$',
                gaps: [4, 8, 12],
                lengths: [16],
                code: {
                    name: 'CID',
                    size: 3
                }
            },
            {
                niceType: 'JCB',
                type: 'jcb',
                pattern: '^(2131|1800|35)\\d*$',
                gaps: [4, 8, 12],
                lengths: [16],
                code: {
                    name: 'CVV',
                    size: 3
                }
            },
            {
                niceType: 'UnionPay',
                type: 'unionpay',
                pattern: '^62\\d*$',
                gaps: [4, 8, 12],
                lengths: [16, 17, 18, 19],
                code: {
                    name: 'CVN',
                    size: 3
                }
            },
            {
                niceType: 'Maestro',
                type: 'maestro',
                pattern: '^(50|5[6-9]|6)\\d*$',
                gaps: [4, 8, 12],
                lengths: [12, 13, 14, 15, 16, 17, 18, 19],
                code: {
                    name: 'CVC',
                    size: 3
                }
            }
        ],

        getCardType: function (cardNumber) {
            var key, value;
            var noMatch = {};

            if (!isString(cardNumber)) {
                return noMatch;
            }

            for (key in types) {
                if (!MUI.Utilities.CardTypes.hasOwnProperty(key)) {
                    continue;
                }

                value = MUI.Utilities.CardTypes.types[key];

                if (RegExp(value.pattern).test(cardNumber)) {
                    return clone(value);
                }
            }

            return noMatch;
        },

        getCardByType: function (type) {
            return _.find(MUI.Utilities.CardTypes.types, function (t) {
                return t.type === type;
            });
        }
    }
};

//// A class to deal with basket JQuery functions
//// This looks for a form with data attribute "data-muifrm='wishlist'"
MUI.WishList = {
    
    init: function() {
        if (MUI.Settings.Endpoints.wishListSurface === undefined || MUI.Settings.Endpoints.wishListSurface === '') return;

        var frm = $('[data-muifrm="wishlist"]');
        if (frm.length > 0) {
            MUI.WishList.bind.form(frm[0]);
        }
    },

    bind: {

        form: function(frm) {
            // Watch for changes in the input fields
            $(frm).find(':input[data-muiaction="updatequantity"]').change(function() {
                var frmRef = $(this).closest('form');

                // post the form to update the basket quantities
                var url = MUI.Settings.Endpoints.wishListSurface + 'UpdateWishList';
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
    
}

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

MUI.Checkout.Address = {
  
    addressType: '',
    
    init: function() {
        if (MUI.Settings.Endpoints.countryRegionApi === undefined || MUI.Settings.Endpoints.countryRegionApi === '') return;

        var frm = $('[data-muistage="BillingAddress"]');

        if (frm.length > 0) {
            MUI.Checkout.Address.addressType = "Billing";
            MUI.Checkout.Address.bind.form(frm[0]);
            var countryddl = frm.find(':input[data-muiaction="populateregion"]');
            //if country not selected, set to default
            if (countryddl.val().length == 0) {
                if (MUI.Settings.Defaults.BillingCountryCode.length > 0) {
                    countryddl.val(MUI.Settings.Defaults.BillingCountryCode);
                }
            }
            MUI.Checkout.Address.populateRegion(countryddl);

        } else {
            frm = $('[data-muistage="ShippingAddress"]');
            if (frm.length > 0) {
                MUI.Checkout.Address.addressType = "Shipping";
                MUI.Checkout.Address.bind.form(frm[0]);
                var countryddl = frm.find(':input[data-muiaction="populateregion"]');
                //if country not selected, set to default
                if (countryddl.val().length == 0) {
                    if (MUI.Settings.Defaults.ShippingCountryCode.length > 0) {
                        countryddl.val(MUI.Settings.Defaults.ShippingCountryCode);
                    }
                }
                MUI.Checkout.Address.populateRegion(countryddl);
            }
        }
    },

    bind: {

        form: function (frm) {
            // Watch for changes in the input fields
            $(frm).find(':input[data-muiaction="populateregion"]').change(function () {
                MUI.Checkout.Address.populateRegion($(this));
            });

            $(frm).find(':input[data-muiaction="updateregion"]').change(function () {
                var frmRef = $(this).closest('form');
                //keep the select list and the region textbox in sync
                frmRef.find(':input[data-muivalue="region"]').val(frmRef.find(':input[data-muiaction="updateregion"]').val());
            });
        }
    },

    populateRegion: function (countryCode) {
        var frmRef = countryCode.closest('form');
        // post the country to get the regions for that country
        var url = MUI.Settings.Endpoints.countryRegionApi + 'PostGetRegionsForCountry?countryCode=' + countryCode.val();
        $.ajax({
            type: 'POST',
            url: url
        }).then(function (results) {
            var regionddl = frmRef.find(':input[data-muiaction="updateregion"]');
            var regiontb = frmRef.find(':input[data-muivalue="region"]');
            if (results.length > 0) {
                //remove all but the first option
                $('option', regionddl).not(':eq(0)').remove();
                $.each(results, function (idx, item) {
                    regionddl.append($("<option></option>").attr("value", item.code).text(item.name));
                });
                //if region already defined and is present in the drop down, select it
                if (regiontb.val().length > 0 && regionddl.find('option[value="' + regiontb.val() + '"]').length > 0) {//and it exists in the drop down) {
                    //set ddl to current value
                    regionddl.val(regiontb.val());
                }
                else {
                    regiontb.val('');
                }
                //show region ddl and hide region textbox
                regionddl.show();
                regiontb.hide();
                //$('#UseForShipping').attr('disabled', false);
            }
            else {
                //hide region ddl and show region textbox
                regionddl.hide();
                regiontb.val('').show();
                //also, if there is no region then billing and shipping must be different??
                //$('#UseForShipping').prop('checked', false).attr('disabled', true);
            }
        }, function (err) {
            MUI.Logger.captureError(err);
        });
    }
    
    
};

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

            if($('.paypal-button').length > 0) {
                // binds the PayPal on time transaction
                MUI.Checkout.Payment.Braintree.bind.btpaypal.init();
            } else {
                // binds the standard transaction
                MUI.Checkout.Payment.Braintree.bind.btstandard.init();
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
            }
        },

        // PayPal payments through Braintree
        btpaypal: {

            init: function() {
                MUI.debugConsole('initializing PayPal OneTime Transaction');

                var token = MUI.Checkout.Payment.Braintree.getBraintreeToken();

                var paypalButton = document.querySelector('.paypal-button');

                // Create a client.
                braintree.client.create({
                    authorization: token
                }, function (clientErr, clientInstance) {

                    // Stop if there was a problem creating the client.
                    // This could happen if there is a network error or if the authorization
                    // is invalid.
                    if (clientErr) {
                        console.error('Error creating client:', clientErr);
                        return;
                    }

                    // Create a PayPal component.
                    braintree.paypal.create({
                        client: clientInstance
                    }, function (paypalErr, paypalInstance) {

                        // Stop if there was a problem creating PayPal.
                        // This could happen if there was a network error or if it's incorrectly
                        // configured.
                        if (paypalErr) {
                            console.error('Error creating PayPal:', paypalErr);
                            return;
                        }

                        // Enable the button.
                        paypalButton.removeAttribute('disabled');

                        // When the button is clicked, attempt to tokenize.
                        paypalButton.addEventListener('click', function (event) {

                            // Because tokenization opens a popup, this has to be called as a result of
                            // customer action, like clicking a button—you cannot call this at any time.
                            paypalInstance.tokenize({
                                flow: 'vault'
                            }, function (tokenizeErr, payload) {

                                // Stop if there was an error.
                                if (tokenizeErr) {
                                    if (tokenizeErr.type !== 'CUSTOMER') {
                                        console.error('Error tokenizing:', tokenizeErr);
                                    }
                                    return;
                                }

                                // Tokenization succeeded!
                                paypalButton.setAttribute('disabled', true);

                                if (MUI.Settings.Endpoints.braintreePayPalSurface !== '')
                                {
                                    var method = 'Process';
                                    var data = { nonce: payload.nonce };
                                    if (MUI.Checkout.Payment.Braintree.invoiceKey !== '') {
                                        method = 'Retry';
                                        data.invoiceKey = MUI.Checkout.Payment.Braintree.invoiceKey;
                                    }

                                    var url = MUI.Settings.Endpoints.braintreePayPalSurface + method;
                                    MUI.Checkout.Payment.Braintree.postBraintreeForm(url, data);
                                }
                            });

                        }, false);
                    });

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
    }
};

// put prototype methods there

// initialize the MUI object
MUI.init();


