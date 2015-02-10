    /**
     * @ngdoc service
     * @name merchello.models.gatewayProviderDisplayBuilder
     *
     * @description
     * A utility service that builds GatewayProviderDisplay models
     */
    angular.module('merchello.models').factory('gatewayProviderDisplayBuilder',
        ['genericModelBuilder', 'extendedDataDisplayBuilder', 'dialogEditorViewDisplayBuilder', 'GatewayProviderDisplay',
        function(genericModelBuilder, extendedDataDisplayBuilder, dialogEditorViewDisplayBuilder, GatewayProviderDisplay) {

            var Constructor = GatewayProviderDisplay;

            return {
                createDefault: function () {
                    var gatewayProvider = new Constructor();
                    gatewayProvider.extendedData = extendedDataDisplayBuilder.createDefault();
                    gatewayProvider.dialogEditorView = dialogEditorViewDisplayBuilder.createDefault();
                    return gatewayProvider;
                },
                transform: function (jsonResult) {
                    var gatewayProviders = genericModelBuilder.transform(jsonResult, Constructor);
                    if (angular.isArray(gatewayProviders)) {
                        for (var i = 0; i < gatewayProviders.length; i++) {
                            gatewayProviders[i].extendedData = extendedDataDisplayBuilder.transform(jsonResult[i].extendedData);
                            gatewayProviders[i].dialogEditorView = dialogEditorViewDisplayBuilder.transform(jsonResult[i].dialogEditorView);
                        }
                    } else {
                        gatewayProviders.extendedData = extendedDataDisplayBuilder.transform(jsonResult.extendedData);
                        gatewayProviders.dialogEditorView = dialogEditorViewDisplayBuilder.transform(jsonResult.dialogEditorView);
                    }
                    return gatewayProviders;
                }
            };
        }]);
