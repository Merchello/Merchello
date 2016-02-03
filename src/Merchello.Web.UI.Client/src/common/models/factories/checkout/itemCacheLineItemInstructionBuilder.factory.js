angular.module('merchello.models').factory('itemCacheLineItemInstructionBuilder',
    ['genericModelBuilder', 'ItemCacheLineItemInstruction',
    function(genericModelBuilder, ItemCacheLineItemInstruction) {


            var Constructor = ItemCacheLineItemInstruction;

            return {
                createDefault: function() {
                    return new Constructor();
                },
                transform: function(jsonResult) {
                    return genericModelBuilder.transform(jsonResult, Constructor);
                }
            };
}]);
