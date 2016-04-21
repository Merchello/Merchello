angular.module('merchello.models').factory('shipmentRateQuoteDisplayBuilder',
    ['genericModelBuilder', 'shipmentDisplayBuilder', 'shipMethodDisplayBuilder', 'extendedDataDisplayBuilder', 'ShipmentRateQuoteDisplay',
    function(genericModelBuilder, shipmentDisplayBuilder, shipMethodDisplayBuilder, extendedDataDisplayBuilder, ShipmentRateQuoteDisplay) {
        var Constructor = ShipmentRateQuoteDisplay;

        return {
            createDefault: function() {
                var quote = new Constructor();
                quote.shipment = shipmentDisplayBuilder.createDefault();
                quote.shipMethod = shipMethodDisplayBuilder.createDefault();
                quote.extendedData = extendedDataDisplayBuilder.createDefault();
                quote.rate = 0;
                return quote;
            },
            transform: function(jsonResult) {
                var quotes = [];
                if (angular.isArray(jsonResult)) {
                    for(var i = 0; i < jsonResult.length; i++) {
                        var quote = genericModelBuilder.transform(jsonResult[ i ], Constructor);
                        quote.shipment = shipmentDisplayBuilder.transform(jsonResult[ i ].shipment);
                        quote.shipMethod = shipMethodDisplayBuilder.transform(jsonResult[ i ].shipMethod);
                        quote.extendedData = extendedDataDisplayBuilder.transform(jsonResult[ i ].extendedData);
                        quotes.push(quote);
                    }
                } else {
                    quotes = genericModelBuilder.transform(jsonResult, Constructor);
                    quotes.shipment = shipmentDisplayBuilder.transform(jsonResult.shipment);
                    quotes.shipMethod = shipMethodDisplayBuilder.transform(jsonResult.shipMethod);
                    quotes.extendedData = extendedDataDisplayBuilder.transform(jsonResult.extendedData);
                }

                return quotes;
            }
        };
    }]);
