var gulp = require('gulp');

var pkg = require('./package.json');


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

    gulp.src(configs + 'merchelloSettings.config')
        .pipe(gulp.dest(tests + 'bin/debug/Configurations/MerchelloSettings/'))
        .pipe(gulp.dest(tests + 'bin/release/Configurations/MerchelloSettings/'));


    gulp.src(configs + 'tests/web.settings.config')
        //.pipe(rename('build/temp/config/settings/web.config'))
        .pipe(gulp.dest(tests + 'bind/debug/Configurations/'))
        .pipe(gulp.dest(tests + 'bind/release/Configurations/'));

    gulp.src(configs + 'merchelloExtensibility.config')
        .pipe(gulp.dest(tests + 'bin/debug/Configurations/MerchelloExtensibility/'))
        .pipe(gulp.dest(tests + 'bin/release/Configurations/MerchelloExtensibility/'));

    gulp.src(configs + 'merchelloCountries.config')
        .pipe(gulp.dest(tests + 'bin/debug/Configurations/MerchelloCountries/'))
        .pipe(gulp.dest(tests + 'bin/release/Configurations/MerchelloCountries/'));
});



/*
------------------------------------------------------
UTILITY
------------------------------------------------------
 */


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

gulp.task('dev', function() {

});



// Init
// The default gulp task
gulp.task('default', ['clean'], function() {
    // place code for your default task here


    console.info('Default');

});


