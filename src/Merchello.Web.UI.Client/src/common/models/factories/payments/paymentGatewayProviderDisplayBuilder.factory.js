    angular.module('merchello.models').factory('paymentGatewayProviderDisplayBuilder',
        ['genericModelBuilder',
            'dialogEditorViewDisplayBuilder', 'extendedDataDisplayBuilder', 'PaymentGatewayProviderDisplay',
        function(genericModelBuilder,
                 dialogEditorViewDisplayBuilder, extendedDataDisplayBuilder, PaymentGatewayProviderDisplay) {

            var Constructor = PaymentGatewayProviderDisplay;

            return {
                createDefault: function() {
                    return new Constructor();
                },
                transform: function(jsonResult) {
                    var providers = [];
                    if(angular.isArray(jsonResult)) {
                        for(var i = 0; i < jsonResult.length; i++) {
                            var provider = genericModelBuilder.transform(jsonResult[ i ], Constructor);
                            provider.dialogEditorView = dialogEditorViewDisplayBuilder.transform(jsonResult[ i ].dialogEditorView);
                            provider.extendedData = extendedDataDisplayBuilder.transform(jsonResult[ i ].extendedData);
                            providers.push(provider);
                        }
                    } else {
                        providers = genericModelBuilder.transform(jsonResult, Constructor);
                        providers.dialogEditorView = dialogEditorViewBuilder.transform(jsonResult.dialogEditorView);
                        providers.extendedData = extendedDataDisplayBuilder.transform(jsonResult.extendedData);
                    }
                    return providers;
                }
            };
    }]);
