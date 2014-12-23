'use strict';

(function () {

    function skuSelector($scope/*, assetsService*/) {
		$scope.assetsloaded = false;
	
        // assetsService
        // .load([
            // "/App_Plugins/Merchello/lib/select2.js",
            // "/App_Plugins/Merchello/lib/ui-select2.js"
        // ])
        // .then(function () {
            //this function will execute when all dependencies have loaded
			$scope.assetsloaded = true;
        // });

        //load the seperat css for the editor to avoid it blocking our js loading
        // assetsService.loadCss("/App_Plugins/Merchello/lib/select2.css");
    };

    angular.module("umbraco").controller('MerchelloSkuSelector', skuSelector);

})();