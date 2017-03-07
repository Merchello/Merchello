/*! Merchello
 * https://github.com/meritage/Merchello
 * Copyright (c) 2017 Across the Pond, LLC.
 * Licensed MIT
 */

(function() { 

angular.module('merchello.services').service('merchDateHelper', [
    '$q', 'localizationService',
    function($q, localizationService) {

        this.convertToJsDate = function(dateString, dateFormat) {
            // date formats in merchello start with MM, dd, or yyyy
            if(dateString.indexOf('/') === -1) {
                dateString = dateString.replace(/-|\./g, '/');
            }
            var splitDate = dateString.split('/');
            var date;
            switch (dateFormat) {
                case 'MM-dd-yyyy':
                    splitDate[0] = (splitDate[0] * 1) - 1;
                    date = new Date(splitDate[2], splitDate[0], splitDate[1], 0, 0, 0);
                    break;
                case 'dd-MM-yyyy':
                    splitDate[1] = (splitDate[1] * 1) - 1;
                    date = new Date(splitDate[2], splitDate[1], splitDate[0], 0, 0, 0);
                    break;
                default:
                    splitDate[1] = (splitDate[1] * 1) - 1;
                    date = new Date(splitDate[0], splitDate[1], splitDate[2], 0, 0, 0);
                    break;
            }

            return date;
        },

        this.convertToIsoDate = function(dateString, dateFormat) {

            var date = this.convertToJsDate(dateString, dateFormat);
            return date.toISOString();
        };

        this.getLocalizedDaysOfWeek = function() {

            var deferred = $q.defer();
            $q.all([
                localizationService.localize('merchelloGeneral_sunday'),
                localizationService.localize('merchelloGeneral_monday'),
                localizationService.localize('merchelloGeneral_tuesday'),
                localizationService.localize('merchelloGeneral_wednesday'),
                localizationService.localize('merchelloGeneral_thursday'),
                localizationService.localize('merchelloGeneral_friday'),
                localizationService.localize('merchelloGeneral_saturday')
            ]).then(function(weekdays) {
                deferred.resolve(weekdays);
            });

            return deferred.promise;
        };

        this.getGmt0EquivalentDate = function(dt) {
           return new Date(dt.getTime() + (dt.getTimezoneOffset() * 60000));
        };
}]);

angular.module('merchello.services').factory('detachedContentHelper',
    ['$q', 'fileManager', 'formHelper',
    function($q, fileManager, formHelper) {

        function validate(args) {
            if (!angular.isObject(args)) {
                throw "args must be an object";
            }
            if (!args.scope) {
                throw "args.scope is not defined";
            }
            if (!args.content) {
                throw "args.content is not defined";
            }
            if (!args.statusMessage) {
                throw "args.statusMessage is not defined";
            }
            if (!args.saveMethod) {
                throw "args.saveMethod is not defined";
            }
        }

        return {

            attributeContentPerformSave: function(args) {
                validate(args);

                var deferred = $q.defer();
                if (args.scope.preValuesLoaded && formHelper.submitForm({ scope: args.scope, statusMessage: args.statusMessage })) {
                    args.scope.preValuesLoaded = false;

                    // get any files from the fileManager
                    var files = fileManager.getFiles();

                    angular.forEach(args.scope.contentTabs, function(ct) {
                        angular.forEach(ct.properties, function (p) {
                            if (typeof p.value !== "function") {
                                args.scope.dialogData.choice.detachedDataValues.setValue(p.alias, angular.toJson(p.value));
                            }
                        });
                    });

                    args.saveMethod(args, files).then(function(data) {

                        formHelper.resetForm({ scope: args.scope, notifications: data.notifications });
                        args.scope.preValuesLoaded = true;
                        deferred.resolve(data);

                    }, function (err) {

                        args.scope.preValuesLoaded = true;
                        deferred.reject(err);
                    });

                } else {
                    deferred.reject();
                }

                return deferred.promise;
            },

            detachedContentPerformSave: function(args) {

                validate(args);

                var deferred = $q.defer();
                if (args.scope.preValuesLoaded && formHelper.submitForm({ scope: args.scope, statusMessage: args.statusMessage })) {
                    args.scope.preValuesLoaded = false;

                    // get any files from the fileManager
                    var files = fileManager.getFiles();

                    // save the current language only
                    angular.forEach(args.scope.contentTabs, function(ct) {
                        if (ct.id === 'render') {
                            args.scope.detachedContent.slug = _.find(ct.properties, function(s) { return s.alias === 'slug'; }).value;
                            args.scope.detachedContent.templateId = _.find(ct.properties, function(t) { return t.alias === 'templateId'; }).value;
                            args.scope.detachedContent.canBeRendered = _.find(ct.properties, function(r) { return r.alias === 'canBeRendered'; }).value === '1' ? true : false;
                        } else {
                            angular.forEach(ct.properties, function (p) {
                                if (typeof p.value !== "function") {
                                    args.scope.detachedContent.detachedDataValues.setValue(p.alias, angular.toJson(p.value));
                                }
                            });
                        }
                    });

                    //args.saveMethod(args.content, args.scope.language.isoCode, files)
                    args.saveMethod(args, files).then(function(data) {
                        
                        formHelper.resetForm({ scope: args.scope, notifications: data.notifications });
                        args.scope.preValuesLoaded = true;
                        deferred.resolve(data);

                    }, function (err) {

                        args.scope.preValuesLoaded = true;
                        deferred.reject(err);
                    });
                } else {
                    deferred.reject();
                }

                return deferred.promise;
            },

            buildRenderTab: function(args) {
                var items = [];
                var i = 1;
                _.each(args.allowedTemplates, function(t) {
                  items.push({ id: t.id, sortOrder: i, value: t.name });
                    i++;
                });

                var tab = {
                    alias: args.tabAlias,
                    label: args.tabLabel,
                    id: args.tabId,
                    properties: [
                        {
                            alias: 'slug',
                            description: args.slugDescription,
                            editor: 'Umbraco.Textbox',
                            hideLabel: false,
                            label: args.slugLabel,
                            validation: {
                                mandatory: true
                            },
                            value: args.slug,
                            view: 'textbox'
                        },
                        {
                            alias: 'templateId',
                            editor: 'Umbraco.DropDown',
                            hideLabel: false,
                            label: args.templateLabel,
                            config: {
                                items: items
                            },
                            description: '',
                            value: args.templateId === 0 ? args.defaultTemplateId : args.templateId,
                            validation: {
                                mandatory: false
                            },
                            view: 'dropdown'
                        },
                        {
                            alias: 'canBeRendered',
                            editor: 'Umbraco.TrueFalse',
                            description: '',
                            label: args.canBeRenderedLabel,
                            hideLabel: false,
                            value: args.canBeRendered ? '1' : '0',
                            view: 'boolean',
                            validation: {
                                mandatory: false
                            }
                        }
                    ]
                };

                return tab;
            }

        };

}]);

angular.module('merchello.services').service('entityCollectionHelper',
    [
        function() {

            this.getEntityTypeByTreeId = function(id) {
                switch(id) {
                    case "products":
                        return "Product";
                    case "sales":
                        return "Invoice";
                    case "customers":
                        return "Customer";
                    default :
                        return "EntityCollection";
                }
            };
        }]);



/**
 * @ngdoc service
 * @name invoiceHelper
 * @description Helper functions for an invoice.
 **/
angular.module('merchello.services').service('invoiceHelper',
    [
    function() {

        /**
         * @ngdoc method
         * @name getTotalsByCurrencyCode
         * @function
         *
         * @description
         * Totals a collection of invoices by currency code.
         */
        // TODO this should be moved to a prototype method for consistency
        this.getTotalsByCurrencyCode = function(invoices) {
            var self = this;
            var totals = [];
            angular.forEach(invoices, function(inv) {
                var cc = inv.getCurrencyCode();
                var total = self.round(inv.total, 2);
                var existing = _.find(totals, function(t) { return t.currencyCode === cc; });
                if (existing === null || existing === undefined) {
                    totals.push({ currencyCode: cc, total: total });
                } else {
                    existing.total += total;
                }
            });
            return _.sortBy(totals, function(o) { return o.currencyCode; });
        },

        /**
         * @ngdoc method
         * @name round
         * @function
         *
         * @description
         * Rounds a decimal to a specific number of places.
         */
        this.round = function(num, places) {
            var rounded = +(Math.round(num + "e+" + places) + "e-" + places);
            return isNaN(rounded) ? 0 : rounded;
        },

        /**
         * @ngdoc method
         * @name valueIsInRange
         * @function
         *
         * @description
         * Verifies a value is within a range of values.
         */
        this.valueIsInRage = function(str,min, max) {
            n = parseFloat(str);
            return (!isNaN(n) && n >= min && n <= max);
        },

        this.padLeft = function(str, char, num) {
            var pad = '';
            for(var i = 0; i < num; i++) {
                pad += char;
            }
            return (pad + str).slice(-num);
        };

}]);

/**
 * @ngdoc service
 * @name merchelloListViewHelper
 * @description Handles list view configurations.
 **/
angular.module('merchello.services').service('merchelloListViewHelper',
    ['$sessionStorage', '$localStorage',
    function($sessionStorage, $localStorage) {

        var configs = {
            product: {
                columns: [
                    { name: 'name', localizeKey: 'merchelloVariant_product' },
                    { name: 'sku', localizeKey: 'merchelloVariant_sku' },
                    { name: 'available', localizeKey: 'merchelloProducts_available' },
                    { name: 'shippable', localizeKey: 'merchelloProducts_shippable' },
                    { name: 'taxable', localizeKey: 'merchelloProducts_taxable' },
                    { name: 'totalInventory', localizeKey: 'merchelloGeneral_quantity', resultColumn: true },
                    { name: 'onSale', localizeKey: 'merchelloVariant_productOnSale', resultColumn: true },
                    { name: 'price', localizeKey: 'merchelloGeneral_price' }
                ]
            },

            // TODO remove this
            productoption: {
                columns: [
                    { name: 'name', localizeKey: 'merchelloTableCaptions_optionName' },
                    { name: 'uiOption', localizeKey: 'merchelloTableCaptions_optionUi' },
                    { name: 'choices', localizeKey: 'merchelloTableCaptions_optionValues', resultColumn: true },
                    { name: 'shared', localizeKey: 'merchelloTableCaptions_shared' },
                    { name: 'sharedCount', localizeKey: 'merchelloTableCaptions_sharedCount' }
                ],
                pageSize: 10,
                orderBy: 'name',
                orderDirection: 'asc'
            },

            customer:  {
                columns: [
                    { name: 'loginName', localizeKey: 'merchelloCustomers_loginName' },
                    { name: 'firstName', localizeKey: 'general_name' },
                    { name: 'location', localizeKey: 'merchelloCustomers_location', resultColumn: true },
                    { name: 'lastInvoiceTotal', localizeKey: 'merchelloCustomers_lastInvoiceTotal', resultColumn: true }
                ]
            },

            invoice: {
                columns: [
                    { name: 'invoiceNumber', localizeKey: 'merchelloSales_invoiceNumber' },
                    { name: 'invoiceDate', localizeKey: 'general_date' },
                    { name: 'billToName', localizeKey: 'merchelloGeneral_customer' },
                    { name: 'paymentStatus', localizeKey: 'merchelloSales_paymentStatus', resultColumn: true },
                    { name: 'fulfillmentStatus', localizeKey: 'merchelloOrder_fulfillmentStatus', resultColumn: true },
                    { name: 'total', localizeKey: 'merchelloGeneral_total' }
                ],
                pageSize: 10,
                orderBy: 'invoiceNumber',
                orderDirection: 'desc'
            },

            saleshistory: {
                columns: [
                    { name: 'invoiceNumber', localizeKey: 'merchelloSales_invoiceNumber' },
                    { name: 'invoiceDate', localizeKey: 'general_date' },
                    { name: 'paymentStatus', localizeKey: 'merchelloSales_paymentStatus', resultColumn: true },
                    { name: 'fulfillmentStatus', localizeKey: 'merchelloOrder_fulfillmentStatus', resultColumn: true },
                    { name: 'total', localizeKey: 'merchelloGeneral_total' }
                ],
                orderBy: 'invoiceNumber',
                orderDirection: 'desc'
            },

            offer: {
                columns: [
                    { name: 'name', localizeKey: 'merchelloTableCaptions_name' },
                    { name: 'offerCode', localizeKey: 'merchelloMarketing_offerCode' },
                    { name: 'offerType', localizeKey: 'merchelloMarketing_offerType' },
                    { name: 'rewards', localizeKey: 'merchelloMarketing_offerRewardsInfo', resultColumn: true },
                    { name: 'offerStartDate', localizeKey: 'merchelloTableCaptions_startDate' },
                    { name: 'offerEndDate', localizeKey: 'merchelloTableCaptions_endDate' },
                    { name: 'active', localizeKey: 'merchelloTableCaptions_active' }
                ]
            },

            customerbaskets: {
                columns: [
                    { name: 'loginName', localizeKey: 'merchelloCustomers_loginName' },
                    { name: 'firstName', localizeKey: 'general_name' },
                    { name: 'lastActivityDate', localizeKey: 'merchelloCustomers_lastActivityDate' },
                    { name: 'items', localizeKey: 'merchelloCustomers_basket' }
                ],
                pageSize: 10,
                orderBy: 'lastActivityDate',
                orderDirection: 'desc'
            }

        };

        this.getConfig = function(listViewType) {
            var ensure = listViewType.toLowerCase();
            return configs[ensure];
        };

        this.cacheSettings = function(entityType, config) {
            var cacheKey = 'listview-settings-' + entityType;

            var settings = config;

            if(settings === undefined) {

                settings = $localStorage[cacheKey];
                if (settings === undefined) {
                    settings = {
                        stickyList: false,
                        stickyCollectionList: false,
                        stickListingTab: false,
                        collectionKey: ''
                    };
                    $localStorage[cacheKey] = settings;
                }

            } else {
                $localStorage[cacheKey] = settings;
            }

            return settings;
        },


        this.cache = function(entityType) {

            function getKey(entityType, key) {
                return entityType + '-' + key;
            }

            return {
                type: entityType,

                setValue: function(key, value) {
                    var cacheKey = getKey(entityType, key);
                    $sessionStorage[cacheKey] = value;
                },

                getValue: function(key) {
                    var cacheKey = getKey(entityType, key);
                    return $sessionStorage[cacheKey];
                },

                hasKey: function(key) {
                    var cacheKey = getKey(entityType, key);
                    return $sessionStorage[cacheKey] !== undefined;
                },

                removeValue: function(key) {
                    var cacheKey = getKey(entityType, key);
                    $sessionStorage[cacheKey] = undefined;
                }
            };
        };

}]);


})();