/*! merchello
 * https://github.com/meritage/Merchello
 * Copyright (c) 2015 Merchello;
 * Licensed MIT
 */

(function() { 

    /**
   * @ngdoc service
   * @name merchello.models.genericModelBuilder
   * 
   * @description
   * A utility service that builds local models for API query results
   *  http://odetocode.com/blogs/scott/archive/2014/03/17/building-better-models-for-angularjs.aspx
   */
    angular.module('merchello.models')
        .factory('genericModelBuilder', [
            function() {
        
        // private
        // transforms json object into a local model
        function transformObject(jsonResult, Constructor) {
            var model = new Constructor();
            angular.extend(model, jsonResult);
            return model;
        }

        function transform(jsonResult, Constructor) {
            if (angular.isArray(jsonResult)) {
                var models = [];
                angular.forEach(jsonResult, function (object) {
                    models.push(transformObject(object, Constructor));
                });
                return models;
            } else {
                return transformObject(jsonResult, Constructor);
            }
        }

        // public
        return {
            transform : transform
        };
    }]);
    /**
     * @ngdoc service
     * @name merchello.models.addressDisplayBuilder
     *
     * @description
     * A utility service that builds AddressDisplay models
     */
    angular.module('merchello.models')
        .factory('addressDisplayBuilder',
            ['genericModelBuilder', 'AddressDisplay',
                function(genericModelBuilder, AddressDisplay) {

                var Constructor = AddressDisplay;

                return {
                    createDefault: function() {
                        return new Constructor();
                    },
                    transform: function(jsonResult) {
                        return genericModelBuilder.transform(jsonResult, Constructor);
                    }
                };
        }]);

    /**
     * @ngdoc service
     * @name merchello.models.appliedPaymentDisplayBuilder
     *
     * @description
     * A utility service that builds applieddPaymentDisplaybuilder
     */
    angular.module('merchello.models')
        .factory('appliedPaymentDisplayBuilder',
        ['genericModelBuilder', 'AppliedPaymentDisplay',
            function(genericModelBuilder, AppliedPaymentDisplay) {

                var Constructor = AppliedPaymentDisplay;

                return {
                    createDefault: function() {
                        return new Constructor();
                    },
                    transform: function(jsonResult) {
                        return genericModelBuilder.transform(jsonResult, Constructor);
                    }
                };
            }]);

    /**
     * @ngdoc service
     * @name merchello.models.countryDisplayBuilder
     *
     * @description
     * A utility service that builds CountryDisplay models
     */
    angular.module('merchello.models')
        .factory('countryDisplayBuilder',
        ['genericModelBuilder', 'provinceDisplayBuilder', 'CountryDisplay',
            function(genericModelBuilder, provinceDisplayBuilder, CountryDisplay) {

                var Constructor = CountryDisplay;

                return {
                    createDefault: function() {
                        return new Constructor();
                    },
                    transform: function(jsonResult) {
                        var countries = genericModelBuilder.transform(jsonResult, Constructor);
                        for(var i = 0; i < countries.length; i++) {
                            countries[i].provinces = provinceDisplayBuilder.transform(jsonResult[ i ].provinces);
                        }
                        return countries;
                    }
                };

        }]);

    /**
     * @ngdoc service
     * @name merchello.models.currencyDisplayBuilder
     *
     * @description
     * A utility service that builds CurrencyDisplay models
     */
    angular.module('merchello.models')
        .factory('currencyDisplayBuilder',
        ['genericModelBuilder', 'CurrencyDisplay',
            function(genericModelBuilder, CurrencyDisplay) {

                var Constructor = CurrencyDisplay;

                return {
                    createDefault: function() {
                        return new Constructor();
                    },
                    transform: function(jsonResult) {
                        return genericModelBuilder.transform(jsonResult, Constructor);
                    }
                };
            }]);

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

    /**
     * @ngdoc service
     * @name merchello.models.gatewayResourceDisplayBuilder
     *
     * @description
     * A utility service that builds GatewayResourceDisplay models
     */
    angular.module('merchello.models')
        .factory('gatewayResourceDisplayBuilder',
        ['genericModelBuilder', 'GatewayResourceDisplay',
            function(genericModelBuilder, GatewayResourceDisplay) {
                var Constructor = GatewayResourceDisplay;
                return {
                    createDefault: function() {
                        return new Constructor();
                    },
                    transform: function(jsonResult) {
                        return genericModelBuilder.transform(jsonResult, Constructor);
                    }
                };
            }]);


    /**
     * @ngdoc service
     * @name merchello.models.invoiceDisplayBuilder
     *
     * @description
     * A utility service that builds InvoiceDisplay models
     */
    angular.module('merchello.models')
        .factory('invoiceDisplayBuilder',
        ['genericModelBuilder', 'invoiceStatusDisplayBuilder', 'invoiceLineItemDisplayBuilder',
            'orderDisplayBuilder', 'InvoiceDisplay',
            function(genericModelBuilder, invoiceStatusDisplayBuilder, invoiceLineItemDisplayBuilder,
                     orderDisplayBuilder, InvoiceDisplay) {
                var Constructor = InvoiceDisplay;

                return {
                    createDefault: function() {
                        var invoice = new Constructor();
                        invoice.invoiceStatus = invoiceStatusDisplayBuilder.createDefault();
                        return invoice;
                    },
                    transform: function(jsonResult) {
                        var invoices = genericModelBuilder.transform(jsonResult, Constructor);
                        for(var i = 0; i < invoices.length; i++) {
                            invoices[ i ].invoiceStatus = invoiceStatusDisplayBuilder.transform(jsonResult[ i ].invoiceStatus);
                            invoices[ i ].items = invoiceLineItemDisplayBuilder.transform(jsonResult[ i ].items);
                            invoices[ i ].orders = orderDisplayBuilder.transform(jsonResult[ i ].orders);
                        }
                        return invoices;
                    }
                };
            }]);
    /**
     * @ngdoc service
     * @name merchello.models.invoiceLineItemDisplayBuilder
     *
     * @description
     * A utility service that builds InvoiceLineItemDisplay models
     */
    angular.module('merchello.models')
        .factory('invoiceLineItemDisplayBuilder',
        ['genericModelBuilder', 'extendedDataDisplayBuilder', 'typeFieldDisplayBuilder', 'InvoiceLineItemDisplay',
            function(genericModelBuilder, extendedDataDisplayBuilder, typeFieldDisplayBuilder, InvoiceLineItemDisplay) {
                var Constructor = InvoiceLineItemDisplay;
                return {
                    createDefault: function() {
                        var invoiceLineItem = new Constructor();
                        invoiceLineItem.lineItemTypeField = typeFieldDisplayBuilder.createDefault();
                        invoiceLineItem.extendedData = extendedDataDisplayBuilder.createDefault();
                        return invoiceLineItem;
                    },
                    transform: function(jsonResult) {
                        var invoiceLineItems = genericModelBuilder.transform(jsonResult, Constructor);
                        for(var i = 0; i < invoiceLineItems.length; i++) {
                            invoiceLineItems[ i ].extendedData = extendedDataDisplayBuilder.transform(jsonResult[ i ].extendedData);
                            invoiceLineItems[ i ].lineItemTypeField = typeFieldDisplayBuilder.transform(jsonResult[ i ].lineItemTypeField);
                        }
                        return invoiceLineItems;
                    }
                };
            }]);

    /**
     * @ngdoc service
     * @name merchello.models.invoiceStatusDisplayBuilder
     *
     * @description
     * A utility service that builds InvoiceStatusDisplay models
     */
    angular.module('merchello.models')
        .factory('invoiceStatusDisplayBuilder',
        ['genericModelBuilder', 'InvoiceStatusDisplay',
            function(genericModelBuilder, InvoiceStatusDisplay) {
                var Constructor = InvoiceStatusDisplay;
                return {
                    createDefault: function() {
                        return new Constructor();
                    },
                    transform: function(jsonResult) {
                        return genericModelBuilder.transform(jsonResult, Constructor);
                    }
                };
            }]);

    /**
     * @ngdoc service
     * @name merchello.models.gatewayResourceDisplayBuilder
     *
     * @description
     * A utility service that builds GatewayResourceDisplay models
     */
    angular.module('merchello.models')
        .factory('orderDisplayBuilder',
        ['genericModelBuilder', 'orderStatusDisplayBuilder', 'orderLineItemDisplayBuilder', 'OrderDisplay',
            function(genericModelBuilder, orderStatusDisplayBuilder, orderLineItemDisplayBuilder, OrderDisplay) {
                var Constructor = OrderDisplay;

                return {
                    createDefault: function() {
                        var order = new Constructor();
                        order.orderStatus = orderStatusDisplayBuilder.createDefault();
                        return order;
                    },
                    transform: function(jsonResult) {
                        var orders = genericModelBuilder.transform(jsonResult, Constructor);
                        for(var i = 0; i < orders.length; i++) {
                            orders[ i ].orderStatus = orderStatusDisplayBuilder.transform(jsonResult[ i ].orderStatus);
                            orders[ i ].items = orderLineItemDisplayBuilder.transform(jsonResult[ i ].items);
                        }
                        return orders;
                    }
                };
            }]);

    /**
     * @ngdoc service
     * @name merchello.models.orderLineItemDisplayBuilder
     *
     * @description
     * A utility service that builds OrderLineItemDisplay models
     */
    angular.module('merchello.models')
        .factory('orderLineItemDisplayBuilder',
        ['genericModelBuilder', 'extendedDataDisplayBuilder', 'typeFieldDisplayBuilder', 'OrderLineItemDisplay',
            function(genericModelBuilder, extendedDataDisplayBuilder, typeFieldDisplayBuilder, OrderLineItemDisplay) {
                var Constructor = OrderLineItemDisplay;
                return {
                    createDefault: function() {
                        var orderLineItem = new Constructor();
                        orderLineItem.extendedData = extendedDataDisplayBuilder.createDefault();
                        orderLineItem.lineItemTypeField = typeFieldDisplayBuilder.createDefault();
                        return orderLineItem;
                    },
                    transform: function(jsonResult) {
                        var orderLineItems = genericModelBuilder.transform(jsonResult, Constructor);
                        for(var i = 0; i < orderLineItems.length; i++) {
                            orderLineItems[ i ].extendedData = extendedDataDisplayBuilder.transform(jsonResult[ i ].extendedData);
                            orderLineItems[ i ].lineItemTypeField = typeFieldDisplayBuilder.transform(jsonResult[ i ].lineItemTypeField);
                        }
                        return orderLineItems;
                    }
                };
            }]);

    /**
     * @ngdoc service
     * @name merchello.models.orderStatusDisplayBuilder
     *
     * @description
     * A utility service that builds OrderStatusDisplay models
     */
    angular.module('merchello.models')
        .factory('orderStatusDisplayBuilder',
        ['genericModelBuilder', 'OrderStatusDisplay',
            function(genericModelBuilder, OrderStatusDisplay) {

                var Constructor = OrderStatusDisplay;

                return {
                    createDefault: function() {
                        return new Constructor();
                    },
                    transform: function(jsonResult) {
                        return genericModelBuilder.transform(jsonResult, Constructor);
                    }
                };
            }]);

    /**
     * @ngdoc service
     * @name merchello.models.paymentDisplayBuilder
     *
     * @description
     * A utility service that builds PaymentDisplay models
     */
    angular.module('merchello.models')
        .factory('paymentDisplayBuilder',
        ['genericModelBuilder', 'appliedPaymentDisplayBuilder', 'extendedDataDisplayBuilder', 'PaymentDisplay',
            function(genericModelBuilder, appliedPaymentDisplayBuilder, extendedDataDisplayBuilder, PaymentDisplay) {

                var Constructor = PaymentDisplay;

                return {
                    createDefault: function() {
                        var payment = new Constructor();
                        payment.extendedData = extendedDataDisplayBuilder.createDefault();
                        return payment;
                    },
                    transform: function(jsonResult) {
                        var payment = genericModelBuilder.transform(jsonResult, Constructor);
                        payment.appliedPayments = appliedPaymentDisplayBuilder.transform(jsonResult.appliedPayments);
                        payment.extendedData = extendedDataDisplayBuilder.transform(jsonResult.extendedData);
                        return payment;
                    }
                };
            }]);
/**
 * @ngdoc service
 * @name merchello.models.paymentRequestDisplayBuilder
 *
 * @description
 * A utility service that builds PaymentRequestDisplay models
 */
angular.module('merchello.models')
    .factory('paymentRequestDisplayBuilder',
    ['genericModelBuilder', 'PaymentRequestDisplay',
        function(genericModelBuilder, PaymentRequestDisplay) {
            var Constructor = PaymentRequestDisplay;
            return {
                createDefault: function() {
                    return new Constructor();
                },
                transform: function(jsonResult) {
                    return genericModelBuilder.transform(jsonResult, Constructor);
                }
            };
        }]);

    /**
     * @ngdoc service
     * @name merchello.models.provinceDisplayBuilder
     *
     * @description
     * A utility service that builds ProvinceDisplay models
     */
    angular.module('merchello.models')
        .factory('provinceDisplayBuilder',
        ['genericModelBuilder', 'ProvinceDisplay',
            function(genericModelBuilder, ProvinceDisplay) {
                var Constructor = ProvinceDisplay;
                return {
                    createDefault: function() {
                        return new Constructor();
                    },
                    transform: function(jsonResult) {
                        return genericModelBuilder.transform(jsonResult, Constructor);
                    }
                };
            }]);

    /**
     * @ngdoc service
     * @name merchello.services.queryDisplayBuilder
     *
     * @description
     * A utility service that builds QueryDisplayModels models
     *
     */
    angular.module('merchello.models')
        .factory('queryDisplayBuilder',
        ['genericModelBuilder', 'QueryDisplay',
            function(genericModelBuilder, QueryDisplay) {
            var Constructor = QueryDisplay;
            return {
                createDefault: function() {
                    return new Constructor();
                },
                transform: function(jsonResult) {
                    return genericModelBuilder.transform(jsonResult, Constructor);
                }
            };
        }]);


    /**
     * @ngdoc service
     * @name merchello.services.queryParameterDisplayBuilder
     *
     * @description
     * A utility service that builds QueryParameterDisplayModels models
     *
     */
    angular.module('merchello.models')
        .factory('queryParameterDisplayBuilder',
            ['genericModelBuilder', 'QueryParameterDisplay',
            function(genericModelBuilder, QueryParameterDisplay) {
            var Constructor = QueryParameterDisplay;
            return {
                createDefault: function() {
                    return new Constructor();
                },
                transform: function(jsonResult) {
                    return genericModelBuilder.transform(jsonResult, Constructor);
                }
            };
        }]);

    /**
     * @ngdoc service
     * @name merchello.services.queryResultDisplayBuilder
     *
     * @description
     * A utility service that builds QueryResultDisplayModels models
     */
    angular.module('merchello.models')
        .factory('queryResultDisplayBuilder',
        ['genericModelBuilder', 'QueryResultDisplay',
            function(genericModelBuilder, QueryResultDisplay) {
            var Constructor = QueryResultDisplay;
            return {
                createDefault: function() {
                    return new Constructor();
                },
                transform: function (jsonResult, itemBuilder) {
                    // this is slightly different than other builders in that there can only ever be a single
                    // QueryResult returned from the WebApiController, so we iterate through the items
                    var result = genericModelBuilder.transform(jsonResult, Constructor);
                    if (itemBuilder !== undefined)
                    {
                        result.items = [];
                        result.items = itemBuilder.transform(jsonResult.items);
                    }
                    return result;
                }
            };
        }]);

    /**
     * @ngdoc service
     * @name merchello.models.salesHistoryMessageDisplayBuilder
     *
     * @description
     * A utility service that builds salesHistoryMessageDisplayBuilder models
     */
    angular.module('merchello.models')
        .factory('salesHistoryMessageDisplayBuilder',
        ['genericModelBuilder', 'SalesHistoryMessageDisplay',
            function(genericModelBuilder, SalesHistoryMessageDisplay) {

                var Constructor = SalesHistoryMessageDisplay;

                return {
                    createDefault: function() {
                        return new Constructor();
                    },
                    transform: function(jsonResult) {
                        return genericModelBuilder.transform(jsonResult, Constructor);
                    }
                };
            }]);

    /**
     * @ngdoc service
     * @name merchello.models.auditLogDisplayBuilder
     *
     * @description
     * A utility service that builds auditLogDisplayBuilder models
     */
    angular.module('merchello.models')
        .factory('auditLogDisplayBuilder',
            ['genericModelBuilder', 'salesHistoryMessageDisplayBuilder', 'extendedDataDisplayBuilder', 'AuditLogDisplay',
            function(genericModelBuilder, salesHistoryMessageDisplayBuilder, extendedDataDisplayBuilder, AuditLogDisplay) {

                var Constructor = AuditLogDisplay;

                return {
                    createDefault: function() {
                        return new Constructor();
                    },
                    transform: function(jsonResult) {
                        var auditLogDisplay = genericModelBuilder.transform(jsonResult, Constructor);
                        auditLogDisplay.extendedData = extendedDataDisplayBuilder.transform(jsonResult.extendedData);

                        // this is a bit brittle - and we should look at the construction of this in the ApiController
                        var message = JSON.parse(jsonResult.message);
                        auditLogDisplay.message = salesHistoryMessageDisplayBuilder.transform(message);
                        return auditLogDisplay;
                    }
                };
        }]);
    /**
     * @ngdoc service
     * @name merchello.models.auditLogDisplayBuilder
     *
     * @description
     * A utility service that builds auditLogDisplayBuilder models
     */
    angular.module('merchello.models')
        .factory('dailyAuditLogDisplayBuilder',
            ['genericModelBuilder', 'auditLogDisplayBuilder', 'DailyAuditLogDisplay',
            function(genericModelBuilder, auditLogDisplayBuilder, DailyAuditLogDisplay) {

                var Constructor = DailyAuditLogDisplay;

                return {
                    createDefault: function() {
                        return new Constructor();
                    },
                    transform: function(jsonResult) {
                        var dailyLog = genericModelBuilder.transform(jsonResult, Constructor);
                        var logs = [];
                        angular.forEach(dailyLog.logs, function(log) {
                            logs.push(auditLogDisplayBuilder.transform(log));
                        });
                        dailyLog.logs = logs;
                        return dailyLog;
                    }
                };
        }]);
    /**
     * @ngdoc service
     * @name merchello.models.salesHistoryDisplayBuilder
     *
     * @description
     * A utility service that builds salesHistoryDisplayBuilder models
     */
    angular.module('merchello.models')
        .factory('salesHistoryDisplayBuilder',
            ['genericModelBuilder', 'dailyAuditLogDisplayBuilder', 'SalesHistoryDisplay',
            function(genericModelBuilder, dailyAuditLogDisplayBuilder, SalesHistoryDisplay) {

                var Constructor = SalesHistoryDisplay;

                return {
                    createDefault: function() {
                        return new Constructor();
                    },
                    transform: function(jsonResult) {
                        var history = this.createDefault();
                        angular.forEach(jsonResult.dailyLogs, function(result) {
                            history.addDailyLog(dailyAuditLogDisplayBuilder.transform(result));
                        });
                        return history;
                    }
                };
        }]);
    /**
     * @ngdoc service
     * @name merchello.models.settingDisplayBuilder
     *
     * @description
     * A utility service that builds SettingDisplay models
     */
    angular.module('merchello.models')
        .factory('settingDisplayBuilder',
        ['genericModelBuilder', 'SettingDisplay',
            function(genericModelBuilder, SettingDisplay) {

                var Constructor = SettingDisplay;

                return {
                    createDefault: function() {
                        return new Constructor();
                    },
                    transform: function(jsonResult) {
                        return genericModelBuilder.transform(jsonResult, Constructor);
                    }
                };
            }]);

    /**
     * @ngdoc service
     * @name merchello.models.shipmentDisplayBuilder
     *
     * @description
     * A utility service that builds ShipmentDisplay models
     */
    angular.module('merchello.models')
        .factory('shipmentDisplayBuilder',
        ['genericModelBuilder',  'shipmentStatusDisplayBuilder', 'orderLineItemDisplayBuilder', 'ShipmentDisplay', 'ShipmentStatusDisplay',
            function(genericModelBuilder, shipmentStatusBuilder, orderLineItemBuilder, ShipmentDisplay, ShipmentStatusDisplay) {

                var Constructor = ShipmentDisplay;

                return {
                    // TODO the default warehouse address (AddressDisplay) could be saved as a config value
                    // and then added to the shipment origin address
                    createDefault: function() {
                        var shipment = new Constructor();
                        shipment.shipmentStatus = shipmentStatusBuilder.createDefault();
                        return shipment;
                    },
                    transform: function(jsonResult) {
                        // the possible list of shipments
                        var shipments = genericModelBuilder.transform(jsonResult, Constructor);
                        for(var i = 0; i < jsonResult.length; i++) {
                            // each shipment has a ShipmentStatusDisplay
                            shipments[ i ].shipmentStatus = shipmentStatusBuilder.transform(jsonResult[ i ].shipmentStatus, ShipmentStatusDisplay);
                            // add the OrderLineItemDisplay(s) associated with the shipment
                            shipments[ i ].items = orderLineItemBuilder.transform(jsonResult[ i ].items);
                        }
                        return shipments;
                    }
                };
            }]);

    /**
     * @ngdoc service
     * @name merchello.models.shipmentStatusDisplayBuilder
     *
     * @description
     * A utility service that builds ShipmentStatusDisplay models
     */
    angular.module('merchello.models')
    .factory('shipmentStatusDisplayBuilder',
    ['genericModelBuilder', 'ShipmentStatusDisplay',
        function(genericModelBuilder, ShipmentStatusDisplay) {

            var Constructor = ShipmentStatusDisplay;

            return {
                createDefault: function() {
                    return new Constructor();
                },
                transform: function(jsonResult) {
                    return genericModelBuilder.transform(jsonResult, Constructor);
                }
            };
        }]);

    /**
     * @ngdoc service
     * @name merchello.models.typeFieldDisplayBuilder
     *
     * @description
     * A utility service that builds TypeFieldDisplay models
     */
    angular.module('merchello.models')
    .factory('typeFieldDisplayBuilder',
    ['genericModelBuilder', 'TypeFieldDisplay',
        function(genericModelBuilder, TypeFieldDisplay) {

            var Constructor = TypeFieldDisplay;

            return {
                createDefault: function() {
                    return new Constructor();
                },
                transform: function(jsonResult) {
                    return genericModelBuilder.transform(jsonResult, Constructor);
                }
            };
        }]);


})();