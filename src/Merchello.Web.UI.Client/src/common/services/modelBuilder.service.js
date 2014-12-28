    /**
     * @ngdoc service
     * @name merchello.services.modelBuilder
     * @function
     *
     * @description
     * A utility service that builds local models for API query results
     *  http://odetocode.com/blogs/scott/archive/2014/03/17/building-better-models-for-angularjs.aspx
     */
    var ModelBuilder = function(modelTransformer) {
        this.transformer = modelTransformer;
    };

    ModelBuilder.prototype.buildAddress = function(jsonResult) {
        var constructor = Merchello.Models.Address;
        return this.transformer.transform(jsonResult, constructor);
    };

    angular.module('merchello.services')
        .factory('modelBuilder', ['modelTransformer',
            ModelBuilder]);
