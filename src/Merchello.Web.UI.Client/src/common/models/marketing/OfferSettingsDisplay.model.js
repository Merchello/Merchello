    /**
     * @ngdoc model
     * @name OfferSettingsDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's OfferSettingsDisplay object
     */
    var OfferSettingsDisplay = function() {
        var self = this;
        self.key = '';
        self.name = '';
        self.offerCode = '';
        self.offerProviderKey = '';
        self.offerExpires = false;
        self.offerStartsDate = '';
        self.offerEndsDate = '';
        self.expired = false;
        self.hasStarted = false;
        self.active = false;
        self.dateFormat = '';  // used to pass back office format to server for parse exact.
        self.componentDefinitions = [];
    };

    OfferSettingsDisplay.prototype = (function() {

        function clone() {
            return angular.extend(new OfferSettingsDisplay(), this);
        }

        // private methods
        function getAssignedComponent(componentKey) {
            return _.find(this.componentDefinitions, function (cd) { return cd.componentKey === componentKey; });
        }

        // adjusts date with timezone
        function localDateString(val) {
            var raw = new Date(val);
            return new Date(raw.getTime() + raw.getTimezoneOffset()*60000).toLocaleDateString();
        }

        // gets the local start date string
        function offerStartsDateLocalDateString() {
            //return this.offerStartsDate;
            return localDateString(this.offerStartsDate);
        }

        // gets the local end date string
        function offerEndsDateLocalDateString() {
            //return this.offerEndsDate;
            return localDateString(this.offerEndsDate);
        }


        function componentDefinitionExtendedDataToArray() {
            angular.forEach(this.componentDefinitions, function(cd) {
                if (!angular.isArray(cd.extendedData)) {
                    cd.extendedData = cd.extendedData.toArray();
                }
            });
        }

        // returns true if the offer has rewards assigned
        function hasRewards() {
            if(!hasComponents.call(this)) {
                return false;
            }
            var reward = _.find(this.componentDefinitions, function(c) { return c.componentType === 'Reward'; } );
            return reward !== undefined && reward !== null;
        }

        function getReward() {
            return _.find(this.componentDefinitions, function(c) { return c.componentType === 'Reward'; } );
        }

        function componentsConfigured() {
            if (!hasComponents.call(this)) {
                return true;
            }
            var notConfigured = _.find(this.componentDefinitions, function(c) { return c.isConfigured() === false; });
            return notConfigured === undefined;
        }

        function hasComponents() {
            return this.componentDefinitions.length > 0;
        }

        // gets the current component type grouping
        function getComponentsTypeGrouping() {
            return hasComponents.call(this) ? this.componentDefinitions[0].typeGrouping : '';
        }

        // ensures that all components work with the same type of objects.
        function ensureTypeGrouping(typeGrouping) {
            if(!hasComponents.call(this)) {
                return true;
            }
            return this.componentDefinitions[0].typeGrouping === typeGrouping;
        }

        function assignComponent(component) {
            var exists =_.find(this.componentDefinitions, function(cd) { return cd.componentKey === component.componentKey; });
            if (exists === undefined && ensureTypeGrouping.call(this, component.typeGrouping)) {
                component.offerCode = this.offerCode;
                component.offerSettingsKey = this.key;
                if (component.componentType === 'Reward') {
                    this.componentDefinitions.unshift(component);
                }
                else
                {
                    this.componentDefinitions.push(component);
                }

                return true;
            }
            return false;
        }

        function updateAssignedComponent(component) {
            var assigned = getAssignedComponent.call(this, component.componentKey);
            if (assigned !== undefined && assigned !== null) {
                assigned.extendedData = component.extendedData;
                assigned.updated = true;
            }
        }

        function setLineItemName(value) {
            if (hasRewards.call(this)) {
                var reward = getReward.call(this);
                reward.extendedData.setValue('lineItemName', value);
            }
        }

        function getLineItemName() {
            if(hasRewards.call(this)) {
                var reward = getReward.call(this);
                var name = reward.extendedData.getValue('lineItemName');
                if (name === '') {
                    name = reward.name;
                }
                return name;
            } else {
                return '';
            }
        }

        function validateComponents() {
            var offerCode = this.offerCode;
            var offerSettingsKey = this.key;
            var invalid = _.filter(this.componentDefinitions, function (cd) { return cd.offerSettingsKey !== this.key || cd.offerCode !== this.offerCode; });
            if (invalid !== undefined) {
                angular.forEach(invalid, function(fix) {
                    fix.offerSettingsKey = offerSettingsKey;
                    fix.offerCode = offerCode;
                });
            }
        }

        function reorderComponent(oldIndex, newIndex) {
            this.componentDefinitions.splice(newIndex, 0, this.componentDefinitions.splice(oldIndex, 1)[0]);
        }

        return {
            clone: clone,
            offerStartsDateLocalDateString: offerStartsDateLocalDateString,
            offerEndsDateLocalDateString: offerEndsDateLocalDateString,
            componentDefinitionExtendedDataToArray: componentDefinitionExtendedDataToArray,
            hasComponents: hasComponents,
            getComponentsTypeGrouping: getComponentsTypeGrouping,
            ensureTypeGrouping: ensureTypeGrouping,
            hasRewards: hasRewards,
            getReward: getReward,
            assignComponent: assignComponent,
            updateAssignedComponent: updateAssignedComponent,
            getAssignedComponent: getAssignedComponent,
            componentsConfigured: componentsConfigured,
            getLineItemName: getLineItemName,
            setLineItemName: setLineItemName,
            validateComponents: validateComponents,
            reorderComponent: reorderComponent
        };

    }());

    angular.module('merchello.models').constant('OfferSettingsDisplay', OfferSettingsDisplay);