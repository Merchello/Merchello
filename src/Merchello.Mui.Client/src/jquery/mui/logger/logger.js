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
