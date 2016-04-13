angular.module('merchello.directives').directive('manageInvoiceAdjustments',
    ['dialogService', 'invoiceDisplayBuilder', 'invoiceLineItemDisplayBuilder',
    function(dialogService, invoiceDisplayBuilder, invoiceLineItemDisplayBuilder) {

        return {
            restrict: 'E',
            replace: true,
            scope: {
                invoice: '=',
                preValuesLoaded: '=',
                currencySymbol: '=',
                save: '&'
            },
            templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/directives/manageinvoiceadjustments.tpl.html',
            link: function (scope, elm, attr) {

                scope.openAdjustmentDialog = openAdjustmentDialog;
                scope.loaded = false;
                
                function init() {

                    // ensure that the parent scope promises have been resolved
                    scope.$watch('preValuesLoaded', function(pvl) {
                        if(pvl === true) {
                            
                        }
                    });
                }


                function openAdjustmentDialog() {
                    var adjustments = scope.invoice.getAdjustmentLineItems();
                    if (adjustments === undefined || adjustments === null) {
                        adjustments = [];
                    }
                    var dialogData = {
                        currencySymbol: scope.currencySymbol,
                        invoiceKey: scope.invoice.key,
                        invoiceNumber: scope.invoice.prefixedInvoiceNumber(),
                        adjustments: adjustments
                    };
                    
                    dialogService.open({
                        template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/sales.manageadjustments.dialog.html',
                        show: true,
                        callback: manageAdjustmentsDialogConfirm,
                        dialogData: dialogData
                    });
                }

                function manageAdjustmentsDialogConfirm(dialogData) {
                    console.info(dialogData);
                }

                init();
            }
        }
}]);
