var gulp = require('gulp');

var pkg = require('./package.json');

// build configuration
var debug = 'bin/Debug/';
var release = 'bin/Release/';

// source locations
var allSrc = 'src/**/*.js';
var configs = 'src/config/';
var dist = './build/App_Plugins/Merchello/';
var tests = '../../tests/Merchello.Tests.Unit/'
var concatenated = [];


// cleans the build directory
gulp.task('clean', function() {
    var del = require('del');
    return del([
       dist
    ]);

    /*
     //'dist/report.csv',
      here we use a globbing pattern to match everything inside the `mobile` folder
     'dist/mobile/ ** / *',
     we don't want to clean this file though so we negate the pattern
    '!dist/mobile/deploy.json'
    E*/
});

// copies the config files to the tests directory to ensure current
gulp.task('copy:tests', function() {
    // todo addd changed

    var copy = require('gulp-copy');
    var rename = require('gulp-rename');

    var paths = {
        settings: 'Configurations/MerchelloSettings/',
        extensibility: 'Configurations/ExtensibilitySettings/',
        countries: 'Configurations/CountrySettings/'
    }

    var tdebug = tests + debug;
    var trelease = tests + release;

    // -------------------
    // setttings
    // -------------------
    // merchelloSettings.config
    gulp.src(configs + 'merchelloSettings.config')
        .pipe(gulp.dest(tdebug + paths.settings))
        .pipe(gulp.dest(tdebug + paths.settings));

    // web.config
    gulp.src(configs + 'tests/web.settings.config')
        .pipe(rename('web.config'))
        .pipe(gulp.dest(tdebug + 'Configurations/MerchelloSettings/'))
        .pipe(gulp.dest(trelease + 'Configurations/MerchelloSettings/'));

    // -------------------
    // extensibility
    // -------------------
    gulp.src(configs + 'merchelloExtensibility.config')
        .pipe(gulp.dest(tdebug + 'Configurations/MerchelloExtensibility/'))
        .pipe(gulp.dest(trelease + 'Configurations/MerchelloExtensibility/'));

    // web.config
    gulp.src(configs + 'tests/web.extensibility.config')
        .pipe(rename('web.config'))
        .pipe(gulp.dest(tdebug + 'Configurations/MerchelloExtensibility/'))
        .pipe(gulp.dest(trelease + 'Configurations/MerchelloExtensibility/'));

    // -------------------
    // countries
    // -------------------
    gulp.src(configs + 'merchelloCountries.config')
        .pipe(gulp.dest(tests + 'bin/debug/Configurations/MerchelloCountries/'))
        .pipe(gulp.dest(tests + 'bin/release/Configurations/MerchelloCountries/'));

    // web.config
    gulp.src(configs + 'tests/web.countries.config')
        .pipe(rename('web.config'))
        .pipe(gulp.dest(tdebug + 'Configurations/MerchelloCountries/'))
        .pipe(gulp.dest(trelease + 'Configurations/MerchelloCountries/'));
});



/********************************************************
*    UTILITY
*********************************************************/

// Code checker that finds common mistakes in scripts
gulp.task('lint', function() {
    var jshint = require('gulp-jshint');
    var jshintStylish = require('jshint-stylish');

    gulp.src(allSrc)
        .pipe(jshint())
        .pipe(jshint.reporter(jshintStylish));
});

// Logs out the total size of files in the stream and optionally the individual file-sizes
gulp.task('size', function () {
    var size = require('gulp-size');

    gulp.src(concatenated)
        .pipe(size({ showFiles: true }));
});


gulp.task('compress', function () {
    var closureCompiler = require('gulp-closure-compiler');
    var bytediff = require('gulp-bytediff');

    gulp.src(concatenated)
        .pipe(bytediff.start())
        .pipe(closureCompiler())
        .pipe(bytediff.stop())
        .pipe(gulp.dest(dist + 'compressed'));
});

// not used
gulp.task('uglify', function() {
    var uglify = require('gulp-uglify');
    var bytediff = require('gulp-bytediff');

    gulp.src(sources)
        .pipe(bytediff.start())
        .pipe(uglify())
        .pipe(bytediff.stop())
        .pipe(gulp.dest(dist + 'uglified'));
});

gulp.task('minify', function() {
    var uglify = require('gulp-uglify');
    var closureCompiler = require('gulp-closure-compiler');
    var bytediff = require('gulp-bytediff');

    gulp.src(concatenated)
        .pipe(bytediff.start())
        .pipe(closureCompiler())
        .pipe(uglify())
        .pipe(bytediff.stop())
        .pipe(gulp.dest(dist + 'minified'));
});


// javascript checker
gulp.task('jscs', function () {
    var jscs = require('gulp-jscs');
    gulp.src(allSrc)
        .pipe(jscs());
});

// adds the header
gulp.task('header', function () {
    var header = require('gulp-header');
    gulp.src(concatenated)
        .pipe(header('This is a header for ${name}!\n', { name : 'gulp test'} ))
        .pipe(gulp.dest(dist + 'header'));
});

// sloc is a simple tool to count SLOC (source lines of code)
gulp.task('sloc', function(){
    var sloc = require('gulp-sloc');

    gulp.src(allSources)
        .pipe(sloc());
});

/********************************************************
 *    USE
 *    ?> gulp test      -- not implemented
 *    ?> gulp dev
 *    ?> gulp release   -- not implemented
 *    ?> gulp
 *********************************************************/

gulp.task('dev', ['copy:tests'], function() {

});

gulp.task('default', ['clean', 'copy:tests'], function() {
    // place code for your default task here


    console.info('Default');

});


