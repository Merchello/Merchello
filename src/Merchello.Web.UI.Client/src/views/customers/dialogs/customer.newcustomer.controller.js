angular.module('merchello').controller('Merchello.Customer.Dialogs.CustomerNewCustomerController',
    ['$scope', '$location', 'dialogDataFactory', 'customerResource', 'notificationsService', 'navigationService', 'customerDisplayBuilder',
        function($scope, $location, dialogDataFactory, customerResource, notificationsService, navigationService, customerDisplayBuilder) {
            $scope.wasFormSubmitted = false;

            $scope.firstName = '';
            $scope.lastName = '';
            $scope.email = '';

            // exposed methods
            $scope.save = save;

            /**
             * @ngdoc method
             * @name submitIfValid
             * @function
             *
             * @description
             * Submit form if valid.
             */
            function save() {
                $scope.wasFormSubmitted = true;
                if ($scope.editInfoForm.email.$valid) {
                    var customer = customerDisplayBuilder.createDefault();
                    customer.loginName = $scope.email;
                    customer.email = $scope.email;
                    customer.firstName = $scope.firstName;
                    customer.lastName = $scope.lastName;

                    var promiseSaveCustomer = customerResource.AddCustomer(customer);
                    promiseSaveCustomer.then(function (customerResponse) {
                        notificationsService.success("Customer Saved", "");
                        navigationService.hideNavigation();
                        $location.url("/merchello/merchello/customeroverview/" + customerResponse.key, true);
                    }, function (reason) {
                        notificationsService.error("Customer Save Failed", reason.message);
                    });
                }
            }
        }]);
