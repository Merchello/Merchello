module.exports = function(grunt) {
    'use strict';

    // Default task.
    //grunt.registerTask('default', ['jshint:dev', 'build', 'karma:unit']);
    grunt.registerTask('default', ['jshint:dev', 'build']);

    //triggered from grunt dev or grunt
    grunt.registerTask('build', ['clean', 'concat', 'recess:min', 'recess:installer', 'recess:canvasdesigner', 'bower', 'copy']);

    //build-dev doesn't min - we are trying to speed this up and we don't want minified stuff when we are in dev mode
    grunt.registerTask('build-dev', ['clean', 'concat', 'copy']);


    // Custom task to run the bower dependency installer
    // tried, a few other things but this seems to work the best.
    // https://coderwall.com/p/xnkdqw
    grunt.registerTask('bower', 'Get js packages listed in bower.json',
        function() {
            var bower = require('bower');
            var done = this.async();

            bower.commands.install(undefined, {}, { interactive: false })
                .on('log', function(data) {
                    grunt.log.write(data.message + "\n");
                })
                .on('error', function(data) {
                    grunt.log.write(data.message + "\n");
                    done(false);
                })
                .on('end', function(data) {
                    done();
                });
        });

    // Project configuration.
    grunt.initConfig({
        buildVersion: grunt.option('buildversion') || '1',
        distdir: 'build/App_Plugins/Merchello',
        bowerfiles: 'bower_components',
        vsdir: '../Merchello.Web.UI/App_Plugins/Merchello2',
        pkg: grunt.file.readJSON('package.json'),

        banner:
            '/*! <%= pkg.title || pkg.name %>\n' +
            '<%= pkg.homepage ? " * " + pkg.homepage + "\\n" : "" %>' +
            ' * Copyright (c) <%= grunt.template.today("yyyy") %> <%= pkg.author %>;\n' +
            ' * Licensed <%= _.pluck(pkg.licenses, "type").join(", ") %>\n */\n',

        src: {
            js: ['src/**/*.js', 'src/*.js'],
            common: ['src/common/**/*.js'],
            specs: ['test/**/*.spec.js'],
            scenarios: ['test/**/*.scenario.js'],
            samples: ['sample files/*.js'],
            html: ['src/index.html', 'src/install.html'], // unused at this point
            everything: ['src/**/*.*', 'test/**/*.*'],
            tpl: {
                app: ['src/views/**/*.html'],
                common: ['src/common/**/*.tpl.html']
            },
            scss: ['src/scss/merchello.scss'], // recess:build doesn't accept ** in its file patterns
            prod: ['<%= distdir %>/js/*.js']
        },

        clean: ['<%= distdir %>/*'],

        copy: {
            views: {
                files: [{ dest: '<%= distdir %>/views', src: ['**/*.*', '!**/*.controller.js'], expand: true, cwd: 'src/views/' }]
            },

            app: {
                files: [
                    { dest: '<%= distdir %>/js', src: '*.js', expand: true, cwd: 'src/' }
                ]
            },

            mocks: {
                files: [{ dest: '<%= distdir %>/js', src: '*.js', expand: true, cwd: 'src/common/mocks/' }]
            },

            //manifest: {
            //    files: [{ dest: '<%= distdir %>/', src: '*.manifest', expand: false, cwd: 'src/' }]
            //},

            vs: {
                files: [
                    //everything except the index.html root file!
                    //then we need to figure out how to not copy all the test stuff either!?
                    { dest: '<%= vsdir %>', src: '*.manfiest', expad: false, cwd: '<%= distdir %>' },
                    { dest: '<%= vsdir %>/assets', src: '**', expand: true, cwd: '<%= distdir %>/assets' },
                    { dest: '<%= vsdir %>/js', src: '**', expand: true, cwd: '<%= distdir %>/js' },
                    { dest: '<%= vsdir %>/lib', src: '**', expand: true, cwd: '<%= distdir %>/lib' },
                    { dest: '<%= vsdir %>/Backoffice/Merchello', src: '**', expand: true, cwd: '<%= distdir %>/views' }
                ]
            }
        },

        concat: {
            models: {
                src: ['src/common/models/*.js', 'src/**/*.model.js'],
                dest: '<%= distdir %>/js/merchello.models.js',
                options: {
                    banner: '<%= banner %>\n(function() { \n\n',
                    footer: '\n\n})();'
                }
            },
            controllers: {
                src: ['src/controllers/**/*.controller.js', 'src/views/**/*.controller.js'],
                dest: '<%= distdir %>/js/merchello.controllers.js',
                options: {
                    banner: '<%= banner %>\n(function() { \n\n',
                    footer: '\n\n})();'
                }
            },
            services: {
                src: ['src/common/services/*.js'],
                dest: '<%= distdir %>/js/merchello.services.js',
                options: {
                    banner: '<%= banner %>\n(function() { \n\n',
                    footer: '\n\n})();'
                }
            },
            resources: {
                src: ['src/common/resources/*.js'],
                dest: '<%= distdir %>/js/merchello.resources.js',
                options: {
                    banner: '<%= banner %>\n(function() { \n\n',
                    footer: '\n\n})();'
                }
            },
            testing: {
                src: ['src/common/mocks/*/*.js'],
                dest: '<%= distdir %>/js/merchello.testing.js',
                options: {
                    banner: "<%= banner %>\n(function() { \n\n",
                    footer: "\n\n})();"
                }
            },
            directives: {
                src: ['src/common/directives/**/*.js'],
                dest: '<%= distdir %>/js/merchello.directives.js',
                options: {
                    banner: '<%= banner %>\n(function() { \n\n',
                    footer: '\n\n})();'
                }
            },
            filters: {
                src: ['src/common/filters/*.js'],
                dest: '<%= distdir %>/js/merchello.filters.js',
                options: {
                    banner: '<%= banner %>\n(function() { \n\n',
                    footer: '\n\n})();'
                }
            }
        },

        //karma: {
        //        unit: { configFile: 'test/config/karma.conf.js', keepalive: true },
        //        e2e: { configFile: 'test/config/e2e.js', keepalive: true },
        //        watch: { configFile: 'test/config/unit.js', singleRun: false, autoWatch: true, keepalive: true }
        //},

        jshint: {
            dev: {
                files: {
                    src: ['<%= src.common %>', '<%= src.specs %>', '<%= src.scenarios %>']
                },
                options: {
                    curly: true,
                    eqeqeq: true,
                    immed: true,
                    latedef: true,
                    newcap: true,
                    noarg: true,
                    sub: true,
                    boss: true,
                    //NOTE: This is required so it doesn't barf on reserved words like delete when doing $http.delete
                    es5: true,
                    eqnull: true,
                    //NOTE: we need to use eval sometimes so ignore it
                    evil: true,
                    //NOTE: we need to check for strings such as "javascript:" so don't throw errors regarding those
                    scripturl: true,
                    //NOTE: we ignore tabs vs spaces because enforcing that causes lots of errors depending on the text editor being used
                    smarttabs: true,
                    globals: {}
                }
            },

            // merchello plugins
            build: {
                files: {
                    src: ['<%= src.prod %>']
                },
                options: {
                    curly: true,
                    eqeqeq: true,
                    immed: true,
                    latedef: true,
                    newcap: true,
                    noarg: true,
                    sub: true,
                    boss: true,
                    //NOTE: This is required so it doesn't barf on reserved words like delete when doing $http.delete
                    es5: true,
                    eqnull: true,
                    //NOTE: we need to use eval sometimes so ignore it
                    evil: true,
                    //NOTE: we need to check for strings such as "javascript:" so don't throw errors regarding those
                    scripturl: true,
                    //NOTE: we ignore tabs vs spaces because enforcing that causes lots of errors depending on the text editor being used
                    smarttabs: true,
                    globalstrict: true,
                    globals: { $: false, jQuery: false, define: false, require: false, window: false }
                }
            }
        }
    });


    grunt.loadNpmTasks('grunt-contrib-concat');
    grunt.loadNpmTasks('grunt-contrib-jshint');
    grunt.loadNpmTasks('grunt-contrib-clean');
    grunt.loadNpmTasks('grunt-contrib-copy');
    grunt.loadNpmTasks('grunt-contrib-uglify');
    grunt.loadNpmTasks('grunt-contrib-sass');
    // grunt.loadNpmTasks('grunt-contrib-watch');
    // grunt.loadNpmTasks('grunt-recess');

    grunt.loadNpmTasks('grunt-karma');

}