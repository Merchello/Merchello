    /**
     * @ngdoc service
     * @name taxMethodDisplayBuilder
     *
     * @description
     * A utility service that builds TaxMethodDisplay models
     */
    angular.module('merchello.models').factory('taxMethodDisplayBuilder',
        ['genericModelBuilder', 'taxProvinceDisplayBuilder', 'TaxMethodDisplay',
            function(genericModelBuilder, taxProvinceDisplayBuilder, TaxMethodDisplay) {

                var Constructor = TaxMethodDisplay;

                return {
                    createDefault: function() {
                        return new Constructor();
                    },
                    transform: function(jsonResult) {
                        var methods = [];
                        if(angular.isArray(jsonResult)) {
                            for(var i = 0; i < jsonResult.length; i++) {
                                var method = genericModelBuilder.transform(jsonResult[ i ]);
                                method.provinces = taxProvinceDisplayBuilder.transform(jsonResult[ i ].provinces);
                                methods.push(method);
                            }
                        } else {
                            methods = genericModelBuilder.transform(jsonResult, Constructor);
                            methods.provinces = taxProvinceDisplayBuilder.transform(jsonResult.provinces);
                        }
                        return methods;
                    }
                };
    }]);
