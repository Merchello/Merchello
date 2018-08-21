;(function ($, window, document, undefined) {
    $.wcpCompress = function(obj, defaults) {
        objCopy = $.extend(true, {}, obj);
        defaultsCopy = $.extend(true, {}, defaults);

        var objSubtracted = subtract(objCopy, defaultsCopy);
        var objCleaned = clean(objSubtracted);

        return objCleaned;
    }
    function subtract(a, b) {
        var r = {};

        // For each property of 'b'
        // if it's different than the corresponding property of 'a'
        // place it in 'r'
        for (var key in b) {
            if (Object.prototype.toString.call(b[key]) === '[object Array]') {
                r[key] = a[key].slice();
            } else if (typeof(b[key]) == 'object') {
                if (!a[key]) a[key] = {};
                r[key] = subtract(a[key], b[key]);
            } else {
                if (b[key] != a[key]) {
                    r[key] = a[key];
                }
            }
        }

        return r;
    }
    function clean(a) {
        var r = undefined;

        // Check if 'a' is an object or array
        if (Object.prototype.toString.call(a) === '[object Array]') {
            if (a.length == 0) {
                r = undefined;
            } else {
                r = a.slice();
            }
        }
        if (typeof(a) == 'object') {
            // If 'a' is an object, check if it's empty and set to undefined if true
            if (isEmpty(a)) {
                r = undefined;
            } else {
                // If 'a' is NOT empty, iterate over each of its properties
                // and recursively clean
                for (var key in a) {
                    var cleaned = clean(a[key]);

                    if (cleaned !== undefined) {
                        if (r === undefined) r = {};

                        r[key] = cleaned;
                    }
                }
            }
        } else {
            r = a;
        }

        return r;
    }
    function isEmpty(obj) {
        for(var prop in obj) {
            if(obj.hasOwnProperty(prop))
            return false;
        }

        return true && JSON.stringify(obj) === JSON.stringify({});
    }
})(jQuery, window, document);
