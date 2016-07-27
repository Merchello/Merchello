angular.module('merchello.models').factory('productOptionUseCountBuilder',
    ['genericModelBuilder', 'entityUseCountBuilder' , 'ProductOptionUseCount',
    function(genericModelBuilder, entityUseCountBuilder, ProductOptionUseCount) {

        var Constructor = ProductOptionUseCount;

        return {
            createDefault: function() {
                return new Constructor();
            },
            transform: function(jsonResult) {
                var result = this.createDefault();
                result.option = entityUseCountBuilder.transform(jsonResult.option);
                result.choices = entityUseCountBuilder.transform(jsonResult.choices);
                return result;
            }
        };

}]);
