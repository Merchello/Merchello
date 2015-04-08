/**
 * @ngdoc directive
 * @name merchello-slide-open-panel
 * @function
 *
 * @description
 * Directive to allow a section of content to slide open/closed based on a boolean value
 */
angular.module('merchello.directives').directive('addPaymentTable', function() {
    return {
        restrict: 'E',
        replace: true,
        transclude: 'true',
        scope: {
            isOpen: '=',
            currencySymbol: '=',
            reload: '&',
            toggleOpen: '&',
            invoice: '=',
            payments: '=',
            paymentMethods: '='
        },
        templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/directives/addpaymenttable.tpl.html',
        controller: function($scope, $timeout, notificationsService, dialogService, dialogDataFactory, paymentResource) {
            $scope.loaded = false;
            $scope.authorizePaymentOnly = false;

            // exposed methods
            $scope.openAddPaymentDialog = openAddPaymentDialog;
            $scope.filterPaymentMethods = filterPaymentMethods;

            /**
             * @ngdoc method
             * @name init
             * @function
             *
             * @description - Initializes the controller
             */
            function init() {
                $scope.$watch('paymentMethods', function(methods) {
                  if (methods.length > 0) {
                      $scope.loaded = true;
                  }
                });
            }

            /**
             * @ngdoc method
             * @name filterPaymentMethods
             * @function
             *
             * @description - Filters payment methods for methods that offer authorize / authorize capture dialogs
             */
            function filterPaymentMethods() {
                if ($scope.authorizePaymentOnly) {
                    return _.filter($scope.paymentMethods, function(auth) { return auth.authorizePaymentEditorView.editorView !== ''; });
                } else {
                    return _.filter($scope.paymentMethods, function(capture) { return capture.authorizeCapturePaymentEditorView.editorView !== ''; });
                }
            }

            /**
             * @ngdoc method
             * @name openAddPaymentDialog
             * @function
             *
             * @description - Opens a dialog to authorize and/or capture a new payment
             */
            function openAddPaymentDialog(paymentMethod) {

                var dialogData = dialogDataFactory.createAddPaymentDialogData();
                dialogData.paymentMethod = paymentMethod;
                dialogData.paymentMethodName = paymentMethod.name;
                dialogData.invoiceBalance = $scope.invoice.remainingBalance($scope.payments);
                dialogData.currencySymbol = $scope.currencySymbol;
                dialogData.invoice = $scope.invoice;
                dialogData.authorizePaymentOnly = $scope.authorizePaymentOnly;
                var dialogView = $scope.authorizePaymentOnly ? paymentMethod.authorizePaymentEditorView.editorView : paymentMethod.authorizeCapturePaymentEditorView.editorView;

                dialogService.open({
                    template: dialogView,
                    show: true,
                    callback: addPaymentDialogConfirm,
                    dialogData: dialogData
                });
            }

            /**
             * @ngdoc method
             * @name addPaymentDialogConfirm
             * @function
             *
             * @description - Authorizes and/or captures a new payment
             */
            function addPaymentDialogConfirm(dialogData) {
                $scope.preValuesLoaded = false;
                var paymentRequest = dialogData.asPaymentRequestDisplay();
                var promise;
                var note =  ' Authorize/Capture ';
                if (dialogData.authorizePaymentOnly) {
                    promise = paymentResource.authorizePayment(paymentRequest);
                    name = ' Authorize ';
                } else {
                    promise = paymentResource.authorizeCapturePayment(paymentRequest);
                }
                promise.then(function (payment) {
                    // added a timeout here to give the examine index
                    $timeout(function() {
                        notificationsService.success('Payment ' + note + 'success');
                        reload()
                    }, 400);
                }, function (reason) {
                    notificationsService.error('Payment ' + note + 'Failed', reason.message);
                });
            }

            function reload() {
                $scope.toggleOpen();
                $scope.reload();
            }

            // initialize the controller
            init();
        }
    };
});
