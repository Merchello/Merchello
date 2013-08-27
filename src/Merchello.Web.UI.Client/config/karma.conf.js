module.exports = function(config) {

    config.set({
        basePath : '../',
        frameworks : ["jasmine"],
        files : [
        'app/lib/angular/angular.js',
        'app/lib/angular/angular-*.js',
        'app/lib/jquery/jquery-*.js',
        'app/lib/select2/select2.js',
        'app/lib/select2/ui-select2.js',
        'test/lib/angular/angular-mocks.js',
        'app/js/**/*.js',
        'test/unit/**/*.js'
        ],

        autoWatch : true,

        browsers : ['Chrome'],

        junitReporter : {
            outputFile: 'test_out/unit.xml',
            suite: 'unit'
        }
    });
};