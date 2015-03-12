angular.module('merchello').controller('Merchello.Backoffice.MerchelloDashboardController',
    ['assetsService',
    function(assetsService) {

        assetsService.loadCss('/App_Plugins/Merchello/assets/css/merchello.css');

    }]);
