    /**
     * @ngdoc resource
     * @name customerResource
     * @description Deals with customers api.
     **/
    angular.module('merchello.resources').factory('customerResource',
        ['$q', '$http', 'umbRequestHelper', 'customerItemCacheDisplayBuilder',
        function($q, $http, umbRequestHelper, customerItemCacheDisplayBuilder) {

            return {

                /**
                 * @ngdoc method
                 * @name AddCustomer
                 * @description Posts to the API a new customer.
                 **/
                AddCustomer: function(customer) {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloCustomerApiBaseUrl'] + 'AddCustomer';
                    return umbRequestHelper.resourcePromise($http.post(url, customer), 'Failed to create customer');
                },

                /**
                 * @ngdoc method
                 * @name AddAnonymousCustomer
                 * @description Posts to the API a new anonymous customer.
                 **/
                AddAnonymousCustomer: function (customer) {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloCustomerApiBaseUrl'] + 'AddAnonymousCustomer';
                    return umbRequestHelper.resourcePromise($http.post(url, customer), 'Failed to create customer');
                },

                /**
                 * @ngdoc method
                 * @name DeleteCustomer
                 * @description Posts to the API a request to delete the specified customer.
                 **/
                DeleteCustomer: function(customerKey) {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloCustomerApiBaseUrl'] + 'DeleteCustomer';
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: url,
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
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloCustomerApiBaseUrl'] + 'GetAllCustomers';
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: url, // TODO POST this is now SearchCustomers w/query
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
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloCustomerApiBaseUrl'] + 'GetCustomer';
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: url,
                            method: "GET",
                            params: { id: customerKey }
                        }),
                        'Failed to load customer');
                },

                getCustomerItemCache: function(customerKey, itemCacheType) {

                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloCustomerApiBaseUrl'] + 'GetCustomerItemCache';

                    var deferred = $q.defer();
                    $q.all([
                            umbRequestHelper.resourcePromise(
                                $http({
                                    url: url,
                                    method: "GET",
                                    params: { customerKey: customerKey, itemCacheType: itemCacheType }
                                }),
                                'Failed to retreive the customer item cache')])
                        .then(function(data) {

                            var results = customerItemCacheDisplayBuilder.transform(data[0]);
                            deferred.resolve(results);
                        });

                    return deferred.promise;
                },

                getGravatarUrl: function(email) {
                    var deferred = $q.defer();

                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloCustomerApiBaseUrl'] + 'GetGravatarUrl';
                        $http({
                            url: url,
                            method: "GET",
                            params: {email: email}
                        }).then(function(resp) {
                            deferred.resolve(resp.data.gravatarUrl);
                        });


                    return deferred.promise;
                },

                /**
                 * @ngdoc method
                 * @name PutCustomer
                 * @description Posts to the API an edited customer.
                 **/
                SaveCustomer: function(customer) {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloCustomerApiBaseUrl'] + 'PutCustomer';
                    return umbRequestHelper.resourcePromise($http.post(url, customer), 'Failed to save customer');
                },

                /**
                 * @ngdoc method
                 * @name searchCustomers
                 * @description Search for a list of customers using the parameters of the listQuery model.
                 * Valid query.sortBy options: firstname, lastname, loginname, email, lastactivitydate
                 * Valid query.sortDirection options: Ascending, Descending
                 * Defaults to sortBy: loginname
                 **/
                searchCustomers: function(query) {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloCustomerApiBaseUrl'] + 'SearchCustomers';
                    return umbRequestHelper.resourcePromise(
                        $http.post(url, query),
                        'Failed to retreive customers');
                }

            };

    }]);
