angular.module('merchello.models').factory('shippingGatewayProviderDisplayBuilder',
    ['genericModelBuilder', 'extendedDataDisplayBuilder', 'ShippingGatewayProviderDisplay',
        function(genericModelBuilder, extendedDataDisplayBuilder, ShippingGatewayProviderDisplay) {

            var Constructor = ShippingGatewayProviderDisplay;

            return {
                createDefault: function() {
                    return new Constructor();
                },
                transform: function(jsonResult) {
                    var providers = genericModelBuilder.transform(jsonResult, Constructor);
                    if(angular.isArray(providers)) {
                        for(var i = 0; i < providers.length; i++) {
                            providers[ i ].extendedData = extendedDataDisplayBuilder.transform(jsonResult[ i ]);
                        }
                    } else {
                        providers.extendedData = extendedDataDisplayBuilder.transform(jsonResult);
                    }
                    return providers;
                }
            };
    }]);
