    /**
     * @ngdoc model
     * @name ExtendedDataDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's ExtendedDataDisplay object
     */
    var ExtendedDataDisplay = function() {
        var self = this;
        self.items = [];
    };

    ExtendedDataDisplay.prototype = (function() {

        function isEmpty() {
            return this.items.length === 0;
        }

        function getValue(key) {
            if (isEmpty.call(this)) {
                return;
            }
            var found = false;
            var i = 0;
            var value = '';
            while(i < this.items.length && !found) {
                if (this.items[ i ].key === key) {
                    found = true;

                    value = this.items[ i ].value;
                } else {
                    i++;
                }
            }
            return value;
        }

        /*function setValue(key, value) {
            var found = false;
            var i = 0;
            while(i < this.items.length && !found) {
                if (this[ i ].key === key) {
                    found = true;
                    this[ i ].value = value;
                }
                i++;
            }
            if (found) {
                return;
            }
            this.push({ key: key, value: value });
        }*/


        return {
            isEmpty: isEmpty,
            getValue: getValue
            //setValue: setValue
        };
    }());

    angular.module('merchello.models').constant('ExtendedDataDisplay', ExtendedDataDisplay);