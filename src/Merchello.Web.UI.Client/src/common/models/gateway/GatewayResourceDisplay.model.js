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

    GatewayResourceDisplay.prototype = (function() {

        function serviceCodeStartsWith(str) {
            return this.serviceCode.indexOf(str) === 0;
        }

        return {
            serviceCodeStartsWith: serviceCodeStartsWith
        };

    }());

    angular.module('merchello.models').constant('GatewayResourceDisplay', GatewayResourceDisplay);