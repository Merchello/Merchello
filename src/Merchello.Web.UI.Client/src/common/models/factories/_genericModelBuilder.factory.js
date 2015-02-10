    /**
   * @ngdoc service
   * @name merchello.models.genericModelBuilder
   * 
   * @description
   * A utility service that builds local models for API query results
   *  http://odetocode.com/blogs/scott/archive/2014/03/17/building-better-models-for-angularjs.aspx
   */
    angular.module('merchello.models')
        .factory('genericModelBuilder', [
            function() {
        
        // private
        // transforms json object into a local model
        function transformObject(jsonResult, Constructor) {
            var model = new Constructor();
            angular.extend(model, jsonResult);
            return model;
        }

        function transform(jsonResult, Constructor) {
            if (angular.isArray(jsonResult)) {
                var models = [];
                angular.forEach(jsonResult, function (object) {
                    models.push(transformObject(object, Constructor));
                });
                return models;
            } else {
                return transformObject(jsonResult, Constructor);
            }
        }

        // public
        return {
            transform : transform
        };
    }]);