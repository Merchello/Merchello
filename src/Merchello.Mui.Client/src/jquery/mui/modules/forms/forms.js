MUI.Forms = {
    init: function() {

        // add the custom validator for MVC unobtrusive validation
        $.validator.addMethod('validateexpiresdate', function(value, element, params) {

            // Make sure the value has a length of 5
            if (value.length != 5) {
                return false;
            }

            var today = new Date();
            var thisYear = today.getFullYear() - 2000;
            var expMonth = +value.substr(0, 2);
            var expYear = +value.substr(3, 4);

            return (expMonth >= 1
            && expMonth <= 12
            && (expYear >= thisYear && expYear < thisYear + 20)
            && (expYear == thisYear ? expMonth >= (today.getMonth() + 1) : true));

        });

        $.validator.unobtrusive.adapters.addBool('validateexpiresdate');
        
    },

    // Post a form and return the promise
    post: function(frm, url) {
        return MUI.resourcePromise(url, $(frm).serialize());
    },
    
    // rebinds a form unobtrusive validation
    rebind: function(frm) {
        $.validator.unobtrusive.parse(frm);
    },
    
    // validates the form
    validate: function(frm) {

        // obtain validator
        var validator = $(frm).validate();

        var isValid = true;
        $(frm).find("input").each(function () {

            // validate every input element inside this step
            if (!validator.element(this)) {
                isValid = false;
            }
        });

        return isValid;
    },

    // tests a string to see if it is in a valid email format
    isValidEmail: function(email) {
        var re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        return re.test(email);
    }
};
