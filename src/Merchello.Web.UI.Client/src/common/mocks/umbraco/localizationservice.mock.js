angular.module('merchello.mocks').
    factory('localizationMocks', ['$httpBackend', 'mocksUtils', function ($httpBackend, mocksUtils) {
        'use strict';

        function getLanguageResource(status, data, headers) {
            //check for existence of a cookie so we can do login/logout in the belle app (ignore for tests).
                return [200, {
                    "update_updateDownloadText": "%0% is ready, click here for download"
                }, null];
        }

        return {
            register: function() {
                $httpBackend
                    .whenGET(mocksUtils.urlRegex('js/language.aspx'))
                    .respond(getLanguageResource);
            }
        };

    }]);
