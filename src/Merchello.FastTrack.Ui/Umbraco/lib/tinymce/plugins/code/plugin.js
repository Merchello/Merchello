(function () {
var code = (function () {
  'use strict';

  var global = tinymce.util.Tools.resolve('tinymce.PluginManager');

  var global$1 = tinymce.util.Tools.resolve('tinymce.dom.DOMUtils');

  var getMinWidth = function (editor) {
    return editor.getParam('code_dialog_width', 600);
  };
  var getMinHeight = function (editor) {
    return editor.getParam('code_dialog_height', Math.min(global$1.DOM.getViewPort().h - 200, 500));
  };
  var $_8sda989hjfuviwmn = {
    getMinWidth: getMinWidth,
    getMinHeight: getMinHeight
  };

  var setContent = function (editor, html) {
    editor.focus();
    editor.undoManager.transact(function () {
      editor.setContent(html);
    });
    editor.selection.setCursorLocation();
    editor.nodeChanged();
  };
  var getContent = function (editor) {
    return editor.getContent({ source_view: true });
  };
  var $_a8y3nm9jjfuviwmp = {
    setContent: setContent,
    getContent: getContent
  };

  var open = function (editor) {
    var minWidth = $_8sda989hjfuviwmn.getMinWidth(editor);
    var minHeight = $_8sda989hjfuviwmn.getMinHeight(editor);
    var win = editor.windowManager.open({
      title: 'Source code',
      body: {
        type: 'textbox',
        name: 'code',
        multiline: true,
        minWidth: minWidth,
        minHeight: minHeight,
        spellcheck: false,
        style: 'direction: ltr; text-align: left'
      },
      onSubmit: function (e) {
        $_a8y3nm9jjfuviwmp.setContent(editor, e.data.code);
      }
    });
    win.find('#code').value($_a8y3nm9jjfuviwmp.getContent(editor));
  };
  var $_1358eu9gjfuviwml = { open: open };

  var register = function (editor) {
    editor.addCommand('mceCodeEditor', function () {
      $_1358eu9gjfuviwml.open(editor);
    });
  };
  var $_d03lq09fjfuviwmj = { register: register };

  var register$1 = function (editor) {
    editor.addButton('code', {
      icon: 'code',
      tooltip: 'Source code',
      onclick: function () {
        $_1358eu9gjfuviwml.open(editor);
      }
    });
    editor.addMenuItem('code', {
      icon: 'code',
      text: 'Source code',
      onclick: function () {
        $_1358eu9gjfuviwml.open(editor);
      }
    });
  };
  var $_g7tt9y9kjfuviwmp = { register: register$1 };

  global.add('code', function (editor) {
    $_d03lq09fjfuviwmj.register(editor);
    $_g7tt9y9kjfuviwmp.register(editor);
    return {};
  });
  function Plugin () {
  }

  return Plugin;

}());
})();
