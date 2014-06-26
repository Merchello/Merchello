/*
 * grunt-markdown
 * https://github.com/treasonx/grunt-markdown
 *
 * Copyright (c) 2012 James Morrin
 * Licensed under the MIT license.
 */

module.exports = function(grunt) {
  'use strict';

  var path = require('path');

  // Internal lib.
  var markdown = require('./lib/markdown').init(grunt);

  grunt.registerMultiTask('markdown', 'compiles markdown files into html', function() {
    var destPath = this.data.dest;
    var options = this.data.options || {};
    var extension = this.data.extenstion || 'html';
    var templateFn = this.data.template || path.join(__dirname, 'template.html');
    var template = grunt.file.read(templateFn);

    grunt.file.expand({filter:'isFile'}, this.data.files).forEach(function(filepath) {

      var file = grunt.file.read(filepath);

      var html = markdown.markdown(file, options, template);
      var ext = path.extname(filepath);
      var dest = path.join(destPath, path.basename(filepath, ext) +'.'+ extension);
      grunt.file.write(dest, html);

    });

  });

};

