    /**
     * @ngdoc model
     * @name NotificationGatewayProviderDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's NotificationGatewayProviderDisplay object
     */
    var NotificationGatewayProviderDisplay = function() {
        var self = this;
        self.key = '';
        self.name = '';
        self.providerTfKey = '';
        self.description = '';
        self.extendedData = {};
        self.encryptExtendedData = false;
        self.activated = false;
        self.showSelectedResource = false;
        self.dialogEditorView = {};
        self.gatewayResources = [];
        self.notificationMethods = [];
    };

    angular.module('merchello.models').constant('NotificationGatewayProviderDisplay', NotificationGatewayProviderDisplay);