/*! MUI
 * https://merchello.com
 * Copyright (c) 2016 Accross the Pond, LLC.
 * Licensed 
 */


// Generic Merchello UI scripts.
// Requires JQuery version 1.10.2 or higher
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
        var evt = _.find(events, function(e) { return e.attempt === alias});
        return evt === undefined ? '' : evt.name;
    }

    // writes events to debug console
    function debugConsoleEvents(events) {
        if (DEBUG_MODE.events) {
            _.each(events, function(ev) {
                CO.on(ev.name, function(name, args) {
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

    init: function() {
        // find all of the AddItem forms
        var frms = $('[data-muifrm="additem"]');
        if (frms.length > 0) {
            $.each(frms, function(idx, frm) {
               MUI.AddItem.bind.form(frm);
            });
        }
    },

    bind: {

        // bind the form;
        form: function(frm) {
            
        }
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
                MUI.Forms.post(frmRef, MUI.Settings.basketSurfaceEndpoint + 'UpdateBasket')
                    .then(function(results) {

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
        
        return $.ajax({
            type: 'POST',
            url: url,
            data: $(frm).serialize()
        });
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
        var consoleLog = args === undefined ? message : { error: e, args: args };
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
        // this can be used to test a logger ex. getsentry.com -> Raven.isSetup();
    }
};

// put prototype methods there

// initialize the MUI object
MUI.init();


