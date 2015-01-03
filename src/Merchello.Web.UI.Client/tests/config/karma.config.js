module.exports = function (karma) {

    karma.set({

        // base path, that will be used to resolve files and exclude
        basePath: '../..',

        frameworks: ["jasmine"],

        // list of files / patterns to load in the browser
        files: [
            'lib/jquery/jquery-2.0.3.min.js',
            'lib/angular/1.1.5/angular.js',
            'lib/angular/1.1.5/angular-cookies.min.js',
            'lib/angular/1.1.5/angular-mocks.js',
            'lib/angular/angular-ui-sortable.js',
            'lib/angular/tmhDynamicLocale.js',

            'lib/underscore/underscore.js',
            'lib/umbraco/Extensions.js',
            'lib/lazyload/lazyload.min.js',
            'lib/select2/select2.js',
            'lib/select2/ui-select2.js',

            // umbraco
            'tests/config/app.unit.js',
            'tests/lib/merchello/umbraco.servervariables.js',
            'tests/lib/umbraco/umbraco.directives.js',
            'tests/lib/umbraco/umbraco.filters.js',
            'tests/lib/umbraco/umbraco.services.js',
            'tests/lib/umbraco/umbraco.security.js',
            'tests/lib/umbraco/umbraco.resources.js',
            'tests/lib/umbraco/umbraco.testing.js',
            'tests/lib/umbraco/umbraco.controllers.js',

            // merchello
            'tests/config/merchello.module.unit.js',

            //// this is to test the actual build files
            /*
            'build/App_Plugins/Merchello/js/merchello.models.js',
            'build/App_Plugins/Merchello/js/merchello.filters.js',
            'build/App_Plugins/Merchello/js/merchello.directives.js',
            'build/App_Plugins/Merchello/js/merchello.resources.js',
            'build/App_Plugins/Merchello/js/merchello.services.js',
            'build/App_Plugins/Merchello/js/merchello.controllers.js',
            'build/App_Plugins/Merchello/js/merchello.testing.js',
            */

            //// this is for development - easier to find line numbers of stuff that is breaking.
            'src/common/models/**/*.model.js',
            'src/common/**/*.js',
            'src/views/**/*.js',
            'tests/unit/**/*.spec.js',
            { pattern: 'lib/**/*.js', watched: true, served: true, included: false }
        ],

        // list of files to exclude
        exclude: [],

        // use dolts reporter, as travis terminal does not support escaping sequences
        // possible values: 'dots', 'progress', 'junit', 'teamcity'
        // CLI --reporters progress
        reporters: ['progress'],

        // web server port
        // CLI --port 9111
        port: 9111,

        // cli runner port
        // CLI --runner-port 9100
        runnerPort: 9110,

        // enable / disable colors in the output (reporters and logs)
        // CLI --colors --no-colors
        colors: true,

        // level of logging
        // possible values: karma.LOG_DISABLE || karma.LOG_ERROR || karma.LOG_WARN || karma.LOG_INFO || karma.LOG_DEBUG
        // CLI --log-level debug
        logLevel: karma.LOG_INFO,

        // enable / disable watching file and executing tests whenever any file changes
        // CLI --auto-watch --no-auto-watch
        autoWatch: false,

        // Start these browsers, currently available:
        // - Chrome
        // - ChromeCanary
        // - Firefox
        // - Opera
        // - Safari (only Mac)
        // - PhantomJS
        // - IE (only Windows)
        // CLI --browsers Chrome,Firefox,Safari
        browsers: ['PhantomJS'],


        // If browser does not capture in given timeout [ms], kill it
        // CLI --capture-timeout 5000
        captureTimeout: 5000,

        // Auto run tests on start (when browsers are captured) and exit
        // CLI --single-run --no-single-run
        singleRun: true,

        // report which specs are slower than 500ms
        // CLI --report-slower-than 500
        reportSlowerThan: 500,

        junitReporter: {
            outputFile: 'test_out/unit.xml',
            suite: 'unit'
        },

        //preprocessors: {
        //    '../Merchello.Web.UI/App_Plugins/Merchello/Modules/Catalog/Directives/product-main-edit.html': 'ng-html2js'
        //},

        plugins: [
          'karma-jasmine',
          'karma-phantomjs-launcher'
        ]

        //ngHtml2JsPreprocessor: {
        //    // strip this from the file path
        //    stripPrefix: '/App_Plugins/',
        //    // prepend this to the
        //    prependPrefix: '../Merchello.Web.UI/App_Plugins/',

        //    moduleName: 'merchellotemplates'
        //}
    });
};