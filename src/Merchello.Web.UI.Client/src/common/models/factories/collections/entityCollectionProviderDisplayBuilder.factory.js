/**
 * @ngdoc service
 * @name entityCollectionDisplayBuilder
 *
 * @description
 * A utility service that builds EntityCollectionDisplay models
 */
angular.module('merchello.models').factory('entityCollectionProviderDisplayBuilder',
    ['genericModelBuilder', 'entityCollectionDisplayBuilder',  'typeFieldDisplayBuilder', 'dialogEditorViewDisplayBuilder',
        'EntityCollectionProviderDisplay',
        function(genericModelBuilder, entityCollectionDisplayBuilder, typeFieldDisplayBuilder, dialogEditorViewDisplayBuilder, EntityCollectionProviderDisplay) {
            var Constructor = EntityCollectionProviderDisplay;
            return {
                createDefault: function() {
                    return new Constructor();
                },
                transform: function(jsonResult) {
                    var providers = [];
                    if(angular.isArray(jsonResult)) {
                        for(var i = 0; i < jsonResult.length; i++) {
                            var provider = genericModelBuilder.transform(jsonResult[ i ], Constructor);
                            provider.managedCollections = entityCollectionDisplayBuilder.transform(jsonResult[ i ].managedCollections);
                            provider.entityTypeField = typeFieldDisplayBuilder.transform(jsonResult[i].entityTypeField);
                            provider.dialogEditorView = dialogEditorViewDisplayBuilder.transform(jsonResult[i].dialogEditorView);
                            providers.push(provider);
                        }
                    } else {
                        providers = genericModelBuilder.transform(jsonResult, Constructor);
                        providers.managedCollections = entityCollectionDisplayBuilder.transform(jsonResult.managedCollections);
                        providers.entityTypeField = typeFieldDisplayBuilder.transform(jsonResult.entityTypeField);
                        providers.dialogEditorView = dialogEditorViewDisplayBuilder.transform(jsonResult.dialogEditorView);
                    }
                    return providers;
                }
            };
        }]);

