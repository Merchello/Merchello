'use strict';

(function () {

    function skuSelector($scope, assetsService) {
        assetsService
        .load([
            "/App_Plugins/Merchello/lib/select2.js",
            "/App_Plugins/Merchello/lib/ui-select2.js"
        ])
        .then(function () {
            //this function will execute when all dependencies have loaded
        });

        //load the seperat css for the editor to avoid it blocking our js loading
        assetsService.loadCss("/App_Plugins/Merchello/lib/select2.css");
    };

    angular.module("umbraco").controller('Merchello.PropertyEditors.MerchelloSkuSelector', skuSelector);

})();