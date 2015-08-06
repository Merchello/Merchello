/**
 * @ngdoc service
 * @name entityCollectionDisplayBuilder
 *
 * @description
 * A utility service that builds EntityCollectionDisplay models
 */
angular.module('merchello.models').factory('entityCollectionDisplayBuilder',
    ['genericModelBuilder', 'typeFieldDisplayBuilder', 'EntityCollectionDisplay',
        function(genericModelBuilder, typeFieldDisplayBuilder, EntityCollectionDisplay) {
            var Constructor = EntityCollectionDisplay;
            return {
                createDefault: function() {
                    return new Constructor();
                },
                transform: function(jsonResult) {
                    var collections = [];
                    if (angular.isArray(jsonResult)) {
                        for(var i = 0; i < jsonResult.length; i++) {
                            var collection = genericModelBuilder.transform(jsonResult[ i ], Constructor);
                            collection.entityTypeField = typeFieldDisplayBuilder.transform(jsonResult[ i ].entityTypeField );
                            collections.push(collection);
                        }
                    } else {
                        collections = genericModelBuilder.transform(jsonResult, Constructor);
                        collections.entityTypeField = typeFieldDisplayBuilder.transform(jsonResult.entityTypeField );
                    }
                    return collections;
                }
            };
}]);
