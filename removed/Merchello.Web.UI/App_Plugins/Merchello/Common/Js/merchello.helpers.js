(function (models, undefined) {


    models.StringHelpers = function () {

        var self = this;

        self.isNullOrEmpty = function (stringToCheck) {
            if (!stringToCheck) {
                return true;
            }

            return false;
        };
    };

    models.Strings = new merchello.Helpers.StringHelpers();

}(window.merchello.Helpers = window.merchello.Helpers || {}));