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
        self.active = false;
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
            return localDateString(this.offerStartsDate);
        }

        // gets the local end date string
        function offerEndsDateLocalDateString() {
            return localDateString(this.offerEndsDate);
        }

        function componentDefinitionExtendedDataToArray() {
            angular.forEach(this.componentDefinitions, function(cd) {
                cd.extendedData = cd.extendedData.toArray();
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
            var notConfigured = _.find(this.componentDefinitions, function(c) { return c.isConfigured() === false});
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

        function updateAssignedComponent(component) {
            console.info(component);
            var assigned = getAssignedComponent.call(this, component.componentKey);
            if (assigned !== undefined && assigned !== null) {
                assigned.extendedData = component.extendedData;
                assigned.updated = true;
            }
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
            updateAssignedComponent: updateAssignedComponent,
            getAssignedComponent: getAssignedComponent,
            componentsConfigured: componentsConfigured
        }

    }());

    angular.module('merchello.models').constant('OfferSettingsDisplay', OfferSettingsDisplay);