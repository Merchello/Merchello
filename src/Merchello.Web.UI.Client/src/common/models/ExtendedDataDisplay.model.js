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
            if (this.items) {
                return this.items.length === 0;
            }
            return true;
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

            // See if items is null and if so, make it an array
            if (!this.items) {
                this.items = [];
            }

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
        
        function asDetachedValueArray() {
            var values = [];
            angular.forEach(this.items, function(dcv) {

                // ensure there are not null values
                if (dcv.value === null) {
                    dcv.value = '';
                }

                // ensure property did not set an array
                if (angular.isArray(dcv.value) && dcv.value.length === 0) {
                    dcv.value = '';
                }
                values.push(dcv);
            });
            
            return values;
        }

        return {
            isEmpty: isEmpty,
            getValue: getValue,
            setValue: setValue,
            toArray: toArray,
            asDetachedValueArray: asDetachedValueArray
        };
    }());

    angular.module('merchello.models').constant('ExtendedDataDisplay', ExtendedDataDisplay);