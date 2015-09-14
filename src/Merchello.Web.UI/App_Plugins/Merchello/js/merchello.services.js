/*! merchello
 * https://github.com/meritage/Merchello
 * Copyright (c) 2015 Merchello;
 * Licensed MIT
 */

(function() { 

angular.module('merchello.services').service('dateHelper', [function() {

    this.convertToIsoDate = function(dateString, dateFormat) {
        // date formats in merchello start with MM, dd, or yyyy
        if(dateString.indexOf('/') === -1) {
            dateString = dateString.replace(/-/g, '/');
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
        console.info(date);
        return date.toISOString();
    }

}]);

angular.module('merchello.services').factory('detachedContentHelper',
    ['$q', 'fileManager', 'formHelper', 'notificationsService',
    function($q, fileManager, formHelper, notificationsService) {

        return {

            detachedContentPerformSave: function(args) {

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

                var deferred = $q.defer();
                if (args.scope.preValuesLoaded && formHelper.submitForm({ scope: args.scope, statusMessage: args.statusMessage })) {
                    args.scope.preValuesLoaded = false;

                    // get any files from the fileManager
                    var files = fileManager.getFiles();


                    // save the current language only
                    angular.forEach(args.scope.contentTabs, function(ct) {
                        if (ct.id === 'render') {
                            args.scope.detachedContent.slug = _.find(ct.properties, function(s) { return s.alias === 'slug'}).value;
                            args.scope.detachedContent.templateId = _.find(ct.properties, function(t) { return t.alias === 'templateId' }).value;
                            args.scope.detachedContent.canBeRendered = _.find(ct.properties, function(r) { return r.alias === 'canBeRendered'}).value === '1' ? true : false;
                        } else {
                            angular.forEach(ct.properties, function (p) {
                                if (typeof p.value !== "function") {
                                    args.scope.detachedContent.detachedDataValues.setValue(p.alias, angular.toJson(p.value));
                                }
                            });
                        }
                    });

                    args.saveMethod(args.content, args.scope.language.isoCode, files).then(function(data) {
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

        }

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
                };
            }
        }]);



    /**
     * @ngdoc service
     * @name gravatarService
     * @description Deals with gravatar.
     **/
    angular.module('merchello.services').service('gravatarService',
        function() {

            /**
             * @ngdoc method
             * @name getAvatarUrl
             * @function
             *
             * @description
             * Returns the url for the gravatar
             */
            this.getAvatarUrl = function (string, params) {

                function RotateLeft(lValue, iShiftBits) {
                    return (lValue << iShiftBits) | (lValue >>> (32 - iShiftBits));
                }

                function AddUnsigned(lX, lY) {
                    var lX4, lY4, lX8, lY8, lResult;
                    lX8 = (lX & 0x80000000);
                    lY8 = (lY & 0x80000000);
                    lX4 = (lX & 0x40000000);
                    lY4 = (lY & 0x40000000);
                    lResult = (lX & 0x3FFFFFFF) + (lY & 0x3FFFFFFF);
                    if (lX4 & lY4) {
                        return (lResult ^ 0x80000000 ^ lX8 ^ lY8);
                    }
                    if (lX4 | lY4) {
                        if (lResult & 0x40000000) {
                            return (lResult ^ 0xC0000000 ^ lX8 ^ lY8);
                        } else {
                            return (lResult ^ 0x40000000 ^ lX8 ^ lY8);
                        }
                    } else {
                        return (lResult ^ lX8 ^ lY8);
                    }
                }

                function F(x, y, z) { return (x & y) | ((~x) & z); }
                function G(x, y, z) { return (x & z) | (y & (~z)); }
                function H(x, y, z) { return (x ^ y ^ z); }
                function I(x, y, z) { return (y ^ (x | (~z))); }

                function FF(a, b, c, d, x, s, ac) {
                    a = AddUnsigned(a, AddUnsigned(AddUnsigned(F(b, c, d), x), ac));
                    return AddUnsigned(RotateLeft(a, s), b);
                }

                function GG(a, b, c, d, x, s, ac) {
                    a = AddUnsigned(a, AddUnsigned(AddUnsigned(G(b, c, d), x), ac));
                    return AddUnsigned(RotateLeft(a, s), b);
                }

                function HH(a, b, c, d, x, s, ac) {
                    a = AddUnsigned(a, AddUnsigned(AddUnsigned(H(b, c, d), x), ac));
                    return AddUnsigned(RotateLeft(a, s), b);
                }

                function II(a, b, c, d, x, s, ac) {
                    a = AddUnsigned(a, AddUnsigned(AddUnsigned(I(b, c, d), x), ac));
                    return AddUnsigned(RotateLeft(a, s), b);
                }

                function ConvertToWordArray(string) {
                    var lWordCount;
                    var lMessageLength = string.length;
                    var lNumberOfWords_temp1 = lMessageLength + 8;
                    var lNumberOfWords_temp2 = (lNumberOfWords_temp1 - (lNumberOfWords_temp1 % 64)) / 64;
                    var lNumberOfWords = (lNumberOfWords_temp2 + 1) * 16;
                    var lWordArray = Array(lNumberOfWords - 1);
                    var lBytePosition = 0;
                    var lByteCount = 0;
                    while (lByteCount < lMessageLength) {
                        lWordCount = (lByteCount - (lByteCount % 4)) / 4;
                        lBytePosition = (lByteCount % 4) * 8;
                        lWordArray[lWordCount] = (lWordArray[lWordCount] | (string.charCodeAt(lByteCount) << lBytePosition));
                        lByteCount++;
                    }
                    lWordCount = (lByteCount - (lByteCount % 4)) / 4;
                    lBytePosition = (lByteCount % 4) * 8;
                    lWordArray[lWordCount] = lWordArray[lWordCount] | (0x80 << lBytePosition);
                    lWordArray[lNumberOfWords - 2] = lMessageLength << 3;
                    lWordArray[lNumberOfWords - 1] = lMessageLength >>> 29;
                    return lWordArray;
                }

                function WordToHex(lValue) {
                    var WordToHexValue = "", WordToHexValue_temp = "", lByte, lCount;
                    for (lCount = 0; lCount <= 3; lCount++) {
                        lByte = (lValue >>> (lCount * 8)) & 255;
                        WordToHexValue_temp = "0" + lByte.toString(16);
                        WordToHexValue = WordToHexValue + WordToHexValue_temp.substr(WordToHexValue_temp.length - 2, 2);
                    }
                    return WordToHexValue;
                }

                function Utf8Encode(string) {
                    string = string.replace(/\r\n/g, "\n");
                    var utftext = "";

                    for (var n = 0; n < string.length; n++) {

                        var c = string.charCodeAt(n);

                        if (c < 128) {
                            utftext += String.fromCharCode(c);
                        }
                        else if ((c > 127) && (c < 2048)) {
                            utftext += String.fromCharCode((c >> 6) | 192);
                            utftext += String.fromCharCode((c & 63) | 128);
                        }
                        else {
                            utftext += String.fromCharCode((c >> 12) | 224);
                            utftext += String.fromCharCode(((c >> 6) & 63) | 128);
                            utftext += String.fromCharCode((c & 63) | 128);
                        }

                    }

                    return utftext;
                }

                var gravatarUrl = '';
                if (string !== '') {
                    var x = Array();
                    var k, AA, BB, CC, DD, a, b, c, d;
                    var S11 = 7, S12 = 12, S13 = 17, S14 = 22;
                    var S21 = 5, S22 = 9, S23 = 14, S24 = 20;
                    var S31 = 4, S32 = 11, S33 = 16, S34 = 23;
                    var S41 = 6, S42 = 10, S43 = 15, S44 = 21;

                    string = Utf8Encode(string);

                    x = ConvertToWordArray(string);

                    a = 0x67452301;
                    b = 0xEFCDAB89;
                    c = 0x98BADCFE;
                    d = 0x10325476;

                    for (k = 0; k < x.length; k += 16) {
                        AA = a;
                        BB = b;
                        CC = c;
                        DD = d;
                        a = FF(a, b, c, d, x[k + 0], S11, 0xD76AA478);
                        d = FF(d, a, b, c, x[k + 1], S12, 0xE8C7B756);
                        c = FF(c, d, a, b, x[k + 2], S13, 0x242070DB);
                        b = FF(b, c, d, a, x[k + 3], S14, 0xC1BDCEEE);
                        a = FF(a, b, c, d, x[k + 4], S11, 0xF57C0FAF);
                        d = FF(d, a, b, c, x[k + 5], S12, 0x4787C62A);
                        c = FF(c, d, a, b, x[k + 6], S13, 0xA8304613);
                        b = FF(b, c, d, a, x[k + 7], S14, 0xFD469501);
                        a = FF(a, b, c, d, x[k + 8], S11, 0x698098D8);
                        d = FF(d, a, b, c, x[k + 9], S12, 0x8B44F7AF);
                        c = FF(c, d, a, b, x[k + 10], S13, 0xFFFF5BB1);
                        b = FF(b, c, d, a, x[k + 11], S14, 0x895CD7BE);
                        a = FF(a, b, c, d, x[k + 12], S11, 0x6B901122);
                        d = FF(d, a, b, c, x[k + 13], S12, 0xFD987193);
                        c = FF(c, d, a, b, x[k + 14], S13, 0xA679438E);
                        b = FF(b, c, d, a, x[k + 15], S14, 0x49B40821);
                        a = GG(a, b, c, d, x[k + 1], S21, 0xF61E2562);
                        d = GG(d, a, b, c, x[k + 6], S22, 0xC040B340);
                        c = GG(c, d, a, b, x[k + 11], S23, 0x265E5A51);
                        b = GG(b, c, d, a, x[k + 0], S24, 0xE9B6C7AA);
                        a = GG(a, b, c, d, x[k + 5], S21, 0xD62F105D);
                        d = GG(d, a, b, c, x[k + 10], S22, 0x2441453);
                        c = GG(c, d, a, b, x[k + 15], S23, 0xD8A1E681);
                        b = GG(b, c, d, a, x[k + 4], S24, 0xE7D3FBC8);
                        a = GG(a, b, c, d, x[k + 9], S21, 0x21E1CDE6);
                        d = GG(d, a, b, c, x[k + 14], S22, 0xC33707D6);
                        c = GG(c, d, a, b, x[k + 3], S23, 0xF4D50D87);
                        b = GG(b, c, d, a, x[k + 8], S24, 0x455A14ED);
                        a = GG(a, b, c, d, x[k + 13], S21, 0xA9E3E905);
                        d = GG(d, a, b, c, x[k + 2], S22, 0xFCEFA3F8);
                        c = GG(c, d, a, b, x[k + 7], S23, 0x676F02D9);
                        b = GG(b, c, d, a, x[k + 12], S24, 0x8D2A4C8A);
                        a = HH(a, b, c, d, x[k + 5], S31, 0xFFFA3942);
                        d = HH(d, a, b, c, x[k + 8], S32, 0x8771F681);
                        c = HH(c, d, a, b, x[k + 11], S33, 0x6D9D6122);
                        b = HH(b, c, d, a, x[k + 14], S34, 0xFDE5380C);
                        a = HH(a, b, c, d, x[k + 1], S31, 0xA4BEEA44);
                        d = HH(d, a, b, c, x[k + 4], S32, 0x4BDECFA9);
                        c = HH(c, d, a, b, x[k + 7], S33, 0xF6BB4B60);
                        b = HH(b, c, d, a, x[k + 10], S34, 0xBEBFBC70);
                        a = HH(a, b, c, d, x[k + 13], S31, 0x289B7EC6);
                        d = HH(d, a, b, c, x[k + 0], S32, 0xEAA127FA);
                        c = HH(c, d, a, b, x[k + 3], S33, 0xD4EF3085);
                        b = HH(b, c, d, a, x[k + 6], S34, 0x4881D05);
                        a = HH(a, b, c, d, x[k + 9], S31, 0xD9D4D039);
                        d = HH(d, a, b, c, x[k + 12], S32, 0xE6DB99E5);
                        c = HH(c, d, a, b, x[k + 15], S33, 0x1FA27CF8);
                        b = HH(b, c, d, a, x[k + 2], S34, 0xC4AC5665);
                        a = II(a, b, c, d, x[k + 0], S41, 0xF4292244);
                        d = II(d, a, b, c, x[k + 7], S42, 0x432AFF97);
                        c = II(c, d, a, b, x[k + 14], S43, 0xAB9423A7);
                        b = II(b, c, d, a, x[k + 5], S44, 0xFC93A039);
                        a = II(a, b, c, d, x[k + 12], S41, 0x655B59C3);
                        d = II(d, a, b, c, x[k + 3], S42, 0x8F0CCC92);
                        c = II(c, d, a, b, x[k + 10], S43, 0xFFEFF47D);
                        b = II(b, c, d, a, x[k + 1], S44, 0x85845DD1);
                        a = II(a, b, c, d, x[k + 8], S41, 0x6FA87E4F);
                        d = II(d, a, b, c, x[k + 15], S42, 0xFE2CE6E0);
                        c = II(c, d, a, b, x[k + 6], S43, 0xA3014314);
                        b = II(b, c, d, a, x[k + 13], S44, 0x4E0811A1);
                        a = II(a, b, c, d, x[k + 4], S41, 0xF7537E82);
                        d = II(d, a, b, c, x[k + 11], S42, 0xBD3AF235);
                        c = II(c, d, a, b, x[k + 2], S43, 0x2AD7D2BB);
                        b = II(b, c, d, a, x[k + 9], S44, 0xEB86D391);
                        a = AddUnsigned(a, AA);
                        b = AddUnsigned(b, BB);
                        c = AddUnsigned(c, CC);
                        d = AddUnsigned(d, DD);
                    }

                    var temp = WordToHex(a) + WordToHex(b) + WordToHex(c) + WordToHex(d);

                    gravatarUrl = temp.toLowerCase();
                    gravatarUrl = '//www.gravatar.com/avatar/' + gravatarUrl;
                } else {
                    gravatarUrl = '//www.gravatar.com/avatar/fakeurl';
                }
                if (!params) {
                    params = {};
                }
                var size = '64', defaultImage = 'mm';
                if (params.size) {
                    size = params.size;
                }
                if (params.d) {
                    defaultImage = params.d;
                }
                gravatarUrl += '?size=' + size + '&d=' + defaultImage;
                return gravatarUrl;
            }
    });
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
            return (pad + str).slice(-num)
        };

}]);

/**
 * @ngdoc service
 * @name merchelloListViewHelper
 * @description Handles list view configurations.
 **/
angular.module('merchello.services').service('merchelloListViewHelper',
    ['$filter',
    function() {

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
            }

        };

        this.getConfig = function(entityType) {
            var ensure = entityType.toLowerCase();
            return configs[ensure];
        };

}]);


})();