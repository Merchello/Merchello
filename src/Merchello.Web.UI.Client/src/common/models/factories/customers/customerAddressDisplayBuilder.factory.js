/**
 * @ngdoc service
 * @name customerAddressDisplayBuilder
 *
 * @description
 * A utility service that builds CustomerAddressDisplay models
 */
angular.module('merchello.models').factory('customerAddressDisplayBuilder',
     ['genericModelBuilder', 'CustomerAddressDisplay',
     function(genericModelBuilder, CustomerAddressDisplay) {

         var Constructor = CustomerAddressDisplay;
         return {
             createDefault: function() {
                 return new Constructor();
             },
             transform: function(jsonResult) {
                 return genericModelBuilder.transform(jsonResult, Constructor);
             }
         };

    }]);
