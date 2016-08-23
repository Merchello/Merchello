angular.module('merchello.services').service('entityCollectionHelper',
    [
        function() {

            this.getEntityTypeByTreeId = function(id) {
                switch(id) {
                    case "products":
                        return "Product";
                    case "sales":
                        return "Invoice";
                    case "customers":
                        return "Customer";
                    default :
                        return "EntityCollection";
                }
            };
        }]);


