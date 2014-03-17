(function (controllers, undefined) {
   
    /**
     * @ngdoc controller
     * @name Merchello.Editors.Order.ViewController
     * @function
     * 
     * @description
     * The controller for the order view page
     */
    controllers.OrderViewController = function ($scope, $routeParams, notificationsService, merchelloInvoiceService) {

        $scope.invoice = {};

        //--------------------------------------------------------------------------------------
        // Initialization methods
        //--------------------------------------------------------------------------------------

        $scope.loadInvoice = function (id) {

            var promise = merchelloInvoiceService.getByKey(id);

            promise.then(function (invoice) {

                $scope.invoice = new merchello.Models.Invoice(invoice);

                $scope.loaded = true;
                $scope.preValuesLoaded = true;

            }, function (reason) {
                notificationsService.error("Invoice Load Failed", reason.message);
            });
        };

        /**
         * @ngdoc method
         * @name init
         * @function
         * 
         * @description
         * Method called on intial page load.  Loads in data from server and sets up scope.
         */
        $scope.init = function () {

            $scope.loadInvoice($routeParams.id);

        };

        $scope.init();


        //--------------------------------------------------------------------------------------
        // Events methods
        //--------------------------------------------------------------------------------------


    };


    angular.module("umbraco").controller("Merchello.Editors.Order.ViewController", merchello.Controllers.OrderViewController);


}(window.merchello.Controllers = window.merchello.Controllers || {}));

