/*! merchello
 * https://github.com/meritage/Merchello
 * Copyright (c) 2015 Merchello;
 * Licensed MIT
 */


(function() { 

    /**
    * @ngdoc model
    * @name AddressDisplay
    * @function
    * 
    * @description
    * Represents a JS version of Merchello's AddressDisplay object
    */
    var AddressDisplay = function () {

        var self = this;

        self.name = '';
        self.address1 = '';
        self.address2 = '';
        self.locality = '';
        self.region = '';
        self.postalCode = '';
        self.countryCode = '';
        self.addressType = '';
        self.organization = '';
        self.phone = '';
        self.email = '';
        self.isCommercial = false;
    };

    AddressDisplay.prototype = (function() {

        function isEmpty() {
            var result = false;
            if (this.address1 === '' || this.locality === '' || this.address1 === null || this.locality === null) {
                result = true;
            }
            return result;
        }

        function clone() {
            var dst = new AddressDisplay();
            angular.extend(dst, this);
            return dst;
        }

        return {
            isEmpty: isEmpty,
            clone: clone
        };
    }());

    angular.module('merchello.models').constant('AddressDisplay', AddressDisplay);

/**
 * @ngdoc model
 * @name BackOfficeTreeDisplay
 * @function
 *
 * @description
 * Represents a JS version of  BackOfficeTreeDisplay object
 */
var BackOfficeTreeDisplay = function() {
    var self = this;
    self.routeId = '';
    self.parentRouteId = '';
    self.title = '';
    self.icon = '';
    self.routePath = '';
    self.sortOrder = 0;
};

angular.module('merchello.models').constant('BackOfficeTreeDisplay', BackOfficeTreeDisplay);
    /**
     * @ngdoc model
     * @name CountryDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's CountryDisplay object
     */
    var CountryDisplay = function() {
        var self = this;

        self.key = '';
        self.countryCode = '';
        self.name = '';
        self.provinceLabel = '';
        self.provinces = [];
    };

    CountryDisplay.prototype = (function() {

        function hasProvinces() {
            return this.provinces.length > 0;
        }

        return {
            hasProvinces: hasProvinces
        };

    }());

    angular.module('merchello.models').constant('CountryDisplay', CountryDisplay);
    /**
     * @ngdoc model
     * @name CurrencyDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's CurrencyDisplay object
     */
    var CurrencyDisplay = function() {
        var self = this;
        self.name = '';
        self.currencyCode = '';
        self.symbol = '';
    };

    angular.module('merchello.models').constant('CurrencyDisplay', CurrencyDisplay);

    /**
     * @ngdoc model
     * @name DialogEditorViewDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's DialogEditorViewDisplay object
     */
    var DialogEditorViewDisplay = function() {
        var self = this;
        self.title = '';
        self.description = '';
        self.editorView = '';
    };

    angular.module('merchello.models').constant('DialogEditorViewDisplay', DialogEditorViewDisplay);
    /**
     * @ngdoc model
     * @name ExtendedDataDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's ExtendedDataDisplay object
     */
    var ExtendedDataDisplay = function() {
        var self = this;
        self.items = [];
    };

    ExtendedDataDisplay.prototype = (function() {

        function isEmpty() {
            return this.items.length === 0;
        }

        function getValue(key) {
            if (isEmpty.call(this)) {
                return '';
            }
            var found = false;
            var i = 0;
            var value = '';
            while(i < this.items.length && !found) {
                if (this.items[ i ].key === key) {
                    found = true;

                    value = this.items[ i ].value;
                } else {
                    i++;
                }
            }
            return value;
        }

        function setValue(key, value) {

            var existing = _.find(this.items, function(item) {
              return item.key === key;
            });
            if(existing) {
                existing.value = value;
                return;
            }

            this.items.push({ key: key, value: value });
        }

        function toArray() {
            return this.items;
        }

        return {
            isEmpty: isEmpty,
            getValue: getValue,
            setValue: setValue,
            toArray: toArray
        };
    }());

    angular.module('merchello.models').constant('ExtendedDataDisplay', ExtendedDataDisplay);
    /**
     * @ngdoc model
     * @name ExtendedDataItemDisplay
     * @function
     *
     * @description
     * Represents a JS version of  ExtendedDataItemDisplay object
     */
    var ExtendedDataItemDisplay = function() {
        this.key = '';
        this.value = '';
    };

    angular.module('merchello.models').constant('ExtendedDataItemDisplay', ExtendedDataItemDisplay);


    /**
     * @ngdoc model
     * @name MerchelloTab
     * @function
     *
     * @description
     * Backoffice model used for tab navigation
     */
    var MerchelloTab = function() {
        var self = this;
        self.id = '';
        self.name = '';
        self.url = '';
        self.active = false;
        self.visible = true;
        self.callback = undefined;
    };

    angular.module('merchello.models').constant('MerchelloTab', MerchelloTab);

    /**
     * @ngdoc model
     * @name MerchelloTabCollection
     * @function
     *
     * @description
     * Backoffice model used for tab navigation
     */
    var MerchelloTabCollection = function() {
        this.items = [];
    };

    MerchelloTabCollection.prototype = (function() {

        // safely adds a tab to the collection
        function addTab(id, name, url) {
            var existing = _.find(this.items, function(tab) { return tab.id === id; });
            if (existing === undefined || existing === null) {
                var tab = new MerchelloTab();
                tab.id = id;
                tab.name = name;
                tab.url = url;
                this.items.push(tab);
            }
        }

        function addActionTab(id, name, callback) {
            var existing = _.find(this.items, function(tab) { return tab.id === id; });
            if (existing === undefined || existing === null) {
                var tab = new MerchelloTab();
                tab.id = id;
                tab.name = name;
                tab.callback = callback;
                this.items.push(tab);
            }
        }

        function setActive(id) {
           angular.forEach(this.items, function(item) {
               if(item.id === id) {
                   item.active = true;
               } else {
                   item.active = false;
               }
           });
        }

        function hideTab(id) {
            var existing = _.find(this.items, function(tab) {return tab.id === id});
            if (existing !== undefined && existing !== null) {
                existing.visible = false;
            }
        }

        function showTab(id) {
            var existing = _.find(this.items, function(tab) {return tab.id === id});
            if (existing !== undefined && existing !== null) {
                existing.visible = true;
            }
        }

        function insertTab(id, name, url, index) {
            var existing = _.find(this.items, function(tab) { return tab.id === id; });
            if (existing === undefined || existing === null) {
                var tab = new MerchelloTab();
                tab.id = id;
                tab.name = name;
                tab.url = url;
                if (this.items.length <= index) {
                    addTab.call(this, tab);
                } else {
                    this.items.splice(index, 0, tab);
                }
            }
        }

        function insertActionTab(id, name, callback, index) {
            var existing = _.find(this.items, function(tab) { return tab.id === id; });
            if (existing === undefined || existing === null) {
                var tab = new MerchelloTab();
                tab.id = id;
                tab.name = name;
                tab.callback = callback;
                if (this.items.length <= index) {
                    addTab.call(this, tab);
                } else {
                    this.items.splice(index, 0, tab);
                }
            }
        }


        /// appends a customer tab to the current collection
        function appendCustomerTab(customerKey) {
            if(customerKey !== '00000000-0000-0000-0000-000000000000') {
                addTab.call(this, 'customer', 'merchelloTabs_customer', '#/merchello/merchello/customeroverview/' + customerKey);
            }
        }

        function appendOfferTab(offerKey, backOfficeTree) {
            var title = 'merchelloTabs_';
            if(backOfficeTree.title === undefined || backOfficeTree.title === '') {
                title += 'offer';
            } else {
                title += backOfficeTree.title.toLowerCase();
            }
            if(offerKey !== '00000000-0000-0000-0000-000000000000' && offerKey !== 'create') {
                addTab.call(this, 'offer', title, '#' + backOfficeTree.routePath.replace('{0}', offerKey));
            } else {
                addTab.call(this, 'offer', title, '#' +backOfficeTree.routePath.replace('{0}', 'create'));
            }
        }

        return {
            addTab: addTab,
            setActive: setActive,
            insertTab: insertTab,
            appendCustomerTab: appendCustomerTab,
            appendOfferTab: appendOfferTab,
            addActionTab: addActionTab,
            insertActionTab: insertActionTab,
            hideTab: hideTab,
            showTab: showTab
        };
    }());

    angular.module('merchello.models').constant('MerchelloTabCollection', MerchelloTabCollection);
    /**
     * @ngdoc model
     * @name ProvinceDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's ProvinceDisplay object
     */
     var ProvinceDisplay = function()
     {
         var self = this;
         self.name = '';
         self.code = '';
     };

    angular.module('merchello.models').constant('ProvinceDisplay', ProvinceDisplay);
    /**
     * @ngdoc model
     * @name Merchello.Models.Province
     * @function
     *
     * @description
     * Represents a JS version of Merchello's SettingDisplay object
     */
    var SettingDisplay = function() {
        self.currencyCode = '';
        self.nextOrderNumber = 0;
        self.nextInvoiceNumber = 0;
        self.nextShipmentNumber = 0;
        self.dateFormat = '';
        self.timeFormat = '';
        self.unitSystem = '';
        self.globalShippable = false;
        self.globalTaxable = false;
        self.globalTrackInventory = false;
        self.globalShippingIsTaxable = false;
        self.globalTaxationApplication = 'invoice';
        self.defaultExtendedContentCulture = 'en-US'
    };

    angular.module('merchello.models').constant('SettingDisplay', SettingDisplay);
    /**
     * @ngdoc model
     * @name Merchello.Models.TypeField
     * @function
     *
     * @description
     * Represents a JS version of Merchello's TypeFieldDisplay object
     */
    var TypeFieldDisplay = function() {
        var self = this;
        self.alias = '';
        self.name = '';
        self.typeKey = '';
    };

    angular.module('merchello.models').constant('TypeFieldDisplay', TypeFieldDisplay);

/**
 * @ngdoc model
 * @name EntityCollectionDisplay
 * @function
 *
 * @description
 * Represents a JS version of Merchello's EntityCollectionDisplay object
 */
var EntityCollectionDisplay = function() {
    var self = this;
    self.key = '';
    self.parentKey = '';
    self.entityTfKey = '';
    self.entityType = '';
    self.entityTypeField = {};
    self.providerKey = '';
    self.name = '';
    self.sortOrder = 0;
};

angular.module('merchello.models').constant('EntityCollectionDisplay', EntityCollectionDisplay);

/**
 * @ngdoc model
 * @name EntityCollectionDisplay
 * @function
 *
 * @description
 * Represents a JS version of Merchello's EntityCollectionProviderDisplay object
 */
var EntityCollectionProviderDisplay = function() {
    var self = this;
    self.key = '';
    self.name = '';
    self.description = '';
    self.entityTypeField = {};
    self.managesUniqueCollection = true;
    self.entityType = '';
    self.managedCollections = [];
};


angular.module('merchello.models').constant('EntityCollectionProviderDisplay', EntityCollectionProviderDisplay);
    /**
     * @ngdoc model
     * @name CustomerAddressDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's CustomerAddressDisplay object
     */
    var CustomerAddressDisplay = function() {
        var self = this;
        self.key = '';
        self.label = '';
        self.customerKey = '';
        self.fullName = '';
        self.address1 = '';
        self.address2 = '';
        self.locality = '';
        self.region = '';
        self.postalCode = '';
        self.addressType = '';
        self.addressTypeFieldKey = '';
        self.company = '';
        self.countryCode = '';
        self.phone = '';
        self.isDefault = false;
    };

    CustomerAddressDisplay.prototype = (function() {

        function isEmpty() {
            var result = false;
            if (this.address1 === '' || this.locality === '' || this.address1 === null || this.locality === null) {
                result = true;
            }
            return result;
        }

        // maps CustomerAddressDisplay to AddressDisplay
        function asAddressDisplay() {
            var address = new AddressDisplay();
            angular.extend(address, this);
            // corrections
            address.name = this.fullName;
            address.organization = this.company;
            return address;
        }

        return {
            isEmpty: isEmpty,
            asAddressDisplay: asAddressDisplay
        };
    }());

    angular.module('merchello.models').constant('CustomerAddressDisplay', CustomerAddressDisplay);
    /**
     * @ngdoc model
     * @name CustomerDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's CustomerDisplay object
     */
    var CustomerDisplay = function() {
        var self = this;
        self.firstName = '';
        self.key = '';
        self.lastActivityDate = '';
        self.lastName = '';
        self.loginName = '';
        self.notes = '';
        self.email = '';
        self.taxExempt = false;
        self.extendedData = {};
        self.addresses = [];
        self.invoices = [];
    };

    CustomerDisplay.prototype = (function() {

        function getDefaultAddress(addressType) {
            return _.find(this.addresses, function(address) {
                return address.addressType === addressType && address.isDefault === true;
            });
        }

        function getAddressesByAddressType(addressType) {
            return _.filter(this.addresses, function(address) {
                return address.addressType === addressType;
            });
        }

        // returns a value indicating whether or not the customer has addresses
        function hasAddresses() {
            return this.addresses.length > 0;
        }

        // returns a value indicating whether or not the customer has a default address of a given type
        function hasDefaultAddressOfType(addressType) {
            var address = getDefaultAddress.call(this, addressType);
            return address !== null && address !== undefined;
        }

        // gets the default billing address
        function getDefaultBillingAddress() {
            var address = getDefaultAddress.call(this, 'Billing');
            if(address === null || address === undefined) {
                address = new CustomerAddressDisplay();
                address.addressType = 'Billing';
            }
            return address;
        }

        // gets the collection of billing addresses
        function getBillingAddresses() {
            return getAddressesByAddressType.call(this, 'Billing');
        }

        // get default shipping address
        function getDefaultShippingAddress() {
            var address = getDefaultAddress.call(this, 'Shipping');
            if(address === null || address === undefined) {
                address = new CustomerAddressDisplay();
                address.addressType = 'Shipping';
            }
            return address;
        }

        // gets the shipping address collection
        function getShippingAddresses() {
            return getAddressesByAddressType.call(this, 'Shipping');
        }

        // gets the last invoice billed to the customer
        function getLastInvoice() {
            if (this.invoices.length > 0) {
                var sorted = _.sortBy(this.invoices, function(invoice) {
                    return -1 * invoice.invoiceNumber;
                });
                if(sorted === undefined || sorted === null) {
                    return new InvoiceDisplay();
                } else {
                    return sorted[0];
                }
            } else {
                return new InvoiceDisplay();
            }
        }

        function getPrimaryLocation() {
            var shippingAddress = getDefaultShippingAddress.call(this);
            if (!shippingAddress.isEmpty()) {
                return shippingAddress;
            } else {
                return getDefaultBillingAddress.call(this);
            }
        }

        return {
            getLastInvoice: getLastInvoice,
            hasAddresses: hasAddresses,
            hasDefaultAddressOfType: hasDefaultAddressOfType,
            getAddressesByAddressType: getAddressesByAddressType,
            getDefaultAddress: getDefaultAddress,
            getDefaultBillingAddress: getDefaultBillingAddress,
            getBillingAddresses: getBillingAddresses,
            getDefaultShippingAddress: getDefaultShippingAddress,
            getShippingAddresses: getShippingAddresses,
            getPrimaryLocation: getPrimaryLocation
        };

    }());

    angular.module('merchello.models').constant('CustomerDisplay', CustomerDisplay);

/**
 * @ngdoc model
 * @name DetachedContentTypeDisplay
 * @function
 *
 * @description
 * Represents a JS version of Merchello's DetachedContentTypeDisplay object
 */
var DetachedContentTypeDisplay = function () {
	var self = this;
	self.key = '';
	self.name = '';
	self.description = '';
	self.entityType = '';
    self.umbContentType = {};
};

angular.module('merchello.models').constant('DetachedContentTypeDisplay', DetachedContentTypeDisplay);
/**
 * @ngdoc model
 * @name UmbContentTypeDisplay
 * @function
 *
 * @description
 * Represents a JS version of Merchello's UmbContentTypeDisplay object
 */
var UmbContentTypeDisplay = function() {
    var self = this;
    self.id = '';
    self.key = '';
    self.name = '';
    self.alias = '';
    self.icon = '';
    self.tabs = [];
    self.defaultTemplateId = 0;
    self.allowedTemplates = [];
};

angular.module('merchello.models').constant('UmbContentTypeDisplay', UmbContentTypeDisplay);

    /**
     * @ngdoc model
     * @name AddEditCustomerAddressDialogData
     * @function
     *
     * @description
     *  A dialog data object for adding or editing CustomerAddressDisplay objects
     */
    var AddEditCustomerAddressDialogData = function() {
        var self = this;
        self.customerAddress = {};
        self.countries = [];
        self.selectedCountry = {};
        self.selectedProvince = {};
        self.setDefault = false;
    };

    angular.module('merchello.models').constant('AddEditCustomerAddressDialogData', AddEditCustomerAddressDialogData);
    /**
     * @ngdoc model
     * @name AddEditCustomerDialogData
     * @function
     *
     * @description
     *  A dialog data object for adding or editing CustomerDisplay objects
     */
    var AddEditCustomerDialogData = function() {
        var self = this;
        self.firstName = '';
        self.lastName = '';
        self.email = '';
    };

    angular.module('merchello.models').constant('AddEditCustomerDialogData', AddEditCustomerDialogData);

/**
 * @ngdoc model
 * @name AddEditEntityStaticCollectionDialog
 * @function
 *
 * @description
 *  A dialog data object for adding or editing Static Collection objects
 */
function AddEditEntityStaticCollectionDialog() {
    var self = this;
    self.entityType = '';
    self.collectionKeys = [];
};

AddEditEntityStaticCollectionDialog.prototype = (function() {

    function exists(key) {
        if (this.collectionKeys.length === 0) return false;
        var found = _.find(this.collectionKeys, function(k) {
            return k === key;
        });
        return found !== undefined;
    }

    function addCollectionKey(key) {
        if (!exists.call(this, key)) {
            this.collectionKeys.push(key);
        }
    }

    function removeCollectionKey(key) {
        this.collectionKeys = _.reject(this.collectionKeys, function(item) {
          return item === key;
        });
    }

    return {
        exists : exists,
        addCollectionKey: addCollectionKey,
        removeCollectionKey: removeCollectionKey
    };
})();

angular.module('merchello.models').constant('AddEditEntityStaticCollectionDialog', AddEditEntityStaticCollectionDialog);

    /**
     * @ngdoc model
     * @name AddEditNotificationMessageDialogData
     * @function
     *
     * @description
     * Represents a JS version of Merchello's AddEditNotificationMessageDialogData object
     */
    var AddEditNotificationMessageDialogData = function() {
        var self = this;
        self.notificationMessage = {};
        self.notificationMonitors = [];
        self.selectedMonitor = {};
    };

    angular.module('merchello.models').constant('AddEditNotificationMessageDialogData', AddEditNotificationMessageDialogData);
    /**
     * @ngdoc model
     * @name AddEditNotificationMethodDialogData
     * @function
     *
     * @description
     * Represents a JS version of Merchello's AddEditNotificationMethodDialogData object
     */
    var AddEditNotificationMethodDialogData = function() {
        var self = this;
        self.notificationMethod = {};
    };

    angular.module('merchello.models').constant('AddEditNotificationMethodDialogData', AddEditNotificationMethodDialogData);
    /**
     * @ngdoc model
     * @name AddEditWarehouseCatalogDialogData
     * @function
     *
     * @description
     * Represents a JS version of Merchello's AddEditWarehouseCatalogDialogData object
     */
    var AddEditWarehouseCatalogDialogData = function() {
        var self = this;
        self.warehouse = {};
        self.warehouseCatalog = {};
    };

    angular.module('merchello.models').constant('AddEditWarehouseCatalogDialogData', AddEditWarehouseCatalogDialogData);
    /**
     * @ngdoc model
     * @name AddShipCountryDialogData
     * @function
     *
     * @description
     * A back office dialogData model used for adding ship countries to shipping configurations.
     */
    var AddEditWarehouseDialogData = function() {
        var self = this;
        self.warehouse = {};
        self.availableCountries = [];
        self.selectedCountry = {};
        self.selectedProvince = {};
    };

    angular.module('merchello.models').constant('AddEditWarehouseDialogData', AddEditWarehouseDialogData);

    /**
     * @ngdoc model
     * @name AddPaymentDialogData
     * @function
     *
     * @description
     * A back office dialogData model used for adding payments to a sale.
     */
    var AddPaymentDialogData = function() {
        var self = this;
        self.paymentMethod = {};
        self.paymentMethodName = '';
        self.invoice = {};
        self.authorizePaymentOnly = false;
        self.invoiceBalance = 0;
        self.amount = 0;
        self.currencySymbol = '';
        self.showSpinner = function() { return true; }
        self.processorArgs = new ProcessorArgumentCollectionDisplay();
    };

    AddPaymentDialogData.prototype = (function() {

        function asPaymentRequestDisplay() {
            var request = new PaymentRequestDisplay();
            request.invoiceKey = this.invoice.key;
            request.paymentMethodKey = this.paymentMethod.key;
            request.amount = this.amount;
            request.processorArgs = this.processorArgs.toArray();
            return request;
        }

        return {
            asPaymentRequestDisplay: asPaymentRequestDisplay
        }
    }());

    angular.module('merchello.models').constant('AddPaymentDialogData', AddPaymentDialogData);

    /**
     * @ngdoc model
     * @name AddShipCountryDialogData
     * @function
     *
     * @description
     * A back office dialogData model used for adding ship countries to shipping configurations.
     */
    var AddShipCountryDialogData = function() {
        var self = this;
        self.availableCountries = [];
        self.selectedCountry = {};
    };

    angular.module('merchello.models').constant('AddShipCountryDialogData', AddShipCountryDialogData);

    /**
     * @ngdoc model
     * @name AddShipCountryProviderDialogData
     * @function
     *
     * @description
     * A back office dialogData model used for adding a shipping provider to ship countries.
     */
    var AddShipCountryProviderDialogData = function() {
        var self = this;
        self.showProvidersDropDown = true;
        self.availableProviders = [];
        self.selectedProvider = {};
        self.shipMethodName = '';
        self.selectedResource = {};
        self.country = {};
    };

    angular.module('merchello.models').constant('AddShipCountryProviderDialogData', AddShipCountryProviderDialogData);

    /**
     * @ngdoc model
     * @name BulkEditInventoryCountsDialogData
     * @function
     *
     * @description
     * A back office dialogData model used for bulk editing of inventory counts.
     */
    var BulkEditInventoryCountsDialogData = function() {
        var self = this;
        self.count = 0;
        self.includeLowCount = false;
        self.lowCount = 0;
        self.warning = '';
    };

    angular.module('merchello.models').constant('BulkEditInventoryCountsDialogData', BulkEditInventoryCountsDialogData);

    /**
     * @ngdoc model
     * @name BulkVariantChangePricesDialogData
     * @function
     *
     * @description
     * A back office dialogData model used for bulk changes to product variant prices.
     */
    var BulkVariantChangePricesDialogData = function() {
        var self = this;
        self.productVariants = [];
        self.currencySymbol = '';
        self.price = 0;
        self.includeSalePrice = false;
        self.salePrice = 0;
    };

    angular.module('merchello.models').constant('BulkVariantChangePricesDialogData', BulkVariantChangePricesDialogData);
    /**
     * @ngdoc model
     * @name PaymentRequest
     * @function
     *
     * @description
     * A back office dialogData model used for making payment requests to a payment provider
     *
     * @note
     * Presently there is not a corresponding Merchello.Web model
     */
    var CapturePaymentDialogData = function() {
        var self = this;
        self.currencySymbol = '';
        self.invoiceKey = '';
        self.paymentKey = '';
        self.payment = {};
        self.paymentMethodKey = '';
        self.invoiceBalance = 0.0;
        self.amount = 0.0;
        self.processorArgs = [];
        self.captureEditorView = '';
    };

    CapturePaymentDialogData.prototype = (function() {

        // helper method to set required associated payment info
        function setPaymentData(payment) {
            this.paymentKey = payment.key;
            this.paymentMethodKey = payment.paymentMethodKey;
            this.paymentMethodName = payment.paymentMethodName;
            this.payment = payment
        }

        //// helper method to set required associated invoice info
        function setInvoiceData(payments, invoice, currencySymbol) {
            if (invoice !== undefined) {
                this.invoiceKey = invoice.key;
                this.invoiceBalance = invoice.remainingBalance(payments);
            }
            if (currencySymbol !== undefined) {
                this.currencySymbol = currencySymbol;
            }
        }

        function isValid() {
            return this.paymentKey !== '' && this.invoiceKey !== '' && this.invoiceBalance !==0;
        }

        return {
            setPaymentData: setPaymentData,
            setInvoiceData: setInvoiceData,
            isValid: isValid
        };

    }());

    angular.module('merchello.models').constant('CapturePaymentDialogData', CapturePaymentDialogData);

    /**
     * @ngdoc model
     * @name ChangeWarehouseCatalogDialogData
     * @function
     *
     * @description
     * A back office dialogData model used for changes warehouse catalogs.
     */
    var ChangeWarehouseCatalogDialogData  = function() {
        var self = this;
        self.warehouse = {};
        self.selectedWarehouseCatalog = {};
    };

    angular.module('merchello.models').constant('ChangeWarehouseCatalogDialogData', ChangeWarehouseCatalogDialogData);
/**
 * @ngdoc model
 * @name ConfigureOfferComponentDialogData
 * @function
 *
 * @description
 * A back office dialogData model used for configuring offer components.
 */
var ConfigureOfferComponentDialogData = function() {
    var self = this;
    self.component = {};
}

ConfigureOfferComponentDialogData.prototype = (function() {

    function setValue(key, value) {
        this.component.extendedData.setValue(key, value);
    }

    function getValue(key) {
        return this.component.extendedData.getValue(key);
    }

    return {
        setValue: setValue,
        getValue: getValue
    }
}());

angular.module('merchello.models').constant('ConfigureOfferComponentDialogData');
    /**
     * @ngdoc model
     * @name CreateShipmentDialogData
     * @function
     *
     * @description
     * A back office dialogData model used for passing shipment creation information to the dialogService
     *
     * @note
     * Presently there is not a corresponding Merchello.Web model
     */
    var CreateShipmentDialogData = function() {
        var self = this;
        self.invoiceKey = '';
        self.order = {};
        self.shipmentStatuses = [];
        self.shipmentStatus = {};
        self.shipmentRequest = {};
        self.shipMethods = {};
        self.trackingNumber = '';
        self.currencySymbol = '';
    };

    angular.module('merchello.models').constant('CreateShipmentDialogData', CreateShipmentDialogData);
    /**
     * @ngdoc model
     * @name DeleteCustomerAddressDialogData
     * @function
     *
     * @description
     * A back office dialogData model used for passing customer addresses for deletion
     */
    var DeleteCustomerAddressDialogData = function() {
        var self = this;
        self.customerAddress = {};
        self.name = '';
    };

    angular.module('merchello.models').constant('DeleteCustomerAddressDialogData', DeleteCustomerAddressDialogData);
    /**
     * @ngdoc model
     * @name DeleteCustomerDialogData
     * @function
     *
     * @description
     *  A dialog data object for deleting CustomerDisplay objects
     */
    var DeleteCustomerDialogData = function() {
        var self = this;
        self.customer = {};
        self.name = '';
    };

    angular.module('merchello.models').constant('DeleteCustomerDialogData', DeleteCustomerDialogData);

    /**
     * @ngdoc model
     * @name DeleteNotificationMessageDialogData
     * @function
     *
     * @description
     * Represents a JS version of Merchello's DeleteNotificationMessageDialogData object
     */
    var DeleteNotificationMessageDialogData = function() {
        var self = this;
        self.notificationMessage = {};
        self.name = '';
    };

    angular.module('merchello.models').constant('DeleteNotificationMessageDialogData', DeleteNotificationMessageDialogData);
    /**
     * @ngdoc model
     * @name DeleteNotificationMethodDialogData
     * @function
     *
     * @description
     * Represents a JS version of Merchello's DeleteNotificationMethodDialogData object
     */
    var DeleteNotificationMethodDialogData = function() {
        var self = this;
        self.notificationMethod = {};
        self.name = '';
    };

    angular.module('merchello.models').constant('DeleteNotificationMethodDialogData', DeleteNotificationMethodDialogData);

/**
 * @ngdoc model
 * @name DeletePaymentMethodDialogData
 * @function
 *
 * @description
 * A back office dialogData model used for deleting payment methods.
 */
    var DeletePaymentMethodDialogData = function() {
        var self = this;
        self.paymentMethod = {};
        self.name = '';
    };

    angular.module('merchello.models').constant('DeletePaymentMethodDialogData', DeletePaymentMethodDialogData);

    /**
     * @ngdoc model
     * @name DeleteProductDialogData
     * @function
     *
     * @description
     * A back office dialogData model used for deleting products methods.
     */
    var DeleteProductDialogData = function() {
        var self = this;
        self.product = {};
        self.name = '';
        self.waring = '';
    };

    angular.module('merchello.models').constant('DeleteProductDialogData', DeleteProductDialogData);

    /**
     * @ngdoc model
     * @name AddShipCountryDialogData
     * @function
     *
     * @description
     * A back office dialogData model used for deleting ship countries to shipping configurations.
     */
    var DeleteShipCountryDialogData = function() {
        var self = this;
        self.country = {};
        self.name = '';
    };

    angular.module('merchello.models').constant('DeleteShipCountryDialogData', DeleteShipCountryDialogData);

    /**
     * @ngdoc model
     * @name DeleteShipCountryShipMethodDialogData
     * @function
     *
     * @description
     * A back office dialogData model used for deleting ship countries ship methods.
     */
    var DeleteShipCountryShipMethodDialogData = function() {
        var self = this;
        self.shipMethod = {};
        self.name = '';
    };

    angular.module('merchello.models').constant('DeleteShipCountryShipMethodDialogData', DeleteShipCountryShipMethodDialogData);
    /**
     * @ngdoc model
     * @name DeleteWarehouseCatalogDialogData
     * @function
     *
     * @description
     * A back office dialogData model used for deleting a warehouse catalog.
     */
    var DeleteWarehouseCatalogDialogData = function() {
        var self = this;
        self.warehouseCatalog = {};
        self.name = '';
    };

    angular.module('merchello.models').constant('DeleteWarehouseCatalogDialogData', DeleteWarehouseCatalogDialogData);

    /**
     * @ngdoc model
     * @name EditAddressDialogData
     * @function
     *
     * @description
     * A back office dialogData model used for address data to the dialogService
     *
     */
    var EditAddressDialogData = function() {
        var self = this;
        self.address = {};
        self.countries = [];
        self.selectedCountry = {};
        self.selectedProvince = {};
        self.warning = '';
    };

    angular.module('merchello.models').constant('EditAddressDialogData', EditAddressDialogData);
/**
 * @ngdoc model
 * @name EditDetachedContentTypeDialogData
 * @function
 *
 * @description
 * A back office dialogData model used for editing detached content types to the dialogService
 *
 */
var EditDetachedContentTypeDialogData = function() {
    var self = this;
    self.contentType = {};
}

angular.module('merchello.models').constant('EditDetachedContentTypeDialogData', EditDetachedContentTypeDialogData);
    /**
     * @ngdoc model
     * @name EditShipmentDialogData
     * @function
     *
     * @description
     * A back office dialogData model used for shipment data to the dialogService
     *
     */
    var EditShipmentDialogData = function() {
        var self = this;
        self.shipment = {};
        self.shipmentStatuses = [];
        self.showPhone = false;
        self.showEmail = false;
        self.showIsCommercial = false;
    };

    angular.module('merchello.models').constant('EditShipmentDialogData', EditShipmentDialogData);
    /**
     * @ngdoc model
     * @name EditShippingGatewayMethodDialogData
     * @function
     *
     * @description
     * A back office dialogData model used for shipment gateway method data to the dialogService
     *
     */
    var EditShippingGatewayMethodDialogData = function() {
        var self = this;
        self.shippingGatewayMethod = {};
        self.currencySymbol = '';
    };

    angular.module('merchello.models').constant('EditShippingGatewayMethodDialogData', EditShippingGatewayMethodDialogData);
    /**
     * @ngdoc model
     * @name EditTaxCountryDialogData
     * @function
     *
     * @description
     * A back office dialogData model used for editing a tax country
     *
     */
    var EditTaxCountryDialogData = function() {
        var self = this;
        self.country = {};
    };

   angular.module('merchello.models').constant('EditTaxCountryDialogData', EditTaxCountryDialogData);

    /**
     * @ngdoc model
     * @name ProcessRefundPaymentDialogData
     * @function
     *
     * @description
     * Dialog data model for refunding payments
     */
    var ProcessRefundPaymentDialogData = function() {
        var self = this;
        self.invoiceKey = '';
        self.paymentMethodKey = '';
        self.paymentKey = '';
        self.amount = 0;
        self.currencySymbol = '';
        self.paymentMethodName = '';
        self.appliedAmount = 0;
        self.processorArgumentCollectionDisplay = new ProcessorArgumentCollectionDisplay();
        self.warning = '';
    };

    ProcessRefundPaymentDialogData.prototype = (function() {
        function toPaymentRequestDisplay() {
            var paymentRequest = angular.extend(this, PaymentRequestDisplay);
            paymentRequest.processorArgs = this.processorArgumentCollectionDisplay.toArray();
            return paymentRequest;
        }

        return {
            toPaymentRequestDisplay: toPaymentRequestDisplay
        }
    }());

    angular.module('merchello.models').constant('ProcessRefundPaymentDialogData', ProcessRefundPaymentDialogData);
    /**
     * @ngdoc model
     * @name ProcessVoidPaymentDialogData
     * @function
     *
     * @description
     * Dialog data model for voiding payments
     */
    var ProcessVoidPaymentDialogData = function() {
        var self = this;
        self.invoiceKey = '';
        self.paymentMethodKey = '';
        self.paymentKey = '';
        self.processorArgumentCollectionDisplay = new ProcessorArgumentCollectionDisplay();
        self.warning = '';
    };

    ProcessVoidPaymentDialogData.prototype = (function() {

        function toPaymentRequestDisplay() {
            var paymentRequest = angular.extend(this, PaymentRequestDisplay);
            paymentRequest.processorArgs = this.processorArgumentCollectionDisplay.toArray();
            return paymentRequest;
        }

        return {
            toPaymentRequestDisplay : toPaymentRequestDisplay
        }

    }());

    angular.module('merchello.models').constant('ProcessVoidPaymentDialogData', ProcessVoidPaymentDialogData);

    /**
     * @ngdoc model
     * @name ProductSelectorDialogData
     * @function
     *
     * @description
     * A dialogData model for use in the product selector
     *
     */
    var ProductSelectorDialogData = function() {
        var self = this;
        self.product = {};
    };

    angular.module('merchello.models').constant('ProductSelectorDialogData', ProductSelectorDialogData);

/**
 * @ngdoc model
 * @name SelectOfferProviderDialogData
 * @function
 *
 * @description
 * A dialogData model for use in the selecting an offer provider
 *
 */
var SelectOfferProviderDialogData = function() {
    var self = this;
    self.offerProviders = [];
    self.selectedProvider = {};
    self.warning = '';
};

angular.module('merchello.models').constant('SelectOfferProviderDialogData', SelectOfferProviderDialogData);
    /**
     * @ngdoc model
     * @name GatewayProviderDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's GatewayProviderDisplay object
     */
    var GatewayProviderDisplay = function() {
        var self = this;
        self.key = '';
        self.providerTfKey = '';
        self.name = '';
        self.description = '';
        self.extendedData = {};
        self.encryptExtendedData = false;
        self.activated = false;
        self.dialogEditorView = {};
    };

    angular.module('merchello.models').constant('GatewayProviderDisplay', GatewayProviderDisplay);
    /**
     * @ngdoc model
     * @name GatewayResourceDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's GatewayResourceDisplay object
     */
    var GatewayResourceDisplay = function() {
        self.name = '';
        self.serviceCode = '';
    };

    GatewayResourceDisplay.prototype = (function() {

        function serviceCodeStartsWith(str) {
            return this.serviceCode.indexOf(str) === 0;
        }

        return {
            serviceCodeStartsWith: serviceCodeStartsWith
        };

    }());

    angular.module('merchello.models').constant('GatewayResourceDisplay', GatewayResourceDisplay);
    /**
     * @ngdoc model
     * @name InvoiceLineItemDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's InvoiceLIneItemDisplay object
     */
    var InvoiceLineItemDisplay = function() {
        var self = this;

        self.key = '';
        self.containerKey = '';
        self.lineItemTfKey = '';
        self.lineItemType = '';
        self.lineItemTypeField = {};  // TODO why is this here
        self.sku = '';
        self.name = '';
        self.quantity = '';
        self.price = '';
        self.exported = false;
        self.extendedData = {};
    };

    angular.module('merchello.models').constant('InvoiceLineItemDisplay', InvoiceLineItemDisplay);
    /**
     * @ngdoc model
     * @name OrderLineItemDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's OrderLIneItemDisplay object
     */
    var OrderLineItemDisplay = function() {
        var self = this;

        self.key = '';
        self.containerKey = '';
        self.lineItemTfKey = '';
        self.lineItemTypeField = {};
        self.sku = '';
        self.name = '';
        self.quantity = 0;
        self.price = 0;
        self.exported = false;
        self.lineItemType = '';
        self.shipmentKey = '';
        self.backOrder = false;
        self.extendedData = [];
    };

    OrderLineItemDisplay.prototype = (function() {

        function getProductVariantKey() {
            var variantKey = '';
            if (this.extendedData.length > 0) {
                variantKey = _.find(self.extendedData, function(extDataItem) {
                    return extDataItem['key'] === "merchProductVariantKey";
                });
            }
            if (variantKey === undefined) {
                variantKey = '';
            }
            return variantKey;
        }

    }());

    angular.module('merchello.models').constant('OrderLineItemDisplay', OrderLineItemDisplay);
/**
 * @ngdoc model
 * @name OfferComponentDefinitionDisplay
 * @function
 *
 * @description
 * Represents a JS version of Merchello's OfferComponentDefinitionDisplay object
 */
var OfferComponentDefinitionDisplay = function() {
    var self = this;
    self.offerSettingsKey = '';
    self.offerCode = '';
    self.componentKey = '';
    self.name = '';
    self.description = '';
    self.typeFullName = '';
    self.typeGrouping = '';
    self.displayConfigurationFormat = '';
    self.extendedData = {};
    self.componentType = '';
    self.dialogEditorView = {};
    self.restrictToType = '';
    self.requiresConfiguration = true;
    self.updated = false;
};

OfferComponentDefinitionDisplay.prototype = (function() {

    function clone() {
        return angular.extend(new OfferComponentDefinitionDisplay(), this);
    }

    function isConfigured() {

        if(!this.requiresConfiguration) {
            return true;
        }
        // hack catch for save call where there's a context switch on this to window
        // happens when saving the offer settings
        if (this.extendedData.items !== undefined) {
            return !this.extendedData.isEmpty();
        } else {
            return true;
        }
    }

    return {
        clone: clone,
        isConfigured: isConfigured
    }
}());

angular.module('merchello.models').constant('OfferComponentDefinitionDisplay', OfferComponentDefinitionDisplay);
/**
 * @ngdoc model
 * @name OfferProviderDisplay
 * @function
 *
 * @description
 * Represents a JS version of Merchello's OfferProviderDisplay object
 */
var OfferProviderDisplay = function() {
    var self = this;
    self.key = '';
    self.managesTypeName = '';
    self.backOfficeTree = {};
};

OfferProviderDisplay.prototype = (function() {

    function editorUrl(key) {
        return this.backOfficeTree.routePath.replace('{0}', key);
    }

    return {
        editorUrl : editorUrl
    }
}());

angular.module('merchello.models').constant('OfferProviderDisplay', OfferProviderDisplay);
    /**
     * @ngdoc model
     * @name OfferSettingsDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's OfferSettingsDisplay object
     */
    var OfferSettingsDisplay = function() {
        var self = this;
        self.key = '';
        self.name = '';
        self.offerCode = '';
        self.offerProviderKey = '';
        self.offerExpires = false;
        self.offerStartsDate = '';
        self.offerEndsDate = '';
        self.expired = false;
        self.hasStarted = false;
        self.active = false;
        self.dateFormat = '';  // used to pass back office format to server for parse exact.
        self.componentDefinitions = [];
    };

    OfferSettingsDisplay.prototype = (function() {

        function clone() {
            return angular.extend(new OfferSettingsDisplay(), this);
        }

        // private methods
        function getAssignedComponent(componentKey) {
            return _.find(this.componentDefinitions, function (cd) { return cd.componentKey === componentKey; });
        }

        // adjusts date with timezone
        function localDateString(val) {
            var raw = new Date(val);
            return new Date(raw.getTime() + raw.getTimezoneOffset()*60000).toLocaleDateString();
        }

        // gets the local start date string
        function offerStartsDateLocalDateString() {
            return localDateString(this.offerStartsDate);
        }

        // gets the local end date string
        function offerEndsDateLocalDateString() {
            return localDateString(this.offerEndsDate);
        }

        function componentDefinitionExtendedDataToArray() {
            angular.forEach(this.componentDefinitions, function(cd) {
                if (!angular.isArray(cd.extendedData)) {
                    cd.extendedData = cd.extendedData.toArray();
                }
            });
        }

        // returns true if the offer has rewards assigned
        function hasRewards() {
            if(!hasComponents.call(this)) {
                return false;
            }
            var reward = _.find(this.componentDefinitions, function(c) { return c.componentType === 'Reward'; } );
            return reward !== undefined && reward !== null;
        }

        function getReward() {
            return _.find(this.componentDefinitions, function(c) { return c.componentType === 'Reward'; } );
        }

        function componentsConfigured() {
            if (!hasComponents.call(this)) {
                return true;
            }
            var notConfigured = _.find(this.componentDefinitions, function(c) { return c.isConfigured() === false});
            return notConfigured === undefined;
        }

        function hasComponents() {
            return this.componentDefinitions.length > 0;
        }

        // gets the current component type grouping
        function getComponentsTypeGrouping() {
            return hasComponents.call(this) ? this.componentDefinitions[0].typeGrouping : '';
        }

        // ensures that all components work with the same type of objects.
        function ensureTypeGrouping(typeGrouping) {
            if(!hasComponents.call(this)) {
                return true;
            }
            return this.componentDefinitions[0].typeGrouping === typeGrouping;
        }

        function assignComponent(component) {
            var exists =_.find(this.componentDefinitions, function(cd) { return cd.componentKey === component.componentKey; });
            if (exists === undefined && ensureTypeGrouping.call(this, component.typeGrouping)) {
                component.offerCode = this.offerCode;
                component.offerSettingsKey = this.key;
                if (component.componentType === 'Reward') {
                    this.componentDefinitions.unshift(component);
                }
                else
                {
                    this.componentDefinitions.push(component);
                }

                return true;
            }
            return false;
        }

        function updateAssignedComponent(component) {
            var assigned = getAssignedComponent.call(this, component.componentKey);
            if (assigned !== undefined && assigned !== null) {
                assigned.extendedData = component.extendedData;
                assigned.updated = true;
            }
        }

        function setLineItemName(value) {
            if (hasRewards.call(this)) {
                var reward = getReward.call(this);
                reward.extendedData.setValue('lineItemName', value);
            }
        }

        function getLineItemName() {
            if(hasRewards.call(this)) {
                var reward = getReward.call(this);
                var name = reward.extendedData.getValue('lineItemName');
                if (name === '') {
                    name = reward.name;
                }
                return name;
            } else {
                return '';
            }
        }

        function validateComponents() {
            var offerCode = this.offerCode;
            var offerSettingsKey = this.key;
            var invalid = _.filter(this.componentDefinitions, function (cd) { return cd.offerSettingsKey !== this.key || cd.offerCode !== this.offerCode; });
            if (invalid !== undefined) {
                angular.forEach(invalid, function(fix) {
                    fix.offerSettingsKey = offerSettingsKey;
                    fix.offerCode = offerCode;
                });
            }
        }

        function reorderComponent(oldIndex, newIndex) {
            this.componentDefinitions.splice(newIndex, 0, this.componentDefinitions.splice(oldIndex, 1)[0]);
        }

        return {
            clone: clone,
            offerStartsDateLocalDateString: offerStartsDateLocalDateString,
            offerEndsDateLocalDateString: offerEndsDateLocalDateString,
            componentDefinitionExtendedDataToArray: componentDefinitionExtendedDataToArray,
            hasComponents: hasComponents,
            getComponentsTypeGrouping: getComponentsTypeGrouping,
            ensureTypeGrouping: ensureTypeGrouping,
            hasRewards: hasRewards,
            getReward: getReward,
            assignComponent: assignComponent,
            updateAssignedComponent: updateAssignedComponent,
            getAssignedComponent: getAssignedComponent,
            componentsConfigured: componentsConfigured,
            getLineItemName: getLineItemName,
            setLineItemName: setLineItemName,
            validateComponents: validateComponents,
            reorderComponent: reorderComponent
        }

    }());

    angular.module('merchello.models').constant('OfferSettingsDisplay', OfferSettingsDisplay);
    /**
     * @ngdoc model
     * @name NotificationGatewayProviderDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's NotificationGatewayProviderDisplay object
     */
    var NotificationGatewayProviderDisplay = function() {
        var self = this;
        self.key = '';
        self.name = '';
        self.providerTfKey = '';
        self.description = '';
        self.extendedData = {};
        self.encryptExtendedData = false;
        self.activated = false;
        self.showSelectedResource = false;
        self.dialogEditorView = {};
        self.gatewayResources = [];
        self.notificationMethods = [];
    };

    angular.module('merchello.models').constant('NotificationGatewayProviderDisplay', NotificationGatewayProviderDisplay);
    /**
     * @ngdoc model
     * @name NotificationMessageDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's NotificationMessageDisplay object
     */
    var NotificationMessageDisplay = function() {
        var self = this;
        self.key = '';
        self.name = '';
        self.description = '';
        self.fromAddress = '';
        self.replyTo = '';
        self.bodyText = '';
        self.maxLength = '';
        self.bodyTextIsFilePath = false;
        self.monitorKey = '';
        self.methodKey = '';
        self.recipients = '';
        self.sendToCustomer = '';
        self.disabled = false;
    };

    angular.module('merchello.models').constant('NotificationMessageDisplay', NotificationMessageDisplay);
    /**
     * @ngdoc model
     * @name NotificationMethodDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's NotificationMethodDisplay object
     */
    var NotificationMethodDisplay = function() {
        var self = this;
        self.key = '';
        self.name = '';
        self.providerKey = '';
        self.description = '';
        self.serviceCode = '';
        self.notificationMessages = [];
        self.dialogEditorView = {};
    };

    angular.module('merchello.models').constant('NotificationMethodDisplay', NotificationMethodDisplay);
    /**
     * @ngdoc model
     * @name NotificationMonitorDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's NotificationMonitorDisplay object
     */
    var NotificationMonitorDisplay = function() {
        var self = this;
        self.monitorKey = '';
        self.name = '';
        self.alias = '';
    };

    angular.module('merchello.models').constant('NotificationMonitorDisplay', NotificationMonitorDisplay);

    /**
     * @ngdoc model
     * @name AppliedPaymentDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's AppliedPaymentDisplay object
     */
    var AppliedPaymentDisplay = function() {
        var self = this;
        self.key = '';
        self.paymentKey = '';
        self.invoiceKey = '';
        self.appliedPaymentTfKey = '';
        self.description = '';
        self.amount = 0.0;
        self.exported = false;
        self.createDate = '';
    };

    angular.module('merchello.models').constant('AppliedPaymentDisplay', AppliedPaymentDisplay);

    /**
     * @ngdoc model
     * @name PaymentDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's PaymentDisplay object
     */
    var PaymentDisplay = function() {
        var self = this;
        self.key = '';
        self.customerKey = '';
        self.paymentMethodKey = '';
        self.paymentTypeFieldKey = '';
        self.paymentMethodType = '';
        self.paymentMethodName = '';
        self.referenceNumber = '';
        self.amount = 0.0;
        self.authorized = false;
        self.voided = false;
        self.collected = false;
        self.exported = false;
        self.extendedData = {};
        self.appliedPayments = [];
    };

    PaymentDisplay.prototype = (function() {

        // private
        function getStatus() {
            var statusArr = [];
            if (this.authorized) {
                statusArr.push("Authorized");
            }
            if (this.collected) {
                statusArr.push("Captured");
            }
            if (this.exported) {
                statusArr.push("Exported");
            }

            return statusArr.join("/");
        }

        function hasAmount() {
            return this.amount > 0;
        }

        function appliedAmount() {
            var applied = 0;
            angular.forEach(this.appliedPayments, function(ap) {
                applied += ap.amount;
            });
            return applied;
        }

        // public
        return {
            getStatus: getStatus,
            hasAmount: hasAmount,
            appliedAmount: appliedAmount
        };
    }());

    angular.module('merchello.models').constant('PaymentDisplay', PaymentDisplay);
    /**
     * @ngdoc model
     * @name PaymentGatewayProviderDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's PaymentGatewayProviderDisplay object
     */
    var PaymentGatewayProviderDisplay = function() {
        var self = this;
        self.key = '';
        self.name = '';
        self.providerTfKey = '';
        self.description = '';
        self.extendedData = {};
        self.gatewayResource = {};
        self.encryptExtendedData = false;
        self.activated = false;
        self.gatewayResources = [];
        self.paymentMethods = [];
        self.dialogEditorView = {};
    };

    angular.module('merchello.models').constant('PaymentGatewayProviderDisplay', PaymentGatewayProviderDisplay);

    /**
     * @ngdoc model
     * @name PaymentMethodDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's PaymentMethodDisplay object
     */
    var PaymentMethodDisplay = function() {
        var self = this;
        self.key = '';
        self.name = '';
        self.providerKey = '';
        self.description = '';
        self.paymentCode = '';
        self.dialogEditorView = {};
        self.authorizePaymentEditorView = {};
        self.authorizeCapturePaymentEditorView = {};
        self.voidPaymentEditorView = {};
        self.refundPaymentEditorView = {};
        self.capturePaymentEditorView = {};
        self.includeInPaymentSelection = true;
        self.requiresCustomer = false;
    };

    angular.module('merchello.models').constant('PaymentMethodDisplay', PaymentMethodDisplay);
    /**
     * @ngdoc model
     * @name PaymentRequestDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's PaymentRequestDisplay object
     */
    var PaymentRequestDisplay = function() {
        var self = this;
        self.invoiceKey = '';
        self.paymentKey = '';
        self.paymentMethodKey = '';
        self.amount = 0.0;
        self.processorArgs = [];
    };

    angular.module('merchello.models').constant('PaymentRequestDisplay', PaymentRequestDisplay);
    /**
     * @ngdoc model
     * @name ProcessorArgumentCollectionDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's ProcessorArgumentCollectionDisplay object
     */
     var ProcessorArgumentCollectionDisplay = function() {
        var self = this;
        self.items = [];
    };

    ProcessorArgumentCollectionDisplay.prototype = (function() {

        function toArray() {
            return this.items;
        };

        function isEmpty() {
            return this.items.length === 0;
        }

        function getValue(key) {
            if (isEmpty.call(this)) {
                return;
            }
            var found = false;
            var i = 0;
            var value = '';
            while(i < this.items.length && !found) {
                if (this.items[ i ].key === key) {
                    found = true;

                    value = this.items[ i ].value;
                } else {
                    i++;
                }
            }
            return value;
        }

        function setValue(key, value) {

            var existing = _.find(this.items, function(item) {
                return item.key === key;
            });
            if(existing) {
                existing.value = value;
                return;
            }

            this.items.push({ key: key, value: value });
        }

        return {
            toArray: toArray,
            getValue: getValue,
            setValue: setValue
        }

    }());

    angular.module('merchello.models').constant('ProcessorArgumentCollectionDisplay', ProcessorArgumentCollectionDisplay);
    /**
     * @ngdoc model
     * @name CatalogInventoryDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's CatalogInventoryDisplay object
     */
    var CatalogInventoryDisplay = function() {
        var self = this;
        self.productVariantKey = '';
        self.catalogKey = '';
        self.count = 0;
        self.lowCount = 0;
        self.location = '';
        self.update = new Date();
        self.active = true;
    };

    angular.module('merchello.models').constant('CatalogInventoryDisplay', CatalogInventoryDisplay);
    /**
     * @ngdoc model
     * @name ProductAttributeDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's ProductAttributeDisplay object
     */
    var ProductAttributeDisplay = function() {
        var self = this;
        self.key = '';
        self.optionKey = '';
        self.name = '';
        self.sku = '';
        self.sortOrder = 0;
    };

    angular.module('merchello.models').constant('ProductAttributeDisplay', ProductAttributeDisplay);

    /**
     * @ngdoc model
     * @name ProductAttributeDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's ProductDisplay object
     */
    var ProductDisplay = function() {
        var self = this;
        self.key = '';
        self.productVariantKey = '';
        self.versionKey = '';
        self.name = '';
        self.sku = '';
        self.price = 0.00;
        self.costOfGoods = 0.00;
        self.salePrice = 0.00;
        self.onSale = false;
        self.manufacturer = '';
        self.manufacturerModelNumber = '';
        self.weight = 0;
        self.length = 0;
        self.width = 0;
        self.height = 0;
        self.barcode = "";
        self.available = true;
        self.trackInventory = false;
        self.outOfStockPurchase = false;
        self.taxable = false;
        self.shippable = false;
        self.download = false;
        //self.master = true;
        self.downloadMediaId = -1;
        self.productOptions = [];
        self.productVariants = [];
        self.catalogInventories = [];
        self.detachedContents = [];
    };

    ProductDisplay.prototype = (function() {

        // returns a product variant with the associated key
        function getProductVariant(productVariantKey) {
            return _.find(this.productVariants, function(v) { return v.key === productVariantKey; });
        }

        // returns a value indicating whether or not the product has variants
        function hasVariants() {
            return this.productVariants.length > 0;
        }

        // gets the master variant that represents a product without variants
        function getMasterVariant() {
            var variant = new ProductVariantDisplay();
            angular.extend(variant, this);
            // clean up
            variant.key = this.productVariantKey;
            variant.productKey = this.key;
            variant.master = true;
            delete variant['productOptions'];
            delete variant['productVariants'];
            return variant;
        }

        // returns a count of total inventory. if product has variants sums all inventory otherwise uses
        // the product inventory count
        function totalInventory() {
            var inventoryCount = 0;
            if (hasVariants.call(this)) {
                var anyTracksInventory = false;
                angular.forEach(this.productVariants, function(pv) {
                    if(pv.trackInventory) {
                        anyTracksInventory = true;
                        angular.forEach(pv.catalogInventories, function(ci) {
                            inventoryCount += ci.count;
                        });
                    }
                });
                if(!anyTracksInventory) {
                    inventoryCount = "n/a";
                }
            } else {
                if(this.trackInventory) {
                    angular.forEach(this.catalogInventories, function(ci) {
                        inventoryCount += ci.count;
                    });
                } else {
                    inventoryCount = "n/a";
                }
            }
            return inventoryCount;
        }

        // adds an empty options
        function addEmptyOption() {
            var option = new ProductOptionDisplay();
            this.productOptions.push(option);
        }

        // removes an option
        function removeOption(option) {
            this.productOptions = _.reject(this.productOptions, function(opt) { return _.isEqual(opt, option); });
        }


        // finds the minimum variant price or sales price
        function variantsMinimumPrice(salePrice) {
            if (this.productVariants.length > 0) {
                if (salePrice === undefined) {
                    return _.min(this.productVariants, function(v) { return v.price; }).price;
                } else {
                    var onSaleVariants = _.filter(this.productVariants, function(osv) { return osv.onSale; });
                    if(onSaleVariants.length > 0) {
                        salePrice = _.min(onSaleVariants,
                            function(v) { return v.salePrice; }
                        ).salePrice;
                        return salePrice;
                    } else {
                        return 0;
                    }
                }
            } else {
                return 0;
            }
        }

        // finds the maximum variant price or sales price
        function variantsMaximumPrice(salePrice) {
            if (this.productVariants.length > 0) {
                if(salePrice === undefined) {
                    return _.max(this.productVariants, function(v) { return v.price; }).price;
                } else {
                    var onSaleVariants = _.filter(this.productVariants, function(osv) { return osv.onSale; });
                    if(onSaleVariants.length > 0) {
                        return _.max(
                            onSaleVariants,
                            function (v) {
                                return v.salePrice;
                            }
                        ).salePrice;
                    } else {
                        return 0;
                    }
                }
            } else {
                return 0;
            }
        }

        // returns a value indicating whether or not any variants are on sale
        function anyVariantsOnSale() {
            var variant = _.find(this.productVariants, function(v) { return v.onSale; });
            return variant === undefined ? false : true;
        }

        // returns a collection of shippable variants
        function shippableVariants() {
            return _.filter(this.productVariants, function(v) { return v.shippable; });
        }

        // returns a collection of taxable variants
        function taxableVariants() {
            return _.filter(this.productVariants, function(v) { return v.taxable; });
        }

        // returns a value indicating whether or not the product has a detached content that can be rendered.
        function canBeRendered() {
            if (!this.available) {
                return false;
            }
            if (this.detachedContents.length === 0) {
                return false;
            }
            if (_.filter(this.detachedContents, function(dc) { return dc.canBeRendered; }).length === 0) {
                return false;
            }
            return true;
        }

        return {
            hasVariants: hasVariants,
            totalInventory: totalInventory,
            getMasterVariant: getMasterVariant,
            addEmptyOption: addEmptyOption,
            removeOption: removeOption,
            variantsMinimumPrice: variantsMinimumPrice,
            variantsMaximumPrice: variantsMaximumPrice,
            anyVariantsOnSale: anyVariantsOnSale,
            shippableVariants: shippableVariants,
            getProductVariant: getProductVariant,
            taxableVariants: taxableVariants,
            canBeRendered: canBeRendered
        };
    }());

    angular.module('merchello.models').constant('ProductDisplay', ProductDisplay);
    /**
     * @ngdoc model
     * @name ProductAttributeDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's ProductOptionDisplay object
     */
    var ProductOptionDisplay = function() {
        var self = this;
        self.key = '';
        self.name = '';
        self.required = true;
        self.sortOrder = 1;
        self.choices = [];
    };

    ProductOptionDisplay.prototype = (function() {

        function addAttributeChoice(choiceName) {
            var attribute = new ProductAttributeDisplay();
            attribute.name = choiceName;
            attribute.sortOrder = this.choices.length + 1;
            // TODO skus
            this.choices.push(attribute);
        }

        // removes the product options choice
        function removeAttributeChoice(idx) {
            console.info(idx);
            if(idx >= 0) {
                this.choices.splice(idx, 1);
            }
        }

        // resets the product options choice sort order
        function resetChoiceSortOrders() {
            for (var i = 0; i < this.choices.length; i++) {
                this.choices[i].sortOrder = i + 1;
            }
        }

        return {
            addAttributeChoice: addAttributeChoice,
            removeAttributeChoice: removeAttributeChoice,
            resetChoiceSortOrders: resetChoiceSortOrders
        };
    }());

    angular.module('merchello.models').constant('ProductOptionDisplay', ProductOptionDisplay);
/**
 * @ngdoc model
 * @name ProductVariantDetachedContentDisplay
 * @function
 *
 * @description
 * Represents a JS version of Merchello's ProductVariantDetachedContentDisplay object
 */
var ProductVariantDetachedContentDisplay = function() {
    var self = this;
    self.key = '';
    self.detachedContentType = {};
    self.productVariantKey = '';
    self.cultureName = '';
    self.templateId = 0;
    self.slug = '';
    self.canBeRendered = true;
    self.detachedDataValues = {};
    self.uploadedFiles = [];
};


angular.module('merchello.models').constant('ProductVariantDetachedContentDisplay', ProductVariantDetachedContentDisplay);
    /**
     * @ngdoc model
     * @name ProductAttributeDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's ProductVariantDisplay object
     */
    var ProductVariantDisplay = function() {
        var self = this;
        self.key = '';
        self.productKey = '';
        self.versionKey = '';
        self.name = '';
        self.sku = '';
        self.price = 0.00;
        self.costOfGoods = 0.00;
        self.salePrice = 0.00;
        self.onSale = false;
        self.manufacturer = '';
        self.manufacturerModelNumber = '';
        self.weight = 0;
        self.length = 0;
        self.width = 0;
        self.height = 0;
        self.barcode = '';
        self.available = true;
        self.trackInventory = false;
        self.outOfStockPurchase = false;
        self.taxable = false;
        self.shippable = false;
        self.download = false;
        self.master = false;
        self.downloadMediaId = -1;
        self.totalInventoryCount = 0;
        self.attributes = [];
        self.catalogInventories = [];
        self.detachedContents = [];
    };

    ProductVariantDisplay.prototype = (function() {

        function getProductForMasterVariant() {
            var product = new ProductDisplay();
            product = angular.extend(product, this);
            // do some corrections
            var pvk = product.key;
            product.key = product.productKey;
            product.productVariantKey = pvk;
            delete product['productKey'];
            delete product['attributes'];
            // remove catalog inventories that are not active
            product.catalogInventories = _.reject(product.catalogInventories, function (ci) {
                return ci.active === false;
            });
            return product;
        }

        // ensures a catalog is selected if the variant is marked shippable
        function ensureCatalogInventory(defaultCatalogKey) {
            if (!this.shippable && !this.trackInventory) {
                return;
            }
            // if this product is not associated with any catalogs we need to add the default catalog
            // so that we can associate shipping information
            if (this.catalogInventories.length === 0) {
                var inv = new CatalogInventoryDisplay();
                inv.productVariantKey = this.key;
                inv.catalogKey = defaultCatalogKey;
                inv.active = true;
                this.catalogInventories.push(inv);
            } else {
                // if there are catalogs and none are selected we need to force the default catalog to be selected.
                var activeInventories = _.filter(this.catalogInventories, function (ci) {
                    return ci.active;
                });
                if (activeInventories.length === 0) {
                    var defaultInv = _.find(this.catalogInventories, function (dci) {
                        return dci.catalogKey === defaultCatalogKey;
                    });
                    if (defaultInv !== undefined) {
                        defaultInv.active = true;
                    }
                }
            }
        }

        // removes inactive catalog inventories from a variant before save
        function removeInActiveInventories() {
            this.catalogInventories = _.reject(this.catalogInventories, function (ci) {
                return ci.active === false;
            });
        }

        // updates all inventory counts to the count passed as a parameter
        function setAllInventoryCount(count) {
            angular.forEach(this.catalogInventories, function (ci) {
                ci.count = count;
            });
        }

        // updates all inventory low count to the low count passed as a parameter
        function setAllInventoryLowCount(lowCount) {
            angular.forEach(this.catalogInventories, function (ci) {
                ci.lowCount = lowCount;
            });
        }

        // TODO something like this could be used to copy productOptions from one product to another.
        // TODO: this method is not used
        function deepClone() {
            var variant = new ProductVariantDisplay();
            variant = angular.extend(variant, this);
            variant.attributes = [];
            angular.forEach(this.attributes, function (att) {
                var attribute = new ProductAttributeDisplay();
                angular.extend(attribute, att);
                variant.attributes.push(attribute);
            });
            variant.catalogInventories = [];
            angular.forEach(this.catalogInventories, function (ci) {
                var inv = new CatalogInventoryDisplay();
                angular.extend(inv, ci);
                variant.catalogInventories.push(ci);
            });
            return variant;
        }

        function hasDetachedContent() {
            return this.detachedContents.length > 0;
        }

        function getDetachedContent(isoCode) {
            if (!this.hasDetachedContent.call(this)) {
                return null;
            } else {
                return _.find(this.detachedContents, function(dc) {
                  return dc.cultureName === isoCode;
                });
            }
        }

        function detachedContentType() {
            if (!this.hasDetachedContent.call(this)) {
                return null;
            } else {
                return this.detachedContents[0].detachedContentType;
            }
        }

        function assertLanguageContent(isoCodes) {
            var self = this;
            var missing = [];
            var removers = [];
            _.each(self.detachedContents, function(dc) {
              var found = _.find(isoCodes, function(code) {
                return code === dc.cultureName;
                });
                if (found === undefined) {
                    removers.push(dc.cultureName);
                }
            });

            angular.forEach(removers, function(ic) {
                this.detachedContents = _.reject(self.detachedContents, function(oust) {
                    return oust.cultureName === ic;
                });
            });

            missing = _.filter(isoCodes, function(check) {
                var fnd = _.find(self.detachedContents, function(dc) {
                  return dc.cultureName === check;
                });
                return fnd === undefined;
            });
            return missing;
        }

        return {
            getProductForMasterVariant: getProductForMasterVariant,
            ensureCatalogInventory: ensureCatalogInventory,
            removeInActiveInventories: removeInActiveInventories,
            setAllInventoryCount: setAllInventoryCount,
            setAllInventoryLowCount: setAllInventoryLowCount,
            hasDetachedContent: hasDetachedContent,
            assertLanguageContent: assertLanguageContent,
            detachedContentType: detachedContentType,
            getDetachedContent: getDetachedContent
        };
    }());

    angular.module('merchello.models').constant('ProductVariantDisplay', ProductVariantDisplay);

    /**
     * @ngdoc model
     * @name QueryDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's QueryDisplay object
     *
     * @remark
     * PetaPoco Page<T> uses a 1 based page rather than a 0 based to represent the first page.
     * We do the conversion in the WebApiController - so the JS QueryDisplay should assume this is 0 based.
     */
    var QueryDisplay = function() {
        var self = this;
        self.currentPage = 0;
        self.itemsPerPage = 25;
        self.parameters = [];
        self.sortBy = '';
        self.sortDirection = 'Ascending'; // valid options are 'Ascending' and 'Descending'
    };

    QueryDisplay.prototype = (function() {
        // private
        function addParameter(queryParameter) {
            this.parameters.push(queryParameter);
        }

        function addCustomerKeyParam(customerKey) {
            var param = new QueryParameterDisplay();
            param.fieldName = 'customerKey';
            param.value = customerKey;
            addParameter.call(this, param);
        }

        function addInvoiceDateParam(dateString, startOrEnd) {
            var param = new QueryParameterDisplay();
            param.fieldName = startOrEnd === 'start' ? 'invoiceDateStart' : 'invoiceDateEnd';
            param.value = dateString;
            addParameter.call(this, param);
        }

        function addCollectionKeyParam(collectionKey) {
            var param = new QueryParameterDisplay();
            param.fieldName = 'collectionKey';
            param.value = collectionKey;
            addParameter.call(this, param);
        }

        function hasCollectionKeyParam() {
            var fnd = _.find(this.parameters, function(p) {
                return p.fieldName === 'collectionKey';
            });
            return fnd !== undefined;
        }

        function addEntityTypeParam(entityType) {
            var param = new QueryParameterDisplay();
            param.fieldName = 'entityType';
            param.value = entityType;
            addParameter.call(this, param);
        }

        function addFilterTermParam(term) {
            if(term === undefined || term.length <= 0) {
                return;
            }
            var param = new QueryParameterDisplay();
            param.fieldName = 'term';
            param.value = term;
            addParameter.call(this, param);
        }

        function applyInvoiceQueryDefaults() {
            this.sortBy = 'invoiceNumber';
            this.sortDirection = 'Descending';
        }

        // public
        return {
            addParameter: addParameter,
            addCustomerKeyParam: addCustomerKeyParam,
            addCollectionKeyParam: addCollectionKeyParam,
            addEntityTypeParam: addEntityTypeParam,
            applyInvoiceQueryDefaults: applyInvoiceQueryDefaults,
            addInvoiceDateParam: addInvoiceDateParam,
            addFilterTermParam: addFilterTermParam,
            hasCollectionKeyParam: hasCollectionKeyParam
        };
    }());

    angular.module('merchello.models').constant('QueryDisplay', QueryDisplay);
    /**
     * @ngdoc model
     * @name QueryParameterDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's ListQueryParameterDisplay object
     */
    var QueryParameterDisplay = function()
    {
        var self = this;
        self.fieldName = '';
        self.value = '';
    };

    angular.module('merchello.models').constant('QueryParameterDisplay', QueryParameterDisplay);

    /**
     * @ngdoc model
     * @name QueryResultDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's QueryResultDisplay object
     */
    var QueryResultDisplay = function() {
        var self = this;
        self.currentPage = 0;
        self.items = [];
        self.itemsPerPage = 0;
        self.totalItems = 0;
        self.totalPages = 0;
    };

    QueryResultDisplay.prototype = (function() {
        function addItem(item) {
            this.items.push(item);
        }

        return {
            addItem: addItem
        };
    }());

    angular.module('merchello.models').constant('QueryResultDisplay', QueryResultDisplay);
    /**
     * @ngdoc model
     * @name InvoiceDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's InvoiceDisplay object
     */
    var InvoiceDisplay = function() {
        var self = this;
        self.key = '';
        self.versionKey = '';
        self.customerKey = '';
        self.invoiceNumberPrefix = '';
        self.invoiceNumber = '';
        self.invoiceDate = '';
        self.invoiceStatusKey = '';
        self.invoiceStatus = {};
        self.billToName = '';
        self.billToAddress1 = '';
        self.billToAddress2 = '';
        self.billToLocality = '';
        self.billToRegion = '';
        self.billToPostalCode = '';
        self.billToCountryCode = '';
        self.billToEmail = '';
        self.billToPhone = '';
        self.billToCompany = '';
        self.exported = '';
        self.archived = '';
        self.total = 0.0;
        self.currency = {};
        self.items = [];
        self.orders = [];
    };

    InvoiceDisplay.prototype = (function() {

        function getBillingAddress() {
            var adr = new AddressDisplay();
            adr.address1 = this.billToAddress1;
            adr.address2 = this.billToAddress2;
            adr.locality = this.billToLocality;
            adr.region = this.billToRegion;
            adr.countryCode = this.billToCountryCode;
            adr.postalCode = this.billToPostalCode;
            adr.name = this.billToName;
            adr.phone = this.billToPhone;
            adr.email = this.billToEmail;
            adr.addressType = 'Billing';
            adr.organization = this.billToCompany;
            return adr;
        }

        function setBillingAddress(adr) {
            this.billToAddress1 = adr.address1;
            this.billToAddress2 = adr.address2;
            this.billToLocality = adr.locality;
            this.billToRegion = adr.region;
            this.billToCountryCode = adr.countryCode;
            this.billToPostalCode = adr.postalCode;
            this.billToName = adr.name;
            this.billToEmail = adr.email;
            this.billToCompany = adr.organization;
        }

        // gets the invoice date as a date string
        function invoiceDateString() {
            return this.invoiceDate.split('T')[0];
        }

        // gets the invoice status name
        // TODO this is incorrectly named
        function getPaymentStatus() {
            return this.invoiceStatus.name;
        }

        function getFulfillmentStatus () {
            if (!_.isEmpty(this.orders)) {
                return this.orders[0].orderStatus.name;
            }
            // TODO this should be localized
            return 'Not Fulfilled';
        }

        // gets the currency code for the invoice
        function getCurrencyCode() {
            if (this.currency.currencyCode === '') {
                var first = this.items[0];
                var currencyCode = first.extendedData.getValue('merchCurrencyCode');
                return currencyCode;
            } else {
                return this.currency.currencyCode;
            }
        }

        // gets the product line items
        function getProductLineItems() {
            return _.filter(this.items, function (item) { return item.lineItemTypeField.alias === 'Product'; });
        }

        // gets the tax line items
        function getTaxLineItem() {
            return _.find(this.items, function (item) { return item.lineItemTypeField.alias === 'Tax'; });
        }

        // gets the shipping line items
        function getShippingLineItems() {
            return _.find(this.items, function (item) {
                return item.lineItemTypeField.alias === 'Shipping';
            });
        }

        // gets the custom line items
        function getCustomLineItems() {
            var custom =  _.filter(this.items, function(item) {
                return item.lineItemType === 'Custom';
            });
            if (custom === undefined) {
                custom = [];
            }
            return custom;
        }

        // gets a collection of discount line items
        function getDiscountLineItems() {
            var discounts = _.filter(this.items, function(item) {
                return item.lineItemTypeField.alias === 'Discount';
            });
            if (discounts === undefined) {
                discounts = [];
            }
            return discounts;
        }

        function shippingTotal() {
            var shippingLineItems = getShippingLineItems.call(this);
            var total = 0;
            if (shippingLineItems) {
                if (shippingLineItems.length) {
                    angular.forEach(shippingLineItems, function(lineItem) {
                      total += lineItem.price;
                    });
                } else {
                    total += shippingLineItems.price;
                }
            }
            return total;
        }

        // gets a value indicating whether or not this invoice has an order
        function hasOrder() {
            return this.orders.length > 0;
        }

        // gets a value indicating whether or not this invoice has been paid
        function isPaid() {
            // http://issues.merchello.com/youtrack/issue/M-659
            var status = this.invoiceStatus.alias;
            return status === 'paid';
        }

        
        // calculates the unpaid balance of the invoice
        function remainingBalance(payments) {
            var amountPaid = 0;
            angular.forEach(payments, function(payment) {
                angular.forEach(payment.appliedPayments, function(applied) {
                    amountPaid += applied.amount;
                });
            });
            return this.total - amountPaid;
        }

        function isAnonymous() {
            return this.customerKey === '00000000-0000-0000-0000-000000000000';
        }

        return {
            getPaymentStatus: getPaymentStatus,
            getFulfillmentStatus: getFulfillmentStatus,
            getCurrencyCode: getCurrencyCode,
            getProductLineItems: getProductLineItems,
            getDiscountLineItems: getDiscountLineItems,
            getTaxLineItem: getTaxLineItem,
            getShippingLineItems: getShippingLineItems,
            getCustomLineItems: getCustomLineItems,
            hasOrder: hasOrder,
            isPaid: isPaid,
            getBillToAddress: getBillingAddress,
            setBillingAddress: setBillingAddress,
            remainingBalance: remainingBalance,
            invoiceDateString: invoiceDateString,
            shippingTotal: shippingTotal,
            isAnonymous:  isAnonymous
        };
    }());

    angular.module('merchello.models').constant('InvoiceDisplay', InvoiceDisplay);
    /**
     * @ngdoc model
     * @name InvoiceStatusDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's InvoiceStatusDisplay object
     */
    var InvoiceStatusDisplay = function() {
        var self = this;

        self.key = '';
        self.name = '';
        self.alias = '';
        self.reportable = '';
        self.active = '';
        self.sortOrder = '';
    };

    angular.module('merchello.models').constant('InvoiceStatusDisplay', InvoiceStatusDisplay);
/**
    * @ngdoc model
    * @name NoteDisplay
    * @function
    *
    * @description
    * Represents a JS version of Merchello's NoteDisplay object
    */
var NoteDisplay = function () {
    var self = this;

    self.entityKey = '';
    self.entityTfKey = '';
    self.entityType = '';
    self.key = '';
    self.message = {};
    self.recordDate = '';
};

NoteDisplay.prototype = (function () {

    function toDateString() {
        return this.recordDate.split('T')[0];
    }

    function toTimeString() {
        var time = this.recordDate.split('T')[1];
        return time.split(':')[0] + ':' + time.split(':')[1];
    }

    return {
        toDateString: toDateString,
        toTimeString: toTimeString
    };

}());

angular.module('merchello.models').constant('NoteDisplay', NoteDisplay);


    /**
     * @ngdoc model
     * @name OrderDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's OrderDisplay object
     */
    var OrderDisplay = function() {
        var self = this;
        self.key = '';
        self.versionKey = '';
        self.invoiceKey = '';
        self.orderNumberPrefix = '';
        self.orderNumber = '';
        self.orderDate = '';
        self.orderStatusKey = ''; // to do this is not needed since we have the OrderStatusDisplay
        self.orderStatus = {};
        self.exported = false;
        self.items = [];
    };

    OrderDisplay.prototype = (function() {

        function getUnShippedItems() {
            return _.filter(this.items, function(item) {
                return (item.shipmentKey === '' || item.shipmentKey === null) && item.extendedData.getValue('merchShippable').toLowerCase() === 'true';
            });
        }

        // public
        return {
            getUnShippedItems: getUnShippedItems
        };

    }());

    angular.module('merchello.models').constant('OrderDisplay', OrderDisplay);
    /**
     * @ngdoc model
     * @name OrderStatusDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's OrderStatusDisplay object
     */
    var OrderStatusDisplay = function() {
        var self = this;
        self.key = '';
        self.name = '';
        self.alias = '';
        self.reportable = '';
        self.active = true;
        self.sortOrder = '';
    };

    angular.module('merchello.models').constant('OrderStatusDisplay', OrderStatusDisplay);
    /**
     * @ngdoc model
     * @name AuditLogDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's AuditLogDisplay object
     */
    var AuditLogDisplay = function() {
        var self = this;

        self.entityKey = '';
        self.entityTfKey = '';
        self.entityType = '';
        self.extendedData = {};
        self.isError = false;
        self.key = '';
        self.message = {};
        self.recordDate = '';
        self.verbosity = '';
    };

    AuditLogDisplay.prototype = (function() {

        function toDateString() {
            return this.recordDate.split('T')[0];
        }

        function toTimeString() {
            var time = this.recordDate.split('T')[1];
            return time.split(':')[0] + ':' + time.split(':')[1];
        }

        return {
            toDateString: toDateString,
            toTimeString: toTimeString
        };

    }());

    angular.module('merchello.models').constant('AuditLogDisplay', AuditLogDisplay);
    /**
     * @ngdoc model
     * @name DailyAuditLogDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's DailyAuditLogDisplay object
     */
    var DailyAuditLogDisplay = function() {
        var self = this;
        this.day = '';
        this.logs = [];
    };

    DailyAuditLogDisplay.prototype = (function() {

        //// TODO this is not working as expected so we are keeping it internal
        function dayToDate() {
            //// 0: YYYY
            //// 1: MM
            //// 2: DD
            var dateParts = toDateString.call(this).split('-');
            var timeParts = toTimeString.call(this).split(':');
            return Date.parse(dateParts[0], dateParts[1] - 1, dateParts[2], timeParts[0], timeParts[1], 0, 0);
        }

        function toDateString() {
            return this.day.split('T')[0];
        }

        function toTimeString() {
            var time = this.day.split('T')[1];
            return time.split(':')[0] + ':' + time.split(':')[1];
        }

        return {
            toDateString: toDateString,
            toTimeString: toTimeString
        };

    }());

    angular.module('merchello.models').constant('DailyAuditLogDisplay', DailyAuditLogDisplay);
    /**
     * @ngdoc model
     * @name SalesHistoryDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's SalesHistoryDisplay object
     */
    var SalesHistoryDisplay = function() {
        var self = this;
        self.dailyLogs = [];
    };

    SalesHistoryDisplay.prototype = (function() {

        // utility method to push a daily log
        function addDailyLog(dailyLog) {
            this.dailyLogs.push(dailyLog);
        }

        return {
            addDailyLog: addDailyLog
        };

    }());

    angular.module('merchello.models').constant('SalesHistoryDisplay', SalesHistoryDisplay);
    /**
     * @ngdoc model
     * @name SalesHistoryMessageDisplay
     * @function
     *
     * @description
     * Represents a sales history message object
     */
    var SalesHistoryMessageDisplay = function() {
        var self = this;
        self.area = '';
        self.key = '';
        self.formattedMessage = '';
    };

    SalesHistoryMessageDisplay.prototype = (function() {

        // constructs a localization key
        function localizationKey() {
            return this.area + '_' + this.key;
        }

        // any extra properties on this object are assumed to be tokens used in the localized
        // message
        function localizationTokens() {
            var allKeys = Object.keys(this);
            var tokens = [];
            for(var i = 0; i < allKeys.length; i++) {
                if (allKeys[i] !== 'area' && allKeys[i] !== 'key')
                {
                    tokens.push(this[allKeys[i]]);
                }
            }
            return tokens;
        }

        return {
            localizationKey: localizationKey,
            localizationTokens: localizationTokens
        };
    }());

    angular.module('merchello.models').constant('SalesHistoryMessageDisplay', SalesHistoryMessageDisplay);
    /**
     * @ngdoc model
     * @name ShipCountryDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's ShipCountryDisplay object
     */
    var ShipCountryDisplay = function() {
        var self = this;
        self.key = '';
        self.catalogKey = '';
        self.countryCode = '';
        self.name = '';
        self.provinceLabel = '';
        self.provinces = [];
        self.hasProvinces = false;
    };

    angular.module('merchello.models').constant('ShipCountryDisplay', ShipCountryDisplay);
    /**
     * @ngdoc model
     * @name ShipMethodDisplay
     *
     * @description
     * Represents a JS version of Merchello's ShipMethodDisplay object
     */
    var ShipMethodDisplay = function() {
        var self = this;
        self.key = '';
        self.name = '';
        self.providerKey = '';
        self.shipCountryKey = '';
        self.surchare = 0.0;
        self.serviceCode = '';
        self.taxable = false;
        self.provinces = [];
    };

    angular.module('merchello.models').constant('ShipMethodDisplay', ShipMethodDisplay);
    /**
     * @ngdoc model
     * @name ShipMethodsQueryDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's ShipMethodsQueryDisplay object
     */
    var ShipMethodsQueryDisplay = function() {
        var self = this;
        self.selected = {};
        self.alternatives = [];
    };

    angular.module('merchello.models').constant('ShipMethodsQueryDisplay', ShipMethodsQueryDisplay);
    /**
     * @ngdoc model
     * @name ShipMethodDisplay
     *
     * @description
     * Represents a JS version of Merchello's ShipProvinceDisplay object
     */
    var ShipProvinceDisplay = function() {
        var self = this;
        self.allowShipping = false;
        // TODO this should be converted to a string in the API for consistency
        self.rateAdjustment = 1;  // possible values are 1 & 2
    };

    angular.module('merchello.models').constant('ShipProvinceDisplay', ShipProvinceDisplay);
    /**
    * @ngdoc model
    * @name ShipmentDisplay
    * @function
    * 
    * @description
    * Represents a JS version of Merchello's ShipmentDisplay object
    */
    var ShipmentDisplay = function () {

        var self = this;

        self.key = '';
        self.shipmentNumber = '';
        self.shipmentNumberPrefix = '';
        self.versionKey = '';
        self.fromOrganization = '';
        self.fromName = '';
        self.fromAddress1 = '';
        self.fromAddress2 = '';
        self.fromLocality = '';
        self.fromRegion = '';
        self.fromPostalCode = '';
        self.fromCountryCode = '';
        self.fromIsCommercial = '';
        self.toOrganization = '';
        self.toName = '';
        self.toAddress1 = '';
        self.toAddress2 = '';
        self.toLocality = '';
        self.toRegion = '';
        self.toPostalCode = '';
        self.toCountryCode = '';
        self.toIsCommercial = '';
        self.shipMethodKey = '';
        self.phone = '';
        self.email = '';
        self.carrier = '';
        self.trackingCode = '';
        self.shippedDate = '';
        self.items = [];
        self.shipmentStatus = {};
    };

    // Shipment Prototype
    // ------------------------------------------------
    ShipmentDisplay.prototype = (function () {

        //// Private members
            // returns the shipment destination as an Address
        var getDestinationAddress = function() {
                return buildAddress.call(this, this.toName, this.toAddress1, this.toAddress2, this.toLocality, this.toRegion,
                    this.toPostalCode, this.toCountryCode, this.toOrganization, this.toIsCommercial, this.phone, this.email, 'shipping');
            },

            // returns the shipment origin as an Address
            getOriginAddress = function() {
                return buildAddress.call(this, this.fromName, this.fromAddress1, this.fromAddress2, this.fromLocality,
                    this.fromRegion, this.fromPostalCode, this.fromCountryCode, this.fromOrganization,
                    this.fromIsCommercial, '', '', 'shipping');
            },

            setDestinationAddress = function(address)
            {
                this.toName = address.name;
                this.toAddress1 = address.address1;
                this.toAddress2 = address.address2;
                this.toLocality = address.locality;
                this.toRegion = address.region;
                this.toPostalCode = address.postalCode;
                this.toCountryCode = address.countryCode;
                this.toOrganization = address.organization;
                this.toIsCommercial = address.isCommercial;
                this.phone = address.phone;
                this.email = address.email;
            },

            setOriginAddress = function(address) {
                this.fromName = address.name;
                this.fromAddress1 = address.address1;
                this.fromAddress2 = address.address2;
                this.fromLocality = address.locality;
                this.fromRegion = address.region;
                this.fromPostalCode = address.postalCode;
                this.fromCountryCode = address.countryCode;
                this.fromOrganization = address.organization;
                this.fromIsCommercial = address.isCommercial;
            },

            // Utility to build an address
            buildAddress = function(name, address1, address2, locality, region, postalCode, countryCode, organization,
                                    isCommercial, phone, email, addressType) {
                var adr = new AddressDisplay();
                adr.name = name;
                adr.address1 = address1;
                adr.address2 = address2;
                adr.locality = locality;
                adr.region = region;
                adr.postalCode = postalCode;
                adr.countryCode = countryCode;
                adr.organization = organization;
                adr.isCommercial = isCommercial;
                adr.phone = phone;
                adr.email = email;
                adr.addressType = addressType;
                return adr;
            };

        // public members
        return {
            /**
            * @ngdoc method
            * @name merchello.models.Shipment.getDestinationAddress
            * @function
            *
            * @description
            * Returns a merchello.models.Address representing the shipment destination
            */
            getDestinationAddress: getDestinationAddress,

            /**
            * @ngdoc method
            * @name merchello.models.Shipment.getOriginAddress
            * @function
            *
            * @description
            * Returns a merchello.models.Address representing the shipment origin
            */
            getOriginAddress: getOriginAddress,

            /**
             * @ngdoc method
             * @name merchello.models.Shipment.setDestinationAddress
             * @function
             *
             * @description
             * Sets the destination address for a shipment
             */
            setDestinationAddress: setDestinationAddress,

            /**
             * @ngdoc method
             * @name merchello.models.Shipment.setOriginAddress
             * @function
             *
             * @description
             * Sets the origin address for a shipment
             */
            setOriginAddress: setOriginAddress
        };
    }());

    angular.module('merchello.models').constant('ShipmentDisplay', ShipmentDisplay);

    /**
     * @ngdoc model
     * @name ShipmentRequestDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's ShipmentRequestDisplay object
     */
    var ShipmentRequestDisplay = function() {
        var self = this;
        self.shipmentStatusKey = '';
        self.order = {};
        self.shipMethodKey = '';
        self.trackingNumber = '';
    };

    angular.module('merchello.models').constant('ShipmentRequestDisplay', ShipmentRequestDisplay);

    /**
    * @ngdoc model
    * @name merchello.models.shipmentStatus
    * @function
    * 
    * @description
    * Represents a JS version of Merchello's ShipmentStatusDisplay object
    */
    var ShipmentStatusDisplay = function () {
        var self = this;
        self.key = '';
        self.name = '';
        self.alias = '';
        self.reportable = '';
        self.active = '';
        self.sortOrder = '';
    };

    angular.module('merchello.models').constant('ShipmentStatusDisplay', ShipmentStatusDisplay);
    /**
     * @ngdoc model
     * @name ShipMethodDisplay
     *
     * @description
     * Represents a JS version of Merchello's ShippingGatewayMethodDisplay object
     */
    var ShippingGatewayMethodDisplay = function() {
        var self = this;
        self.gatewayResource = {};
        self.shipMethod = {};
        self.shipCountry = {};
        self.dialogEditorView = {};
    };

    ShippingGatewayMethodDisplay.prototype = (function() {

        function getName() {
            return this.shipMethod.name;
        }

        function getKey() {
            return this.shipMethod.key;
        }

        return {
            getName: getName,
            getKey: getKey
        };
    }());

    angular.module('merchello.models').constant('ShippingGatewayMethodDisplay', ShippingGatewayMethodDisplay);

    /**
     * @ngdoc model
     * @name ShippingGatewayProviderDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's ShipmentDisplay object
     */
    var ShippingGatewayProviderDisplay = function() {
        var self = this;
        self.key = '';
        self.name = '';
        self.extendedData = {};
        self.shipMethods = [];
        self.availableResources = [];
    };

    ShippingGatewayProviderDisplay.prototype = (function() {

        function addMethod(shippingMethod) {
            this.shipMethods.push(shippingMethod);
        }

        function removeMethod(shippingMethod) {
            this.shipMethods = _.reject(this.shipMethods, function(m) {
              return m.key === shipmethod.key;
            });
        }

        return {
            addMethod: addMethod,
            removeMethod: removeMethod
        };

    }());

    angular.module('merchello.models').constant('ShippingGatewayProviderDisplay', ShippingGatewayProviderDisplay);
    /**
     * @ngdoc model
     * @name ShipFixedRateTableDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's ShipFixedRateTableDisplay object
     */
    var ShipFixedRateTableDisplay = function() {
        var self = this;
        self.shipMethodKey = '';
        self.shipCountryKey = '';
        self.rows = [];
    };

    ShipFixedRateTableDisplay.prototype = (function() {

        // pushes a new row into the rate table rows collection
        function addRow(row) {
            this.rows.push(row);
        }

        // removes an existing row from the rate table
        function removeRow(row) {
            this.rows = _.reject(this.rows, function(r) { return r.key === row.key; });
        }

        return {
            addRow: addRow,
            removeRow: removeRow
        };
    }());

    angular.module('merchello.models').constant('ShipFixedRateTableDisplay', ShipFixedRateTableDisplay);
    /**
     * @ngdoc model
     * @name ShipRateTierDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's ShipRateTierDisplay object
     */
    var ShipRateTierDisplay = function() {
        var self = this;
        self.key = '';
        self.shipMethodKey = '';
        self.rangeLow = 0.0;
        self.rangeHigh = 0.0;
        self.rate = 0.0;
    };

    angular.module('merchello.models').constant('ShipRateTierDisplay', ShipRateTierDisplay);
    /**
     * @ngdoc model
     * @name TaxCountryDisplay
     * @function
     *
     * @description
     * Represents a TaxCountryDisplay
     *
     * @note
     * There is not a corresponding Merchello model
     */
    var TaxCountryDisplay = function() {
        var self = this;
        self.name = '';
        self.country = {};
        self.provider = {};
        self.taxMethod = {};
        self.gatewayResource = {};
        self.addTaxesToProduct = false;
        self.sortOrder = 0;
    };

    TaxCountryDisplay.prototype = (function() {

        function setCountryName(str) {
            this.name = str;
        }

        return {
            setCountryName: setCountryName
        };

    }());

    angular.module('merchello.models').constant('TaxCountryDisplay', TaxCountryDisplay);
    /**
     * @ngdoc model
     * @name TaxMethodDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's TaxMethodDisplay object
     */
    var TaxMethodDisplay = function() {
        var self = this;
        self.key = '';
        self.providerKey = '';
        self.name = '';
        self.countryCode = '';
        self.percentageTaxRate = 0.0;
        self.provinces = [];
        self.productTaxMethod = false;
    };

    TaxMethodDisplay.prototype = (function() {

        function hasProvinces() {
            return this.provinces.length > 0;
        }

        return {
            hasProvinces: hasProvinces
        };
    }());

    angular.module('merchello.models').constant('TaxMethodDisplay', TaxMethodDisplay);

    /**
     * @ngdoc model
     * @name TaxProvinceDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's TaxProvinceDisplay object
     */
    var TaxProvinceDisplay = function() {
        var self = this;
        self.name = '';
        self.code = '';
        self.percentAdjustment = 0;
    };

    angular.module('merchello.models').constant('TaxProvinceDisplay', TaxProvinceDisplay);

var TaxationGatewayProviderDisplay = function () {
    var self = this;
    GatewayProviderDisplay.apply(self, arguments);
    self.taxationByProductProvider = false;
};

TaxationGatewayProviderDisplay.prototype = GatewayProviderDisplay.prototype;
TaxationGatewayProviderDisplay.prototype.constructor = TaxationGatewayProviderDisplay;

angular.module('merchello.models').constant('TaxationGatewayProviderDisplay', TaxationGatewayProviderDisplay);
    /**
     * @ngdoc model
     * @name WarehouseCatalogDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's WarehouseCatalogDisplay object
     */
    var WarehouseCatalogDisplay = function() {
        var self = this;
        self.key = '';
        self.warehouseKey = '';
        self.name = '';
        self.description = '';
        self.isDefault = true;
    };

    angular.module('merchello.models').constant('WarehouseCatalogDisplay', WarehouseCatalogDisplay);
    /**
     * @ngdoc model
     * @name WarehouseDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's WarehouseDisplay object
     */
    var WarehouseDisplay = function() {
        var self = this;
        self.key = '';
        self.name = '';
        self.address1 = '';
        self.address2 = '';
        self.locality = '';
        self.region = '';
        self.postalCode = '';
        self.countryCode = '';
        self.phone = '';
        self.email = '';
        self.isDefault = true;
        self.warehouseCatalogs = [];
    };

    WarehouseDisplay.prototype = (function() {

        function getAddress() {
            var adr = new AddressDisplay();
            adr.name = this.name;
            adr.address1 = this.address1;
            adr.address2 = this.address2;
            adr.locality = this.locality;
            adr.region = this.region;
            adr.postalCode = this.postalCode;
            adr.countryCode = this.countryCode;
            adr.phone = this.phone;
            adr.email = this.email;
            adr.addressType = 'shipping';
            return adr;
        }

        function setAddress(address) {
            this.name = address.name;
            this.address1 = address.address1;
            this.address2 = address.address2;
            this.locality = address.locality;
            this.region = address.region;
            this.postalCode = address.postalCode;
            this.countryCode = address.countryCode;
            this.phone = address.phone;
            this.email = address.email;
        }

        function findDefaultCatalog() {
            return _.find(this.warehouseCatalogs, function (catalog) { return catalog.isDefault; });
        }

        return {
            getAddress: getAddress,
            setAddress: setAddress,
            findDefaultCatalog: findDefaultCatalog
        };
    }());

    angular.module('merchello.models').constant('WarehouseDisplay', WarehouseDisplay);
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
        /**
         * @ngdoc method
         * @name isStringifyJson
         * @function
         *
         * @description
         * Checks the value to determine if it is a stringified Json value
         */
        function isStringifyJson(value) {
            return (/^[\],:{}\s]*$/.test(value.replace(/\\["\\\/bfnrtu]/g, '@').
                replace(/"[^"\\\n\r]*"|true|false|null|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?/g, ']').
                replace(/(?:^|:|,)(?:\s*\[)+/g, '')));
        }


        // public
        return {
            transform : transform,
            isStringifyJson: isStringifyJson
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
 * @name backOfficeTreeDisplayBuilder
 *
 * @description
 * A utility service that builds backOfficeTreeDisplay models
 */
angular.module('merchello.models').factory('backOfficeTreeDisplayBuilder',
    ['genericModelBuilder', 'BackOfficeTreeDisplay',
    function(genericModelBuilder, BackOfficeTreeDisplay) {
        var Constructor = BackOfficeTreeDisplay;

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
 * @name entityCollectionDisplayBuilder
 *
 * @description
 * A utility service that builds EntityCollectionDisplay models
 */
angular.module('merchello.models').factory('entityCollectionDisplayBuilder',
    ['genericModelBuilder', 'typeFieldDisplayBuilder', 'EntityCollectionDisplay',
        function(genericModelBuilder, typeFieldDisplayBuilder, EntityCollectionDisplay) {
            var Constructor = EntityCollectionDisplay;
            return {
                createDefault: function() {
                    return new Constructor();
                },
                transform: function(jsonResult) {
                    var collections = [];
                    if (angular.isArray(jsonResult)) {
                        for(var i = 0; i < jsonResult.length; i++) {
                            var collection = genericModelBuilder.transform(jsonResult[ i ], Constructor);
                            collection.entityTypeField = typeFieldDisplayBuilder.transform(jsonResult[ i ].entityTypeField );
                            collections.push(collection);
                        }
                    } else {
                        collections = genericModelBuilder.transform(jsonResult, Constructor);
                        collections.entityTypeField = typeFieldDisplayBuilder.transform(jsonResult.entityTypeField );
                    }
                    return collections;
                }
            };
}]);

/**
 * @ngdoc service
 * @name entityCollectionDisplayBuilder
 *
 * @description
 * A utility service that builds EntityCollectionDisplay models
 */
angular.module('merchello.models').factory('entityCollectionProviderDisplayBuilder',
    ['genericModelBuilder', 'entityCollectionDisplayBuilder',  'typeFieldDisplayBuilder', 'EntityCollectionProviderDisplay',
        function(genericModelBuilder, entityCollectionDisplayBuilder, typeFieldDisplayBuilder, EntityCollectionProviderDisplay) {
            var Constructor = EntityCollectionProviderDisplay;
            return {
                createDefault: function() {
                    return new Constructor();
                },
                transform: function(jsonResult) {
                    var providers = [];
                    if(angular.isArray(jsonResult)) {
                        for(var i = 0; i < jsonResult.length; i++) {
                            var provider = genericModelBuilder.transform(jsonResult[ i ], Constructor);
                            provider.managedCollections = entityCollectionDisplayBuilder.transform(jsonResult[ i ].managedCollections);
                            provider.entityTypeField = typeFieldDisplayBuilder.transform(jsonResult[i].entityTypeField);
                            providers.push(provider);
                        }
                    } else {
                        providers = genericModelBuilder.transform(jsonResult, Constructor);
                        providers.managedCollections = entityCollectionDisplayBuilder.transform(jsonResult.managedCollections);
                        providers.entityTypeField = typeFieldDisplayBuilder.transform(jsonResult.entityTypeField);
                    }
                    return providers;
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

    /**
     * @ngdoc service
     * @name customerDisplayBuilder
     *
     * @description
     * A utility service that builds CustomerDisplay models
     */
    angular.module('merchello.models').factory('customerDisplayBuilder',
        ['genericModelBuilder', 'customerAddressDisplayBuilder', 'extendedDataDisplayBuilder', 'invoiceDisplayBuilder', 'CustomerDisplay',
        function(genericModelBuilder, customerAddressDisplayBuilder, extendedDataDisplayBuilder,
                 invoiceDisplayBuilder, CustomerDisplay) {

            var Constructor = CustomerDisplay;
            return {
                createDefault: function() {
                    var customer = new Constructor();
                    customer.extendedData = extendedDataDisplayBuilder.createDefault();
                    return customer;
                },
                transform: function(jsonResult) {
                    var customers = [];
                    if(angular.isArray(jsonResult)) {
                        for(var i = 0; i < jsonResult.length; i++) {
                            var customer = genericModelBuilder.transform(jsonResult[ i ], Constructor);
                            customer.addresses = customerAddressDisplayBuilder.transform(jsonResult[ i ].addresses);
                            customer.invoices = invoiceDisplayBuilder.transform(jsonResult[ i ].invoices);
                            customer.extendedData = extendedDataDisplayBuilder.transform(jsonResult[ i ].extendedData);
                            customers.push(customer);
                        }
                    } else {
                        customers = genericModelBuilder.transform(jsonResult, Constructor);
                        customers.addresses = customerAddressDisplayBuilder.transform(jsonResult.addresses);
                        customers.invoices = invoiceDisplayBuilder.transform(jsonResult.invoices);
                        customers.extendedData = extendedDataDisplayBuilder.transform(jsonResult.extendedData);
                    }
                    return customers;
                }
            };

    }]);

/**
 * @ngdoc service
 * @name detachedContentTypeDisplayBuilder
 *
 * @description
 * A utility service that builds DetachedContentTypeDisplay models
 */
angular.module('merchello.models').factory('detachedContentTypeDisplayBuilder',
	['genericModelBuilder', 'typeFieldDisplayBuilder', 'umbContentTypeDisplayBuilder', 'DetachedContentTypeDisplay',
	function (genericModelBuilder, typeFieldDisplayBuilder, umbContentTypeDisplayBuilder, DetachedContentTypeDisplay) {

		var Constructor = DetachedContentTypeDisplay;

		return {
			createDefault: function () {
				var content = new Constructor();
				content.entityTypeField = typeFieldDisplayBuilder.createDefault();
				content.umbContentType = umbContentTypeDisplayBuilder.createDefault();
			    return content;
			},
			transform: function (jsonResult) {
				var contents = [];
				if (angular.isArray(jsonResult)) {
					for(var i = 0; i < jsonResult.length; i++) {
						var content = genericModelBuilder.transform(jsonResult[ i ], Constructor);
						content.entityTypeField = typeFieldDisplayBuilder.transform(jsonResult[ i ].entityTypeField);
						content.umbContentType = umbContentTypeDisplayBuilder.transform(jsonResult[ i ].umbContentType);
						contents.push(content);
					}
				} else {
				    contents = genericModelBuilder.transform(jsonResult, Constructor);
					contents.entityTypeField = typeFieldDisplayBuilder.transform(jsonResult.entityTypeField);
					contents.umbContentType = umbContentTypeDisplayBuilder.transform(jsonResult.umbContentType);
				}
				return contents;
			}
		};
	}
]);
/**
 * @ngdoc service
 * @name umbContentTypeDisplayBuilder
 *
 * @description
 * A utility service that builds UmbContentTypeDisplay models
 */
angular.module('merchello.models').factory('umbContentTypeDisplayBuilder',
    ['genericModelBuilder', 'UmbContentTypeDisplay',
    function(genericModelBuilder, UmbContentTypeDisplay) {

        var Constructor = UmbContentTypeDisplay;

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
 * @name merchello.models.dialogDataFactory
 *
 * @description
 * A utility service that builds dialogData models
 */
angular.module('merchello.models').factory('dialogDataFactory',
    [
    function() {

        // creates dialogData object for capturing a payment
        function createCapturePaymentDialogData() {
            return new CapturePaymentDialogData();
        }

        // creates dialogData for creating a shipment
        function createCreateShipmentDialogData() {
            return new CreateShipmentDialogData();
        }

        // creates dialogData for editing ShipmentDisplay
        function createEditShipmentDialogData() {
            return new EditShipmentDialogData();
        }

        // creates dialogData for editing AddressDisplay
        function createEditAddressDialogData() {
            return new EditAddressDialogData();
        }

        // creates dialogData for adding Ship Countries
        function createAddShipCountryDialogData() {
            return new AddShipCountryDialogData();
        }

        // creates dialogData for deleting ship countries
        function createDeleteShipCountryDialogData() {
            return new DeleteShipCountryDialogData();
        }

        // creates dialogData for adding providers to ship countries
        function createAddShipCountryProviderDialogData() {
            return new AddShipCountryProviderDialogData();
        }

        // creates a dialogData for deleting ship country ship methods
        function createDeleteShipCountryShipMethodDialogData() {
            return new DeleteShipCountryShipMethodDialogData();
        }

        // creates a dialogData for editing shipping gateway methods
        function createEditShippingGatewayMethodDialogData() {
            return new EditShippingGatewayMethodDialogData();
        }

        // creates a dialogData for adding or editing warehouses
        function createAddEditWarehouseDialogData() {
            return new AddEditWarehouseDialogData();
        }

        // creates a dialogData for adding or editing warehouse catalogs
        function createAddEditWarehouseCatalogDialogData() {
            return new AddEditWarehouseCatalogDialogData();
        }

        function createDeleteWarehouseCatalogDialogData() {
            return new DeleteWarehouseCatalogDialogData();
        }

        function createChangeWarehouseCatalogDialogData() {
            return new ChangeWarehouseCatalogDialogData();
        }

        function createEditTaxCountryDialogData() {
            return new EditTaxCountryDialogData();
        }

        function createDeletePaymentMethodDialogData() {
            return new DeletePaymentMethodDialogData();
        }

        // creates a dialog data model for adding or editing a notification method
        function createAddEditNotificationMethodDialogData() {
            return new AddEditNotificationMethodDialogData();
        }

        // creates a dialog data model for deleting a notification method
        function createDeleteNotificationMethodDialogData() {
            return new DeleteNotificationMethodDialogData();
        }

        // creates a dialog data model for adding and editing a notification message
        function createAddEditNotificationMessageDialogData() {
            return new AddEditNotificationMessageDialogData();
        }

        // creates a dialog data model for deleting a notification message
        function createDeleteNotificationMessageDialogData() {
            return new DeleteNotificationMessageDialogData();
        }

        // creates a dialog data model for adding or updating a customer
        function createAddEditCustomerDialogData() {
            return new AddEditCustomerDialogData();
        }

        // creates a dialog data model for deleting a customer
        function createDeleteCustomerDialogData() {
            return new DeleteCustomerDialogData();
        }

        // creates a dialog data model for adding or updating customer addresses
        function createAddEditCustomerAddressDialogData() {
            return new AddEditCustomerAddressDialogData();
        }

        // creates a dialog data model for deleting a customer address
        function createDeleteCustomerAddressDialogData() {
            return new DeleteCustomerAddressDialogData();
        }

        // creates a dialog data model for deleting a product dialog
        function createDeleteProductDialogData() {
            return new DeleteProductDialogData();
        }

        // Product Bulk Actions

        // creates a dialog data model for bulk action update product variant pricing
        function createBulkVariantChangePricesDialogData() {
            return new BulkVariantChangePricesDialogData();
        }

        // creates a dialog data module for bulk action update of product inventories
        function createBulkEditInventoryCountsDialogData() {
            return new BulkEditInventoryCountsDialogData();
        }

        // creates a dialog data for voiding payments
        function createProcessVoidPaymentDialogData() {
            return new ProcessVoidPaymentDialogData();
        }

        // creates a dialog data for refunding payments
        function createProcessRefundPaymentDialogData() {
            return new ProcessRefundPaymentDialogData();
        }

        // creates a dialog data for adding new payments
        function createAddPaymentDialogData() {
            return new AddPaymentDialogData();
        }

        // Marketing
        function createSelectOfferProviderDialogData() {
            return new SelectOfferProviderDialogData();
        }

        // offer components
        function createConfigureOfferComponentDialogData() {
            return new ConfigureOfferComponentDialogData();
        }

        function createAddEditEntityStaticCollectionDialog() {
            return new AddEditEntityStaticCollectionDialog();
        }

        // detached content
        function createEditDetachedContentTypeDialogData() {
            return new EditDetachedContentTypeDialogData();
        }

        /*----------------------------------------------------------------------------------------
        Property Editors
        -------------------------------------------------------------------------------------------*/

        function createProductSelectorDialogData() {
            return new ProductSelectorDialogData();
        }

        return {
            createAddShipCountryDialogData: createAddShipCountryDialogData,
            createDeleteShipCountryDialogData: createDeleteShipCountryDialogData,
            createAddShipCountryProviderDialogData: createAddShipCountryProviderDialogData,
            createChangeWarehouseCatalogDialogData: createChangeWarehouseCatalogDialogData,
            createDeleteWarehouseCatalogDialogData: createDeleteWarehouseCatalogDialogData,
            createEditShippingGatewayMethodDialogData: createEditShippingGatewayMethodDialogData,
            createAddEditWarehouseCatalogDialogData: createAddEditWarehouseCatalogDialogData,
            createCapturePaymentDialogData: createCapturePaymentDialogData,
            createCreateShipmentDialogData: createCreateShipmentDialogData,
            createEditShipmentDialogData: createEditShipmentDialogData,
            createEditAddressDialogData: createEditAddressDialogData,
            createAddEditWarehouseDialogData: createAddEditWarehouseDialogData,
            createDeleteShipCountryShipMethodDialogData: createDeleteShipCountryShipMethodDialogData,
            createEditTaxCountryDialogData: createEditTaxCountryDialogData,
            createDeletePaymentMethodDialogData: createDeletePaymentMethodDialogData,
            createAddEditNotificationMethodDialogData: createAddEditNotificationMethodDialogData,
            createDeleteNotificationMethodDialogData: createDeleteNotificationMethodDialogData,
            createAddEditNotificationMessageDialogData: createAddEditNotificationMessageDialogData,
            createDeleteNotificationMessageDialogData: createDeleteNotificationMessageDialogData,
            createAddEditCustomerDialogData: createAddEditCustomerDialogData,
            createDeleteCustomerDialogData: createDeleteCustomerDialogData,
            createAddEditCustomerAddressDialogData: createAddEditCustomerAddressDialogData,
            createDeleteCustomerAddressDialogData: createDeleteCustomerAddressDialogData,
            createDeleteProductDialogData: createDeleteProductDialogData,
            createBulkVariantChangePricesDialogData: createBulkVariantChangePricesDialogData,
            createBulkEditInventoryCountsDialogData: createBulkEditInventoryCountsDialogData,
            createProductSelectorDialogData: createProductSelectorDialogData,
            createProcessVoidPaymentDialogData: createProcessVoidPaymentDialogData,
            createProcessRefundPaymentDialogData: createProcessRefundPaymentDialogData,
            createAddPaymentDialogData: createAddPaymentDialogData,
            createSelectOfferProviderDialogData: createSelectOfferProviderDialogData,
            createConfigureOfferComponentDialogData: createConfigureOfferComponentDialogData,
            createAddEditEntityStaticCollectionDialog: createAddEditEntityStaticCollectionDialog,
            createEditDetachedContentTypeDialogData: createEditDetachedContentTypeDialogData
        };
}]);

    /**
     * @ngdoc service
     * @name merchello.models.dialogEditorViewDisplayBuilder
     *
     * @description
     * A utility service that builds dialogEditorViewDisplay models
     */
    angular.module('merchello.models').factory('dialogEditorViewDisplayBuilder',
        ['genericModelBuilder', 'DialogEditorViewDisplay',
            function(genericModelBuilder, DialogEditorViewDisplay) {

                var Constructor = DialogEditorViewDisplay;

                return {
                    createDefault: function () {
                        return new Constructor();
                    },
                    transform: function (jsonResult) {
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
        ['genericModelBuilder', 'ExtendedDataDisplay', 'ExtendedDataItemDisplay',
            function(genericModelBuilder, ExtendedDataDisplay, ExtendedDataItemDisplay) {

                var Constructor = ExtendedDataDisplay;


                return {
                    createDefault: function() {
                        return new Constructor();
                    },
                    transform: function(jsonResult) {
                        var extendedData = new Constructor();
                        if (jsonResult !== undefined) {
                            var items = genericModelBuilder.transform(jsonResult, ExtendedDataItemDisplay);
                            if(items.length > 0) {
                                extendedData.items = items;
                            }
                        }
                        return extendedData;
                    }
                };
            }]);

    /**
     * @ngdoc service
     * @name merchello.models.gatewayProviderDisplayBuilder
     *
     * @description
     * A utility service that builds GatewayProviderDisplay models
     */
    angular.module('merchello.models').factory('gatewayProviderDisplayBuilder',
        ['genericModelBuilder', 'extendedDataDisplayBuilder', 'dialogEditorViewDisplayBuilder', 'GatewayProviderDisplay',
        function(genericModelBuilder, extendedDataDisplayBuilder, dialogEditorViewDisplayBuilder, GatewayProviderDisplay) {

            var Constructor = GatewayProviderDisplay;

            return {
                createDefault: function () {
                    var gatewayProvider = new Constructor();
                    gatewayProvider.extendedData = extendedDataDisplayBuilder.createDefault();
                    gatewayProvider.dialogEditorView = dialogEditorViewDisplayBuilder.createDefault();
                    return gatewayProvider;
                },
                transform: function (jsonResult) {
                    var gatewayProviders = genericModelBuilder.transform(jsonResult, Constructor);
                    if (angular.isArray(gatewayProviders)) {
                        for (var i = 0; i < gatewayProviders.length; i++) {
                            gatewayProviders[i].extendedData = extendedDataDisplayBuilder.transform(jsonResult[i].extendedData);
                            gatewayProviders[i].dialogEditorView = dialogEditorViewDisplayBuilder.transform(jsonResult[i].dialogEditorView);
                        }
                    } else {
                        gatewayProviders.extendedData = extendedDataDisplayBuilder.transform(jsonResult.extendedData);
                        gatewayProviders.dialogEditorView = dialogEditorViewDisplayBuilder.transform(jsonResult.dialogEditorView);
                    }
                    return gatewayProviders;
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
     * @name merchello.models.offerComponentDefinitionDisplayBuilder
     *
     * @description
     * A utility service that builds OfferComponentDefinitionDisplay models
     */
    angular.module('merchello.models').factory('offerComponentDefinitionDisplayBuilder',
        ['genericModelBuilder', 'extendedDataDisplayBuilder', 'dialogEditorViewDisplayBuilder', 'OfferComponentDefinitionDisplay',
        function(genericModelBuilder, extendedDataDisplayBuilder, dialogEditorViewDisplayBuilder, OfferComponentDefinitionDisplay) {

            var Constructor = OfferComponentDefinitionDisplay;

            return {
                createDefault: function() {
                    var definition = new Constructor();
                    definition.extendedData = extendedDataDisplayBuilder.createDefault();
                    return definition;
                },
                transform: function(jsonResult) {
                    var definitions = [];
                    if(angular.isArray(jsonResult)) {
                        for(var i = 0; i < jsonResult.length; i++) {
                            var definition = genericModelBuilder.transform(jsonResult[ i ], Constructor);
                            definition.extendedData = extendedDataDisplayBuilder.transform(jsonResult[ i ].extendedData);
                            definition.dialogEditorView = dialogEditorViewDisplayBuilder.transform(jsonResult[ i ].dialogEditorView);
                            definitions.push(definition);
                        }
                    } else {
                        definitions = genericModelBuilder.transform(jsonResult[ i ], Constructor);
                        definitions.extendedData = extendedDataDisplayBuilder.transform(jsonResult[ i ].extendedData);
                        definitions.dialogEditorView = dialogEditorViewDisplayBuilder.transform(jsonResult[ i ].dialogEditorView);
                    }
                    return definitions;
                }
            };
        }]);
/**
 * @ngdoc service
 * @name offerProviderDisplayBuilder
 *
 * @description
 * A utility service that builds OfferProviderDisplay models
 */
angular.module('merchello.models').factory('offerProviderDisplayBuilder', [
    'genericModelBuilder', 'backOfficeTreeDisplayBuilder', 'OfferProviderDisplay',
    function(genericModelBuilder, backOfficeTreeDisplayBuilder, OfferProviderDisplay) {

        var Constructor = OfferProviderDisplay;

        return {
            createDefault: function () {
                var provider = new Constructor();
                provider.backOfficeTree = backOfficeTreeDisplayBuilder.createDefault();
                return provider;
            },
            transform: function (jsonResult) {
                var providers = genericModelBuilder.transform(jsonResult, Constructor);
                if (angular.isArray(providers)) {
                    for (var i = 0; i < providers.length; i++) {
                        providers[i].backOfficeTree = backOfficeTreeDisplayBuilder.transform(jsonResult[i].backOfficeTree);
                    }
                } else {
                    providers.backOfficeTree = backOfficeTreeDisplayBuilder.transform(jsonResult.backOfficeTree);
                }
                return providers;
            }
        };

    }]);
/**
 * @ngdoc service
 * @name merchello.models.offerSettingsDisplayBuilder
 *
 * @description
 * A utility service that builds OfferSettingsDisplay models
 */
angular.module('merchello.models').factory('offerSettingsDisplayBuilder',
    ['genericModelBuilder', 'offerComponentDefinitionDisplayBuilder', 'OfferSettingsDisplay',
    function(genericModelBuilder, offerComponentDefinitionDisplayBuilder, OfferSettingsDisplay) {
        var Constructor = OfferSettingsDisplay;

        function formatDateString(val) {
            var raw = new Date(val.split('T')[0]);
            return new Date(raw.getTime() + raw.getTimezoneOffset()*60000);
        }

        return {
            createDefault: function() {
                return new Constructor();
            },
            transform: function(jsonResult) {
                var settings = [];
                if(angular.isArray(jsonResult)) {
                    angular.forEach(jsonResult, function(json) {
                        var setting = genericModelBuilder.transform(json, Constructor);
                        setting.offerStartsDate = formatDateString(setting.offerStartsDate);
                        setting.offerEndsDate = formatDateString(setting.offerEndsDate);
                        setting.componentDefinitions = offerComponentDefinitionDisplayBuilder.transform(json.componentDefinitions);
                        settings.push(setting);
                    });
                } else {
                    settings = genericModelBuilder.transform(jsonResult,Constructor);
                    settings.offerStartsDate = formatDateString(settings.offerStartsDate);
                    settings.offerEndsDate = formatDateString(settings.offerEndsDate);
                    settings.componentDefinitions = offerComponentDefinitionDisplayBuilder.transform(jsonResult.componentDefinitions);
                }
                return settings;
            }
        };
    }]);
angular.module('merchello.models').factory('merchelloTabsFactory',
    ['MerchelloTabCollection',
        function(MerchelloTabCollection) {

            var Constructor = MerchelloTabCollection;

            // creates tabs for the product listing page
            function createProductListTabs() {
                var tabs = new Constructor();
                tabs.addTab('productlist', 'merchelloTabs_productListing', '#/merchello/merchello/productlist/manage');
                tabs.addTab('productContentTypeList', 'merchelloTabs_productContentTypes', '#/merchello/merchello/productcontenttypelist/manage')
                return tabs;
            }

           // creates tabs for the product editor page
            function createNewProductEditorTabs() {
                var tabs = new Constructor();
                tabs.addTab('productlist', 'merchelloTabs_productListing', '#/merchello/merchello/productlist/manage');
                tabs.addTab('createproduct', 'merchelloTabs_product', '#/merchello/merchello/productedit/');
                return tabs;
            }

            // creates tabs for the product editor page
            function createProductEditorTabs(productKey, hasVariants) {
                if (hasVariants !== undefined && hasVariants == true)
                {
                    return createProductEditorWithOptionsTabs(productKey);
                }
                var tabs = new Constructor();
                tabs.addTab('productlist', 'merchelloTabs_productListing', '#/merchello/merchello/productlist/manage');
                tabs.addTab('productedit', 'merchelloTabs_product', '#/merchello/merchello/productedit/' + productKey);
                tabs.addTab('productcontent', 'merchelloTabs_detachedContent', '#/merchello/merchello/productdetachedcontent/' + productKey);
                return tabs;
            }

            // creates tabs for the product editor with options tabs
            function createProductEditorWithOptionsTabs(productKey) {
                var tabs = new Constructor();
                tabs.addTab('productlist', 'merchelloTabs_productListing', '#/merchello/merchello/productlist/manage');
                tabs.addTab('productedit', 'merchelloTabs_product', '#/merchello/merchello/productedit/' + productKey);
                tabs.addTab('productcontent', 'merchelloTabs_detachedContent', '#/merchello/merchello/productdetachedcontent/' + productKey);
                tabs.addTab('variantlist', 'merchelloTabs_productVariants', '#/merchello/merchello/producteditwithoptions/' + productKey);
                tabs.addTab('optionslist', 'merchelloTabs_productOptions', '#/merchello/merchello/productoptionseditor/' + productKey);
                return tabs;
            }

            // creates tabs for the product variant editor
           function createProductVariantEditorTabs(productKey, productVariantKey) {
                var tabs = new Constructor();
                tabs.addTab('productlist', 'merchelloTabs_productListing', '#/merchello/merchello/productlist/manage');
                tabs.addTab('productedit', 'merchelloTabs_product', '#/merchello/merchello/productedit/' + productKey);
                tabs.addTab('variantlist', 'merchelloTabs_productVariants', '#/merchello/merchello/producteditwithoptions/' + productKey);
                tabs.addTab('varianteditor', 'merchelloTabs_productVariantEditor', '#/merchello/merchello/productedit/' + productKey + '?variantid=' + productVariantKey);
               tabs.addTab('productcontent', 'merchelloTabs_detachedContent', '#/merchello/merchello/productdetachedcontent/' + productKey + '?variantid=' + productVariantKey);
                return tabs;
            }


            // creates tabs for the sales listing page
            function createSalesListTabs() {
                var tabs = new Constructor();
                tabs.addTab('saleslist', 'merchelloTabs_salesListing', '#/merchello/merchello/saleslist/manage');
                return tabs;
            }

            // creates the tabs for sales overview section
            function createSalesTabs(invoiceKey) {
                var tabs = new Constructor();
                tabs.addTab('saleslist', 'merchelloTabs_salesListing', '#/merchello/merchello/saleslist/manage');
                tabs.addTab('overview', 'merchelloTabs_sales', '#/merchello/merchello/saleoverview/' + invoiceKey);
                tabs.addTab('payments', 'merchelloTabs_payments', '#/merchello/merchello/invoicepayments/' + invoiceKey);
                tabs.addTab('shipments', 'merchelloTabs_shipments', '#/merchello/merchello/ordershipments/' + invoiceKey);
                return tabs;
            }

            // creates the tabs for the customer list page
            function createCustomerListTabs() {
                var tabs = new Constructor();
                tabs.addTab('customerlist', 'merchelloTabs_customerListing', '#/merchello/merchello/customerlist/manage');
                return tabs;
            }

            // creates the customer overview tabs
            function createCustomerOverviewTabs(customerKey, hasAddresses) {
                var tabs = new Constructor();
                tabs.addTab('customerlist', 'merchelloTabs_customerListing', '#/merchello/merchello/customerlist/manage');
                tabs.addTab('overview', 'merchelloTabs_customer', '#/merchello/merchello/customeroverview/' + customerKey);
                if(hasAddresses) {
                    tabs.addTab('addresses', 'merchelloTabs_customerAddresses', '#/merchello/merchello/customeraddresses/' + customerKey);
                }
                return tabs;
            }

            // creates the tabs for the gateway provider section
            function createGatewayProviderTabs() {
                var tabs = new Constructor();
                tabs.addTab('providers', 'merchelloTabs_gatewayProviders', '#/merchello/merchello/gatewayproviderlist/manage');
                tabs.addTab('notification', 'merchelloTabs_notification', '#/merchello/merchello/notificationproviders/manage');
                tabs.addTab('payment', 'merchelloTabs_payment', '#/merchello/merchello/paymentproviders/manage');
                tabs.addTab('shipping', 'merchelloTabs_shipping', '#/merchello/merchello/shippingproviders/manage');
                tabs.addTab('taxation', 'merchelloTabs_taxation', '#/merchello/merchello/taxationproviders/manage');
                return tabs;
            }

            // creates the tabs for the marketing section
            function createMarketingTabs() {
                var tabs = new Constructor();
                tabs.addTab('offers', 'merchelloTabs_offerListing', '#/merchello/merchello/offerslist/manage');
                return tabs;
            }

            function createReportsTabs() {
                var tabs = new Constructor();
                tabs.addTab('reportslist', 'merchelloTabs_reports', '#/merchello/merchello/reportslist/manage');
                return tabs;
            }




            return {
                createNewProductEditorTabs: createNewProductEditorTabs,
                createProductListTabs: createProductListTabs,
                createProductEditorTabs: createProductEditorTabs,
                createProductEditorWithOptionsTabs: createProductEditorWithOptionsTabs,
                createSalesListTabs: createSalesListTabs,
                createSalesTabs: createSalesTabs,
                createCustomerListTabs: createCustomerListTabs,
                createCustomerOverviewTabs: createCustomerOverviewTabs,
                createGatewayProviderTabs: createGatewayProviderTabs,
                createReportsTabs: createReportsTabs,
                createProductVariantEditorTabs: createProductVariantEditorTabs,
                createMarketingTabs: createMarketingTabs
            };

}]);

/**
 * @ngdoc service
 * @name notificationGatewayProviderDisplayBuilder
 *
 * @description
 * A utility service that builds NotificationGatewayProviderDisplay models
 */
angular.module('merchello.models').factory('notificationGatewayProviderDisplayBuilder',
    ['genericModelBuilder',
        'dialogEditorViewDisplayBuilder', 'extendedDataDisplayBuilder', 'NotificationGatewayProviderDisplay',
        function(genericModelBuilder,
                 dialogEditorViewDisplayBuilder, extendedDataDisplayBuilder, NotificationGatewayProviderDisplay) {

            var Constructor = NotificationGatewayProviderDisplay;

            return {
                createDefault: function() {
                    return new Constructor();
                },
                transform: function(jsonResult) {
                    var providers = [];
                    if(angular.isArray(jsonResult)) {
                        for(var i = 0; i < jsonResult.length; i++) {
                            var provider = genericModelBuilder.transform(jsonResult[ i ], Constructor);
                            provider.dialogEditorView = dialogEditorViewDisplayBuilder.transform(jsonResult[ i ].dialogEditorView);
                            provider.extendedData = extendedDataDisplayBuilder.transform(jsonResult[ i ].extendedData);
                            providers.push(provider);
                        }
                    } else {
                        providers = genericModelBuilder.transform(jsonResult, Constructor);
                        providers.dialogEditorView = dialogEditorViewBuilder.transform(jsonResult.dialogEditorView);
                        providers.extendedData = extendedDataDisplayBuilder.transform(jsonResult.extendedData);
                    }
                    return providers;
                }
            };
        }]);


    /**
     * @ngdoc service
     * @name notificationMessageDisplayBuilder
     *
     * @description
     * A utility service that builds NotificationMessageDisplay models
     */
    angular.module('merchello.models').factory('notificationMessageDisplayBuilder',
        ['genericModelBuilder', 'NotificationMessageDisplay',
            function(genericModelBuilder, NotificationMessageDisplay) {

                var Constructor = NotificationMessageDisplay;
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
     * @name notificationMethodDisplayBuilder
     *
     * @description
     * A utility service that builds NotificationMethodDisplay models
     */
    angular.module('merchello.models').factory('notificationMethodDisplayBuilder',
    ['genericModelBuilder', 'notificationMessageDisplayBuilder', 'NotificationMethodDisplay',
        function(genericModelBuilder, notificationMessageDisplayBuilder, NotificationMethodDisplay) {

            var Constructor = NotificationMethodDisplay;

            return {
                createDefault: function() {
                    return new Constructor();
                },
                transform: function(jsonResult) {
                    var methods = [];
                    if(angular.isArray(jsonResult)) {
                        for(var i = 0; i < jsonResult.length; i++) {
                            var method = genericModelBuilder.transform(jsonResult[ i ], Constructor);
                            method.notificationMessages = notificationMessageDisplayBuilder.transform(jsonResult[ i ].notificationMessages);
                            methods.push(method);
                        }
                    } else {
                        methods = genericModelBuilder.transform(jsonResult, Constructor);
                        methods.notificationMessages = notificationMessageDisplayBuilder.transform(jsonResult.notificationMessages);
                    }
                    return methods;
                }
            };
    }]);

    /**
     * @ngdoc service
     * @name notificationMonitorDisplayBuilder
     *
     * @description
     * A utility service that builds NotificationMonitorDisplayDisplay models
     */
    angular.module('merchello.models').factory('notificationMonitorDisplayBuilder',
        ['genericModelBuilder', 'NotificationMonitorDisplay',
        function(genericModelBuilder, NotificationMonitorDisplay) {
            var Constructor = NotificationMonitorDisplay;
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
                        var payments = [];
                        if (angular.isArray(jsonResult)) {
                            for(var i = 0; i < jsonResult.length; i++) {
                                var payment = genericModelBuilder.transform(jsonResult[ i ], Constructor);
                                payment.appliedPayments = appliedPaymentDisplayBuilder.transform(jsonResult[ i ].appliedPayments);
                                payment.extendedData = extendedDataDisplayBuilder.transform(jsonResult[ i ].extendedData);
                                payments.push(payment);
                            }
                        } else {
                            payments = genericModelBuilder.transform(jsonResult, Constructor);
                            payments.appliedPayments = appliedPaymentDisplayBuilder.transform(jsonResult.appliedPayments);
                            payments.extendedData = extendedDataDisplayBuilder.transform(jsonResult.extendedData);
                        }
                        return payments;
                    }
                };
            }]);
    angular.module('merchello.models').factory('paymentGatewayProviderDisplayBuilder',
        ['genericModelBuilder',
            'dialogEditorViewDisplayBuilder', 'extendedDataDisplayBuilder', 'PaymentGatewayProviderDisplay',
        function(genericModelBuilder,
                 dialogEditorViewDisplayBuilder, extendedDataDisplayBuilder, PaymentGatewayProviderDisplay) {

            var Constructor = PaymentGatewayProviderDisplay;

            return {
                createDefault: function() {
                    return new Constructor();
                },
                transform: function(jsonResult) {
                    var providers = [];
                    if(angular.isArray(jsonResult)) {
                        for(var i = 0; i < jsonResult.length; i++) {
                            var provider = genericModelBuilder.transform(jsonResult[ i ], Constructor);
                            provider.dialogEditorView = dialogEditorViewDisplayBuilder.transform(jsonResult[ i ].dialogEditorView);
                            provider.extendedData = extendedDataDisplayBuilder.transform(jsonResult[ i ].extendedData);
                            providers.push(provider);
                        }
                    } else {
                        providers = genericModelBuilder.transform(jsonResult, Constructor);
                        providers.dialogEditorView = dialogEditorViewBuilder.transform(jsonResult.dialogEditorView);
                        providers.extendedData = extendedDataDisplayBuilder.transform(jsonResult.extendedData);
                    }
                    return providers;
                }
            };
    }]);

    angular.module('merchello.models').factory('paymentMethodDisplayBuilder',
        ['genericModelBuilder', 'dialogEditorViewDisplayBuilder', 'PaymentMethodDisplay',
        function(genericModelBuilder, dialogEditorViewDisplayBuilder, PaymentMethodDisplay) {

            var Constructor = PaymentMethodDisplay;

            return {
                createDefault: function() {
                    var paymentMethod = new Constructor();
                    paymentMethod.dialogEditorView = dialogEditorViewDisplayBuilder.createDefault();
                    paymentMethod.authorizePaymentEditorView = dialogEditorViewDisplayBuilder.createDefault();
                    paymentMethod.authorizeCapturePaymentEditorView = dialogEditorViewDisplayBuilder.createDefault();
                    paymentMethod.voidPaymentEditorView = dialogEditorViewDisplayBuilder.createDefault();
                    paymentMethod.refundPaymentEditorView = dialogEditorViewDisplayBuilder.createDefault();
                    paymentMethod.capturePaymentEditorView = dialogEditorViewDisplayBuilder.createDefault();
                    return paymentMethod;
                },
                transform: function(jsonResult) {
                    var paymentMethods = [];
                    if(angular.isArray(jsonResult)) {
                        for(var i = 0; i < jsonResult.length; i++) {
                            var paymentMethod = genericModelBuilder.transform(jsonResult[ i ], Constructor);
                            paymentMethod.dialogEditorView = dialogEditorViewDisplayBuilder.transform(jsonResult[ i ].dialogEditorView);
                            paymentMethod.authorizePaymentEditorView = dialogEditorViewDisplayBuilder.transform(jsonResult[i].authorizePaymentEditorView);
                            paymentMethod.authorizeCapturePaymentEditorView = dialogEditorViewDisplayBuilder.transform(jsonResult[ i ].authorizeCapturePaymentEditorView);
                            paymentMethod.voidPaymentEditorView = dialogEditorViewDisplayBuilder.transform(jsonResult[ i ].voidPaymentEditorView);
                            paymentMethod.refundPaymentEditorView = dialogEditorViewDisplayBuilder.transform(jsonResult[ i ].refundPaymentEditorView);
                            paymentMethod.capturePaymentEditorView = dialogEditorViewDisplayBuilder.transform(jsonResult[ i ].capturePaymentEditorView);
                            paymentMethods.push(paymentMethod);
                        }
                    } else {
                        paymentMethods = genericModelBuilder.transform(jsonResult, Constructor);
                        paymentMethods.dialogEditorView = dialogEditorViewDisplayBuilder.transform(jsonResult.dialogEditorView);
                        paymentMethods.authorizePaymentEditorView = dialogEditorViewDisplayBuilder.transform(jsonResult.authorizePaymentEditorView);
                        paymentMethods.authorizeCapturePaymentEditorView = dialogEditorViewDisplayBuilder.transform(jsonResult.authorizeCapturePaymentEditorView);
                        paymentMethods.voidPaymentEditorView = dialogEditorViewDisplayBuilder.transform(jsonResult.voidPaymentEditorView);
                        paymentMethods.refundPaymentEditorView = dialogEditorViewDisplayBuilder.transform(jsonResult.refundPaymentEditorView);
                        paymentMethods.capturePaymentEditorView = dialogEditorViewDisplayBuilder.transform(jsonResult.capturePaymentEditorView);
                    }
                    return paymentMethods;
                }
            };
        }]);

    /**
     * @ngdoc models
     * @name catalogInventoryDisplayBuilder
     *
     * @description
     * A utility service that builds ProductAttributeDisplay models
     */
    angular.module('merchello.models').factory('catalogInventoryDisplayBuilder',
        ['genericModelBuilder', 'CatalogInventoryDisplay',
        function(genericModelBuilder, CatalogInventoryDisplay) {

            var Constructor = CatalogInventoryDisplay;
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
     * @ngdoc models
     * @name productAttributeDisplayBuilder
     *
     * @description
     * A utility service that builds ProductAttributeDisplay models
     */
    angular.module('merchello.models').factory('productAttributeDisplayBuilder',
        ['genericModelBuilder', 'ProductAttributeDisplay',
        function(genericModelBuilder, ProductAttributeDisplay) {

            var Constructor = ProductAttributeDisplay;
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
     * @ngdoc models
     * @name productDisplayBuilder
     *
     * @description
     * A utility service that builds ProductDisplay models
     */
    angular.module('merchello.models').factory('productDisplayBuilder',
        ['genericModelBuilder', 'productVariantDisplayBuilder', 'productOptionDisplayBuilder', 'catalogInventoryDisplayBuilder',
            'productVariantDetachedContentDisplayBuilder', 'ProductDisplay',
        function(genericModelBuilder, productVariantDisplayBuilder, productOptionDisplayBuilder, catalogInventoryDisplayBuilder,
                 productVariantDetachedContentDisplayBuilder, ProductDisplay) {

            var Constructor = ProductDisplay;
            return {
                createDefault: function() {
                    return new Constructor();
                },
                transform: function(jsonResult) {
                    var products = [];
                    if(angular.isArray(jsonResult)) {
                        for(var i = 0; i < jsonResult.length; i++) {
                            var product = genericModelBuilder.transform(jsonResult[ i ], Constructor);
                            product.productVariants = productVariantDisplayBuilder.transform(jsonResult[ i ].productVariants);
                            product.productOptions = productOptionDisplayBuilder.transform(jsonResult[ i ].productOptions);
                            product.catalogInventories = catalogInventoryDisplayBuilder.transform(jsonResult[ i ].catalogInventories);
                            product.detachedContents = productVariantDetachedContentDisplayBuilder.transform(jsonResult[i].detachedContents);
                            products.push(product);
                        }
                    } else {
                        products = genericModelBuilder.transform(jsonResult, Constructor);
                        products.productVariants = productVariantDisplayBuilder.transform(jsonResult.productVariants);
                        products.productOptions = productOptionDisplayBuilder.transform(jsonResult.productOptions);
                        products.catalogInventories = catalogInventoryDisplayBuilder.transform(jsonResult.catalogInventories);
                        products.detachedContents = productVariantDetachedContentDisplayBuilder.transform(jsonResult.detachedContents);
                    }
                    return products;

                }
            };

    }]);

    /**
     * @ngdoc models
     * @name productOptionDisplayBuilder
     *
     * @description
     * A utility service that builds ProductOptionDisplay models
     */
    angular.module('merchello.models').factory('productOptionDisplayBuilder',
        ['genericModelBuilder', 'productAttributeDisplayBuilder', 'ProductOptionDisplay',
        function(genericModelBuilder, productAttributeDisplayBuilder, ProductOptionDisplay) {

            var Constructor = ProductOptionDisplay;
            return {
                createDefault: function() {
                    return new Constructor();
                },
                transform: function(jsonResult) {
                    var options = [];
                    if(angular.isArray(jsonResult)) {
                        for(var i = 0; i < jsonResult.length; i++) {
                            var option = genericModelBuilder.transform(jsonResult[ i ], Constructor);
                            option.choices = productAttributeDisplayBuilder.transform(jsonResult[ i ].choices);
                            options.push(option);
                        }
                    } else {
                        options = genericModelBuilder.transform(jsonResult, Constructor);
                        options.choices = productAttributeDisplayBuilder.transform(jsonResult.choices);
                    }
                    return options;
                }
            };

    }]);

/**
 * @ngdoc models
 * @name productVariantDetachedContentDisplayBuilder
 *
 * @description
 * A utility factory that builds ProductVariantDetachedContentDisplay models
 */
angular.module('merchello.models').factory('productVariantDetachedContentDisplayBuilder',
    ['genericModelBuilder', 'detachedContentTypeDisplayBuilder', 'extendedDataDisplayBuilder', 'ProductVariantDetachedContentDisplay',
    function(genericModelBuilder, detachedContentTypeBuilder, extendedDataDisplayBuilder, ProductVariantDetachedContentDisplay) {

        var Constructor = ProductVariantDetachedContentDisplay;

        return {
            createDefault: function() {

                var content = new Constructor();
                content.detachedContentType = detachedContentTypeBuilder.createDefault();
                content.detachedDataValues = extendedDataDisplayBuilder.createDefault();

                return content;
            },
            transform: function(jsonResult) {
                var contents = [];
                if (angular.isArray(jsonResult)) {
                    for(var i = 0; i < jsonResult.length; i++) {
                        var content = genericModelBuilder.transform(jsonResult[ i ], Constructor);
                        content.detachedContentType = detachedContentTypeBuilder.transform(jsonResult[ i ].detachedContentType);
                        content.detachedDataValues = extendedDataDisplayBuilder.transform(jsonResult[ i ].detachedDataValues);
                        contents.push(content);
                    }
                } else {
                    contents = genericModelBuilder.transform(jsonResult, Constructor);
                    contents.detachedContentType = detachedContentTypeBuilder.transform(jsonResult.detachedContentType);
                    contents.detachedDataValues = extendedDataDisplayBuilder.transform(jsonResult.detachedDataValues);
                }
                return contents;
            }
        };
}]);

    /**
     * @ngdoc models
     * @name productVariantDisplayBuilder
     *
     * @description
     * A utility service that builds ProductVariantDisplay models
     */
    angular.module('merchello.models').factory('productVariantDisplayBuilder',
        ['genericModelBuilder', 'productAttributeDisplayBuilder', 'catalogInventoryDisplayBuilder', 'productVariantDetachedContentDisplayBuilder', 'ProductVariantDisplay',
        function(genericModelBuilder, productAttributeDisplayBuilder, catalogInventoryDisplayBuilder, productVariantDetachedContentDisplayBuilder, ProductVariantDisplay) {

            var Constructor = ProductVariantDisplay;
            return {
                createDefault: function() {
                    return new Constructor();
                },
                transform: function(jsonResult) {
                    var variants = [];
                    if(angular.isArray(jsonResult)) {
                        for(var i = 0; i < jsonResult.length; i++) {
                            var variant = genericModelBuilder.transform(jsonResult[ i ], Constructor);
                            variant.attributes = productAttributeDisplayBuilder.transform(jsonResult[ i ].attributes);
                            variant.catalogInventories = catalogInventoryDisplayBuilder.transform(jsonResult[ i ].catalogInventories);
                            variant.detachedContents = productVariantDetachedContentDisplayBuilder.transform(jsonResult[i].detachedContents);
                            variants.push(variant);
                        }
                    } else {
                        variants = genericModelBuilder.transform(jsonResult, Constructor);
                        variants.attributes = productAttributeDisplayBuilder.transform(jsonResult.attributes);
                        variants.catalogInventories = catalogInventoryDisplayBuilder.transform(jsonResult.catalogInventories);
                        variants.detachedContents = productVariantDetachedContentDisplayBuilder.transform(jsonResult.detachedContents);
                    }
                    return variants;
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
                        result.items = itemBuilder.transform(jsonResult.items);
                    }
                    return result;
                }
            };
        }]);

    /**
     * @ngdoc service
     * @name merchello.services.shipMethodsQueryDisplayBuilder
     *
     * @description
     * A utility service that builds ShipMethodsQueryDisplay models
     */
    angular.module('merchello.services')
        .factory('shipMethodsQueryDisplayBuilder',
        ['genericModelBuilder', 'shipMethodDisplayBuilder', 'ShipMethodsQueryDisplay',
        function(genericModelBuilder, shipMethodDisplayBuilder, ShipMethodsQueryDisplay) {

            var Constructor = ShipMethodsQueryDisplay;

            return {
                createDefault: function() {
                    return new Constructor();
                },
                transform: function(jsonResult) {
                    var query = new Constructor();
                    if (jsonResult) {
                        query.selected = shipMethodDisplayBuilder.transform(jsonResult.selected);
                        query.alternatives = shipMethodDisplayBuilder.transform(jsonResult.alternatives);
                    }
                    return query;
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
            'orderDisplayBuilder', 'currencyDisplayBuilder', 'InvoiceDisplay',
            function(genericModelBuilder, invoiceStatusDisplayBuilder, invoiceLineItemDisplayBuilder,
                     orderDisplayBuilder, currencyDisplayBuilder, InvoiceDisplay) {
                var Constructor = InvoiceDisplay;

                return {
                    createDefault: function() {
                        var invoice = new Constructor();
                        invoice.invoiceStatus = invoiceStatusDisplayBuilder.createDefault();
                        invoice.currency = currencyDisplayBuilder.createDefault();
                        return invoice;
                    },
                    transform: function(jsonResult) {
                        var invoices = genericModelBuilder.transform(jsonResult, Constructor);
                        if (angular.isArray(invoices)) {
                            for(var i = 0; i < invoices.length; i++) {
                                invoices[ i ].invoiceStatus = invoiceStatusDisplayBuilder.transform(jsonResult[ i ].invoiceStatus);
                                invoices[ i ].items = invoiceLineItemDisplayBuilder.transform(jsonResult[ i ].items);
                                invoices[ i ].orders = orderDisplayBuilder.transform(jsonResult[ i ].orders);
                                invoices[ i ].currency = currencyDisplayBuilder.transform(jsonResult[ i ].currency);
                            }
                        } else {
                            //jsonResult = JSON.stringify(jsonResult);
                            invoices.invoiceStatus = invoiceStatusDisplayBuilder.transform(jsonResult.invoiceStatus);
                            invoices.items = invoiceLineItemDisplayBuilder.transform(jsonResult.items);
                            invoices.orders = orderDisplayBuilder.transform(jsonResult.orders);
                            invoices.currency = currencyDisplayBuilder.transform(jsonResult.currency);
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
                        if(angular.isArray(invoiceLineItems)) {
                            for(var i = 0; i < invoiceLineItems.length; i++) {
                                invoiceLineItems[ i ].extendedData = extendedDataDisplayBuilder.transform(jsonResult[ i ].extendedData);
                                invoiceLineItems[ i ].lineItemTypeField = typeFieldDisplayBuilder.transform(jsonResult[ i ].lineItemTypeField);
                            }
                        } else {
                            invoiceLineItems.extendedData = extendedDataDisplayBuilder.transform(jsonResult.extendedData);
                            invoiceLineItems.lineItemTypeField = typeFieldDisplayBuilder.transform(jsonResult.lineItemTypeField);
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
 * @name merchello.models.noteDisplayBuilder
 *
 * @description
 * A utility service that builds noteDisplayBuilder models
 */
angular.module('merchello.models')
    .factory('noteDisplayBuilder',
        ['genericModelBuilder', 'NoteDisplay',
            function (genericModelBuilder, NoteDisplay) {

                var Constructor = NoteDisplay;

                return {
                    createDefault: function () {
                        return new Constructor();
                    },
                    transform: function (jsonResult) {
                        var noteDisplay = genericModelBuilder.transform(jsonResult, Constructor);

                        noteDisplay.message = jsonResult.message;

                        return noteDisplay;
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
                        if (angular.isArray(orders)) {
                            for(var i = 0; i < orders.length; i++) {
                                orders[ i ].orderStatus = orderStatusDisplayBuilder.transform(jsonResult[ i ].orderStatus);
                                orders[ i ].items = orderLineItemDisplayBuilder.transform(jsonResult[ i ].items);
                            }
                        } else {
                            if (jsonResult.orderStatus) {
                                orders.orderStatus = orderStatusDisplayBuilder.transform(jsonResult.orderStatus);
                            }
                            if (jsonResult.items) {
                                orders.items = orderLineItemDisplayBuilder.transform(jsonResult.items);
                            }
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
                        if (orderLineItems.length) {
                            for (var i = 0; i < orderLineItems.length; i++) {
                                orderLineItems[i].extendedData = extendedDataDisplayBuilder.transform(jsonResult[i].extendedData);
                                orderLineItems[i].lineItemTypeField = typeFieldDisplayBuilder.transform(jsonResult[i].lineItemTypeField);
                            }
                        } else {
                            orderLineItems.extendedData = extendedDataDisplayBuilder.transform(jsonResult.extendedData);
                            orderLineItems.lineItemTypeField = typeFieldDisplayBuilder.transform(jsonResult.lineItemTypeField);
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

                        // this checks to see if the message in the result is a JSON object
                        if (genericModelBuilder.isStringifyJson(jsonResult.message)) {
                            // if so, this is going to be something we can localize later (get from the lang files)
                            var message = JSON.parse(jsonResult.message);
                            auditLogDisplay.message = salesHistoryMessageDisplayBuilder.transform(message);
                        } else {
                            // otherwise we assume the developer simply put a note into the audit logs and thus
                            // we can't localize.
                            auditLogDisplay.message = salesHistoryMessageDisplayBuilder.createDefault();
                            auditLogDisplay.message.formattedMessage = jsonResult.message;
                        }
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
     * @name merchello.models.shipCountryDisplayBuilder
     *
     * @description
     * A utility service that builds ShipCountryDisplay models
     */
    angular.module('merchello.models')
        .factory('shipCountryDisplayBuilder',
        ['genericModelBuilder', 'shipProvinceDisplayBuilder', 'ShipCountryDisplay',
        function(genericModelBuilder, shipProvinceDisplayBuilder, ShipCountryDisplay) {
            var Constructor = ShipCountryDisplay;
            return {
                createDefault: function() {
                    return new Constructor();
                },
                transform: function(jsonResult) {
                    if(jsonResult === undefined || jsonResult === null) {
                        return;
                    }
                    var countries = genericModelBuilder.transform(jsonResult, Constructor);
                    if(angular.isArray(jsonResult)) {
                        for(var i = 0; i < jsonResult.length; i++) {
                            countries[ i ].provinces = shipProvinceDisplayBuilder.transform(jsonResult[ i ].provinces);
                        }
                    } else {
                        countries.provinces = shipProvinceDisplayBuilder.transform(jsonResult.provinces);
                    }
                    return countries;
                }
            };
        }]);

/**
 * @ngdoc service
 * @name shipFixedRateTableDisplayBuilder
 *
 * @description
 * A utility service that builds ShipFixedRateTableDisplay models
 */
angular.module('merchello.models').factory('shipFixedRateTableDisplayBuilder',
    ['genericModelBuilder', 'shipRateTierDisplayBuilder', 'ShipFixedRateTableDisplay',
    function(genericModelBuilder, shipRateTierDisplayBuilder, ShipFixedRateTableDisplay) {

        var Constructor = ShipFixedRateTableDisplay;

        return {
            createDefault: function() {
                return new Constructor();
            },
            transform: function(jsonResult) {
                var rateTable = genericModelBuilder.transform(jsonResult, Constructor);
                rateTable.rows = shipRateTierDisplayBuilder.transform(jsonResult.rows);
                return rateTable;
            }
        };
    }]);

    /**
     * @ngdoc service
     * @name merchello.models.shipMethodDisplayBuilder
     *
     * @description
     * A utility service that builds ShipMethodDisplay models
     */
    angular.module('merchello.models')
        .factory('shipMethodDisplayBuilder',
            ['genericModelBuilder', 'dialogEditorViewDisplayBuilder', 'shipProvinceDisplayBuilder', 'ShipMethodDisplay',
            function(genericModelBuilder, dialogEditorViewDisplayBuilder, shipProvinceDisplayBuilder, ShipMethodDisplay) {

                var Constructor = ShipMethodDisplay;

                return {
                    createDefault: function() {
                        return new Constructor();
                    },
                    transform: function(jsonResult) {
                        var shipMethods = genericModelBuilder.transform(jsonResult, Constructor);
                        if(!jsonResult) {
                            return;
                        }
                        if (angular.isArray(jsonResult))
                        {
                            for(var i = 0; i < jsonResult.length; i++) {
                                // todo these should never be returned by the api
                                if(jsonResult[i] !== null) {
                                    shipMethods[ i ].provinces = shipProvinceDisplayBuilder.transform(jsonResult[ i ].provinces);
                                }
                            }
                        } else {
                            if(jsonResult.provinces) {
                                shipMethods.provinces = shipProvinceDisplayBuilder.transform(jsonResult.provinces);
                            }
                        }
                        return shipMethods;
                    }
                };

        }]);
    /**
     * @ngdoc service
     * @name merchello.models.shipProvinceDisplayBuilder
     *
     * @description
     * A utility service that builds ShipProvinceDisplay models
     */
    angular.module('merchello.services').factory('shipProvinceDisplayBuilder',
        ['genericModelBuilder', 'ShipProvinceDisplay', function(genericModelBuilder, ShipProvinceDisplay) {

            var Constructor = ShipProvinceDisplay;

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
 * @name shipRateTierDisplayBuilder
 *
 * @description
 * A utility service that builds ShipRateTierDisplay models
 */
angular.module('merchello.models').factory('shipRateTierDisplayBuilder',
    ['genericModelBuilder', 'ShipRateTierDisplay',
        function(genericModelBuilder, ShipRateTierDisplay) {
            var Constructor = ShipRateTierDisplay;
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

angular.module('merchello.models').factory('shippingGatewayMethodDisplayBuilder',
    ['genericModelBuilder', 'shipMethodDisplayBuilder', 'shipCountryDisplayBuilder', 'gatewayResourceDisplayBuilder',
        'dialogEditorViewDisplayBuilder', 'ShippingGatewayMethodDisplay',
        function(genericModelBuilder, shipMethodDisplayBuilder, shipCountryDisplayBuilder, gatewayResourceDisplayBuilder,
                 dialogEditorViewDisplayBuilder, ShippingGatewayMethodDisplay) {

            var Constructor = ShippingGatewayMethodDisplay;

            return {
                createDefault: function() {
                    return new Constructor();
                },
                transform: function(jsonResult) {

                    if(angular.isArray(jsonResult)) {
                        var methods = [];
                        angular.forEach(jsonResult, function(result) {
                            var method = new Constructor();
                            method.gatewayResource = gatewayResourceDisplayBuilder.transform(result.gatewayResource);
                            method.shipMethod = shipMethodDisplayBuilder.transform(result.shipMethod);
                            method.shipCountry = shipCountryDisplayBuilder.transform(result.shipCountry);
                            method.dialogEditorView = dialogEditorViewDisplayBuilder.transform(result.dialogEditorView);
                            methods.push(method);
                        });
                        return methods;
                    } else {
                        var method = new Constructor();
                        method.gatewayResource = gatewayResourceDisplayBuilder.transform(jsonResult.gatewayResource);
                        method.shipMethod = shipMethodDisplayBuilder.transform(jsonResult.shipMethod);
                        method.shipCountry = shipCountryDisplayBuilder.transform(jsonResult.shipCountry);
                        method.dialogEditorView = dialogEditorViewDisplayBuilder.transform(jsonResult.dialogEditorView);
                        return method;
                    }

                }
            };

    }]);

angular.module('merchello.models').factory('shippingGatewayProviderDisplayBuilder',
    ['genericModelBuilder', 'extendedDataDisplayBuilder', 'shipMethodDisplayBuilder', 'gatewayResourceDisplayBuilder', 'ShippingGatewayProviderDisplay',
        function(genericModelBuilder, extendedDataDisplayBuilder, shipMethodDisplayBuilder, gatewayResourceDisplayBuilder, ShippingGatewayProviderDisplay) {

            var Constructor = ShippingGatewayProviderDisplay;

            return {
                createDefault: function() {
                    return new Constructor();
                },
                transform: function(jsonResult) {
                    var providers = genericModelBuilder.transform(jsonResult, Constructor);
                    if(angular.isArray(providers)) {
                        for(var i = 0; i < providers.length; i++) {
                            providers[ i ].extendedData = extendedDataDisplayBuilder.transform(jsonResult[ i ].extendedData);
                            providers[ i ].shipMethods = shipMethodDisplayBuilder.transform(jsonResult[ i ].shipMethods);
                            providers[ i ].availableResources = gatewayResourceDisplayBuilder.transform(jsonResult[ i ].availableResources);
                        }
                    } else {
                        providers.extendedData = extendedDataDisplayBuilder.transform(jsonResult.extendedData);
                        providers.shipMethods = shipmentDisplayBuilder.transform(jsonResult.shipMethods);
                        providers.availableResources = gatewayResourceDisplayBuilder.transform(jsonResult.availableResources);
                    }

                    return providers;
                }
            };
    }]);

    /**
     * @ngdoc service
     * @name merchello.models.gatewayProviderDisplayBuilder
     *
     * @description
     * A utility service that builds GatewayProviderDisplay models
     */
angular.module('merchello.models').factory('taxationGatewayProviderDisplayBuilder',
        ['genericModelBuilder', 'extendedDataDisplayBuilder', 'dialogEditorViewDisplayBuilder', 'TaxationGatewayProviderDisplay',
        function (genericModelBuilder, extendedDataDisplayBuilder, dialogEditorViewDisplayBuilder, TaxationGatewayProviderDisplay) {

            var Constructor = TaxationGatewayProviderDisplay;

            return {
                createDefault: function () {
                    var gatewayProvider = new Constructor();
                    gatewayProvider.extendedData = extendedDataDisplayBuilder.createDefault();
                    gatewayProvider.dialogEditorView = dialogEditorViewDisplayBuilder.createDefault();
                    return gatewayProvider;
                },
                transform: function (jsonResult) {
                    var gatewayProviders = genericModelBuilder.transform(jsonResult, Constructor);
                    if (angular.isArray(gatewayProviders)) {
                        for (var i = 0; i < gatewayProviders.length; i++) {
                            gatewayProviders[i].extendedData = extendedDataDisplayBuilder.transform(jsonResult[i].extendedData);
                            gatewayProviders[i].dialogEditorView = dialogEditorViewDisplayBuilder.transform(jsonResult[i].dialogEditorView);
                        }
                    } else {
                        gatewayProviders.extendedData = extendedDataDisplayBuilder.transform(jsonResult.extendedData);
                        gatewayProviders.dialogEditorView = dialogEditorViewDisplayBuilder.transform(jsonResult.dialogEditorView);
                    }
                    return gatewayProviders;
                }
            };
        }]);

    /**
     * @ngdoc service
     * @name taxCountryDisplayBuilder
     *
     * @description
     * A utility service that builds TaxCountryDisplay models
     */
    angular.module('merchello.models').factory('taxCountryDisplayBuilder', [
        'genericModelBuilder', 'gatewayResourceDisplayBuilder', 'TaxCountryDisplay',
        function(genericModelBuilder, gatewayResourceDisplayBuilder, TaxCountryDisplay) {

            var Constructor = TaxCountryDisplay;

            function buildSingle(resource) {
                var taxCountry = new Constructor();
                taxCountry.name = resource.name;
                taxCountry.gatewayResource = resource;
                return taxCountry;
            }

            return {
                createDefault: function() {
                    return buildSingle(gatewayResourceDisplayBuilder.createDefault());
                },
                transform: function(jsonResult) {
                    var resources = gatewayResourceDisplayBuilder.transform(jsonResult);
                    var countries = [];
                    if(angular.isArray(resources)) {
                        angular.forEach(resources, function(resource) {
                            countries.push(buildSingle(resource));
                        });
                    } else {
                        countries = buildSingle(resources);
                    }
                    return countries;
                }
            };
        }]);

    /**
     * @ngdoc service
     * @name taxMethodDisplayBuilder
     *
     * @description
     * A utility service that builds TaxMethodDisplay models
     */
    angular.module('merchello.models').factory('taxMethodDisplayBuilder',
        ['genericModelBuilder', 'taxProvinceDisplayBuilder', 'TaxMethodDisplay',
            function(genericModelBuilder, taxProvinceDisplayBuilder, TaxMethodDisplay) {

                var Constructor = TaxMethodDisplay;

                return {
                    createDefault: function() {
                        return new Constructor();
                    },
                    transform: function(jsonResult) {
                        var methods = [];
                        if(angular.isArray(jsonResult)) {
                            for(var i = 0; i < jsonResult.length; i++) {
                                var method = genericModelBuilder.transform(jsonResult[ i ]);
                                method.provinces = taxProvinceDisplayBuilder.transform(jsonResult[ i ].provinces);
                                methods.push(method);
                            }
                        } else {
                            methods = genericModelBuilder.transform(jsonResult, Constructor);
                            methods.provinces = taxProvinceDisplayBuilder.transform(jsonResult.provinces);
                        }
                        return methods;
                    }
                };
    }]);

    /**
     * @ngdoc service
     * @name taxProvinceDisplayBuilder
     *
     * @description
     * A utility service that builds TaxProvinceDisplay models
     */
    angular.module('merchello.models').factory('taxProvinceDisplayBuilder',
        ['genericModelBuilder', 'TaxProvinceDisplay',
        function(genericModelBuilder, TaxProvinceDisplay) {

            var Constructor = TaxProvinceDisplay;

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

    /**
     * @ngdoc service
     * @name merchello.models.warehouseCatalogDisplayBuilder
     *
     * @description
     * A utility service that builds WarehouseCatalogDisplay models
     */
    angular.module('merchello.models')
        .factory('warehouseCatalogDisplayBuilder',
        ['genericModelBuilder', 'WarehouseCatalogDisplay',
            function(genericModelBuilder, WarehouseCatalogDisplay) {

                var Constructor = WarehouseCatalogDisplay;
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
     * @name merchello.models.warehouseDisplayBuilder
     *
     * @description
     * A utility service that builds WarehouseDisplay models
     */
    angular.module('merchello.models')
        .factory('warehouseDisplayBuilder',
        ['genericModelBuilder', 'warehouseCatalogDisplayBuilder', 'WarehouseDisplay',
        function(genericModelBuilder, warehouseCatalogDisplayBuilder, WarehouseDisplay) {

            var Constructor = WarehouseDisplay;
            return {
                createDefault: function() {
                    return new Constructor();
                },
                transform: function(jsonResult) {
                    var warehouse = genericModelBuilder.transform(jsonResult, Constructor);
                    warehouse.warehouseCatalogs = warehouseCatalogDisplayBuilder.transform(jsonResult.warehouseCatalogs);
                    return warehouse;
                }
            };

    }]);


})();