    /**
     * @ngdoc model
     * @name GatewayProviderDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's GatewayProviderDisplay object
     */
    var GatewayProviderDisplay = function() {
        var self = this;
        self.key = '';
        self.providerTfKey = '';
        self.name = '';
        self.description = '';
        self.extendedData = {};
        self.encryptExtendedData = false;
        self.activated = false;
        self.dialogEditorView = {};
    };

    angular.module('merchello.models').constant('GatewayProviderDisplay', GatewayProviderDisplay);