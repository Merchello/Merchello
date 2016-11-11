/**
 * @ngdoc service
 * @name entityCollectionDisplayBuilder
 *
 * @description
 * A utility service that builds EntityCollectionDisplay models
 */
angular.module('merchello.models').factory('entityCollectionDisplayBuilder',
    ['genericModelBuilder', 'typeFieldDisplayBuilder', 'extendedDataDisplayBuilder', 'EntityCollectionDisplay',
        function(genericModelBuilder, typeFieldDisplayBuilder, extendedDataDisplayBuilder, EntityCollectionDisplay) {
            var Constructor = EntityCollectionDisplay;
            return {
                createDefault: function() {
                    var c = new Constructor();
                    c.extendedData = extendedDataDisplayBuilder.createDefault();
                    return c;
                },
                transform: function(jsonResult) {
                    var collections = [];
                    if (angular.isArray(jsonResult)) {
                        for(var i = 0; i < jsonResult.length; i++) {
                            var filters = null;
                            if (jsonResult[i].filters) {
                                filters = this.transform(jsonResult[i].filters);
                            }
                            var collection = genericModelBuilder.transform(jsonResult[ i ], Constructor);
                            collection.entityTypeField = typeFieldDisplayBuilder.transform(jsonResult[ i ].entityTypeField );
                            collection.extendedData = extendedDataDisplayBuilder.transform(jsonResult[i].extendedData);
                            if (filters !== null) {
                                collection.filters = filters;
                            }
                            collections.push(collection);
                        }
                    } else {
                        collections = genericModelBuilder.transform(jsonResult, Constructor);
                        collections.entityTypeField = typeFieldDisplayBuilder.transform(jsonResult.entityTypeField );
                        collections.extendedData = extendedDataDisplayBuilder.transform(jsonResult.extendedData);
                        if (jsonResult.filters) {
                            collections.filters = this.transform(jsonResult.filters);
                        }
                    }
                    return collections;
                }
            };
}]);
