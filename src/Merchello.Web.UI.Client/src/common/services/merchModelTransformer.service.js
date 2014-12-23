
    /**
   * @ngdoc model
   * @name merchello.services.modelTransformer
   * @function
   * 
   * @description
   * A utility service that builds local models for API query results
   */
    var merchModelTransformer = {};

    // http://odetocode.com/blogs/scott/archive/2014/03/17/building-better-models-for-angularjs.aspx
    merchModelTransformer.prototype = function() {

        // private
            
            // transforms json object into a local model
        var transformObject = function (jsonResult, constructor) {
                var model = new constructor();
                angular.extend(model, jsonResult);
                return model;
            },

            // transforms an array of json results to an array of local models
            transformResult = function (jsonResult, constructor) {
                if (angular.isArray(jsonResult)) {
                    var models = [];
                    angular.forEach(jsonResult, function (object) {
                        models.push(transformObject(object, constructor));
                    });
                    return models;
                } else {
                    return transformObject(jsonResult, constructor);
                }
            };

        //public
        return {
            transform : transformResult
        };
    }();

    angular.module('merchello.services').factory('modelTransformer', [], merchModelTransformer);