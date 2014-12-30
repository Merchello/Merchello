    /**
     * @ngdoc service
     * @name merchello.services.queryResultDisplayBuilder
     *
     * @description
     * A utility service that builds QueryResultDisplayModels models
     */
    angular.module('merchello.models')
        .factory('queryResultDisplayBuilder',
        ['genericModelBuilder',
            function(genericModelBuilder) {
            var Constructor = QueryResultDisplay;
            return {
                createDefault: function() {
                    return new Constructor();
                },
                transform: function (jsonResult, itemBuilder) {
                    // this is slightly different than other builders in that there can only ever be a single
                    // QueryResult returned from the WebApiController, so we iterate through the items
                    var result = genericModelBuilder.transform(jsonResult, Constructor);
                    angular.forEach(jsonResult.items, function(item) {
                      if (itemBuilder !== undefined) {
                          item = itemBuilder.transform(item);
                      }
                      result.items.push(item);
                    });
                    return result;
                }
            };
        }]);
