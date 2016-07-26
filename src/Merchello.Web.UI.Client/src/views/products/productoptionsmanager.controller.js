angular.module('merchello').controller('Merchello.Backoffice.ProductOptionsManagerController', [
    '$scope', '$q', '$routeParams', '$location', '$timeout', 'notificationsService', 'dialogService',
        'merchelloTabsFactory', 'productResource', 'eventsService', 'settingsResource',
        'productOptionDisplayBuilder', 'productDisplayBuilder', 'queryResultDisplayBuilder',
    function($scope, $q, $routeParams, $location, $timeout, notificationsService, dialogService,
             merchelloTabsFactory, productResource, eventsService, settingsResource,
             productOptionDisplayBuilder, productDisplayBuilder, queryResultDisplayBuilder) {

        $scope.product = {};
        $scope.preValuesLoaded = false;

        $scope.save = save;
        $scope.deleteProductDialog = deleteProductDialog;

        var onAdd = 'merchelloProductOptionOnAddOpen';

        $scope.load = function(query) {

            var deferred = $q.defer();

            var hasOptions = $scope.product.productOptions.length > 0;
            var itemsPerPage = hasOptions ?
                $scope.product.productOptions.length : query.itemsPerPage;

            var result = queryResultDisplayBuilder.createDefault();
            result.currentPage = 1;
            result.itemsPerPage = itemsPerPage;
            result.totalPages = hasOptions ? 1 : 0;
            result.totalItems = $scope.product.productOptions.length;
            result.items = [];

            _.each($scope.product.productOptions, function(po) {
                result.items.push(po);
            });

            deferred.resolve(result);

            return deferred.promise;

        }

        $scope.doEdit = function(option) {
            executeReload(function() {
                _.extend(_.findWhere($scope.product.options, { key: option.key }), option);
            });

        }

        $scope.doDelete = function(option) {
            executeReload(function() {
                $scope.product.removeOption(option);
            });
        }


        $scope.doAdd = function(option) {
            option.sortOrder = $scope.product.productOptions.length + 1;

            executeReload(function() {
                $scope.product.productOptions.push(option);
            });
        }

        function init() {

            eventsService.on(onAdd, function(name, args) {
                args.productKey = $scope.product.key;
            });

            var key = $routeParams.id;
            $q.all([
                settingsResource.getCurrencySymbol(),
                loadProduct(key)
            ]).then(function(data) {
                $scope.currencySymbol = data[0];
                $scope.product = data[1];
                setTabs();
            });
        }

        function setTabs() {
            $scope.tabs = merchelloTabsFactory.createProductEditorTabs($scope.product.key, $scope.product.hasVariants());
            $scope.tabs.hideTab('productcontent');
            $scope.tabs.setActive('optionslist');
            $scope.preValuesLoaded = true;
        }

        function loadProduct(key) {
            var deferred = $q.defer();
            productResource.getByKey(key).then(function(prod) {
                var p = productDisplayBuilder.transform(prod);
                deferred.resolve(p);
            });

            return deferred.promise;
        }

        /**
         * @ngdoc method
         * @name save
         * @function
         *
         * @description
         * Saves the product - used for changing the master variant name
         */
        function save(thisForm) {
            // TODO we should unbind the return click event
            // so that we can quickly add the options and remove the following
            if(thisForm === undefined) {
                return;
            }
            if (thisForm.$valid) {
                notificationsService.info("Saving Product...", "");

                $scope.product.productOptions = _.sortBy($scope.product.productOptions, function (po) {
                    return po.sortOrder;
                });

                $scope.preValuesLoaded = false;
                productResource.save($scope.product).then(function (product) {
                    notificationsService.success("Product Saved", "");
                    $scope.product = productDisplayBuilder.transform(product);
                    setTabs();
                }, function (reason) {
                    notificationsService.error("Product Save Failed", reason.message);
                });
            }
        }

        function executeReload(callback) {
            $scope.preValuesLoaded = false;
            // we need a timeout here so that the directive has time to catch the pre value toggle
            $timeout(function() {
                callback();
                $scope.preValuesLoaded = true;
            }, 400);
        }

        /**
         * @ngdoc method
         * @name deleteProductDialog
         * @function
         *
         * @description
         * Opens the delete confirmation dialog via the Umbraco dialogService.
         */
        function deleteProductDialog() {
            var dialogData = dialogDataFactory.createDeleteProductDialogData();
            dialogData.product = $scope.product;
            dialogData.name = $scope.product.name + ' (' + $scope.product.sku + ')';
            dialogData.warning = 'This action cannot be reversed.';

            dialogService.open({
                template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/delete.confirmation.html',
                show: true,
                callback: deleteProductDialogConfirmation,
                dialogData: dialogData
            });
        }

        /**
         * @ngdoc method
         * @name deleteProductDialogConfirmation
         * @function
         *
         * @description
         * Called when the Delete Product button is pressed.
         */
        function deleteProductDialogConfirmation() {
            var promiseDel = productResource.deleteProduct($scope.product);
            promiseDel.then(function () {
                notificationsService.success("Product Deleted", "");
                $location.url("/merchello/merchello/productlist/manage", true);
            }, function (reason) {
                notificationsService.error("Product Deletion Failed", reason.message);
            });
        }


        init();

    }]);
