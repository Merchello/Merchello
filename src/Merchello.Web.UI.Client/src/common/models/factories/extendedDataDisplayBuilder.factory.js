    /**
     * @ngdoc service
     * @name merchello.models.extendedDataDisplayBuilder
     *
     * @description
     * A utility service that builds ExtendedDataBuilder models
     */
    angular.module('merchello.models')
        .factory('extendedDataDisplayBuilder',
        ['genericModelBuilder', 'ExtendedDataDisplay',
            function(genericModelBuilder, ExtendedDataDisplay) {

                var Constructor = ExtendedDataDisplay;
                var extendedDataItem = function() {
                    this.key = '';
                    this.value = '';
                };

                return {
                    createDefault: function() {
                        return new Constructor();
                    },
                    transform: function(jsonResult) {
                        var extendedData = new Constructor();
                        extendedData.items = genericModelBuilder.transform(jsonResult, extendedDataItem);
                        return extendedData;
                    }
                };
            }]);
