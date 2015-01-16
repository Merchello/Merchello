angular.module('merchello.models').factory('shippingGatewayProviderDisplayBuilder',
    ['genericModelBuilder', 'extendedDataDisplayBuilder', 'shipMethodDisplayBuilder', 'gatewayResourceDisplayBuilder', 'ShippingGatewayProviderDisplay',
        function(genericModelBuilder, extendedDataDisplayBuilder, shipMethodDisplayBuilder, gatewayResourceDisplayBuilder, ShippingGatewayProviderDisplay) {

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
                            providers[ i ].availableResources = gatewayResourceDisplayBuilder.transform(jsonResult[ i ].availableResources);
                        }
                    } else {
                        providers.extendedData = extendedDataDisplayBuilder.transform(jsonResult.extendedData);
                        providers.shipMethods = shipmentDisplayBuilder.transform(jsonResult.shipMethods);
                        providers.availableResources = gatewayResourceDisplayBuilder.transform(jsonResult.availableResources);
                    }

                    return providers;
                }
            };
    }]);
