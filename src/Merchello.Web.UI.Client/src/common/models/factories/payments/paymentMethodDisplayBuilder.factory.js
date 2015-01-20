    angular.module('merchello.models').factory('paymentMethodDisplayBuilder',
        ['genericModelBuilder', 'dialogEditorViewDisplayBuilder', 'PaymentMethodDisplay',
        function(genericModelBuilder, dialogEditorViewDisplayBuilder, PaymentMethodDisplay) {

            var Constructor = PaymentMethodDisplay;

            return {
                createDefault: function() {
                    var paymentMethod = new Constructor();
                    paymentMethod.dialogEditorView = dialogEditorViewDisplayBuilder.createDefault();
                    return paymentMethod;
                },
                transform: function(jsonResult) {
                    var paymentMethods = [];
                    if(angular.isArray(jsonResult)) {
                        for(var i = 0; i < jsonResult.length; i++) {
                            var paymentMethod = genericModelBuilder.transform(jsonResult[ i ], Constructor);
                            paymentMethod.dialogEditorView = dialogEditorViewDisplayBuilder.transform(jsonResult[ i ].dialogEditorView);
                            paymentMethods.push(paymentMethod);
                        }
                    } else {
                        var paymentMethods = genericModelBuilder.transform(jsonResult, Constructor);
                        paymentMethods.dialogEditorView = dialogEditorViewDisplayBuilder.transform(jsonResult.dialogEditorView);
                    }
                    return paymentMethods;
                }
            };
        }]);
