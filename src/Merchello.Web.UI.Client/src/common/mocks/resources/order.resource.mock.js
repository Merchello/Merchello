angular.module('merchello.mocks')
    .factory('orderResourceMocks', [
        '$httpBackend', 'mocksUtils', 'addressMocks',
        function($httpBackend, mocksUtils, addressMocks) {

            function getShippingAddress() {
                return new addressMocks.getRandomAddress();
            }

            return {
                register : function() {
                    $httpBackend
                        .whenPOST(mocksUtils.urlRegex('/umbraco/backoffice/Merchello/OrderApi/getShippingAddress'))
                        .respond(getShippingAddress());
                }
            };
    }]);
