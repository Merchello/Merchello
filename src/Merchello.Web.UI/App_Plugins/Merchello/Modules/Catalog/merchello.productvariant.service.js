(function (merchelloServices, undefined) {


    /**
        * @ngdoc service
        * @name umbraco.resources.MerchelloProductVariantService
        * @description Loads in data for data types
        **/
    merchelloServices.MerchelloProductVariantService = function ($q, $http, umbDataFormatter, umbRequestHelper) {

        return {

            create: function (productVariant) {

                return umbRequestHelper.resourcePromise(
                   $http.post(
                       '/umbraco/Merchello/ProductVariantApi/NewProductVariant',
                       productVariant
                   ),
                   'Failed to create product variant ' + productVariant.sku);
            },

            getById: function (id) {

                return umbRequestHelper.resourcePromise(
                   $http({
                       url: '/umbraco/Merchello/ProductVariantApi/GetProductVariant',
                       method: "GET",
                       params: { id: id }
                   }),
                   'Failed to retreive data for product variant id ' + id);
            },

            getByProduct: function (productkey) {

                return umbRequestHelper.resourcePromise(
                   $http({
                       url: '/umbraco/Merchello/ProductVariantApi/GetByProduct',
                       method: "GET",
                       params: { key: productkey }
                   }),
                   'Failed to retreive data for product key ' + productkey);
            },

            deleteVariant: function (key) {

                return umbRequestHelper.resourcePromise(
                   $http({
                       url: '/umbraco/Merchello/ProductVariantApi/' + key,
                       method: "DELETE"
                   }),
                   'Failed to delete variant data for key ' + key);
            },

            deleteAllByProduct: function (productkey) {

                return umbRequestHelper.resourcePromise(
                   $http({
                       url: '/umbraco/Merchello/ProductVariantApi/DeleteAllVariants',
                       method: "GET",
                       params: { productkey: productkey }
                   }),
                   'Failed to delete variants data for product key ' + productkey);
            },

            /** saves or updates a product variant object */
            save: function (productVariant) {

                return umbRequestHelper.resourcePromise(
                    $http.put(
                        '/umbraco/Merchello/ProductVariantApi/PutProductVariant',
                        productVariant
                    ),
                    'Failed to save data for product variant id ' + productVariant.id);
            },

        };
    }

    angular.module('umbraco.resources').factory('merchelloProductVariantService', merchello.Services.MerchelloProductVariantService);

}(window.merchello.Services = window.merchello.Services || {}));
