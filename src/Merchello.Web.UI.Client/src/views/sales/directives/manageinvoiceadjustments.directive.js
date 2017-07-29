angular.module('merchello.directives').directive('manageInvoiceAdjustments',
    ['$timeout', 'dialogService', 'userService', 'invoiceDisplayBuilder', 'invoiceResource',
    function($timeout, dialogService, userService, invoiceDisplayBuilder, invoiceResource) {

        return {
            restrict: 'E',
            replace: true,
            scope: {
                invoice: '=',
                payments: '=',
                allPayments: '=',
                paymentMethods: '=',
                preValuesLoaded: '=',
                currencySymbol: '=',
                reload: '&'
            },
            templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/directives/manageinvoiceadjustments.tpl.html',
            link: function (scope, elm, attr) {

                scope.openAdjustmentDialog = openAdjustmentDialog;
                scope.loaded = false;
                
                function init() {

                    // ensure that the parent scope promises have been resolved
                    scope.$watch('preValuesLoaded', function(pvl) {
                        if(pvl === true) {
                            scope.loaded = true;
                        }
                    });

                }


                function openAdjustmentDialog() {
                    // we need to clone the invoice so we are not affecting the saved invoice
                    // in previews
                    var clone = angular.extend(invoiceDisplayBuilder.createDefault(), scope.invoice);

                    var dialogData = {
                        payments: scope.payments,
                        allPayments: scope.allPayments,
                        paymentMethods: scope.paymentMethods,
                        currencySymbol: scope.currencySymbol,
                        invoice: clone
                    };
                    
                    dialogService.open({
                        template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/sales.manageadjustments.dialog.html',
                        show: true,
                        callback: manageAdjustmentsDialogConfirm,
                        dialogData: dialogData
                    });
                }

                function manageAdjustmentsDialogConfirm(adjustments) {
                    if (adjustments.items !== undefined) {

                    userService.getCurrentUser().then(function(user) {

                        _.each(adjustments.items, function(item) {
                            if (item.key === '') {
                                item.userName = user.name;
                                item.email = user.email;
                            }
                        });

                         invoiceResource.saveInvoiceAdjustments(adjustments).then(function() {
                         $timeout(function () {
                         scope.reload();
                         }, 400);
                         });
                    });
                }


                }

                init();
            }
        }
}]);
