module.exports = function(config) {

    config.set({
        basePath : '../',
        frameworks: ["jasmine"],
        files : [
            'app/lib/angular/angular.js',
            'app/lib/angular/angular-*.js',
            'app/lib/jquery/jquery-*.js',
            'app/lib/underscore/*.js',
            'app/lib/select2/select2.js',
            'app/lib/select2/ui-select2.js',
            'test/lib/angular/angular-mocks.js',
            'app/js/**/*.js',
            '../Merchello.Web.UI/App_Plugins/Merchello/Common/Js/merchello.namespaces.js',
            '../Merchello.Web.UI/App_Plugins/Merchello/Common/Js/**/*.js',
            '../Merchello.Web.UI/App_Plugins/Merchello/Modules/Catalog/product.models.js',
            '../Merchello.Web.UI/App_Plugins/Merchello/Modules/Catalog/Directives/productvariant.directives.js',
            'test/unit/**/*.js'
        ],

        autoWatch : true,

        browsers : ['Chrome'],

        junitReporter : {
            outputFile: 'test_out/unit.xml',
            suite: 'unit'
        },

        // level of logging
        // possible values: LOG_DISABLE || LOG_ERROR || LOG_WARN || LOG_INFO || LOG_DEBUG
        logLevel: LOG_DEBUG,

        preprocessors: {
            '../Merchello.Web.UI/App_Plugins/Merchello/Modules/Catalog/Directives/product-main-edit.html': 'ng-html2js'
        },

        ngHtml2JsPreprocessor: {
            // strip this from the file path
            stripPrefix: '/App_Plugins/',
                // prepend this to the
            prependPrefix: '../Merchello.Web.UI/App_Plugins/',

            moduleName: 'merchellotemplates'
        }
    });
};