angular.module('merchello.models').factory('abandonedBasketResultBuilder',
    ['genericModelBuilder', 'AbandonedBasketResult',
    function(genericModelBuilder, AbandonedBasketResult) {

        var Constructor = AbandonedBasketResult;

        return {
            createDefault: function() {
                return new Constructor();
            },
            transform: function(jsonResult) {
                return genericModelBuilder.transform(jsonResult, Constructor);
            }
        };

}]);
