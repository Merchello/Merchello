angular.module('merchello.models').factory('shippingGatewayMethodDisplayBuilder',
    ['genericModelBuilder', 'shipMethodDisplayBuilder', 'shipCountryDisplayBuilder', 'gatewayResourceDisplayBuilder', 'ShippingGatewayMethodDisplay',
        function(genericModelBuilder, shipMethodDisplayBuilder, shipCountryDisplayBuilder, gatewayResourceDisplayBuilder, ShippingGatewayMethodDisplay) {

            var Constructor = ShippingGatewayMethodDisplay;

            return {
                createDefault: function() {
                    return new Constructor();
                },
                transform: function(jsonResult) {

                    if(angular.isArray(jsonResult)) {
                        var methods = [];
                        angular.forEach(jsonResult, function(result) {
                            var method = new Constructor();
                            method.gatewayResource = gatewayResourceDisplayBuilder.transform(result.gatewayResource);
                            method.shipMethod = shipMethodDisplayBuilder.transform(result.shipMethod);
                            method.shipCountry = shipCountryDisplayBuilder.transform(result.shipCountry);
                            methods.push(method);
                        });
                        return methods;
                    } else {
                        var method = new Constructor();
                        method.gatewayResource = gatewayResourceDisplayBuilder.transform(jsonResult.gatewayResource);
                        method.shipMethod = shipMethodDisplayBuilder.transform(jsonResult.shipMethod);
                        method.shipCountry = shipCountryDisplayBuilder.transform(jsonResult.shipCountry);
                        return method;
                    }

                }
            };

    }]);
