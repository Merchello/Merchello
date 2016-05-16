MUI.Utilities = {
    // Ensures a null or undefined value has either a value or a default value
    // defaultValue itself defaults to an empty string.
    EnsureNullAsValue: function(value, defaultValue) {
        if (defaultValue === undefined) {
            defaultValue = '';
        }

        return value === null || value === undefined ? defaultValue : value;
    },

    // Gets a query string parameter value
    // http://stackoverflow.com/questions/901115/how-can-i-get-query-string-values-in-javascript
    getQueryStringValue: function(name, url) {
        if (!url) url = window.location.href;
        url = url.toLowerCase(); // This is just to avoid case sensitiveness
        name = name.replace(/[\[\]]/g, "\\$&").toLowerCase();// This is just to avoid case sensitiveness for query parameter name
        var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
            results = regex.exec(url);
        if (!results) return undefined;
        if (!results[2]) return '';
        return decodeURIComponent(results[2].replace(/\+/g, " "));
    },


    // Credit card types
    // Adapted from https://github.com/braintree/credit-card-type
    CardTypes : {

        types: [
            {
                niceType: 'Visa',
                type: 'visa',
                pattern: '^4\\d*$',
                gaps: [4, 8, 12],
                lengths: [16],
                code: {
                    name: 'CVV',
                    size: 3
                }
            },
            {
                niceType: 'MasterCard',
                type: 'master-card',
                pattern: '^5[1-5]\\d*$',
                gaps: [4, 8, 12],
                lengths: [16],
                code: {
                    name: 'CVC',
                    size: 3
                }
            },
            {
                niceType: 'American Express',
                type: 'american-express',
                pattern: '^3[47]\\d*$',
                isAmex: true,
                gaps: [4, 10],
                lengths: [15],
                code: {
                    name: 'CID',
                    size: 4
                }
            },
            {
                niceType: 'DinersClub',
                type: 'diners-club',
                pattern: '^3(0[0-5]|[689])\\d*$',
                gaps: [4, 10],
                lengths: [14],
                code: {
                    name: 'CVV',
                    size: 3
                }
            },
            {
                niceType: 'Discover',
                type: 'discover',
                pattern: '^6(011|5|4[4-9])\\d*$',
                gaps: [4, 8, 12],
                lengths: [16],
                code: {
                    name: 'CID',
                    size: 3
                }
            },
            {
                niceType: 'JCB',
                type: 'jcb',
                pattern: '^(2131|1800|35)\\d*$',
                gaps: [4, 8, 12],
                lengths: [16],
                code: {
                    name: 'CVV',
                    size: 3
                }
            },
            {
                niceType: 'UnionPay',
                type: 'unionpay',
                pattern: '^62\\d*$',
                gaps: [4, 8, 12],
                lengths: [16, 17, 18, 19],
                code: {
                    name: 'CVN',
                    size: 3
                }
            },
            {
                niceType: 'Maestro',
                type: 'maestro',
                pattern: '^(50|5[6-9]|6)\\d*$',
                gaps: [4, 8, 12],
                lengths: [12, 13, 14, 15, 16, 17, 18, 19],
                code: {
                    name: 'CVC',
                    size: 3
                }
            }
        ],

        getCardType: function (cardNumber) {
            var key, value;
            var noMatch = {};

            if (!isString(cardNumber)) {
                return noMatch;
            }

            for (key in types) {
                if (!MUI.Utilities.CardTypes.hasOwnProperty(key)) {
                    continue;
                }

                value = MUI.Utilities.CardTypes.types[key];

                if (RegExp(value.pattern).test(cardNumber)) {
                    return clone(value);
                }
            }

            return noMatch;
        },

        getCardByType: function (type) {
            return _.find(MUI.Utilities.CardTypes.types, function (t) {
                return t.type === type;
            });
        }
    }
};
