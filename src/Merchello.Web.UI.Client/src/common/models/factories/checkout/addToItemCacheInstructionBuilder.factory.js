angular.module('merchello.models').factory('addToItemCacheInstructionBuilder',
    ['genericModelBuilder', 'AddToItemCacheInstruction',
        function(genericModelBuilder, AddToItemCacheInstruction) {


            var Constructor = AddToItemCacheInstruction;

            return {
                createDefault: function() {
                    return new Constructor();
                },
                transform: function(jsonResult) {
                    return genericModelBuilder.transform(jsonResult, Constructor);
                }
            };
        }]);
