angular.module('merchello.mocks').
    factory('dialogDataMocks', [
        function () {
            'use strict';

            // Private
            function getAddressDialogData(addressMocks) {
                var dialogData = {};
                dialogData.address = addressMocks.getSingleAddress();
                return dialogData;
            }

            // Public
            return {
                getAddressDialogData : getAddressDialogData
            };
        }]);
