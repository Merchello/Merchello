angular.module('merchello').controller('MerchelloDashboardController',
    ['assetsService',
    function(assetsService) {

        assetsService.loadCss('/App_Plugins/Merchello/assets/merchello.css');

    }]);
