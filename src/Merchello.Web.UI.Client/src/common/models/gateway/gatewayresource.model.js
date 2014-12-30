    /**
     * @ngdoc model
     * @name GatewayResourceDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's GatewayResourceDisplay object
     */
    var GatewayResourceDisplay = function() {
        self.name = '';
        self.serviceCode = '';
    };

    angular.module('merchello.models').constant('GatewayResourceDisplay', GatewayResourceDisplay);