(function () {
    angular.module('umbraco.filters', []);
    angular.module('umbraco.filters').filter('compareArrays', function () {
        return function inArray(array, compareArray, compareProperty) {
            var result = [];
            angular.forEach(array, function (arrayItem) {
                var exists = false;
                angular.forEach(compareArray, function (compareItem) {
                    if (arrayItem[compareProperty] === compareItem[compareProperty]) {
                        exists = true;
                    }
                });
                if (!exists) {
                    result.push(arrayItem);
                }
            });
            return result;
        };
    });
    // Filter to take a node id and grab it's name instead
    // Usage: {{ pickerAlias | ncNodeName }}
    // Cache for node names so we don't make a ton of requests
    var ncNodeNameCache = {
        id: '',
        keys: {}
    };
    angular.module('umbraco.filters').filter('ncNodeName', function (editorState, entityResource) {
        return function (input) {
            // Check we have a value at all
            if (input === '' || input.toString() === '0') {
                return '';
            }
            var currentNode = editorState.getCurrent();
            // Ensure a unique cache per editor instance
            var key = 'ncNodeName_' + currentNode.key;
            if (ncNodeNameCache.id !== key) {
                ncNodeNameCache.id = key;
                ncNodeNameCache.keys = {};
            }
            // See if there is a value in the cache and use that
            if (ncNodeNameCache.keys[input]) {
                return ncNodeNameCache.keys[input];
            }
            // No value, so go fetch one 
            // We'll put a temp value in the cache though so we don't 
            // make a load of requests while we wait for a response
            ncNodeNameCache.keys[input] = 'Loading...';
            entityResource.getById(input, 'Document').then(function (ent) {
                ncNodeNameCache.keys[input] = ent.name;
            });
            // Return the current value for now
            return ncNodeNameCache.keys[input];
        };
    });
    /**
* @ngdoc filter
* @name umbraco.filters.preserveNewLineInHtml
* @description 
* Used when rendering a string as HTML (i.e. with ng-bind-html) to convert line-breaks to <br /> tags
**/
    angular.module('umbraco.filters').filter('preserveNewLineInHtml', function () {
        return function (text) {
            if (!text) {
                return '';
            }
            return text.replace(/\n/g, '<br />');
        };
    });
    angular.module('umbraco.filters').filter('timespan', function () {
        return function (input) {
            var sec_num = parseInt(input, 10);
            var hours = Math.floor(sec_num / 3600);
            var minutes = Math.floor((sec_num - hours * 3600) / 60);
            var seconds = sec_num - hours * 3600 - minutes * 60;
            if (hours < 10) {
                hours = '0' + hours;
            }
            if (minutes < 10) {
                minutes = '0' + minutes;
            }
            if (seconds < 10) {
                seconds = '0' + seconds;
            }
            var time = hours + ':' + minutes + ':' + seconds;
            return time;
        };
    });
    /**
 * @ngdoc filter
 * @name umbraco.filters.filter:umbWordLimit
 * @namespace umbWordLimitFilter
 *
 * @description
 * Limits the number of words in a string to the passed in value
 */
    (function () {
        'use strict';
        function umbWordLimitFilter() {
            return function (collection, property) {
                if (!angular.isString(collection)) {
                    return collection;
                }
                if (angular.isUndefined(property)) {
                    return collection;
                }
                var newString = '';
                var array = [];
                array = collection.split(' ', property);
                array.length = property;
                newString = array.join(' ');
                return newString;
            };
        }
        angular.module('umbraco.filters').filter('umbWordLimit', umbWordLimitFilter);
    }());
}());