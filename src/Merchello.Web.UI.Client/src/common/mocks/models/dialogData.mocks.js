angular.module('merchello.mocks').
    factory('dialogDataMocks', [
        function () {
            'use strict';

            // Private
            function getAddressDialogData(addressMocks) {
                var dialogData = {};
                var address = addressMocks.getSingleAddress();
                dialogData.address = address;
                return dialogData;
            }

            // Public
            return {
                getAddressDialogData : getAddressDialogData
            };
        }]);
