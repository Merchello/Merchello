angular.module('merchello.models').factory('shippingGatewayProviderDisplayBuilder',
    ['genericModelBuilder', 'extendedDataDisplayBuilder', 'shipMethodDisplayBuilder', 'ShippingGatewayProviderDisplay',
        function(genericModelBuilder, extendedDataDisplayBuilder, shipMethodDisplayBuilder, ShippingGatewayProviderDisplay) {

            var Constructor = ShippingGatewayProviderDisplay;

            return {
                createDefault: function() {
                    return new Constructor();
                },
                transform: function(jsonResult) {
                    var providers = genericModelBuilder.transform(jsonResult, Constructor);
                    if(angular.isArray(providers)) {
                        for(var i = 0; i < providers.length; i++) {
                            providers[ i ].extendedData = extendedDataDisplayBuilder.transform(jsonResult[ i ].extendedData);
                            providers[ i ].shipMethods = shipMethodDisplayBuilder.transform(jsonResult[ i ].shipMethods);
                        }
                    } else {
                        providers.extendedData = extendedDataDisplayBuilder.transform(jsonResult.extendedData);
                        providers.shipMethods = shipmentDisplayBuilder.transform(jsonResult.shipMethods)
                    }
                    return providers;
                }
            };
    }]);
