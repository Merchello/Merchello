angular.module('merchello.mocks').
    factory('addressMocks', ['AddressDisplay',
        function () {
            'use strict';

            var Constructor = AddressDisplay;

            function getConstructor()
            {
                return Constructor;
            }

            function getSingleAddress() {
                var address = new Constructor();
                address.name = 'Disney World';
                address.address1 = 'Walt Disney World Resort';
                address.locality = 'Lake Buena Vista';
                address.region = 'FL';
                address.postalCode = '32830';
                address.countryCode = 'US';
                return address;
            }

            function getBadAddressResult()
            {
                return {
                    name : 'Mindfly',
                    address1: '114 W. Magnolia St.',
                    address2: 'Suite 300',
                    locality: 'Bellingham',
                    region : 'WA',
                    postalCode:  '98225',
                    countryCode: 'US',

                    // bad data
                    property1: 'should not be here',
                    property2: 'should not be here'
                };
            }

            function getRandomAddress(genericModelBuilder)
            {
                var addresses = getAddressArray();
                var index = Math.floor(Math.random() * addresses.length);

                if (genericModelBuilder === undefined) {
                    return addresses[index];
                }
                return genericModelBuilder.transform(addresses[index], Constructor);
            }

            function getAddressArray(genericModelBuilder) {

                var addresses = [
                    {
                        name : 'Mindfly',
                        address1: '114 W. Magnolia St.',
                        address2: 'Suite 300',
                        locality: 'Bellingham',
                        region : 'WA',
                        postalCode:  '98225',
                        countryCode: 'US'
                    },
                    {
                        name : 'Rockefeller Center',
                        address1: '45 Rockefeller Plz',
                        locality: 'New York',
                        region : 'NY',
                        postalCode:  '10111',
                        countryCode: 'US'
                    },
                    {
                        name : 'Eiffel Tower',
                        address1: 'Champs-de-Mars',
                        locality: 'Paris',
                        postalCode:  '75007',
                        countryCode: 'FR'
                    },
                    {
                        name : 'Buckingham Palace',
                        address1: 'SW1A 1AA',
                        locality: 'London',
                        countryCode: 'UK'
                    },
                    {
                        name : 'Space Needle',
                        address1: '400 Broad St',
                        locality: 'Seattle',
                        region : 'WA',
                        postalCode:  '98102',
                        countryCode: 'US'
                    },
                    {
                        name : 'Sydney Opera House',
                        address1: 'Bennelong Point',
                        locality: 'Sydney',
                        postalCode:  'NSW 2000',
                        countryCode: 'AU'
                    }
                ];

                if (genericModelBuilder === undefined) {
                    return addresses;
                }

                return genericModelBuilder.transform(addresses, Constructor);
            }

            return {
                getConstructor: getConstructor,
                getSingleAddress : getSingleAddress,
                getBadAddressResult: getBadAddressResult,
                getAddressArray: getAddressArray,
                getRandomAddress: getRandomAddress
            };

        }]);