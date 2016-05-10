if (MUI !== undefined) {

    // MUI Settings
    // Allows for overriding MUI defaults
    MUI.Settings = {

        // Notifications - the notification bar
        Notifications: {
            // If true, the notification bar will be appended before the </body> tag and notfication
            // messages will be inserted into the bar
            enabled: true,

            infoCss: 'alert-info',
            successCss: 'alert-success',
            warnCss: 'alert-warning',
            errorCss: 'alert-danger',

            // the template for the notification bar
            template: '<div class="alert mui-notify-bar" data-muinotify="notifybar"><div class="container" data-muivalue="nofity">Success</div></div>',
        },

        // API endpoints
        // These can be adjusted to match custom controllers which implement base classes
        Endpoints: {
            // the basket surface controller end point
            basketSurface: '/umbraco/QuickMart/Basket/',

            // the product table api controller end point
            productTableApi: '/umbraco/QuickMart/ProductDataTableApi/'
        }
    }
};
