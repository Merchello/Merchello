angular.module('merchello.directives').directive('productRecentUpdatesButton',
    ['productResource', 'queryDisplayBuilder', 'queryResultDisplayBuilder', 'productDisplayBuilder',
    function(productResource, queryDisplayBuilder, queryResultDisplayBuilder, productDisplayBuilder) {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            product: '=',
            parentForm: '=',
            classes: '='
        },
        templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/Directives/product.recentupdatebuttonlist.tpl.html',
        link: function(scope, elm, attr) {

            scope.visible = false;
            scope.products = [];

            scope.getLink = function(product) {
                return '<a href="' + getEditUrl(product) + '">' + product.name + '</a>'
            }


            function init() {
                var query = queryDisplayBuilder.createDefault();
                query.currentPage = 0;
                query.itemsPerPage = 10;
                productResource.getRecentlyUpdated(query).then(function(data) {
                   var results = queryResultDisplayBuilder.transform(data, productDisplayBuilder);
                   if (results.totalItems > 0) {
                       scope.products = results.items;
                       scope.visible = true;
                   }
                });
            }

            function getEditUrl(product) {
                return "#/merchello/merchello/productedit/" + product.key;
            }

            init();
        }
    }
}]);
