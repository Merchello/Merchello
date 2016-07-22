angular.module('merchello.models').factory('entityUseCountBuilder',
    ['genericModelBuilder', 'EntityUseCount',
    function(genericModelBuilder, EntityUseCount) {

        var Constructor = EntityUseCount;

        return {
            createDefault: function() {
                return new Constructor();
            },
            transform: function(jsonResult) {
                return genericModelBuilder.transform(jsonResult, Constructor);
            }
        };

}]);
