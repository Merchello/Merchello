# TL;DR
[Video and Blog Post using
grunt-markdown](http://treasonx.com/articles/GruntMarkDown.html)

# grunt-markdown

Compile markdown to html. This grunt task will take a given set of input markdown files and convert them to HTML.It supports [GFM](http://github.github.com/github-flavored-markdown/) with code highlighting. The code highlighting is done using [highlight.js](http://softwaremaniacs.org/soft/highlight/en/).

## Getting Started
Install this grunt plugin next to your project's [Gruntfile.js][getting_started] with: `npm install grunt-markdown`

Then add this line to your project's `grunt.js` gruntfile:

```javascript
grunt.loadNpmTasks('grunt-markdown');
```

[grunt]: http://gruntjs.com/
[getting_started]: https://github.com/gruntjs/grunt/blob/master/docs/getting_started.md

## Documentation
Creating a markdown task is simple. In your grunt file add the following config.

```javascript
grunt.initConfig({
  markdown: {
    all: {
      files: ['test/sample/*'],
      dest: 'test/out',
      options: {
        gfm: true,
        highlight: manual
        codeLines: {
          before: '<span>',
          after: '</span>'
        }
      }
    }  
  }  
});

````

The `markdown` task is a multitask and can have different targets. In the example above we have target specified called `all`.

* `files`: The list of markdown files.
* `template`: If you wish to provide your own html template specify here.
* `dest`: The location for the compiled HTML files
* `options`: options to be passed to the markdown parser

The parser options are passed to the [marked](https://github.com/chjj/marked) markdown parser. The only option that is processed prior to compiling the markdown is the `highlight` option. If you specify `auto`or `manual` the task will handle highlighting code blocks for you use highlight.js. If you pass a custom function as the highlight option it will be used to highlight the code.

* `auto`: Will try to detect the language
* `manual`: will pass the language name from markdown to the highlight function
* `codeLines`: specify text that should wrap code lines


###Template File

You can provide any template file you would like and it will be used in place of the generic template provided. Where the HTML is injected into your template is specified using `<%=content%>` tag in your html template.

## License
Copyright (c) 2012 James Morrin  
Licensed under the MIT license.
