angular.module('merchello.mocks')
    .factory('warehouseResourceMock', [
        '$httpBackend', 'mocksUtils',
        function($httpBackend, mocksUtils) {

            return {
                register: function() {

                }
            };
        }]);
