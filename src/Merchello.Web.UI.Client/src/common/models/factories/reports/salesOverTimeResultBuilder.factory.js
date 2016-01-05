angular.module('merchello.models').factory('salesOverTimeResultBuilder',
    ['genericModelBuilder', 'SalesOverTimeResult',
        function(genericModelBuilder, SalesOverTimeResult) {

            var Constructor = SalesOverTimeResult;

            return {
                createDefault: function() {
                    return new Constructor();
                },
                transform: function(jsonResult) {
                    return genericModelBuilder.transform(jsonResult, Constructor);
                }
            };
}]);