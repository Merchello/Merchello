    angular.module('merchello.models').factory('paymentMethodDisplayBuilder',
        ['genericModelBuilder', 'dialogEditorViewDisplayBuilder', 'PaymentMethodDisplay',
        function(genericModelBuilder, dialogEditorViewDisplayBuilder, PaymentMethodDisplay) {

            var Constructor = PaymentMethodDisplay;

            return {
                createDefault: function() {
                    var paymentMethod = new Constructor();
                    paymentMethod.dialogEditorView = dialogEditorViewDisplayBuilder.createDefault();
                    paymentMethod.authorizeCapturePaymentEditorView = dialogEditorViewDisplayBuilder.createDefault();
                    paymentMethod.voidPaymentEditorView = dialogEditorViewDisplayBuilder.createDefault();
                    paymentMethod.refundPaymentEditorView = dialogEditorViewDisplayBuilder.createDefault();
                    return paymentMethod;
                },
                transform: function(jsonResult) {
                    var paymentMethods = [];
                    if(angular.isArray(jsonResult)) {
                        for(var i = 0; i < jsonResult.length; i++) {
                            var paymentMethod = genericModelBuilder.transform(jsonResult[ i ], Constructor);
                            paymentMethod.dialogEditorView = dialogEditorViewDisplayBuilder.transform(jsonResult[ i ].dialogEditorView);
                            paymentMethod.authorizeCapturePaymentEditorView = dialogEditorViewDisplayBuilder.transform(jsonResult[ i ].authorizeCapturePaymentEditorView);
                            paymentMethod.voidPaymentEditorView = dialogEditorViewDisplayBuilder.transform(jsonResult[ i ].voidPaymentEditorView);
                            paymentMethod.refundPaymentEditorView = dialogEditorViewDisplayBuilder.transform(jsonResult[ i ].refundPaymentEditorView);
                            paymentMethods.push(paymentMethod);
                        }
                    } else {
                        paymentMethods = genericModelBuilder.transform(jsonResult, Constructor);
                        paymentMethods.dialogEditorView = dialogEditorViewDisplayBuilder.transform(jsonResult.dialogEditorView);
                        paymentMethods.authorizeCapturePaymentEditorView = dialogEditorViewDisplayBuilder.transform(jsonResult.authorizeCapturePaymentEditorView);
                        paymentMethods.voidPaymentEditorView = dialogEditorViewDisplayBuilder.transform(jsonResult.voidPaymentEditorView);
                        paymentMethods.refundPaymentEditorView = dialogEditorViewDisplayBuilder.transform(jsonResult.refundPaymentEditorView);
                    }
                    return paymentMethods;
                }
            };
        }]);
