/*! merchello
 * https://github.com/meritage/Merchello
 * Copyright (c) 2015 Merchello;
 * Licensed MIT
 */

(function() { 

    /**
     * @ngdoc resource
     * @name auditLogResource
     * @description Loads in data and allows modification of audit logs
     **/
    angular.module('merchello.resources').factory('auditLogResource', [
        '$http', 'umbRequestHelper',
        function($http, umbRequestHelper) {
        return {

            /**
             * @ngdoc method
             * @name getSalesHistoryByInvoiceKey
             * @description
             **/
            getSalesHistoryByInvoiceKey: function (key) {
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloAuditLogApiBaseUrl', 'GetSalesHistoryByInvoiceKey'),
                        method: "GET",
                        params: { id: key }
                    }),
                    'Failed to retreive sales history log for invoice with following key: ' + key);
            }
        };
    }]);

    /**
     * @ngdoc resource
     * @name customerResource
     * @description Deals with customers api.
     **/
    angular.module('merchello.resources').factory('customerResource',
        ['$http', 'umbRequestHelper',
        function($http, umbRequestHelper) {

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
                 * @name AddAnonymousCustomer
                 * @description Posts to the API a new anonymous customer.
                 **/
                AddAnonymousCustomer: function (customer) {
                    return umbRequestHelper.resourcePromise($http.post(umbRequestHelper.getApiUrl('merchelloCustomerApiBaseUrl', 'AddAnonymousCustomer'), customer), 'Failed to create customer');
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
                            url: umbRequestHelper.getApiUrl('merchelloCustomerApiBaseUrl', 'GetAllCustomers'), // TODO POST this is now SearchCustomers w/query
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
                    return umbRequestHelper.resourcePromise(
                        $http.post(umbRequestHelper.getApiUrl('merchelloCustomerApiBaseUrl', 'SearchCustomers'), query),
                        'Failed to retreive customers');
                }

            };

    }]);

/**
 * @ngdoc resource
 * @name gatewayProviderResource
 * @description Loads in data and allows modification of gateway providers
 **/
angular.module('merchello.resources')
    .factory('gatewayProviderResource',
    ['$http', 'umbRequestHelper',
        function($http, umbRequestHelper) {

        return {
            getGatewayProvider: function (providerKey) {

                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloGatewayProviderApiBaseUrl', 'GetGatewayProvider'),
                        method: "GET",
                        params: { id: providerKey }
                    }),
                    'Failed to retreive gateway provider data');
            },

            getResolvedNotificationGatewayProviders: function () {

                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloGatewayProviderApiBaseUrl', 'GetResolvedNotificationGatewayProviders'),
                        method: "GET"
                    }),
                    'Failed to retrieve data for all resolved notification gateway providers');
            },

            getResolvedPaymentGatewayProviders: function () {

                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloGatewayProviderApiBaseUrl', 'GetResolvedPaymentGatewayProviders'),
                        method: "GET"
                    }),
                    'Failed to retreive data for all resolved payment gateway providers');
            },

            getResolvedShippingGatewayProviders: function () {

                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloGatewayProviderApiBaseUrl', 'GetResolvedShippingGatewayProviders'),
                        method: "GET"
                    }),
                    'Failed to retreive data for all resolved shipping gateway providers');
            },

            getResolvedTaxationGatewayProviders: function () {

                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloGatewayProviderApiBaseUrl', 'GetResolvedTaxationGatewayProviders'),
                        method: "GET"
                    }),
                    'Failed to retreive data for all resolved taxation gateway providers');
            },

            activateGatewayProvider: function (gatewayProvider) {

                return umbRequestHelper.resourcePromise(
                    $http.post(umbRequestHelper.getApiUrl('merchelloGatewayProviderApiBaseUrl', 'ActivateGatewayProvider'),
                        gatewayProvider
                    ),
                    'Failed to activate gateway provider');
            },

            deactivateGatewayProvider: function (gatewayProvider) {

                return umbRequestHelper.resourcePromise(
                    $http.post(umbRequestHelper.getApiUrl('merchelloGatewayProviderApiBaseUrl', 'DeactivateGatewayProvider'),
                        gatewayProvider
                    ),
                    'Failed to deactivate gateway provider');
            },

            saveGatewayProvider: function(gatewayProvider) {
                // we need to hack the extended data here
                gatewayProvider.extendedData = gatewayProvider.extendedData.toArray();
                return umbRequestHelper.resourcePromise(
                    $http.post(umbRequestHelper.getApiUrl('merchelloGatewayProviderApiBaseUrl', 'PutGatewayProvider'),
                        gatewayProvider
                    ),
                    'Failed to save gateway provider');
            }
        };
    }]);

    /**
     * @ngdoc resource
     * @name invoiceResource
     * @description Loads in data and allows modification for invoices
     **/
    angular.module('merchello.resources')
        .factory('invoiceResource', [
            '$q', '$http', 'umbRequestHelper',
            function($q, $http, umbRequestHelper) {

                return {

                    /**
                     * @ngdoc method
                     * @name getByKey
                     * @description
                     **/
                    getByKey: function (id) {
                        return umbRequestHelper.resourcePromise(
                            $http({
                                url: umbRequestHelper.getApiUrl('merchelloInvoiceApiBaseUrl', 'GetInvoice'),
                                method: "GET",
                                params: { id: id }
                            }),
                            'Failed to retreive data for invoice id: ' + id);
                    },

                    /**
                     * @ngdoc method
                     * @name getByCustomerKey
                     * @description
                     **/
                    getByCustomerKey: function(customerKey) {
                        var query = queryDisplayBuilder.createDefault();
                        query.applyInvoiceQueryDefaults();
                        query.addCustomerKeyParam(customerKey);

                        return umbRequestHelper.resourcePromise(
                            $http.post(umbRequestHelper.getApiUrl('merchelloInvoiceApiBaseUrl', 'SearchByCustomer'), query),
                            'Failed to retreive invoices');
                    },

                    /**
                     * @ngdoc method
                     * @name searchInvoices
                     * @description
                     **/
                    searchInvoices: function (query) {
                        if (query === undefined) {
                            query = queryDisplayBuilder.createDefault();
                            query.applyInvoiceQueryDefaults();
                        }

                        return umbRequestHelper.resourcePromise(
                            $http.post(umbRequestHelper.getApiUrl('merchelloInvoiceApiBaseUrl', 'SearchInvoices'), query),
                            'Failed to retreive invoices');
                    },

                    nextsearchInvoices: function(query) {
                        var deferred = $q.defer();
                        var promises = [];
                        promises.push(this.doSearchInvoices(query));

                    },

                    /**
                     * @ngdoc method
                     * @name searchInvoicesByDateRange
                     * @description
                     **/
                    searchInvoicesByDateRange: function (query) {
                        if (query === undefined) {
                            query = queryDisplayBuilder.createDefault();
                            query.applyInvoiceQueryDefaults();
                        }
                        return umbRequestHelper.resourcePromise(
                            $http.post(umbRequestHelper.getApiUrl('merchelloInvoiceApiBaseUrl', 'SearchByDateRange'), query),
                            'Failed to retreive invoices');
                    },

                    /**
                     * @ngdoc method
                     * @name saveInvoice
                     * @description
                     **/
                    saveInvoice: function (invoice) {
                        return umbRequestHelper.resourcePromise(
                            $http.post(umbRequestHelper.getApiUrl('merchelloInvoiceApiBaseUrl', 'PutInvoice'),
                                invoice
                            ),
                            'Failed to save invoice');
                    },

                    /**
                     * @ngdoc method
                     * @name deleteInvoice
                     * @description
                     **/
                    deleteInvoice: function (invoiceKey) {
                        return umbRequestHelper.resourcePromise(
                            $http({
                                url: umbRequestHelper.getApiUrl('merchelloInvoiceApiBaseUrl', 'DeleteInvoice'),
                                method: "GET",
                                params: { id: invoiceKey }
                            }),
                            'Failed to delete invoice');
                    }

                };
            }]);

    /**
     * @ngdoc resource
     * @name notificationGatewayProviderResource
     * @description Loads in data for notification providers
     **/
    angular.module('merchello.resources').factory('notificationGatewayProviderResource',
        ['$http', 'umbRequestHelper',
            function($http, umbRequestHelper) {

                return {

                    getGatewayResources: function (key) {

                        return umbRequestHelper.resourcePromise(
                            $http({
                                url: umbRequestHelper.getApiUrl('merchelloNotificationApiBaseUrl', 'GetGatewayResources') + "?id=" + key,
                                method: "GET"
                            }),
                            'Failed to save data for Notification');
                    },

                    getAllGatewayProviders: function () {

                        return umbRequestHelper.resourcePromise(
                            $http({
                                url: umbRequestHelper.getApiUrl('merchelloNotificationApiBaseUrl', 'GetAllGatewayProviders'),
                                method: "GET"
                            }),
                            'Failed to retreive data for all gateway providers');
                    },

                    getAllNotificationMonitors: function () {
                        return umbRequestHelper.resourcePromise(
                            $http({
                                url: umbRequestHelper.getApiUrl('merchelloNotificationApiBaseUrl', 'GetAllNotificationMonitors'),
                                method: "GET"
                            }),
                            'Failed to retreive data for all gateway providers');
                    },

                    getNotificationProviderNotificationMethods: function (id) {

                        return umbRequestHelper.resourcePromise(
                            $http({
                                url: umbRequestHelper.getApiUrl('merchelloNotificationApiBaseUrl', 'GetNotificationProviderNotificationMethods') + "?id=" + id,
                                method: "GET"
                            }),
                            'Failed to save data for Notification');
                    },

                    saveNotificationMethod: function (method) {

                        return umbRequestHelper.resourcePromise(
                            $http.post(
                                umbRequestHelper.getApiUrl('merchelloNotificationApiBaseUrl', 'AddNotificationMethod'),
                                angular.toJson(method)
                            ),
                            'Failed to save data for Notification');
                    },

                    deleteNotificationMethod: function (methodKey) {

                        return umbRequestHelper.resourcePromise(
                            $http({
                                url: umbRequestHelper.getApiUrl('merchelloNotificationApiBaseUrl', 'DeleteNotificationMethod') + "?id=" + methodKey,
                                method: "DELETE"
                            }),
                            'Failed to delete data for Notification');
                    },

                    getNotificationMessagesByKey: function (id) {

                        return umbRequestHelper.resourcePromise(
                            $http({
                                url: umbRequestHelper.getApiUrl('merchelloNotificationApiBaseUrl', 'GetNotificationMessagesByKey') + "?id=" + id,
                                method: "GET"
                            }),
                            'Failed to save data for Notification');

                    },

                    saveNotificationMessage: function (notification) {

                        return umbRequestHelper.resourcePromise(
                            $http.post(
                                umbRequestHelper.getApiUrl('merchelloNotificationApiBaseUrl', 'PutNotificationMessage'),
                                angular.toJson(notification)
                            ),
                            'Failed to save data for Notification');
                    },

                    deleteNotificationMessage: function (methodKey) {

                        return umbRequestHelper.resourcePromise(
                            $http({
                                url: umbRequestHelper.getApiUrl('merchelloNotificationApiBaseUrl', 'DeleteNotificationMessage') + "?id=" + methodKey,
                                method: "DELETE"
                            }),
                            'Failed to delete data for Notification');
                    },

                    updateNotificationMessage: function (notification) {

                        return umbRequestHelper.resourcePromise(
                            $http.post(
                                umbRequestHelper.getApiUrl('merchelloNotificationApiBaseUrl', 'UpdateNotificationMessage'),
                                angular.toJson(notification)
                            ),
                            'Failed to save data for Notification');
                    }
                };
    }]);

    /**
     * @ngdoc resource
     * @name orderResource
     * @description Loads in data and allows modification for orders
     **/
    angular.module('merchello.resources')
        .factory('orderResource', ['$http', 'umbRequestHelper',
            function($http, umbRequestHelper) {

            return {

                getOrder: function (orderKey) {

                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: umbRequestHelper.getApiUrl('merchelloOrderApiBaseUrl', 'GetOrder'),
                            method: "GET",
                            params: { id: orderKey }
                        }),
                        'Failed to get order: ' + orderKey);
                },

                getOrdersByInvoice: function (invoiceKey) {

                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: umbRequestHelper.getApiUrl('merchelloOrderApiBaseUrl', 'GetOrdersByInvoiceKey'),
                            method: "GET",
                            params: { id: invoiceKey }
                        }),
                        'Failed to get orders by invoice: ' + invoiceKey);
                },

                getUnFulfilledItems: function (invoiceKey) {

                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: umbRequestHelper.getApiUrl('merchelloOrderApiBaseUrl', 'GetUnFulfilledItems'),
                            method: "GET",
                            params: { id: invoiceKey }
                        }),
                        'Failed to get unfulfilled items by invoice: ' + invoiceKey);
                },

                getShippingAddress: function (invoiceKey) {
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: umbRequestHelper.getApiUrl('merchelloOrderApiBaseUrl', 'GetShippingAddress'),
                            method: "GET",
                            params: { id: invoiceKey }
                        }),
                        'Failed to get orders by invoice: ' + invoiceKey);
                },

                processesProductsToBackofficeOrder: function (customerKey, products, shippingAddress, billingAddress) {
                    var model = {};
                    model.CustomerKey = customerKey;
                    model.ProductKeys = products;
                    model.ShippingAddress = shippingAddress;
                    model.BillingAddress = billingAddress;

                    return umbRequestHelper.resourcePromise(
                        $http.post(umbRequestHelper.getApiUrl('merchelloOrderApiBaseUrl', 'ProcessesProductsToBackofficeOrder'),
                            model
                        ),
                        'Failed to add products to invoice');
                },

                getShippingMethods: function (customerKey, products, shippingAddress, billingAddress) {
                    var model = {};
                    model.CustomerKey = customerKey;
                    model.ProductKeys = products;
                    model.ShippingAddress = shippingAddress;
                    model.BillingAddress = billingAddress;

                    return umbRequestHelper.resourcePromise(
                        $http.post(umbRequestHelper.getApiUrl('merchelloOrderApiBaseUrl', 'GetShippingMethods'),
                            model
                        ),
                        'Failed to get shipping methods');
                },

                getPaymentMethods: function () {
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: umbRequestHelper.getApiUrl('merchelloOrderApiBaseUrl', 'GetPaymentMethods'),
                            method: "GET"
                        }),
                        'Failed to get payment methods');
                },

                finalizeBackofficeOrder: function (customerKey, products, shippingAddress, billingAddress, paymentKey, paymentProviderKey, shipmentKey) {
                    var model = {};
                    model.CustomerKey = customerKey;
                    model.ProductKeys = products;
                    model.ShippingAddress = shippingAddress;
                    model.BillingAddress = billingAddress;
                    model.PaymentKey = paymentKey;
                    model.PaymentProviderKey = paymentProviderKey;
                    model.ShipmentKey = shipmentKey;
                    return umbRequestHelper.resourcePromise(
                        $http.post(umbRequestHelper.getApiUrl('merchelloOrderApiBaseUrl', 'FinalizeBackofficeOrder'),
                            model
                        ),
                        'Failed to finalize backoffice order');
                }
            };

        }]);

    /**
     * @ngdoc resource
     * @name paymentResource
     * @description Loads in data and allows modification for payments
     **/
    angular.module('merchello.resources').factory('paymentResource',
        ['$q', '$http', 'umbRequestHelper',
        function($q, $http, umbRequestHelper) {

        return {

            getPayment: function (key) {
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloPaymentApiBaseUrl', 'GetPayment'),
                        method: "GET",
                        params: { id: key }
                    }),
                    'Failed to get payment: ' + key);
            },

            getPaymentMethod : function(key) {
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloPaymentApiBaseUrl', 'GetPaymentMethod'),
                        method: "GET",
                        params: { id: key }
                    }),
                    'Failed to get payment method: ' + key);
            },

            getPaymentsByInvoice: function (invoiceKey) {

                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloPaymentApiBaseUrl', 'GetPaymentsByInvoice'),
                        method: "GET",
                        params: { id: invoiceKey }
                    }),
                    'Failed to get payments by invoice: ' + invoiceKey);
            },

            authorizePayment: function (paymentRequest) {

                return umbRequestHelper.resourcePromise(
                    $http.post(umbRequestHelper.getApiUrl('merchelloPaymentApiBaseUrl', 'AuthorizePayment'),
                        paymentRequest
                    ),
                    'Failed to authorize payment');
            },

            capturePayment: function (paymentRequest) {

                return umbRequestHelper.resourcePromise(
                    $http.post(umbRequestHelper.getApiUrl('merchelloPaymentApiBaseUrl', 'CapturePayment'),
                        paymentRequest
                    ),
                    'Failed to capture payment');
            },

            authorizeCapturePayment: function (paymentRequest) {

                return umbRequestHelper.resourcePromise(
                    $http.post(umbRequestHelper.getApiUrl('merchelloPaymentApiBaseUrl', 'AuthorizeCapturePayment'),
                        paymentRequest
                    ),
                    'Failed to authorize capture payment');
            },

            refundPayment: function (paymentRequest) {

                return umbRequestHelper.resourcePromise(
                    $http.post(umbRequestHelper.getApiUrl('merchelloPaymentApiBaseUrl', 'RefundPayment'),
                        paymentRequest
                    ),
                    'Failed to refund payment');
            }
        };
    }]);

    /**
     * @ngdoc resource
     * @name paymentGatewayProviderResource
     * @description Loads in data for payment providers
     **/
    angular.module('merchello.resources').factory('paymentGatewayProviderResource',
        ['$http', 'umbRequestHelper',
        function($http, umbRequestHelper) {

            return {
                getGatewayResources: function (paymentGatewayProviderKey) {

                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: umbRequestHelper.getApiUrl('merchelloPaymentGatewayApiBaseUrl', 'GetGatewayResources'),
                            method: "GET",
                            params: {id: paymentGatewayProviderKey}
                        }),
                        'Failed to retreive gateway resource data for warehouse catalog');
                },

                getAllGatewayProviders: function () {

                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: umbRequestHelper.getApiUrl('merchelloPaymentGatewayApiBaseUrl', 'GetAllGatewayProviders'),
                            method: "GET"
                        }),
                        'Failed to retreive data for all gateway providers');
                },

                getPaymentProviderPaymentMethods: function (paymentGatewayProviderKey) {

                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: umbRequestHelper.getApiUrl('merchelloPaymentGatewayApiBaseUrl', 'GetPaymentProviderPaymentMethods'),
                            method: "GET",
                            params: {id: paymentGatewayProviderKey}
                        }),
                        'Failed to payment provider methods for: ' + paymentGatewayProviderKey);
                },

                addPaymentMethod: function (paymentMethod) {

                    return umbRequestHelper.resourcePromise(
                        $http.post(umbRequestHelper.getApiUrl('merchelloPaymentGatewayApiBaseUrl', 'AddPaymentMethod'),
                            paymentMethod
                        ),
                        'Failed to create paymentMethod');
                },

                savePaymentMethod: function (paymentMethod) {

                    return umbRequestHelper.resourcePromise(
                        $http.post(umbRequestHelper.getApiUrl('merchelloPaymentGatewayApiBaseUrl', 'PutPaymentMethod'),
                            paymentMethod
                        ),
                        'Failed to save paymentMethod');
                },

                deletePaymentMethod: function (paymentMethodKey) {

                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: umbRequestHelper.getApiUrl('merchelloPaymentGatewayApiBaseUrl', 'DeletePaymentMethod'),
                            method: "GET",
                            params: {id: paymentMethodKey}
                        }),
                        'Failed to delete paymentMethod');
                }

            };

    }]);

    /**
     * @ngdoc resource
     * @name productResource
     * @description Loads in data and allows modification of products
     **/
    angular.module('merchello.resources').factory('productResource',
        ['$q', '$http', 'umbRequestHelper',
        function($q, $http, umbRequestHelper) {

            return {

                ///////////////////////////////////////////////////////////////////////////////////////////
                /// Server http requests
                ///////////////////////////////////////////////////////////////////////////////////////////

                /**
                 * @ngdoc method
                 * @name add
                 * @description Creates a new product with an API call to the server
                 **/
                add: function (product) {

                    return umbRequestHelper.resourcePromise(
                        $http.post(umbRequestHelper.getApiUrl('merchelloProductApiBaseUrl', 'AddProduct'),
                            product
                        ),
                        'Failed to create product sku ' + product.sku);
                },

                /**
                 * @ngdoc method
                 * @name getByKey
                 * @description Gets a product with an API call to the server
                 **/
                getByKey: function (key) {

                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: umbRequestHelper.getApiUrl('merchelloProductApiBaseUrl', 'GetProduct', [{ id: key }]),
                            method: "GET"
                        }),
                        'Failed to retreive data for product key ' + key);
                },

                /**
                 * @ngdoc method
                 * @name getVariant
                 * @description Gets a product variant with an API call to the server
                 **/
                getVariant: function (key) {

                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: umbRequestHelper.getApiUrl('merchelloProductApiBaseUrl', 'GetProductVariant', [{ id: key }]),
                            method: "GET"
                        }),
                        'Failed to retreive data for product variant key ' + key);
                },

                /**
                 * @ngdoc method
                 * @name save
                 * @description Saves / updates product with an api call back to the server
                 **/
                save: function (product) {

                    return umbRequestHelper.resourcePromise(
                        $http.post(umbRequestHelper.getApiUrl('merchelloProductApiBaseUrl', 'PutProduct'),
                            product
                        ),
                        'Failed to save data for product key ' + product.key);
                },

                /**
                 * @ngdoc method
                 * @name saveVariant
                 * @description Saves / updates product variant with an api call back to the server
                 **/
                saveVariant: function (productVariant) {

                    return umbRequestHelper.resourcePromise(
                        $http.post(umbRequestHelper.getApiUrl('merchelloProductApiBaseUrl', 'PutProductVariant'),
                            productVariant
                        ),
                        'Failed to save data for product variant key ' + productVariant.key);
                },

                /**
                 * @ngdoc method
                 * @name deleteProduct
                 * @description Deletes product with an api call back to the server
                 **/
                deleteProduct: function (product) {

                    return umbRequestHelper.resourcePromise(
                        $http.post(umbRequestHelper.getApiUrl('merchelloProductApiBaseUrl', 'DeleteProduct'),
                            product.key,
                            { params: { id: product.key }}
                        ),
                        'Failed to delete product with key: ' + product.key);
                },

                /**
                 * @ngdoc method
                 * @name searchProducts
                 * @description Searches for all products with a ListQuery object
                 **/
                searchProducts: function (query) {

                    return umbRequestHelper.resourcePromise(
                        $http.post(
                            umbRequestHelper.getApiUrl('merchelloProductApiBaseUrl', 'SearchProducts'),
                            query
                        ),
                        'Failed to search products');

                },

                ///////////////////////////////////////////////////////////////////////////////////////////
                /// Business logic
                ///////////////////////////////////////////////////////////////////////////////////////////


                /**
                 * @ngdoc method
                 * @name updateProductVariant
                 * @description Saves product variant changes and delivers the new ProductVariant model in the promise data
                 **/
                updateProductVariant: function (productVariant) {
                    var deferred = $q.defer();
                    var promise = prodservice.saveVariant(productVariant);

                    promise.then(function (savedProductVariant) {

                        productVariant = new merchello.Models.ProductVariant(savedProductVariant);

                        deferred.resolve(productVariant);

                    }, function (reason) {
                        deferred.reject(reason);
                    });

                    return deferred.promise;
                }
            };
    }]);

    /**
     * @ngdoc resource
     * @name settingsResource
     * @description Loads in data and allows modification for invoices
     **/
    angular.module('merchello.resources').factory('settingsResource',
        ['$q', '$http', '$cacheFactory', 'umbRequestHelper',
            function($q, $http, $cacheFactory, umbRequestHelper) {

        /* cacheFactory instance for cached items in the merchelloSettingsService */
        var _settingsCache = $cacheFactory('merchelloSettings');

        /* helper method to get from cache or fall back to an http api call */
        function getCachedOrApi(cacheKey, apiMethod, entityName)
        {
            var deferred = $q.defer();

            var dataFromCache = _settingsCache.get(cacheKey);

            if (dataFromCache) {
                deferred.resolve(dataFromCache);
            }
            else {
                var promiseFromApi = umbRequestHelper.resourcePromise(
                    $http.get(
                        umbRequestHelper.getApiUrl('merchelloSettingsApiBaseUrl', apiMethod)
                    ),
                    'Failed to get all ' + entityName);

                promiseFromApi.then(function (dataFromApi) {
                    _settingsCache.put(cacheKey, dataFromApi);
                    deferred.resolve(dataFromApi);
                }, function (reason) {
                    deferred.reject(reason);
                });
            }

            return deferred.promise;
        }

        /**
         * @class merchelloSettingsService
         */
        var settingsServices = {

            /**
             * @ngdoc method
             * @name merchello.services.merchelloSettingsService#getAllCountries
             * @methodOf merchello.services.merchelloSettingsService
             * @function
             *
             * @description
             * Gets the supported countries / provinces from the merchello.config
             *
             * @returns {object} an angularjs promise object
             */
            getAllCountries: function () {
                return getCachedOrApi("SettingsCountries", "GetAllCountries", "countries");
            },

            /**
             * @ngdoc method
             * @name merchello.services.merchelloSettingsService#save
             * @methodOf merchello.services.merchelloSettingsService
             * @function
             *
             * @description
             * Saves or updates a store setting value
             *
             * @param {object} storeSettings StoreSettings object for the key/value pairs
             *
             * @returns {object} an angularjs promise object
             */
            save: function (storeSettings) {

                _settingsCache.remove("AllSettings");

                return umbRequestHelper.resourcePromise(
                    $http.post(
                        umbRequestHelper.getApiUrl('merchelloSettingsApiBaseUrl', 'PutSettings'),
                        storeSettings
                    ),
                    'Failed to save data for Store Settings');
            },

            /**
             * @ngdoc method
             * @name merchello.services.merchelloSettingsService#getAllSettings
             * @methodOf merchello.services.merchelloSettingsService
             * @function
             *
             * @description
             * Gets all store setting values
             *
             * @returns {object} an angularjs promise object
             */
            getAllSettings: function () {
                return getCachedOrApi("AllSettings", "GetAllSettings", "settings");
            },

            getCurrentSettings: function() {
                var deferred = $q.defer();

                var promiseArray = [];

                promiseArray.push(this.getAllSettings());

                var promise = $q.all(promiseArray);
                promise.then(function (data) {
                    deferred.resolve(data[0]);
                }, function(reason) {
                    deferred.reject(reason);
                });

                return deferred.promise;
            },

            /**
             * @ngdoc method
             * @name merchello.services.merchelloSettingsService#getAllCurrencies
             * @methodOf merchello.services.merchelloSettingsService
             * @function
             *
             * @description
             * Gets all the currencies
             *
             * @returns {object} an angularjs promise object
             */
            getAllCurrencies: function () {
                return getCachedOrApi("AllCurrency", "GetAllCurrencies", "settings");
            },

            getCurrencySymbol: function () {
                var deferred = $q.defer();

                var promiseArray = [];

                promiseArray.push(this.getAllSettings());
                promiseArray.push(this.getAllCurrencies());

                var promise = $q.all(promiseArray);
                promise.then(function (data) {
                    var settingsFromServer = data[0];
                    var currencyList =  data[1];
                    var selectedCurrency = _.find(currencyList, function (currency) {
                        return currency.currencyCode === settingsFromServer.currencyCode;
                    });

                    deferred.resolve(selectedCurrency.symbol);
                }, function (reason) {
                    deferred.reject(reason);
                });

                return deferred.promise;
            },

            /**
             * @ngdoc method
             * @name merchello.services.merchelloSettingsService#getTypeFields
             * @methodOf merchello.services.merchelloSettingsService
             * @function
             *
             * @description
             * Gets all the type fields
             *
             * @returns {object} an angularjs promise object
             */
            getTypeFields: function () {
                return getCachedOrApi("AllTypeFields", "GetTypeFields", "settings");
            }

        };

        return settingsServices;

    }]);

    /**
     * @ngdoc resource
     * @name shipmentResource
     * @description Loads in data and allows modification for shipments
     **/
    angular.module('merchello.resources').factory('shipmentResource',
        ['$http', 'umbRequestHelper', function($http, umbRequestHelper) {
        return {

            getAllShipmentStatuses: function () {

                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloShipmentApiBaseUrl', 'GetAllShipmentStatuses'),
                        method: 'GET'
                    }),
                    'Failed to get shipment statuses');

            },

            getShipment: function (key) {

                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloShipmentApiBaseUrl', 'GetShipment'),
                        method: "GET",
                        params: {id: key}
                    }),
                    'Failed to get shipment: ' + key);
            },

            getShipmentsByInvoice: function (invoice) {
                var shipmentKeys = [];

                angular.forEach(invoice.orders, function (order) {
                    var newShipmentKeys = _.map(order.items, function (orderLineItem) {
                        return orderLineItem.shipmentKey;
                    });
                    shipmentKeys = _.union(shipmentKeys, newShipmentKeys);
                });


                var shipmentKeysStr = shipmentKeys.join("&ids=");

                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloShipmentApiBaseUrl', 'GetShipments', shipmentKeysStr),
                        method: "GET",
                        params: {ids: shipmentKeys}
                    }),
                    'Failed to get shipments: ' + shipmentKeys);
            },

            getShipMethodAndAlternatives: function (shipMethodRequest) {

                return umbRequestHelper.resourcePromise(
                    $http.post(umbRequestHelper.getApiUrl('merchelloShipmentApiBaseUrl', 'SearchShipMethodAndAlternatives'),
                        shipMethodRequest
                    ),
                    'Failed to get the ship methods');
            },

            newShipment: function (shipmentRequest) {

                return umbRequestHelper.resourcePromise(
                    $http.post(umbRequestHelper.getApiUrl('merchelloShipmentApiBaseUrl', 'NewShipment'),
                        shipmentRequest
                    ),
                    'Failed to create shipment');
            },

            saveShipment: function (shipment) {
                return umbRequestHelper.resourcePromise(
                    $http.post(umbRequestHelper.getApiUrl('merchelloShipmentApiBaseUrl', 'PutShipment'),
                        shipment
                    ),
                    'Failed to save shipment');
            },

            updateShippingAddressLineItem: function(shipment) {
                return umbRequestHelper.resourcePromise(
                    $http.post(umbRequestHelper.getApiUrl('merchelloShipmentApiBaseUrl', 'UpdateShippingAddressLineItem'),
                        shipment
                    ),
                    'Failed to save shipment');
            },

            deleteShipment: function(shipment) {
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloShipmentApiBaseUrl', 'DeleteShipment'),
                        method: "GET",
                        params: { id: shipment.key }
                    }), 'Failed to delete shipment');
            }
        };
    }]);
angular.module('merchello.resources')
    .factory('shippingFixedRateProviderResource',
    ['$http', 'umbRequestHelper',
    function($http, umbRequestHelper) {

        return {

            getRateTable: function(shipMethod) {

                return umbRequestHelper.resourcePromise(
                    $http.post(umbRequestHelper.getApiUrl('merchelloFixedRateShippingApiBaseUrl', 'GetShipFixedRateTable'), shipMethod),
                    'Failed to acquire rate table');

            },

            saveRateTable: function(rateTable) {
                return umbRequestHelper.resourcePromise(
                    $http.post(umbRequestHelper.getApiUrl('merchelloFixedRateShippingApiBaseUrl', 'PutShipFixedRateTable'), rateTable),
                    'Failed to save rate table');
            }

        };

    }]);

/**
 * @ngdoc resource
 * @name shippingGatewayProviderResource
 * @description Loads in data for shipping providers and store shipping settings
 **/
angular.module('merchello.resources')
    .factory('shippingGatewayProviderResource',
    ['$http', 'umbRequestHelper',
        function($http, umbRequestHelper) {

            return {

                addShipMethod: function (shipMethod) {

                    return umbRequestHelper.resourcePromise(
                        $http.post(umbRequestHelper.getApiUrl('merchelloShippingGatewayApiBaseUrl', 'AddShipMethod'),
                            shipMethod
                        ),
                        'Failed to create ship method');
                },

                deleteShipCountry: function (shipCountryKey) {
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: umbRequestHelper.getApiUrl('merchelloShippingGatewayApiBaseUrl', 'DeleteShipCountry'),
                            method: "GET",
                            params: { id: shipCountryKey }
                        }),
                        'Failed to delete ship country');
                },

                deleteShipMethod: function (shipMethod) {
                    return umbRequestHelper.resourcePromise(
                        $http.post(umbRequestHelper.getApiUrl('merchelloShippingGatewayApiBaseUrl', 'DeleteShipMethod'),
                            shipMethod
                        ),
                        'Failed to delete ship method');
                },

                getAllShipCountryProviders: function (shipCountry) {

                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: umbRequestHelper.getApiUrl('merchelloShippingGatewayApiBaseUrl', 'GetAllShipCountryProviders'),
                            method: "GET",
                            params: { id: shipCountry.key }
                        }),
                        'Failed to retreive shipping gateway providers');
                },

                getAllShipGatewayProviders: function () {

                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: umbRequestHelper.getApiUrl('merchelloShippingGatewayApiBaseUrl', 'GetAllShipGatewayProviders'),
                            method: "GET"
                        }),
                        'Failed to retreive shipping gateway providers');
                },

                getShippingProviderShipMethods: function (shipProvider) {

                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: umbRequestHelper.getApiUrl('merchelloShippingGatewayApiBaseUrl', 'GetShippingProviderShipMethods'),
                            method: "GET",
                            params: { id: shipProvider.key }
                        }),
                        'Failed to retreive shipping methods');
                },

                getShippingGatewayMethodsByCountry: function (shipProvider, shipCountry) {

                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: umbRequestHelper.getApiUrl('merchelloShippingGatewayApiBaseUrl', 'GetShippingGatewayMethodsByCountry'),
                            method: "GET",
                            params: { id: shipProvider.key, shipCountryId: shipCountry.key }
                        }),
                        'Failed to retreive shipping methods');
                },

                getAllShipGatewayResourcesForProvider: function (shipProvider) {

                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: umbRequestHelper.getApiUrl('merchelloShippingGatewayApiBaseUrl', 'GetAllShipGatewayResourcesForProvider'),
                            method: "GET",
                            params: { id: shipProvider.key }
                        }),
                        'Failed to retreive shipping gateway provider resources');
                },

                getShippingCountry: function (id) {

                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: umbRequestHelper.getApiUrl('merchelloShippingGatewayApiBaseUrl', 'GetShipCountry'),
                            method: "GET",
                            params: { id: id }
                        }),
                        'Failed to retreive data for shipping country: ' + id);
                },

                getWarehouseCatalogShippingCountries: function (id) {

                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: umbRequestHelper.getApiUrl('merchelloShippingGatewayApiBaseUrl', 'GetAllShipCountries'),
                            method: "GET",
                            params: { id: id }
                        }),
                        'Failed to retreive shipping country data for warehouse catalog');
                },

                newWarehouseCatalogShippingCountry: function (catalogKey, countryCode) {

                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: umbRequestHelper.getApiUrl('merchelloShippingGatewayApiBaseUrl', 'NewShipCountry'),
                            method: "GET",
                            params: { catalogKey: catalogKey, countryCode: countryCode }
                        }),
                        'Failed to create ship country: ' + countryCode);
                },

                saveShipMethod: function (shipMethod) {

                    if (shipMethod.key === '') {
                        return this.addShipMethod(shipMethod);
                    }

                    return umbRequestHelper.resourcePromise(
                        $http.post(umbRequestHelper.getApiUrl('merchelloShippingGatewayApiBaseUrl', 'PutShipMethod'),
                            shipMethod
                        ),
                        'Failed to save ship method');
                }

            };
        }]);

/**
 * @ngdoc resource
 * @name taxationGatewayProviderResource
 * @description Loads in data for taxation providers
 **/
angular.module('merchello.resources').factory('taxationGatewayProviderResource',
    ['$http', 'umbRequestHelper',
    function($http, umbRequestHelper) {
    return {
        getGatewayResources: function (taxationGatewayProviderKey) {

            return umbRequestHelper.resourcePromise(
                $http({
                    url: umbRequestHelper.getApiUrl('merchelloTaxationGatewayApiBaseUrl', 'GetGatewayResources'),
                    method: "GET",
                    params: {id: taxationGatewayProviderKey}
                }),
                'Failed to retreive gateway resource data for warehouse catalog');
        },

        getAllGatewayProviders: function () {

            return umbRequestHelper.resourcePromise(
                $http({
                    url: umbRequestHelper.getApiUrl('merchelloTaxationGatewayApiBaseUrl', 'GetAllGatewayProviders'),
                    method: "GET"
                }),
                'Failed to retreive data for all gateway providers');
        },

        getTaxationProviderTaxMethods: function (taxationGatewayProviderKey) {

            return umbRequestHelper.resourcePromise(
                $http({
                    url: umbRequestHelper.getApiUrl('merchelloTaxationGatewayApiBaseUrl', 'GetTaxationProviderTaxMethods'),
                    method: "GET",
                    params: {id: taxationGatewayProviderKey}
                }),
                'Failed to tax provider methods for: ' + taxationGatewayProviderKey);
        },

        addTaxMethod: function (taxMethod) {

            return umbRequestHelper.resourcePromise(
                $http.post(umbRequestHelper.getApiUrl('merchelloTaxationGatewayApiBaseUrl', 'AddTaxMethod'),
                    taxMethod
                ),
                'Failed to create taxMethod');
        },

        saveTaxMethod: function (taxMethod) {

            return umbRequestHelper.resourcePromise(
                $http.post(umbRequestHelper.getApiUrl('merchelloTaxationGatewayApiBaseUrl', 'PutTaxMethod'),
                    taxMethod
                ),
                'Failed to save taxMethod');
        },

        deleteTaxMethod: function (taxMethodKey) {

            return umbRequestHelper.resourcePromise(
                $http({
                    url: umbRequestHelper.getApiUrl('merchelloTaxationGatewayApiBaseUrl', 'DeleteTaxMethod'),
                    method: "GET",
                    params: {id: taxMethodKey}
                }),
                'Failed to delete tax method');
        }
    };
}]);

    /**
     * @ngdoc resource
     * @name warehouseResource
     * @description Loads in data and allows modification of warehouses
     **/
    angular.module('merchello.resources').factory('warehouseResource',
        ['$q', '$http', '$cacheFactory', 'umbDataFormatter', 'umbRequestHelper',
        function($q, $http, $cacheFactory, umbDataFormatter, umbRequestHelper) {

            /* cacheFactory instance for cached items in the merchelloWarehouseService */
            var _warehouseCache = $cacheFactory('merchelloWarehouse');

            /* helper method to get from cache or fall back to an http api call */
            function getCachedOrApi(cacheKey, apiMethod, entityName) {
                var deferred = $q.defer();

                var dataFromCache = _warehouseCache.get(cacheKey);

                if (dataFromCache) {
                    deferred.resolve(dataFromCache);
                }
                else {
                    var promiseFromApi = umbRequestHelper.resourcePromise(
                        $http.get(
                            umbRequestHelper.getApiUrl('merchelloWarehouseApiBaseUrl', apiMethod)
                        ),
                        'Failed to get ' + entityName);

                    promiseFromApi.then(function (dataFromApi) {
                        _warehouseCache.put(cacheKey, dataFromApi);
                        deferred.resolve(dataFromApi);
                    }, function (reason) {
                        deferred.reject(reason);
                    });
                }

                return deferred.promise;
            }

            return {

                /**
                 * @ngdoc method
                 * @name addWarehouseCatalog
                 * @function
                 *
                 * @description
                 * Posts a new warehouse catalog to the API.
                 **/
                addWarehouseCatalog: function (catalog) {
                    return umbRequestHelper.resourcePromise(
                        $http.post(umbRequestHelper.getApiUrl('merchelloWarehouseApiBaseUrl', 'AddWarehouseCatalog'), catalog),
                        'Failed to add warehouse catalog');
                },

                /**
                 * @ngdoc method
                 * @name deleteWarehouseCatalog
                 * @function
                 *
                 * @description
                 * Deletes a warehouse catalog in the API.
                 **/
                deleteWarehouseCatalog: function (key) {
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: umbRequestHelper.getApiUrl('merchelloWarehouseApiBaseUrl', 'DeleteWarehouseCatalog'),
                            method: 'GET',
                            params: { id: key }
                        }),
                        'Failed to delete warehouse catalog with key: ' + key);
                },

                /**
                 * @ngdoc method
                 * @name getById
                 * @function
                 *
                 * @description
                 * Gets a Warehouse from the API by its id.
                 **/
                getById: function (id) {
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: umbRequestHelper.getApiUrl('merchelloWarehouseApiBaseUrl', 'GetWarehouse'),
                            method: "GET",
                            params: { id: id }
                        }),
                        'Failed to retreive data for warehouse: ' + id);
                },

                /**
                 * @ngdoc method
                 * @name getDefaultWarehouse
                 * @function
                 *
                 * @description Gets the default warehouse from the API.
                 **/
                getDefaultWarehouse: function () {

                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: umbRequestHelper.getApiUrl('merchelloWarehouseApiBaseUrl', 'GetDefaultWarehouse'),
                            method: "GET"
                        }),
                        'Failed to retreive data for default warehouse');
                },

                /**
                 * @ngdoc method
                 * @name getWarehouseCatalogs
                 * @function
                 *
                 * @description
                 * Gets the catalogs from the warehouse with the given warehouse key.
                 **/
                getWarehouseCatalogs: function (key) {
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: umbRequestHelper.getApiUrl('merchelloWarehouseApiBaseUrl', 'GetWarehouseCatalgos'),
                            method: 'GET',
                            params: { id: key }
                        }),
                        'Failed to get catalogs for warehouse: ' + key);
                },

                /**
                 * @ngdoc method
                 * @name putWarehouseCatalog
                 * @function
                 *
                 * @description
                 * Updates a warehouse catalog in the API.
                 **/
                putWarehouseCatalog: function (catalog) {
                    return umbRequestHelper.resourcePromise(
                        $http.post(umbRequestHelper.getApiUrl('merchelloWarehouseApiBaseUrl', 'PutWarehouseCatalog'), catalog),
                        'Failed to update warehouse catalog');
                },

                /**
                 * @ngdoc method
                 * @name save
                 * @function
                 *
                 * @description
                 * Saves the provided warehouse to the API.
                 **/
                save: function (warehouse) {

                    _warehouseCache.remove("DefaultWarehouse");

                    return umbRequestHelper.resourcePromise(
                        $http.post(
                            umbRequestHelper.getApiUrl('merchelloWarehouseApiBaseUrl', 'PutWarehouse'),
                            warehouse
                        ),
                        'Failed to save data for warehouse: ' + warehouse.id);
                },

            };

        }]);

})();