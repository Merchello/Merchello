angular.module('merchello.directives').directive('uniqueOfferCode', function() {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            offer: '=',
            offerCode: '=',
            offerForm: '='
        },
        templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/Directives/offer.uniqueoffercode.tpl.html',
        controller: function($scope, eventsService, marketingResource) {

            $scope.loaded = false;
            $scope.checking = false;
            $scope.isUnique = true;

            var eventOfferSavingName = 'merchello.offercoupon.saving';
            var input = angular.element( document.querySelector( '#offerCode' ) );
            var container = angular.element( document.querySelector("#unique-offer-check") );

            var currentCode = '';

            function init() {
                container.hide();
                eventsService.on(eventOfferSavingName, onOfferSaving)
                input.bind("keyup keypress", function (event) {
                    var code = event.which;
                    // alpha , numbers, ! and backspace

                    if ((code >47 && code <58) || (code >64 && code <91) || (code >96 && code <123) || code === 33 || code == 8) {
                        $scope.$apply(function () {
                            if ($scope.offerCode !== '') {
                                checkUniqueOfferCode($scope.offerCode);
                            }
                        });
                    } else {
                        $scope.checking = true;
                        event.preventDefault();
                    }
                });
                $scope.$watch('offerCode', function(oc) {
                    if($scope.offerCode !== undefined) {
                        if (!$scope.loaded) {
                            $scope.loaded = true;
                            currentCode = $scope.offerCode;
                            checkUniqueOfferCode($scope.offerCode);
                        }
                    }
                });
            }
            function checkUniqueOfferCode(offerCode) {
                $scope.checking = true;
                container.show();
                if (offerCode === currentCode) {
                    $scope.checking = false;
                    return true;
                }
                var checkPromise = marketingResource.checkOfferCodeIsUnique(offerCode);
                checkPromise.then(function(result) {
                    $scope.checking = false;
                    $scope.isUnique = result;

                });
            }

            function onOfferSaving(e, frm) {
                frm.offerCode.$setValidity('offerCode', checkUniqueOfferCode($scope.offerCode));
            }
            // Initialize
            init();
        }
    };
});
