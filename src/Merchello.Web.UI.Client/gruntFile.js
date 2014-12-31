module.exports = function(grunt) {
    'use strict';

    // Default task.
    //grunt.registerTask('default', ['jshint:dev', 'build', 'karma:unit']);
    grunt.registerTask('default', ['jshint:dev', 'build']);
    grunt.registerTask('dev', ['jshint:dev', 'build-dev']);

    //triggered from grunt dev or grunt
    grunt.registerTask('build', ['clean', 'concat', 'sass:build', 'copy']);

    //build-dev doesn't min - we are trying to speed this up and we don't want minified stuff when we are in dev mode
    grunt.registerTask('build-dev', ['clean', 'concat', 'sass:dev', 'copy', 'karma:unit', 'watch']);

    // watches
    grunt.registerTask('watch-css', ['sass:dev', 'copy:assets', 'copy:vs']);
    grunt.registerTask('watch-js', ['jshint:dev', 'concat', 'copy:app', 'copy:vs', 'karma:unit']);
    grunt.registerTask('watch-test', ['jshint:dev', 'karma:unit']);
    grunt.registerTask('watch-html', ['copy:views', 'copy:vs']);

    // Print a timestamp (useful for when watching)
    grunt.registerTask('timestamp', function() {
        grunt.log.subhead(Date());
    });

    // Project configuration.
    grunt.initConfig({
        buildVersion: grunt.option('buildversion') || '1',
        distdir: 'build/App_Plugins/Merchello',
        bowerfiles: 'bower_components',
        vsdir: '../Merchello.Web.UI/App_Plugins/Merchello2',
        pkg: grunt.file.readJSON('package.json'),

        // The comment block that is inserted at the top of files during build
        banner:
            '/*! <%= pkg.title || pkg.name %>\n' +
            '<%= pkg.homepage ? " * " + pkg.homepage + "\\n" : "" %>' +
            ' * Copyright (c) <%= grunt.template.today("yyyy") %> <%= pkg.author %>;\n' +
            ' * Licensed <%= _.pluck(pkg.licenses, "type").join(", ") %>\n */\n',
        
        // file locations
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
            scss: ['src/scss/merchello.scss'], 
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

           /* mocks: {
                files: [{ dest: '<%= distdir %>/js', src: '*.js', expand: true, cwd: 'src/common/mocks/' }]
            }, */

            manifest: {
                files: [{ dest: '<%= distdir %>/', src: '*.manifest', expand: true, cwd: 'src/' }]
            },

            config: {
              files: [{ dest: '<%= distdir %>/config', src: '**/*.*', expand: true, cwd: 'src/config/'}]  
            },

            assets: {
                // this requires that the scss as been compiled.
                files: [{ dest: '<%= distdir %>/assets/css', src: '*.css', expand: true, cwd: 'src/scss/' }]
            },

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
                    banner: '<%= banner %>\n\n(function() { \n\n',
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
                src: ['src/common/services/*.js', 'src/common/models/factories/**/*.factory.js'],
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
                src: ['src/common/mocks/**/*.js'],
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

        sass: {
            dev: {
                files: {
                    '<%= distdir %>/assets/css/<%= pkg.name %>.css':
                    '<%= src.scss %>'
                }
            },
            build: {
                files: {
                    '<%= distdir %>/assets/css/<%= pkg.name %>.css':
                    '<%= src.scss %>'
                },
                options: {
                    style: 'compressed'
                }
            }
        },

        watch: {
            css: {
                files: '**/*.scss',
                tasks: ['watch-css']
            },
            js: {
                files: ['src/**/*.js', 'src/*.js'],
                tasks: ['watch-js', 'timestamp'],
            },
            test: {
                files: ['tests/**/*.js'],
                tasks: ['watch-test', 'timestamp']
            },
            html: {
                files: ['src/views/**/*.html', 'src/*.html'],
                tasks: ['watch-html', 'timestamp']
            }
        },

        // not in use ATM
        uglify: {
            options: {
                mangle: true
            },
            combine: {
                files: {
                    '<%= distdir %>/js/merchello.min.js': ['<%= distdir %>/js/merchello.*.js']
                }
            }
        },

        karma: {
                unit: { configFile: 'tests/config/karma.config.js', keepalive: true },
                //e2e: { configFile: 'test/config/e2e.js', keepalive: true },
                watch: { configFile: 'test/config/*.unit.js', singleRun: false, autoWatch: true, keepalive: true }
        },

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

            // /App_Plugins/Merchello
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
    grunt.loadNpmTasks('grunt-contrib-watch');
    grunt.loadNpmTasks('grunt-karma');
    grunt.loadNpmTasks('grunt-open');
    grunt.loadNpmTasks('grunt-contrib-connect');

    grunt.loadNpmTasks('grunt-ngdocs');
}