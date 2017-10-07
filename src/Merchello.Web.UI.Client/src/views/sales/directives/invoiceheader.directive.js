angular.module('merchello.directives').directive('invoiceHeader',
    ['$timeout', 'dialogService', 'invoiceResource', 'invoiceDisplayBuilder',
     function($timeout, dialogService, invoiceResource, invoiceDisplayBuilder) {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            invoice: '=',
            refresh: '&'
        },
        templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/directives/invoiceheader.tpl.html',
        link: function(scope, elm, attr) {
            
            scope.loaded = false;
            
            scope.openHeaderEdit = function() {

                var clone = invoiceDisplayBuilder.transform(scope.invoice);

                var dialogData = {
                    invoice: clone
                };

                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/edit.invoiceheader.html',
                    show: true,
                    callback: saveInvoice,
                    dialogData: dialogData
                });
            }

            function saveInvoice(dialogData) {
                invoiceResource.saveInvoice(dialogData.invoice);
                scope.invoice.poNumber = dialogData.invoice.poNumber;
                scope.invoice.invoiceNumberPrefix = dialogData.invoice.invoiceNumberPrefix;
                //scope.refresh();
            }
            
            function init() {
                scope.$watch('invoice', function(inv) {
                    if (inv.key !== null && inv.key !== undefined) {
                        scope.loaded = true;
                    }
                });
            }


            init();
        }
    }
}])
