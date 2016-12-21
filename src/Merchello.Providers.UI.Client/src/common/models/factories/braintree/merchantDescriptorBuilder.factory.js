
// OBSOLETE
angular.module('merchello.providers.models').factory('merchantDescriptorBuilder',
    ['genericModelBuilder', 'MerchantDescriptor',
        function(genericModelBuilder, MerchantDescriptor) {

            var Constructor = MerchantDescriptor;

            return {
                createDefault: function () {
                    return new Constructor();
                },
                transform: function (jsonResult) {
                    return genericModelBuilder.transform(jsonResult, Constructor);
                }
            };
        }]);
