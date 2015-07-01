angular.module('merchello.services').service('dateHelper', [function() {

    this.convertToIsoDate = function(dateString, dateFormat) {
        // date formats in merchello start with MM, dd, or yyyy
        if(dateString.indexOf('/') === -1) {
            dateString = dateString.replace(/-/g, '/');
        }
        var splitDate = dateString.split('/');
        var date;
        switch (dateFormat) {
            case 'MM-dd-yyyy':
                splitDate[0] = (splitDate[0] * 1) - 1;
                date = new Date(splitDate[2], splitDate[0], splitDate[1], 0, 0, 0);
                break;
            case 'dd-MM-yyyy':
                splitDate[1] = (splitDate[1] * 1) - 1;
                date = new Date(splitDate[2], splitDate[1], splitDate[0], 0, 0, 0);
                break;
            default:
                splitDate[1] = (splitDate[1] * 1) - 1;
                date = new Date(splitDate[0], splitDate[1], splitDate[2], 0, 0, 0);
                break;
        }
        console.info(date);
        return date.toISOString();
    }

}]);
