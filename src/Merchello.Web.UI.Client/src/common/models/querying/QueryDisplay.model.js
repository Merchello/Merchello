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

        function addCustomerKeyParam(customerKey) {
            var param = new QueryParameterDisplay();
            param.fieldName = 'customerKey';
            param.value = customerKey;
            addParameter(param);
        }

        function addInvoiceDateParam(dateString, startOrEnd) {
            var param = new QueryParameterDisplay();
            param.fieldName = startOrEnd === 'start' ? 'invoiceDateStart' : 'invoiceEndDate';
            param.value = dateString;
            addParameter(param);
        }

        function applyInvoiceQueryDefaults() {
            this.sortBy = 'invoiceNumber';
            this.sortDirection = 'Descending';
        }

        // public
        return {
            addParameter: addParameter,
            addCustomerKeyParam: addCustomerKeyParam,
            applyInvoiceQueryDefaults: applyInvoiceQueryDefaults
        };
    }());

    angular.module('merchello.models').constant('QueryDisplay', QueryDisplay);