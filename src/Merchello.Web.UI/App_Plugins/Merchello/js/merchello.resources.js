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
            getByEntityKey: function(key) {
                return umbRequestHelper.resourcePromise(
                $http({
                    url: umbRequestHelper.getApiUrl('merchelloAuditLogApiBaseUrl', 'GetByEntityKey'),
                    method: "GET",
                    params: { id: key }
                }),
                'Failed to audit logs for entity with following key: ' + key);
            },

            /**
             * @ngdoc method
             * @name getSalesHistoryByInvoiceKey
             * @description
             **/
            getSalesHistoryByInvoiceKey: function (key) {
                var url = Umbraco.Sys.ServerVariables["merchelloUrls"]["merchelloAuditLogApiBaseUrl"] + 'GetSalesHistoryByInvoiceKey';
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: url,
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

/**
 * @ngdoc controller
 * @name detachedContentResource
 * @function
 *
 * @description
 * Handles the detached content API
 */
angular.module('merchello.resources').factory('detachedContentResource',
    ['$http', 'umbRequestHelper',
    function($http, umbRequestHelper) {

        var baseUrl = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloDetachedContentApiBaseUrl'];

        return {
            getAllLanguages: function() {
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: baseUrl + 'GetAllLanguages',
                        method: "GET"
                    }),
                    'Failed to get Umbraco languages');
            },
            getContentTypes: function() {
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: baseUrl + 'GetContentTypes',
                        method: "GET"
                    }),
                    'Failed to get Umbraco content types');
            },
            getDetachedContentTypeByEntityType: function(enumValue) {
                var url = baseUrl + 'GetDetachedContentTypesByEntityType';
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: url,
                        method: "GET",
                        params: { enumValue: enumValue}
                    }),
                    'Failed to get detached content types');
            },
            addDetachedContentType : function(detachedContentType) {
                var url = baseUrl + 'PostAddDetachedContentType';
                return umbRequestHelper.resourcePromise(
                    $http.post(url,
                        detachedContentType
                    ),
                    'Failed to add a detached content type');
            },
            saveDetachedContentType: function(detachedContentType) {
                var url = baseUrl + 'PutSaveDetachedContentType';
                return umbRequestHelper.resourcePromise(
                    $http.post(url,
                        detachedContentType
                    ),
                    'Failed to save detached content type');
            },
            deleteDetachedContentType: function(key) {
                var url = baseUrl + 'DeleteDetachedContentType';
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: url,
                        method: "GET",
                        params: { key : key }
                    }),
                    'Failed to delete detached content type');
            }
        };
}]);

/**
 * @ngdoc controller
 * @name entityCollectionResource
 * @function
 *
 * @description
 * Handles entity collection API
 */
angular.module('merchello.resources').factory('entityCollectionResource',
    ['$http', 'umbRequestHelper',
        function($http, umbRequestHelper) {

            var baseUrl = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloEntityCollectionApiBaseUrl'];

            return {
                getByKey : function(key) {
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: baseUrl + 'GetByKey',
                            method: "GET",
                            params: { key: key }
                        }),
                        'Failed to get entity collection by key');
                },
                getSortableProviderKeys : function() {
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: baseUrl + 'GetSortableProviderKeys',
                            method: "GET"
                        }),
                        'Failed to get valid sortable provider keys');
                },
                getRootCollectionsByEntityType : function(entityType) {
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: baseUrl + 'GetRootEntityCollections',
                            method: "GET",
                            params: { entityType: entityType }
                        }),
                        'Failed to get entity collection by the entity type');
                },
                getChildEntityCollections : function(parentKey) {
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: baseUrl + 'GetChildEntityCollections',
                            method: "GET",
                            params: { parentKey: parentKey }
                        }),
                        'Failed to get entity collection by the parentKey');
                },
                getEntityCollectionsByEntity : function (entity, entityType) {
                    var url = baseUrl + 'PostGetEntityCollectionsByEntity';
                    return umbRequestHelper.resourcePromise(
                        $http.post(url,
                            { key: entity.key, entityType: entityType }
                        ),
                        'Failed to get entity collections for entity');
                },
                getDefaultEntityCollectionProviders : function() {
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: baseUrl + 'GetDefaultEntityCollectionProviders',
                            method: "GET"
                        }),
                        'Failed to get default entity collection providers');
                },
                getEntityCollectionProviders : function() {
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: baseUrl + 'GetEntityCollectionProviders',
                            method: "GET"
                        }),
                        'Failed to get entity collection providers');
                },
                addEntityCollection : function(entityCollection) {
                    var url = baseUrl + 'PostAddEntityCollection';
                    return umbRequestHelper.resourcePromise(
                        $http.post(url,
                            entityCollection
                        ),
                        'Failed to add an entity collection');
                },
                saveEntityCollection : function(collection) {
                    var url = baseUrl + 'PutEntityCollection';
                    return umbRequestHelper.resourcePromise(
                        $http.post(url,
                            collection
                        ),
                        'Failed to save an entity collection');
                },
                addEntityToCollections: function(entityKey, collectionKeys) {
                    var url = baseUrl + 'PostAddEntityToCollections';
                    var data = [];
                    angular.forEach(collectionKeys, function(ck) {
                      data.push({ entityKey: entityKey, collectionKey: ck })
                    });
                    return umbRequestHelper.resourcePromise(
                        $http.post(url,
                            data
                        ),
                        'Failed to add an entity to a collection');
                },
                addEntityToCollection : function(entityKey, collectionKey) {
                    var url = baseUrl + 'PostAddEntityToCollection';
                    return umbRequestHelper.resourcePromise(
                        $http.post(url,
                            { entityKey: entityKey, collectionKey: collectionKey }
                        ),
                        'Failed to add an entity to a collection');
                },
                removeEntityFromCollections : function(entityKey, collectionKeys) {
                    var url = baseUrl + 'DeleteEntityFromCollections';
                    var data = [];
                    angular.forEach(collectionKeys, function(ck) {
                        data.push({ entityKey: entityKey, collectionKey: ck })
                    });
                    return umbRequestHelper.resourcePromise(
                        $http.post(url,
                           data
                        ),
                        'Failed to remove an entity from a collection');
                },
                removeEntityFromCollection : function(entityKey, collectionKey) {
                    var url = baseUrl + 'DeleteEntityFromCollection';
                    return umbRequestHelper.resourcePromise(
                        $http.post(url,
                            { entityKey: entityKey, collectionKey: collectionKey }
                        ),
                        'Failed to remove an entity from a collection');
                },
                getCollectionEntities : function(query) {
                    var url = baseUrl + 'PostGetCollectionEntities';
                    return umbRequestHelper.resourcePromise(
                        $http.post(
                            url,
                            query
                        ),
                        'Failed to get colleciton entities');
                },
                getEntitiesNotInCollection: function(query) {
                    var url = baseUrl + 'PostGetEntitiesNotInCollection';
                    return umbRequestHelper.resourcePromise(
                        $http.post(
                            url,
                            query
                        ),
                        'Failed to get colleciton entities');
                },
                updateSortOrders : function(entityCollections) {
                    var url = baseUrl + 'PutUpdateSortOrders';
                    return umbRequestHelper.resourcePromise(
                        $http.post(url,
                            entityCollections
                        ),
                        'Failed to update sort orders');
                },
                deleteEntityCollection: function(key) {
                    var url = baseUrl + 'DeleteEntityCollection';
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: url,
                            method: "GET",
                            params: { key: key }
                        }),
                        'Failed to delete the entity collection');
                }

            }

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
                var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloGatewayProviderApiBaseUrl'] + 'GetGatewayProvider';
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: url,
                        method: "GET",
                        params: { id: providerKey }
                    }),
                    'Failed to retreive gateway provider data');
            },

            getResolvedNotificationGatewayProviders: function () {
                var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloGatewayProviderApiBaseUrl'] + 'GetResolvedNotificationGatewayProviders';
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: url,
                        method: "GET"
                    }),
                    'Failed to retrieve data for all resolved notification gateway providers');
            },

            getResolvedPaymentGatewayProviders: function () {
                var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloGatewayProviderApiBaseUrl'] + 'GetResolvedPaymentGatewayProviders';
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: url,
                        method: "GET"
                    }),
                    'Failed to retreive data for all resolved payment gateway providers');
            },

            getResolvedShippingGatewayProviders: function () {
                var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloGatewayProviderApiBaseUrl'] + 'GetResolvedShippingGatewayProviders';
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: url,
                        method: "GET"
                    }),
                    'Failed to retreive data for all resolved shipping gateway providers');
            },

            getResolvedTaxationGatewayProviders: function () {
                var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloGatewayProviderApiBaseUrl'] + 'GetResolvedTaxationGatewayProviders';
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: url,
                        method: "GET"
                    }),
                    'Failed to retreive data for all resolved taxation gateway providers');
            },

            activateGatewayProvider: function (gatewayProvider) {
                var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloGatewayProviderApiBaseUrl'] + 'ActivateGatewayProvider';
                return umbRequestHelper.resourcePromise(
                    $http.post(url,
                        gatewayProvider
                    ),
                    'Failed to activate gateway provider');
            },

            deactivateGatewayProvider: function (gatewayProvider) {
                var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloGatewayProviderApiBaseUrl'] + 'DeactivateGatewayProvider';
                return umbRequestHelper.resourcePromise(
                    $http.post(url,
                        gatewayProvider
                    ),
                    'Failed to deactivate gateway provider');
            },

            saveGatewayProvider: function(gatewayProvider) {
                // we need to hack the extended data here
                var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloGatewayProviderApiBaseUrl'] + 'PutGatewayProvider';
                gatewayProvider.extendedData = gatewayProvider.extendedData.toArray();
                return umbRequestHelper.resourcePromise(
                    $http.post(url,
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

                var baseUrl = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloInvoiceApiBaseUrl'];

                return {

                    /**
                     * @ngdoc method
                     * @name getByKey
                     * @description
                     **/
                    getByKey: function (id) {
                        var url = baseUrl + 'GetInvoice';
                        return umbRequestHelper.resourcePromise(
                            $http({
                                url: url,
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

                    searchByCustomer : function(query) {
                        var url = baseUrl + 'SearchByCustomer';
                        return umbRequestHelper.resourcePromise(
                            $http.post(url, query),
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
                        var url = baseUrl + 'SearchInvoices';
                        return umbRequestHelper.resourcePromise(
                            $http.post(url, query),
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
                        var url = baseUrl + 'SearchByDateRange';
                        return umbRequestHelper.resourcePromise(
                            $http.post(url, query),
                            'Failed to retreive invoices');
                    },

                    /**
                     * @ngdoc method
                     * @name saveInvoice
                     * @description
                     **/
                    saveInvoice: function (invoice) {
                        var url = baseUrl + 'PutInvoice';
                        return umbRequestHelper.resourcePromise(
                            $http.post(url,
                                invoice
                            ),
                            'Failed to save invoice');
                    },

                    saveInvoiceShippingAddress: function (data) {
                        var url = baseUrl + 'PutInvoiceShippingAddress';
                        return umbRequestHelper.resourcePromise(
                            $http.post(url,
                                data
                            ),
                            'Failed to save invoice');
                    },

                    /**
                     * @ngdoc method
                     * @name deleteInvoice
                     * @description
                     **/
                    deleteInvoice: function (invoiceKey) {
                        var url = baseUrl + 'DeleteInvoice';
                        return umbRequestHelper.resourcePromise(
                            $http({
                                url: url,
                                method: "GET",
                                params: { id: invoiceKey }
                            }),
                            'Failed to delete invoice');
                    }

                };
            }]);

/**
 * @ngdoc resource
 * @name marketingResource
 * @description Loads in data and allows modification for marketing information
 **/
angular.module('merchello.resources')
    .factory('marketingResource',
       ['$http', 'umbRequestHelper',
        function($http, umbRequestHelper) {

            var baseUrl = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloMarketingApiBaseUrl'];

            function setUtcDates(offerSettings) {
                console.info(offerSettings);
            }


            return {
                getOfferProviders: function() {
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: baseUrl + 'GetOfferProviders',
                            method: "GET"
                        }),
                        'Failed to get offer providers');
                },
                getOfferSettings: function(key) {
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: baseUrl + 'GetOfferSettings',
                            method: "GET",
                            params: { id: key }
                        }),
                        'Failed to get offer settings');
                },
                searchOffers: function(query) {
                    return umbRequestHelper.resourcePromise(
                        $http.post(baseUrl + "SearchOffers",
                            query
                        ),
                        'Failed to search offers');
                },
                getAllOfferSettings: function() {
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: baseUrl + 'GetAllOfferSettings',
                            method: "GET"
                        }),
                        'Failed to get offer settings');
                },
                getAvailableOfferComponents: function(offerProviderKey) {
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: baseUrl + 'GetAvailableOfferComponents',
                            method: "GET",
                            params: { offerProviderKey: offerProviderKey}
                        }),
                        'Failed to get offer components for the provider');
                },
                checkOfferCodeIsUnique: function(offerCode) {
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: baseUrl + 'OfferCodeIsUnique',
                            method: "GET",
                            params: { offerCode: offerCode }
                        }),
                        'Failed to get offer components for the provider');
                },
                newOfferSettings: function (offerSettings) {
                    offerSettings.componentDefinitionExtendedDataToArray();

                    return umbRequestHelper.resourcePromise(
                        $http.post(baseUrl + "PostAddOfferSettings",
                            offerSettings
                        ),
                        'Failed to create offer');
                },
                saveOfferSettings: function(offerSettings) {
                    offerSettings.componentDefinitionExtendedDataToArray();
                    setUtcDates(offerSettings);
                    return umbRequestHelper.resourcePromise(
                        $http.post(baseUrl + "PutUpdateOfferSettings",
                            offerSettings
                        ),
                        'Failed to create offer');
                },
                deleteOfferSettings: function(offerSettings) {
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: baseUrl + 'DeleteOfferSettings',
                            method: "GET",
                            params: { id: offerSettings.key }
                        }),
                        'Failed to delete offer settings');
                }

            };
        }]);

/**
  * @ngdoc resource
  * @name noteResource
  * @description Loads in data and allows modification of notes
  **/
angular.module('merchello.resources').factory('noteResource', [
    '$http', 'umbRequestHelper',
    function ($http, umbRequestHelper) {
        return {
            /**
             * @ngdoc method
             * @name getSalesHistoryByInvoiceKey
             * @description
             **/
            getByEntityKey: function (key) {
                var url = Umbraco.Sys.ServerVariables["merchelloUrls"]["merchelloNoteApiBaseUrl"] + 'GetByEntityKey';
                return umbRequestHelper.resourcePromise(
                $http({
                    url: url,
                    method: "GET",
                    params: { id: key }
                }),
                'Failed to retrieve notes for entity with following key: ' + key);
            },


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
                        var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloNotificationApiBaseUrl'] + 'GetGatewayResources';
                        return umbRequestHelper.resourcePromise(
                            $http({
                                url: url + "?id=" + key,
                                method: "GET"
                            }),
                            'Failed to save data for Notification');
                    },

                    getAllGatewayProviders: function () {
                        var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloNotificationApiBaseUrl'] + 'GetAllGatewayProviders';
                        return umbRequestHelper.resourcePromise(
                            $http({
                                url: url,
                                method: "GET"
                            }),
                            'Failed to retreive data for all gateway providers');
                    },

                    getAllNotificationMonitors: function () {
                        var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloNotificationApiBaseUrl'] + 'GetAllNotificationMonitors';
                        return umbRequestHelper.resourcePromise(
                            $http({
                                url: url,
                                method: "GET"
                            }),
                            'Failed to retreive data for all gateway providers');
                    },

                    getNotificationProviderNotificationMethods: function (id) {
                        var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloNotificationApiBaseUrl'] + 'GetNotificationProviderNotificationMethods';
                        return umbRequestHelper.resourcePromise(
                            $http({
                                url: url + "?id=" + id,
                                method: "GET"
                            }),
                            'Failed to save data for Notification');
                    },

                    saveNotificationMethod: function (method) {
                        var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloNotificationApiBaseUrl'] + 'AddNotificationMethod';
                        return umbRequestHelper.resourcePromise(
                            $http.post(
                                url,
                                angular.toJson(method)
                            ),
                            'Failed to save data for Notification');
                    },

                    deleteNotificationMethod: function (methodKey) {
                        var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloNotificationApiBaseUrl'] + 'DeleteNotificationMethod';
                        return umbRequestHelper.resourcePromise(
                            $http({
                                url: url + "?id=" + methodKey,
                                method: "DELETE"
                            }),
                            'Failed to delete data for Notification');
                    },

                    getNotificationMessagesByKey: function (id) {
                        var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloNotificationApiBaseUrl'] + 'GetNotificationMessagesByKey';
                        return umbRequestHelper.resourcePromise(
                            $http({
                                url: url + "?id=" + id,
                                method: "GET"
                            }),
                            'Failed to save data for Notification');

                    },

                    saveNotificationMessage: function (notification) {
                        var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloNotificationApiBaseUrl'] + 'PutNotificationMessage';
                        return umbRequestHelper.resourcePromise(
                            $http.post(
                                url,
                                angular.toJson(notification)
                            ),
                            'Failed to save data for Notification');
                    },

                    deleteNotificationMessage: function (methodKey) {
                        var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloNotificationApiBaseUrl'] + 'DeleteNotificationMessage';
                        return umbRequestHelper.resourcePromise(
                            $http({
                                url: url + "?id=" + methodKey,
                                method: "DELETE"
                            }),
                            'Failed to delete data for Notification');
                    },

                    updateNotificationMessage: function (notification) {
                        var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloNotificationApiBaseUrl'] + 'UpdateNotificationMessage';
                        return umbRequestHelper.resourcePromise(
                            $http.post(
                                url,
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
                    var url = Umbraco.Sys.ServerVariables['merchello']['merchelloOrderApiBaseUrl'] + 'GetOrder';
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: url,
                            method: "GET",
                            params: { id: orderKey }
                        }),
                        'Failed to get order: ' + orderKey);
                },

                getOrdersByInvoice: function (invoiceKey) {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloOrderApiBaseUrl'] + 'GetOrdersByInvoiceKey';
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: url,
                            method: "GET",
                            params: { id: invoiceKey }
                        }),
                        'Failed to get orders by invoice: ' + invoiceKey);
                },

                getUnFulfilledItems: function (invoiceKey) {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloOrderApiBaseUrl'] + 'GetUnFulfilledItems';
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: url,
                            method: "GET",
                            params: { id: invoiceKey }
                        }),
                        'Failed to get unfulfilled items by invoice: ' + invoiceKey);
                },

                getShippingAddress: function (invoiceKey) {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloOrderApiBaseUrl'] + 'GetShippingAddress';
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: url,
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
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloOrderApiBaseUrl'] + 'ProcessesProductsToBackofficeOrder';
                    return umbRequestHelper.resourcePromise(
                        $http.post(url,
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
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloOrderApiBaseUrl'] + 'GetShippingMethods';
                    return umbRequestHelper.resourcePromise(
                        $http.post(url,
                            model
                        ),
                        'Failed to get shipping methods');
                },

                getPaymentMethods: function () {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloOrderApiBaseUrl'] + 'GetPaymentMethods';
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: url,
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
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloOrderApiBaseUrl'] + 'FinalizeBackofficeOrder';
                    return umbRequestHelper.resourcePromise(
                        $http.post(url,
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

        var baseUrl = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloPaymentApiBaseUrl'];

        return {

            getPayment: function (key) {
                var url = baseUrl + 'GetPaymeent';
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: url,
                        method: "GET",
                        params: { id: key }
                    }),
                    'Failed to get payment: ' + key);
            },

            getPaymentMethod : function(key) {
                var url = baseUrl + 'GetPaymentMethod';
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: url,
                        method: "GET",
                        params: { id: key }
                    }),
                    'Failed to get payment method: ' + key);
            },

            getPaymentsByInvoice: function (invoiceKey) {
                var url = baseUrl + 'GetPaymentsByInvoice';
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: url,
                        method: "GET",
                        params: { id: invoiceKey }
                    }),
                    'Failed to get payments by invoice: ' + invoiceKey);
            },

            authorizePayment: function (paymentRequest) {
                var url = baseUrl + 'AuthorizePayment';
                return umbRequestHelper.resourcePromise(
                    $http.post(url,
                        paymentRequest
                    ),
                    'Failed to authorize payment');
            },

            capturePayment: function (paymentRequest) {
                var url = baseUrl + 'CapturePayment';
                return umbRequestHelper.resourcePromise(
                    $http.post(url,
                        paymentRequest
                    ),
                    'Failed to capture payment');
            },

            authorizeCapturePayment: function (paymentRequest) {
                var url = baseUrl + 'AuthorizeCapturePayment';
                return umbRequestHelper.resourcePromise(
                    $http.post(url,
                        paymentRequest
                    ),
                    'Failed to authorize capture payment');
            },

            refundPayment: function (paymentRequest) {
                var url = baseUrl + 'RefundPayment';
                return umbRequestHelper.resourcePromise(
                    $http.post(url,
                        paymentRequest
                    ),
                    'Failed to refund payment');
            },

            voidPayment: function (paymentRequest) {
                return umbRequestHelper.resourcePromise(
                    $http.post(baseUrl + 'VoidPayment',
                        paymentRequest
                    ),
                    'Failed to void payment');
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

            var baseUrl = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloPaymentGatewayApiBaseUrl'];

            return {
                getGatewayResources: function (paymentGatewayProviderKey) {
                    var url = baseUrl + 'GetGatewayResources';
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: url,
                            method: "GET",
                            params: {id: paymentGatewayProviderKey}
                        }),
                        'Failed to retreive gateway resource data for warehouse catalog');
                },

                getAllGatewayProviders: function () {
                    var url = baseUrl + 'GetAllGatewayProviders';
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: url,
                            method: "GET"
                        }),
                        'Failed to retreive data for all gateway providers');
                },

                getPaymentProviderPaymentMethods: function (paymentGatewayProviderKey) {
                    var url = baseUrl + 'GetPaymentProviderPaymentMethods';
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: url,
                            method: "GET",
                            params: {id: paymentGatewayProviderKey}
                        }),
                        'Failed to payment provider methods for: ' + paymentGatewayProviderKey);
                },

                getAvailablePaymentMethods: function() {
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: baseUrl + 'GetAvailablePaymentMethods',
                            method: "GET"
                        }),
                        'Failed to load payment methods');
                },

                getPaymentMethodByKey: function(paymentMethodKey) {
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: baseUrl + 'GetPaymentMethodByKey',
                            method: "GET",
                            params: {key: paymentMethodKey}
                        }),
                        'Failed to payment method: ' + paymentMethodKey);
                },

                addPaymentMethod: function (paymentMethod) {
                    var url = baseUrl + 'AddPaymentMethod';
                    return umbRequestHelper.resourcePromise(
                        $http.post(url,
                            paymentMethod
                        ),
                        'Failed to create paymentMethod');
                },

                savePaymentMethod: function (paymentMethod) {
                    var url = baseUrl + 'PutPaymentMethod';
                    return umbRequestHelper.resourcePromise(
                        $http.post(url,
                            paymentMethod
                        ),
                        'Failed to save paymentMethod');
                },

                deletePaymentMethod: function (paymentMethodKey) {
                    var url = baseUrl + 'DeletePaymentMethod';
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: url,
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
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloProductApiBaseUrl'] + 'AddProduct';
                    return umbRequestHelper.resourcePromise(
                        $http.post(url,
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
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloProductApiBaseUrl'] + 'GetProductFromService';
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: url + '?id=' + key,
                            method: "GET"
                        }),
                        'Failed to retreive data for product key ' + key);
                },

                getByKeys: function(keys) {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloProductApiBaseUrl'] + 'GetByKeys';
                    return umbRequestHelper.resourcePromise(
                        $http.post(url,
                            keys
                        ),
                        'Failed to retreive data for product key ' + keys);
                },

                /**
                 * @ngdoc method
                 * @name getVariant
                 * @description Gets a product variant with an API call to the server
                 **/
                getVariant: function (key) {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloProductApiBaseUrl'] + 'GetProductVariant';
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: url + '?id=' + key,
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
                    angular.forEach(product.detachedContents, function(dc) {
                        dc.detachedDataValues = dc.detachedDataValues.toArray();
                    });
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloProductApiBaseUrl'] + 'PutProduct';
                    return umbRequestHelper.resourcePromise(
                        $http.post(url,
                            product
                        ),
                        'Failed to save data for product key ' + product.key);
                },

                saveProductContent: function(product, cultureName, files) {
                    angular.forEach(product.detachedContents, function(dc) {
                        dc.detachedDataValues = dc.detachedDataValues.toArray();
                    });

                    angular.forEach(product.productVariants, function(pv) {
                      if (pv.detachedContents.length > 0) {
                          angular.forEach(pv.detachedContents, function(pvdc) {
                            pvdc.detachedDataValues = pvdc.detachedDataValues.toArray();
                          });
                      }
                    });

                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloProductApiBaseUrl'] + 'PutProductWithDetachedContent';
                    var deferred = $q.defer();
                    umbRequestHelper.postMultiPartRequest(
                        url,
                        { key: "detachedContentItem", value: { display: product, cultureName: cultureName} },
                        function (data, formData) {
                            //now add all of the assigned files
                            for (var f in files) {
                                //each item has a property alias and the file object, we'll ensure that the alias is suffixed to the key
                                // so we know which property it belongs to on the server side
                                formData.append("file_" + files[f].alias, files[f].file);
                            }
                        },
                        function (data, status, headers, config) {

                            deferred.resolve(data);

                        }, function(reason) {
                            deferred.reject('Failed to save product content ' + reason)
                        });

                    return deferred.promise;

                },

                /**
                 * @ngdoc method
                 * @name saveVariant
                 * @description Saves / updates product variant with an api call back to the server
                 **/
                saveVariant: function (productVariant) {
                    angular.forEach(productVariant.detachedContents, function(dc) {
                        dc.detachedDataValues = dc.detachedDataValues.toArray();
                    });
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloProductApiBaseUrl'] + 'PutProductVariant';
                    return umbRequestHelper.resourcePromise(
                        $http.post(url,
                            productVariant
                        ),
                        'Failed to save data for product variant key ' + productVariant.key);
                },

                saveVariantContent: function(productVariant, cultureName, files) {
                    angular.forEach(productVariant.detachedContents, function(dc) {
                        dc.detachedDataValues = dc.detachedDataValues.toArray();
                    });
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloProductApiBaseUrl'] + 'PutProductVariantWithDetachedContent';

                    var deferred = $q.defer();
                    umbRequestHelper.postMultiPartRequest(
                        url,
                        { key: "detachedContentItem", value: { display: productVariant, cultureName: cultureName} },
                        function (data, formData) {
                            //now add all of the assigned files
                            for (var f in files) {
                                //each item has a property alias and the file object, we'll ensure that the alias is suffixed to the key
                                // so we know which property it belongs to on the server side
                                formData.append("file_" + files[f].alias, files[f].file);
                            }
                        },
                        function (data, status, headers, config) {
                            deferred.resolve(data);
                        }, function(reason) {
                            deferred.reject(reason);
                        });

                    return deferred.promise;
                },

                copyProduct: function(product, name, sku) {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloProductApiBaseUrl'] + 'PostCopyProduct';
                    return umbRequestHelper.resourcePromise(
                        $http.post(url,
                            { product: product, name: name, sku: sku }
                        ),
                        'Failed to delete detached content');
                },

                /**
                 * @ngdoc method
                 * @name deleteProduct
                 * @description Deletes product with an api call back to the server
                 **/
                deleteProduct: function (product) {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloProductApiBaseUrl'] + 'DeleteProduct';
                    return umbRequestHelper.resourcePromise(
                        $http.post(url,
                            product.key,
                            { params: { id: product.key }}
                        ),
                        'Failed to delete product with key: ' + product.key);
                },

                deleteDetachedContent: function(variant) {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloProductApiBaseUrl'] + 'DeleteDetachedContent';
                    return umbRequestHelper.resourcePromise(
                        $http.post(url,
                            variant
                        ),
                        'Failed to delete detached content');
                },

                /**
                 * @ngdoc method
                 * @name searchProducts
                 * @description Searches for all products with a ListQuery object
                 **/
                searchProducts: function (query) {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloProductApiBaseUrl'] + 'SearchProducts';
                    return umbRequestHelper.resourcePromise(
                        $http.post(
                            url,
                            query
                        ),
                        'Failed to search products');
                }
            };
    }]);

    /**
     * @ngdoc resource
     * @name settingsResource
     * @description Loads in data and allows modification for invoices
     **/
    angular.module('merchello.resources').factory('settingsResource',
        ['$q', '$http', '$cacheFactory', 'umbRequestHelper', 'countryDisplayBuilder', 'settingDisplayBuilder',
            function($q, $http, $cacheFactory, umbRequestHelper, countryDisplayBuilder, settingDisplayBuilder) {

        /* cacheFactory instance for cached items in the merchelloSettingsService */
        var _settingsCache = $cacheFactory('merchelloSettings');

        /* helper method to get from cache or fall back to an http api call */
        function getCachedOrApi(cacheKey, apiMethod, entityName)
        {
            var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloSettingsApiBaseUrl'] + apiMethod;
            var deferred = $q.defer();

            var dataFromCache = _settingsCache.get(cacheKey);

            if (dataFromCache) {
                deferred.resolve(dataFromCache);
            }
            else {
                var promiseFromApi = umbRequestHelper.resourcePromise(
                    $http.get(
                        url
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
             * @name getMerchelloVersion
             * @methodOf settingsResource
             * @function
             *
             * @description
             * Gets the current Merchello Version
             *
             * @returns {object} an angularjs promise object
             */
            getMerchelloVersion: function() {
                return getCachedOrApi("MerchelloVersion", "GetMerchelloVersion", "merchelloversion");
            },

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

                var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloSettingsApiBaseUrl'] + 'PutSettings';
                return umbRequestHelper.resourcePromise(
                    $http.post(
                        url,
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

            getAllCombined: function() {
                var deferred = $q.defer();
                var promises = [
                    this.getAllSettings(),
                    this.getAllCurrencies(),
                    this.getAllCountries()
                ];
                $q.all(promises).then(function(data) {
                    var result = {
                        settings: settingDisplayBuilder.transform(data[0]),
                        currencies: data[1],
                        currencySymbol: _.find(data[1], function(c) {
                            return c.currencyCode === data[0].currencyCode
                        }).symbol,
                        countries: countryDisplayBuilder.transform(data[2])
                    };
                    deferred.resolve(result);
                });

                return deferred.promise;
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
                var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloShipmentApiBaseUrl'] + 'GetAllShipmentStatuses';
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: url,
                        method: 'GET'
                    }),
                    'Failed to get shipment statuses');

            },

            getShipment: function (key) {
                var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloShipmentApiBaseUrl'] + 'GetShipment';
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: url,
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
                var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloShipmentApiBaseUrl'] + 'GetShipments';
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: url,
                        method: "GET",
                        params: {ids: shipmentKeys}
                    }),
                    'Failed to get shipments: ' + shipmentKeys);
            },

            getShipMethodAndAlternatives: function (shipMethodRequest) {
                var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloShipmentApiBaseUrl'] + 'SearchShipMethodAndAlternatives';
                return umbRequestHelper.resourcePromise(
                    $http.post(url,
                        shipMethodRequest
                    ),
                    'Failed to get the ship methods');
            },

            newShipment: function (shipmentRequest) {
                var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloShipmentApiBaseUrl'] + 'NewShipment';
                return umbRequestHelper.resourcePromise(
                    $http.post(url,
                        shipmentRequest
                    ),
                    'Failed to create shipment');
            },

            saveShipment: function (shipment) {
                var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloShipmentApiBaseUrl'] + 'PutShipment';
                return umbRequestHelper.resourcePromise(
                    $http.post(url,
                        shipment
                    ),
                    'Failed to save shipment');
            },

            updateShippingAddressLineItem: function(shipment) {
                var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloShipmentApiBaseUrl'] + 'UpdateShippingAddressLineItem';
                return umbRequestHelper.resourcePromise(
                    $http.post(url,
                        shipment
                    ),
                    'Failed to save shipment');
            },

            deleteShipment: function(shipment) {
                var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloShipmentApiBaseUrl'] + 'DeleteShipment';
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: url,
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
                var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloFixedRateShippingApiBaseUrl'] + 'GetShipFixedRateTable';
                return umbRequestHelper.resourcePromise(
                    $http.post(url, shipMethod),
                    'Failed to acquire rate table');

            },

            saveRateTable: function(rateTable) {
                var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloFixedRateShippingApiBaseUrl'] + 'PutShipFixedRateTable';
                return umbRequestHelper.resourcePromise(
                    $http.post(url, rateTable),
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
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloShippingGatewayApiBaseUrl'] + 'AddShipMethod';
                    return umbRequestHelper.resourcePromise(
                        $http.post(url,
                            shipMethod
                        ),
                        'Failed to create ship method');
                },

                deleteShipCountry: function (shipCountryKey) {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloShippingGatewayApiBaseUrl'] + 'DeleteShipCountry';
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: url,
                            method: "GET",
                            params: { id: shipCountryKey }
                        }),
                        'Failed to delete ship country');
                },

                deleteShipMethod: function (shipMethod) {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloShippingGatewayApiBaseUrl'] + 'DeleteShipMethod';
                    return umbRequestHelper.resourcePromise(
                        $http.post(url,
                            shipMethod
                        ),
                        'Failed to delete ship method');
                },

                getAllShipCountryProviders: function (shipCountry) {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloShippingGatewayApiBaseUrl'] + 'GetAllShipCountryProviders';
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: url,
                            method: "GET",
                            params: { id: shipCountry.key }
                        }),
                        'Failed to retreive shipping gateway providers');
                },

                getAllShipGatewayProviders: function () {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloShippingGatewayApiBaseUrl'] + 'GetAllShipGatewayProviders';
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: url,
                            method: "GET"
                        }),
                        'Failed to retreive shipping gateway providers');
                },

                getShippingProviderShipMethods: function (shipProvider) {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloShippingGatewayApiBaseUrl'] + 'GetShippingProviderShipMethods';
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: url,
                            method: "GET",
                            params: { id: shipProvider.key }
                        }),
                        'Failed to retreive shipping methods');
                },

                getShippingGatewayMethodsByCountry: function (shipProvider, shipCountry) {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloShippingGatewayApiBaseUrl'] + 'GetShippingGatewayMethodsByCountry';
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: url,
                            method: "GET",
                            params: { id: shipProvider.key, shipCountryId: shipCountry.key }
                        }),
                        'Failed to retreive shipping methods');
                },

                getAllShipGatewayResourcesForProvider: function (shipProvider) {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloShippingGatewayApiBaseUrl'] + 'GetAllShipGatewayResourcesForProvider';
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: url,
                            method: "GET",
                            params: { id: shipProvider.key }
                        }),
                        'Failed to retreive shipping gateway provider resources');
                },

                getShippingCountry: function (id) {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloShippingGatewayApiBaseUrl'] + 'GetShipCountry';
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: url,
                            method: "GET",
                            params: { id: id }
                        }),
                        'Failed to retreive data for shipping country: ' + id);
                },

                getWarehouseCatalogShippingCountries: function (id) {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloShippingGatewayApiBaseUrl'] + 'GetAllShipCountries';
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: url,
                            method: "GET",
                            params: { id: id }
                        }),
                        'Failed to retreive shipping country data for warehouse catalog');
                },

                newWarehouseCatalogShippingCountry: function (catalogKey, countryCode) {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloShippingGatewayApiBaseUrl'] + 'NewShipCountry';
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: url,
                            method: "GET",
                            params: { catalogKey: catalogKey, countryCode: countryCode }
                        }),
                        'Failed to create ship country: ' + countryCode);
                },

                saveShipMethod: function (shipMethod) {

                    if (shipMethod.key === '') {
                        return this.addShipMethod(shipMethod);
                    }
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloShippingGatewayApiBaseUrl'] + 'PutShipMethod';
                    return umbRequestHelper.resourcePromise(
                        $http.post(url,
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
            var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloTaxationGatewayApiBaseUrl'] + 'GetGatewayResources';
            return umbRequestHelper.resourcePromise(
                $http({
                    url: url,
                    method: "GET",
                    params: {id: taxationGatewayProviderKey}
                }),
                'Failed to retreive gateway resource data for warehouse catalog');
        },

        getAllGatewayProviders: function () {
            var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloTaxationGatewayApiBaseUrl'] + 'GetAllGatewayProviders';
            return umbRequestHelper.resourcePromise(
                $http({
                    url: url,
                    method: "GET"
                }),
                'Failed to retreive data for all gateway providers');
        },

        getTaxationProviderTaxMethods: function (taxationGatewayProviderKey) {
            var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloTaxationGatewayApiBaseUrl'] + 'GetTaxationProviderTaxMethods';
            return umbRequestHelper.resourcePromise(
                $http({
                    url: url,
                    method: "GET",
                    params: {id: taxationGatewayProviderKey}
                }),
                'Failed to tax provider methods for: ' + taxationGatewayProviderKey);
        },

        addTaxMethod: function (taxMethod) {
            var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloTaxationGatewayApiBaseUrl'] + 'AddTaxMethod';
            return umbRequestHelper.resourcePromise(
                $http.post(url,
                    taxMethod
                ),
                'Failed to create taxMethod');
        },

        saveTaxMethod: function (taxMethod) {
            var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloTaxationGatewayApiBaseUrl'] + 'PutTaxMethod';
            return umbRequestHelper.resourcePromise(
                $http.post(url,
                    taxMethod
                ),
                'Failed to save taxMethod');
        },

        deleteTaxMethod: function (taxMethodKey) {
            var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloTaxationGatewayApiBaseUrl'] + 'DeleteTaxMethod';
            return umbRequestHelper.resourcePromise(
                $http({
                    url: url,
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
                var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloWarehouseApiBaseUrl'] + apiMethod;
                var dataFromCache = _warehouseCache.get(cacheKey);

                if (dataFromCache) {
                    deferred.resolve(dataFromCache);
                }
                else {
                    var promiseFromApi = umbRequestHelper.resourcePromise(
                        $http.get(
                            url
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
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloWarehouseApiBaseUrl'] + 'AddWarehouseCatalog';
                    return umbRequestHelper.resourcePromise(
                        $http.post(url, catalog),
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
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloWarehouseApiBaseUrl'] + 'DeleteWarehouseCatalog';
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: url,
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
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloWarehouseApiBaseUrl'] + 'GetWarehouse';
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: url,
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
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloWarehouseApiBaseUrl'] + 'GetDefaultWarehouse';
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: url,
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
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloWarehouseApiBaseUrl'] + 'GetWarehouseCatalogs';
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: url,
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
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloWarehouseApiBaseUrl'] + 'PutWarehouseCatalog';
                    return umbRequestHelper.resourcePromise(
                        $http.post(url, catalog),
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
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloWarehouseApiBaseUrl'] + 'PutWarehouse';
                    _warehouseCache.remove("DefaultWarehouse");

                    return umbRequestHelper.resourcePromise(
                        $http.post(
                            url,
                            warehouse
                        ),
                        'Failed to save data for warehouse: ' + warehouse.id);
                },

            };

        }]);

})();