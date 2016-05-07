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
        return;
    },

    captureMessage: function(msg, args) {
        return;
    },

    isReady: function() {
        return MUI.Logger.hasLogger;
        // this can be used to test a logger ex. getsentry.com -> Raven.isSetup();
    }
};
