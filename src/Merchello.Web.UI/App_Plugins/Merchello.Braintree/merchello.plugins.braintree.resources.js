(function() {
   angular.module('merchello.plugins.braintree').factory('braintreeResource',
       ['$http', 'umbRequestHelper',
       function($http, umbRequestHelper) {

           var baseUrl = Umbraco.Sys.ServerVariables["merchelloBraintree"]["merchelloBraintreeBaseUrl"];

           return {

               getClientRequestToken : function(customerKey) {
                   return umbRequestHelper.resourcePromise(
                       $http({
                           url: baseUrl + 'GetClientRequestToken',
                           method: "GET",
                           params: { customerKey: customerKey }
                       }),
                       'Failed to retreive customer request token');
               }
           };
   }]);
}());
