angular.module('merchello.mocks').
    factory('addressMocks', [
        function () {
            'use strict';

            function getSingleAddress() {
                var address = new Merchello.Models.Address();
                address.name = 'Disney World';
                address.address1 = 'Walt Disney World Resort';
                address.locality = 'Lake Buena Vista';
                address.region = 'FL';
                address.postalCode = '32830';
                address.countryCode = 'US';
                return address;
            }


            return {
                getSingleAddress : getSingleAddress
            };

        }]);