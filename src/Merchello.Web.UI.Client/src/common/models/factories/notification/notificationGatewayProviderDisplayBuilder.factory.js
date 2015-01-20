/**
 * @ngdoc service
 * @name notificationGatewayProviderDisplayBuilder
 *
 * @description
 * A utility service that builds NotificationGatewayProviderDisplay models
 */
angular.module('merchello.models').factory('notificationGatewayProviderDisplayBuilder',
    ['genericModelBuilder',
        'dialogEditorViewDisplayBuilder', 'extendedDataDisplayBuilder', 'NotificationGatewayProviderDisplay',
        function(genericModelBuilder,
                 dialogEditorViewDisplayBuilder, extendedDataDisplayBuilder, NotificationGatewayProviderDisplay) {

            var Constructor = NotificationGatewayProviderDisplay;

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

