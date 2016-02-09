angular.module('merchello.models').factory('itemCacheInstructionBuilder',
    ['genericModelBuilder', 'ItemCacheInstruction',
    function(genericModelBuilder, ItemCacheInstruction) {


            var Constructor = ItemCacheInstruction;

            return {
                createDefault: function() {
                    return new Constructor();
                },
                transform: function(jsonResult) {
                    return genericModelBuilder.transform(jsonResult, Constructor);
                }
            };
}]);
