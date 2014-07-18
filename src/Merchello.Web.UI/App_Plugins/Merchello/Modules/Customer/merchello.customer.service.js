(function (merchelloServices, undefined) {


    /**
    * @ngdoc service
    * @name umbraco.resources.MerchelloCustomerService
    * @description Deals with customers api.
    **/
    merchelloServices.MerchelloCustomerService = function ($http, umbRequestHelper) {

        return {

            /**
            * @ngdoc method
            * @name AddCustomer
            * @description Posts to the API a new customer.
            **/
            AddCustomer: function(customer) {
                return umbRequestHelper.resourcePromise($http.post(umbRequestHelper.getApiUrl('merchelloCustomerApiBaseUrl', 'AddCustomer'), customer), 'Failed to create customer');
            },

            /**
            * @ngdoc method
            * @name DeleteCustomer
            * @description Posts to the API a request to delete the specified customer.
            **/
            DeleteCustomer: function(customerKey) {
                return umbRequestHelper.resourcePromise(
                $http({
                    url: umbRequestHelper.getApiUrl('merchelloCustomerApiBaseUrl', 'DeleteCustomer'),
                    method: "GET",
                    params: { id: customerKey }
                }),
                'Failed to delete customer');
            },

            /**
            * @ngdoc method
            * @name GetAllCustomers
            * @description Requests from the API a list of all the customers.
            **/
            GetAllCustomers: function(page, perPage) {
                if (page === undefined) {
                    page = 1;
                }
                if (page < 1) {
                    page = 1;
                }
                if (perPage === undefined) {
                    perPage = 100;
                }
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloCustomerApiBaseUrl', 'GetAllCustomers'),
                        method: "GET",
                        params: { page: page, perPage: perPage }
                    }),
                    'Failed to load customers');
            },

            /**
            * @ngdoc method
            * @name GetCustomer
            * @description Requests from the API a customer with the provided customerKey.
            **/
            GetCustomer: function(customerKey) {
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloCustomerApiBaseUrl', 'GetCustomer'),
                        method: "GET",
                        params: { id: customerKey }
                    }),
                    'Failed to load customer');
            },

            /**
            * @ngdoc method
            * @name PutCustomer
            * @description Posts to the API an edited customer.
            **/
            SaveCustomer: function(customer) {
                return umbRequestHelper.resourcePromise($http.post(umbRequestHelper.getApiUrl('merchelloCustomerApiBaseUrl', 'PutCustomer'), customer), 'Failed to save customer');
            }

        };
    };

    angular.module('umbraco.resources').factory('merchelloCustomerService', ['$http', 'umbRequestHelper', merchello.Services.MerchelloCustomerService]);

}(window.merchello.Services = window.merchello.Services || {}));
