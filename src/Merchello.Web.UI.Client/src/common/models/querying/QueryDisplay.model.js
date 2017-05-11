    /**
     * @ngdoc model
     * @name QueryDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's QueryDisplay object
     *
     * @remark
     * PetaPoco Page<T> uses a 1 based page rather than a 0 based to represent the first page.
     * We do the conversion in the WebApiController - so the JS QueryDisplay should assume this is 0 based.
     */
    var QueryDisplay = function() {
        var self = this;
        self.currentPage = 0;
        self.itemsPerPage = 25;
        self.parameters = [];
        self.sortBy = '';
        self.sortDirection = 'Ascending'; // valid options are 'Ascending' and 'Descending'
    };

    QueryDisplay.prototype = (function() {
        // private
        function addParameter(queryParameter) {
            this.parameters.push(queryParameter);
        }

        function addCustomParam(fieldName, value) {
            var param = new QueryParameterDisplay();
            param.fieldName = fieldName;
            param.value = value;
            addParameter.call(this, param);
        }

        function addCustomerKeyParam(customerKey) {
            var param = new QueryParameterDisplay();
            param.fieldName = 'customerKey';
            param.value = customerKey;
            addParameter.call(this, param);
        }

        function addInvoiceDateParam(dateString, startOrEnd) {
            var param = new QueryParameterDisplay();
            param.fieldName = startOrEnd === 'start' ? 'invoiceDateStart' : 'invoiceDateEnd';
            param.value = dateString;
            addParameter.call(this, param);
        }

        function addCollectionKeyParam(collectionKey) {
            var param = new QueryParameterDisplay();
            param.fieldName = 'collectionKey';
            param.value = collectionKey;
            addParameter.call(this, param);
        }

        function hasCollectionKeyParam() {
            var fnd = _.find(this.parameters, function(p) {
                return p.fieldName === 'collectionKey';
            });
            return fnd !== undefined;
        }

        function addEntityTypeParam(entityType) {
            var param = new QueryParameterDisplay();
            param.fieldName = 'entityType';
            param.value = entityType;
            addParameter.call(this, param);
        }

        function addFilterTermParam(term) {
            if(term === undefined || term.length <= 0) {
                return;
            }
            var param = new QueryParameterDisplay();
            param.fieldName = 'term';
            param.value = term;
            addParameter.call(this, param);
        }
        
        function addSharedOptionOnlyParam(sharedOnly) {
            if (sharedOnly === undefined || sharedOnly === true) {
                return;
            }
            var param = new QueryParameterDisplay();
            param.fieldName = 'sharedOnly';
            param.value = 'false';
            addParameter.call(this, param);
        }

        function applyInvoiceQueryDefaults() {
            this.sortBy = 'invoiceNumber';
            this.sortDirection = 'Descending';
        }

        // public
        return {
            addParameter: addParameter,
            addCustomerKeyParam: addCustomerKeyParam,
            addCollectionKeyParam: addCollectionKeyParam,
            addCustomParam: addCustomParam,
            addEntityTypeParam: addEntityTypeParam,
            applyInvoiceQueryDefaults: applyInvoiceQueryDefaults,
            addInvoiceDateParam: addInvoiceDateParam,
            addFilterTermParam: addFilterTermParam,
            hasCollectionKeyParam: hasCollectionKeyParam,
            addSharedOptionOnlyParam: addSharedOptionOnlyParam
        };
    }());

    angular.module('merchello.models').constant('QueryDisplay', QueryDisplay);