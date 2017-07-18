    /**
     * @ngdoc service
     * @name merchello.models.extendedDataDisplayBuilder
     *
     * @description
     * A utility service that builds ExtendedDataBuilder models
     */
    angular.module('merchello.models')
        .factory('extendedDataDisplayBuilder',
        ['genericModelBuilder', 'ExtendedDataDisplay', 'ExtendedDataItemDisplay',
            function(genericModelBuilder, ExtendedDataDisplay, ExtendedDataItemDisplay) {

                var Constructor = ExtendedDataDisplay;


                return {
                    createDefault: function() {
                        return new Constructor();
                    },
                    transform: function(jsonResult) {
                        var extendedData = new Constructor();
                        if (jsonResult !== undefined) {
                            var items = genericModelBuilder.transform(jsonResult, ExtendedDataItemDisplay);
                            if (items.length > 0) {
                                extendedData.items = items;
                            } else {
                                extendedData.items = null;
                            }
                        }
                        return extendedData;
                    }
                };
            }]);
