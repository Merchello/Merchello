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
