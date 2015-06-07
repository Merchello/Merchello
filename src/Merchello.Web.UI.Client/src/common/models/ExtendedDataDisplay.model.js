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
                return '';
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

        function setValue(key, value) {

            var existing = _.find(this.items, function(item) {
              return item.key === key;
            });
            if(existing) {
                existing.value = value;
                return;
            }

            this.items.push({ key: key, value: value });
        }

        function toArray() {
            return this.items;
        }

        return {
            isEmpty: isEmpty,
            getValue: getValue,
            setValue: setValue,
            toArray: toArray
        };
    }());

    angular.module('merchello.models').constant('ExtendedDataDisplay', ExtendedDataDisplay);