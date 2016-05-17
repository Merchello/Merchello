/**
  * @ngdoc resource
  * @name noteResource
  * @description Loads in data and allows modification of notes
  **/
angular.module('merchello.resources').factory('noteResource', [
    '$http', 'umbRequestHelper',
    function ($http, umbRequestHelper) {
        return {
            /**
             * @ngdoc method
             * @name getSalesHistoryByInvoiceKey
             * @description
             **/
            getByEntityKey: function (key) {
                var url = Umbraco.Sys.ServerVariables["merchelloUrls"]["merchelloNoteApiBaseUrl"] + 'GetByEntityKey';
                return umbRequestHelper.resourcePromise(
                $http({
                    url: url,
                    method: "GET",
                    params: { id: key }
                }),
                'Failed to retrieve notes for entity with following key: ' + key);
            }

        };
    }]);