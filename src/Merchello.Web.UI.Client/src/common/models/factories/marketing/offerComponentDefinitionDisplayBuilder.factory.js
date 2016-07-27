    /**
     * @ngdoc service
     * @name merchello.models.offerComponentDefinitionDisplayBuilder
     *
     * @description
     * A utility service that builds OfferComponentDefinitionDisplay models
     */
    angular.module('merchello.models').factory('offerComponentDefinitionDisplayBuilder',
        ['genericModelBuilder', 'extendedDataDisplayBuilder', 'dialogEditorViewDisplayBuilder', 'OfferComponentDefinitionDisplay',
        function(genericModelBuilder, extendedDataDisplayBuilder, dialogEditorViewDisplayBuilder, OfferComponentDefinitionDisplay) {

            var Constructor = OfferComponentDefinitionDisplay;

            return {
                createDefault: function() {
                    var definition = new Constructor();
                    definition.extendedData = extendedDataDisplayBuilder.createDefault();
                    return definition;
                },
                transform: function(jsonResult) {
                    var definitions = [];
                    if(angular.isArray(jsonResult)) {
                        for(var i = 0; i < jsonResult.length; i++) {
                            var definition = genericModelBuilder.transform(jsonResult[ i ], Constructor);
                            definition.extendedData = extendedDataDisplayBuilder.transform(jsonResult[ i ].extendedData);
                            definition.dialogEditorView = dialogEditorViewDisplayBuilder.transform(jsonResult[ i ].dialogEditorView);
                            definitions.push(definition);
                        }
                    } else {
                        definitions = genericModelBuilder.transform(jsonResult, Constructor);
                        definitions.extendedData = extendedDataDisplayBuilder.transform(jsonResult.extendedData);
                        definitions.dialogEditorView = dialogEditorViewDisplayBuilder.transform(jsonResult.dialogEditorView);
                    }
                    return definitions;
                }
            };
        }]);