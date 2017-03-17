angular.module('merchello.services').service('merchDateHelper', [
    '$q', 'localizationService',
    function($q, localizationService) {

        this.convertToJsDate = function(dateString, dateFormat) {
            // date formats in merchello start with MM, dd, or yyyy
            if(dateString.indexOf('/') === -1) {
                dateString = dateString.replace(/-|\./g, '/');
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

            return date;
        },

        this.convertToIsoDate = function(dateString, dateFormat) {

            var date = this.convertToJsDate(dateString, dateFormat);
            return date.toISOString();
        };

        this.getLocalizedDaysOfWeek = function() {

            var deferred = $q.defer();
            $q.all([
                localizationService.localize('merchelloGeneral_sunday'),
                localizationService.localize('merchelloGeneral_monday'),
                localizationService.localize('merchelloGeneral_tuesday'),
                localizationService.localize('merchelloGeneral_wednesday'),
                localizationService.localize('merchelloGeneral_thursday'),
                localizationService.localize('merchelloGeneral_friday'),
                localizationService.localize('merchelloGeneral_saturday')
            ]).then(function(weekdays) {
                deferred.resolve(weekdays);
            });

            return deferred.promise;
        };

        this.getGmt0EquivalentDate = function(dt) {
           return new Date(dt.getTime() + (dt.getTimezoneOffset() * 60000));
        };
}]);
