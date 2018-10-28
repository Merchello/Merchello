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
  var $_7a5bps9ojh8lpugv = {
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
  var $_4f0hos9qjh8lpugw = {
    setContent: setContent,
    getContent: getContent
  };

  var open = function (editor) {
    var minWidth = $_7a5bps9ojh8lpugv.getMinWidth(editor);
    var minHeight = $_7a5bps9ojh8lpugv.getMinHeight(editor);
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
        $_4f0hos9qjh8lpugw.setContent(editor, e.data.code);
      }
    });
    win.find('#code').value($_4f0hos9qjh8lpugw.getContent(editor));
  };
  var $_8t2ji69njh8lpugu = { open: open };

  var register = function (editor) {
    editor.addCommand('mceCodeEditor', function () {
      $_8t2ji69njh8lpugu.open(editor);
    });
  };
  var $_1cb0ek9mjh8lpugt = { register: register };

  var register$1 = function (editor) {
    editor.addButton('code', {
      icon: 'code',
      tooltip: 'Source code',
      onclick: function () {
        $_8t2ji69njh8lpugu.open(editor);
      }
    });
    editor.addMenuItem('code', {
      icon: 'code',
      text: 'Source code',
      onclick: function () {
        $_8t2ji69njh8lpugu.open(editor);
      }
    });
  };
  var $_aziuou9rjh8lpugx = { register: register$1 };

  global.add('code', function (editor) {
    $_1cb0ek9mjh8lpugt.register(editor);
    $_aziuou9rjh8lpugx.register(editor);
    return {};
  });
  function Plugin () {
  }

  return Plugin;

}());
})();
