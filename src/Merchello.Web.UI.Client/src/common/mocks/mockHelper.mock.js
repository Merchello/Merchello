angular.module('merchello.mocks')
    .factory('mockHelper', [function() {

        function downCaseProperties(object) {
            var newObject = {};
            for (var prop in object) {
                if (object.hasOwnProperty(prop)) {
                    var propertyName = prop;
                    var propertyValue = object[prop];
                    var newPropertyName = propertyName.charAt(0).toLowerCase() + propertyName.slice(1);
                    if ((typeof propertyValue) === "object") {
                        propertyValue = downCaseProperties(propertyValue);
                    }
                    newObject[newPropertyName] = propertyValue;
                }
            }
            return newObject;
        }

        function downCasePropertiesInObjectArray(objArray) {
            var formatted = [];
            angular.forEach(objArray, function(obj) {
              formatted.push(downCaseProperties(obj));
            });
            return formatted;
        }

        return {
            downCaseProperties: downCaseProperties,
            downCasePropertiesInObjectArray: downCasePropertiesInObjectArray
        };
    }]);