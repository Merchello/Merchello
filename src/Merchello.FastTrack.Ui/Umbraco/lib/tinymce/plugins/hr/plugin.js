(function () {
var hr = (function () {
  'use strict';

  var global = tinymce.util.Tools.resolve('tinymce.PluginManager');

  var register = function (editor) {
    editor.addCommand('InsertHorizontalRule', function () {
      editor.execCommand('mceInsertContent', false, '<hr />');
    });
  };
  var $_1613cuc0jfuviwwp = { register: register };

  var register$1 = function (editor) {
    editor.addButton('hr', {
      icon: 'hr',
      tooltip: 'Horizontal line',
      cmd: 'InsertHorizontalRule'
    });
    editor.addMenuItem('hr', {
      icon: 'hr',
      text: 'Horizontal line',
      cmd: 'InsertHorizontalRule',
      context: 'insert'
    });
  };
  var $_6b0mxvc1jfuviwwq = { register: register$1 };

  global.add('hr', function (editor) {
    $_1613cuc0jfuviwwp.register(editor);
    $_6b0mxvc1jfuviwwq.register(editor);
  });
  function Plugin () {
  }

  return Plugin;

}());
})();
