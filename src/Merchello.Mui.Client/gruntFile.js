module.exports = function(grunt) {
    'use strict';

    // Default task.
    //grunt.registerTask('default', ['jshint:dev', 'build', 'karma:unit']);
    grunt.registerTask('default', ['jshint:dev', 'build']);

    //triggered from grunt dev or grunt
    grunt.registerTask('build', ['clean', 'concat', 'sass', 'uglify:build', 'copy']);



    // Print a timestamp (useful for when watching)
    grunt.registerTask('timestamp', function() {
        grunt.log.subhead(Date());
    });

    // Project configuration.
    grunt.initConfig({
        buildVersion: grunt.option('buildversion') || '1',
        distdir: 'build/App_Plugins/Merchello',
        vsdir: '../Merchello.FastTrack.UI/App_Plugins/Merchello',
        appdir: '../Merchello.FastTrack.UI',
        pkg: grunt.file.readJSON('package.json'),

        // The comment block that is inserted at the top of files during build
        banner:
            '/*! <%= pkg.title || pkg.name %>\n' +
            '<%= pkg.homepage ? " * " + pkg.homepage + "\\n" : "" %>' +
            ' * Copyright (c) <%= grunt.template.today("yyyy") %> <%= pkg.author %>.\n' +
            ' * Licensed <%= _.map(pkg.licenses, "type").join(", ") %>\n */\n',
        
        // file locations
        src: {
            mui: ['src/jquery/**/*.js'],
            img: ['src/images/**/*.*'],
            lib: ['lib/**/*.js'],
            scss: ['src/scss/mui.scss'],
            prod: ['<%= distdir %>/js/*.js']
        },

        clean: ['<%= distdir %>/*'],

        copy: {

            assets: {
                // this requires that the scss as been compiled.
                files: [
                    { dest: '<%= distdir %>/client/css', src: '*.css', expand: true, cwd: 'src/scss/' },
                    { dest: '<%= distdir %>/client/img', src: '*.*', expand: true, cwd: 'src/images/' },
                    { dest: '<%= distdir %>/client/lib', src: 'card-validator.min.js', expand: true, cwd: 'lib/' }
                ]
            },

            vs: {
                files: [
                    //then we need to figure out how to not copy all the test stuff either!?
                    { dest: '<%= vsdir %>/client', src: '**', expand: true, cwd: '<%= distdir %>/client/' },
                    //{ dest: '<%= vsdir %>/client', src: '**', expand: true, cwd: '<%= distdir %>/client/js' },
                    { dest: '<%= vsdir %>/config', src: '*.config', expand: true, cwd: '<%= distdir %>/config' },
                    { dest: '<%= vsdir %>/propertyeditors', src: '**', expand: true, cwd: '<%= distdir %>/views/propertyeditors' },
                    { dest: '<%= vsdir %>/backoffice/dialogs', src: '**', expand: true, cwd: '<%= distdir %>/views/dialogs' }
                ]
            }
        },

        concat: {
            mui: {
                src: ['src/jquery/mui/*.js', 'src/jquery/mui/logger/**/*.js', 'src/jquery/mui/services/**/*.js', 'src/jquery/mui/modules/*/*.js', 'src/jquery/mui/modules/*/components/*.js', 'src/jquery/bootstrapper.js'],
                dest: '<%= distdir %>/client/js/merchello.ui.js',
                options: {
                    banner: '<%= banner %>\n\n',
                    footer: '\n\n'
                }
            },

            settings: {
                src: ['src/jquery/mui.settings.js'],
                dest: '<%= distdir %>/client/js/merchello.ui.settings.js',
                options: {
                    banner: '<%= banner %>\n\n',
                    footer: '\n\n'
                }
            },

            fasttrack: {
                src: ['src/jquery/fasttrack.js'],
                dest: '<%= distdir %>/client/js/fasttrack.js',
                options: {
                    banner: '<%= banner %>\n\n',
                    footer: '\n\n'
                }
            }
        },

        sass: {
            dev: {
                files: {
                    '<%= distdir %>/client/css/merchello.ui.css':
                    '<%= src.scss %>'
                }
            },
            prod: {
                files: {
                    '<%= distdir %>/client/css/merchello.ui.min.css':
                    '<%= src.scss %>'
                },
                options: {
                    style: 'compressed'
                }
            }
        },

        uglify: {
            build: {
                files: {
                    '<%= distdir %>/client/js/merchello.ui.min.js': ['<%= distdir %>/client/js/merchello.ui.js']
                },
                options: {
                    mangle: true,
                    sourceMap: true,
                    sourceMapName: '<%= distdir %>/client/js/merchello.ui.js.map',
                    banner: '<%= banner %>\n\n',
                    footer: '\n'
                }
            }
        },


        jshint: {
            dev: {
                files: {
                    src: ['<%= src.common %>', '<%= src.specs %>', '<%= src.scenarios %>']
                },
                options: {
                    JQuery: true,
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

            // /App_Plugins/MerchelloPaymentProviders
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
}