angular.module('merchello.models').factory('shippingGatewayMethodDisplayBuilder',
    ['genericModelBuilder', 'shipMethodDisplayBuilder', 'shipCountryDisplayBuilder', 'gatewayResourceDisplayBuilder',
        'dialogEditorViewDisplayBuilder', 'ShippingGatewayMethodDisplay',
        function(genericModelBuilder, shipMethodDisplayBuilder, shipCountryDisplayBuilder, gatewayResourceDisplayBuilder,
                 dialogEditorViewDisplayBuilder, ShippingGatewayMethodDisplay) {

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
                            method.dialogEditorView = dialogEditorViewDisplayBuilder.transform(result.dialogEditorView);
                            methods.push(method);
                        });
                        return methods;
                    } else {
                        var method = new Constructor();
                        method.gatewayResource = gatewayResourceDisplayBuilder.transform(jsonResult.gatewayResource);
                        method.shipMethod = shipMethodDisplayBuilder.transform(jsonResult.shipMethod);
                        method.shipCountry = shipCountryDisplayBuilder.transform(jsonResult.shipCountry);
                        method.dialogEditorView = dialogEditorViewDisplayBuilder.transform(jsonResult.dialogEditorView);
                        return method;
                    }

                }
            };

    }]);
