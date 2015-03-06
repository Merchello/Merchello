(function () {
    angular.module('merchello.plugins.purchaseorder',
        [
            'merchello.models'
        ]);

    angular.module('merchello.plugins').requires.push('merchello.plugins.purchaseorder');
}());