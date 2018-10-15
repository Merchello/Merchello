(function () {
var table = (function () {
  'use strict';

  var global = tinymce.util.Tools.resolve('tinymce.PluginManager');

  var noop = function () {
    var x = [];
    for (var _i = 0; _i < arguments.length; _i++) {
      x[_i] = arguments[_i];
    }
  };
  var noarg = function (f) {
    return function () {
      var x = [];
      for (var _i = 0; _i < arguments.length; _i++) {
        x[_i] = arguments[_i];
      }
      return f();
    };
  };
  var compose = function (fa, fb) {
    return function () {
      var x = [];
      for (var _i = 0; _i < arguments.length; _i++) {
        x[_i] = arguments[_i];
      }
      return fa(fb.apply(null, arguments));
    };
  };
  var constant = function (value) {
    return function () {
      return value;
    };
  };
  var identity = function (x) {
    return x;
  };
  var tripleEquals = function (a, b) {
    return a === b;
  };
  var curry = function (f) {
    var x = [];
    for (var _i = 1; _i < arguments.length; _i++) {
      x[_i - 1] = arguments[_i];
    }
    var args = new Array(arguments.length - 1);
    for (var i = 1; i < arguments.length; i++)
      args[i - 1] = arguments[i];
    return function () {
      var x = [];
      for (var _i = 0; _i < arguments.length; _i++) {
        x[_i] = arguments[_i];
      }
      var newArgs = new Array(arguments.length);
      for (var j = 0; j < newArgs.length; j++)
        newArgs[j] = arguments[j];
      var all = args.concat(newArgs);
      return f.apply(null, all);
    };
  };
  var not = function (f) {
    return function () {
      var x = [];
      for (var _i = 0; _i < arguments.length; _i++) {
        x[_i] = arguments[_i];
      }
      return !f.apply(null, arguments);
    };
  };
  var die = function (msg) {
    return function () {
      throw new Error(msg);
    };
  };
  var apply = function (f) {
    return f();
  };
  var call = function (f) {
    f();
  };
  var never = constant(false);
  var always = constant(true);
  var $_fdch7uk7jfuvixxb = {
    noop: noop,
    noarg: noarg,
    compose: compose,
    constant: constant,
    identity: identity,
    tripleEquals: tripleEquals,
    curry: curry,
    not: not,
    die: die,
    apply: apply,
    call: call,
    never: never,
    always: always
  };

  var never$1 = $_fdch7uk7jfuvixxb.never;
  var always$1 = $_fdch7uk7jfuvixxb.always;
  var none = function () {
    return NONE;
  };
  var NONE = function () {
    var eq = function (o) {
      return o.isNone();
    };
    var call = function (thunk) {
      return thunk();
    };
    var id = function (n) {
      return n;
    };
    var noop = function () {
    };
    var me = {
      fold: function (n, s) {
        return n();
      },
      is: never$1,
      isSome: never$1,
      isNone: always$1,
      getOr: id,
      getOrThunk: call,
      getOrDie: function (msg) {
        throw new Error(msg || 'error: getOrDie called on none.');
      },
      or: id,
      orThunk: call,
      map: none,
      ap: none,
      each: noop,
      bind: none,
      flatten: none,
      exists: never$1,
      forall: always$1,
      filter: none,
      equals: eq,
      equals_: eq,
      toArray: function () {
        return [];
      },
      toString: $_fdch7uk7jfuvixxb.constant('none()')
    };
    if (Object.freeze)
      Object.freeze(me);
    return me;
  }();
  var some = function (a) {
    var constant_a = function () {
      return a;
    };
    var self = function () {
      return me;
    };
    var map = function (f) {
      return some(f(a));
    };
    var bind = function (f) {
      return f(a);
    };
    var me = {
      fold: function (n, s) {
        return s(a);
      },
      is: function (v) {
        return a === v;
      },
      isSome: always$1,
      isNone: never$1,
      getOr: constant_a,
      getOrThunk: constant_a,
      getOrDie: constant_a,
      or: self,
      orThunk: self,
      map: map,
      ap: function (optfab) {
        return optfab.fold(none, function (fab) {
          return some(fab(a));
        });
      },
      each: function (f) {
        f(a);
      },
      bind: bind,
      flatten: constant_a,
      exists: bind,
      forall: bind,
      filter: function (f) {
        return f(a) ? me : NONE;
      },
      equals: function (o) {
        return o.is(a);
      },
      equals_: function (o, elementEq) {
        return o.fold(never$1, function (b) {
          return elementEq(a, b);
        });
      },
      toArray: function () {
        return [a];
      },
      toString: function () {
        return 'some(' + a + ')';
      }
    };
    return me;
  };
  var from = function (value) {
    return value === null || value === undefined ? NONE : some(value);
  };
  var Option = {
    some: some,
    none: none,
    from: from
  };

  var typeOf = function (x) {
    if (x === null)
      return 'null';
    var t = typeof x;
    if (t === 'object' && Array.prototype.isPrototypeOf(x))
      return 'array';
    if (t === 'object' && String.prototype.isPrototypeOf(x))
      return 'string';
    return t;
  };
  var isType = function (type) {
    return function (value) {
      return typeOf(value) === type;
    };
  };
  var $_13kw1fk8jfuvixxd = {
    isString: isType('string'),
    isObject: isType('object'),
    isArray: isType('array'),
    isNull: isType('null'),
    isBoolean: isType('boolean'),
    isUndefined: isType('undefined'),
    isFunction: isType('function'),
    isNumber: isType('number')
  };

  var rawIndexOf = function () {
    var pIndexOf = Array.prototype.indexOf;
    var fastIndex = function (xs, x) {
      return pIndexOf.call(xs, x);
    };
    var slowIndex = function (xs, x) {
      return slowIndexOf(xs, x);
    };
    return pIndexOf === undefined ? slowIndex : fastIndex;
  }();
  var indexOf = function (xs, x) {
    var r = rawIndexOf(xs, x);
    return r === -1 ? Option.none() : Option.some(r);
  };
  var contains = function (xs, x) {
    return rawIndexOf(xs, x) > -1;
  };
  var exists = function (xs, pred) {
    return findIndex(xs, pred).isSome();
  };
  var range = function (num, f) {
    var r = [];
    for (var i = 0; i < num; i++) {
      r.push(f(i));
    }
    return r;
  };
  var chunk = function (array, size) {
    var r = [];
    for (var i = 0; i < array.length; i += size) {
      var s = array.slice(i, i + size);
      r.push(s);
    }
    return r;
  };
  var map = function (xs, f) {
    var len = xs.length;
    var r = new Array(len);
    for (var i = 0; i < len; i++) {
      var x = xs[i];
      r[i] = f(x, i, xs);
    }
    return r;
  };
  var each = function (xs, f) {
    for (var i = 0, len = xs.length; i < len; i++) {
      var x = xs[i];
      f(x, i, xs);
    }
  };
  var eachr = function (xs, f) {
    for (var i = xs.length - 1; i >= 0; i--) {
      var x = xs[i];
      f(x, i, xs);
    }
  };
  var partition = function (xs, pred) {
    var pass = [];
    var fail = [];
    for (var i = 0, len = xs.length; i < len; i++) {
      var x = xs[i];
      var arr = pred(x, i, xs) ? pass : fail;
      arr.push(x);
    }
    return {
      pass: pass,
      fail: fail
    };
  };
  var filter = function (xs, pred) {
    var r = [];
    for (var i = 0, len = xs.length; i < len; i++) {
      var x = xs[i];
      if (pred(x, i, xs)) {
        r.push(x);
      }
    }
    return r;
  };
  var groupBy = function (xs, f) {
    if (xs.length === 0) {
      return [];
    } else {
      var wasType = f(xs[0]);
      var r = [];
      var group = [];
      for (var i = 0, len = xs.length; i < len; i++) {
        var x = xs[i];
        var type = f(x);
        if (type !== wasType) {
          r.push(group);
          group = [];
        }
        wasType = type;
        group.push(x);
      }
      if (group.length !== 0) {
        r.push(group);
      }
      return r;
    }
  };
  var foldr = function (xs, f, acc) {
    eachr(xs, function (x) {
      acc = f(acc, x);
    });
    return acc;
  };
  var foldl = function (xs, f, acc) {
    each(xs, function (x) {
      acc = f(acc, x);
    });
    return acc;
  };
  var find = function (xs, pred) {
    for (var i = 0, len = xs.length; i < len; i++) {
      var x = xs[i];
      if (pred(x, i, xs)) {
        return Option.some(x);
      }
    }
    return Option.none();
  };
  var findIndex = function (xs, pred) {
    for (var i = 0, len = xs.length; i < len; i++) {
      var x = xs[i];
      if (pred(x, i, xs)) {
        return Option.some(i);
      }
    }
    return Option.none();
  };
  var slowIndexOf = function (xs, x) {
    for (var i = 0, len = xs.length; i < len; ++i) {
      if (xs[i] === x) {
        return i;
      }
    }
    return -1;
  };
  var push = Array.prototype.push;
  var flatten = function (xs) {
    var r = [];
    for (var i = 0, len = xs.length; i < len; ++i) {
      if (!Array.prototype.isPrototypeOf(xs[i]))
        throw new Error('Arr.flatten item ' + i + ' was not an array, input: ' + xs);
      push.apply(r, xs[i]);
    }
    return r;
  };
  var bind = function (xs, f) {
    var output = map(xs, f);
    return flatten(output);
  };
  var forall = function (xs, pred) {
    for (var i = 0, len = xs.length; i < len; ++i) {
      var x = xs[i];
      if (pred(x, i, xs) !== true) {
        return false;
      }
    }
    return true;
  };
  var equal = function (a1, a2) {
    return a1.length === a2.length && forall(a1, function (x, i) {
      return x === a2[i];
    });
  };
  var slice = Array.prototype.slice;
  var reverse = function (xs) {
    var r = slice.call(xs, 0);
    r.reverse();
    return r;
  };
  var difference = function (a1, a2) {
    return filter(a1, function (x) {
      return !contains(a2, x);
    });
  };
  var mapToObject = function (xs, f) {
    var r = {};
    for (var i = 0, len = xs.length; i < len; i++) {
      var x = xs[i];
      r[String(x)] = f(x, i);
    }
    return r;
  };
  var pure = function (x) {
    return [x];
  };
  var sort = function (xs, comparator) {
    var copy = slice.call(xs, 0);
    copy.sort(comparator);
    return copy;
  };
  var head = function (xs) {
    return xs.length === 0 ? Option.none() : Option.some(xs[0]);
  };
  var last = function (xs) {
    return xs.length === 0 ? Option.none() : Option.some(xs[xs.length - 1]);
  };
  var from$1 = $_13kw1fk8jfuvixxd.isFunction(Array.from) ? Array.from : function (x) {
    return slice.call(x);
  };
  var $_4jja6kk5jfuvixx1 = {
    map: map,
    each: each,
    eachr: eachr,
    partition: partition,
    filter: filter,
    groupBy: groupBy,
    indexOf: indexOf,
    foldr: foldr,
    foldl: foldl,
    find: find,
    findIndex: findIndex,
    flatten: flatten,
    bind: bind,
    forall: forall,
    exists: exists,
    contains: contains,
    equal: equal,
    reverse: reverse,
    chunk: chunk,
    difference: difference,
    mapToObject: mapToObject,
    pure: pure,
    sort: sort,
    range: range,
    head: head,
    last: last,
    from: from$1
  };

  var keys = function () {
    var fastKeys = Object.keys;
    var slowKeys = function (o) {
      var r = [];
      for (var i in o) {
        if (o.hasOwnProperty(i)) {
          r.push(i);
        }
      }
      return r;
    };
    return fastKeys === undefined ? slowKeys : fastKeys;
  }();
  var each$1 = function (obj, f) {
    var props = keys(obj);
    for (var k = 0, len = props.length; k < len; k++) {
      var i = props[k];
      var x = obj[i];
      f(x, i, obj);
    }
  };
  var objectMap = function (obj, f) {
    return tupleMap(obj, function (x, i, obj) {
      return {
        k: i,
        v: f(x, i, obj)
      };
    });
  };
  var tupleMap = function (obj, f) {
    var r = {};
    each$1(obj, function (x, i) {
      var tuple = f(x, i, obj);
      r[tuple.k] = tuple.v;
    });
    return r;
  };
  var bifilter = function (obj, pred) {
    var t = {};
    var f = {};
    each$1(obj, function (x, i) {
      var branch = pred(x, i) ? t : f;
      branch[i] = x;
    });
    return {
      t: t,
      f: f
    };
  };
  var mapToArray = function (obj, f) {
    var r = [];
    each$1(obj, function (value, name) {
      r.push(f(value, name));
    });
    return r;
  };
  var find$1 = function (obj, pred) {
    var props = keys(obj);
    for (var k = 0, len = props.length; k < len; k++) {
      var i = props[k];
      var x = obj[i];
      if (pred(x, i, obj)) {
        return Option.some(x);
      }
    }
    return Option.none();
  };
  var values = function (obj) {
    return mapToArray(obj, function (v) {
      return v;
    });
  };
  var size = function (obj) {
    return values(obj).length;
  };
  var $_afb9m6kajfuvixy8 = {
    bifilter: bifilter,
    each: each$1,
    map: objectMap,
    mapToArray: mapToArray,
    tupleMap: tupleMap,
    find: find$1,
    keys: keys,
    values: values,
    size: size
  };

  function Immutable () {
    var fields = [];
    for (var _i = 0; _i < arguments.length; _i++) {
      fields[_i] = arguments[_i];
    }
    return function () {
      var values = [];
      for (var _i = 0; _i < arguments.length; _i++) {
        values[_i] = arguments[_i];
      }
      if (fields.length !== values.length) {
        throw new Error('Wrong number of arguments to struct. Expected "[' + fields.length + ']", got ' + values.length + ' arguments');
      }
      var struct = {};
      $_4jja6kk5jfuvixx1.each(fields, function (name, i) {
        struct[name] = $_fdch7uk7jfuvixxb.constant(values[i]);
      });
      return struct;
    };
  }

  var sort$1 = function (arr) {
    return arr.slice(0).sort();
  };
  var reqMessage = function (required, keys) {
    throw new Error('All required keys (' + sort$1(required).join(', ') + ') were not specified. Specified keys were: ' + sort$1(keys).join(', ') + '.');
  };
  var unsuppMessage = function (unsupported) {
    throw new Error('Unsupported keys for object: ' + sort$1(unsupported).join(', '));
  };
  var validateStrArr = function (label, array) {
    if (!$_13kw1fk8jfuvixxd.isArray(array))
      throw new Error('The ' + label + ' fields must be an array. Was: ' + array + '.');
    $_4jja6kk5jfuvixx1.each(array, function (a) {
      if (!$_13kw1fk8jfuvixxd.isString(a))
        throw new Error('The value ' + a + ' in the ' + label + ' fields was not a string.');
    });
  };
  var invalidTypeMessage = function (incorrect, type) {
    throw new Error('All values need to be of type: ' + type + '. Keys (' + sort$1(incorrect).join(', ') + ') were not.');
  };
  var checkDupes = function (everything) {
    var sorted = sort$1(everything);
    var dupe = $_4jja6kk5jfuvixx1.find(sorted, function (s, i) {
      return i < sorted.length - 1 && s === sorted[i + 1];
    });
    dupe.each(function (d) {
      throw new Error('The field: ' + d + ' occurs more than once in the combined fields: [' + sorted.join(', ') + '].');
    });
  };
  var $_cov59pkejfuvixyf = {
    sort: sort$1,
    reqMessage: reqMessage,
    unsuppMessage: unsuppMessage,
    validateStrArr: validateStrArr,
    invalidTypeMessage: invalidTypeMessage,
    checkDupes: checkDupes
  };

  function MixedBag (required, optional) {
    var everything = required.concat(optional);
    if (everything.length === 0)
      throw new Error('You must specify at least one required or optional field.');
    $_cov59pkejfuvixyf.validateStrArr('required', required);
    $_cov59pkejfuvixyf.validateStrArr('optional', optional);
    $_cov59pkejfuvixyf.checkDupes(everything);
    return function (obj) {
      var keys = $_afb9m6kajfuvixy8.keys(obj);
      var allReqd = $_4jja6kk5jfuvixx1.forall(required, function (req) {
        return $_4jja6kk5jfuvixx1.contains(keys, req);
      });
      if (!allReqd)
        $_cov59pkejfuvixyf.reqMessage(required, keys);
      var unsupported = $_4jja6kk5jfuvixx1.filter(keys, function (key) {
        return !$_4jja6kk5jfuvixx1.contains(everything, key);
      });
      if (unsupported.length > 0)
        $_cov59pkejfuvixyf.unsuppMessage(unsupported);
      var r = {};
      $_4jja6kk5jfuvixx1.each(required, function (req) {
        r[req] = $_fdch7uk7jfuvixxb.constant(obj[req]);
      });
      $_4jja6kk5jfuvixx1.each(optional, function (opt) {
        r[opt] = $_fdch7uk7jfuvixxb.constant(Object.prototype.hasOwnProperty.call(obj, opt) ? Option.some(obj[opt]) : Option.none());
      });
      return r;
    };
  }

  var $_96oqrskbjfuvixya = {
    immutable: Immutable,
    immutableBag: MixedBag
  };

  var dimensions = $_96oqrskbjfuvixya.immutable('width', 'height');
  var grid = $_96oqrskbjfuvixya.immutable('rows', 'columns');
  var address = $_96oqrskbjfuvixya.immutable('row', 'column');
  var coords = $_96oqrskbjfuvixya.immutable('x', 'y');
  var detail = $_96oqrskbjfuvixya.immutable('element', 'rowspan', 'colspan');
  var detailnew = $_96oqrskbjfuvixya.immutable('element', 'rowspan', 'colspan', 'isNew');
  var extended = $_96oqrskbjfuvixya.immutable('element', 'rowspan', 'colspan', 'row', 'column');
  var rowdata = $_96oqrskbjfuvixya.immutable('element', 'cells', 'section');
  var elementnew = $_96oqrskbjfuvixya.immutable('element', 'isNew');
  var rowdatanew = $_96oqrskbjfuvixya.immutable('element', 'cells', 'section', 'isNew');
  var rowcells = $_96oqrskbjfuvixya.immutable('cells', 'section');
  var rowdetails = $_96oqrskbjfuvixya.immutable('details', 'section');
  var bounds = $_96oqrskbjfuvixya.immutable('startRow', 'startCol', 'finishRow', 'finishCol');
  var $_g02m1vkgjfuvixyt = {
    dimensions: dimensions,
    grid: grid,
    address: address,
    coords: coords,
    extended: extended,
    detail: detail,
    detailnew: detailnew,
    rowdata: rowdata,
    elementnew: elementnew,
    rowdatanew: rowdatanew,
    rowcells: rowcells,
    rowdetails: rowdetails,
    bounds: bounds
  };

  var fromHtml = function (html, scope) {
    var doc = scope || document;
    var div = doc.createElement('div');
    div.innerHTML = html;
    if (!div.hasChildNodes() || div.childNodes.length > 1) {
      console.error('HTML does not have a single root node', html);
      throw 'HTML must have a single root node';
    }
    return fromDom(div.childNodes[0]);
  };
  var fromTag = function (tag, scope) {
    var doc = scope || document;
    var node = doc.createElement(tag);
    return fromDom(node);
  };
  var fromText = function (text, scope) {
    var doc = scope || document;
    var node = doc.createTextNode(text);
    return fromDom(node);
  };
  var fromDom = function (node) {
    if (node === null || node === undefined)
      throw new Error('Node cannot be null or undefined');
    return { dom: $_fdch7uk7jfuvixxb.constant(node) };
  };
  var fromPoint = function (doc, x, y) {
    return Option.from(doc.dom().elementFromPoint(x, y)).map(fromDom);
  };
  var $_4sdhm4kkjfuviy0e = {
    fromHtml: fromHtml,
    fromTag: fromTag,
    fromText: fromText,
    fromDom: fromDom,
    fromPoint: fromPoint
  };

  var $_9kanxfkljfuviy0l = {
    ATTRIBUTE: 2,
    CDATA_SECTION: 4,
    COMMENT: 8,
    DOCUMENT: 9,
    DOCUMENT_TYPE: 10,
    DOCUMENT_FRAGMENT: 11,
    ELEMENT: 1,
    TEXT: 3,
    PROCESSING_INSTRUCTION: 7,
    ENTITY_REFERENCE: 5,
    ENTITY: 6,
    NOTATION: 12
  };

  var ELEMENT = $_9kanxfkljfuviy0l.ELEMENT;
  var DOCUMENT = $_9kanxfkljfuviy0l.DOCUMENT;
  var is = function (element, selector) {
    var elem = element.dom();
    if (elem.nodeType !== ELEMENT)
      return false;
    else if (elem.matches !== undefined)
      return elem.matches(selector);
    else if (elem.msMatchesSelector !== undefined)
      return elem.msMatchesSelector(selector);
    else if (elem.webkitMatchesSelector !== undefined)
      return elem.webkitMatchesSelector(selector);
    else if (elem.mozMatchesSelector !== undefined)
      return elem.mozMatchesSelector(selector);
    else
      throw new Error('Browser lacks native selectors');
  };
  var bypassSelector = function (dom) {
    return dom.nodeType !== ELEMENT && dom.nodeType !== DOCUMENT || dom.childElementCount === 0;
  };
  var all = function (selector, scope) {
    var base = scope === undefined ? document : scope.dom();
    return bypassSelector(base) ? [] : $_4jja6kk5jfuvixx1.map(base.querySelectorAll(selector), $_4sdhm4kkjfuviy0e.fromDom);
  };
  var one = function (selector, scope) {
    var base = scope === undefined ? document : scope.dom();
    return bypassSelector(base) ? Option.none() : Option.from(base.querySelector(selector)).map($_4sdhm4kkjfuviy0e.fromDom);
  };
  var $_aphf8fkjjfuviy04 = {
    all: all,
    is: is,
    one: one
  };

  var toArray = function (target, f) {
    var r = [];
    var recurse = function (e) {
      r.push(e);
      return f(e);
    };
    var cur = f(target);
    do {
      cur = cur.bind(recurse);
    } while (cur.isSome());
    return r;
  };
  var $_a5zu42knjfuviy12 = { toArray: toArray };

  var global$1 = typeof window !== 'undefined' ? window : Function('return this;')();

  var path = function (parts, scope) {
    var o = scope !== undefined && scope !== null ? scope : global$1;
    for (var i = 0; i < parts.length && o !== undefined && o !== null; ++i)
      o = o[parts[i]];
    return o;
  };
  var resolve = function (p, scope) {
    var parts = p.split('.');
    return path(parts, scope);
  };
  var step = function (o, part) {
    if (o[part] === undefined || o[part] === null)
      o[part] = {};
    return o[part];
  };
  var forge = function (parts, target) {
    var o = target !== undefined ? target : global$1;
    for (var i = 0; i < parts.length; ++i)
      o = step(o, parts[i]);
    return o;
  };
  var namespace = function (name, target) {
    var parts = name.split('.');
    return forge(parts, target);
  };
  var $_8sz23skrjfuviy1k = {
    path: path,
    resolve: resolve,
    forge: forge,
    namespace: namespace
  };

  var unsafe = function (name, scope) {
    return $_8sz23skrjfuviy1k.resolve(name, scope);
  };
  var getOrDie = function (name, scope) {
    var actual = unsafe(name, scope);
    if (actual === undefined || actual === null)
      throw name + ' not available on this browser';
    return actual;
  };
  var $_9yhrxfkqjfuviy1h = { getOrDie: getOrDie };

  var node = function () {
    var f = $_9yhrxfkqjfuviy1h.getOrDie('Node');
    return f;
  };
  var compareDocumentPosition = function (a, b, match) {
    return (a.compareDocumentPosition(b) & match) !== 0;
  };
  var documentPositionPreceding = function (a, b) {
    return compareDocumentPosition(a, b, node().DOCUMENT_POSITION_PRECEDING);
  };
  var documentPositionContainedBy = function (a, b) {
    return compareDocumentPosition(a, b, node().DOCUMENT_POSITION_CONTAINED_BY);
  };
  var $_af30gpkpjfuviy1g = {
    documentPositionPreceding: documentPositionPreceding,
    documentPositionContainedBy: documentPositionContainedBy
  };

  var cached = function (f) {
    var called = false;
    var r;
    return function () {
      if (!called) {
        called = true;
        r = f.apply(null, arguments);
      }
      return r;
    };
  };
  var $_eprzwrkujfuviy1p = { cached: cached };

  var firstMatch = function (regexes, s) {
    for (var i = 0; i < regexes.length; i++) {
      var x = regexes[i];
      if (x.test(s))
        return x;
    }
    return undefined;
  };
  var find$2 = function (regexes, agent) {
    var r = firstMatch(regexes, agent);
    if (!r)
      return {
        major: 0,
        minor: 0
      };
    var group = function (i) {
      return Number(agent.replace(r, '$' + i));
    };
    return nu(group(1), group(2));
  };
  var detect = function (versionRegexes, agent) {
    var cleanedAgent = String(agent).toLowerCase();
    if (versionRegexes.length === 0)
      return unknown();
    return find$2(versionRegexes, cleanedAgent);
  };
  var unknown = function () {
    return nu(0, 0);
  };
  var nu = function (major, minor) {
    return {
      major: major,
      minor: minor
    };
  };
  var $_9sqjpfkxjfuviy1v = {
    nu: nu,
    detect: detect,
    unknown: unknown
  };

  var edge = 'Edge';
  var chrome = 'Chrome';
  var ie = 'IE';
  var opera = 'Opera';
  var firefox = 'Firefox';
  var safari = 'Safari';
  var isBrowser = function (name, current) {
    return function () {
      return current === name;
    };
  };
  var unknown$1 = function () {
    return nu$1({
      current: undefined,
      version: $_9sqjpfkxjfuviy1v.unknown()
    });
  };
  var nu$1 = function (info) {
    var current = info.current;
    var version = info.version;
    return {
      current: current,
      version: version,
      isEdge: isBrowser(edge, current),
      isChrome: isBrowser(chrome, current),
      isIE: isBrowser(ie, current),
      isOpera: isBrowser(opera, current),
      isFirefox: isBrowser(firefox, current),
      isSafari: isBrowser(safari, current)
    };
  };
  var $_c33ahakwjfuviy1s = {
    unknown: unknown$1,
    nu: nu$1,
    edge: $_fdch7uk7jfuvixxb.constant(edge),
    chrome: $_fdch7uk7jfuvixxb.constant(chrome),
    ie: $_fdch7uk7jfuvixxb.constant(ie),
    opera: $_fdch7uk7jfuvixxb.constant(opera),
    firefox: $_fdch7uk7jfuvixxb.constant(firefox),
    safari: $_fdch7uk7jfuvixxb.constant(safari)
  };

  var windows = 'Windows';
  var ios = 'iOS';
  var android = 'Android';
  var linux = 'Linux';
  var osx = 'OSX';
  var solaris = 'Solaris';
  var freebsd = 'FreeBSD';
  var isOS = function (name, current) {
    return function () {
      return current === name;
    };
  };
  var unknown$2 = function () {
    return nu$2({
      current: undefined,
      version: $_9sqjpfkxjfuviy1v.unknown()
    });
  };
  var nu$2 = function (info) {
    var current = info.current;
    var version = info.version;
    return {
      current: current,
      version: version,
      isWindows: isOS(windows, current),
      isiOS: isOS(ios, current),
      isAndroid: isOS(android, current),
      isOSX: isOS(osx, current),
      isLinux: isOS(linux, current),
      isSolaris: isOS(solaris, current),
      isFreeBSD: isOS(freebsd, current)
    };
  };
  var $_frrfmikyjfuviy1w = {
    unknown: unknown$2,
    nu: nu$2,
    windows: $_fdch7uk7jfuvixxb.constant(windows),
    ios: $_fdch7uk7jfuvixxb.constant(ios),
    android: $_fdch7uk7jfuvixxb.constant(android),
    linux: $_fdch7uk7jfuvixxb.constant(linux),
    osx: $_fdch7uk7jfuvixxb.constant(osx),
    solaris: $_fdch7uk7jfuvixxb.constant(solaris),
    freebsd: $_fdch7uk7jfuvixxb.constant(freebsd)
  };

  function DeviceType (os, browser, userAgent) {
    var isiPad = os.isiOS() && /ipad/i.test(userAgent) === true;
    var isiPhone = os.isiOS() && !isiPad;
    var isAndroid3 = os.isAndroid() && os.version.major === 3;
    var isAndroid4 = os.isAndroid() && os.version.major === 4;
    var isTablet = isiPad || isAndroid3 || isAndroid4 && /mobile/i.test(userAgent) === true;
    var isTouch = os.isiOS() || os.isAndroid();
    var isPhone = isTouch && !isTablet;
    var iOSwebview = browser.isSafari() && os.isiOS() && /safari/i.test(userAgent) === false;
    return {
      isiPad: $_fdch7uk7jfuvixxb.constant(isiPad),
      isiPhone: $_fdch7uk7jfuvixxb.constant(isiPhone),
      isTablet: $_fdch7uk7jfuvixxb.constant(isTablet),
      isPhone: $_fdch7uk7jfuvixxb.constant(isPhone),
      isTouch: $_fdch7uk7jfuvixxb.constant(isTouch),
      isAndroid: os.isAndroid,
      isiOS: os.isiOS,
      isWebView: $_fdch7uk7jfuvixxb.constant(iOSwebview)
    };
  }

  var detect$1 = function (candidates, userAgent) {
    var agent = String(userAgent).toLowerCase();
    return $_4jja6kk5jfuvixx1.find(candidates, function (candidate) {
      return candidate.search(agent);
    });
  };
  var detectBrowser = function (browsers, userAgent) {
    return detect$1(browsers, userAgent).map(function (browser) {
      var version = $_9sqjpfkxjfuviy1v.detect(browser.versionRegexes, userAgent);
      return {
        current: browser.name,
        version: version
      };
    });
  };
  var detectOs = function (oses, userAgent) {
    return detect$1(oses, userAgent).map(function (os) {
      var version = $_9sqjpfkxjfuviy1v.detect(os.versionRegexes, userAgent);
      return {
        current: os.name,
        version: version
      };
    });
  };
  var $_96kc15l0jfuviy23 = {
    detectBrowser: detectBrowser,
    detectOs: detectOs
  };

  var addToStart = function (str, prefix) {
    return prefix + str;
  };
  var addToEnd = function (str, suffix) {
    return str + suffix;
  };
  var removeFromStart = function (str, numChars) {
    return str.substring(numChars);
  };
  var removeFromEnd = function (str, numChars) {
    return str.substring(0, str.length - numChars);
  };
  var $_2zxn62l3jfuviy2i = {
    addToStart: addToStart,
    addToEnd: addToEnd,
    removeFromStart: removeFromStart,
    removeFromEnd: removeFromEnd
  };

  var first = function (str, count) {
    return str.substr(0, count);
  };
  var last$1 = function (str, count) {
    return str.substr(str.length - count, str.length);
  };
  var head$1 = function (str) {
    return str === '' ? Option.none() : Option.some(str.substr(0, 1));
  };
  var tail = function (str) {
    return str === '' ? Option.none() : Option.some(str.substring(1));
  };
  var $_52rhkfl4jfuviy2j = {
    first: first,
    last: last$1,
    head: head$1,
    tail: tail
  };

  var checkRange = function (str, substr, start) {
    if (substr === '')
      return true;
    if (str.length < substr.length)
      return false;
    var x = str.substr(start, start + substr.length);
    return x === substr;
  };
  var supplant = function (str, obj) {
    var isStringOrNumber = function (a) {
      var t = typeof a;
      return t === 'string' || t === 'number';
    };
    return str.replace(/\${([^{}]*)}/g, function (a, b) {
      var value = obj[b];
      return isStringOrNumber(value) ? value : a;
    });
  };
  var removeLeading = function (str, prefix) {
    return startsWith(str, prefix) ? $_2zxn62l3jfuviy2i.removeFromStart(str, prefix.length) : str;
  };
  var removeTrailing = function (str, prefix) {
    return endsWith(str, prefix) ? $_2zxn62l3jfuviy2i.removeFromEnd(str, prefix.length) : str;
  };
  var ensureLeading = function (str, prefix) {
    return startsWith(str, prefix) ? str : $_2zxn62l3jfuviy2i.addToStart(str, prefix);
  };
  var ensureTrailing = function (str, prefix) {
    return endsWith(str, prefix) ? str : $_2zxn62l3jfuviy2i.addToEnd(str, prefix);
  };
  var contains$1 = function (str, substr) {
    return str.indexOf(substr) !== -1;
  };
  var capitalize = function (str) {
    return $_52rhkfl4jfuviy2j.head(str).bind(function (head) {
      return $_52rhkfl4jfuviy2j.tail(str).map(function (tail) {
        return head.toUpperCase() + tail;
      });
    }).getOr(str);
  };
  var startsWith = function (str, prefix) {
    return checkRange(str, prefix, 0);
  };
  var endsWith = function (str, suffix) {
    return checkRange(str, suffix, str.length - suffix.length);
  };
  var trim = function (str) {
    return str.replace(/^\s+|\s+$/g, '');
  };
  var lTrim = function (str) {
    return str.replace(/^\s+/g, '');
  };
  var rTrim = function (str) {
    return str.replace(/\s+$/g, '');
  };
  var $_2rmckll2jfuviy2d = {
    supplant: supplant,
    startsWith: startsWith,
    removeLeading: removeLeading,
    removeTrailing: removeTrailing,
    ensureLeading: ensureLeading,
    ensureTrailing: ensureTrailing,
    endsWith: endsWith,
    contains: contains$1,
    trim: trim,
    lTrim: lTrim,
    rTrim: rTrim,
    capitalize: capitalize
  };

  var normalVersionRegex = /.*?version\/\ ?([0-9]+)\.([0-9]+).*/;
  var checkContains = function (target) {
    return function (uastring) {
      return $_2rmckll2jfuviy2d.contains(uastring, target);
    };
  };
  var browsers = [
    {
      name: 'Edge',
      versionRegexes: [/.*?edge\/ ?([0-9]+)\.([0-9]+)$/],
      search: function (uastring) {
        var monstrosity = $_2rmckll2jfuviy2d.contains(uastring, 'edge/') && $_2rmckll2jfuviy2d.contains(uastring, 'chrome') && $_2rmckll2jfuviy2d.contains(uastring, 'safari') && $_2rmckll2jfuviy2d.contains(uastring, 'applewebkit');
        return monstrosity;
      }
    },
    {
      name: 'Chrome',
      versionRegexes: [
        /.*?chrome\/([0-9]+)\.([0-9]+).*/,
        normalVersionRegex
      ],
      search: function (uastring) {
        return $_2rmckll2jfuviy2d.contains(uastring, 'chrome') && !$_2rmckll2jfuviy2d.contains(uastring, 'chromeframe');
      }
    },
    {
      name: 'IE',
      versionRegexes: [
        /.*?msie\ ?([0-9]+)\.([0-9]+).*/,
        /.*?rv:([0-9]+)\.([0-9]+).*/
      ],
      search: function (uastring) {
        return $_2rmckll2jfuviy2d.contains(uastring, 'msie') || $_2rmckll2jfuviy2d.contains(uastring, 'trident');
      }
    },
    {
      name: 'Opera',
      versionRegexes: [
        normalVersionRegex,
        /.*?opera\/([0-9]+)\.([0-9]+).*/
      ],
      search: checkContains('opera')
    },
    {
      name: 'Firefox',
      versionRegexes: [/.*?firefox\/\ ?([0-9]+)\.([0-9]+).*/],
      search: checkContains('firefox')
    },
    {
      name: 'Safari',
      versionRegexes: [
        normalVersionRegex,
        /.*?cpu os ([0-9]+)_([0-9]+).*/
      ],
      search: function (uastring) {
        return ($_2rmckll2jfuviy2d.contains(uastring, 'safari') || $_2rmckll2jfuviy2d.contains(uastring, 'mobile/')) && $_2rmckll2jfuviy2d.contains(uastring, 'applewebkit');
      }
    }
  ];
  var oses = [
    {
      name: 'Windows',
      search: checkContains('win'),
      versionRegexes: [/.*?windows\ nt\ ?([0-9]+)\.([0-9]+).*/]
    },
    {
      name: 'iOS',
      search: function (uastring) {
        return $_2rmckll2jfuviy2d.contains(uastring, 'iphone') || $_2rmckll2jfuviy2d.contains(uastring, 'ipad');
      },
      versionRegexes: [
        /.*?version\/\ ?([0-9]+)\.([0-9]+).*/,
        /.*cpu os ([0-9]+)_([0-9]+).*/,
        /.*cpu iphone os ([0-9]+)_([0-9]+).*/
      ]
    },
    {
      name: 'Android',
      search: checkContains('android'),
      versionRegexes: [/.*?android\ ?([0-9]+)\.([0-9]+).*/]
    },
    {
      name: 'OSX',
      search: checkContains('os x'),
      versionRegexes: [/.*?os\ x\ ?([0-9]+)_([0-9]+).*/]
    },
    {
      name: 'Linux',
      search: checkContains('linux'),
      versionRegexes: []
    },
    {
      name: 'Solaris',
      search: checkContains('sunos'),
      versionRegexes: []
    },
    {
      name: 'FreeBSD',
      search: checkContains('freebsd'),
      versionRegexes: []
    }
  ];
  var $_5ncirnl1jfuviy27 = {
    browsers: $_fdch7uk7jfuvixxb.constant(browsers),
    oses: $_fdch7uk7jfuvixxb.constant(oses)
  };

  var detect$2 = function (userAgent) {
    var browsers = $_5ncirnl1jfuviy27.browsers();
    var oses = $_5ncirnl1jfuviy27.oses();
    var browser = $_96kc15l0jfuviy23.detectBrowser(browsers, userAgent).fold($_c33ahakwjfuviy1s.unknown, $_c33ahakwjfuviy1s.nu);
    var os = $_96kc15l0jfuviy23.detectOs(oses, userAgent).fold($_frrfmikyjfuviy1w.unknown, $_frrfmikyjfuviy1w.nu);
    var deviceType = DeviceType(os, browser, userAgent);
    return {
      browser: browser,
      os: os,
      deviceType: deviceType
    };
  };
  var $_9d5oodkvjfuviy1q = { detect: detect$2 };

  var detect$3 = $_eprzwrkujfuviy1p.cached(function () {
    var userAgent = navigator.userAgent;
    return $_9d5oodkvjfuviy1q.detect(userAgent);
  });
  var $_8chrc7ktjfuviy1m = { detect: detect$3 };

  var eq = function (e1, e2) {
    return e1.dom() === e2.dom();
  };
  var isEqualNode = function (e1, e2) {
    return e1.dom().isEqualNode(e2.dom());
  };
  var member = function (element, elements) {
    return $_4jja6kk5jfuvixx1.exists(elements, $_fdch7uk7jfuvixxb.curry(eq, element));
  };
  var regularContains = function (e1, e2) {
    var d1 = e1.dom(), d2 = e2.dom();
    return d1 === d2 ? false : d1.contains(d2);
  };
  var ieContains = function (e1, e2) {
    return $_af30gpkpjfuviy1g.documentPositionContainedBy(e1.dom(), e2.dom());
  };
  var browser = $_8chrc7ktjfuviy1m.detect().browser;
  var contains$2 = browser.isIE() ? ieContains : regularContains;
  var $_g6ztqikojfuviy13 = {
    eq: eq,
    isEqualNode: isEqualNode,
    member: member,
    contains: contains$2,
    is: $_aphf8fkjjfuviy04.is
  };

  var owner = function (element) {
    return $_4sdhm4kkjfuviy0e.fromDom(element.dom().ownerDocument);
  };
  var documentElement = function (element) {
    var doc = owner(element);
    return $_4sdhm4kkjfuviy0e.fromDom(doc.dom().documentElement);
  };
  var defaultView = function (element) {
    var el = element.dom();
    var defaultView = el.ownerDocument.defaultView;
    return $_4sdhm4kkjfuviy0e.fromDom(defaultView);
  };
  var parent = function (element) {
    var dom = element.dom();
    return Option.from(dom.parentNode).map($_4sdhm4kkjfuviy0e.fromDom);
  };
  var findIndex$1 = function (element) {
    return parent(element).bind(function (p) {
      var kin = children(p);
      return $_4jja6kk5jfuvixx1.findIndex(kin, function (elem) {
        return $_g6ztqikojfuviy13.eq(element, elem);
      });
    });
  };
  var parents = function (element, isRoot) {
    var stop = $_13kw1fk8jfuvixxd.isFunction(isRoot) ? isRoot : $_fdch7uk7jfuvixxb.constant(false);
    var dom = element.dom();
    var ret = [];
    while (dom.parentNode !== null && dom.parentNode !== undefined) {
      var rawParent = dom.parentNode;
      var parent = $_4sdhm4kkjfuviy0e.fromDom(rawParent);
      ret.push(parent);
      if (stop(parent) === true)
        break;
      else
        dom = rawParent;
    }
    return ret;
  };
  var siblings = function (element) {
    var filterSelf = function (elements) {
      return $_4jja6kk5jfuvixx1.filter(elements, function (x) {
        return !$_g6ztqikojfuviy13.eq(element, x);
      });
    };
    return parent(element).map(children).map(filterSelf).getOr([]);
  };
  var offsetParent = function (element) {
    var dom = element.dom();
    return Option.from(dom.offsetParent).map($_4sdhm4kkjfuviy0e.fromDom);
  };
  var prevSibling = function (element) {
    var dom = element.dom();
    return Option.from(dom.previousSibling).map($_4sdhm4kkjfuviy0e.fromDom);
  };
  var nextSibling = function (element) {
    var dom = element.dom();
    return Option.from(dom.nextSibling).map($_4sdhm4kkjfuviy0e.fromDom);
  };
  var prevSiblings = function (element) {
    return $_4jja6kk5jfuvixx1.reverse($_a5zu42knjfuviy12.toArray(element, prevSibling));
  };
  var nextSiblings = function (element) {
    return $_a5zu42knjfuviy12.toArray(element, nextSibling);
  };
  var children = function (element) {
    var dom = element.dom();
    return $_4jja6kk5jfuvixx1.map(dom.childNodes, $_4sdhm4kkjfuviy0e.fromDom);
  };
  var child = function (element, index) {
    var children = element.dom().childNodes;
    return Option.from(children[index]).map($_4sdhm4kkjfuviy0e.fromDom);
  };
  var firstChild = function (element) {
    return child(element, 0);
  };
  var lastChild = function (element) {
    return child(element, element.dom().childNodes.length - 1);
  };
  var childNodesCount = function (element) {
    return element.dom().childNodes.length;
  };
  var hasChildNodes = function (element) {
    return element.dom().hasChildNodes();
  };
  var spot = $_96oqrskbjfuvixya.immutable('element', 'offset');
  var leaf = function (element, offset) {
    var cs = children(element);
    return cs.length > 0 && offset < cs.length ? spot(cs[offset], 0) : spot(element, offset);
  };
  var $_87w3h3kmjfuviy0m = {
    owner: owner,
    defaultView: defaultView,
    documentElement: documentElement,
    parent: parent,
    findIndex: findIndex$1,
    parents: parents,
    siblings: siblings,
    prevSibling: prevSibling,
    offsetParent: offsetParent,
    prevSiblings: prevSiblings,
    nextSibling: nextSibling,
    nextSiblings: nextSiblings,
    children: children,
    child: child,
    firstChild: firstChild,
    lastChild: lastChild,
    childNodesCount: childNodesCount,
    hasChildNodes: hasChildNodes,
    leaf: leaf
  };

  var firstLayer = function (scope, selector) {
    return filterFirstLayer(scope, selector, $_fdch7uk7jfuvixxb.constant(true));
  };
  var filterFirstLayer = function (scope, selector, predicate) {
    return $_4jja6kk5jfuvixx1.bind($_87w3h3kmjfuviy0m.children(scope), function (x) {
      return $_aphf8fkjjfuviy04.is(x, selector) ? predicate(x) ? [x] : [] : filterFirstLayer(x, selector, predicate);
    });
  };
  var $_f9na0ikijfuvixzq = {
    firstLayer: firstLayer,
    filterFirstLayer: filterFirstLayer
  };

  var name = function (element) {
    var r = element.dom().nodeName;
    return r.toLowerCase();
  };
  var type = function (element) {
    return element.dom().nodeType;
  };
  var value = function (element) {
    return element.dom().nodeValue;
  };
  var isType$1 = function (t) {
    return function (element) {
      return type(element) === t;
    };
  };
  var isComment = function (element) {
    return type(element) === $_9kanxfkljfuviy0l.COMMENT || name(element) === '#comment';
  };
  var isElement = isType$1($_9kanxfkljfuviy0l.ELEMENT);
  var isText = isType$1($_9kanxfkljfuviy0l.TEXT);
  var isDocument = isType$1($_9kanxfkljfuviy0l.DOCUMENT);
  var $_6mcqmml6jfuviy2u = {
    name: name,
    type: type,
    value: value,
    isElement: isElement,
    isText: isText,
    isDocument: isDocument,
    isComment: isComment
  };

  var rawSet = function (dom, key, value) {
    if ($_13kw1fk8jfuvixxd.isString(value) || $_13kw1fk8jfuvixxd.isBoolean(value) || $_13kw1fk8jfuvixxd.isNumber(value)) {
      dom.setAttribute(key, value + '');
    } else {
      console.error('Invalid call to Attr.set. Key ', key, ':: Value ', value, ':: Element ', dom);
      throw new Error('Attribute value was not simple');
    }
  };
  var set = function (element, key, value) {
    rawSet(element.dom(), key, value);
  };
  var setAll = function (element, attrs) {
    var dom = element.dom();
    $_afb9m6kajfuvixy8.each(attrs, function (v, k) {
      rawSet(dom, k, v);
    });
  };
  var get = function (element, key) {
    var v = element.dom().getAttribute(key);
    return v === null ? undefined : v;
  };
  var has = function (element, key) {
    var dom = element.dom();
    return dom && dom.hasAttribute ? dom.hasAttribute(key) : false;
  };
  var remove = function (element, key) {
    element.dom().removeAttribute(key);
  };
  var hasNone = function (element) {
    var attrs = element.dom().attributes;
    return attrs === undefined || attrs === null || attrs.length === 0;
  };
  var clone = function (element) {
    return $_4jja6kk5jfuvixx1.foldl(element.dom().attributes, function (acc, attr) {
      acc[attr.name] = attr.value;
      return acc;
    }, {});
  };
  var transferOne = function (source, destination, attr) {
    if (has(source, attr) && !has(destination, attr))
      set(destination, attr, get(source, attr));
  };
  var transfer = function (source, destination, attrs) {
    if (!$_6mcqmml6jfuviy2u.isElement(source) || !$_6mcqmml6jfuviy2u.isElement(destination))
      return;
    $_4jja6kk5jfuvixx1.each(attrs, function (attr) {
      transferOne(source, destination, attr);
    });
  };
  var $_2ekobel5jfuviy2m = {
    clone: clone,
    set: set,
    setAll: setAll,
    get: get,
    has: has,
    remove: remove,
    hasNone: hasNone,
    transfer: transfer
  };

  var inBody = function (element) {
    var dom = $_6mcqmml6jfuviy2u.isText(element) ? element.dom().parentNode : element.dom();
    return dom !== undefined && dom !== null && dom.ownerDocument.body.contains(dom);
  };
  var body = $_eprzwrkujfuviy1p.cached(function () {
    return getBody($_4sdhm4kkjfuviy0e.fromDom(document));
  });
  var getBody = function (doc) {
    var body = doc.dom().body;
    if (body === null || body === undefined)
      throw 'Body is not available yet';
    return $_4sdhm4kkjfuviy0e.fromDom(body);
  };
  var $_43dxxcl9jfuviy31 = {
    body: body,
    getBody: getBody,
    inBody: inBody
  };

  var all$1 = function (predicate) {
    return descendants($_43dxxcl9jfuviy31.body(), predicate);
  };
  var ancestors = function (scope, predicate, isRoot) {
    return $_4jja6kk5jfuvixx1.filter($_87w3h3kmjfuviy0m.parents(scope, isRoot), predicate);
  };
  var siblings$1 = function (scope, predicate) {
    return $_4jja6kk5jfuvixx1.filter($_87w3h3kmjfuviy0m.siblings(scope), predicate);
  };
  var children$1 = function (scope, predicate) {
    return $_4jja6kk5jfuvixx1.filter($_87w3h3kmjfuviy0m.children(scope), predicate);
  };
  var descendants = function (scope, predicate) {
    var result = [];
    $_4jja6kk5jfuvixx1.each($_87w3h3kmjfuviy0m.children(scope), function (x) {
      if (predicate(x)) {
        result = result.concat([x]);
      }
      result = result.concat(descendants(x, predicate));
    });
    return result;
  };
  var $_g5dooll8jfuviy2x = {
    all: all$1,
    ancestors: ancestors,
    siblings: siblings$1,
    children: children$1,
    descendants: descendants
  };

  var all$2 = function (selector) {
    return $_aphf8fkjjfuviy04.all(selector);
  };
  var ancestors$1 = function (scope, selector, isRoot) {
    return $_g5dooll8jfuviy2x.ancestors(scope, function (e) {
      return $_aphf8fkjjfuviy04.is(e, selector);
    }, isRoot);
  };
  var siblings$2 = function (scope, selector) {
    return $_g5dooll8jfuviy2x.siblings(scope, function (e) {
      return $_aphf8fkjjfuviy04.is(e, selector);
    });
  };
  var children$2 = function (scope, selector) {
    return $_g5dooll8jfuviy2x.children(scope, function (e) {
      return $_aphf8fkjjfuviy04.is(e, selector);
    });
  };
  var descendants$1 = function (scope, selector) {
    return $_aphf8fkjjfuviy04.all(selector, scope);
  };
  var $_a3hs1bl7jfuviy2w = {
    all: all$2,
    ancestors: ancestors$1,
    siblings: siblings$2,
    children: children$2,
    descendants: descendants$1
  };

  function ClosestOrAncestor (is, ancestor, scope, a, isRoot) {
    return is(scope, a) ? Option.some(scope) : $_13kw1fk8jfuvixxd.isFunction(isRoot) && isRoot(scope) ? Option.none() : ancestor(scope, a, isRoot);
  }

  var first$1 = function (predicate) {
    return descendant($_43dxxcl9jfuviy31.body(), predicate);
  };
  var ancestor = function (scope, predicate, isRoot) {
    var element = scope.dom();
    var stop = $_13kw1fk8jfuvixxd.isFunction(isRoot) ? isRoot : $_fdch7uk7jfuvixxb.constant(false);
    while (element.parentNode) {
      element = element.parentNode;
      var el = $_4sdhm4kkjfuviy0e.fromDom(element);
      if (predicate(el))
        return Option.some(el);
      else if (stop(el))
        break;
    }
    return Option.none();
  };
  var closest = function (scope, predicate, isRoot) {
    var is = function (scope) {
      return predicate(scope);
    };
    return ClosestOrAncestor(is, ancestor, scope, predicate, isRoot);
  };
  var sibling = function (scope, predicate) {
    var element = scope.dom();
    if (!element.parentNode)
      return Option.none();
    return child$1($_4sdhm4kkjfuviy0e.fromDom(element.parentNode), function (x) {
      return !$_g6ztqikojfuviy13.eq(scope, x) && predicate(x);
    });
  };
  var child$1 = function (scope, predicate) {
    var result = $_4jja6kk5jfuvixx1.find(scope.dom().childNodes, $_fdch7uk7jfuvixxb.compose(predicate, $_4sdhm4kkjfuviy0e.fromDom));
    return result.map($_4sdhm4kkjfuviy0e.fromDom);
  };
  var descendant = function (scope, predicate) {
    var descend = function (element) {
      for (var i = 0; i < element.childNodes.length; i++) {
        if (predicate($_4sdhm4kkjfuviy0e.fromDom(element.childNodes[i])))
          return Option.some($_4sdhm4kkjfuviy0e.fromDom(element.childNodes[i]));
        var res = descend(element.childNodes[i]);
        if (res.isSome())
          return res;
      }
      return Option.none();
    };
    return descend(scope.dom());
  };
  var $_eg4f87lbjfuviy37 = {
    first: first$1,
    ancestor: ancestor,
    closest: closest,
    sibling: sibling,
    child: child$1,
    descendant: descendant
  };

  var first$2 = function (selector) {
    return $_aphf8fkjjfuviy04.one(selector);
  };
  var ancestor$1 = function (scope, selector, isRoot) {
    return $_eg4f87lbjfuviy37.ancestor(scope, function (e) {
      return $_aphf8fkjjfuviy04.is(e, selector);
    }, isRoot);
  };
  var sibling$1 = function (scope, selector) {
    return $_eg4f87lbjfuviy37.sibling(scope, function (e) {
      return $_aphf8fkjjfuviy04.is(e, selector);
    });
  };
  var child$2 = function (scope, selector) {
    return $_eg4f87lbjfuviy37.child(scope, function (e) {
      return $_aphf8fkjjfuviy04.is(e, selector);
    });
  };
  var descendant$1 = function (scope, selector) {
    return $_aphf8fkjjfuviy04.one(selector, scope);
  };
  var closest$1 = function (scope, selector, isRoot) {
    return ClosestOrAncestor($_aphf8fkjjfuviy04.is, ancestor$1, scope, selector, isRoot);
  };
  var $_26gnp6lajfuviy35 = {
    first: first$2,
    ancestor: ancestor$1,
    sibling: sibling$1,
    child: child$2,
    descendant: descendant$1,
    closest: closest$1
  };

  var lookup = function (tags, element, _isRoot) {
    var isRoot = _isRoot !== undefined ? _isRoot : $_fdch7uk7jfuvixxb.constant(false);
    if (isRoot(element))
      return Option.none();
    if ($_4jja6kk5jfuvixx1.contains(tags, $_6mcqmml6jfuviy2u.name(element)))
      return Option.some(element);
    var isRootOrUpperTable = function (element) {
      return $_aphf8fkjjfuviy04.is(element, 'table') || isRoot(element);
    };
    return $_26gnp6lajfuviy35.ancestor(element, tags.join(','), isRootOrUpperTable);
  };
  var cell = function (element, isRoot) {
    return lookup([
      'td',
      'th'
    ], element, isRoot);
  };
  var cells = function (ancestor) {
    return $_f9na0ikijfuvixzq.firstLayer(ancestor, 'th,td');
  };
  var notCell = function (element, isRoot) {
    return lookup([
      'caption',
      'tr',
      'tbody',
      'tfoot',
      'thead'
    ], element, isRoot);
  };
  var neighbours = function (selector, element) {
    return $_87w3h3kmjfuviy0m.parent(element).map(function (parent) {
      return $_a3hs1bl7jfuviy2w.children(parent, selector);
    });
  };
  var neighbourCells = $_fdch7uk7jfuvixxb.curry(neighbours, 'th,td');
  var neighbourRows = $_fdch7uk7jfuvixxb.curry(neighbours, 'tr');
  var firstCell = function (ancestor) {
    return $_26gnp6lajfuviy35.descendant(ancestor, 'th,td');
  };
  var table = function (element, isRoot) {
    return $_26gnp6lajfuviy35.closest(element, 'table', isRoot);
  };
  var row = function (element, isRoot) {
    return lookup(['tr'], element, isRoot);
  };
  var rows = function (ancestor) {
    return $_f9na0ikijfuvixzq.firstLayer(ancestor, 'tr');
  };
  var attr = function (element, property) {
    return parseInt($_2ekobel5jfuviy2m.get(element, property), 10);
  };
  var grid$1 = function (element, rowProp, colProp) {
    var rows = attr(element, rowProp);
    var cols = attr(element, colProp);
    return $_g02m1vkgjfuvixyt.grid(rows, cols);
  };
  var $_dmqxswkhjfuvixyz = {
    cell: cell,
    firstCell: firstCell,
    cells: cells,
    neighbourCells: neighbourCells,
    table: table,
    row: row,
    rows: rows,
    notCell: notCell,
    neighbourRows: neighbourRows,
    attr: attr,
    grid: grid$1
  };

  var fromTable = function (table) {
    var rows = $_dmqxswkhjfuvixyz.rows(table);
    return $_4jja6kk5jfuvixx1.map(rows, function (row) {
      var element = row;
      var parent = $_87w3h3kmjfuviy0m.parent(element);
      var parentSection = parent.bind(function (parent) {
        var parentName = $_6mcqmml6jfuviy2u.name(parent);
        return parentName === 'tfoot' || parentName === 'thead' || parentName === 'tbody' ? parentName : 'tbody';
      });
      var cells = $_4jja6kk5jfuvixx1.map($_dmqxswkhjfuvixyz.cells(row), function (cell) {
        var rowspan = $_2ekobel5jfuviy2m.has(cell, 'rowspan') ? parseInt($_2ekobel5jfuviy2m.get(cell, 'rowspan'), 10) : 1;
        var colspan = $_2ekobel5jfuviy2m.has(cell, 'colspan') ? parseInt($_2ekobel5jfuviy2m.get(cell, 'colspan'), 10) : 1;
        return $_g02m1vkgjfuvixyt.detail(cell, rowspan, colspan);
      });
      return $_g02m1vkgjfuvixyt.rowdata(element, cells, parentSection);
    });
  };
  var fromPastedRows = function (rows, example) {
    return $_4jja6kk5jfuvixx1.map(rows, function (row) {
      var cells = $_4jja6kk5jfuvixx1.map($_dmqxswkhjfuvixyz.cells(row), function (cell) {
        var rowspan = $_2ekobel5jfuviy2m.has(cell, 'rowspan') ? parseInt($_2ekobel5jfuviy2m.get(cell, 'rowspan'), 10) : 1;
        var colspan = $_2ekobel5jfuviy2m.has(cell, 'colspan') ? parseInt($_2ekobel5jfuviy2m.get(cell, 'colspan'), 10) : 1;
        return $_g02m1vkgjfuvixyt.detail(cell, rowspan, colspan);
      });
      return $_g02m1vkgjfuvixyt.rowdata(row, cells, example.section());
    });
  };
  var $_2vc4gykfjfuvixyi = {
    fromTable: fromTable,
    fromPastedRows: fromPastedRows
  };

  var key = function (row, column) {
    return row + ',' + column;
  };
  var getAt = function (warehouse, row, column) {
    var raw = warehouse.access()[key(row, column)];
    return raw !== undefined ? Option.some(raw) : Option.none();
  };
  var findItem = function (warehouse, item, comparator) {
    var filtered = filterItems(warehouse, function (detail) {
      return comparator(item, detail.element());
    });
    return filtered.length > 0 ? Option.some(filtered[0]) : Option.none();
  };
  var filterItems = function (warehouse, predicate) {
    var all = $_4jja6kk5jfuvixx1.bind(warehouse.all(), function (r) {
      return r.cells();
    });
    return $_4jja6kk5jfuvixx1.filter(all, predicate);
  };
  var generate = function (list) {
    var access = {};
    var cells = [];
    var maxRows = list.length;
    var maxColumns = 0;
    $_4jja6kk5jfuvixx1.each(list, function (details, r) {
      var currentRow = [];
      $_4jja6kk5jfuvixx1.each(details.cells(), function (detail, c) {
        var start = 0;
        while (access[key(r, start)] !== undefined) {
          start++;
        }
        var current = $_g02m1vkgjfuvixyt.extended(detail.element(), detail.rowspan(), detail.colspan(), r, start);
        for (var i = 0; i < detail.colspan(); i++) {
          for (var j = 0; j < detail.rowspan(); j++) {
            var cr = r + j;
            var cc = start + i;
            var newpos = key(cr, cc);
            access[newpos] = current;
            maxColumns = Math.max(maxColumns, cc + 1);
          }
        }
        currentRow.push(current);
      });
      cells.push($_g02m1vkgjfuvixyt.rowdata(details.element(), currentRow, details.section()));
    });
    var grid = $_g02m1vkgjfuvixyt.grid(maxRows, maxColumns);
    return {
      grid: $_fdch7uk7jfuvixxb.constant(grid),
      access: $_fdch7uk7jfuvixxb.constant(access),
      all: $_fdch7uk7jfuvixxb.constant(cells)
    };
  };
  var justCells = function (warehouse) {
    var rows = $_4jja6kk5jfuvixx1.map(warehouse.all(), function (w) {
      return w.cells();
    });
    return $_4jja6kk5jfuvixx1.flatten(rows);
  };
  var $_bwrthsldjfuviy3q = {
    generate: generate,
    getAt: getAt,
    findItem: findItem,
    filterItems: filterItems,
    justCells: justCells
  };

  var isSupported = function (dom) {
    return dom.style !== undefined;
  };
  var $_3bn25blfjfuviy4f = { isSupported: isSupported };

  var internalSet = function (dom, property, value) {
    if (!$_13kw1fk8jfuvixxd.isString(value)) {
      console.error('Invalid call to CSS.set. Property ', property, ':: Value ', value, ':: Element ', dom);
      throw new Error('CSS value must be a string: ' + value);
    }
    if ($_3bn25blfjfuviy4f.isSupported(dom))
      dom.style.setProperty(property, value);
  };
  var internalRemove = function (dom, property) {
    if ($_3bn25blfjfuviy4f.isSupported(dom))
      dom.style.removeProperty(property);
  };
  var set$1 = function (element, property, value) {
    var dom = element.dom();
    internalSet(dom, property, value);
  };
  var setAll$1 = function (element, css) {
    var dom = element.dom();
    $_afb9m6kajfuvixy8.each(css, function (v, k) {
      internalSet(dom, k, v);
    });
  };
  var setOptions = function (element, css) {
    var dom = element.dom();
    $_afb9m6kajfuvixy8.each(css, function (v, k) {
      v.fold(function () {
        internalRemove(dom, k);
      }, function (value) {
        internalSet(dom, k, value);
      });
    });
  };
  var get$1 = function (element, property) {
    var dom = element.dom();
    var styles = window.getComputedStyle(dom);
    var r = styles.getPropertyValue(property);
    var v = r === '' && !$_43dxxcl9jfuviy31.inBody(element) ? getUnsafeProperty(dom, property) : r;
    return v === null ? undefined : v;
  };
  var getUnsafeProperty = function (dom, property) {
    return $_3bn25blfjfuviy4f.isSupported(dom) ? dom.style.getPropertyValue(property) : '';
  };
  var getRaw = function (element, property) {
    var dom = element.dom();
    var raw = getUnsafeProperty(dom, property);
    return Option.from(raw).filter(function (r) {
      return r.length > 0;
    });
  };
  var getAllRaw = function (element) {
    var css = {};
    var dom = element.dom();
    if ($_3bn25blfjfuviy4f.isSupported(dom)) {
      for (var i = 0; i < dom.style.length; i++) {
        var ruleName = dom.style.item(i);
        css[ruleName] = dom.style[ruleName];
      }
    }
    return css;
  };
  var isValidValue = function (tag, property, value) {
    var element = $_4sdhm4kkjfuviy0e.fromTag(tag);
    set$1(element, property, value);
    var style = getRaw(element, property);
    return style.isSome();
  };
  var remove$1 = function (element, property) {
    var dom = element.dom();
    internalRemove(dom, property);
    if ($_2ekobel5jfuviy2m.has(element, 'style') && $_2rmckll2jfuviy2d.trim($_2ekobel5jfuviy2m.get(element, 'style')) === '') {
      $_2ekobel5jfuviy2m.remove(element, 'style');
    }
  };
  var preserve = function (element, f) {
    var oldStyles = $_2ekobel5jfuviy2m.get(element, 'style');
    var result = f(element);
    var restore = oldStyles === undefined ? $_2ekobel5jfuviy2m.remove : $_2ekobel5jfuviy2m.set;
    restore(element, 'style', oldStyles);
    return result;
  };
  var copy = function (source, target) {
    var sourceDom = source.dom();
    var targetDom = target.dom();
    if ($_3bn25blfjfuviy4f.isSupported(sourceDom) && $_3bn25blfjfuviy4f.isSupported(targetDom)) {
      targetDom.style.cssText = sourceDom.style.cssText;
    }
  };
  var reflow = function (e) {
    return e.dom().offsetWidth;
  };
  var transferOne$1 = function (source, destination, style) {
    getRaw(source, style).each(function (value) {
      if (getRaw(destination, style).isNone())
        set$1(destination, style, value);
    });
  };
  var transfer$1 = function (source, destination, styles) {
    if (!$_6mcqmml6jfuviy2u.isElement(source) || !$_6mcqmml6jfuviy2u.isElement(destination))
      return;
    $_4jja6kk5jfuvixx1.each(styles, function (style) {
      transferOne$1(source, destination, style);
    });
  };
  var $_2lr8nrlejfuviy40 = {
    copy: copy,
    set: set$1,
    preserve: preserve,
    setAll: setAll$1,
    setOptions: setOptions,
    remove: remove$1,
    get: get$1,
    getRaw: getRaw,
    getAllRaw: getAllRaw,
    isValidValue: isValidValue,
    reflow: reflow,
    transfer: transfer$1
  };

  var before = function (marker, element) {
    var parent = $_87w3h3kmjfuviy0m.parent(marker);
    parent.each(function (v) {
      v.dom().insertBefore(element.dom(), marker.dom());
    });
  };
  var after = function (marker, element) {
    var sibling = $_87w3h3kmjfuviy0m.nextSibling(marker);
    sibling.fold(function () {
      var parent = $_87w3h3kmjfuviy0m.parent(marker);
      parent.each(function (v) {
        append(v, element);
      });
    }, function (v) {
      before(v, element);
    });
  };
  var prepend = function (parent, element) {
    var firstChild = $_87w3h3kmjfuviy0m.firstChild(parent);
    firstChild.fold(function () {
      append(parent, element);
    }, function (v) {
      parent.dom().insertBefore(element.dom(), v.dom());
    });
  };
  var append = function (parent, element) {
    parent.dom().appendChild(element.dom());
  };
  var appendAt = function (parent, element, index) {
    $_87w3h3kmjfuviy0m.child(parent, index).fold(function () {
      append(parent, element);
    }, function (v) {
      before(v, element);
    });
  };
  var wrap = function (element, wrapper) {
    before(element, wrapper);
    append(wrapper, element);
  };
  var $_5zcsfmlgjfuviy4g = {
    before: before,
    after: after,
    prepend: prepend,
    append: append,
    appendAt: appendAt,
    wrap: wrap
  };

  var before$1 = function (marker, elements) {
    $_4jja6kk5jfuvixx1.each(elements, function (x) {
      $_5zcsfmlgjfuviy4g.before(marker, x);
    });
  };
  var after$1 = function (marker, elements) {
    $_4jja6kk5jfuvixx1.each(elements, function (x, i) {
      var e = i === 0 ? marker : elements[i - 1];
      $_5zcsfmlgjfuviy4g.after(e, x);
    });
  };
  var prepend$1 = function (parent, elements) {
    $_4jja6kk5jfuvixx1.each(elements.slice().reverse(), function (x) {
      $_5zcsfmlgjfuviy4g.prepend(parent, x);
    });
  };
  var append$1 = function (parent, elements) {
    $_4jja6kk5jfuvixx1.each(elements, function (x) {
      $_5zcsfmlgjfuviy4g.append(parent, x);
    });
  };
  var $_44mr3plijfuviy4p = {
    before: before$1,
    after: after$1,
    prepend: prepend$1,
    append: append$1
  };

  var empty = function (element) {
    element.dom().textContent = '';
    $_4jja6kk5jfuvixx1.each($_87w3h3kmjfuviy0m.children(element), function (rogue) {
      remove$2(rogue);
    });
  };
  var remove$2 = function (element) {
    var dom = element.dom();
    if (dom.parentNode !== null)
      dom.parentNode.removeChild(dom);
  };
  var unwrap = function (wrapper) {
    var children = $_87w3h3kmjfuviy0m.children(wrapper);
    if (children.length > 0)
      $_44mr3plijfuviy4p.before(wrapper, children);
    remove$2(wrapper);
  };
  var $_5ud3colhjfuviy4l = {
    empty: empty,
    remove: remove$2,
    unwrap: unwrap
  };

  var stats = $_96oqrskbjfuvixya.immutable('minRow', 'minCol', 'maxRow', 'maxCol');
  var findSelectedStats = function (house, isSelected) {
    var totalColumns = house.grid().columns();
    var totalRows = house.grid().rows();
    var minRow = totalRows;
    var minCol = totalColumns;
    var maxRow = 0;
    var maxCol = 0;
    $_afb9m6kajfuvixy8.each(house.access(), function (detail) {
      if (isSelected(detail)) {
        var startRow = detail.row();
        var endRow = startRow + detail.rowspan() - 1;
        var startCol = detail.column();
        var endCol = startCol + detail.colspan() - 1;
        if (startRow < minRow)
          minRow = startRow;
        else if (endRow > maxRow)
          maxRow = endRow;
        if (startCol < minCol)
          minCol = startCol;
        else if (endCol > maxCol)
          maxCol = endCol;
      }
    });
    return stats(minRow, minCol, maxRow, maxCol);
  };
  var makeCell = function (list, seenSelected, rowIndex) {
    var row = list[rowIndex].element();
    var td = $_4sdhm4kkjfuviy0e.fromTag('td');
    $_5zcsfmlgjfuviy4g.append(td, $_4sdhm4kkjfuviy0e.fromTag('br'));
    var f = seenSelected ? $_5zcsfmlgjfuviy4g.append : $_5zcsfmlgjfuviy4g.prepend;
    f(row, td);
  };
  var fillInGaps = function (list, house, stats, isSelected) {
    var totalColumns = house.grid().columns();
    var totalRows = house.grid().rows();
    for (var i = 0; i < totalRows; i++) {
      var seenSelected = false;
      for (var j = 0; j < totalColumns; j++) {
        if (!(i < stats.minRow() || i > stats.maxRow() || j < stats.minCol() || j > stats.maxCol())) {
          var needCell = $_bwrthsldjfuviy3q.getAt(house, i, j).filter(isSelected).isNone();
          if (needCell)
            makeCell(list, seenSelected, i);
          else
            seenSelected = true;
        }
      }
    }
  };
  var clean = function (table, stats) {
    var emptyRows = $_4jja6kk5jfuvixx1.filter($_f9na0ikijfuvixzq.firstLayer(table, 'tr'), function (row) {
      return row.dom().childElementCount === 0;
    });
    $_4jja6kk5jfuvixx1.each(emptyRows, $_5ud3colhjfuviy4l.remove);
    if (stats.minCol() === stats.maxCol() || stats.minRow() === stats.maxRow()) {
      $_4jja6kk5jfuvixx1.each($_f9na0ikijfuvixzq.firstLayer(table, 'th,td'), function (cell) {
        $_2ekobel5jfuviy2m.remove(cell, 'rowspan');
        $_2ekobel5jfuviy2m.remove(cell, 'colspan');
      });
    }
    $_2ekobel5jfuviy2m.remove(table, 'width');
    $_2ekobel5jfuviy2m.remove(table, 'height');
    $_2lr8nrlejfuviy40.remove(table, 'width');
    $_2lr8nrlejfuviy40.remove(table, 'height');
  };
  var extract = function (table, selectedSelector) {
    var isSelected = function (detail) {
      return $_aphf8fkjjfuviy04.is(detail.element(), selectedSelector);
    };
    var list = $_2vc4gykfjfuvixyi.fromTable(table);
    var house = $_bwrthsldjfuviy3q.generate(list);
    var stats = findSelectedStats(house, isSelected);
    var selector = 'th:not(' + selectedSelector + ')' + ',td:not(' + selectedSelector + ')';
    var unselectedCells = $_f9na0ikijfuvixzq.filterFirstLayer(table, 'th,td', function (cell) {
      return $_aphf8fkjjfuviy04.is(cell, selector);
    });
    $_4jja6kk5jfuvixx1.each(unselectedCells, $_5ud3colhjfuviy4l.remove);
    fillInGaps(list, house, stats, isSelected);
    clean(table, stats);
    return table;
  };
  var $_6u5qawk9jfuvixxg = { extract: extract };

  var clone$1 = function (original, deep) {
    return $_4sdhm4kkjfuviy0e.fromDom(original.dom().cloneNode(deep));
  };
  var shallow = function (original) {
    return clone$1(original, false);
  };
  var deep = function (original) {
    return clone$1(original, true);
  };
  var shallowAs = function (original, tag) {
    var nu = $_4sdhm4kkjfuviy0e.fromTag(tag);
    var attributes = $_2ekobel5jfuviy2m.clone(original);
    $_2ekobel5jfuviy2m.setAll(nu, attributes);
    return nu;
  };
  var copy$1 = function (original, tag) {
    var nu = shallowAs(original, tag);
    var cloneChildren = $_87w3h3kmjfuviy0m.children(deep(original));
    $_44mr3plijfuviy4p.append(nu, cloneChildren);
    return nu;
  };
  var mutate = function (original, tag) {
    var nu = shallowAs(original, tag);
    $_5zcsfmlgjfuviy4g.before(original, nu);
    var children = $_87w3h3kmjfuviy0m.children(original);
    $_44mr3plijfuviy4p.append(nu, children);
    $_5ud3colhjfuviy4l.remove(original);
    return nu;
  };
  var $_1i39h2lkjfuviy5n = {
    shallow: shallow,
    shallowAs: shallowAs,
    deep: deep,
    copy: copy$1,
    mutate: mutate
  };

  function NodeValue (is, name) {
    var get = function (element) {
      if (!is(element))
        throw new Error('Can only get ' + name + ' value of a ' + name + ' node');
      return getOption(element).getOr('');
    };
    var getOptionIE10 = function (element) {
      try {
        return getOptionSafe(element);
      } catch (e) {
        return Option.none();
      }
    };
    var getOptionSafe = function (element) {
      return is(element) ? Option.from(element.dom().nodeValue) : Option.none();
    };
    var browser = $_8chrc7ktjfuviy1m.detect().browser;
    var getOption = browser.isIE() && browser.version.major === 10 ? getOptionIE10 : getOptionSafe;
    var set = function (element, value) {
      if (!is(element))
        throw new Error('Can only set raw ' + name + ' value of a ' + name + ' node');
      element.dom().nodeValue = value;
    };
    return {
      get: get,
      getOption: getOption,
      set: set
    };
  }

  var api = NodeValue($_6mcqmml6jfuviy2u.isText, 'text');
  var get$2 = function (element) {
    return api.get(element);
  };
  var getOption = function (element) {
    return api.getOption(element);
  };
  var set$2 = function (element, value) {
    api.set(element, value);
  };
  var $_cvagqflnjfuviy5y = {
    get: get$2,
    getOption: getOption,
    set: set$2
  };

  var getEnd = function (element) {
    return $_6mcqmml6jfuviy2u.name(element) === 'img' ? 1 : $_cvagqflnjfuviy5y.getOption(element).fold(function () {
      return $_87w3h3kmjfuviy0m.children(element).length;
    }, function (v) {
      return v.length;
    });
  };
  var isEnd = function (element, offset) {
    return getEnd(element) === offset;
  };
  var isStart = function (element, offset) {
    return offset === 0;
  };
  var NBSP = '\xA0';
  var isTextNodeWithCursorPosition = function (el) {
    return $_cvagqflnjfuviy5y.getOption(el).filter(function (text) {
      return text.trim().length !== 0 || text.indexOf(NBSP) > -1;
    }).isSome();
  };
  var elementsWithCursorPosition = [
    'img',
    'br'
  ];
  var isCursorPosition = function (elem) {
    var hasCursorPosition = isTextNodeWithCursorPosition(elem);
    return hasCursorPosition || $_4jja6kk5jfuvixx1.contains(elementsWithCursorPosition, $_6mcqmml6jfuviy2u.name(elem));
  };
  var $_cj2m48lmjfuviy5u = {
    getEnd: getEnd,
    isEnd: isEnd,
    isStart: isStart,
    isCursorPosition: isCursorPosition
  };

  var first$3 = function (element) {
    return $_eg4f87lbjfuviy37.descendant(element, $_cj2m48lmjfuviy5u.isCursorPosition);
  };
  var last$2 = function (element) {
    return descendantRtl(element, $_cj2m48lmjfuviy5u.isCursorPosition);
  };
  var descendantRtl = function (scope, predicate) {
    var descend = function (element) {
      var children = $_87w3h3kmjfuviy0m.children(element);
      for (var i = children.length - 1; i >= 0; i--) {
        var child = children[i];
        if (predicate(child))
          return Option.some(child);
        var res = descend(child);
        if (res.isSome())
          return res;
      }
      return Option.none();
    };
    return descend(scope);
  };
  var $_dsfijblljfuviy5q = {
    first: first$3,
    last: last$2
  };

  var cell$1 = function () {
    var td = $_4sdhm4kkjfuviy0e.fromTag('td');
    $_5zcsfmlgjfuviy4g.append(td, $_4sdhm4kkjfuviy0e.fromTag('br'));
    return td;
  };
  var replace = function (cell, tag, attrs) {
    var replica = $_1i39h2lkjfuviy5n.copy(cell, tag);
    $_afb9m6kajfuvixy8.each(attrs, function (v, k) {
      if (v === null)
        $_2ekobel5jfuviy2m.remove(replica, k);
      else
        $_2ekobel5jfuviy2m.set(replica, k, v);
    });
    return replica;
  };
  var pasteReplace = function (cellContent) {
    return cellContent;
  };
  var newRow = function (doc) {
    return function () {
      return $_4sdhm4kkjfuviy0e.fromTag('tr', doc.dom());
    };
  };
  var cloneFormats = function (oldCell, newCell, formats) {
    var first = $_dsfijblljfuviy5q.first(oldCell);
    return first.map(function (firstText) {
      var formatSelector = formats.join(',');
      var parents = $_a3hs1bl7jfuviy2w.ancestors(firstText, formatSelector, function (element) {
        return $_g6ztqikojfuviy13.eq(element, oldCell);
      });
      return $_4jja6kk5jfuvixx1.foldr(parents, function (last, parent) {
        var clonedFormat = $_1i39h2lkjfuviy5n.shallow(parent);
        $_5zcsfmlgjfuviy4g.append(last, clonedFormat);
        return clonedFormat;
      }, newCell);
    }).getOr(newCell);
  };
  var cellOperations = function (mutate, doc, formatsToClone) {
    var newCell = function (prev) {
      var doc = $_87w3h3kmjfuviy0m.owner(prev.element());
      var td = $_4sdhm4kkjfuviy0e.fromTag($_6mcqmml6jfuviy2u.name(prev.element()), doc.dom());
      var formats = formatsToClone.getOr([
        'strong',
        'em',
        'b',
        'i',
        'span',
        'font',
        'h1',
        'h2',
        'h3',
        'h4',
        'h5',
        'h6',
        'p',
        'div'
      ]);
      var lastNode = formats.length > 0 ? cloneFormats(prev.element(), td, formats) : td;
      $_5zcsfmlgjfuviy4g.append(lastNode, $_4sdhm4kkjfuviy0e.fromTag('br'));
      $_2lr8nrlejfuviy40.copy(prev.element(), td);
      $_2lr8nrlejfuviy40.remove(td, 'height');
      if (prev.colspan() !== 1)
        $_2lr8nrlejfuviy40.remove(prev.element(), 'width');
      mutate(prev.element(), td);
      return td;
    };
    return {
      row: newRow(doc),
      cell: newCell,
      replace: replace,
      gap: cell$1
    };
  };
  var paste = function (doc) {
    return {
      row: newRow(doc),
      cell: cell$1,
      replace: pasteReplace,
      gap: cell$1
    };
  };
  var $_clky4ljjfuviy4t = {
    cellOperations: cellOperations,
    paste: paste
  };

  var fromHtml$1 = function (html, scope) {
    var doc = scope || document;
    var div = doc.createElement('div');
    div.innerHTML = html;
    return $_87w3h3kmjfuviy0m.children($_4sdhm4kkjfuviy0e.fromDom(div));
  };
  var fromTags = function (tags, scope) {
    return $_4jja6kk5jfuvixx1.map(tags, function (x) {
      return $_4sdhm4kkjfuviy0e.fromTag(x, scope);
    });
  };
  var fromText$1 = function (texts, scope) {
    return $_4jja6kk5jfuvixx1.map(texts, function (x) {
      return $_4sdhm4kkjfuviy0e.fromText(x, scope);
    });
  };
  var fromDom$1 = function (nodes) {
    return $_4jja6kk5jfuvixx1.map(nodes, $_4sdhm4kkjfuviy0e.fromDom);
  };
  var $_ek5zoelpjfuviy65 = {
    fromHtml: fromHtml$1,
    fromTags: fromTags,
    fromText: fromText$1,
    fromDom: fromDom$1
  };

  var TagBoundaries = [
    'body',
    'p',
    'div',
    'article',
    'aside',
    'figcaption',
    'figure',
    'footer',
    'header',
    'nav',
    'section',
    'ol',
    'ul',
    'li',
    'table',
    'thead',
    'tbody',
    'tfoot',
    'caption',
    'tr',
    'td',
    'th',
    'h1',
    'h2',
    'h3',
    'h4',
    'h5',
    'h6',
    'blockquote',
    'pre',
    'address'
  ];

  function DomUniverse () {
    var clone = function (element) {
      return $_4sdhm4kkjfuviy0e.fromDom(element.dom().cloneNode(false));
    };
    var isBoundary = function (element) {
      if (!$_6mcqmml6jfuviy2u.isElement(element))
        return false;
      if ($_6mcqmml6jfuviy2u.name(element) === 'body')
        return true;
      return $_4jja6kk5jfuvixx1.contains(TagBoundaries, $_6mcqmml6jfuviy2u.name(element));
    };
    var isEmptyTag = function (element) {
      if (!$_6mcqmml6jfuviy2u.isElement(element))
        return false;
      return $_4jja6kk5jfuvixx1.contains([
        'br',
        'img',
        'hr',
        'input'
      ], $_6mcqmml6jfuviy2u.name(element));
    };
    var comparePosition = function (element, other) {
      return element.dom().compareDocumentPosition(other.dom());
    };
    var copyAttributesTo = function (source, destination) {
      var as = $_2ekobel5jfuviy2m.clone(source);
      $_2ekobel5jfuviy2m.setAll(destination, as);
    };
    return {
      up: $_fdch7uk7jfuvixxb.constant({
        selector: $_26gnp6lajfuviy35.ancestor,
        closest: $_26gnp6lajfuviy35.closest,
        predicate: $_eg4f87lbjfuviy37.ancestor,
        all: $_87w3h3kmjfuviy0m.parents
      }),
      down: $_fdch7uk7jfuvixxb.constant({
        selector: $_a3hs1bl7jfuviy2w.descendants,
        predicate: $_g5dooll8jfuviy2x.descendants
      }),
      styles: $_fdch7uk7jfuvixxb.constant({
        get: $_2lr8nrlejfuviy40.get,
        getRaw: $_2lr8nrlejfuviy40.getRaw,
        set: $_2lr8nrlejfuviy40.set,
        remove: $_2lr8nrlejfuviy40.remove
      }),
      attrs: $_fdch7uk7jfuvixxb.constant({
        get: $_2ekobel5jfuviy2m.get,
        set: $_2ekobel5jfuviy2m.set,
        remove: $_2ekobel5jfuviy2m.remove,
        copyTo: copyAttributesTo
      }),
      insert: $_fdch7uk7jfuvixxb.constant({
        before: $_5zcsfmlgjfuviy4g.before,
        after: $_5zcsfmlgjfuviy4g.after,
        afterAll: $_44mr3plijfuviy4p.after,
        append: $_5zcsfmlgjfuviy4g.append,
        appendAll: $_44mr3plijfuviy4p.append,
        prepend: $_5zcsfmlgjfuviy4g.prepend,
        wrap: $_5zcsfmlgjfuviy4g.wrap
      }),
      remove: $_fdch7uk7jfuvixxb.constant({
        unwrap: $_5ud3colhjfuviy4l.unwrap,
        remove: $_5ud3colhjfuviy4l.remove
      }),
      create: $_fdch7uk7jfuvixxb.constant({
        nu: $_4sdhm4kkjfuviy0e.fromTag,
        clone: clone,
        text: $_4sdhm4kkjfuviy0e.fromText
      }),
      query: $_fdch7uk7jfuvixxb.constant({
        comparePosition: comparePosition,
        prevSibling: $_87w3h3kmjfuviy0m.prevSibling,
        nextSibling: $_87w3h3kmjfuviy0m.nextSibling
      }),
      property: $_fdch7uk7jfuvixxb.constant({
        children: $_87w3h3kmjfuviy0m.children,
        name: $_6mcqmml6jfuviy2u.name,
        parent: $_87w3h3kmjfuviy0m.parent,
        isText: $_6mcqmml6jfuviy2u.isText,
        isComment: $_6mcqmml6jfuviy2u.isComment,
        isElement: $_6mcqmml6jfuviy2u.isElement,
        getText: $_cvagqflnjfuviy5y.get,
        setText: $_cvagqflnjfuviy5y.set,
        isBoundary: isBoundary,
        isEmptyTag: isEmptyTag
      }),
      eq: $_g6ztqikojfuviy13.eq,
      is: $_g6ztqikojfuviy13.is
    };
  }

  var leftRight = $_96oqrskbjfuvixya.immutable('left', 'right');
  var bisect = function (universe, parent, child) {
    var children = universe.property().children(parent);
    var index = $_4jja6kk5jfuvixx1.findIndex(children, $_fdch7uk7jfuvixxb.curry(universe.eq, child));
    return index.map(function (ind) {
      return {
        before: $_fdch7uk7jfuvixxb.constant(children.slice(0, ind)),
        after: $_fdch7uk7jfuvixxb.constant(children.slice(ind + 1))
      };
    });
  };
  var breakToRight = function (universe, parent, child) {
    return bisect(universe, parent, child).map(function (parts) {
      var second = universe.create().clone(parent);
      universe.insert().appendAll(second, parts.after());
      universe.insert().after(parent, second);
      return leftRight(parent, second);
    });
  };
  var breakToLeft = function (universe, parent, child) {
    return bisect(universe, parent, child).map(function (parts) {
      var prior = universe.create().clone(parent);
      universe.insert().appendAll(prior, parts.before().concat([child]));
      universe.insert().appendAll(parent, parts.after());
      universe.insert().before(parent, prior);
      return leftRight(prior, parent);
    });
  };
  var breakPath = function (universe, item, isTop, breaker) {
    var result = $_96oqrskbjfuvixya.immutable('first', 'second', 'splits');
    var next = function (child, group, splits) {
      var fallback = result(child, Option.none(), splits);
      if (isTop(child))
        return result(child, group, splits);
      else {
        return universe.property().parent(child).bind(function (parent) {
          return breaker(universe, parent, child).map(function (breakage) {
            var extra = [{
                first: breakage.left,
                second: breakage.right
              }];
            var nextChild = isTop(parent) ? parent : breakage.left();
            return next(nextChild, Option.some(breakage.right()), splits.concat(extra));
          }).getOr(fallback);
        });
      }
    };
    return next(item, Option.none(), []);
  };
  var $_abr277lyjfuviy9c = {
    breakToLeft: breakToLeft,
    breakToRight: breakToRight,
    breakPath: breakPath
  };

  var all$3 = function (universe, look, elements, f) {
    var head = elements[0];
    var tail = elements.slice(1);
    return f(universe, look, head, tail);
  };
  var oneAll = function (universe, look, elements) {
    return elements.length > 0 ? all$3(universe, look, elements, unsafeOne) : Option.none();
  };
  var unsafeOne = function (universe, look, head, tail) {
    var start = look(universe, head);
    return $_4jja6kk5jfuvixx1.foldr(tail, function (b, a) {
      var current = look(universe, a);
      return commonElement(universe, b, current);
    }, start);
  };
  var commonElement = function (universe, start, end) {
    return start.bind(function (s) {
      return end.filter($_fdch7uk7jfuvixxb.curry(universe.eq, s));
    });
  };
  var $_bupazclzjfuviy9n = { oneAll: oneAll };

  var eq$1 = function (universe, item) {
    return $_fdch7uk7jfuvixxb.curry(universe.eq, item);
  };
  var unsafeSubset = function (universe, common, ps1, ps2) {
    var children = universe.property().children(common);
    if (universe.eq(common, ps1[0]))
      return Option.some([ps1[0]]);
    if (universe.eq(common, ps2[0]))
      return Option.some([ps2[0]]);
    var finder = function (ps) {
      var topDown = $_4jja6kk5jfuvixx1.reverse(ps);
      var index = $_4jja6kk5jfuvixx1.findIndex(topDown, eq$1(universe, common)).getOr(-1);
      var item = index < topDown.length - 1 ? topDown[index + 1] : topDown[index];
      return $_4jja6kk5jfuvixx1.findIndex(children, eq$1(universe, item));
    };
    var startIndex = finder(ps1);
    var endIndex = finder(ps2);
    return startIndex.bind(function (sIndex) {
      return endIndex.map(function (eIndex) {
        var first = Math.min(sIndex, eIndex);
        var last = Math.max(sIndex, eIndex);
        return children.slice(first, last + 1);
      });
    });
  };
  var ancestors$2 = function (universe, start, end, _isRoot) {
    var isRoot = _isRoot !== undefined ? _isRoot : $_fdch7uk7jfuvixxb.constant(false);
    var ps1 = [start].concat(universe.up().all(start));
    var ps2 = [end].concat(universe.up().all(end));
    var prune = function (path) {
      var index = $_4jja6kk5jfuvixx1.findIndex(path, isRoot);
      return index.fold(function () {
        return path;
      }, function (ind) {
        return path.slice(0, ind + 1);
      });
    };
    var pruned1 = prune(ps1);
    var pruned2 = prune(ps2);
    var shared = $_4jja6kk5jfuvixx1.find(pruned1, function (x) {
      return $_4jja6kk5jfuvixx1.exists(pruned2, eq$1(universe, x));
    });
    return {
      firstpath: $_fdch7uk7jfuvixxb.constant(pruned1),
      secondpath: $_fdch7uk7jfuvixxb.constant(pruned2),
      shared: $_fdch7uk7jfuvixxb.constant(shared)
    };
  };
  var subset = function (universe, start, end) {
    var ancs = ancestors$2(universe, start, end);
    return ancs.shared().bind(function (shared) {
      return unsafeSubset(universe, shared, ancs.firstpath(), ancs.secondpath());
    });
  };
  var $_gb7ghem0jfuviy9y = {
    subset: subset,
    ancestors: ancestors$2
  };

  var sharedOne = function (universe, look, elements) {
    return $_bupazclzjfuviy9n.oneAll(universe, look, elements);
  };
  var subset$1 = function (universe, start, finish) {
    return $_gb7ghem0jfuviy9y.subset(universe, start, finish);
  };
  var ancestors$3 = function (universe, start, finish, _isRoot) {
    return $_gb7ghem0jfuviy9y.ancestors(universe, start, finish, _isRoot);
  };
  var breakToLeft$1 = function (universe, parent, child) {
    return $_abr277lyjfuviy9c.breakToLeft(universe, parent, child);
  };
  var breakToRight$1 = function (universe, parent, child) {
    return $_abr277lyjfuviy9c.breakToRight(universe, parent, child);
  };
  var breakPath$1 = function (universe, child, isTop, breaker) {
    return $_abr277lyjfuviy9c.breakPath(universe, child, isTop, breaker);
  };
  var $_11qknclxjfuviy9a = {
    sharedOne: sharedOne,
    subset: subset$1,
    ancestors: ancestors$3,
    breakToLeft: breakToLeft$1,
    breakToRight: breakToRight$1,
    breakPath: breakPath$1
  };

  var universe = DomUniverse();
  var sharedOne$1 = function (look, elements) {
    return $_11qknclxjfuviy9a.sharedOne(universe, function (universe, element) {
      return look(element);
    }, elements);
  };
  var subset$2 = function (start, finish) {
    return $_11qknclxjfuviy9a.subset(universe, start, finish);
  };
  var ancestors$4 = function (start, finish, _isRoot) {
    return $_11qknclxjfuviy9a.ancestors(universe, start, finish, _isRoot);
  };
  var breakToLeft$2 = function (parent, child) {
    return $_11qknclxjfuviy9a.breakToLeft(universe, parent, child);
  };
  var breakToRight$2 = function (parent, child) {
    return $_11qknclxjfuviy9a.breakToRight(universe, parent, child);
  };
  var breakPath$2 = function (child, isTop, breaker) {
    return $_11qknclxjfuviy9a.breakPath(universe, child, isTop, function (u, p, c) {
      return breaker(p, c);
    });
  };
  var $_1tw8e1lujfuviy7z = {
    sharedOne: sharedOne$1,
    subset: subset$2,
    ancestors: ancestors$4,
    breakToLeft: breakToLeft$2,
    breakToRight: breakToRight$2,
    breakPath: breakPath$2
  };

  var inSelection = function (bounds, detail) {
    var leftEdge = detail.column();
    var rightEdge = detail.column() + detail.colspan() - 1;
    var topEdge = detail.row();
    var bottomEdge = detail.row() + detail.rowspan() - 1;
    return leftEdge <= bounds.finishCol() && rightEdge >= bounds.startCol() && (topEdge <= bounds.finishRow() && bottomEdge >= bounds.startRow());
  };
  var isWithin = function (bounds, detail) {
    return detail.column() >= bounds.startCol() && detail.column() + detail.colspan() - 1 <= bounds.finishCol() && detail.row() >= bounds.startRow() && detail.row() + detail.rowspan() - 1 <= bounds.finishRow();
  };
  var isRectangular = function (warehouse, bounds) {
    var isRect = true;
    var detailIsWithin = $_fdch7uk7jfuvixxb.curry(isWithin, bounds);
    for (var i = bounds.startRow(); i <= bounds.finishRow(); i++) {
      for (var j = bounds.startCol(); j <= bounds.finishCol(); j++) {
        isRect = isRect && $_bwrthsldjfuviy3q.getAt(warehouse, i, j).exists(detailIsWithin);
      }
    }
    return isRect ? Option.some(bounds) : Option.none();
  };
  var $_5ehsnem3jfuviyal = {
    inSelection: inSelection,
    isWithin: isWithin,
    isRectangular: isRectangular
  };

  var getBounds = function (detailA, detailB) {
    return $_g02m1vkgjfuvixyt.bounds(Math.min(detailA.row(), detailB.row()), Math.min(detailA.column(), detailB.column()), Math.max(detailA.row() + detailA.rowspan() - 1, detailB.row() + detailB.rowspan() - 1), Math.max(detailA.column() + detailA.colspan() - 1, detailB.column() + detailB.colspan() - 1));
  };
  var getAnyBox = function (warehouse, startCell, finishCell) {
    var startCoords = $_bwrthsldjfuviy3q.findItem(warehouse, startCell, $_g6ztqikojfuviy13.eq);
    var finishCoords = $_bwrthsldjfuviy3q.findItem(warehouse, finishCell, $_g6ztqikojfuviy13.eq);
    return startCoords.bind(function (sc) {
      return finishCoords.map(function (fc) {
        return getBounds(sc, fc);
      });
    });
  };
  var getBox = function (warehouse, startCell, finishCell) {
    return getAnyBox(warehouse, startCell, finishCell).bind(function (bounds) {
      return $_5ehsnem3jfuviyal.isRectangular(warehouse, bounds);
    });
  };
  var $_dmj7zzm4jfuviyas = {
    getAnyBox: getAnyBox,
    getBox: getBox
  };

  var moveBy = function (warehouse, cell, row, column) {
    return $_bwrthsldjfuviy3q.findItem(warehouse, cell, $_g6ztqikojfuviy13.eq).bind(function (detail) {
      var startRow = row > 0 ? detail.row() + detail.rowspan() - 1 : detail.row();
      var startCol = column > 0 ? detail.column() + detail.colspan() - 1 : detail.column();
      var dest = $_bwrthsldjfuviy3q.getAt(warehouse, startRow + row, startCol + column);
      return dest.map(function (d) {
        return d.element();
      });
    });
  };
  var intercepts = function (warehouse, start, finish) {
    return $_dmj7zzm4jfuviyas.getAnyBox(warehouse, start, finish).map(function (bounds) {
      var inside = $_bwrthsldjfuviy3q.filterItems(warehouse, $_fdch7uk7jfuvixxb.curry($_5ehsnem3jfuviyal.inSelection, bounds));
      return $_4jja6kk5jfuvixx1.map(inside, function (detail) {
        return detail.element();
      });
    });
  };
  var parentCell = function (warehouse, innerCell) {
    var isContainedBy = function (c1, c2) {
      return $_g6ztqikojfuviy13.contains(c2, c1);
    };
    return $_bwrthsldjfuviy3q.findItem(warehouse, innerCell, isContainedBy).bind(function (detail) {
      return detail.element();
    });
  };
  var $_6im98qm2jfuviyac = {
    moveBy: moveBy,
    intercepts: intercepts,
    parentCell: parentCell
  };

  var moveBy$1 = function (cell, deltaRow, deltaColumn) {
    return $_dmqxswkhjfuvixyz.table(cell).bind(function (table) {
      var warehouse = getWarehouse(table);
      return $_6im98qm2jfuviyac.moveBy(warehouse, cell, deltaRow, deltaColumn);
    });
  };
  var intercepts$1 = function (table, first, last) {
    var warehouse = getWarehouse(table);
    return $_6im98qm2jfuviyac.intercepts(warehouse, first, last);
  };
  var nestedIntercepts = function (table, first, firstTable, last, lastTable) {
    var warehouse = getWarehouse(table);
    var startCell = $_g6ztqikojfuviy13.eq(table, firstTable) ? first : $_6im98qm2jfuviyac.parentCell(warehouse, first);
    var lastCell = $_g6ztqikojfuviy13.eq(table, lastTable) ? last : $_6im98qm2jfuviyac.parentCell(warehouse, last);
    return $_6im98qm2jfuviyac.intercepts(warehouse, startCell, lastCell);
  };
  var getBox$1 = function (table, first, last) {
    var warehouse = getWarehouse(table);
    return $_dmj7zzm4jfuviyas.getBox(warehouse, first, last);
  };
  var getWarehouse = function (table) {
    var list = $_2vc4gykfjfuvixyi.fromTable(table);
    return $_bwrthsldjfuviy3q.generate(list);
  };
  var $_dzfbvm1jfuviya7 = {
    moveBy: moveBy$1,
    intercepts: intercepts$1,
    nestedIntercepts: nestedIntercepts,
    getBox: getBox$1
  };

  var lookupTable = function (container, isRoot) {
    return $_26gnp6lajfuviy35.ancestor(container, 'table');
  };
  var identified = $_96oqrskbjfuvixya.immutableBag([
    'boxes',
    'start',
    'finish'
  ], []);
  var identify = function (start, finish, isRoot) {
    var getIsRoot = function (rootTable) {
      return function (element) {
        return isRoot(element) || $_g6ztqikojfuviy13.eq(element, rootTable);
      };
    };
    if ($_g6ztqikojfuviy13.eq(start, finish)) {
      return Option.some(identified({
        boxes: Option.some([start]),
        start: start,
        finish: finish
      }));
    } else {
      return lookupTable(start, isRoot).bind(function (startTable) {
        return lookupTable(finish, isRoot).bind(function (finishTable) {
          if ($_g6ztqikojfuviy13.eq(startTable, finishTable)) {
            return Option.some(identified({
              boxes: $_dzfbvm1jfuviya7.intercepts(startTable, start, finish),
              start: start,
              finish: finish
            }));
          } else if ($_g6ztqikojfuviy13.contains(startTable, finishTable)) {
            var ancestorCells = $_a3hs1bl7jfuviy2w.ancestors(finish, 'td,th', getIsRoot(startTable));
            var finishCell = ancestorCells.length > 0 ? ancestorCells[ancestorCells.length - 1] : finish;
            return Option.some(identified({
              boxes: $_dzfbvm1jfuviya7.nestedIntercepts(startTable, start, startTable, finish, finishTable),
              start: start,
              finish: finishCell
            }));
          } else if ($_g6ztqikojfuviy13.contains(finishTable, startTable)) {
            var ancestorCells = $_a3hs1bl7jfuviy2w.ancestors(start, 'td,th', getIsRoot(finishTable));
            var startCell = ancestorCells.length > 0 ? ancestorCells[ancestorCells.length - 1] : start;
            return Option.some(identified({
              boxes: $_dzfbvm1jfuviya7.nestedIntercepts(finishTable, start, startTable, finish, finishTable),
              start: start,
              finish: startCell
            }));
          } else {
            return $_1tw8e1lujfuviy7z.ancestors(start, finish).shared().bind(function (lca) {
              return $_26gnp6lajfuviy35.closest(lca, 'table', isRoot).bind(function (lcaTable) {
                var finishAncestorCells = $_a3hs1bl7jfuviy2w.ancestors(finish, 'td,th', getIsRoot(lcaTable));
                var finishCell = finishAncestorCells.length > 0 ? finishAncestorCells[finishAncestorCells.length - 1] : finish;
                var startAncestorCells = $_a3hs1bl7jfuviy2w.ancestors(start, 'td,th', getIsRoot(lcaTable));
                var startCell = startAncestorCells.length > 0 ? startAncestorCells[startAncestorCells.length - 1] : start;
                return Option.some(identified({
                  boxes: $_dzfbvm1jfuviya7.nestedIntercepts(lcaTable, start, startTable, finish, finishTable),
                  start: startCell,
                  finish: finishCell
                }));
              });
            });
          }
        });
      });
    }
  };
  var retrieve = function (container, selector) {
    var sels = $_a3hs1bl7jfuviy2w.descendants(container, selector);
    return sels.length > 0 ? Option.some(sels) : Option.none();
  };
  var getLast = function (boxes, lastSelectedSelector) {
    return $_4jja6kk5jfuvixx1.find(boxes, function (box) {
      return $_aphf8fkjjfuviy04.is(box, lastSelectedSelector);
    });
  };
  var getEdges = function (container, firstSelectedSelector, lastSelectedSelector) {
    return $_26gnp6lajfuviy35.descendant(container, firstSelectedSelector).bind(function (first) {
      return $_26gnp6lajfuviy35.descendant(container, lastSelectedSelector).bind(function (last) {
        return $_1tw8e1lujfuviy7z.sharedOne(lookupTable, [
          first,
          last
        ]).map(function (tbl) {
          return {
            first: $_fdch7uk7jfuvixxb.constant(first),
            last: $_fdch7uk7jfuvixxb.constant(last),
            table: $_fdch7uk7jfuvixxb.constant(tbl)
          };
        });
      });
    });
  };
  var expandTo = function (finish, firstSelectedSelector) {
    return $_26gnp6lajfuviy35.ancestor(finish, 'table').bind(function (table) {
      return $_26gnp6lajfuviy35.descendant(table, firstSelectedSelector).bind(function (start) {
        return identify(start, finish).bind(function (identified) {
          return identified.boxes().map(function (boxes) {
            return {
              boxes: $_fdch7uk7jfuvixxb.constant(boxes),
              start: $_fdch7uk7jfuvixxb.constant(identified.start()),
              finish: $_fdch7uk7jfuvixxb.constant(identified.finish())
            };
          });
        });
      });
    });
  };
  var shiftSelection = function (boxes, deltaRow, deltaColumn, firstSelectedSelector, lastSelectedSelector) {
    return getLast(boxes, lastSelectedSelector).bind(function (last) {
      return $_dzfbvm1jfuviya7.moveBy(last, deltaRow, deltaColumn).bind(function (finish) {
        return expandTo(finish, firstSelectedSelector);
      });
    });
  };
  var $_8vwxtkltjfuviy76 = {
    identify: identify,
    retrieve: retrieve,
    shiftSelection: shiftSelection,
    getEdges: getEdges
  };

  var retrieve$1 = function (container, selector) {
    return $_8vwxtkltjfuviy76.retrieve(container, selector);
  };
  var retrieveBox = function (container, firstSelectedSelector, lastSelectedSelector) {
    return $_8vwxtkltjfuviy76.getEdges(container, firstSelectedSelector, lastSelectedSelector).bind(function (edges) {
      var isRoot = function (ancestor) {
        return $_g6ztqikojfuviy13.eq(container, ancestor);
      };
      var firstAncestor = $_26gnp6lajfuviy35.ancestor(edges.first(), 'thead,tfoot,tbody,table', isRoot);
      var lastAncestor = $_26gnp6lajfuviy35.ancestor(edges.last(), 'thead,tfoot,tbody,table', isRoot);
      return firstAncestor.bind(function (fA) {
        return lastAncestor.bind(function (lA) {
          return $_g6ztqikojfuviy13.eq(fA, lA) ? $_dzfbvm1jfuviya7.getBox(edges.table(), edges.first(), edges.last()) : Option.none();
        });
      });
    });
  };
  var $_7o7mthlsjfuviy6v = {
    retrieve: retrieve$1,
    retrieveBox: retrieveBox
  };

  var selected = 'data-mce-selected';
  var selectedSelector = 'td[' + selected + '],th[' + selected + ']';
  var attributeSelector = '[' + selected + ']';
  var firstSelected = 'data-mce-first-selected';
  var firstSelectedSelector = 'td[' + firstSelected + '],th[' + firstSelected + ']';
  var lastSelected = 'data-mce-last-selected';
  var lastSelectedSelector = 'td[' + lastSelected + '],th[' + lastSelected + ']';
  var $_g37vw7m5jfuviyax = {
    selected: $_fdch7uk7jfuvixxb.constant(selected),
    selectedSelector: $_fdch7uk7jfuvixxb.constant(selectedSelector),
    attributeSelector: $_fdch7uk7jfuvixxb.constant(attributeSelector),
    firstSelected: $_fdch7uk7jfuvixxb.constant(firstSelected),
    firstSelectedSelector: $_fdch7uk7jfuvixxb.constant(firstSelectedSelector),
    lastSelected: $_fdch7uk7jfuvixxb.constant(lastSelected),
    lastSelectedSelector: $_fdch7uk7jfuvixxb.constant(lastSelectedSelector)
  };

  var generate$1 = function (cases) {
    if (!$_13kw1fk8jfuvixxd.isArray(cases)) {
      throw new Error('cases must be an array');
    }
    if (cases.length === 0) {
      throw new Error('there must be at least one case');
    }
    var constructors = [];
    var adt = {};
    $_4jja6kk5jfuvixx1.each(cases, function (acase, count) {
      var keys = $_afb9m6kajfuvixy8.keys(acase);
      if (keys.length !== 1) {
        throw new Error('one and only one name per case');
      }
      var key = keys[0];
      var value = acase[key];
      if (adt[key] !== undefined) {
        throw new Error('duplicate key detected:' + key);
      } else if (key === 'cata') {
        throw new Error('cannot have a case named cata (sorry)');
      } else if (!$_13kw1fk8jfuvixxd.isArray(value)) {
        throw new Error('case arguments must be an array');
      }
      constructors.push(key);
      adt[key] = function () {
        var argLength = arguments.length;
        if (argLength !== value.length) {
          throw new Error('Wrong number of arguments to case ' + key + '. Expected ' + value.length + ' (' + value + '), got ' + argLength);
        }
        var args = new Array(argLength);
        for (var i = 0; i < args.length; i++)
          args[i] = arguments[i];
        var match = function (branches) {
          var branchKeys = $_afb9m6kajfuvixy8.keys(branches);
          if (constructors.length !== branchKeys.length) {
            throw new Error('Wrong number of arguments to match. Expected: ' + constructors.join(',') + '\nActual: ' + branchKeys.join(','));
          }
          var allReqd = $_4jja6kk5jfuvixx1.forall(constructors, function (reqKey) {
            return $_4jja6kk5jfuvixx1.contains(branchKeys, reqKey);
          });
          if (!allReqd)
            throw new Error('Not all branches were specified when using match. Specified: ' + branchKeys.join(', ') + '\nRequired: ' + constructors.join(', '));
          return branches[key].apply(null, args);
        };
        return {
          fold: function () {
            if (arguments.length !== cases.length) {
              throw new Error('Wrong number of arguments to fold. Expected ' + cases.length + ', got ' + arguments.length);
            }
            var target = arguments[count];
            return target.apply(null, args);
          },
          match: match,
          log: function (label) {
            console.log(label, {
              constructors: constructors,
              constructor: key,
              params: args
            });
          }
        };
      };
    });
    return adt;
  };
  var $_27harem7jfuviyb3 = { generate: generate$1 };

  var type$1 = $_27harem7jfuviyb3.generate([
    { none: [] },
    { multiple: ['elements'] },
    { single: ['selection'] }
  ]);
  var cata = function (subject, onNone, onMultiple, onSingle) {
    return subject.fold(onNone, onMultiple, onSingle);
  };
  var $_1jmyeim6jfuviyb0 = {
    cata: cata,
    none: type$1.none,
    multiple: type$1.multiple,
    single: type$1.single
  };

  var selection = function (cell, selections) {
    return $_1jmyeim6jfuviyb0.cata(selections.get(), $_fdch7uk7jfuvixxb.constant([]), $_fdch7uk7jfuvixxb.identity, $_fdch7uk7jfuvixxb.constant([cell]));
  };
  var unmergable = function (cell, selections) {
    var hasSpan = function (elem) {
      return $_2ekobel5jfuviy2m.has(elem, 'rowspan') && parseInt($_2ekobel5jfuviy2m.get(elem, 'rowspan'), 10) > 1 || $_2ekobel5jfuviy2m.has(elem, 'colspan') && parseInt($_2ekobel5jfuviy2m.get(elem, 'colspan'), 10) > 1;
    };
    var candidates = selection(cell, selections);
    return candidates.length > 0 && $_4jja6kk5jfuvixx1.forall(candidates, hasSpan) ? Option.some(candidates) : Option.none();
  };
  var mergable = function (table, selections) {
    return $_1jmyeim6jfuviyb0.cata(selections.get(), Option.none, function (cells, _env) {
      if (cells.length === 0) {
        return Option.none();
      }
      return $_7o7mthlsjfuviy6v.retrieveBox(table, $_g37vw7m5jfuviyax.firstSelectedSelector(), $_g37vw7m5jfuviyax.lastSelectedSelector()).bind(function (bounds) {
        return cells.length > 1 ? Option.some({
          bounds: $_fdch7uk7jfuvixxb.constant(bounds),
          cells: $_fdch7uk7jfuvixxb.constant(cells)
        }) : Option.none();
      });
    }, Option.none);
  };
  var $_fnw8s9lrjfuviy6g = {
    mergable: mergable,
    unmergable: unmergable,
    selection: selection
  };

  var noMenu = function (cell) {
    return {
      element: $_fdch7uk7jfuvixxb.constant(cell),
      mergable: Option.none,
      unmergable: Option.none,
      selection: $_fdch7uk7jfuvixxb.constant([cell])
    };
  };
  var forMenu = function (selections, table, cell) {
    return {
      element: $_fdch7uk7jfuvixxb.constant(cell),
      mergable: $_fdch7uk7jfuvixxb.constant($_fnw8s9lrjfuviy6g.mergable(table, selections)),
      unmergable: $_fdch7uk7jfuvixxb.constant($_fnw8s9lrjfuviy6g.unmergable(cell, selections)),
      selection: $_fdch7uk7jfuvixxb.constant($_fnw8s9lrjfuviy6g.selection(cell, selections))
    };
  };
  var notCell$1 = function (element) {
    return noMenu(element);
  };
  var paste$1 = $_96oqrskbjfuvixya.immutable('element', 'clipboard', 'generators');
  var pasteRows = function (selections, table, cell, clipboard, generators) {
    return {
      element: $_fdch7uk7jfuvixxb.constant(cell),
      mergable: Option.none,
      unmergable: Option.none,
      selection: $_fdch7uk7jfuvixxb.constant($_fnw8s9lrjfuviy6g.selection(cell, selections)),
      clipboard: $_fdch7uk7jfuvixxb.constant(clipboard),
      generators: $_fdch7uk7jfuvixxb.constant(generators)
    };
  };
  var $_fgxiq3lqjfuviy6a = {
    noMenu: noMenu,
    forMenu: forMenu,
    notCell: notCell$1,
    paste: paste$1,
    pasteRows: pasteRows
  };

  var extractSelected = function (cells) {
    return $_dmqxswkhjfuvixyz.table(cells[0]).map($_1i39h2lkjfuviy5n.deep).map(function (replica) {
      return [$_6u5qawk9jfuvixxg.extract(replica, $_g37vw7m5jfuviyax.attributeSelector())];
    });
  };
  var serializeElement = function (editor, elm) {
    return editor.selection.serializer.serialize(elm.dom(), {});
  };
  var registerEvents = function (editor, selections, actions, cellSelection) {
    editor.on('BeforeGetContent', function (e) {
      var multiCellContext = function (cells) {
        e.preventDefault();
        extractSelected(cells).each(function (elements) {
          e.content = $_4jja6kk5jfuvixx1.map(elements, function (elm) {
            return serializeElement(editor, elm);
          }).join('');
        });
      };
      if (e.selection === true) {
        $_1jmyeim6jfuviyb0.cata(selections.get(), $_fdch7uk7jfuvixxb.noop, multiCellContext, $_fdch7uk7jfuvixxb.noop);
      }
    });
    editor.on('BeforeSetContent', function (e) {
      if (e.selection === true && e.paste === true) {
        var cellOpt = Option.from(editor.dom.getParent(editor.selection.getStart(), 'th,td'));
        cellOpt.each(function (domCell) {
          var cell = $_4sdhm4kkjfuviy0e.fromDom(domCell);
          var table = $_dmqxswkhjfuvixyz.table(cell);
          table.bind(function (table) {
            var elements = $_4jja6kk5jfuvixx1.filter($_ek5zoelpjfuviy65.fromHtml(e.content), function (content) {
              return $_6mcqmml6jfuviy2u.name(content) !== 'meta';
            });
            if (elements.length === 1 && $_6mcqmml6jfuviy2u.name(elements[0]) === 'table') {
              e.preventDefault();
              var doc = $_4sdhm4kkjfuviy0e.fromDom(editor.getDoc());
              var generators = $_clky4ljjfuviy4t.paste(doc);
              var targets = $_fgxiq3lqjfuviy6a.paste(cell, elements[0], generators);
              actions.pasteCells(table, targets).each(function (rng) {
                editor.selection.setRng(rng);
                editor.focus();
                cellSelection.clear(table);
              });
            }
          });
        });
      }
    });
  };
  var $_jmgm9k4jfuvixw9 = { registerEvents: registerEvents };

  function Dimension (name, getOffset) {
    var set = function (element, h) {
      if (!$_13kw1fk8jfuvixxd.isNumber(h) && !h.match(/^[0-9]+$/))
        throw name + '.set accepts only positive integer values. Value was ' + h;
      var dom = element.dom();
      if ($_3bn25blfjfuviy4f.isSupported(dom))
        dom.style[name] = h + 'px';
    };
    var get = function (element) {
      var r = getOffset(element);
      if (r <= 0 || r === null) {
        var css = $_2lr8nrlejfuviy40.get(element, name);
        return parseFloat(css) || 0;
      }
      return r;
    };
    var getOuter = get;
    var aggregate = function (element, properties) {
      return $_4jja6kk5jfuvixx1.foldl(properties, function (acc, property) {
        var val = $_2lr8nrlejfuviy40.get(element, property);
        var value = val === undefined ? 0 : parseInt(val, 10);
        return isNaN(value) ? acc : acc + value;
      }, 0);
    };
    var max = function (element, value, properties) {
      var cumulativeInclusions = aggregate(element, properties);
      var absoluteMax = value > cumulativeInclusions ? value - cumulativeInclusions : 0;
      return absoluteMax;
    };
    return {
      set: set,
      get: get,
      getOuter: getOuter,
      aggregate: aggregate,
      max: max
    };
  }

  var api$1 = Dimension('height', function (element) {
    return $_43dxxcl9jfuviy31.inBody(element) ? element.dom().getBoundingClientRect().height : element.dom().offsetHeight;
  });
  var set$3 = function (element, h) {
    api$1.set(element, h);
  };
  var get$3 = function (element) {
    return api$1.get(element);
  };
  var getOuter = function (element) {
    return api$1.getOuter(element);
  };
  var setMax = function (element, value) {
    var inclusions = [
      'margin-top',
      'border-top-width',
      'padding-top',
      'padding-bottom',
      'border-bottom-width',
      'margin-bottom'
    ];
    var absMax = api$1.max(element, value, inclusions);
    $_2lr8nrlejfuviy40.set(element, 'max-height', absMax + 'px');
  };
  var $_e3phjumcjfuviyct = {
    set: set$3,
    get: get$3,
    getOuter: getOuter,
    setMax: setMax
  };

  var api$2 = Dimension('width', function (element) {
    return element.dom().offsetWidth;
  });
  var set$4 = function (element, h) {
    api$2.set(element, h);
  };
  var get$4 = function (element) {
    return api$2.get(element);
  };
  var getOuter$1 = function (element) {
    return api$2.getOuter(element);
  };
  var setMax$1 = function (element, value) {
    var inclusions = [
      'margin-left',
      'border-left-width',
      'padding-left',
      'padding-right',
      'border-right-width',
      'margin-right'
    ];
    var absMax = api$2.max(element, value, inclusions);
    $_2lr8nrlejfuviy40.set(element, 'max-width', absMax + 'px');
  };
  var $_4u9lbfmejfuviyd2 = {
    set: set$4,
    get: get$4,
    getOuter: getOuter$1,
    setMax: setMax$1
  };

  var platform = $_8chrc7ktjfuviy1m.detect();
  var needManualCalc = function () {
    return platform.browser.isIE() || platform.browser.isEdge();
  };
  var toNumber = function (px, fallback) {
    var num = parseFloat(px);
    return isNaN(num) ? fallback : num;
  };
  var getProp = function (elm, name, fallback) {
    return toNumber($_2lr8nrlejfuviy40.get(elm, name), fallback);
  };
  var getCalculatedHeight = function (cell) {
    var paddingTop = getProp(cell, 'padding-top', 0);
    var paddingBottom = getProp(cell, 'padding-bottom', 0);
    var borderTop = getProp(cell, 'border-top-width', 0);
    var borderBottom = getProp(cell, 'border-bottom-width', 0);
    var height = cell.dom().getBoundingClientRect().height;
    var boxSizing = $_2lr8nrlejfuviy40.get(cell, 'box-sizing');
    var borders = borderTop + borderBottom;
    return boxSizing === 'border-box' ? height : height - paddingTop - paddingBottom - borders;
  };
  var getWidth = function (cell) {
    return getProp(cell, 'width', $_4u9lbfmejfuviyd2.get(cell));
  };
  var getHeight = function (cell) {
    return needManualCalc() ? getCalculatedHeight(cell) : getProp(cell, 'height', $_e3phjumcjfuviyct.get(cell));
  };
  var $_d797pzmbjfuviyci = {
    getWidth: getWidth,
    getHeight: getHeight
  };

  var genericSizeRegex = /(\d+(\.\d+)?)(\w|%)*/;
  var percentageBasedSizeRegex = /(\d+(\.\d+)?)%/;
  var pixelBasedSizeRegex = /(\d+(\.\d+)?)px|em/;
  var setPixelWidth = function (cell, amount) {
    $_2lr8nrlejfuviy40.set(cell, 'width', amount + 'px');
  };
  var setPercentageWidth = function (cell, amount) {
    $_2lr8nrlejfuviy40.set(cell, 'width', amount + '%');
  };
  var setHeight = function (cell, amount) {
    $_2lr8nrlejfuviy40.set(cell, 'height', amount + 'px');
  };
  var getHeightValue = function (cell) {
    return $_2lr8nrlejfuviy40.getRaw(cell, 'height').getOrThunk(function () {
      return $_d797pzmbjfuviyci.getHeight(cell) + 'px';
    });
  };
  var convert = function (cell, number, getter, setter) {
    var newSize = $_dmqxswkhjfuvixyz.table(cell).map(function (table) {
      var total = getter(table);
      return Math.floor(number / 100 * total);
    }).getOr(number);
    setter(cell, newSize);
    return newSize;
  };
  var normalizePixelSize = function (value, cell, getter, setter) {
    var number = parseInt(value, 10);
    return $_2rmckll2jfuviy2d.endsWith(value, '%') && $_6mcqmml6jfuviy2u.name(cell) !== 'table' ? convert(cell, number, getter, setter) : number;
  };
  var getTotalHeight = function (cell) {
    var value = getHeightValue(cell);
    if (!value)
      return $_e3phjumcjfuviyct.get(cell);
    return normalizePixelSize(value, cell, $_e3phjumcjfuviyct.get, setHeight);
  };
  var get$5 = function (cell, type, f) {
    var v = f(cell);
    var span = getSpan(cell, type);
    return v / span;
  };
  var getSpan = function (cell, type) {
    return $_2ekobel5jfuviy2m.has(cell, type) ? parseInt($_2ekobel5jfuviy2m.get(cell, type), 10) : 1;
  };
  var getRawWidth = function (element) {
    var cssWidth = $_2lr8nrlejfuviy40.getRaw(element, 'width');
    return cssWidth.fold(function () {
      return Option.from($_2ekobel5jfuviy2m.get(element, 'width'));
    }, function (width) {
      return Option.some(width);
    });
  };
  var normalizePercentageWidth = function (cellWidth, tableSize) {
    return cellWidth / tableSize.pixelWidth() * 100;
  };
  var choosePercentageSize = function (element, width, tableSize) {
    if (percentageBasedSizeRegex.test(width)) {
      var percentMatch = percentageBasedSizeRegex.exec(width);
      return parseFloat(percentMatch[1]);
    } else {
      var fallbackWidth = $_4u9lbfmejfuviyd2.get(element);
      var intWidth = parseInt(fallbackWidth, 10);
      return normalizePercentageWidth(intWidth, tableSize);
    }
  };
  var getPercentageWidth = function (cell, tableSize) {
    var width = getRawWidth(cell);
    return width.fold(function () {
      var width = $_4u9lbfmejfuviyd2.get(cell);
      var intWidth = parseInt(width, 10);
      return normalizePercentageWidth(intWidth, tableSize);
    }, function (width) {
      return choosePercentageSize(cell, width, tableSize);
    });
  };
  var normalizePixelWidth = function (cellWidth, tableSize) {
    return cellWidth / 100 * tableSize.pixelWidth();
  };
  var choosePixelSize = function (element, width, tableSize) {
    if (pixelBasedSizeRegex.test(width)) {
      var pixelMatch = pixelBasedSizeRegex.exec(width);
      return parseInt(pixelMatch[1], 10);
    } else if (percentageBasedSizeRegex.test(width)) {
      var percentMatch = percentageBasedSizeRegex.exec(width);
      var floatWidth = parseFloat(percentMatch[1]);
      return normalizePixelWidth(floatWidth, tableSize);
    } else {
      var fallbackWidth = $_4u9lbfmejfuviyd2.get(element);
      return parseInt(fallbackWidth, 10);
    }
  };
  var getPixelWidth = function (cell, tableSize) {
    var width = getRawWidth(cell);
    return width.fold(function () {
      var width = $_4u9lbfmejfuviyd2.get(cell);
      var intWidth = parseInt(width, 10);
      return intWidth;
    }, function (width) {
      return choosePixelSize(cell, width, tableSize);
    });
  };
  var getHeight$1 = function (cell) {
    return get$5(cell, 'rowspan', getTotalHeight);
  };
  var getGenericWidth = function (cell) {
    var width = getRawWidth(cell);
    return width.bind(function (width) {
      if (genericSizeRegex.test(width)) {
        var match = genericSizeRegex.exec(width);
        return Option.some({
          width: $_fdch7uk7jfuvixxb.constant(match[1]),
          unit: $_fdch7uk7jfuvixxb.constant(match[3])
        });
      } else {
        return Option.none();
      }
    });
  };
  var setGenericWidth = function (cell, amount, unit) {
    $_2lr8nrlejfuviy40.set(cell, 'width', amount + unit);
  };
  var $_9h89znmajfuviybw = {
    percentageBasedSizeRegex: $_fdch7uk7jfuvixxb.constant(percentageBasedSizeRegex),
    pixelBasedSizeRegex: $_fdch7uk7jfuvixxb.constant(pixelBasedSizeRegex),
    setPixelWidth: setPixelWidth,
    setPercentageWidth: setPercentageWidth,
    setHeight: setHeight,
    getPixelWidth: getPixelWidth,
    getPercentageWidth: getPercentageWidth,
    getGenericWidth: getGenericWidth,
    setGenericWidth: setGenericWidth,
    getHeight: getHeight$1,
    getRawWidth: getRawWidth
  };

  var halve = function (main, other) {
    var width = $_9h89znmajfuviybw.getGenericWidth(main);
    width.each(function (width) {
      var newWidth = width.width() / 2;
      $_9h89znmajfuviybw.setGenericWidth(main, newWidth, width.unit());
      $_9h89znmajfuviybw.setGenericWidth(other, newWidth, width.unit());
    });
  };
  var $_7fw3p6m9jfuviybu = { halve: halve };

  var attached = function (element, scope) {
    var doc = scope || $_4sdhm4kkjfuviy0e.fromDom(document.documentElement);
    return $_eg4f87lbjfuviy37.ancestor(element, $_fdch7uk7jfuvixxb.curry($_g6ztqikojfuviy13.eq, doc)).isSome();
  };
  var windowOf = function (element) {
    var dom = element.dom();
    if (dom === dom.window)
      return element;
    return $_6mcqmml6jfuviy2u.isDocument(element) ? dom.defaultView || dom.parentWindow : null;
  };
  var $_1liz6umjjfuviydr = {
    attached: attached,
    windowOf: windowOf
  };

  var r = function (left, top) {
    var translate = function (x, y) {
      return r(left + x, top + y);
    };
    return {
      left: $_fdch7uk7jfuvixxb.constant(left),
      top: $_fdch7uk7jfuvixxb.constant(top),
      translate: translate
    };
  };

  var boxPosition = function (dom) {
    var box = dom.getBoundingClientRect();
    return r(box.left, box.top);
  };
  var firstDefinedOrZero = function (a, b) {
    return a !== undefined ? a : b !== undefined ? b : 0;
  };
  var absolute = function (element) {
    var doc = element.dom().ownerDocument;
    var body = doc.body;
    var win = $_1liz6umjjfuviydr.windowOf($_4sdhm4kkjfuviy0e.fromDom(doc));
    var html = doc.documentElement;
    var scrollTop = firstDefinedOrZero(win.pageYOffset, html.scrollTop);
    var scrollLeft = firstDefinedOrZero(win.pageXOffset, html.scrollLeft);
    var clientTop = firstDefinedOrZero(html.clientTop, body.clientTop);
    var clientLeft = firstDefinedOrZero(html.clientLeft, body.clientLeft);
    return viewport(element).translate(scrollLeft - clientLeft, scrollTop - clientTop);
  };
  var relative = function (element) {
    var dom = element.dom();
    return r(dom.offsetLeft, dom.offsetTop);
  };
  var viewport = function (element) {
    var dom = element.dom();
    var doc = dom.ownerDocument;
    var body = doc.body;
    var html = $_4sdhm4kkjfuviy0e.fromDom(doc.documentElement);
    if (body === dom)
      return r(body.offsetLeft, body.offsetTop);
    if (!$_1liz6umjjfuviydr.attached(element, html))
      return r(0, 0);
    return boxPosition(dom);
  };
  var $_6ti42xmijfuviydp = {
    absolute: absolute,
    relative: relative,
    viewport: viewport
  };

  var rowInfo = $_96oqrskbjfuvixya.immutable('row', 'y');
  var colInfo = $_96oqrskbjfuvixya.immutable('col', 'x');
  var rtlEdge = function (cell) {
    var pos = $_6ti42xmijfuviydp.absolute(cell);
    return pos.left() + $_4u9lbfmejfuviyd2.getOuter(cell);
  };
  var ltrEdge = function (cell) {
    return $_6ti42xmijfuviydp.absolute(cell).left();
  };
  var getLeftEdge = function (index, cell) {
    return colInfo(index, ltrEdge(cell));
  };
  var getRightEdge = function (index, cell) {
    return colInfo(index, rtlEdge(cell));
  };
  var getTop = function (cell) {
    return $_6ti42xmijfuviydp.absolute(cell).top();
  };
  var getTopEdge = function (index, cell) {
    return rowInfo(index, getTop(cell));
  };
  var getBottomEdge = function (index, cell) {
    return rowInfo(index, getTop(cell) + $_e3phjumcjfuviyct.getOuter(cell));
  };
  var findPositions = function (getInnerEdge, getOuterEdge, array) {
    if (array.length === 0)
      return [];
    var lines = $_4jja6kk5jfuvixx1.map(array.slice(1), function (cellOption, index) {
      return cellOption.map(function (cell) {
        return getInnerEdge(index, cell);
      });
    });
    var lastLine = array[array.length - 1].map(function (cell) {
      return getOuterEdge(array.length - 1, cell);
    });
    return lines.concat([lastLine]);
  };
  var negate = function (step, _table) {
    return -step;
  };
  var height = {
    delta: $_fdch7uk7jfuvixxb.identity,
    positions: $_fdch7uk7jfuvixxb.curry(findPositions, getTopEdge, getBottomEdge),
    edge: getTop
  };
  var ltr = {
    delta: $_fdch7uk7jfuvixxb.identity,
    edge: ltrEdge,
    positions: $_fdch7uk7jfuvixxb.curry(findPositions, getLeftEdge, getRightEdge)
  };
  var rtl = {
    delta: negate,
    edge: rtlEdge,
    positions: $_fdch7uk7jfuvixxb.curry(findPositions, getRightEdge, getLeftEdge)
  };
  var $_bj1b3kmhjfuviyd6 = {
    height: height,
    rtl: rtl,
    ltr: ltr
  };

  var $_66dmn1mgjfuviyd5 = {
    ltr: $_bj1b3kmhjfuviyd6.ltr,
    rtl: $_bj1b3kmhjfuviyd6.rtl
  };

  function TableDirection (directionAt) {
    var auto = function (table) {
      return directionAt(table).isRtl() ? $_66dmn1mgjfuviyd5.rtl : $_66dmn1mgjfuviyd5.ltr;
    };
    var delta = function (amount, table) {
      return auto(table).delta(amount, table);
    };
    var positions = function (cols, table) {
      return auto(table).positions(cols, table);
    };
    var edge = function (cell) {
      return auto(cell).edge(cell);
    };
    return {
      delta: delta,
      edge: edge,
      positions: positions
    };
  }

  var getGridSize = function (table) {
    var input = $_2vc4gykfjfuvixyi.fromTable(table);
    var warehouse = $_bwrthsldjfuviy3q.generate(input);
    return warehouse.grid();
  };
  var $_23hnlomljfuviydz = { getGridSize: getGridSize };

  var Cell = function (initial) {
    var value = initial;
    var get = function () {
      return value;
    };
    var set = function (v) {
      value = v;
    };
    var clone = function () {
      return Cell(get());
    };
    return {
      get: get,
      set: set,
      clone: clone
    };
  };

  var base = function (handleUnsupported, required) {
    return baseWith(handleUnsupported, required, {
      validate: $_13kw1fk8jfuvixxd.isFunction,
      label: 'function'
    });
  };
  var baseWith = function (handleUnsupported, required, pred) {
    if (required.length === 0)
      throw new Error('You must specify at least one required field.');
    $_cov59pkejfuvixyf.validateStrArr('required', required);
    $_cov59pkejfuvixyf.checkDupes(required);
    return function (obj) {
      var keys = $_afb9m6kajfuvixy8.keys(obj);
      var allReqd = $_4jja6kk5jfuvixx1.forall(required, function (req) {
        return $_4jja6kk5jfuvixx1.contains(keys, req);
      });
      if (!allReqd)
        $_cov59pkejfuvixyf.reqMessage(required, keys);
      handleUnsupported(required, keys);
      var invalidKeys = $_4jja6kk5jfuvixx1.filter(required, function (key) {
        return !pred.validate(obj[key], key);
      });
      if (invalidKeys.length > 0)
        $_cov59pkejfuvixyf.invalidTypeMessage(invalidKeys, pred.label);
      return obj;
    };
  };
  var handleExact = function (required, keys) {
    var unsupported = $_4jja6kk5jfuvixx1.filter(keys, function (key) {
      return !$_4jja6kk5jfuvixx1.contains(required, key);
    });
    if (unsupported.length > 0)
      $_cov59pkejfuvixyf.unsuppMessage(unsupported);
  };
  var allowExtra = $_fdch7uk7jfuvixxb.noop;
  var $_5ebbzmpjfuviyf6 = {
    exactly: $_fdch7uk7jfuvixxb.curry(base, handleExact),
    ensure: $_fdch7uk7jfuvixxb.curry(base, allowExtra),
    ensureWith: $_fdch7uk7jfuvixxb.curry(baseWith, allowExtra)
  };

  var elementToData = function (element) {
    var colspan = $_2ekobel5jfuviy2m.has(element, 'colspan') ? parseInt($_2ekobel5jfuviy2m.get(element, 'colspan'), 10) : 1;
    var rowspan = $_2ekobel5jfuviy2m.has(element, 'rowspan') ? parseInt($_2ekobel5jfuviy2m.get(element, 'rowspan'), 10) : 1;
    return {
      element: $_fdch7uk7jfuvixxb.constant(element),
      colspan: $_fdch7uk7jfuvixxb.constant(colspan),
      rowspan: $_fdch7uk7jfuvixxb.constant(rowspan)
    };
  };
  var modification = function (generators, _toData) {
    contract(generators);
    var position = Cell(Option.none());
    var toData = _toData !== undefined ? _toData : elementToData;
    var nu = function (data) {
      return generators.cell(data);
    };
    var nuFrom = function (element) {
      var data = toData(element);
      return nu(data);
    };
    var add = function (element) {
      var replacement = nuFrom(element);
      if (position.get().isNone())
        position.set(Option.some(replacement));
      recent = Option.some({
        item: element,
        replacement: replacement
      });
      return replacement;
    };
    var recent = Option.none();
    var getOrInit = function (element, comparator) {
      return recent.fold(function () {
        return add(element);
      }, function (p) {
        return comparator(element, p.item) ? p.replacement : add(element);
      });
    };
    return {
      getOrInit: getOrInit,
      cursor: position.get
    };
  };
  var transform = function (scope, tag) {
    return function (generators) {
      var position = Cell(Option.none());
      contract(generators);
      var list = [];
      var find = function (element, comparator) {
        return $_4jja6kk5jfuvixx1.find(list, function (x) {
          return comparator(x.item, element);
        });
      };
      var makeNew = function (element) {
        var cell = generators.replace(element, tag, { scope: scope });
        list.push({
          item: element,
          sub: cell
        });
        if (position.get().isNone())
          position.set(Option.some(cell));
        return cell;
      };
      var replaceOrInit = function (element, comparator) {
        return find(element, comparator).fold(function () {
          return makeNew(element);
        }, function (p) {
          return comparator(element, p.item) ? p.sub : makeNew(element);
        });
      };
      return {
        replaceOrInit: replaceOrInit,
        cursor: position.get
      };
    };
  };
  var merging = function (generators) {
    contract(generators);
    var position = Cell(Option.none());
    var combine = function (cell) {
      if (position.get().isNone())
        position.set(Option.some(cell));
      return function () {
        var raw = generators.cell({
          element: $_fdch7uk7jfuvixxb.constant(cell),
          colspan: $_fdch7uk7jfuvixxb.constant(1),
          rowspan: $_fdch7uk7jfuvixxb.constant(1)
        });
        $_2lr8nrlejfuviy40.remove(raw, 'width');
        $_2lr8nrlejfuviy40.remove(cell, 'width');
        return raw;
      };
    };
    return {
      combine: combine,
      cursor: position.get
    };
  };
  var contract = $_5ebbzmpjfuviyf6.exactly([
    'cell',
    'row',
    'replace',
    'gap'
  ]);
  var $_blm67mmnjfuviyel = {
    modification: modification,
    transform: transform,
    merging: merging
  };

  var blockList = [
    'body',
    'p',
    'div',
    'article',
    'aside',
    'figcaption',
    'figure',
    'footer',
    'header',
    'nav',
    'section',
    'ol',
    'ul',
    'table',
    'thead',
    'tfoot',
    'tbody',
    'caption',
    'tr',
    'td',
    'th',
    'h1',
    'h2',
    'h3',
    'h4',
    'h5',
    'h6',
    'blockquote',
    'pre',
    'address'
  ];
  var isList = function (universe, item) {
    var tagName = universe.property().name(item);
    return $_4jja6kk5jfuvixx1.contains([
      'ol',
      'ul'
    ], tagName);
  };
  var isBlock = function (universe, item) {
    var tagName = universe.property().name(item);
    return $_4jja6kk5jfuvixx1.contains(blockList, tagName);
  };
  var isFormatting = function (universe, item) {
    var tagName = universe.property().name(item);
    return $_4jja6kk5jfuvixx1.contains([
      'address',
      'pre',
      'p',
      'h1',
      'h2',
      'h3',
      'h4',
      'h5',
      'h6'
    ], tagName);
  };
  var isHeading = function (universe, item) {
    var tagName = universe.property().name(item);
    return $_4jja6kk5jfuvixx1.contains([
      'h1',
      'h2',
      'h3',
      'h4',
      'h5',
      'h6'
    ], tagName);
  };
  var isContainer = function (universe, item) {
    return $_4jja6kk5jfuvixx1.contains([
      'div',
      'li',
      'td',
      'th',
      'blockquote',
      'body',
      'caption'
    ], universe.property().name(item));
  };
  var isEmptyTag = function (universe, item) {
    return $_4jja6kk5jfuvixx1.contains([
      'br',
      'img',
      'hr',
      'input'
    ], universe.property().name(item));
  };
  var isFrame = function (universe, item) {
    return universe.property().name(item) === 'iframe';
  };
  var isInline = function (universe, item) {
    return !(isBlock(universe, item) || isEmptyTag(universe, item)) && universe.property().name(item) !== 'li';
  };
  var $_2jungumsjfuviyg6 = {
    isBlock: isBlock,
    isList: isList,
    isFormatting: isFormatting,
    isHeading: isHeading,
    isContainer: isContainer,
    isEmptyTag: isEmptyTag,
    isFrame: isFrame,
    isInline: isInline
  };

  var universe$1 = DomUniverse();
  var isBlock$1 = function (element) {
    return $_2jungumsjfuviyg6.isBlock(universe$1, element);
  };
  var isList$1 = function (element) {
    return $_2jungumsjfuviyg6.isList(universe$1, element);
  };
  var isFormatting$1 = function (element) {
    return $_2jungumsjfuviyg6.isFormatting(universe$1, element);
  };
  var isHeading$1 = function (element) {
    return $_2jungumsjfuviyg6.isHeading(universe$1, element);
  };
  var isContainer$1 = function (element) {
    return $_2jungumsjfuviyg6.isContainer(universe$1, element);
  };
  var isEmptyTag$1 = function (element) {
    return $_2jungumsjfuviyg6.isEmptyTag(universe$1, element);
  };
  var isFrame$1 = function (element) {
    return $_2jungumsjfuviyg6.isFrame(universe$1, element);
  };
  var isInline$1 = function (element) {
    return $_2jungumsjfuviyg6.isInline(universe$1, element);
  };
  var $_fkl6wvmrjfuviyg2 = {
    isBlock: isBlock$1,
    isList: isList$1,
    isFormatting: isFormatting$1,
    isHeading: isHeading$1,
    isContainer: isContainer$1,
    isEmptyTag: isEmptyTag$1,
    isFrame: isFrame$1,
    isInline: isInline$1
  };

  var merge = function (cells) {
    var isBr = function (el) {
      return $_6mcqmml6jfuviy2u.name(el) === 'br';
    };
    var advancedBr = function (children) {
      return $_4jja6kk5jfuvixx1.forall(children, function (c) {
        return isBr(c) || $_6mcqmml6jfuviy2u.isText(c) && $_cvagqflnjfuviy5y.get(c).trim().length === 0;
      });
    };
    var isListItem = function (el) {
      return $_6mcqmml6jfuviy2u.name(el) === 'li' || $_eg4f87lbjfuviy37.ancestor(el, $_fkl6wvmrjfuviyg2.isList).isSome();
    };
    var siblingIsBlock = function (el) {
      return $_87w3h3kmjfuviy0m.nextSibling(el).map(function (rightSibling) {
        if ($_fkl6wvmrjfuviyg2.isBlock(rightSibling))
          return true;
        if ($_fkl6wvmrjfuviyg2.isEmptyTag(rightSibling)) {
          return $_6mcqmml6jfuviy2u.name(rightSibling) === 'img' ? false : true;
        }
      }).getOr(false);
    };
    var markCell = function (cell) {
      return $_dsfijblljfuviy5q.last(cell).bind(function (rightEdge) {
        var rightSiblingIsBlock = siblingIsBlock(rightEdge);
        return $_87w3h3kmjfuviy0m.parent(rightEdge).map(function (parent) {
          return rightSiblingIsBlock === true || isListItem(parent) || isBr(rightEdge) || $_fkl6wvmrjfuviyg2.isBlock(parent) && !$_g6ztqikojfuviy13.eq(cell, parent) ? [] : [$_4sdhm4kkjfuviy0e.fromTag('br')];
        });
      }).getOr([]);
    };
    var markContent = function () {
      var content = $_4jja6kk5jfuvixx1.bind(cells, function (cell) {
        var children = $_87w3h3kmjfuviy0m.children(cell);
        return advancedBr(children) ? [] : children.concat(markCell(cell));
      });
      return content.length === 0 ? [$_4sdhm4kkjfuviy0e.fromTag('br')] : content;
    };
    var contents = markContent();
    $_5ud3colhjfuviy4l.empty(cells[0]);
    $_44mr3plijfuviy4p.append(cells[0], contents);
  };
  var $_af8qgymqjfuviyf9 = { merge: merge };

  var shallow$1 = function (old, nu) {
    return nu;
  };
  var deep$1 = function (old, nu) {
    var bothObjects = $_13kw1fk8jfuvixxd.isObject(old) && $_13kw1fk8jfuvixxd.isObject(nu);
    return bothObjects ? deepMerge(old, nu) : nu;
  };
  var baseMerge = function (merger) {
    return function () {
      var objects = new Array(arguments.length);
      for (var i = 0; i < objects.length; i++)
        objects[i] = arguments[i];
      if (objects.length === 0)
        throw new Error('Can\'t merge zero objects');
      var ret = {};
      for (var j = 0; j < objects.length; j++) {
        var curObject = objects[j];
        for (var key in curObject)
          if (curObject.hasOwnProperty(key)) {
            ret[key] = merger(ret[key], curObject[key]);
          }
      }
      return ret;
    };
  };
  var deepMerge = baseMerge(deep$1);
  var merge$1 = baseMerge(shallow$1);
  var $_8d366dmujfuviygv = {
    deepMerge: deepMerge,
    merge: merge$1
  };

  var cat = function (arr) {
    var r = [];
    var push = function (x) {
      r.push(x);
    };
    for (var i = 0; i < arr.length; i++) {
      arr[i].each(push);
    }
    return r;
  };
  var findMap = function (arr, f) {
    for (var i = 0; i < arr.length; i++) {
      var r = f(arr[i], i);
      if (r.isSome()) {
        return r;
      }
    }
    return Option.none();
  };
  var liftN = function (arr, f) {
    var r = [];
    for (var i = 0; i < arr.length; i++) {
      var x = arr[i];
      if (x.isSome()) {
        r.push(x.getOrDie());
      } else {
        return Option.none();
      }
    }
    return Option.some(f.apply(null, r));
  };
  var $_6i1r9dmvjfuviygw = {
    cat: cat,
    findMap: findMap,
    liftN: liftN
  };

  var addCell = function (gridRow, index, cell) {
    var cells = gridRow.cells();
    var before = cells.slice(0, index);
    var after = cells.slice(index);
    var newCells = before.concat([cell]).concat(after);
    return setCells(gridRow, newCells);
  };
  var mutateCell = function (gridRow, index, cell) {
    var cells = gridRow.cells();
    cells[index] = cell;
  };
  var setCells = function (gridRow, cells) {
    return $_g02m1vkgjfuvixyt.rowcells(cells, gridRow.section());
  };
  var mapCells = function (gridRow, f) {
    var cells = gridRow.cells();
    var r = $_4jja6kk5jfuvixx1.map(cells, f);
    return $_g02m1vkgjfuvixyt.rowcells(r, gridRow.section());
  };
  var getCell = function (gridRow, index) {
    return gridRow.cells()[index];
  };
  var getCellElement = function (gridRow, index) {
    return getCell(gridRow, index).element();
  };
  var cellLength = function (gridRow) {
    return gridRow.cells().length;
  };
  var $_f8bgbgmyjfuviyha = {
    addCell: addCell,
    setCells: setCells,
    mutateCell: mutateCell,
    getCell: getCell,
    getCellElement: getCellElement,
    mapCells: mapCells,
    cellLength: cellLength
  };

  var getColumn = function (grid, index) {
    return $_4jja6kk5jfuvixx1.map(grid, function (row) {
      return $_f8bgbgmyjfuviyha.getCell(row, index);
    });
  };
  var getRow = function (grid, index) {
    return grid[index];
  };
  var findDiff = function (xs, comp) {
    if (xs.length === 0)
      return 0;
    var first = xs[0];
    var index = $_4jja6kk5jfuvixx1.findIndex(xs, function (x) {
      return !comp(first.element(), x.element());
    });
    return index.fold(function () {
      return xs.length;
    }, function (ind) {
      return ind;
    });
  };
  var subgrid = function (grid, row, column, comparator) {
    var restOfRow = getRow(grid, row).cells().slice(column);
    var endColIndex = findDiff(restOfRow, comparator);
    var restOfColumn = getColumn(grid, column).slice(row);
    var endRowIndex = findDiff(restOfColumn, comparator);
    return {
      colspan: $_fdch7uk7jfuvixxb.constant(endColIndex),
      rowspan: $_fdch7uk7jfuvixxb.constant(endRowIndex)
    };
  };
  var $_gf3zj4mxjfuviyh3 = { subgrid: subgrid };

  var toDetails = function (grid, comparator) {
    var seen = $_4jja6kk5jfuvixx1.map(grid, function (row, ri) {
      return $_4jja6kk5jfuvixx1.map(row.cells(), function (col, ci) {
        return false;
      });
    });
    var updateSeen = function (ri, ci, rowspan, colspan) {
      for (var r = ri; r < ri + rowspan; r++) {
        for (var c = ci; c < ci + colspan; c++) {
          seen[r][c] = true;
        }
      }
    };
    return $_4jja6kk5jfuvixx1.map(grid, function (row, ri) {
      var details = $_4jja6kk5jfuvixx1.bind(row.cells(), function (cell, ci) {
        if (seen[ri][ci] === false) {
          var result = $_gf3zj4mxjfuviyh3.subgrid(grid, ri, ci, comparator);
          updateSeen(ri, ci, result.rowspan(), result.colspan());
          return [$_g02m1vkgjfuvixyt.detailnew(cell.element(), result.rowspan(), result.colspan(), cell.isNew())];
        } else {
          return [];
        }
      });
      return $_g02m1vkgjfuvixyt.rowdetails(details, row.section());
    });
  };
  var toGrid = function (warehouse, generators, isNew) {
    var grid = [];
    for (var i = 0; i < warehouse.grid().rows(); i++) {
      var rowCells = [];
      for (var j = 0; j < warehouse.grid().columns(); j++) {
        var element = $_bwrthsldjfuviy3q.getAt(warehouse, i, j).map(function (item) {
          return $_g02m1vkgjfuvixyt.elementnew(item.element(), isNew);
        }).getOrThunk(function () {
          return $_g02m1vkgjfuvixyt.elementnew(generators.gap(), true);
        });
        rowCells.push(element);
      }
      var row = $_g02m1vkgjfuvixyt.rowcells(rowCells, warehouse.all()[i].section());
      grid.push(row);
    }
    return grid;
  };
  var $_b883t1mwjfuviygz = {
    toDetails: toDetails,
    toGrid: toGrid
  };

  var setIfNot = function (element, property, value, ignore) {
    if (value === ignore)
      $_2ekobel5jfuviy2m.remove(element, property);
    else
      $_2ekobel5jfuviy2m.set(element, property, value);
  };
  var render = function (table, grid) {
    var newRows = [];
    var newCells = [];
    var renderSection = function (gridSection, sectionName) {
      var section = $_26gnp6lajfuviy35.child(table, sectionName).getOrThunk(function () {
        var tb = $_4sdhm4kkjfuviy0e.fromTag(sectionName, $_87w3h3kmjfuviy0m.owner(table).dom());
        $_5zcsfmlgjfuviy4g.append(table, tb);
        return tb;
      });
      $_5ud3colhjfuviy4l.empty(section);
      var rows = $_4jja6kk5jfuvixx1.map(gridSection, function (row) {
        if (row.isNew()) {
          newRows.push(row.element());
        }
        var tr = row.element();
        $_5ud3colhjfuviy4l.empty(tr);
        $_4jja6kk5jfuvixx1.each(row.cells(), function (cell) {
          if (cell.isNew()) {
            newCells.push(cell.element());
          }
          setIfNot(cell.element(), 'colspan', cell.colspan(), 1);
          setIfNot(cell.element(), 'rowspan', cell.rowspan(), 1);
          $_5zcsfmlgjfuviy4g.append(tr, cell.element());
        });
        return tr;
      });
      $_44mr3plijfuviy4p.append(section, rows);
    };
    var removeSection = function (sectionName) {
      $_26gnp6lajfuviy35.child(table, sectionName).bind($_5ud3colhjfuviy4l.remove);
    };
    var renderOrRemoveSection = function (gridSection, sectionName) {
      if (gridSection.length > 0) {
        renderSection(gridSection, sectionName);
      } else {
        removeSection(sectionName);
      }
    };
    var headSection = [];
    var bodySection = [];
    var footSection = [];
    $_4jja6kk5jfuvixx1.each(grid, function (row) {
      switch (row.section()) {
      case 'thead':
        headSection.push(row);
        break;
      case 'tbody':
        bodySection.push(row);
        break;
      case 'tfoot':
        footSection.push(row);
        break;
      }
    });
    renderOrRemoveSection(headSection, 'thead');
    renderOrRemoveSection(bodySection, 'tbody');
    renderOrRemoveSection(footSection, 'tfoot');
    return {
      newRows: $_fdch7uk7jfuvixxb.constant(newRows),
      newCells: $_fdch7uk7jfuvixxb.constant(newCells)
    };
  };
  var copy$2 = function (grid) {
    var rows = $_4jja6kk5jfuvixx1.map(grid, function (row) {
      var tr = $_1i39h2lkjfuviy5n.shallow(row.element());
      $_4jja6kk5jfuvixx1.each(row.cells(), function (cell) {
        var clonedCell = $_1i39h2lkjfuviy5n.deep(cell.element());
        setIfNot(clonedCell, 'colspan', cell.colspan(), 1);
        setIfNot(clonedCell, 'rowspan', cell.rowspan(), 1);
        $_5zcsfmlgjfuviy4g.append(tr, clonedCell);
      });
      return tr;
    });
    return rows;
  };
  var $_clwdhumzjfuviyhf = {
    render: render,
    copy: copy$2
  };

  var repeat = function (repititions, f) {
    var r = [];
    for (var i = 0; i < repititions; i++) {
      r.push(f(i));
    }
    return r;
  };
  var range$1 = function (start, end) {
    var r = [];
    for (var i = start; i < end; i++) {
      r.push(i);
    }
    return r;
  };
  var unique = function (xs, comparator) {
    var result = [];
    $_4jja6kk5jfuvixx1.each(xs, function (x, i) {
      if (i < xs.length - 1 && !comparator(x, xs[i + 1])) {
        result.push(x);
      } else if (i === xs.length - 1) {
        result.push(x);
      }
    });
    return result;
  };
  var deduce = function (xs, index) {
    if (index < 0 || index >= xs.length - 1)
      return Option.none();
    var current = xs[index].fold(function () {
      var rest = $_4jja6kk5jfuvixx1.reverse(xs.slice(0, index));
      return $_6i1r9dmvjfuviygw.findMap(rest, function (a, i) {
        return a.map(function (aa) {
          return {
            value: aa,
            delta: i + 1
          };
        });
      });
    }, function (c) {
      return Option.some({
        value: c,
        delta: 0
      });
    });
    var next = xs[index + 1].fold(function () {
      var rest = xs.slice(index + 1);
      return $_6i1r9dmvjfuviygw.findMap(rest, function (a, i) {
        return a.map(function (aa) {
          return {
            value: aa,
            delta: i + 1
          };
        });
      });
    }, function (n) {
      return Option.some({
        value: n,
        delta: 1
      });
    });
    return current.bind(function (c) {
      return next.map(function (n) {
        var extras = n.delta + c.delta;
        return Math.abs(n.value - c.value) / extras;
      });
    });
  };
  var $_7hhay7n2jfuviyj3 = {
    repeat: repeat,
    range: range$1,
    unique: unique,
    deduce: deduce
  };

  var columns = function (warehouse) {
    var grid = warehouse.grid();
    var cols = $_7hhay7n2jfuviyj3.range(0, grid.columns());
    var rows = $_7hhay7n2jfuviyj3.range(0, grid.rows());
    return $_4jja6kk5jfuvixx1.map(cols, function (col) {
      var getBlock = function () {
        return $_4jja6kk5jfuvixx1.bind(rows, function (r) {
          return $_bwrthsldjfuviy3q.getAt(warehouse, r, col).filter(function (detail) {
            return detail.column() === col;
          }).fold($_fdch7uk7jfuvixxb.constant([]), function (detail) {
            return [detail];
          });
        });
      };
      var isSingle = function (detail) {
        return detail.colspan() === 1;
      };
      var getFallback = function () {
        return $_bwrthsldjfuviy3q.getAt(warehouse, 0, col);
      };
      return decide(getBlock, isSingle, getFallback);
    });
  };
  var decide = function (getBlock, isSingle, getFallback) {
    var inBlock = getBlock();
    var singleInBlock = $_4jja6kk5jfuvixx1.find(inBlock, isSingle);
    var detailOption = singleInBlock.orThunk(function () {
      return Option.from(inBlock[0]).orThunk(getFallback);
    });
    return detailOption.map(function (detail) {
      return detail.element();
    });
  };
  var rows$1 = function (warehouse) {
    var grid = warehouse.grid();
    var rows = $_7hhay7n2jfuviyj3.range(0, grid.rows());
    var cols = $_7hhay7n2jfuviyj3.range(0, grid.columns());
    return $_4jja6kk5jfuvixx1.map(rows, function (row) {
      var getBlock = function () {
        return $_4jja6kk5jfuvixx1.bind(cols, function (c) {
          return $_bwrthsldjfuviy3q.getAt(warehouse, row, c).filter(function (detail) {
            return detail.row() === row;
          }).fold($_fdch7uk7jfuvixxb.constant([]), function (detail) {
            return [detail];
          });
        });
      };
      var isSingle = function (detail) {
        return detail.rowspan() === 1;
      };
      var getFallback = function () {
        return $_bwrthsldjfuviy3q.getAt(warehouse, row, 0);
      };
      return decide(getBlock, isSingle, getFallback);
    });
  };
  var $_beg7ofn1jfuviyiu = {
    columns: columns,
    rows: rows$1
  };

  var col = function (column, x, y, w, h) {
    var blocker = $_4sdhm4kkjfuviy0e.fromTag('div');
    $_2lr8nrlejfuviy40.setAll(blocker, {
      position: 'absolute',
      left: x - w / 2 + 'px',
      top: y + 'px',
      height: h + 'px',
      width: w + 'px'
    });
    $_2ekobel5jfuviy2m.setAll(blocker, {
      'data-column': column,
      'role': 'presentation'
    });
    return blocker;
  };
  var row$1 = function (row, x, y, w, h) {
    var blocker = $_4sdhm4kkjfuviy0e.fromTag('div');
    $_2lr8nrlejfuviy40.setAll(blocker, {
      position: 'absolute',
      left: x + 'px',
      top: y - h / 2 + 'px',
      height: h + 'px',
      width: w + 'px'
    });
    $_2ekobel5jfuviy2m.setAll(blocker, {
      'data-row': row,
      'role': 'presentation'
    });
    return blocker;
  };
  var $_97ni1cn3jfuviyjc = {
    col: col,
    row: row$1
  };

  var css = function (namespace) {
    var dashNamespace = namespace.replace(/\./g, '-');
    var resolve = function (str) {
      return dashNamespace + '-' + str;
    };
    return { resolve: resolve };
  };
  var $_13iai7n5jfuviyjr = { css: css };

  var styles = $_13iai7n5jfuviyjr.css('ephox-snooker');
  var $_a98vnwn4jfuviyjo = { resolve: styles.resolve };

  function Toggler (turnOff, turnOn, initial) {
    var active = initial || false;
    var on = function () {
      turnOn();
      active = true;
    };
    var off = function () {
      turnOff();
      active = false;
    };
    var toggle = function () {
      var f = active ? off : on;
      f();
    };
    var isOn = function () {
      return active;
    };
    return {
      on: on,
      off: off,
      toggle: toggle,
      isOn: isOn
    };
  }

  var read = function (element, attr) {
    var value = $_2ekobel5jfuviy2m.get(element, attr);
    return value === undefined || value === '' ? [] : value.split(' ');
  };
  var add = function (element, attr, id) {
    var old = read(element, attr);
    var nu = old.concat([id]);
    $_2ekobel5jfuviy2m.set(element, attr, nu.join(' '));
  };
  var remove$3 = function (element, attr, id) {
    var nu = $_4jja6kk5jfuvixx1.filter(read(element, attr), function (v) {
      return v !== id;
    });
    if (nu.length > 0)
      $_2ekobel5jfuviy2m.set(element, attr, nu.join(' '));
    else
      $_2ekobel5jfuviy2m.remove(element, attr);
  };
  var $_3iq9wqn9jfuviyjz = {
    read: read,
    add: add,
    remove: remove$3
  };

  var supports = function (element) {
    return element.dom().classList !== undefined;
  };
  var get$6 = function (element) {
    return $_3iq9wqn9jfuviyjz.read(element, 'class');
  };
  var add$1 = function (element, clazz) {
    return $_3iq9wqn9jfuviyjz.add(element, 'class', clazz);
  };
  var remove$4 = function (element, clazz) {
    return $_3iq9wqn9jfuviyjz.remove(element, 'class', clazz);
  };
  var toggle = function (element, clazz) {
    if ($_4jja6kk5jfuvixx1.contains(get$6(element), clazz)) {
      remove$4(element, clazz);
    } else {
      add$1(element, clazz);
    }
  };
  var $_1jx5cbn8jfuviyjv = {
    get: get$6,
    add: add$1,
    remove: remove$4,
    toggle: toggle,
    supports: supports
  };

  var add$2 = function (element, clazz) {
    if ($_1jx5cbn8jfuviyjv.supports(element))
      element.dom().classList.add(clazz);
    else
      $_1jx5cbn8jfuviyjv.add(element, clazz);
  };
  var cleanClass = function (element) {
    var classList = $_1jx5cbn8jfuviyjv.supports(element) ? element.dom().classList : $_1jx5cbn8jfuviyjv.get(element);
    if (classList.length === 0) {
      $_2ekobel5jfuviy2m.remove(element, 'class');
    }
  };
  var remove$5 = function (element, clazz) {
    if ($_1jx5cbn8jfuviyjv.supports(element)) {
      var classList = element.dom().classList;
      classList.remove(clazz);
    } else
      $_1jx5cbn8jfuviyjv.remove(element, clazz);
    cleanClass(element);
  };
  var toggle$1 = function (element, clazz) {
    return $_1jx5cbn8jfuviyjv.supports(element) ? element.dom().classList.toggle(clazz) : $_1jx5cbn8jfuviyjv.toggle(element, clazz);
  };
  var toggler = function (element, clazz) {
    var hasClasslist = $_1jx5cbn8jfuviyjv.supports(element);
    var classList = element.dom().classList;
    var off = function () {
      if (hasClasslist)
        classList.remove(clazz);
      else
        $_1jx5cbn8jfuviyjv.remove(element, clazz);
    };
    var on = function () {
      if (hasClasslist)
        classList.add(clazz);
      else
        $_1jx5cbn8jfuviyjv.add(element, clazz);
    };
    return Toggler(off, on, has$1(element, clazz));
  };
  var has$1 = function (element, clazz) {
    return $_1jx5cbn8jfuviyjv.supports(element) && element.dom().classList.contains(clazz);
  };
  var $_c1zp6in6jfuviyjs = {
    add: add$2,
    remove: remove$5,
    toggle: toggle$1,
    toggler: toggler,
    has: has$1
  };

  var resizeBar = $_a98vnwn4jfuviyjo.resolve('resizer-bar');
  var resizeRowBar = $_a98vnwn4jfuviyjo.resolve('resizer-rows');
  var resizeColBar = $_a98vnwn4jfuviyjo.resolve('resizer-cols');
  var BAR_THICKNESS = 7;
  var clear = function (wire) {
    var previous = $_a3hs1bl7jfuviy2w.descendants(wire.parent(), '.' + resizeBar);
    $_4jja6kk5jfuvixx1.each(previous, $_5ud3colhjfuviy4l.remove);
  };
  var drawBar = function (wire, positions, create) {
    var origin = wire.origin();
    $_4jja6kk5jfuvixx1.each(positions, function (cpOption, i) {
      cpOption.each(function (cp) {
        var bar = create(origin, cp);
        $_c1zp6in6jfuviyjs.add(bar, resizeBar);
        $_5zcsfmlgjfuviy4g.append(wire.parent(), bar);
      });
    });
  };
  var refreshCol = function (wire, colPositions, position, tableHeight) {
    drawBar(wire, colPositions, function (origin, cp) {
      var colBar = $_97ni1cn3jfuviyjc.col(cp.col(), cp.x() - origin.left(), position.top() - origin.top(), BAR_THICKNESS, tableHeight);
      $_c1zp6in6jfuviyjs.add(colBar, resizeColBar);
      return colBar;
    });
  };
  var refreshRow = function (wire, rowPositions, position, tableWidth) {
    drawBar(wire, rowPositions, function (origin, cp) {
      var rowBar = $_97ni1cn3jfuviyjc.row(cp.row(), position.left() - origin.left(), cp.y() - origin.top(), tableWidth, BAR_THICKNESS);
      $_c1zp6in6jfuviyjs.add(rowBar, resizeRowBar);
      return rowBar;
    });
  };
  var refreshGrid = function (wire, table, rows, cols, hdirection, vdirection) {
    var position = $_6ti42xmijfuviydp.absolute(table);
    var rowPositions = rows.length > 0 ? hdirection.positions(rows, table) : [];
    refreshRow(wire, rowPositions, position, $_4u9lbfmejfuviyd2.getOuter(table));
    var colPositions = cols.length > 0 ? vdirection.positions(cols, table) : [];
    refreshCol(wire, colPositions, position, $_e3phjumcjfuviyct.getOuter(table));
  };
  var refresh = function (wire, table, hdirection, vdirection) {
    clear(wire);
    var list = $_2vc4gykfjfuvixyi.fromTable(table);
    var warehouse = $_bwrthsldjfuviy3q.generate(list);
    var rows = $_beg7ofn1jfuviyiu.rows(warehouse);
    var cols = $_beg7ofn1jfuviyiu.columns(warehouse);
    refreshGrid(wire, table, rows, cols, hdirection, vdirection);
  };
  var each$2 = function (wire, f) {
    var bars = $_a3hs1bl7jfuviy2w.descendants(wire.parent(), '.' + resizeBar);
    $_4jja6kk5jfuvixx1.each(bars, f);
  };
  var hide = function (wire) {
    each$2(wire, function (bar) {
      $_2lr8nrlejfuviy40.set(bar, 'display', 'none');
    });
  };
  var show = function (wire) {
    each$2(wire, function (bar) {
      $_2lr8nrlejfuviy40.set(bar, 'display', 'block');
    });
  };
  var isRowBar = function (element) {
    return $_c1zp6in6jfuviyjs.has(element, resizeRowBar);
  };
  var isColBar = function (element) {
    return $_c1zp6in6jfuviyjs.has(element, resizeColBar);
  };
  var $_8qdnrkn0jfuviyi7 = {
    refresh: refresh,
    hide: hide,
    show: show,
    destroy: clear,
    isRowBar: isRowBar,
    isColBar: isColBar
  };

  var fromWarehouse = function (warehouse, generators) {
    return $_b883t1mwjfuviygz.toGrid(warehouse, generators, false);
  };
  var deriveRows = function (rendered, generators) {
    var findRow = function (details) {
      var rowOfCells = $_6i1r9dmvjfuviygw.findMap(details, function (detail) {
        return $_87w3h3kmjfuviy0m.parent(detail.element()).map(function (row) {
          var isNew = $_87w3h3kmjfuviy0m.parent(row).isNone();
          return $_g02m1vkgjfuvixyt.elementnew(row, isNew);
        });
      });
      return rowOfCells.getOrThunk(function () {
        return $_g02m1vkgjfuvixyt.elementnew(generators.row(), true);
      });
    };
    return $_4jja6kk5jfuvixx1.map(rendered, function (details) {
      var row = findRow(details.details());
      return $_g02m1vkgjfuvixyt.rowdatanew(row.element(), details.details(), details.section(), row.isNew());
    });
  };
  var toDetailList = function (grid, generators) {
    var rendered = $_b883t1mwjfuviygz.toDetails(grid, $_g6ztqikojfuviy13.eq);
    return deriveRows(rendered, generators);
  };
  var findInWarehouse = function (warehouse, element) {
    var all = $_4jja6kk5jfuvixx1.flatten($_4jja6kk5jfuvixx1.map(warehouse.all(), function (r) {
      return r.cells();
    }));
    return $_4jja6kk5jfuvixx1.find(all, function (e) {
      return $_g6ztqikojfuviy13.eq(element, e.element());
    });
  };
  var run = function (operation, extract, adjustment, postAction, genWrappers) {
    return function (wire, table, target, generators, direction) {
      var input = $_2vc4gykfjfuvixyi.fromTable(table);
      var warehouse = $_bwrthsldjfuviy3q.generate(input);
      var output = extract(warehouse, target).map(function (info) {
        var model = fromWarehouse(warehouse, generators);
        var result = operation(model, info, $_g6ztqikojfuviy13.eq, genWrappers(generators));
        var grid = toDetailList(result.grid(), generators);
        return {
          grid: $_fdch7uk7jfuvixxb.constant(grid),
          cursor: result.cursor
        };
      });
      return output.fold(function () {
        return Option.none();
      }, function (out) {
        var newElements = $_clwdhumzjfuviyhf.render(table, out.grid());
        adjustment(table, out.grid(), direction);
        postAction(table);
        $_8qdnrkn0jfuviyi7.refresh(wire, table, $_bj1b3kmhjfuviyd6.height, direction);
        return Option.some({
          cursor: out.cursor,
          newRows: newElements.newRows,
          newCells: newElements.newCells
        });
      });
    };
  };
  var onCell = function (warehouse, target) {
    return $_dmqxswkhjfuvixyz.cell(target.element()).bind(function (cell) {
      return findInWarehouse(warehouse, cell);
    });
  };
  var onPaste = function (warehouse, target) {
    return $_dmqxswkhjfuvixyz.cell(target.element()).bind(function (cell) {
      return findInWarehouse(warehouse, cell).map(function (details) {
        return $_8d366dmujfuviygv.merge(details, {
          generators: target.generators,
          clipboard: target.clipboard
        });
      });
    });
  };
  var onPasteRows = function (warehouse, target) {
    var details = $_4jja6kk5jfuvixx1.map(target.selection(), function (cell) {
      return $_dmqxswkhjfuvixyz.cell(cell).bind(function (lc) {
        return findInWarehouse(warehouse, lc);
      });
    });
    var cells = $_6i1r9dmvjfuviygw.cat(details);
    return cells.length > 0 ? Option.some($_8d366dmujfuviygv.merge({ cells: cells }, {
      generators: target.generators,
      clipboard: target.clipboard
    })) : Option.none();
  };
  var onMergable = function (warehouse, target) {
    return target.mergable();
  };
  var onUnmergable = function (warehouse, target) {
    return target.unmergable();
  };
  var onCells = function (warehouse, target) {
    var details = $_4jja6kk5jfuvixx1.map(target.selection(), function (cell) {
      return $_dmqxswkhjfuvixyz.cell(cell).bind(function (lc) {
        return findInWarehouse(warehouse, lc);
      });
    });
    var cells = $_6i1r9dmvjfuviygw.cat(details);
    return cells.length > 0 ? Option.some(cells) : Option.none();
  };
  var $_b9rtfhmtjfuviygc = {
    run: run,
    toDetailList: toDetailList,
    onCell: onCell,
    onCells: onCells,
    onPaste: onPaste,
    onPasteRows: onPasteRows,
    onMergable: onMergable,
    onUnmergable: onUnmergable
  };

  var value$1 = function (o) {
    var is = function (v) {
      return o === v;
    };
    var or = function (opt) {
      return value$1(o);
    };
    var orThunk = function (f) {
      return value$1(o);
    };
    var map = function (f) {
      return value$1(f(o));
    };
    var each = function (f) {
      f(o);
    };
    var bind = function (f) {
      return f(o);
    };
    var fold = function (_, onValue) {
      return onValue(o);
    };
    var exists = function (f) {
      return f(o);
    };
    var forall = function (f) {
      return f(o);
    };
    var toOption = function () {
      return Option.some(o);
    };
    return {
      is: is,
      isValue: $_fdch7uk7jfuvixxb.always,
      isError: $_fdch7uk7jfuvixxb.never,
      getOr: $_fdch7uk7jfuvixxb.constant(o),
      getOrThunk: $_fdch7uk7jfuvixxb.constant(o),
      getOrDie: $_fdch7uk7jfuvixxb.constant(o),
      or: or,
      orThunk: orThunk,
      fold: fold,
      map: map,
      each: each,
      bind: bind,
      exists: exists,
      forall: forall,
      toOption: toOption
    };
  };
  var error = function (message) {
    var getOrThunk = function (f) {
      return f();
    };
    var getOrDie = function () {
      return $_fdch7uk7jfuvixxb.die(String(message))();
    };
    var or = function (opt) {
      return opt;
    };
    var orThunk = function (f) {
      return f();
    };
    var map = function (f) {
      return error(message);
    };
    var bind = function (f) {
      return error(message);
    };
    var fold = function (onError, _) {
      return onError(message);
    };
    return {
      is: $_fdch7uk7jfuvixxb.never,
      isValue: $_fdch7uk7jfuvixxb.never,
      isError: $_fdch7uk7jfuvixxb.always,
      getOr: $_fdch7uk7jfuvixxb.identity,
      getOrThunk: getOrThunk,
      getOrDie: getOrDie,
      or: or,
      orThunk: orThunk,
      fold: fold,
      map: map,
      each: $_fdch7uk7jfuvixxb.noop,
      bind: bind,
      exists: $_fdch7uk7jfuvixxb.never,
      forall: $_fdch7uk7jfuvixxb.always,
      toOption: Option.none
    };
  };
  var Result = {
    value: value$1,
    error: error
  };

  var measure = function (startAddress, gridA, gridB) {
    if (startAddress.row() >= gridA.length || startAddress.column() > $_f8bgbgmyjfuviyha.cellLength(gridA[0]))
      return Result.error('invalid start address out of table bounds, row: ' + startAddress.row() + ', column: ' + startAddress.column());
    var rowRemainder = gridA.slice(startAddress.row());
    var colRemainder = rowRemainder[0].cells().slice(startAddress.column());
    var colRequired = $_f8bgbgmyjfuviyha.cellLength(gridB[0]);
    var rowRequired = gridB.length;
    return Result.value({
      rowDelta: $_fdch7uk7jfuvixxb.constant(rowRemainder.length - rowRequired),
      colDelta: $_fdch7uk7jfuvixxb.constant(colRemainder.length - colRequired)
    });
  };
  var measureWidth = function (gridA, gridB) {
    var colLengthA = $_f8bgbgmyjfuviyha.cellLength(gridA[0]);
    var colLengthB = $_f8bgbgmyjfuviyha.cellLength(gridB[0]);
    return {
      rowDelta: $_fdch7uk7jfuvixxb.constant(0),
      colDelta: $_fdch7uk7jfuvixxb.constant(colLengthA - colLengthB)
    };
  };
  var fill = function (cells, generator) {
    return $_4jja6kk5jfuvixx1.map(cells, function () {
      return $_g02m1vkgjfuvixyt.elementnew(generator.cell(), true);
    });
  };
  var rowFill = function (grid, amount, generator) {
    return grid.concat($_7hhay7n2jfuviyj3.repeat(amount, function (_row) {
      return $_f8bgbgmyjfuviyha.setCells(grid[grid.length - 1], fill(grid[grid.length - 1].cells(), generator));
    }));
  };
  var colFill = function (grid, amount, generator) {
    return $_4jja6kk5jfuvixx1.map(grid, function (row) {
      return $_f8bgbgmyjfuviyha.setCells(row, row.cells().concat(fill($_7hhay7n2jfuviyj3.range(0, amount), generator)));
    });
  };
  var tailor = function (gridA, delta, generator) {
    var fillCols = delta.colDelta() < 0 ? colFill : $_fdch7uk7jfuvixxb.identity;
    var fillRows = delta.rowDelta() < 0 ? rowFill : $_fdch7uk7jfuvixxb.identity;
    var modifiedCols = fillCols(gridA, Math.abs(delta.colDelta()), generator);
    var tailoredGrid = fillRows(modifiedCols, Math.abs(delta.rowDelta()), generator);
    return tailoredGrid;
  };
  var $_b36tqhnbjfuviyk9 = {
    measure: measure,
    measureWidth: measureWidth,
    tailor: tailor
  };

  var merge$2 = function (grid, bounds, comparator, substitution) {
    if (grid.length === 0)
      return grid;
    for (var i = bounds.startRow(); i <= bounds.finishRow(); i++) {
      for (var j = bounds.startCol(); j <= bounds.finishCol(); j++) {
        $_f8bgbgmyjfuviyha.mutateCell(grid[i], j, $_g02m1vkgjfuvixyt.elementnew(substitution(), false));
      }
    }
    return grid;
  };
  var unmerge = function (grid, target, comparator, substitution) {
    var first = true;
    for (var i = 0; i < grid.length; i++) {
      for (var j = 0; j < $_f8bgbgmyjfuviyha.cellLength(grid[0]); j++) {
        var current = $_f8bgbgmyjfuviyha.getCellElement(grid[i], j);
        var isToReplace = comparator(current, target);
        if (isToReplace === true && first === false) {
          $_f8bgbgmyjfuviyha.mutateCell(grid[i], j, $_g02m1vkgjfuvixyt.elementnew(substitution(), true));
        } else if (isToReplace === true) {
          first = false;
        }
      }
    }
    return grid;
  };
  var uniqueCells = function (row, comparator) {
    return $_4jja6kk5jfuvixx1.foldl(row, function (rest, cell) {
      return $_4jja6kk5jfuvixx1.exists(rest, function (currentCell) {
        return comparator(currentCell.element(), cell.element());
      }) ? rest : rest.concat([cell]);
    }, []);
  };
  var splitRows = function (grid, index, comparator, substitution) {
    if (index > 0 && index < grid.length) {
      var rowPrevCells = grid[index - 1].cells();
      var cells = uniqueCells(rowPrevCells, comparator);
      $_4jja6kk5jfuvixx1.each(cells, function (cell) {
        var replacement = Option.none();
        for (var i = index; i < grid.length; i++) {
          for (var j = 0; j < $_f8bgbgmyjfuviyha.cellLength(grid[0]); j++) {
            var current = grid[i].cells()[j];
            var isToReplace = comparator(current.element(), cell.element());
            if (isToReplace) {
              if (replacement.isNone()) {
                replacement = Option.some(substitution());
              }
              replacement.each(function (sub) {
                $_f8bgbgmyjfuviyha.mutateCell(grid[i], j, $_g02m1vkgjfuvixyt.elementnew(sub, true));
              });
            }
          }
        }
      });
    }
    return grid;
  };
  var $_aovyapndjfuviykl = {
    merge: merge$2,
    unmerge: unmerge,
    splitRows: splitRows
  };

  var isSpanning = function (grid, row, col, comparator) {
    var candidate = $_f8bgbgmyjfuviyha.getCell(grid[row], col);
    var matching = $_fdch7uk7jfuvixxb.curry(comparator, candidate.element());
    var currentRow = grid[row];
    return grid.length > 1 && $_f8bgbgmyjfuviyha.cellLength(currentRow) > 1 && (col > 0 && matching($_f8bgbgmyjfuviyha.getCellElement(currentRow, col - 1)) || col < currentRow.length - 1 && matching($_f8bgbgmyjfuviyha.getCellElement(currentRow, col + 1)) || row > 0 && matching($_f8bgbgmyjfuviyha.getCellElement(grid[row - 1], col)) || row < grid.length - 1 && matching($_f8bgbgmyjfuviyha.getCellElement(grid[row + 1], col)));
  };
  var mergeTables = function (startAddress, gridA, gridB, generator, comparator) {
    var startRow = startAddress.row();
    var startCol = startAddress.column();
    var mergeHeight = gridB.length;
    var mergeWidth = $_f8bgbgmyjfuviyha.cellLength(gridB[0]);
    var endRow = startRow + mergeHeight;
    var endCol = startCol + mergeWidth;
    for (var r = startRow; r < endRow; r++) {
      for (var c = startCol; c < endCol; c++) {
        if (isSpanning(gridA, r, c, comparator)) {
          $_aovyapndjfuviykl.unmerge(gridA, $_f8bgbgmyjfuviyha.getCellElement(gridA[r], c), comparator, generator.cell);
        }
        var newCell = $_f8bgbgmyjfuviyha.getCellElement(gridB[r - startRow], c - startCol);
        var replacement = generator.replace(newCell);
        $_f8bgbgmyjfuviyha.mutateCell(gridA[r], c, $_g02m1vkgjfuvixyt.elementnew(replacement, true));
      }
    }
    return gridA;
  };
  var merge$3 = function (startAddress, gridA, gridB, generator, comparator) {
    var result = $_b36tqhnbjfuviyk9.measure(startAddress, gridA, gridB);
    return result.map(function (delta) {
      var fittedGrid = $_b36tqhnbjfuviyk9.tailor(gridA, delta, generator);
      return mergeTables(startAddress, fittedGrid, gridB, generator, comparator);
    });
  };
  var insert = function (index, gridA, gridB, generator, comparator) {
    $_aovyapndjfuviykl.splitRows(gridA, index, comparator, generator.cell);
    var delta = $_b36tqhnbjfuviyk9.measureWidth(gridB, gridA);
    var fittedNewGrid = $_b36tqhnbjfuviyk9.tailor(gridB, delta, generator);
    var secondDelta = $_b36tqhnbjfuviyk9.measureWidth(gridA, fittedNewGrid);
    var fittedOldGrid = $_b36tqhnbjfuviyk9.tailor(gridA, secondDelta, generator);
    return fittedOldGrid.slice(0, index).concat(fittedNewGrid).concat(fittedOldGrid.slice(index, fittedOldGrid.length));
  };
  var $_e6g8nvnajfuviyk4 = {
    merge: merge$3,
    insert: insert
  };

  var insertRowAt = function (grid, index, example, comparator, substitution) {
    var before = grid.slice(0, index);
    var after = grid.slice(index);
    var between = $_f8bgbgmyjfuviyha.mapCells(grid[example], function (ex, c) {
      var withinSpan = index > 0 && index < grid.length && comparator($_f8bgbgmyjfuviyha.getCellElement(grid[index - 1], c), $_f8bgbgmyjfuviyha.getCellElement(grid[index], c));
      var ret = withinSpan ? $_f8bgbgmyjfuviyha.getCell(grid[index], c) : $_g02m1vkgjfuvixyt.elementnew(substitution(ex.element(), comparator), true);
      return ret;
    });
    return before.concat([between]).concat(after);
  };
  var insertColumnAt = function (grid, index, example, comparator, substitution) {
    return $_4jja6kk5jfuvixx1.map(grid, function (row) {
      var withinSpan = index > 0 && index < $_f8bgbgmyjfuviyha.cellLength(row) && comparator($_f8bgbgmyjfuviyha.getCellElement(row, index - 1), $_f8bgbgmyjfuviyha.getCellElement(row, index));
      var sub = withinSpan ? $_f8bgbgmyjfuviyha.getCell(row, index) : $_g02m1vkgjfuvixyt.elementnew(substitution($_f8bgbgmyjfuviyha.getCellElement(row, example), comparator), true);
      return $_f8bgbgmyjfuviyha.addCell(row, index, sub);
    });
  };
  var splitCellIntoColumns = function (grid, exampleRow, exampleCol, comparator, substitution) {
    var index = exampleCol + 1;
    return $_4jja6kk5jfuvixx1.map(grid, function (row, i) {
      var isTargetCell = i === exampleRow;
      var sub = isTargetCell ? $_g02m1vkgjfuvixyt.elementnew(substitution($_f8bgbgmyjfuviyha.getCellElement(row, exampleCol), comparator), true) : $_f8bgbgmyjfuviyha.getCell(row, exampleCol);
      return $_f8bgbgmyjfuviyha.addCell(row, index, sub);
    });
  };
  var splitCellIntoRows = function (grid, exampleRow, exampleCol, comparator, substitution) {
    var index = exampleRow + 1;
    var before = grid.slice(0, index);
    var after = grid.slice(index);
    var between = $_f8bgbgmyjfuviyha.mapCells(grid[exampleRow], function (ex, i) {
      var isTargetCell = i === exampleCol;
      return isTargetCell ? $_g02m1vkgjfuvixyt.elementnew(substitution(ex.element(), comparator), true) : ex;
    });
    return before.concat([between]).concat(after);
  };
  var deleteColumnsAt = function (grid, start, finish) {
    var rows = $_4jja6kk5jfuvixx1.map(grid, function (row) {
      var cells = row.cells().slice(0, start).concat(row.cells().slice(finish + 1));
      return $_g02m1vkgjfuvixyt.rowcells(cells, row.section());
    });
    return $_4jja6kk5jfuvixx1.filter(rows, function (row) {
      return row.cells().length > 0;
    });
  };
  var deleteRowsAt = function (grid, start, finish) {
    return grid.slice(0, start).concat(grid.slice(finish + 1));
  };
  var $_3fg4ocnejfuviykt = {
    insertRowAt: insertRowAt,
    insertColumnAt: insertColumnAt,
    splitCellIntoColumns: splitCellIntoColumns,
    splitCellIntoRows: splitCellIntoRows,
    deleteRowsAt: deleteRowsAt,
    deleteColumnsAt: deleteColumnsAt
  };

  var replaceIn = function (grid, targets, comparator, substitution) {
    var isTarget = function (cell) {
      return $_4jja6kk5jfuvixx1.exists(targets, function (target) {
        return comparator(cell.element(), target.element());
      });
    };
    return $_4jja6kk5jfuvixx1.map(grid, function (row) {
      return $_f8bgbgmyjfuviyha.mapCells(row, function (cell) {
        return isTarget(cell) ? $_g02m1vkgjfuvixyt.elementnew(substitution(cell.element(), comparator), true) : cell;
      });
    });
  };
  var notStartRow = function (grid, rowIndex, colIndex, comparator) {
    return $_f8bgbgmyjfuviyha.getCellElement(grid[rowIndex], colIndex) !== undefined && (rowIndex > 0 && comparator($_f8bgbgmyjfuviyha.getCellElement(grid[rowIndex - 1], colIndex), $_f8bgbgmyjfuviyha.getCellElement(grid[rowIndex], colIndex)));
  };
  var notStartColumn = function (row, index, comparator) {
    return index > 0 && comparator($_f8bgbgmyjfuviyha.getCellElement(row, index - 1), $_f8bgbgmyjfuviyha.getCellElement(row, index));
  };
  var replaceColumn = function (grid, index, comparator, substitution) {
    var targets = $_4jja6kk5jfuvixx1.bind(grid, function (row, i) {
      var alreadyAdded = notStartRow(grid, i, index, comparator) || notStartColumn(row, index, comparator);
      return alreadyAdded ? [] : [$_f8bgbgmyjfuviyha.getCell(row, index)];
    });
    return replaceIn(grid, targets, comparator, substitution);
  };
  var replaceRow = function (grid, index, comparator, substitution) {
    var targetRow = grid[index];
    var targets = $_4jja6kk5jfuvixx1.bind(targetRow.cells(), function (item, i) {
      var alreadyAdded = notStartRow(grid, index, i, comparator) || notStartColumn(targetRow, i, comparator);
      return alreadyAdded ? [] : [item];
    });
    return replaceIn(grid, targets, comparator, substitution);
  };
  var $_qcrcfnfjfuviykz = {
    replaceColumn: replaceColumn,
    replaceRow: replaceRow
  };

  var none$1 = function () {
    return folder(function (n, o, l, m, r) {
      return n();
    });
  };
  var only = function (index) {
    return folder(function (n, o, l, m, r) {
      return o(index);
    });
  };
  var left = function (index, next) {
    return folder(function (n, o, l, m, r) {
      return l(index, next);
    });
  };
  var middle = function (prev, index, next) {
    return folder(function (n, o, l, m, r) {
      return m(prev, index, next);
    });
  };
  var right = function (prev, index) {
    return folder(function (n, o, l, m, r) {
      return r(prev, index);
    });
  };
  var folder = function (fold) {
    return { fold: fold };
  };
  var $_16dvf6nijfuviylf = {
    none: none$1,
    only: only,
    left: left,
    middle: middle,
    right: right
  };

  var neighbours$1 = function (input, index) {
    if (input.length === 0)
      return $_16dvf6nijfuviylf.none();
    if (input.length === 1)
      return $_16dvf6nijfuviylf.only(0);
    if (index === 0)
      return $_16dvf6nijfuviylf.left(0, 1);
    if (index === input.length - 1)
      return $_16dvf6nijfuviylf.right(index - 1, index);
    if (index > 0 && index < input.length - 1)
      return $_16dvf6nijfuviylf.middle(index - 1, index, index + 1);
    return $_16dvf6nijfuviylf.none();
  };
  var determine = function (input, column, step, tableSize) {
    var result = input.slice(0);
    var context = neighbours$1(input, column);
    var zero = function (array) {
      return $_4jja6kk5jfuvixx1.map(array, $_fdch7uk7jfuvixxb.constant(0));
    };
    var onNone = $_fdch7uk7jfuvixxb.constant(zero(result));
    var onOnly = function (index) {
      return tableSize.singleColumnWidth(result[index], step);
    };
    var onChange = function (index, next) {
      if (step >= 0) {
        var newNext = Math.max(tableSize.minCellWidth(), result[next] - step);
        return zero(result.slice(0, index)).concat([
          step,
          newNext - result[next]
        ]).concat(zero(result.slice(next + 1)));
      } else {
        var newThis = Math.max(tableSize.minCellWidth(), result[index] + step);
        var diffx = result[index] - newThis;
        return zero(result.slice(0, index)).concat([
          newThis - result[index],
          diffx
        ]).concat(zero(result.slice(next + 1)));
      }
    };
    var onLeft = onChange;
    var onMiddle = function (prev, index, next) {
      return onChange(index, next);
    };
    var onRight = function (prev, index) {
      if (step >= 0) {
        return zero(result.slice(0, index)).concat([step]);
      } else {
        var size = Math.max(tableSize.minCellWidth(), result[index] + step);
        return zero(result.slice(0, index)).concat([size - result[index]]);
      }
    };
    return context.fold(onNone, onOnly, onLeft, onMiddle, onRight);
  };
  var $_1cyj6ynhjfuviyl9 = { determine: determine };

  var getSpan$1 = function (cell, type) {
    return $_2ekobel5jfuviy2m.has(cell, type) && parseInt($_2ekobel5jfuviy2m.get(cell, type), 10) > 1;
  };
  var hasColspan = function (cell) {
    return getSpan$1(cell, 'colspan');
  };
  var hasRowspan = function (cell) {
    return getSpan$1(cell, 'rowspan');
  };
  var getInt = function (element, property) {
    return parseInt($_2lr8nrlejfuviy40.get(element, property), 10);
  };
  var $_edgkrmnkjfuviylu = {
    hasColspan: hasColspan,
    hasRowspan: hasRowspan,
    minWidth: $_fdch7uk7jfuvixxb.constant(10),
    minHeight: $_fdch7uk7jfuvixxb.constant(10),
    getInt: getInt
  };

  var getRaw$1 = function (cell, property, getter) {
    return $_2lr8nrlejfuviy40.getRaw(cell, property).fold(function () {
      return getter(cell) + 'px';
    }, function (raw) {
      return raw;
    });
  };
  var getRawW = function (cell) {
    return getRaw$1(cell, 'width', $_9h89znmajfuviybw.getPixelWidth);
  };
  var getRawH = function (cell) {
    return getRaw$1(cell, 'height', $_9h89znmajfuviybw.getHeight);
  };
  var getWidthFrom = function (warehouse, direction, getWidth, fallback, tableSize) {
    var columns = $_beg7ofn1jfuviyiu.columns(warehouse);
    var backups = $_4jja6kk5jfuvixx1.map(columns, function (cellOption) {
      return cellOption.map(direction.edge);
    });
    return $_4jja6kk5jfuvixx1.map(columns, function (cellOption, c) {
      var columnCell = cellOption.filter($_fdch7uk7jfuvixxb.not($_edgkrmnkjfuviylu.hasColspan));
      return columnCell.fold(function () {
        var deduced = $_7hhay7n2jfuviyj3.deduce(backups, c);
        return fallback(deduced);
      }, function (cell) {
        return getWidth(cell, tableSize);
      });
    });
  };
  var getDeduced = function (deduced) {
    return deduced.map(function (d) {
      return d + 'px';
    }).getOr('');
  };
  var getRawWidths = function (warehouse, direction) {
    return getWidthFrom(warehouse, direction, getRawW, getDeduced);
  };
  var getPercentageWidths = function (warehouse, direction, tableSize) {
    return getWidthFrom(warehouse, direction, $_9h89znmajfuviybw.getPercentageWidth, function (deduced) {
      return deduced.fold(function () {
        return tableSize.minCellWidth();
      }, function (cellWidth) {
        return cellWidth / tableSize.pixelWidth() * 100;
      });
    }, tableSize);
  };
  var getPixelWidths = function (warehouse, direction, tableSize) {
    return getWidthFrom(warehouse, direction, $_9h89znmajfuviybw.getPixelWidth, function (deduced) {
      return deduced.getOrThunk(tableSize.minCellWidth);
    }, tableSize);
  };
  var getHeightFrom = function (warehouse, direction, getHeight, fallback) {
    var rows = $_beg7ofn1jfuviyiu.rows(warehouse);
    var backups = $_4jja6kk5jfuvixx1.map(rows, function (cellOption) {
      return cellOption.map(direction.edge);
    });
    return $_4jja6kk5jfuvixx1.map(rows, function (cellOption, c) {
      var rowCell = cellOption.filter($_fdch7uk7jfuvixxb.not($_edgkrmnkjfuviylu.hasRowspan));
      return rowCell.fold(function () {
        var deduced = $_7hhay7n2jfuviyj3.deduce(backups, c);
        return fallback(deduced);
      }, function (cell) {
        return getHeight(cell);
      });
    });
  };
  var getPixelHeights = function (warehouse, direction) {
    return getHeightFrom(warehouse, direction, $_9h89znmajfuviybw.getHeight, function (deduced) {
      return deduced.getOrThunk($_edgkrmnkjfuviylu.minHeight);
    });
  };
  var getRawHeights = function (warehouse, direction) {
    return getHeightFrom(warehouse, direction, getRawH, getDeduced);
  };
  var $_4g5tkonjjfuviylh = {
    getRawWidths: getRawWidths,
    getPixelWidths: getPixelWidths,
    getPercentageWidths: getPercentageWidths,
    getPixelHeights: getPixelHeights,
    getRawHeights: getRawHeights
  };

  var total = function (start, end, measures) {
    var r = 0;
    for (var i = start; i < end; i++) {
      r += measures[i] !== undefined ? measures[i] : 0;
    }
    return r;
  };
  var recalculateWidth = function (warehouse, widths) {
    var all = $_bwrthsldjfuviy3q.justCells(warehouse);
    return $_4jja6kk5jfuvixx1.map(all, function (cell) {
      var width = total(cell.column(), cell.column() + cell.colspan(), widths);
      return {
        element: cell.element,
        width: $_fdch7uk7jfuvixxb.constant(width),
        colspan: cell.colspan
      };
    });
  };
  var recalculateHeight = function (warehouse, heights) {
    var all = $_bwrthsldjfuviy3q.justCells(warehouse);
    return $_4jja6kk5jfuvixx1.map(all, function (cell) {
      var height = total(cell.row(), cell.row() + cell.rowspan(), heights);
      return {
        element: cell.element,
        height: $_fdch7uk7jfuvixxb.constant(height),
        rowspan: cell.rowspan
      };
    });
  };
  var matchRowHeight = function (warehouse, heights) {
    return $_4jja6kk5jfuvixx1.map(warehouse.all(), function (row, i) {
      return {
        element: row.element,
        height: $_fdch7uk7jfuvixxb.constant(heights[i])
      };
    });
  };
  var $_chwc6znljfuviym2 = {
    recalculateWidth: recalculateWidth,
    recalculateHeight: recalculateHeight,
    matchRowHeight: matchRowHeight
  };

  var percentageSize = function (width, element) {
    var floatWidth = parseFloat(width);
    var pixelWidth = $_4u9lbfmejfuviyd2.get(element);
    var getCellDelta = function (delta) {
      return delta / pixelWidth * 100;
    };
    var singleColumnWidth = function (width, _delta) {
      return [100 - width];
    };
    var minCellWidth = function () {
      return $_edgkrmnkjfuviylu.minWidth() / pixelWidth * 100;
    };
    var setTableWidth = function (table, _newWidths, delta) {
      var total = floatWidth + delta;
      $_9h89znmajfuviybw.setPercentageWidth(table, total);
    };
    return {
      width: $_fdch7uk7jfuvixxb.constant(floatWidth),
      pixelWidth: $_fdch7uk7jfuvixxb.constant(pixelWidth),
      getWidths: $_4g5tkonjjfuviylh.getPercentageWidths,
      getCellDelta: getCellDelta,
      singleColumnWidth: singleColumnWidth,
      minCellWidth: minCellWidth,
      setElementWidth: $_9h89znmajfuviybw.setPercentageWidth,
      setTableWidth: setTableWidth
    };
  };
  var pixelSize = function (width) {
    var intWidth = parseInt(width, 10);
    var getCellDelta = $_fdch7uk7jfuvixxb.identity;
    var singleColumnWidth = function (width, delta) {
      var newNext = Math.max($_edgkrmnkjfuviylu.minWidth(), width + delta);
      return [newNext - width];
    };
    var setTableWidth = function (table, newWidths, _delta) {
      var total = $_4jja6kk5jfuvixx1.foldr(newWidths, function (b, a) {
        return b + a;
      }, 0);
      $_9h89znmajfuviybw.setPixelWidth(table, total);
    };
    return {
      width: $_fdch7uk7jfuvixxb.constant(intWidth),
      pixelWidth: $_fdch7uk7jfuvixxb.constant(intWidth),
      getWidths: $_4g5tkonjjfuviylh.getPixelWidths,
      getCellDelta: getCellDelta,
      singleColumnWidth: singleColumnWidth,
      minCellWidth: $_edgkrmnkjfuviylu.minWidth,
      setElementWidth: $_9h89znmajfuviybw.setPixelWidth,
      setTableWidth: setTableWidth
    };
  };
  var chooseSize = function (element, width) {
    if ($_9h89znmajfuviybw.percentageBasedSizeRegex().test(width)) {
      var percentMatch = $_9h89znmajfuviybw.percentageBasedSizeRegex().exec(width);
      return percentageSize(percentMatch[1], element);
    } else if ($_9h89znmajfuviybw.pixelBasedSizeRegex().test(width)) {
      var pixelMatch = $_9h89znmajfuviybw.pixelBasedSizeRegex().exec(width);
      return pixelSize(pixelMatch[1]);
    } else {
      var fallbackWidth = $_4u9lbfmejfuviyd2.get(element);
      return pixelSize(fallbackWidth);
    }
  };
  var getTableSize = function (element) {
    var width = $_9h89znmajfuviybw.getRawWidth(element);
    return width.fold(function () {
      var fallbackWidth = $_4u9lbfmejfuviyd2.get(element);
      return pixelSize(fallbackWidth);
    }, function (width) {
      return chooseSize(element, width);
    });
  };
  var $_707ctxnmjfuviym9 = { getTableSize: getTableSize };

  var getWarehouse$1 = function (list) {
    return $_bwrthsldjfuviy3q.generate(list);
  };
  var sumUp = function (newSize) {
    return $_4jja6kk5jfuvixx1.foldr(newSize, function (b, a) {
      return b + a;
    }, 0);
  };
  var getTableWarehouse = function (table) {
    var list = $_2vc4gykfjfuvixyi.fromTable(table);
    return getWarehouse$1(list);
  };
  var adjustWidth = function (table, delta, index, direction) {
    var tableSize = $_707ctxnmjfuviym9.getTableSize(table);
    var step = tableSize.getCellDelta(delta);
    var warehouse = getTableWarehouse(table);
    var widths = tableSize.getWidths(warehouse, direction, tableSize);
    var deltas = $_1cyj6ynhjfuviyl9.determine(widths, index, step, tableSize);
    var newWidths = $_4jja6kk5jfuvixx1.map(deltas, function (dx, i) {
      return dx + widths[i];
    });
    var newSizes = $_chwc6znljfuviym2.recalculateWidth(warehouse, newWidths);
    $_4jja6kk5jfuvixx1.each(newSizes, function (cell) {
      tableSize.setElementWidth(cell.element(), cell.width());
    });
    if (index === warehouse.grid().columns() - 1) {
      tableSize.setTableWidth(table, newWidths, step);
    }
  };
  var adjustHeight = function (table, delta, index, direction) {
    var warehouse = getTableWarehouse(table);
    var heights = $_4g5tkonjjfuviylh.getPixelHeights(warehouse, direction);
    var newHeights = $_4jja6kk5jfuvixx1.map(heights, function (dy, i) {
      return index === i ? Math.max(delta + dy, $_edgkrmnkjfuviylu.minHeight()) : dy;
    });
    var newCellSizes = $_chwc6znljfuviym2.recalculateHeight(warehouse, newHeights);
    var newRowSizes = $_chwc6znljfuviym2.matchRowHeight(warehouse, newHeights);
    $_4jja6kk5jfuvixx1.each(newRowSizes, function (row) {
      $_9h89znmajfuviybw.setHeight(row.element(), row.height());
    });
    $_4jja6kk5jfuvixx1.each(newCellSizes, function (cell) {
      $_9h89znmajfuviybw.setHeight(cell.element(), cell.height());
    });
    var total = sumUp(newHeights);
    $_9h89znmajfuviybw.setHeight(table, total);
  };
  var adjustWidthTo = function (table, list, direction) {
    var tableSize = $_707ctxnmjfuviym9.getTableSize(table);
    var warehouse = getWarehouse$1(list);
    var widths = tableSize.getWidths(warehouse, direction, tableSize);
    var newSizes = $_chwc6znljfuviym2.recalculateWidth(warehouse, widths);
    $_4jja6kk5jfuvixx1.each(newSizes, function (cell) {
      tableSize.setElementWidth(cell.element(), cell.width());
    });
    var total = $_4jja6kk5jfuvixx1.foldr(widths, function (b, a) {
      return a + b;
    }, 0);
    if (newSizes.length > 0) {
      tableSize.setElementWidth(table, total);
    }
  };
  var $_f6vpt3ngjfuviyl3 = {
    adjustWidth: adjustWidth,
    adjustHeight: adjustHeight,
    adjustWidthTo: adjustWidthTo
  };

  var prune = function (table) {
    var cells = $_dmqxswkhjfuvixyz.cells(table);
    if (cells.length === 0)
      $_5ud3colhjfuviy4l.remove(table);
  };
  var outcome = $_96oqrskbjfuvixya.immutable('grid', 'cursor');
  var elementFromGrid = function (grid, row, column) {
    return findIn(grid, row, column).orThunk(function () {
      return findIn(grid, 0, 0);
    });
  };
  var findIn = function (grid, row, column) {
    return Option.from(grid[row]).bind(function (r) {
      return Option.from(r.cells()[column]).bind(function (c) {
        return Option.from(c.element());
      });
    });
  };
  var bundle = function (grid, row, column) {
    return outcome(grid, findIn(grid, row, column));
  };
  var uniqueRows = function (details) {
    return $_4jja6kk5jfuvixx1.foldl(details, function (rest, detail) {
      return $_4jja6kk5jfuvixx1.exists(rest, function (currentDetail) {
        return currentDetail.row() === detail.row();
      }) ? rest : rest.concat([detail]);
    }, []).sort(function (detailA, detailB) {
      return detailA.row() - detailB.row();
    });
  };
  var uniqueColumns = function (details) {
    return $_4jja6kk5jfuvixx1.foldl(details, function (rest, detail) {
      return $_4jja6kk5jfuvixx1.exists(rest, function (currentDetail) {
        return currentDetail.column() === detail.column();
      }) ? rest : rest.concat([detail]);
    }, []).sort(function (detailA, detailB) {
      return detailA.column() - detailB.column();
    });
  };
  var insertRowBefore = function (grid, detail, comparator, genWrappers) {
    var example = detail.row();
    var targetIndex = detail.row();
    var newGrid = $_3fg4ocnejfuviykt.insertRowAt(grid, targetIndex, example, comparator, genWrappers.getOrInit);
    return bundle(newGrid, targetIndex, detail.column());
  };
  var insertRowsBefore = function (grid, details, comparator, genWrappers) {
    var example = details[0].row();
    var targetIndex = details[0].row();
    var rows = uniqueRows(details);
    var newGrid = $_4jja6kk5jfuvixx1.foldl(rows, function (newGrid, _row) {
      return $_3fg4ocnejfuviykt.insertRowAt(newGrid, targetIndex, example, comparator, genWrappers.getOrInit);
    }, grid);
    return bundle(newGrid, targetIndex, details[0].column());
  };
  var insertRowAfter = function (grid, detail, comparator, genWrappers) {
    var example = detail.row();
    var targetIndex = detail.row() + detail.rowspan();
    var newGrid = $_3fg4ocnejfuviykt.insertRowAt(grid, targetIndex, example, comparator, genWrappers.getOrInit);
    return bundle(newGrid, targetIndex, detail.column());
  };
  var insertRowsAfter = function (grid, details, comparator, genWrappers) {
    var rows = uniqueRows(details);
    var example = rows[rows.length - 1].row();
    var targetIndex = rows[rows.length - 1].row() + rows[rows.length - 1].rowspan();
    var newGrid = $_4jja6kk5jfuvixx1.foldl(rows, function (newGrid, _row) {
      return $_3fg4ocnejfuviykt.insertRowAt(newGrid, targetIndex, example, comparator, genWrappers.getOrInit);
    }, grid);
    return bundle(newGrid, targetIndex, details[0].column());
  };
  var insertColumnBefore = function (grid, detail, comparator, genWrappers) {
    var example = detail.column();
    var targetIndex = detail.column();
    var newGrid = $_3fg4ocnejfuviykt.insertColumnAt(grid, targetIndex, example, comparator, genWrappers.getOrInit);
    return bundle(newGrid, detail.row(), targetIndex);
  };
  var insertColumnsBefore = function (grid, details, comparator, genWrappers) {
    var columns = uniqueColumns(details);
    var example = columns[0].column();
    var targetIndex = columns[0].column();
    var newGrid = $_4jja6kk5jfuvixx1.foldl(columns, function (newGrid, _row) {
      return $_3fg4ocnejfuviykt.insertColumnAt(newGrid, targetIndex, example, comparator, genWrappers.getOrInit);
    }, grid);
    return bundle(newGrid, details[0].row(), targetIndex);
  };
  var insertColumnAfter = function (grid, detail, comparator, genWrappers) {
    var example = detail.column();
    var targetIndex = detail.column() + detail.colspan();
    var newGrid = $_3fg4ocnejfuviykt.insertColumnAt(grid, targetIndex, example, comparator, genWrappers.getOrInit);
    return bundle(newGrid, detail.row(), targetIndex);
  };
  var insertColumnsAfter = function (grid, details, comparator, genWrappers) {
    var example = details[details.length - 1].column();
    var targetIndex = details[details.length - 1].column() + details[details.length - 1].colspan();
    var columns = uniqueColumns(details);
    var newGrid = $_4jja6kk5jfuvixx1.foldl(columns, function (newGrid, _row) {
      return $_3fg4ocnejfuviykt.insertColumnAt(newGrid, targetIndex, example, comparator, genWrappers.getOrInit);
    }, grid);
    return bundle(newGrid, details[0].row(), targetIndex);
  };
  var makeRowHeader = function (grid, detail, comparator, genWrappers) {
    var newGrid = $_qcrcfnfjfuviykz.replaceRow(grid, detail.row(), comparator, genWrappers.replaceOrInit);
    return bundle(newGrid, detail.row(), detail.column());
  };
  var makeColumnHeader = function (grid, detail, comparator, genWrappers) {
    var newGrid = $_qcrcfnfjfuviykz.replaceColumn(grid, detail.column(), comparator, genWrappers.replaceOrInit);
    return bundle(newGrid, detail.row(), detail.column());
  };
  var unmakeRowHeader = function (grid, detail, comparator, genWrappers) {
    var newGrid = $_qcrcfnfjfuviykz.replaceRow(grid, detail.row(), comparator, genWrappers.replaceOrInit);
    return bundle(newGrid, detail.row(), detail.column());
  };
  var unmakeColumnHeader = function (grid, detail, comparator, genWrappers) {
    var newGrid = $_qcrcfnfjfuviykz.replaceColumn(grid, detail.column(), comparator, genWrappers.replaceOrInit);
    return bundle(newGrid, detail.row(), detail.column());
  };
  var splitCellIntoColumns$1 = function (grid, detail, comparator, genWrappers) {
    var newGrid = $_3fg4ocnejfuviykt.splitCellIntoColumns(grid, detail.row(), detail.column(), comparator, genWrappers.getOrInit);
    return bundle(newGrid, detail.row(), detail.column());
  };
  var splitCellIntoRows$1 = function (grid, detail, comparator, genWrappers) {
    var newGrid = $_3fg4ocnejfuviykt.splitCellIntoRows(grid, detail.row(), detail.column(), comparator, genWrappers.getOrInit);
    return bundle(newGrid, detail.row(), detail.column());
  };
  var eraseColumns = function (grid, details, comparator, _genWrappers) {
    var columns = uniqueColumns(details);
    var newGrid = $_3fg4ocnejfuviykt.deleteColumnsAt(grid, columns[0].column(), columns[columns.length - 1].column());
    var cursor = elementFromGrid(newGrid, details[0].row(), details[0].column());
    return outcome(newGrid, cursor);
  };
  var eraseRows = function (grid, details, comparator, _genWrappers) {
    var rows = uniqueRows(details);
    var newGrid = $_3fg4ocnejfuviykt.deleteRowsAt(grid, rows[0].row(), rows[rows.length - 1].row());
    var cursor = elementFromGrid(newGrid, details[0].row(), details[0].column());
    return outcome(newGrid, cursor);
  };
  var mergeCells = function (grid, mergable, comparator, _genWrappers) {
    var cells = mergable.cells();
    $_af8qgymqjfuviyf9.merge(cells);
    var newGrid = $_aovyapndjfuviykl.merge(grid, mergable.bounds(), comparator, $_fdch7uk7jfuvixxb.constant(cells[0]));
    return outcome(newGrid, Option.from(cells[0]));
  };
  var unmergeCells = function (grid, unmergable, comparator, genWrappers) {
    var newGrid = $_4jja6kk5jfuvixx1.foldr(unmergable, function (b, cell) {
      return $_aovyapndjfuviykl.unmerge(b, cell, comparator, genWrappers.combine(cell));
    }, grid);
    return outcome(newGrid, Option.from(unmergable[0]));
  };
  var pasteCells = function (grid, pasteDetails, comparator, genWrappers) {
    var gridify = function (table, generators) {
      var list = $_2vc4gykfjfuvixyi.fromTable(table);
      var wh = $_bwrthsldjfuviy3q.generate(list);
      return $_b883t1mwjfuviygz.toGrid(wh, generators, true);
    };
    var gridB = gridify(pasteDetails.clipboard(), pasteDetails.generators());
    var startAddress = $_g02m1vkgjfuvixyt.address(pasteDetails.row(), pasteDetails.column());
    var mergedGrid = $_e6g8nvnajfuviyk4.merge(startAddress, grid, gridB, pasteDetails.generators(), comparator);
    return mergedGrid.fold(function () {
      return outcome(grid, Option.some(pasteDetails.element()));
    }, function (nuGrid) {
      var cursor = elementFromGrid(nuGrid, pasteDetails.row(), pasteDetails.column());
      return outcome(nuGrid, cursor);
    });
  };
  var gridifyRows = function (rows, generators, example) {
    var pasteDetails = $_2vc4gykfjfuvixyi.fromPastedRows(rows, example);
    var wh = $_bwrthsldjfuviy3q.generate(pasteDetails);
    return $_b883t1mwjfuviygz.toGrid(wh, generators, true);
  };
  var pasteRowsBefore = function (grid, pasteDetails, comparator, genWrappers) {
    var example = grid[pasteDetails.cells[0].row()];
    var index = pasteDetails.cells[0].row();
    var gridB = gridifyRows(pasteDetails.clipboard(), pasteDetails.generators(), example);
    var mergedGrid = $_e6g8nvnajfuviyk4.insert(index, grid, gridB, pasteDetails.generators(), comparator);
    var cursor = elementFromGrid(mergedGrid, pasteDetails.cells[0].row(), pasteDetails.cells[0].column());
    return outcome(mergedGrid, cursor);
  };
  var pasteRowsAfter = function (grid, pasteDetails, comparator, genWrappers) {
    var example = grid[pasteDetails.cells[0].row()];
    var index = pasteDetails.cells[pasteDetails.cells.length - 1].row() + pasteDetails.cells[pasteDetails.cells.length - 1].rowspan();
    var gridB = gridifyRows(pasteDetails.clipboard(), pasteDetails.generators(), example);
    var mergedGrid = $_e6g8nvnajfuviyk4.insert(index, grid, gridB, pasteDetails.generators(), comparator);
    var cursor = elementFromGrid(mergedGrid, pasteDetails.cells[0].row(), pasteDetails.cells[0].column());
    return outcome(mergedGrid, cursor);
  };
  var resize = $_f6vpt3ngjfuviyl3.adjustWidthTo;
  var $_gdqcovmmjfuviye2 = {
    insertRowBefore: $_b9rtfhmtjfuviygc.run(insertRowBefore, $_b9rtfhmtjfuviygc.onCell, $_fdch7uk7jfuvixxb.noop, $_fdch7uk7jfuvixxb.noop, $_blm67mmnjfuviyel.modification),
    insertRowsBefore: $_b9rtfhmtjfuviygc.run(insertRowsBefore, $_b9rtfhmtjfuviygc.onCells, $_fdch7uk7jfuvixxb.noop, $_fdch7uk7jfuvixxb.noop, $_blm67mmnjfuviyel.modification),
    insertRowAfter: $_b9rtfhmtjfuviygc.run(insertRowAfter, $_b9rtfhmtjfuviygc.onCell, $_fdch7uk7jfuvixxb.noop, $_fdch7uk7jfuvixxb.noop, $_blm67mmnjfuviyel.modification),
    insertRowsAfter: $_b9rtfhmtjfuviygc.run(insertRowsAfter, $_b9rtfhmtjfuviygc.onCells, $_fdch7uk7jfuvixxb.noop, $_fdch7uk7jfuvixxb.noop, $_blm67mmnjfuviyel.modification),
    insertColumnBefore: $_b9rtfhmtjfuviygc.run(insertColumnBefore, $_b9rtfhmtjfuviygc.onCell, resize, $_fdch7uk7jfuvixxb.noop, $_blm67mmnjfuviyel.modification),
    insertColumnsBefore: $_b9rtfhmtjfuviygc.run(insertColumnsBefore, $_b9rtfhmtjfuviygc.onCells, resize, $_fdch7uk7jfuvixxb.noop, $_blm67mmnjfuviyel.modification),
    insertColumnAfter: $_b9rtfhmtjfuviygc.run(insertColumnAfter, $_b9rtfhmtjfuviygc.onCell, resize, $_fdch7uk7jfuvixxb.noop, $_blm67mmnjfuviyel.modification),
    insertColumnsAfter: $_b9rtfhmtjfuviygc.run(insertColumnsAfter, $_b9rtfhmtjfuviygc.onCells, resize, $_fdch7uk7jfuvixxb.noop, $_blm67mmnjfuviyel.modification),
    splitCellIntoColumns: $_b9rtfhmtjfuviygc.run(splitCellIntoColumns$1, $_b9rtfhmtjfuviygc.onCell, resize, $_fdch7uk7jfuvixxb.noop, $_blm67mmnjfuviyel.modification),
    splitCellIntoRows: $_b9rtfhmtjfuviygc.run(splitCellIntoRows$1, $_b9rtfhmtjfuviygc.onCell, $_fdch7uk7jfuvixxb.noop, $_fdch7uk7jfuvixxb.noop, $_blm67mmnjfuviyel.modification),
    eraseColumns: $_b9rtfhmtjfuviygc.run(eraseColumns, $_b9rtfhmtjfuviygc.onCells, resize, prune, $_blm67mmnjfuviyel.modification),
    eraseRows: $_b9rtfhmtjfuviygc.run(eraseRows, $_b9rtfhmtjfuviygc.onCells, $_fdch7uk7jfuvixxb.noop, prune, $_blm67mmnjfuviyel.modification),
    makeColumnHeader: $_b9rtfhmtjfuviygc.run(makeColumnHeader, $_b9rtfhmtjfuviygc.onCell, $_fdch7uk7jfuvixxb.noop, $_fdch7uk7jfuvixxb.noop, $_blm67mmnjfuviyel.transform('row', 'th')),
    unmakeColumnHeader: $_b9rtfhmtjfuviygc.run(unmakeColumnHeader, $_b9rtfhmtjfuviygc.onCell, $_fdch7uk7jfuvixxb.noop, $_fdch7uk7jfuvixxb.noop, $_blm67mmnjfuviyel.transform(null, 'td')),
    makeRowHeader: $_b9rtfhmtjfuviygc.run(makeRowHeader, $_b9rtfhmtjfuviygc.onCell, $_fdch7uk7jfuvixxb.noop, $_fdch7uk7jfuvixxb.noop, $_blm67mmnjfuviyel.transform('col', 'th')),
    unmakeRowHeader: $_b9rtfhmtjfuviygc.run(unmakeRowHeader, $_b9rtfhmtjfuviygc.onCell, $_fdch7uk7jfuvixxb.noop, $_fdch7uk7jfuvixxb.noop, $_blm67mmnjfuviyel.transform(null, 'td')),
    mergeCells: $_b9rtfhmtjfuviygc.run(mergeCells, $_b9rtfhmtjfuviygc.onMergable, $_fdch7uk7jfuvixxb.noop, $_fdch7uk7jfuvixxb.noop, $_blm67mmnjfuviyel.merging),
    unmergeCells: $_b9rtfhmtjfuviygc.run(unmergeCells, $_b9rtfhmtjfuviygc.onUnmergable, resize, $_fdch7uk7jfuvixxb.noop, $_blm67mmnjfuviyel.merging),
    pasteCells: $_b9rtfhmtjfuviygc.run(pasteCells, $_b9rtfhmtjfuviygc.onPaste, resize, $_fdch7uk7jfuvixxb.noop, $_blm67mmnjfuviyel.modification),
    pasteRowsBefore: $_b9rtfhmtjfuviygc.run(pasteRowsBefore, $_b9rtfhmtjfuviygc.onPasteRows, $_fdch7uk7jfuvixxb.noop, $_fdch7uk7jfuvixxb.noop, $_blm67mmnjfuviyel.modification),
    pasteRowsAfter: $_b9rtfhmtjfuviygc.run(pasteRowsAfter, $_b9rtfhmtjfuviygc.onPasteRows, $_fdch7uk7jfuvixxb.noop, $_fdch7uk7jfuvixxb.noop, $_blm67mmnjfuviyel.modification)
  };

  var getBody$1 = function (editor) {
    return $_4sdhm4kkjfuviy0e.fromDom(editor.getBody());
  };
  var getIsRoot = function (editor) {
    return function (element) {
      return $_g6ztqikojfuviy13.eq(element, getBody$1(editor));
    };
  };
  var removePxSuffix = function (size) {
    return size ? size.replace(/px$/, '') : '';
  };
  var addSizeSuffix = function (size) {
    if (/^[0-9]+$/.test(size)) {
      size += 'px';
    }
    return size;
  };
  var $_aheu0fnnjfuviymj = {
    getBody: getBody$1,
    getIsRoot: getIsRoot,
    addSizeSuffix: addSizeSuffix,
    removePxSuffix: removePxSuffix
  };

  var onDirection = function (isLtr, isRtl) {
    return function (element) {
      return getDirection(element) === 'rtl' ? isRtl : isLtr;
    };
  };
  var getDirection = function (element) {
    return $_2lr8nrlejfuviy40.get(element, 'direction') === 'rtl' ? 'rtl' : 'ltr';
  };
  var $_8szyc1npjfuviyms = {
    onDirection: onDirection,
    getDirection: getDirection
  };

  var ltr$1 = { isRtl: $_fdch7uk7jfuvixxb.constant(false) };
  var rtl$1 = { isRtl: $_fdch7uk7jfuvixxb.constant(true) };
  var directionAt = function (element) {
    var dir = $_8szyc1npjfuviyms.getDirection(element);
    return dir === 'rtl' ? rtl$1 : ltr$1;
  };
  var $_1g25rnojfuviymn = { directionAt: directionAt };

  var defaultTableToolbar = [
    'tableprops',
    'tabledelete',
    '|',
    'tableinsertrowbefore',
    'tableinsertrowafter',
    'tabledeleterow',
    '|',
    'tableinsertcolbefore',
    'tableinsertcolafter',
    'tabledeletecol'
  ];
  var defaultStyles = {
    'border-collapse': 'collapse',
    'width': '100%'
  };
  var defaultAttributes = { border: '1' };
  var getDefaultAttributes = function (editor) {
    return editor.getParam('table_default_attributes', defaultAttributes, 'object');
  };
  var getDefaultStyles = function (editor) {
    return editor.getParam('table_default_styles', defaultStyles, 'object');
  };
  var hasTableResizeBars = function (editor) {
    return editor.getParam('table_resize_bars', true, 'boolean');
  };
  var hasTabNavigation = function (editor) {
    return editor.getParam('table_tab_navigation', true, 'boolean');
  };
  var hasAdvancedCellTab = function (editor) {
    return editor.getParam('table_cell_advtab', true, 'boolean');
  };
  var hasAdvancedRowTab = function (editor) {
    return editor.getParam('table_row_advtab', true, 'boolean');
  };
  var hasAdvancedTableTab = function (editor) {
    return editor.getParam('table_advtab', true, 'boolean');
  };
  var hasAppearanceOptions = function (editor) {
    return editor.getParam('table_appearance_options', true, 'boolean');
  };
  var hasTableGrid = function (editor) {
    return editor.getParam('table_grid', true, 'boolean');
  };
  var shouldStyleWithCss = function (editor) {
    return editor.getParam('table_style_by_css', false, 'boolean');
  };
  var getCellClassList = function (editor) {
    return editor.getParam('table_cell_class_list', [], 'array');
  };
  var getRowClassList = function (editor) {
    return editor.getParam('table_row_class_list', [], 'array');
  };
  var getTableClassList = function (editor) {
    return editor.getParam('table_class_list', [], 'array');
  };
  var getColorPickerCallback = function (editor) {
    return editor.getParam('color_picker_callback');
  };
  var isPixelsForced = function (editor) {
    return editor.getParam('table_responsive_width') === false;
  };
  var getCloneElements = function (editor) {
    var cloneElements = editor.getParam('table_clone_elements');
    if ($_13kw1fk8jfuvixxd.isString(cloneElements)) {
      return Option.some(cloneElements.split(/[ ,]/));
    } else if (Array.isArray(cloneElements)) {
      return Option.some(cloneElements);
    } else {
      return Option.none();
    }
  };
  var hasObjectResizing = function (editor) {
    var objectResizing = editor.getParam('object_resizing', true);
    return objectResizing === 'table' || objectResizing;
  };
  var getToolbar = function (editor) {
    var toolbar = editor.getParam('table_toolbar', defaultTableToolbar);
    if (toolbar === '' || toolbar === false) {
      return [];
    } else if ($_13kw1fk8jfuvixxd.isString(toolbar)) {
      return toolbar.split(/[ ,]/);
    } else if ($_13kw1fk8jfuvixxd.isArray(toolbar)) {
      return toolbar;
    } else {
      return [];
    }
  };

  var fireNewRow = function (editor, row) {
    return editor.fire('newrow', { node: row });
  };
  var fireNewCell = function (editor, cell) {
    return editor.fire('newcell', { node: cell });
  };

  var TableActions = function (editor, lazyWire) {
    var isTableBody = function (editor) {
      return $_6mcqmml6jfuviy2u.name($_aheu0fnnjfuviymj.getBody(editor)) === 'table';
    };
    var lastRowGuard = function (table) {
      var size = $_23hnlomljfuviydz.getGridSize(table);
      return isTableBody(editor) === false || size.rows() > 1;
    };
    var lastColumnGuard = function (table) {
      var size = $_23hnlomljfuviydz.getGridSize(table);
      return isTableBody(editor) === false || size.columns() > 1;
    };
    var cloneFormats = getCloneElements(editor);
    var execute = function (operation, guard, mutate, lazyWire) {
      return function (table, target) {
        var dataStyleCells = $_a3hs1bl7jfuviy2w.descendants(table, 'td[data-mce-style],th[data-mce-style]');
        $_4jja6kk5jfuvixx1.each(dataStyleCells, function (cell) {
          $_2ekobel5jfuviy2m.remove(cell, 'data-mce-style');
        });
        var wire = lazyWire();
        var doc = $_4sdhm4kkjfuviy0e.fromDom(editor.getDoc());
        var direction = TableDirection($_1g25rnojfuviymn.directionAt);
        var generators = $_clky4ljjfuviy4t.cellOperations(mutate, doc, cloneFormats);
        return guard(table) ? operation(wire, table, target, generators, direction).bind(function (result) {
          $_4jja6kk5jfuvixx1.each(result.newRows(), function (row) {
            fireNewRow(editor, row.dom());
          });
          $_4jja6kk5jfuvixx1.each(result.newCells(), function (cell) {
            fireNewCell(editor, cell.dom());
          });
          return result.cursor().map(function (cell) {
            var rng = editor.dom.createRng();
            rng.setStart(cell.dom(), 0);
            rng.setEnd(cell.dom(), 0);
            return rng;
          });
        }) : Option.none();
      };
    };
    var deleteRow = execute($_gdqcovmmjfuviye2.eraseRows, lastRowGuard, $_fdch7uk7jfuvixxb.noop, lazyWire);
    var deleteColumn = execute($_gdqcovmmjfuviye2.eraseColumns, lastColumnGuard, $_fdch7uk7jfuvixxb.noop, lazyWire);
    var insertRowsBefore = execute($_gdqcovmmjfuviye2.insertRowsBefore, $_fdch7uk7jfuvixxb.always, $_fdch7uk7jfuvixxb.noop, lazyWire);
    var insertRowsAfter = execute($_gdqcovmmjfuviye2.insertRowsAfter, $_fdch7uk7jfuvixxb.always, $_fdch7uk7jfuvixxb.noop, lazyWire);
    var insertColumnsBefore = execute($_gdqcovmmjfuviye2.insertColumnsBefore, $_fdch7uk7jfuvixxb.always, $_7fw3p6m9jfuviybu.halve, lazyWire);
    var insertColumnsAfter = execute($_gdqcovmmjfuviye2.insertColumnsAfter, $_fdch7uk7jfuvixxb.always, $_7fw3p6m9jfuviybu.halve, lazyWire);
    var mergeCells = execute($_gdqcovmmjfuviye2.mergeCells, $_fdch7uk7jfuvixxb.always, $_fdch7uk7jfuvixxb.noop, lazyWire);
    var unmergeCells = execute($_gdqcovmmjfuviye2.unmergeCells, $_fdch7uk7jfuvixxb.always, $_fdch7uk7jfuvixxb.noop, lazyWire);
    var pasteRowsBefore = execute($_gdqcovmmjfuviye2.pasteRowsBefore, $_fdch7uk7jfuvixxb.always, $_fdch7uk7jfuvixxb.noop, lazyWire);
    var pasteRowsAfter = execute($_gdqcovmmjfuviye2.pasteRowsAfter, $_fdch7uk7jfuvixxb.always, $_fdch7uk7jfuvixxb.noop, lazyWire);
    var pasteCells = execute($_gdqcovmmjfuviye2.pasteCells, $_fdch7uk7jfuvixxb.always, $_fdch7uk7jfuvixxb.noop, lazyWire);
    return {
      deleteRow: deleteRow,
      deleteColumn: deleteColumn,
      insertRowsBefore: insertRowsBefore,
      insertRowsAfter: insertRowsAfter,
      insertColumnsBefore: insertColumnsBefore,
      insertColumnsAfter: insertColumnsAfter,
      mergeCells: mergeCells,
      unmergeCells: unmergeCells,
      pasteRowsBefore: pasteRowsBefore,
      pasteRowsAfter: pasteRowsAfter,
      pasteCells: pasteCells
    };
  };

  var copyRows = function (table, target, generators) {
    var list = $_2vc4gykfjfuvixyi.fromTable(table);
    var house = $_bwrthsldjfuviy3q.generate(list);
    var details = $_b9rtfhmtjfuviygc.onCells(house, target);
    return details.map(function (selectedCells) {
      var grid = $_b883t1mwjfuviygz.toGrid(house, generators, false);
      var slicedGrid = grid.slice(selectedCells[0].row(), selectedCells[selectedCells.length - 1].row() + selectedCells[selectedCells.length - 1].rowspan());
      var slicedDetails = $_b9rtfhmtjfuviygc.toDetailList(slicedGrid, generators);
      return $_clwdhumzjfuviyhf.copy(slicedDetails);
    });
  };
  var $_bjdegsntjfuviynl = { copyRows: copyRows };

  var global$2 = tinymce.util.Tools.resolve('tinymce.util.Tools');

  var getTDTHOverallStyle = function (dom, elm, name) {
    var cells = dom.select('td,th', elm);
    var firstChildStyle;
    var checkChildren = function (firstChildStyle, elms) {
      for (var i = 0; i < elms.length; i++) {
        var currentStyle = dom.getStyle(elms[i], name);
        if (typeof firstChildStyle === 'undefined') {
          firstChildStyle = currentStyle;
        }
        if (firstChildStyle !== currentStyle) {
          return '';
        }
      }
      return firstChildStyle;
    };
    firstChildStyle = checkChildren(firstChildStyle, cells);
    return firstChildStyle;
  };
  var applyAlign = function (editor, elm, name) {
    if (name) {
      editor.formatter.apply('align' + name, {}, elm);
    }
  };
  var applyVAlign = function (editor, elm, name) {
    if (name) {
      editor.formatter.apply('valign' + name, {}, elm);
    }
  };
  var unApplyAlign = function (editor, elm) {
    global$2.each('left center right'.split(' '), function (name) {
      editor.formatter.remove('align' + name, {}, elm);
    });
  };
  var unApplyVAlign = function (editor, elm) {
    global$2.each('top middle bottom'.split(' '), function (name) {
      editor.formatter.remove('valign' + name, {}, elm);
    });
  };
  var $_35zgp3nwjfuviynz = {
    applyAlign: applyAlign,
    applyVAlign: applyVAlign,
    unApplyAlign: unApplyAlign,
    unApplyVAlign: unApplyVAlign,
    getTDTHOverallStyle: getTDTHOverallStyle
  };

  var buildListItems = function (inputList, itemCallback, startItems) {
    var appendItems = function (values, output) {
      output = output || [];
      global$2.each(values, function (item) {
        var menuItem = { text: item.text || item.title };
        if (item.menu) {
          menuItem.menu = appendItems(item.menu);
        } else {
          menuItem.value = item.value;
          if (itemCallback) {
            itemCallback(menuItem);
          }
        }
        output.push(menuItem);
      });
      return output;
    };
    return appendItems(inputList, startItems || []);
  };
  var updateStyleField = function (editor, evt) {
    var dom = editor.dom;
    var rootControl = evt.control.rootControl;
    var data = rootControl.toJSON();
    var css = dom.parseStyle(data.style);
    if (evt.control.name() === 'style') {
      rootControl.find('#borderStyle').value(css['border-style'] || '')[0].fire('select');
      rootControl.find('#borderColor').value(css['border-color'] || '')[0].fire('change');
      rootControl.find('#backgroundColor').value(css['background-color'] || '')[0].fire('change');
      rootControl.find('#width').value(css.width || '').fire('change');
      rootControl.find('#height').value(css.height || '').fire('change');
    } else {
      css['border-style'] = data.borderStyle;
      css['border-color'] = data.borderColor;
      css['background-color'] = data.backgroundColor;
      css.width = data.width ? $_aheu0fnnjfuviymj.addSizeSuffix(data.width) : '';
      css.height = data.height ? $_aheu0fnnjfuviymj.addSizeSuffix(data.height) : '';
    }
    rootControl.find('#style').value(dom.serializeStyle(dom.parseStyle(dom.serializeStyle(css))));
  };
  var extractAdvancedStyles = function (dom, elm) {
    var css = dom.parseStyle(dom.getAttrib(elm, 'style'));
    var data = {};
    if (css['border-style']) {
      data.borderStyle = css['border-style'];
    }
    if (css['border-color']) {
      data.borderColor = css['border-color'];
    }
    if (css['background-color']) {
      data.backgroundColor = css['background-color'];
    }
    data.style = dom.serializeStyle(css);
    return data;
  };
  var createStyleForm = function (editor) {
    var createColorPickAction = function () {
      var colorPickerCallback = getColorPickerCallback(editor);
      if (colorPickerCallback) {
        return function (evt) {
          return colorPickerCallback.call(editor, function (value) {
            evt.control.value(value).fire('change');
          }, evt.control.value());
        };
      }
    };
    return {
      title: 'Advanced',
      type: 'form',
      defaults: { onchange: $_fdch7uk7jfuvixxb.curry(updateStyleField, editor) },
      items: [
        {
          label: 'Style',
          name: 'style',
          type: 'textbox'
        },
        {
          type: 'form',
          padding: 0,
          formItemDefaults: {
            layout: 'grid',
            alignH: [
              'start',
              'right'
            ]
          },
          defaults: { size: 7 },
          items: [
            {
              label: 'Border style',
              type: 'listbox',
              name: 'borderStyle',
              width: 90,
              onselect: $_fdch7uk7jfuvixxb.curry(updateStyleField, editor),
              values: [
                {
                  text: 'Select...',
                  value: ''
                },
                {
                  text: 'Solid',
                  value: 'solid'
                },
                {
                  text: 'Dotted',
                  value: 'dotted'
                },
                {
                  text: 'Dashed',
                  value: 'dashed'
                },
                {
                  text: 'Double',
                  value: 'double'
                },
                {
                  text: 'Groove',
                  value: 'groove'
                },
                {
                  text: 'Ridge',
                  value: 'ridge'
                },
                {
                  text: 'Inset',
                  value: 'inset'
                },
                {
                  text: 'Outset',
                  value: 'outset'
                },
                {
                  text: 'None',
                  value: 'none'
                },
                {
                  text: 'Hidden',
                  value: 'hidden'
                }
              ]
            },
            {
              label: 'Border color',
              type: 'colorbox',
              name: 'borderColor',
              onaction: createColorPickAction()
            },
            {
              label: 'Background color',
              type: 'colorbox',
              name: 'backgroundColor',
              onaction: createColorPickAction()
            }
          ]
        }
      ]
    };
  };
  var $_etnm2gnxjfuviyo0 = {
    createStyleForm: createStyleForm,
    buildListItems: buildListItems,
    updateStyleField: updateStyleField,
    extractAdvancedStyles: extractAdvancedStyles
  };

  var updateStyles = function (elm, cssText) {
    delete elm.dataset.mceStyle;
    elm.style.cssText += ';' + cssText;
  };
  var extractDataFromElement = function (editor, elm) {
    var dom = editor.dom;
    var data = {
      width: dom.getStyle(elm, 'width') || dom.getAttrib(elm, 'width'),
      height: dom.getStyle(elm, 'height') || dom.getAttrib(elm, 'height'),
      scope: dom.getAttrib(elm, 'scope'),
      class: dom.getAttrib(elm, 'class'),
      type: elm.nodeName.toLowerCase(),
      style: '',
      align: '',
      valign: ''
    };
    global$2.each('left center right'.split(' '), function (name) {
      if (editor.formatter.matchNode(elm, 'align' + name)) {
        data.align = name;
      }
    });
    global$2.each('top middle bottom'.split(' '), function (name) {
      if (editor.formatter.matchNode(elm, 'valign' + name)) {
        data.valign = name;
      }
    });
    if (hasAdvancedCellTab(editor)) {
      global$2.extend(data, $_etnm2gnxjfuviyo0.extractAdvancedStyles(dom, elm));
    }
    return data;
  };
  var onSubmitCellForm = function (editor, cells, evt) {
    var dom = editor.dom;
    var data;
    function setAttrib(elm, name, value) {
      if (value) {
        dom.setAttrib(elm, name, value);
      }
    }
    function setStyle(elm, name, value) {
      if (value) {
        dom.setStyle(elm, name, value);
      }
    }
    $_etnm2gnxjfuviyo0.updateStyleField(editor, evt);
    data = evt.control.rootControl.toJSON();
    editor.undoManager.transact(function () {
      global$2.each(cells, function (cellElm) {
        setAttrib(cellElm, 'scope', data.scope);
        if (cells.length === 1) {
          setAttrib(cellElm, 'style', data.style);
        } else {
          updateStyles(cellElm, data.style);
        }
        setAttrib(cellElm, 'class', data.class);
        setStyle(cellElm, 'width', $_aheu0fnnjfuviymj.addSizeSuffix(data.width));
        setStyle(cellElm, 'height', $_aheu0fnnjfuviymj.addSizeSuffix(data.height));
        if (data.type && cellElm.nodeName.toLowerCase() !== data.type) {
          cellElm = dom.rename(cellElm, data.type);
        }
        if (cells.length === 1) {
          $_35zgp3nwjfuviynz.unApplyAlign(editor, cellElm);
          $_35zgp3nwjfuviynz.unApplyVAlign(editor, cellElm);
        }
        if (data.align) {
          $_35zgp3nwjfuviynz.applyAlign(editor, cellElm, data.align);
        }
        if (data.valign) {
          $_35zgp3nwjfuviynz.applyVAlign(editor, cellElm, data.valign);
        }
      });
      editor.focus();
    });
  };
  var open = function (editor) {
    var cellElm, data, classListCtrl, cells = [];
    cells = editor.dom.select('td[data-mce-selected],th[data-mce-selected]');
    cellElm = editor.dom.getParent(editor.selection.getStart(), 'td,th');
    if (!cells.length && cellElm) {
      cells.push(cellElm);
    }
    cellElm = cellElm || cells[0];
    if (!cellElm) {
      return;
    }
    if (cells.length > 1) {
      data = {
        width: '',
        height: '',
        scope: '',
        class: '',
        align: '',
        valign: '',
        style: '',
        type: cellElm.nodeName.toLowerCase()
      };
    } else {
      data = extractDataFromElement(editor, cellElm);
    }
    if (getCellClassList(editor).length > 0) {
      classListCtrl = {
        name: 'class',
        type: 'listbox',
        label: 'Class',
        values: $_etnm2gnxjfuviyo0.buildListItems(getCellClassList(editor), function (item) {
          if (item.value) {
            item.textStyle = function () {
              return editor.formatter.getCssText({
                block: 'td',
                classes: [item.value]
              });
            };
          }
        })
      };
    }
    var generalCellForm = {
      type: 'form',
      layout: 'flex',
      direction: 'column',
      labelGapCalc: 'children',
      padding: 0,
      items: [
        {
          type: 'form',
          layout: 'grid',
          columns: 2,
          labelGapCalc: false,
          padding: 0,
          defaults: {
            type: 'textbox',
            maxWidth: 50
          },
          items: [
            {
              label: 'Width',
              name: 'width',
              onchange: $_fdch7uk7jfuvixxb.curry($_etnm2gnxjfuviyo0.updateStyleField, editor)
            },
            {
              label: 'Height',
              name: 'height',
              onchange: $_fdch7uk7jfuvixxb.curry($_etnm2gnxjfuviyo0.updateStyleField, editor)
            },
            {
              label: 'Cell type',
              name: 'type',
              type: 'listbox',
              text: 'None',
              minWidth: 90,
              maxWidth: null,
              values: [
                {
                  text: 'Cell',
                  value: 'td'
                },
                {
                  text: 'Header cell',
                  value: 'th'
                }
              ]
            },
            {
              label: 'Scope',
              name: 'scope',
              type: 'listbox',
              text: 'None',
              minWidth: 90,
              maxWidth: null,
              values: [
                {
                  text: 'None',
                  value: ''
                },
                {
                  text: 'Row',
                  value: 'row'
                },
                {
                  text: 'Column',
                  value: 'col'
                },
                {
                  text: 'Row group',
                  value: 'rowgroup'
                },
                {
                  text: 'Column group',
                  value: 'colgroup'
                }
              ]
            },
            {
              label: 'H Align',
              name: 'align',
              type: 'listbox',
              text: 'None',
              minWidth: 90,
              maxWidth: null,
              values: [
                {
                  text: 'None',
                  value: ''
                },
                {
                  text: 'Left',
                  value: 'left'
                },
                {
                  text: 'Center',
                  value: 'center'
                },
                {
                  text: 'Right',
                  value: 'right'
                }
              ]
            },
            {
              label: 'V Align',
              name: 'valign',
              type: 'listbox',
              text: 'None',
              minWidth: 90,
              maxWidth: null,
              values: [
                {
                  text: 'None',
                  value: ''
                },
                {
                  text: 'Top',
                  value: 'top'
                },
                {
                  text: 'Middle',
                  value: 'middle'
                },
                {
                  text: 'Bottom',
                  value: 'bottom'
                }
              ]
            }
          ]
        },
        classListCtrl
      ]
    };
    if (hasAdvancedCellTab(editor)) {
      editor.windowManager.open({
        title: 'Cell properties',
        bodyType: 'tabpanel',
        data: data,
        body: [
          {
            title: 'General',
            type: 'form',
            items: generalCellForm
          },
          $_etnm2gnxjfuviyo0.createStyleForm(editor)
        ],
        onsubmit: $_fdch7uk7jfuvixxb.curry(onSubmitCellForm, editor, cells)
      });
    } else {
      editor.windowManager.open({
        title: 'Cell properties',
        data: data,
        body: generalCellForm,
        onsubmit: $_fdch7uk7jfuvixxb.curry(onSubmitCellForm, editor, cells)
      });
    }
  };
  var $_ff2aywnvjfuviynt = { open: open };

  var extractDataFromElement$1 = function (editor, elm) {
    var dom = editor.dom;
    var data = {
      height: dom.getStyle(elm, 'height') || dom.getAttrib(elm, 'height'),
      scope: dom.getAttrib(elm, 'scope'),
      class: dom.getAttrib(elm, 'class'),
      align: '',
      style: '',
      type: elm.parentNode.nodeName.toLowerCase()
    };
    global$2.each('left center right'.split(' '), function (name) {
      if (editor.formatter.matchNode(elm, 'align' + name)) {
        data.align = name;
      }
    });
    if (hasAdvancedRowTab(editor)) {
      global$2.extend(data, $_etnm2gnxjfuviyo0.extractAdvancedStyles(dom, elm));
    }
    return data;
  };
  var switchRowType = function (dom, rowElm, toType) {
    var tableElm = dom.getParent(rowElm, 'table');
    var oldParentElm = rowElm.parentNode;
    var parentElm = dom.select(toType, tableElm)[0];
    if (!parentElm) {
      parentElm = dom.create(toType);
      if (tableElm.firstChild) {
        if (tableElm.firstChild.nodeName === 'CAPTION') {
          dom.insertAfter(parentElm, tableElm.firstChild);
        } else {
          tableElm.insertBefore(parentElm, tableElm.firstChild);
        }
      } else {
        tableElm.appendChild(parentElm);
      }
    }
    parentElm.appendChild(rowElm);
    if (!oldParentElm.hasChildNodes()) {
      dom.remove(oldParentElm);
    }
  };
  function onSubmitRowForm(editor, rows, oldData, evt) {
    var dom = editor.dom;
    function setAttrib(elm, name, value) {
      if (value) {
        dom.setAttrib(elm, name, value);
      }
    }
    function setStyle(elm, name, value) {
      if (value) {
        dom.setStyle(elm, name, value);
      }
    }
    $_etnm2gnxjfuviyo0.updateStyleField(editor, evt);
    var data = evt.control.rootControl.toJSON();
    editor.undoManager.transact(function () {
      global$2.each(rows, function (rowElm) {
        setAttrib(rowElm, 'scope', data.scope);
        setAttrib(rowElm, 'style', data.style);
        setAttrib(rowElm, 'class', data.class);
        setStyle(rowElm, 'height', $_aheu0fnnjfuviymj.addSizeSuffix(data.height));
        if (data.type !== rowElm.parentNode.nodeName.toLowerCase()) {
          switchRowType(editor.dom, rowElm, data.type);
        }
        if (data.align !== oldData.align) {
          $_35zgp3nwjfuviynz.unApplyAlign(editor, rowElm);
          $_35zgp3nwjfuviynz.applyAlign(editor, rowElm, data.align);
        }
      });
      editor.focus();
    });
  }
  var open$1 = function (editor) {
    var dom = editor.dom;
    var tableElm, cellElm, rowElm, classListCtrl, data;
    var rows = [];
    var generalRowForm;
    tableElm = dom.getParent(editor.selection.getStart(), 'table');
    cellElm = dom.getParent(editor.selection.getStart(), 'td,th');
    global$2.each(tableElm.rows, function (row) {
      global$2.each(row.cells, function (cell) {
        if (dom.getAttrib(cell, 'data-mce-selected') || cell === cellElm) {
          rows.push(row);
          return false;
        }
      });
    });
    rowElm = rows[0];
    if (!rowElm) {
      return;
    }
    if (rows.length > 1) {
      data = {
        height: '',
        scope: '',
        style: '',
        class: '',
        align: '',
        type: rowElm.parentNode.nodeName.toLowerCase()
      };
    } else {
      data = extractDataFromElement$1(editor, rowElm);
    }
    if (getRowClassList(editor).length > 0) {
      classListCtrl = {
        name: 'class',
        type: 'listbox',
        label: 'Class',
        values: $_etnm2gnxjfuviyo0.buildListItems(getRowClassList(editor), function (item) {
          if (item.value) {
            item.textStyle = function () {
              return editor.formatter.getCssText({
                block: 'tr',
                classes: [item.value]
              });
            };
          }
        })
      };
    }
    generalRowForm = {
      type: 'form',
      columns: 2,
      padding: 0,
      defaults: { type: 'textbox' },
      items: [
        {
          type: 'listbox',
          name: 'type',
          label: 'Row type',
          text: 'Header',
          maxWidth: null,
          values: [
            {
              text: 'Header',
              value: 'thead'
            },
            {
              text: 'Body',
              value: 'tbody'
            },
            {
              text: 'Footer',
              value: 'tfoot'
            }
          ]
        },
        {
          type: 'listbox',
          name: 'align',
          label: 'Alignment',
          text: 'None',
          maxWidth: null,
          values: [
            {
              text: 'None',
              value: ''
            },
            {
              text: 'Left',
              value: 'left'
            },
            {
              text: 'Center',
              value: 'center'
            },
            {
              text: 'Right',
              value: 'right'
            }
          ]
        },
        {
          label: 'Height',
          name: 'height'
        },
        classListCtrl
      ]
    };
    if (hasAdvancedRowTab(editor)) {
      editor.windowManager.open({
        title: 'Row properties',
        data: data,
        bodyType: 'tabpanel',
        body: [
          {
            title: 'General',
            type: 'form',
            items: generalRowForm
          },
          $_etnm2gnxjfuviyo0.createStyleForm(editor)
        ],
        onsubmit: $_fdch7uk7jfuvixxb.curry(onSubmitRowForm, editor, rows, data)
      });
    } else {
      editor.windowManager.open({
        title: 'Row properties',
        data: data,
        body: generalRowForm,
        onsubmit: $_fdch7uk7jfuvixxb.curry(onSubmitRowForm, editor, rows, data)
      });
    }
  };
  var $_9342zrnyjfuviyo6 = { open: open$1 };

  var global$3 = tinymce.util.Tools.resolve('tinymce.Env');

  var DefaultRenderOptions = {
    styles: {
      'border-collapse': 'collapse',
      width: '100%'
    },
    attributes: { border: '1' },
    percentages: true
  };
  var makeTable = function () {
    return $_4sdhm4kkjfuviy0e.fromTag('table');
  };
  var tableBody = function () {
    return $_4sdhm4kkjfuviy0e.fromTag('tbody');
  };
  var tableRow = function () {
    return $_4sdhm4kkjfuviy0e.fromTag('tr');
  };
  var tableHeaderCell = function () {
    return $_4sdhm4kkjfuviy0e.fromTag('th');
  };
  var tableCell = function () {
    return $_4sdhm4kkjfuviy0e.fromTag('td');
  };
  var render$1 = function (rows, columns, rowHeaders, columnHeaders, renderOpts) {
    if (renderOpts === void 0) {
      renderOpts = DefaultRenderOptions;
    }
    var table = makeTable();
    $_2lr8nrlejfuviy40.setAll(table, renderOpts.styles);
    $_2ekobel5jfuviy2m.setAll(table, renderOpts.attributes);
    var tbody = tableBody();
    $_5zcsfmlgjfuviy4g.append(table, tbody);
    var trs = [];
    for (var i = 0; i < rows; i++) {
      var tr = tableRow();
      for (var j = 0; j < columns; j++) {
        var td = i < rowHeaders || j < columnHeaders ? tableHeaderCell() : tableCell();
        if (j < columnHeaders) {
          $_2ekobel5jfuviy2m.set(td, 'scope', 'row');
        }
        if (i < rowHeaders) {
          $_2ekobel5jfuviy2m.set(td, 'scope', 'col');
        }
        $_5zcsfmlgjfuviy4g.append(td, $_4sdhm4kkjfuviy0e.fromTag('br'));
        if (renderOpts.percentages) {
          $_2lr8nrlejfuviy40.set(td, 'width', 100 / columns + '%');
        }
        $_5zcsfmlgjfuviy4g.append(tr, td);
      }
      trs.push(tr);
    }
    $_44mr3plijfuviy4p.append(tbody, trs);
    return table;
  };

  var get$7 = function (element) {
    return element.dom().innerHTML;
  };
  var set$5 = function (element, content) {
    var owner = $_87w3h3kmjfuviy0m.owner(element);
    var docDom = owner.dom();
    var fragment = $_4sdhm4kkjfuviy0e.fromDom(docDom.createDocumentFragment());
    var contentElements = $_ek5zoelpjfuviy65.fromHtml(content, docDom);
    $_44mr3plijfuviy4p.append(fragment, contentElements);
    $_5ud3colhjfuviy4l.empty(element);
    $_5zcsfmlgjfuviy4g.append(element, fragment);
  };
  var getOuter$2 = function (element) {
    var container = $_4sdhm4kkjfuviy0e.fromTag('div');
    var clone = $_4sdhm4kkjfuviy0e.fromDom(element.dom().cloneNode(true));
    $_5zcsfmlgjfuviy4g.append(container, clone);
    return get$7(container);
  };
  var $_59qtovo4jfuviypd = {
    get: get$7,
    set: set$5,
    getOuter: getOuter$2
  };

  var placeCaretInCell = function (editor, cell) {
    editor.selection.select(cell.dom(), true);
    editor.selection.collapse(true);
  };
  var selectFirstCellInTable = function (editor, tableElm) {
    $_26gnp6lajfuviy35.descendant(tableElm, 'td,th').each($_fdch7uk7jfuvixxb.curry(placeCaretInCell, editor));
  };
  var fireEvents = function (editor, table) {
    $_4jja6kk5jfuvixx1.each($_a3hs1bl7jfuviy2w.descendants(table, 'tr'), function (row) {
      fireNewRow(editor, row.dom());
      $_4jja6kk5jfuvixx1.each($_a3hs1bl7jfuviy2w.descendants(row, 'th,td'), function (cell) {
        fireNewCell(editor, cell.dom());
      });
    });
  };
  var isPercentage = function (width) {
    return $_13kw1fk8jfuvixxd.isString(width) && width.indexOf('%') !== -1;
  };
  var insert$1 = function (editor, columns, rows) {
    var defaultStyles = getDefaultStyles(editor);
    var options = {
      styles: defaultStyles,
      attributes: getDefaultAttributes(editor),
      percentages: isPercentage(defaultStyles.width) && !isPixelsForced(editor)
    };
    var table = render$1(rows, columns, 0, 0, options);
    $_2ekobel5jfuviy2m.set(table, 'data-mce-id', '__mce');
    var html = $_59qtovo4jfuviypd.getOuter(table);
    editor.insertContent(html);
    return $_26gnp6lajfuviy35.descendant($_aheu0fnnjfuviymj.getBody(editor), 'table[data-mce-id="__mce"]').map(function (table) {
      if (isPixelsForced(editor)) {
        $_2lr8nrlejfuviy40.set(table, 'width', $_2lr8nrlejfuviy40.get(table, 'width'));
      }
      $_2ekobel5jfuviy2m.remove(table, 'data-mce-id');
      fireEvents(editor, table);
      selectFirstCellInTable(editor, table);
      return table.dom();
    }).getOr(null);
  };
  var $_53bmjpo1jfuviyoi = { insert: insert$1 };

  function styleTDTH(dom, elm, name, value) {
    if (elm.tagName === 'TD' || elm.tagName === 'TH') {
      dom.setStyle(elm, name, value);
    } else {
      if (elm.children) {
        for (var i = 0; i < elm.children.length; i++) {
          styleTDTH(dom, elm.children[i], name, value);
        }
      }
    }
  }
  var extractDataFromElement$2 = function (editor, tableElm) {
    var dom = editor.dom;
    var data = {
      width: dom.getStyle(tableElm, 'width') || dom.getAttrib(tableElm, 'width'),
      height: dom.getStyle(tableElm, 'height') || dom.getAttrib(tableElm, 'height'),
      cellspacing: dom.getStyle(tableElm, 'border-spacing') || dom.getAttrib(tableElm, 'cellspacing'),
      cellpadding: dom.getAttrib(tableElm, 'data-mce-cell-padding') || dom.getAttrib(tableElm, 'cellpadding') || $_35zgp3nwjfuviynz.getTDTHOverallStyle(editor.dom, tableElm, 'padding'),
      border: dom.getAttrib(tableElm, 'data-mce-border') || dom.getAttrib(tableElm, 'border') || $_35zgp3nwjfuviynz.getTDTHOverallStyle(editor.dom, tableElm, 'border'),
      borderColor: dom.getAttrib(tableElm, 'data-mce-border-color'),
      caption: !!dom.select('caption', tableElm)[0],
      class: dom.getAttrib(tableElm, 'class')
    };
    global$2.each('left center right'.split(' '), function (name) {
      if (editor.formatter.matchNode(tableElm, 'align' + name)) {
        data.align = name;
      }
    });
    if (hasAdvancedTableTab(editor)) {
      global$2.extend(data, $_etnm2gnxjfuviyo0.extractAdvancedStyles(dom, tableElm));
    }
    return data;
  };
  var applyDataToElement = function (editor, tableElm, data) {
    var dom = editor.dom;
    var attrs = {};
    var styles = {};
    attrs.class = data.class;
    styles.height = $_aheu0fnnjfuviymj.addSizeSuffix(data.height);
    if (dom.getAttrib(tableElm, 'width') && !shouldStyleWithCss(editor)) {
      attrs.width = $_aheu0fnnjfuviymj.removePxSuffix(data.width);
    } else {
      styles.width = $_aheu0fnnjfuviymj.addSizeSuffix(data.width);
    }
    if (shouldStyleWithCss(editor)) {
      styles['border-width'] = $_aheu0fnnjfuviymj.addSizeSuffix(data.border);
      styles['border-spacing'] = $_aheu0fnnjfuviymj.addSizeSuffix(data.cellspacing);
      global$2.extend(attrs, {
        'data-mce-border-color': data.borderColor,
        'data-mce-cell-padding': data.cellpadding,
        'data-mce-border': data.border
      });
    } else {
      global$2.extend(attrs, {
        border: data.border,
        cellpadding: data.cellpadding,
        cellspacing: data.cellspacing
      });
    }
    if (shouldStyleWithCss(editor)) {
      if (tableElm.children) {
        for (var i = 0; i < tableElm.children.length; i++) {
          styleTDTH(dom, tableElm.children[i], {
            'border-width': $_aheu0fnnjfuviymj.addSizeSuffix(data.border),
            'border-color': data.borderColor,
            'padding': $_aheu0fnnjfuviymj.addSizeSuffix(data.cellpadding)
          });
        }
      }
    }
    if (data.style) {
      global$2.extend(styles, dom.parseStyle(data.style));
    } else {
      styles = global$2.extend({}, dom.parseStyle(dom.getAttrib(tableElm, 'style')), styles);
    }
    attrs.style = dom.serializeStyle(styles);
    dom.setAttribs(tableElm, attrs);
  };
  var onSubmitTableForm = function (editor, tableElm, evt) {
    var dom = editor.dom;
    var captionElm;
    var data;
    $_etnm2gnxjfuviyo0.updateStyleField(editor, evt);
    data = evt.control.rootControl.toJSON();
    if (data.class === false) {
      delete data.class;
    }
    editor.undoManager.transact(function () {
      if (!tableElm) {
        tableElm = $_53bmjpo1jfuviyoi.insert(editor, data.cols || 1, data.rows || 1);
      }
      applyDataToElement(editor, tableElm, data);
      captionElm = dom.select('caption', tableElm)[0];
      if (captionElm && !data.caption) {
        dom.remove(captionElm);
      }
      if (!captionElm && data.caption) {
        captionElm = dom.create('caption');
        captionElm.innerHTML = !global$3.ie ? '<br data-mce-bogus="1"/>' : '\xA0';
        tableElm.insertBefore(captionElm, tableElm.firstChild);
      }
      $_35zgp3nwjfuviynz.unApplyAlign(editor, tableElm);
      if (data.align) {
        $_35zgp3nwjfuviynz.applyAlign(editor, tableElm, data.align);
      }
      editor.focus();
      editor.addVisual();
    });
  };
  var open$2 = function (editor, isProps) {
    var dom = editor.dom;
    var tableElm, colsCtrl, rowsCtrl, classListCtrl, data = {}, generalTableForm;
    if (isProps === true) {
      tableElm = dom.getParent(editor.selection.getStart(), 'table');
      if (tableElm) {
        data = extractDataFromElement$2(editor, tableElm);
      }
    } else {
      colsCtrl = {
        label: 'Cols',
        name: 'cols'
      };
      rowsCtrl = {
        label: 'Rows',
        name: 'rows'
      };
    }
    if (getTableClassList(editor).length > 0) {
      if (data.class) {
        data.class = data.class.replace(/\s*mce\-item\-table\s*/g, '');
      }
      classListCtrl = {
        name: 'class',
        type: 'listbox',
        label: 'Class',
        values: $_etnm2gnxjfuviyo0.buildListItems(getTableClassList(editor), function (item) {
          if (item.value) {
            item.textStyle = function () {
              return editor.formatter.getCssText({
                block: 'table',
                classes: [item.value]
              });
            };
          }
        })
      };
    }
    generalTableForm = {
      type: 'form',
      layout: 'flex',
      direction: 'column',
      labelGapCalc: 'children',
      padding: 0,
      items: [
        {
          type: 'form',
          labelGapCalc: false,
          padding: 0,
          layout: 'grid',
          columns: 2,
          defaults: {
            type: 'textbox',
            maxWidth: 50
          },
          items: hasAppearanceOptions(editor) ? [
            colsCtrl,
            rowsCtrl,
            {
              label: 'Width',
              name: 'width',
              onchange: $_fdch7uk7jfuvixxb.curry($_etnm2gnxjfuviyo0.updateStyleField, editor)
            },
            {
              label: 'Height',
              name: 'height',
              onchange: $_fdch7uk7jfuvixxb.curry($_etnm2gnxjfuviyo0.updateStyleField, editor)
            },
            {
              label: 'Cell spacing',
              name: 'cellspacing'
            },
            {
              label: 'Cell padding',
              name: 'cellpadding'
            },
            {
              label: 'Border',
              name: 'border'
            },
            {
              label: 'Caption',
              name: 'caption',
              type: 'checkbox'
            }
          ] : [
            colsCtrl,
            rowsCtrl,
            {
              label: 'Width',
              name: 'width',
              onchange: $_fdch7uk7jfuvixxb.curry($_etnm2gnxjfuviyo0.updateStyleField, editor)
            },
            {
              label: 'Height',
              name: 'height',
              onchange: $_fdch7uk7jfuvixxb.curry($_etnm2gnxjfuviyo0.updateStyleField, editor)
            }
          ]
        },
        {
          label: 'Alignment',
          name: 'align',
          type: 'listbox',
          text: 'None',
          values: [
            {
              text: 'None',
              value: ''
            },
            {
              text: 'Left',
              value: 'left'
            },
            {
              text: 'Center',
              value: 'center'
            },
            {
              text: 'Right',
              value: 'right'
            }
          ]
        },
        classListCtrl
      ]
    };
    if (hasAdvancedTableTab(editor)) {
      editor.windowManager.open({
        title: 'Table properties',
        data: data,
        bodyType: 'tabpanel',
        body: [
          {
            title: 'General',
            type: 'form',
            items: generalTableForm
          },
          $_etnm2gnxjfuviyo0.createStyleForm(editor)
        ],
        onsubmit: $_fdch7uk7jfuvixxb.curry(onSubmitTableForm, editor, tableElm)
      });
    } else {
      editor.windowManager.open({
        title: 'Table properties',
        data: data,
        body: generalTableForm,
        onsubmit: $_fdch7uk7jfuvixxb.curry(onSubmitTableForm, editor, tableElm)
      });
    }
  };
  var $_53k9g0nzjfuviyoc = { open: open$2 };

  var each$3 = global$2.each;
  var registerCommands = function (editor, actions, cellSelection, selections, clipboardRows) {
    var isRoot = $_aheu0fnnjfuviymj.getIsRoot(editor);
    var eraseTable = function () {
      var cell = $_4sdhm4kkjfuviy0e.fromDom(editor.dom.getParent(editor.selection.getStart(), 'th,td'));
      var table = $_dmqxswkhjfuvixyz.table(cell, isRoot);
      table.filter($_fdch7uk7jfuvixxb.not(isRoot)).each(function (table) {
        var cursor = $_4sdhm4kkjfuviy0e.fromText('');
        $_5zcsfmlgjfuviy4g.after(table, cursor);
        $_5ud3colhjfuviy4l.remove(table);
        var rng = editor.dom.createRng();
        rng.setStart(cursor.dom(), 0);
        rng.setEnd(cursor.dom(), 0);
        editor.selection.setRng(rng);
      });
    };
    var getSelectionStartCell = function () {
      return $_4sdhm4kkjfuviy0e.fromDom(editor.dom.getParent(editor.selection.getStart(), 'th,td'));
    };
    var getTableFromCell = function (cell) {
      return $_dmqxswkhjfuvixyz.table(cell, isRoot);
    };
    var actOnSelection = function (execute) {
      var cell = getSelectionStartCell();
      var table = getTableFromCell(cell);
      table.each(function (table) {
        var targets = $_fgxiq3lqjfuviy6a.forMenu(selections, table, cell);
        execute(table, targets).each(function (rng) {
          editor.selection.setRng(rng);
          editor.focus();
          cellSelection.clear(table);
        });
      });
    };
    var copyRowSelection = function (execute) {
      var cell = getSelectionStartCell();
      var table = getTableFromCell(cell);
      return table.bind(function (table) {
        var doc = $_4sdhm4kkjfuviy0e.fromDom(editor.getDoc());
        var targets = $_fgxiq3lqjfuviy6a.forMenu(selections, table, cell);
        var generators = $_clky4ljjfuviy4t.cellOperations($_fdch7uk7jfuvixxb.noop, doc, Option.none());
        return $_bjdegsntjfuviynl.copyRows(table, targets, generators);
      });
    };
    var pasteOnSelection = function (execute) {
      clipboardRows.get().each(function (rows) {
        var clonedRows = $_4jja6kk5jfuvixx1.map(rows, function (row) {
          return $_1i39h2lkjfuviy5n.deep(row);
        });
        var cell = getSelectionStartCell();
        var table = getTableFromCell(cell);
        table.bind(function (table) {
          var doc = $_4sdhm4kkjfuviy0e.fromDom(editor.getDoc());
          var generators = $_clky4ljjfuviy4t.paste(doc);
          var targets = $_fgxiq3lqjfuviy6a.pasteRows(selections, table, cell, clonedRows, generators);
          execute(table, targets).each(function (rng) {
            editor.selection.setRng(rng);
            editor.focus();
            cellSelection.clear(table);
          });
        });
      });
    };
    each$3({
      mceTableSplitCells: function () {
        actOnSelection(actions.unmergeCells);
      },
      mceTableMergeCells: function () {
        actOnSelection(actions.mergeCells);
      },
      mceTableInsertRowBefore: function () {
        actOnSelection(actions.insertRowsBefore);
      },
      mceTableInsertRowAfter: function () {
        actOnSelection(actions.insertRowsAfter);
      },
      mceTableInsertColBefore: function () {
        actOnSelection(actions.insertColumnsBefore);
      },
      mceTableInsertColAfter: function () {
        actOnSelection(actions.insertColumnsAfter);
      },
      mceTableDeleteCol: function () {
        actOnSelection(actions.deleteColumn);
      },
      mceTableDeleteRow: function () {
        actOnSelection(actions.deleteRow);
      },
      mceTableCutRow: function (grid) {
        clipboardRows.set(copyRowSelection());
        actOnSelection(actions.deleteRow);
      },
      mceTableCopyRow: function (grid) {
        clipboardRows.set(copyRowSelection());
      },
      mceTablePasteRowBefore: function (grid) {
        pasteOnSelection(actions.pasteRowsBefore);
      },
      mceTablePasteRowAfter: function (grid) {
        pasteOnSelection(actions.pasteRowsAfter);
      },
      mceTableDelete: eraseTable
    }, function (func, name) {
      editor.addCommand(name, func);
    });
    each$3({
      mceInsertTable: $_fdch7uk7jfuvixxb.curry($_53k9g0nzjfuviyoc.open, editor),
      mceTableProps: $_fdch7uk7jfuvixxb.curry($_53k9g0nzjfuviyoc.open, editor, true),
      mceTableRowProps: $_fdch7uk7jfuvixxb.curry($_9342zrnyjfuviyo6.open, editor),
      mceTableCellProps: $_fdch7uk7jfuvixxb.curry($_ff2aywnvjfuviynt.open, editor)
    }, function (func, name) {
      editor.addCommand(name, function (ui, val) {
        func(val);
      });
    });
  };
  var $_5hhd4unsjfuviyn2 = { registerCommands: registerCommands };

  var only$1 = function (element) {
    var parent = Option.from(element.dom().documentElement).map($_4sdhm4kkjfuviy0e.fromDom).getOr(element);
    return {
      parent: $_fdch7uk7jfuvixxb.constant(parent),
      view: $_fdch7uk7jfuvixxb.constant(element),
      origin: $_fdch7uk7jfuvixxb.constant(r(0, 0))
    };
  };
  var detached = function (editable, chrome) {
    var origin = $_fdch7uk7jfuvixxb.curry($_6ti42xmijfuviydp.absolute, chrome);
    return {
      parent: $_fdch7uk7jfuvixxb.constant(chrome),
      view: $_fdch7uk7jfuvixxb.constant(editable),
      origin: origin
    };
  };
  var body$1 = function (editable, chrome) {
    return {
      parent: $_fdch7uk7jfuvixxb.constant(chrome),
      view: $_fdch7uk7jfuvixxb.constant(editable),
      origin: $_fdch7uk7jfuvixxb.constant(r(0, 0))
    };
  };
  var $_ar0shqo6jfuviyq0 = {
    only: only$1,
    detached: detached,
    body: body$1
  };

  function Event (fields) {
    var struct = $_96oqrskbjfuvixya.immutable.apply(null, fields);
    var handlers = [];
    var bind = function (handler) {
      if (handler === undefined) {
        throw 'Event bind error: undefined handler';
      }
      handlers.push(handler);
    };
    var unbind = function (handler) {
      handlers = $_4jja6kk5jfuvixx1.filter(handlers, function (h) {
        return h !== handler;
      });
    };
    var trigger = function () {
      var event = struct.apply(null, arguments);
      $_4jja6kk5jfuvixx1.each(handlers, function (handler) {
        handler(event);
      });
    };
    return {
      bind: bind,
      unbind: unbind,
      trigger: trigger
    };
  }

  var create = function (typeDefs) {
    var registry = $_afb9m6kajfuvixy8.map(typeDefs, function (event) {
      return {
        bind: event.bind,
        unbind: event.unbind
      };
    });
    var trigger = $_afb9m6kajfuvixy8.map(typeDefs, function (event) {
      return event.trigger;
    });
    return {
      registry: registry,
      trigger: trigger
    };
  };
  var $_6yrn63o9jfuviyqu = { create: create };

  var mode = $_5ebbzmpjfuviyf6.exactly([
    'compare',
    'extract',
    'mutate',
    'sink'
  ]);
  var sink = $_5ebbzmpjfuviyf6.exactly([
    'element',
    'start',
    'stop',
    'destroy'
  ]);
  var api$3 = $_5ebbzmpjfuviyf6.exactly([
    'forceDrop',
    'drop',
    'move',
    'delayDrop'
  ]);
  var $_gabrxvodjfuviysh = {
    mode: mode,
    sink: sink,
    api: api$3
  };

  var styles$1 = $_13iai7n5jfuviyjr.css('ephox-dragster');
  var $_3hw8i9ofjfuviysz = { resolve: styles$1.resolve };

  function Blocker (options) {
    var settings = $_8d366dmujfuviygv.merge({ 'layerClass': $_3hw8i9ofjfuviysz.resolve('blocker') }, options);
    var div = $_4sdhm4kkjfuviy0e.fromTag('div');
    $_2ekobel5jfuviy2m.set(div, 'role', 'presentation');
    $_2lr8nrlejfuviy40.setAll(div, {
      position: 'fixed',
      left: '0px',
      top: '0px',
      width: '100%',
      height: '100%'
    });
    $_c1zp6in6jfuviyjs.add(div, $_3hw8i9ofjfuviysz.resolve('blocker'));
    $_c1zp6in6jfuviyjs.add(div, settings.layerClass);
    var element = function () {
      return div;
    };
    var destroy = function () {
      $_5ud3colhjfuviy4l.remove(div);
    };
    return {
      element: element,
      destroy: destroy
    };
  }

  var mkEvent = function (target, x, y, stop, prevent, kill, raw) {
    return {
      'target': $_fdch7uk7jfuvixxb.constant(target),
      'x': $_fdch7uk7jfuvixxb.constant(x),
      'y': $_fdch7uk7jfuvixxb.constant(y),
      'stop': stop,
      'prevent': prevent,
      'kill': kill,
      'raw': $_fdch7uk7jfuvixxb.constant(raw)
    };
  };
  var handle = function (filter, handler) {
    return function (rawEvent) {
      if (!filter(rawEvent))
        return;
      var target = $_4sdhm4kkjfuviy0e.fromDom(rawEvent.target);
      var stop = function () {
        rawEvent.stopPropagation();
      };
      var prevent = function () {
        rawEvent.preventDefault();
      };
      var kill = $_fdch7uk7jfuvixxb.compose(prevent, stop);
      var evt = mkEvent(target, rawEvent.clientX, rawEvent.clientY, stop, prevent, kill, rawEvent);
      handler(evt);
    };
  };
  var binder = function (element, event, filter, handler, useCapture) {
    var wrapped = handle(filter, handler);
    element.dom().addEventListener(event, wrapped, useCapture);
    return { unbind: $_fdch7uk7jfuvixxb.curry(unbind, element, event, wrapped, useCapture) };
  };
  var bind$1 = function (element, event, filter, handler) {
    return binder(element, event, filter, handler, false);
  };
  var capture = function (element, event, filter, handler) {
    return binder(element, event, filter, handler, true);
  };
  var unbind = function (element, event, handler, useCapture) {
    element.dom().removeEventListener(event, handler, useCapture);
  };
  var $_dn0nleohjfuviyt7 = {
    bind: bind$1,
    capture: capture
  };

  var filter$1 = $_fdch7uk7jfuvixxb.constant(true);
  var bind$2 = function (element, event, handler) {
    return $_dn0nleohjfuviyt7.bind(element, event, filter$1, handler);
  };
  var capture$1 = function (element, event, handler) {
    return $_dn0nleohjfuviyt7.capture(element, event, filter$1, handler);
  };
  var $_2t4o5uogjfuviyt3 = {
    bind: bind$2,
    capture: capture$1
  };

  var compare = function (old, nu) {
    return r(nu.left() - old.left(), nu.top() - old.top());
  };
  var extract$1 = function (event) {
    return Option.some(r(event.x(), event.y()));
  };
  var mutate$1 = function (mutation, info) {
    mutation.mutate(info.left(), info.top());
  };
  var sink$1 = function (dragApi, settings) {
    var blocker = Blocker(settings);
    var mdown = $_2t4o5uogjfuviyt3.bind(blocker.element(), 'mousedown', dragApi.forceDrop);
    var mup = $_2t4o5uogjfuviyt3.bind(blocker.element(), 'mouseup', dragApi.drop);
    var mmove = $_2t4o5uogjfuviyt3.bind(blocker.element(), 'mousemove', dragApi.move);
    var mout = $_2t4o5uogjfuviyt3.bind(blocker.element(), 'mouseout', dragApi.delayDrop);
    var destroy = function () {
      blocker.destroy();
      mup.unbind();
      mmove.unbind();
      mout.unbind();
      mdown.unbind();
    };
    var start = function (parent) {
      $_5zcsfmlgjfuviy4g.append(parent, blocker.element());
    };
    var stop = function () {
      $_5ud3colhjfuviy4l.remove(blocker.element());
    };
    return $_gabrxvodjfuviysh.sink({
      element: blocker.element,
      start: start,
      stop: stop,
      destroy: destroy
    });
  };
  var MouseDrag = $_gabrxvodjfuviysh.mode({
    compare: compare,
    extract: extract$1,
    sink: sink$1,
    mutate: mutate$1
  });

  function InDrag () {
    var previous = Option.none();
    var reset = function () {
      previous = Option.none();
    };
    var update = function (mode, nu) {
      var result = previous.map(function (old) {
        return mode.compare(old, nu);
      });
      previous = Option.some(nu);
      return result;
    };
    var onEvent = function (event, mode) {
      var dataOption = mode.extract(event);
      dataOption.each(function (data) {
        var offset = update(mode, data);
        offset.each(function (d) {
          events.trigger.move(d);
        });
      });
    };
    var events = $_6yrn63o9jfuviyqu.create({ move: Event(['info']) });
    return {
      onEvent: onEvent,
      reset: reset,
      events: events.registry
    };
  }

  function NoDrag (anchor) {
    var onEvent = function (event, mode) {
    };
    return {
      onEvent: onEvent,
      reset: $_fdch7uk7jfuvixxb.noop
    };
  }

  function Movement () {
    var noDragState = NoDrag();
    var inDragState = InDrag();
    var dragState = noDragState;
    var on = function () {
      dragState.reset();
      dragState = inDragState;
    };
    var off = function () {
      dragState.reset();
      dragState = noDragState;
    };
    var onEvent = function (event, mode) {
      dragState.onEvent(event, mode);
    };
    var isOn = function () {
      return dragState === inDragState;
    };
    return {
      on: on,
      off: off,
      isOn: isOn,
      onEvent: onEvent,
      events: inDragState.events
    };
  }

  var adaptable = function (fn, rate) {
    var timer = null;
    var args = null;
    var cancel = function () {
      if (timer !== null) {
        clearTimeout(timer);
        timer = null;
        args = null;
      }
    };
    var throttle = function () {
      args = arguments;
      if (timer === null) {
        timer = setTimeout(function () {
          fn.apply(null, args);
          timer = null;
          args = null;
        }, rate);
      }
    };
    return {
      cancel: cancel,
      throttle: throttle
    };
  };
  var first$4 = function (fn, rate) {
    var timer = null;
    var cancel = function () {
      if (timer !== null) {
        clearTimeout(timer);
        timer = null;
      }
    };
    var throttle = function () {
      var args = arguments;
      if (timer === null) {
        timer = setTimeout(function () {
          fn.apply(null, args);
          timer = null;
          args = null;
        }, rate);
      }
    };
    return {
      cancel: cancel,
      throttle: throttle
    };
  };
  var last$3 = function (fn, rate) {
    var timer = null;
    var cancel = function () {
      if (timer !== null) {
        clearTimeout(timer);
        timer = null;
      }
    };
    var throttle = function () {
      var args = arguments;
      if (timer !== null)
        clearTimeout(timer);
      timer = setTimeout(function () {
        fn.apply(null, args);
        timer = null;
        args = null;
      }, rate);
    };
    return {
      cancel: cancel,
      throttle: throttle
    };
  };
  var $_bjcpcmomjfuviyu4 = {
    adaptable: adaptable,
    first: first$4,
    last: last$3
  };

  var setup = function (mutation, mode, settings) {
    var active = false;
    var events = $_6yrn63o9jfuviyqu.create({
      start: Event([]),
      stop: Event([])
    });
    var movement = Movement();
    var drop = function () {
      sink.stop();
      if (movement.isOn()) {
        movement.off();
        events.trigger.stop();
      }
    };
    var throttledDrop = $_bjcpcmomjfuviyu4.last(drop, 200);
    var go = function (parent) {
      sink.start(parent);
      movement.on();
      events.trigger.start();
    };
    var mousemove = function (event, ui) {
      throttledDrop.cancel();
      movement.onEvent(event, mode);
    };
    movement.events.move.bind(function (event) {
      mode.mutate(mutation, event.info());
    });
    var on = function () {
      active = true;
    };
    var off = function () {
      active = false;
    };
    var runIfActive = function (f) {
      return function () {
        var args = Array.prototype.slice.call(arguments, 0);
        if (active) {
          return f.apply(null, args);
        }
      };
    };
    var sink = mode.sink($_gabrxvodjfuviysh.api({
      forceDrop: drop,
      drop: runIfActive(drop),
      move: runIfActive(mousemove),
      delayDrop: runIfActive(throttledDrop.throttle)
    }), settings);
    var destroy = function () {
      sink.destroy();
    };
    return {
      element: sink.element,
      go: go,
      on: on,
      off: off,
      destroy: destroy,
      events: events.registry
    };
  };
  var $_3eefdkoijfuviytb = { setup: setup };

  var transform$1 = function (mutation, options) {
    var settings = options !== undefined ? options : {};
    var mode = settings.mode !== undefined ? settings.mode : MouseDrag;
    return $_3eefdkoijfuviytb.setup(mutation, mode, options);
  };
  var $_al5begobjfuviys2 = { transform: transform$1 };

  function Mutation () {
    var events = $_6yrn63o9jfuviyqu.create({
      'drag': Event([
        'xDelta',
        'yDelta'
      ])
    });
    var mutate = function (x, y) {
      events.trigger.drag(x, y);
    };
    return {
      mutate: mutate,
      events: events.registry
    };
  }

  function BarMutation () {
    var events = $_6yrn63o9jfuviyqu.create({
      drag: Event([
        'xDelta',
        'yDelta',
        'target'
      ])
    });
    var target = Option.none();
    var delegate = Mutation();
    delegate.events.drag.bind(function (event) {
      target.each(function (t) {
        events.trigger.drag(event.xDelta(), event.yDelta(), t);
      });
    });
    var assign = function (t) {
      target = Option.some(t);
    };
    var get = function () {
      return target;
    };
    return {
      assign: assign,
      get: get,
      mutate: delegate.mutate,
      events: events.registry
    };
  }

  var any = function (selector) {
    return $_26gnp6lajfuviy35.first(selector).isSome();
  };
  var ancestor$2 = function (scope, selector, isRoot) {
    return $_26gnp6lajfuviy35.ancestor(scope, selector, isRoot).isSome();
  };
  var sibling$2 = function (scope, selector) {
    return $_26gnp6lajfuviy35.sibling(scope, selector).isSome();
  };
  var child$3 = function (scope, selector) {
    return $_26gnp6lajfuviy35.child(scope, selector).isSome();
  };
  var descendant$2 = function (scope, selector) {
    return $_26gnp6lajfuviy35.descendant(scope, selector).isSome();
  };
  var closest$2 = function (scope, selector, isRoot) {
    return $_26gnp6lajfuviy35.closest(scope, selector, isRoot).isSome();
  };
  var $_a72h2qopjfuviyyz = {
    any: any,
    ancestor: ancestor$2,
    sibling: sibling$2,
    child: child$3,
    descendant: descendant$2,
    closest: closest$2
  };

  var resizeBarDragging = $_a98vnwn4jfuviyjo.resolve('resizer-bar-dragging');
  function BarManager (wire, direction, hdirection) {
    var mutation = BarMutation();
    var resizing = $_al5begobjfuviys2.transform(mutation, {});
    var hoverTable = Option.none();
    var getResizer = function (element, type) {
      return Option.from($_2ekobel5jfuviy2m.get(element, type));
    };
    mutation.events.drag.bind(function (event) {
      getResizer(event.target(), 'data-row').each(function (_dataRow) {
        var currentRow = $_edgkrmnkjfuviylu.getInt(event.target(), 'top');
        $_2lr8nrlejfuviy40.set(event.target(), 'top', currentRow + event.yDelta() + 'px');
      });
      getResizer(event.target(), 'data-column').each(function (_dataCol) {
        var currentCol = $_edgkrmnkjfuviylu.getInt(event.target(), 'left');
        $_2lr8nrlejfuviy40.set(event.target(), 'left', currentCol + event.xDelta() + 'px');
      });
    });
    var getDelta = function (target, direction) {
      var newX = $_edgkrmnkjfuviylu.getInt(target, direction);
      var oldX = parseInt($_2ekobel5jfuviy2m.get(target, 'data-initial-' + direction), 10);
      return newX - oldX;
    };
    resizing.events.stop.bind(function () {
      mutation.get().each(function (target) {
        hoverTable.each(function (table) {
          getResizer(target, 'data-row').each(function (row) {
            var delta = getDelta(target, 'top');
            $_2ekobel5jfuviy2m.remove(target, 'data-initial-top');
            events.trigger.adjustHeight(table, delta, parseInt(row, 10));
          });
          getResizer(target, 'data-column').each(function (column) {
            var delta = getDelta(target, 'left');
            $_2ekobel5jfuviy2m.remove(target, 'data-initial-left');
            events.trigger.adjustWidth(table, delta, parseInt(column, 10));
          });
          $_8qdnrkn0jfuviyi7.refresh(wire, table, hdirection, direction);
        });
      });
    });
    var handler = function (target, direction) {
      events.trigger.startAdjust();
      mutation.assign(target);
      $_2ekobel5jfuviy2m.set(target, 'data-initial-' + direction, parseInt($_2lr8nrlejfuviy40.get(target, direction), 10));
      $_c1zp6in6jfuviyjs.add(target, resizeBarDragging);
      $_2lr8nrlejfuviy40.set(target, 'opacity', '0.2');
      resizing.go(wire.parent());
    };
    var mousedown = $_2t4o5uogjfuviyt3.bind(wire.parent(), 'mousedown', function (event) {
      if ($_8qdnrkn0jfuviyi7.isRowBar(event.target()))
        handler(event.target(), 'top');
      if ($_8qdnrkn0jfuviyi7.isColBar(event.target()))
        handler(event.target(), 'left');
    });
    var isRoot = function (e) {
      return $_g6ztqikojfuviy13.eq(e, wire.view());
    };
    var mouseover = $_2t4o5uogjfuviyt3.bind(wire.view(), 'mouseover', function (event) {
      if ($_6mcqmml6jfuviy2u.name(event.target()) === 'table' || $_a72h2qopjfuviyyz.closest(event.target(), 'table', isRoot)) {
        hoverTable = $_6mcqmml6jfuviy2u.name(event.target()) === 'table' ? Option.some(event.target()) : $_26gnp6lajfuviy35.ancestor(event.target(), 'table', isRoot);
        hoverTable.each(function (ht) {
          $_8qdnrkn0jfuviyi7.refresh(wire, ht, hdirection, direction);
        });
      } else if ($_43dxxcl9jfuviy31.inBody(event.target())) {
        $_8qdnrkn0jfuviyi7.destroy(wire);
      }
    });
    var destroy = function () {
      mousedown.unbind();
      mouseover.unbind();
      resizing.destroy();
      $_8qdnrkn0jfuviyi7.destroy(wire);
    };
    var refresh = function (tbl) {
      $_8qdnrkn0jfuviyi7.refresh(wire, tbl, hdirection, direction);
    };
    var events = $_6yrn63o9jfuviyqu.create({
      adjustHeight: Event([
        'table',
        'delta',
        'row'
      ]),
      adjustWidth: Event([
        'table',
        'delta',
        'column'
      ]),
      startAdjust: Event([])
    });
    return {
      destroy: destroy,
      refresh: refresh,
      on: resizing.on,
      off: resizing.off,
      hideBars: $_fdch7uk7jfuvixxb.curry($_8qdnrkn0jfuviyi7.hide, wire),
      showBars: $_fdch7uk7jfuvixxb.curry($_8qdnrkn0jfuviyi7.show, wire),
      events: events.registry
    };
  }

  function TableResize (wire, vdirection) {
    var hdirection = $_bj1b3kmhjfuviyd6.height;
    var manager = BarManager(wire, vdirection, hdirection);
    var events = $_6yrn63o9jfuviyqu.create({
      beforeResize: Event(['table']),
      afterResize: Event(['table']),
      startDrag: Event([])
    });
    manager.events.adjustHeight.bind(function (event) {
      events.trigger.beforeResize(event.table());
      var delta = hdirection.delta(event.delta(), event.table());
      $_f6vpt3ngjfuviyl3.adjustHeight(event.table(), delta, event.row(), hdirection);
      events.trigger.afterResize(event.table());
    });
    manager.events.startAdjust.bind(function (event) {
      events.trigger.startDrag();
    });
    manager.events.adjustWidth.bind(function (event) {
      events.trigger.beforeResize(event.table());
      var delta = vdirection.delta(event.delta(), event.table());
      $_f6vpt3ngjfuviyl3.adjustWidth(event.table(), delta, event.column(), vdirection);
      events.trigger.afterResize(event.table());
    });
    return {
      on: manager.on,
      off: manager.off,
      hideBars: manager.hideBars,
      showBars: manager.showBars,
      destroy: manager.destroy,
      events: events.registry
    };
  }

  var createContainer = function () {
    var container = $_4sdhm4kkjfuviy0e.fromTag('div');
    $_2lr8nrlejfuviy40.setAll(container, {
      position: 'static',
      height: '0',
      width: '0',
      padding: '0',
      margin: '0',
      border: '0'
    });
    $_5zcsfmlgjfuviy4g.append($_43dxxcl9jfuviy31.body(), container);
    return container;
  };
  var get$8 = function (editor, container) {
    return editor.inline ? $_ar0shqo6jfuviyq0.body($_aheu0fnnjfuviymj.getBody(editor), createContainer()) : $_ar0shqo6jfuviyq0.only($_4sdhm4kkjfuviy0e.fromDom(editor.getDoc()));
  };
  var remove$6 = function (editor, wire) {
    if (editor.inline) {
      $_5ud3colhjfuviy4l.remove(wire.parent());
    }
  };
  var $_88ebgxoqjfuviyz1 = {
    get: get$8,
    remove: remove$6
  };

  var ResizeHandler = function (editor) {
    var selectionRng = Option.none();
    var resize = Option.none();
    var wire = Option.none();
    var percentageBasedSizeRegex = /(\d+(\.\d+)?)%/;
    var startW, startRawW;
    var isTable = function (elm) {
      return elm.nodeName === 'TABLE';
    };
    var getRawWidth = function (elm) {
      return editor.dom.getStyle(elm, 'width') || editor.dom.getAttrib(elm, 'width');
    };
    var lazyResize = function () {
      return resize;
    };
    var lazyWire = function () {
      return wire.getOr($_ar0shqo6jfuviyq0.only($_4sdhm4kkjfuviy0e.fromDom(editor.getBody())));
    };
    var destroy = function () {
      resize.each(function (sz) {
        sz.destroy();
      });
      wire.each(function (w) {
        $_88ebgxoqjfuviyz1.remove(editor, w);
      });
    };
    editor.on('init', function () {
      var direction = TableDirection($_1g25rnojfuviymn.directionAt);
      var rawWire = $_88ebgxoqjfuviyz1.get(editor);
      wire = Option.some(rawWire);
      if (hasObjectResizing(editor) && hasTableResizeBars(editor)) {
        var sz = TableResize(rawWire, direction);
        sz.on();
        sz.events.startDrag.bind(function (event) {
          selectionRng = Option.some(editor.selection.getRng());
        });
        sz.events.afterResize.bind(function (event) {
          var table = event.table();
          var dataStyleCells = $_a3hs1bl7jfuviy2w.descendants(table, 'td[data-mce-style],th[data-mce-style]');
          $_4jja6kk5jfuvixx1.each(dataStyleCells, function (cell) {
            $_2ekobel5jfuviy2m.remove(cell, 'data-mce-style');
          });
          selectionRng.each(function (rng) {
            editor.selection.setRng(rng);
            editor.focus();
          });
          editor.undoManager.add();
        });
        resize = Option.some(sz);
      }
    });
    editor.on('ObjectResizeStart', function (e) {
      var targetElm = e.target;
      if (isTable(targetElm)) {
        startW = e.width;
        startRawW = getRawWidth(targetElm);
      }
    });
    editor.on('ObjectResized', function (e) {
      var targetElm = e.target;
      if (isTable(targetElm)) {
        var table = targetElm;
        if (percentageBasedSizeRegex.test(startRawW)) {
          var percentW = parseFloat(percentageBasedSizeRegex.exec(startRawW)[1]);
          var targetPercentW = e.width * percentW / startW;
          editor.dom.setStyle(table, 'width', targetPercentW + '%');
        } else {
          var newCellSizes_1 = [];
          global$2.each(table.rows, function (row) {
            global$2.each(row.cells, function (cell) {
              var width = editor.dom.getStyle(cell, 'width', true);
              newCellSizes_1.push({
                cell: cell,
                width: width
              });
            });
          });
          global$2.each(newCellSizes_1, function (newCellSize) {
            editor.dom.setStyle(newCellSize.cell, 'width', newCellSize.width);
            editor.dom.setAttrib(newCellSize.cell, 'width', null);
          });
        }
      }
    });
    return {
      lazyResize: lazyResize,
      lazyWire: lazyWire,
      destroy: destroy
    };
  };

  var none$2 = function (current) {
    return folder$1(function (n, f, m, l) {
      return n(current);
    });
  };
  var first$5 = function (current) {
    return folder$1(function (n, f, m, l) {
      return f(current);
    });
  };
  var middle$1 = function (current, target) {
    return folder$1(function (n, f, m, l) {
      return m(current, target);
    });
  };
  var last$4 = function (current) {
    return folder$1(function (n, f, m, l) {
      return l(current);
    });
  };
  var folder$1 = function (fold) {
    return { fold: fold };
  };
  var $_5ee2qxotjfuviz05 = {
    none: none$2,
    first: first$5,
    middle: middle$1,
    last: last$4
  };

  var detect$4 = function (current, isRoot) {
    return $_dmqxswkhjfuvixyz.table(current, isRoot).bind(function (table) {
      var all = $_dmqxswkhjfuvixyz.cells(table);
      var index = $_4jja6kk5jfuvixx1.findIndex(all, function (x) {
        return $_g6ztqikojfuviy13.eq(current, x);
      });
      return index.map(function (ind) {
        return {
          index: $_fdch7uk7jfuvixxb.constant(ind),
          all: $_fdch7uk7jfuvixxb.constant(all)
        };
      });
    });
  };
  var next = function (current, isRoot) {
    var detection = detect$4(current, isRoot);
    return detection.fold(function () {
      return $_5ee2qxotjfuviz05.none(current);
    }, function (info) {
      return info.index() + 1 < info.all().length ? $_5ee2qxotjfuviz05.middle(current, info.all()[info.index() + 1]) : $_5ee2qxotjfuviz05.last(current);
    });
  };
  var prev = function (current, isRoot) {
    var detection = detect$4(current, isRoot);
    return detection.fold(function () {
      return $_5ee2qxotjfuviz05.none();
    }, function (info) {
      return info.index() - 1 >= 0 ? $_5ee2qxotjfuviz05.middle(current, info.all()[info.index() - 1]) : $_5ee2qxotjfuviz05.first(current);
    });
  };
  var $_elg569osjfuviyzx = {
    next: next,
    prev: prev
  };

  var adt = $_27harem7jfuviyb3.generate([
    { 'before': ['element'] },
    {
      'on': [
        'element',
        'offset'
      ]
    },
    { after: ['element'] }
  ]);
  var cata$1 = function (subject, onBefore, onOn, onAfter) {
    return subject.fold(onBefore, onOn, onAfter);
  };
  var getStart = function (situ) {
    return situ.fold($_fdch7uk7jfuvixxb.identity, $_fdch7uk7jfuvixxb.identity, $_fdch7uk7jfuvixxb.identity);
  };
  var $_8a4rxdovjfuviz0c = {
    before: adt.before,
    on: adt.on,
    after: adt.after,
    cata: cata$1,
    getStart: getStart
  };

  var type$2 = $_27harem7jfuviyb3.generate([
    { domRange: ['rng'] },
    {
      relative: [
        'startSitu',
        'finishSitu'
      ]
    },
    {
      exact: [
        'start',
        'soffset',
        'finish',
        'foffset'
      ]
    }
  ]);
  var range$2 = $_96oqrskbjfuvixya.immutable('start', 'soffset', 'finish', 'foffset');
  var exactFromRange = function (simRange) {
    return type$2.exact(simRange.start(), simRange.soffset(), simRange.finish(), simRange.foffset());
  };
  var getStart$1 = function (selection) {
    return selection.match({
      domRange: function (rng) {
        return $_4sdhm4kkjfuviy0e.fromDom(rng.startContainer);
      },
      relative: function (startSitu, finishSitu) {
        return $_8a4rxdovjfuviz0c.getStart(startSitu);
      },
      exact: function (start, soffset, finish, foffset) {
        return start;
      }
    });
  };
  var getWin = function (selection) {
    var start = getStart$1(selection);
    return $_87w3h3kmjfuviy0m.defaultView(start);
  };
  var $_cirtsuoujfuviz06 = {
    domRange: type$2.domRange,
    relative: type$2.relative,
    exact: type$2.exact,
    exactFromRange: exactFromRange,
    range: range$2,
    getWin: getWin
  };

  var makeRange = function (start, soffset, finish, foffset) {
    var doc = $_87w3h3kmjfuviy0m.owner(start);
    var rng = doc.dom().createRange();
    rng.setStart(start.dom(), soffset);
    rng.setEnd(finish.dom(), foffset);
    return rng;
  };
  var commonAncestorContainer = function (start, soffset, finish, foffset) {
    var r = makeRange(start, soffset, finish, foffset);
    return $_4sdhm4kkjfuviy0e.fromDom(r.commonAncestorContainer);
  };
  var after$2 = function (start, soffset, finish, foffset) {
    var r = makeRange(start, soffset, finish, foffset);
    var same = $_g6ztqikojfuviy13.eq(start, finish) && soffset === foffset;
    return r.collapsed && !same;
  };
  var $_uw3kloxjfuviz0o = {
    after: after$2,
    commonAncestorContainer: commonAncestorContainer
  };

  var fromElements = function (elements, scope) {
    var doc = scope || document;
    var fragment = doc.createDocumentFragment();
    $_4jja6kk5jfuvixx1.each(elements, function (element) {
      fragment.appendChild(element.dom());
    });
    return $_4sdhm4kkjfuviy0e.fromDom(fragment);
  };
  var $_6dn3efoyjfuviz0q = { fromElements: fromElements };

  var selectNodeContents = function (win, element) {
    var rng = win.document.createRange();
    selectNodeContentsUsing(rng, element);
    return rng;
  };
  var selectNodeContentsUsing = function (rng, element) {
    rng.selectNodeContents(element.dom());
  };
  var isWithin$1 = function (outerRange, innerRange) {
    return innerRange.compareBoundaryPoints(outerRange.END_TO_START, outerRange) < 1 && innerRange.compareBoundaryPoints(outerRange.START_TO_END, outerRange) > -1;
  };
  var create$1 = function (win) {
    return win.document.createRange();
  };
  var setStart = function (rng, situ) {
    situ.fold(function (e) {
      rng.setStartBefore(e.dom());
    }, function (e, o) {
      rng.setStart(e.dom(), o);
    }, function (e) {
      rng.setStartAfter(e.dom());
    });
  };
  var setFinish = function (rng, situ) {
    situ.fold(function (e) {
      rng.setEndBefore(e.dom());
    }, function (e, o) {
      rng.setEnd(e.dom(), o);
    }, function (e) {
      rng.setEndAfter(e.dom());
    });
  };
  var replaceWith = function (rng, fragment) {
    deleteContents(rng);
    rng.insertNode(fragment.dom());
  };
  var relativeToNative = function (win, startSitu, finishSitu) {
    var range = win.document.createRange();
    setStart(range, startSitu);
    setFinish(range, finishSitu);
    return range;
  };
  var exactToNative = function (win, start, soffset, finish, foffset) {
    var rng = win.document.createRange();
    rng.setStart(start.dom(), soffset);
    rng.setEnd(finish.dom(), foffset);
    return rng;
  };
  var deleteContents = function (rng) {
    rng.deleteContents();
  };
  var cloneFragment = function (rng) {
    var fragment = rng.cloneContents();
    return $_4sdhm4kkjfuviy0e.fromDom(fragment);
  };
  var toRect = function (rect) {
    return {
      left: $_fdch7uk7jfuvixxb.constant(rect.left),
      top: $_fdch7uk7jfuvixxb.constant(rect.top),
      right: $_fdch7uk7jfuvixxb.constant(rect.right),
      bottom: $_fdch7uk7jfuvixxb.constant(rect.bottom),
      width: $_fdch7uk7jfuvixxb.constant(rect.width),
      height: $_fdch7uk7jfuvixxb.constant(rect.height)
    };
  };
  var getFirstRect = function (rng) {
    var rects = rng.getClientRects();
    var rect = rects.length > 0 ? rects[0] : rng.getBoundingClientRect();
    return rect.width > 0 || rect.height > 0 ? Option.some(rect).map(toRect) : Option.none();
  };
  var getBounds$1 = function (rng) {
    var rect = rng.getBoundingClientRect();
    return rect.width > 0 || rect.height > 0 ? Option.some(rect).map(toRect) : Option.none();
  };
  var toString = function (rng) {
    return rng.toString();
  };
  var $_17pa03ozjfuviz0x = {
    create: create$1,
    replaceWith: replaceWith,
    selectNodeContents: selectNodeContents,
    selectNodeContentsUsing: selectNodeContentsUsing,
    relativeToNative: relativeToNative,
    exactToNative: exactToNative,
    deleteContents: deleteContents,
    cloneFragment: cloneFragment,
    getFirstRect: getFirstRect,
    getBounds: getBounds$1,
    isWithin: isWithin$1,
    toString: toString
  };

  var adt$1 = $_27harem7jfuviyb3.generate([
    {
      ltr: [
        'start',
        'soffset',
        'finish',
        'foffset'
      ]
    },
    {
      rtl: [
        'start',
        'soffset',
        'finish',
        'foffset'
      ]
    }
  ]);
  var fromRange = function (win, type, range) {
    return type($_4sdhm4kkjfuviy0e.fromDom(range.startContainer), range.startOffset, $_4sdhm4kkjfuviy0e.fromDom(range.endContainer), range.endOffset);
  };
  var getRanges = function (win, selection) {
    return selection.match({
      domRange: function (rng) {
        return {
          ltr: $_fdch7uk7jfuvixxb.constant(rng),
          rtl: Option.none
        };
      },
      relative: function (startSitu, finishSitu) {
        return {
          ltr: $_eprzwrkujfuviy1p.cached(function () {
            return $_17pa03ozjfuviz0x.relativeToNative(win, startSitu, finishSitu);
          }),
          rtl: $_eprzwrkujfuviy1p.cached(function () {
            return Option.some($_17pa03ozjfuviz0x.relativeToNative(win, finishSitu, startSitu));
          })
        };
      },
      exact: function (start, soffset, finish, foffset) {
        return {
          ltr: $_eprzwrkujfuviy1p.cached(function () {
            return $_17pa03ozjfuviz0x.exactToNative(win, start, soffset, finish, foffset);
          }),
          rtl: $_eprzwrkujfuviy1p.cached(function () {
            return Option.some($_17pa03ozjfuviz0x.exactToNative(win, finish, foffset, start, soffset));
          })
        };
      }
    });
  };
  var doDiagnose = function (win, ranges) {
    var rng = ranges.ltr();
    if (rng.collapsed) {
      var reversed = ranges.rtl().filter(function (rev) {
        return rev.collapsed === false;
      });
      return reversed.map(function (rev) {
        return adt$1.rtl($_4sdhm4kkjfuviy0e.fromDom(rev.endContainer), rev.endOffset, $_4sdhm4kkjfuviy0e.fromDom(rev.startContainer), rev.startOffset);
      }).getOrThunk(function () {
        return fromRange(win, adt$1.ltr, rng);
      });
    } else {
      return fromRange(win, adt$1.ltr, rng);
    }
  };
  var diagnose = function (win, selection) {
    var ranges = getRanges(win, selection);
    return doDiagnose(win, ranges);
  };
  var asLtrRange = function (win, selection) {
    var diagnosis = diagnose(win, selection);
    return diagnosis.match({
      ltr: function (start, soffset, finish, foffset) {
        var rng = win.document.createRange();
        rng.setStart(start.dom(), soffset);
        rng.setEnd(finish.dom(), foffset);
        return rng;
      },
      rtl: function (start, soffset, finish, foffset) {
        var rng = win.document.createRange();
        rng.setStart(finish.dom(), foffset);
        rng.setEnd(start.dom(), soffset);
        return rng;
      }
    });
  };
  var $_6ufympp0jfuviz14 = {
    ltr: adt$1.ltr,
    rtl: adt$1.rtl,
    diagnose: diagnose,
    asLtrRange: asLtrRange
  };

  var searchForPoint = function (rectForOffset, x, y, maxX, length) {
    if (length === 0)
      return 0;
    else if (x === maxX)
      return length - 1;
    var xDelta = maxX;
    for (var i = 1; i < length; i++) {
      var rect = rectForOffset(i);
      var curDeltaX = Math.abs(x - rect.left);
      if (y > rect.bottom) {
      } else if (y < rect.top || curDeltaX > xDelta) {
        return i - 1;
      } else {
        xDelta = curDeltaX;
      }
    }
    return 0;
  };
  var inRect = function (rect, x, y) {
    return x >= rect.left && x <= rect.right && y >= rect.top && y <= rect.bottom;
  };
  var $_69qd6cp3jfuviz1r = {
    inRect: inRect,
    searchForPoint: searchForPoint
  };

  var locateOffset = function (doc, textnode, x, y, rect) {
    var rangeForOffset = function (offset) {
      var r = doc.dom().createRange();
      r.setStart(textnode.dom(), offset);
      r.collapse(true);
      return r;
    };
    var rectForOffset = function (offset) {
      var r = rangeForOffset(offset);
      return r.getBoundingClientRect();
    };
    var length = $_cvagqflnjfuviy5y.get(textnode).length;
    var offset = $_69qd6cp3jfuviz1r.searchForPoint(rectForOffset, x, y, rect.right, length);
    return rangeForOffset(offset);
  };
  var locate = function (doc, node, x, y) {
    var r = doc.dom().createRange();
    r.selectNode(node.dom());
    var rects = r.getClientRects();
    var foundRect = $_6i1r9dmvjfuviygw.findMap(rects, function (rect) {
      return $_69qd6cp3jfuviz1r.inRect(rect, x, y) ? Option.some(rect) : Option.none();
    });
    return foundRect.map(function (rect) {
      return locateOffset(doc, node, x, y, rect);
    });
  };
  var $_g1jcqdp4jfuviz1t = { locate: locate };

  var searchInChildren = function (doc, node, x, y) {
    var r = doc.dom().createRange();
    var nodes = $_87w3h3kmjfuviy0m.children(node);
    return $_6i1r9dmvjfuviygw.findMap(nodes, function (n) {
      r.selectNode(n.dom());
      return $_69qd6cp3jfuviz1r.inRect(r.getBoundingClientRect(), x, y) ? locateNode(doc, n, x, y) : Option.none();
    });
  };
  var locateNode = function (doc, node, x, y) {
    var locator = $_6mcqmml6jfuviy2u.isText(node) ? $_g1jcqdp4jfuviz1t.locate : searchInChildren;
    return locator(doc, node, x, y);
  };
  var locate$1 = function (doc, node, x, y) {
    var r = doc.dom().createRange();
    r.selectNode(node.dom());
    var rect = r.getBoundingClientRect();
    var boundedX = Math.max(rect.left, Math.min(rect.right, x));
    var boundedY = Math.max(rect.top, Math.min(rect.bottom, y));
    return locateNode(doc, node, boundedX, boundedY);
  };
  var $_7hieojp2jfuviz1k = { locate: locate$1 };

  var COLLAPSE_TO_LEFT = true;
  var COLLAPSE_TO_RIGHT = false;
  var getCollapseDirection = function (rect, x) {
    return x - rect.left < rect.right - x ? COLLAPSE_TO_LEFT : COLLAPSE_TO_RIGHT;
  };
  var createCollapsedNode = function (doc, target, collapseDirection) {
    var r = doc.dom().createRange();
    r.selectNode(target.dom());
    r.collapse(collapseDirection);
    return r;
  };
  var locateInElement = function (doc, node, x) {
    var cursorRange = doc.dom().createRange();
    cursorRange.selectNode(node.dom());
    var rect = cursorRange.getBoundingClientRect();
    var collapseDirection = getCollapseDirection(rect, x);
    var f = collapseDirection === COLLAPSE_TO_LEFT ? $_dsfijblljfuviy5q.first : $_dsfijblljfuviy5q.last;
    return f(node).map(function (target) {
      return createCollapsedNode(doc, target, collapseDirection);
    });
  };
  var locateInEmpty = function (doc, node, x) {
    var rect = node.dom().getBoundingClientRect();
    var collapseDirection = getCollapseDirection(rect, x);
    return Option.some(createCollapsedNode(doc, node, collapseDirection));
  };
  var search = function (doc, node, x) {
    var f = $_87w3h3kmjfuviy0m.children(node).length === 0 ? locateInEmpty : locateInElement;
    return f(doc, node, x);
  };
  var $_f8hpchp5jfuviz1z = { search: search };

  var caretPositionFromPoint = function (doc, x, y) {
    return Option.from(doc.dom().caretPositionFromPoint(x, y)).bind(function (pos) {
      if (pos.offsetNode === null)
        return Option.none();
      var r = doc.dom().createRange();
      r.setStart(pos.offsetNode, pos.offset);
      r.collapse();
      return Option.some(r);
    });
  };
  var caretRangeFromPoint = function (doc, x, y) {
    return Option.from(doc.dom().caretRangeFromPoint(x, y));
  };
  var searchTextNodes = function (doc, node, x, y) {
    var r = doc.dom().createRange();
    r.selectNode(node.dom());
    var rect = r.getBoundingClientRect();
    var boundedX = Math.max(rect.left, Math.min(rect.right, x));
    var boundedY = Math.max(rect.top, Math.min(rect.bottom, y));
    return $_7hieojp2jfuviz1k.locate(doc, node, boundedX, boundedY);
  };
  var searchFromPoint = function (doc, x, y) {
    return $_4sdhm4kkjfuviy0e.fromPoint(doc, x, y).bind(function (elem) {
      var fallback = function () {
        return $_f8hpchp5jfuviz1z.search(doc, elem, x);
      };
      return $_87w3h3kmjfuviy0m.children(elem).length === 0 ? fallback() : searchTextNodes(doc, elem, x, y).orThunk(fallback);
    });
  };
  var availableSearch = document.caretPositionFromPoint ? caretPositionFromPoint : document.caretRangeFromPoint ? caretRangeFromPoint : searchFromPoint;
  var fromPoint$1 = function (win, x, y) {
    var doc = $_4sdhm4kkjfuviy0e.fromDom(win.document);
    return availableSearch(doc, x, y).map(function (rng) {
      return $_cirtsuoujfuviz06.range($_4sdhm4kkjfuviy0e.fromDom(rng.startContainer), rng.startOffset, $_4sdhm4kkjfuviy0e.fromDom(rng.endContainer), rng.endOffset);
    });
  };
  var $_3rwbmap1jfuviz1g = { fromPoint: fromPoint$1 };

  var withinContainer = function (win, ancestor, outerRange, selector) {
    var innerRange = $_17pa03ozjfuviz0x.create(win);
    var self = $_aphf8fkjjfuviy04.is(ancestor, selector) ? [ancestor] : [];
    var elements = self.concat($_a3hs1bl7jfuviy2w.descendants(ancestor, selector));
    return $_4jja6kk5jfuvixx1.filter(elements, function (elem) {
      $_17pa03ozjfuviz0x.selectNodeContentsUsing(innerRange, elem);
      return $_17pa03ozjfuviz0x.isWithin(outerRange, innerRange);
    });
  };
  var find$3 = function (win, selection, selector) {
    var outerRange = $_6ufympp0jfuviz14.asLtrRange(win, selection);
    var ancestor = $_4sdhm4kkjfuviy0e.fromDom(outerRange.commonAncestorContainer);
    return $_6mcqmml6jfuviy2u.isElement(ancestor) ? withinContainer(win, ancestor, outerRange, selector) : [];
  };
  var $_frv4mfp6jfuviz24 = { find: find$3 };

  var beforeSpecial = function (element, offset) {
    var name = $_6mcqmml6jfuviy2u.name(element);
    if ('input' === name)
      return $_8a4rxdovjfuviz0c.after(element);
    else if (!$_4jja6kk5jfuvixx1.contains([
        'br',
        'img'
      ], name))
      return $_8a4rxdovjfuviz0c.on(element, offset);
    else
      return offset === 0 ? $_8a4rxdovjfuviz0c.before(element) : $_8a4rxdovjfuviz0c.after(element);
  };
  var preprocessRelative = function (startSitu, finishSitu) {
    var start = startSitu.fold($_8a4rxdovjfuviz0c.before, beforeSpecial, $_8a4rxdovjfuviz0c.after);
    var finish = finishSitu.fold($_8a4rxdovjfuviz0c.before, beforeSpecial, $_8a4rxdovjfuviz0c.after);
    return $_cirtsuoujfuviz06.relative(start, finish);
  };
  var preprocessExact = function (start, soffset, finish, foffset) {
    var startSitu = beforeSpecial(start, soffset);
    var finishSitu = beforeSpecial(finish, foffset);
    return $_cirtsuoujfuviz06.relative(startSitu, finishSitu);
  };
  var preprocess = function (selection) {
    return selection.match({
      domRange: function (rng) {
        var start = $_4sdhm4kkjfuviy0e.fromDom(rng.startContainer);
        var finish = $_4sdhm4kkjfuviy0e.fromDom(rng.endContainer);
        return preprocessExact(start, rng.startOffset, finish, rng.endOffset);
      },
      relative: preprocessRelative,
      exact: preprocessExact
    });
  };
  var $_9fuk5hp7jfuviz29 = {
    beforeSpecial: beforeSpecial,
    preprocess: preprocess,
    preprocessRelative: preprocessRelative,
    preprocessExact: preprocessExact
  };

  var doSetNativeRange = function (win, rng) {
    Option.from(win.getSelection()).each(function (selection) {
      selection.removeAllRanges();
      selection.addRange(rng);
    });
  };
  var doSetRange = function (win, start, soffset, finish, foffset) {
    var rng = $_17pa03ozjfuviz0x.exactToNative(win, start, soffset, finish, foffset);
    doSetNativeRange(win, rng);
  };
  var findWithin = function (win, selection, selector) {
    return $_frv4mfp6jfuviz24.find(win, selection, selector);
  };
  var setRangeFromRelative = function (win, relative) {
    return $_6ufympp0jfuviz14.diagnose(win, relative).match({
      ltr: function (start, soffset, finish, foffset) {
        doSetRange(win, start, soffset, finish, foffset);
      },
      rtl: function (start, soffset, finish, foffset) {
        var selection = win.getSelection();
        if (selection.setBaseAndExtent) {
          selection.setBaseAndExtent(start.dom(), soffset, finish.dom(), foffset);
        } else if (selection.extend) {
          selection.collapse(start.dom(), soffset);
          selection.extend(finish.dom(), foffset);
        } else {
          doSetRange(win, finish, foffset, start, soffset);
        }
      }
    });
  };
  var setExact = function (win, start, soffset, finish, foffset) {
    var relative = $_9fuk5hp7jfuviz29.preprocessExact(start, soffset, finish, foffset);
    setRangeFromRelative(win, relative);
  };
  var setRelative = function (win, startSitu, finishSitu) {
    var relative = $_9fuk5hp7jfuviz29.preprocessRelative(startSitu, finishSitu);
    setRangeFromRelative(win, relative);
  };
  var toNative = function (selection) {
    var win = $_cirtsuoujfuviz06.getWin(selection).dom();
    var getDomRange = function (start, soffset, finish, foffset) {
      return $_17pa03ozjfuviz0x.exactToNative(win, start, soffset, finish, foffset);
    };
    var filtered = $_9fuk5hp7jfuviz29.preprocess(selection);
    return $_6ufympp0jfuviz14.diagnose(win, filtered).match({
      ltr: getDomRange,
      rtl: getDomRange
    });
  };
  var readRange = function (selection) {
    if (selection.rangeCount > 0) {
      var firstRng = selection.getRangeAt(0);
      var lastRng = selection.getRangeAt(selection.rangeCount - 1);
      return Option.some($_cirtsuoujfuviz06.range($_4sdhm4kkjfuviy0e.fromDom(firstRng.startContainer), firstRng.startOffset, $_4sdhm4kkjfuviy0e.fromDom(lastRng.endContainer), lastRng.endOffset));
    } else {
      return Option.none();
    }
  };
  var doGetExact = function (selection) {
    var anchorNode = $_4sdhm4kkjfuviy0e.fromDom(selection.anchorNode);
    var focusNode = $_4sdhm4kkjfuviy0e.fromDom(selection.focusNode);
    return $_uw3kloxjfuviz0o.after(anchorNode, selection.anchorOffset, focusNode, selection.focusOffset) ? Option.some($_cirtsuoujfuviz06.range($_4sdhm4kkjfuviy0e.fromDom(selection.anchorNode), selection.anchorOffset, $_4sdhm4kkjfuviy0e.fromDom(selection.focusNode), selection.focusOffset)) : readRange(selection);
  };
  var setToElement = function (win, element) {
    var rng = $_17pa03ozjfuviz0x.selectNodeContents(win, element);
    doSetNativeRange(win, rng);
  };
  var forElement = function (win, element) {
    var rng = $_17pa03ozjfuviz0x.selectNodeContents(win, element);
    return $_cirtsuoujfuviz06.range($_4sdhm4kkjfuviy0e.fromDom(rng.startContainer), rng.startOffset, $_4sdhm4kkjfuviy0e.fromDom(rng.endContainer), rng.endOffset);
  };
  var getExact = function (win) {
    var selection = win.getSelection();
    return selection.rangeCount > 0 ? doGetExact(selection) : Option.none();
  };
  var get$9 = function (win) {
    return getExact(win).map(function (range) {
      return $_cirtsuoujfuviz06.exact(range.start(), range.soffset(), range.finish(), range.foffset());
    });
  };
  var getFirstRect$1 = function (win, selection) {
    var rng = $_6ufympp0jfuviz14.asLtrRange(win, selection);
    return $_17pa03ozjfuviz0x.getFirstRect(rng);
  };
  var getBounds$2 = function (win, selection) {
    var rng = $_6ufympp0jfuviz14.asLtrRange(win, selection);
    return $_17pa03ozjfuviz0x.getBounds(rng);
  };
  var getAtPoint = function (win, x, y) {
    return $_3rwbmap1jfuviz1g.fromPoint(win, x, y);
  };
  var getAsString = function (win, selection) {
    var rng = $_6ufympp0jfuviz14.asLtrRange(win, selection);
    return $_17pa03ozjfuviz0x.toString(rng);
  };
  var clear$1 = function (win) {
    var selection = win.getSelection();
    selection.removeAllRanges();
  };
  var clone$2 = function (win, selection) {
    var rng = $_6ufympp0jfuviz14.asLtrRange(win, selection);
    return $_17pa03ozjfuviz0x.cloneFragment(rng);
  };
  var replace$1 = function (win, selection, elements) {
    var rng = $_6ufympp0jfuviz14.asLtrRange(win, selection);
    var fragment = $_6dn3efoyjfuviz0q.fromElements(elements, win.document);
    $_17pa03ozjfuviz0x.replaceWith(rng, fragment);
  };
  var deleteAt = function (win, selection) {
    var rng = $_6ufympp0jfuviz14.asLtrRange(win, selection);
    $_17pa03ozjfuviz0x.deleteContents(rng);
  };
  var isCollapsed = function (start, soffset, finish, foffset) {
    return $_g6ztqikojfuviy13.eq(start, finish) && soffset === foffset;
  };
  var $_3z0lbzowjfuviz0i = {
    setExact: setExact,
    getExact: getExact,
    get: get$9,
    setRelative: setRelative,
    toNative: toNative,
    setToElement: setToElement,
    clear: clear$1,
    clone: clone$2,
    replace: replace$1,
    deleteAt: deleteAt,
    forElement: forElement,
    getFirstRect: getFirstRect$1,
    getBounds: getBounds$2,
    getAtPoint: getAtPoint,
    findWithin: findWithin,
    getAsString: getAsString,
    isCollapsed: isCollapsed
  };

  var global$4 = tinymce.util.Tools.resolve('tinymce.util.VK');

  var forward = function (editor, isRoot, cell, lazyWire) {
    return go(editor, isRoot, $_elg569osjfuviyzx.next(cell), lazyWire);
  };
  var backward = function (editor, isRoot, cell, lazyWire) {
    return go(editor, isRoot, $_elg569osjfuviyzx.prev(cell), lazyWire);
  };
  var getCellFirstCursorPosition = function (editor, cell) {
    var selection = $_cirtsuoujfuviz06.exact(cell, 0, cell, 0);
    return $_3z0lbzowjfuviz0i.toNative(selection);
  };
  var getNewRowCursorPosition = function (editor, table) {
    var rows = $_a3hs1bl7jfuviy2w.descendants(table, 'tr');
    return $_4jja6kk5jfuvixx1.last(rows).bind(function (last) {
      return $_26gnp6lajfuviy35.descendant(last, 'td,th').map(function (first) {
        return getCellFirstCursorPosition(editor, first);
      });
    });
  };
  var go = function (editor, isRoot, cell, actions, lazyWire) {
    return cell.fold(Option.none, Option.none, function (current, next) {
      return $_dsfijblljfuviy5q.first(next).map(function (cell) {
        return getCellFirstCursorPosition(editor, cell);
      });
    }, function (current) {
      return $_dmqxswkhjfuvixyz.table(current, isRoot).bind(function (table) {
        var targets = $_fgxiq3lqjfuviy6a.noMenu(current);
        editor.undoManager.transact(function () {
          actions.insertRowsAfter(table, targets);
        });
        return getNewRowCursorPosition(editor, table);
      });
    });
  };
  var rootElements = [
    'table',
    'li',
    'dl'
  ];
  var handle$1 = function (event, editor, actions, lazyWire) {
    if (event.keyCode === global$4.TAB) {
      var body_1 = $_aheu0fnnjfuviymj.getBody(editor);
      var isRoot_1 = function (element) {
        var name = $_6mcqmml6jfuviy2u.name(element);
        return $_g6ztqikojfuviy13.eq(element, body_1) || $_4jja6kk5jfuvixx1.contains(rootElements, name);
      };
      var rng = editor.selection.getRng();
      if (rng.collapsed) {
        var start = $_4sdhm4kkjfuviy0e.fromDom(rng.startContainer);
        $_dmqxswkhjfuvixyz.cell(start, isRoot_1).each(function (cell) {
          event.preventDefault();
          var navigation = event.shiftKey ? backward : forward;
          var rng = navigation(editor, isRoot_1, cell, actions, lazyWire);
          rng.each(function (range) {
            editor.selection.setRng(range);
          });
        });
      }
    }
  };
  var $_flm1p1orjfuviyzd = { handle: handle$1 };

  var response = $_96oqrskbjfuvixya.immutable('selection', 'kill');
  var $_8nrj5cpbjfuviz3g = { response: response };

  var isKey = function (key) {
    return function (keycode) {
      return keycode === key;
    };
  };
  var isUp = isKey(38);
  var isDown = isKey(40);
  var isNavigation = function (keycode) {
    return keycode >= 37 && keycode <= 40;
  };
  var $_668mt4pcjfuviz3j = {
    ltr: {
      isBackward: isKey(37),
      isForward: isKey(39)
    },
    rtl: {
      isBackward: isKey(39),
      isForward: isKey(37)
    },
    isUp: isUp,
    isDown: isDown,
    isNavigation: isNavigation
  };

  var convertToRange = function (win, selection) {
    var rng = $_6ufympp0jfuviz14.asLtrRange(win, selection);
    return {
      start: $_fdch7uk7jfuvixxb.constant($_4sdhm4kkjfuviy0e.fromDom(rng.startContainer)),
      soffset: $_fdch7uk7jfuvixxb.constant(rng.startOffset),
      finish: $_fdch7uk7jfuvixxb.constant($_4sdhm4kkjfuviy0e.fromDom(rng.endContainer)),
      foffset: $_fdch7uk7jfuvixxb.constant(rng.endOffset)
    };
  };
  var makeSitus = function (start, soffset, finish, foffset) {
    return {
      start: $_fdch7uk7jfuvixxb.constant($_8a4rxdovjfuviz0c.on(start, soffset)),
      finish: $_fdch7uk7jfuvixxb.constant($_8a4rxdovjfuviz0c.on(finish, foffset))
    };
  };
  var $_ehlw1upejfuviz45 = {
    convertToRange: convertToRange,
    makeSitus: makeSitus
  };

  var isSafari = $_8chrc7ktjfuviy1m.detect().browser.isSafari();
  var get$10 = function (_doc) {
    var doc = _doc !== undefined ? _doc.dom() : document;
    var x = doc.body.scrollLeft || doc.documentElement.scrollLeft;
    var y = doc.body.scrollTop || doc.documentElement.scrollTop;
    return r(x, y);
  };
  var to = function (x, y, _doc) {
    var doc = _doc !== undefined ? _doc.dom() : document;
    var win = doc.defaultView;
    win.scrollTo(x, y);
  };
  var by = function (x, y, _doc) {
    var doc = _doc !== undefined ? _doc.dom() : document;
    var win = doc.defaultView;
    win.scrollBy(x, y);
  };
  var setToElement$1 = function (win, element) {
    var pos = $_6ti42xmijfuviydp.absolute(element);
    var doc = $_4sdhm4kkjfuviy0e.fromDom(win.document);
    to(pos.left(), pos.top(), doc);
  };
  var preserve$1 = function (doc, f) {
    var before = get$10(doc);
    f();
    var after = get$10(doc);
    if (before.top() !== after.top() || before.left() !== after.left()) {
      to(before.left(), before.top(), doc);
    }
  };
  var capture$2 = function (doc) {
    var previous = Option.none();
    var save = function () {
      previous = Option.some(get$10(doc));
    };
    var restore = function () {
      previous.each(function (p) {
        to(p.left(), p.top(), doc);
      });
    };
    save();
    return {
      save: save,
      restore: restore
    };
  };
  var intoView = function (element, alignToTop) {
    if (isSafari && $_13kw1fk8jfuvixxd.isFunction(element.dom().scrollIntoViewIfNeeded)) {
      element.dom().scrollIntoViewIfNeeded(false);
    } else {
      element.dom().scrollIntoView(alignToTop);
    }
  };
  var intoViewIfNeeded = function (element, container) {
    var containerBox = container.dom().getBoundingClientRect();
    var elementBox = element.dom().getBoundingClientRect();
    if (elementBox.top < containerBox.top) {
      intoView(element, true);
    } else if (elementBox.bottom > containerBox.bottom) {
      intoView(element, false);
    }
  };
  var scrollBarWidth = function () {
    var scrollDiv = $_4sdhm4kkjfuviy0e.fromHtml('<div style="width: 100px; height: 100px; overflow: scroll; position: absolute; top: -9999px;"></div>');
    $_5zcsfmlgjfuviy4g.after($_43dxxcl9jfuviy31.body(), scrollDiv);
    var w = scrollDiv.dom().offsetWidth - scrollDiv.dom().clientWidth;
    $_5ud3colhjfuviy4l.remove(scrollDiv);
    return w;
  };
  var $_49zbyzpfjfuviz4g = {
    get: get$10,
    to: to,
    by: by,
    preserve: preserve$1,
    capture: capture$2,
    intoView: intoView,
    intoViewIfNeeded: intoViewIfNeeded,
    setToElement: setToElement$1,
    scrollBarWidth: scrollBarWidth
  };

  function WindowBridge (win) {
    var elementFromPoint = function (x, y) {
      return Option.from(win.document.elementFromPoint(x, y)).map($_4sdhm4kkjfuviy0e.fromDom);
    };
    var getRect = function (element) {
      return element.dom().getBoundingClientRect();
    };
    var getRangedRect = function (start, soffset, finish, foffset) {
      var sel = $_cirtsuoujfuviz06.exact(start, soffset, finish, foffset);
      return $_3z0lbzowjfuviz0i.getFirstRect(win, sel).map(function (structRect) {
        return $_afb9m6kajfuvixy8.map(structRect, $_fdch7uk7jfuvixxb.apply);
      });
    };
    var getSelection = function () {
      return $_3z0lbzowjfuviz0i.get(win).map(function (exactAdt) {
        return $_ehlw1upejfuviz45.convertToRange(win, exactAdt);
      });
    };
    var fromSitus = function (situs) {
      var relative = $_cirtsuoujfuviz06.relative(situs.start(), situs.finish());
      return $_ehlw1upejfuviz45.convertToRange(win, relative);
    };
    var situsFromPoint = function (x, y) {
      return $_3z0lbzowjfuviz0i.getAtPoint(win, x, y).map(function (exact) {
        return {
          start: $_fdch7uk7jfuvixxb.constant($_8a4rxdovjfuviz0c.on(exact.start(), exact.soffset())),
          finish: $_fdch7uk7jfuvixxb.constant($_8a4rxdovjfuviz0c.on(exact.finish(), exact.foffset()))
        };
      });
    };
    var clearSelection = function () {
      $_3z0lbzowjfuviz0i.clear(win);
    };
    var selectContents = function (element) {
      $_3z0lbzowjfuviz0i.setToElement(win, element);
    };
    var setSelection = function (sel) {
      $_3z0lbzowjfuviz0i.setExact(win, sel.start(), sel.soffset(), sel.finish(), sel.foffset());
    };
    var setRelativeSelection = function (start, finish) {
      $_3z0lbzowjfuviz0i.setRelative(win, start, finish);
    };
    var getInnerHeight = function () {
      return win.innerHeight;
    };
    var getScrollY = function () {
      var pos = $_49zbyzpfjfuviz4g.get($_4sdhm4kkjfuviy0e.fromDom(win.document));
      return pos.top();
    };
    var scrollBy = function (x, y) {
      $_49zbyzpfjfuviz4g.by(x, y, $_4sdhm4kkjfuviy0e.fromDom(win.document));
    };
    return {
      elementFromPoint: elementFromPoint,
      getRect: getRect,
      getRangedRect: getRangedRect,
      getSelection: getSelection,
      fromSitus: fromSitus,
      situsFromPoint: situsFromPoint,
      clearSelection: clearSelection,
      setSelection: setSelection,
      setRelativeSelection: setRelativeSelection,
      selectContents: selectContents,
      getInnerHeight: getInnerHeight,
      getScrollY: getScrollY,
      scrollBy: scrollBy
    };
  }

  var sync = function (container, isRoot, start, soffset, finish, foffset, selectRange) {
    if (!($_g6ztqikojfuviy13.eq(start, finish) && soffset === foffset)) {
      return $_26gnp6lajfuviy35.closest(start, 'td,th', isRoot).bind(function (s) {
        return $_26gnp6lajfuviy35.closest(finish, 'td,th', isRoot).bind(function (f) {
          return detect$5(container, isRoot, s, f, selectRange);
        });
      });
    } else {
      return Option.none();
    }
  };
  var detect$5 = function (container, isRoot, start, finish, selectRange) {
    if (!$_g6ztqikojfuviy13.eq(start, finish)) {
      return $_8vwxtkltjfuviy76.identify(start, finish, isRoot).bind(function (cellSel) {
        var boxes = cellSel.boxes().getOr([]);
        if (boxes.length > 0) {
          selectRange(container, boxes, cellSel.start(), cellSel.finish());
          return Option.some($_8nrj5cpbjfuviz3g.response(Option.some($_ehlw1upejfuviz45.makeSitus(start, 0, start, $_cj2m48lmjfuviy5u.getEnd(start))), true));
        } else {
          return Option.none();
        }
      });
    } else {
      return Option.none();
    }
  };
  var update = function (rows, columns, container, selected, annotations) {
    var updateSelection = function (newSels) {
      annotations.clear(container);
      annotations.selectRange(container, newSels.boxes(), newSels.start(), newSels.finish());
      return newSels.boxes();
    };
    return $_8vwxtkltjfuviy76.shiftSelection(selected, rows, columns, annotations.firstSelectedSelector(), annotations.lastSelectedSelector()).map(updateSelection);
  };
  var $_9sykywpgjfuviz4r = {
    sync: sync,
    detect: detect$5,
    update: update
  };

  var nu$3 = $_96oqrskbjfuvixya.immutableBag([
    'left',
    'top',
    'right',
    'bottom'
  ], []);
  var moveDown = function (caret, amount) {
    return nu$3({
      left: caret.left(),
      top: caret.top() + amount,
      right: caret.right(),
      bottom: caret.bottom() + amount
    });
  };
  var moveUp = function (caret, amount) {
    return nu$3({
      left: caret.left(),
      top: caret.top() - amount,
      right: caret.right(),
      bottom: caret.bottom() - amount
    });
  };
  var moveBottomTo = function (caret, bottom) {
    var height = caret.bottom() - caret.top();
    return nu$3({
      left: caret.left(),
      top: bottom - height,
      right: caret.right(),
      bottom: bottom
    });
  };
  var moveTopTo = function (caret, top) {
    var height = caret.bottom() - caret.top();
    return nu$3({
      left: caret.left(),
      top: top,
      right: caret.right(),
      bottom: top + height
    });
  };
  var translate = function (caret, xDelta, yDelta) {
    return nu$3({
      left: caret.left() + xDelta,
      top: caret.top() + yDelta,
      right: caret.right() + xDelta,
      bottom: caret.bottom() + yDelta
    });
  };
  var getTop$1 = function (caret) {
    return caret.top();
  };
  var getBottom = function (caret) {
    return caret.bottom();
  };
  var toString$1 = function (caret) {
    return '(' + caret.left() + ', ' + caret.top() + ') -> (' + caret.right() + ', ' + caret.bottom() + ')';
  };
  var $_an24q2pjjfuviz6a = {
    nu: nu$3,
    moveUp: moveUp,
    moveDown: moveDown,
    moveBottomTo: moveBottomTo,
    moveTopTo: moveTopTo,
    getTop: getTop$1,
    getBottom: getBottom,
    translate: translate,
    toString: toString$1
  };

  var getPartialBox = function (bridge, element, offset) {
    if (offset >= 0 && offset < $_cj2m48lmjfuviy5u.getEnd(element))
      return bridge.getRangedRect(element, offset, element, offset + 1);
    else if (offset > 0)
      return bridge.getRangedRect(element, offset - 1, element, offset);
    return Option.none();
  };
  var toCaret = function (rect) {
    return $_an24q2pjjfuviz6a.nu({
      left: rect.left,
      top: rect.top,
      right: rect.right,
      bottom: rect.bottom
    });
  };
  var getElemBox = function (bridge, element) {
    return Option.some(bridge.getRect(element));
  };
  var getBoxAt = function (bridge, element, offset) {
    if ($_6mcqmml6jfuviy2u.isElement(element))
      return getElemBox(bridge, element).map(toCaret);
    else if ($_6mcqmml6jfuviy2u.isText(element))
      return getPartialBox(bridge, element, offset).map(toCaret);
    else
      return Option.none();
  };
  var getEntireBox = function (bridge, element) {
    if ($_6mcqmml6jfuviy2u.isElement(element))
      return getElemBox(bridge, element).map(toCaret);
    else if ($_6mcqmml6jfuviy2u.isText(element))
      return bridge.getRangedRect(element, 0, element, $_cj2m48lmjfuviy5u.getEnd(element)).map(toCaret);
    else
      return Option.none();
  };
  var $_3hp2alpkjfuviz6e = {
    getBoxAt: getBoxAt,
    getEntireBox: getEntireBox
  };

  var traverse = $_96oqrskbjfuvixya.immutable('item', 'mode');
  var backtrack = function (universe, item, direction, _transition) {
    var transition = _transition !== undefined ? _transition : sidestep;
    return universe.property().parent(item).map(function (p) {
      return traverse(p, transition);
    });
  };
  var sidestep = function (universe, item, direction, _transition) {
    var transition = _transition !== undefined ? _transition : advance;
    return direction.sibling(universe, item).map(function (p) {
      return traverse(p, transition);
    });
  };
  var advance = function (universe, item, direction, _transition) {
    var transition = _transition !== undefined ? _transition : advance;
    var children = universe.property().children(item);
    var result = direction.first(children);
    return result.map(function (r) {
      return traverse(r, transition);
    });
  };
  var successors = [
    {
      current: backtrack,
      next: sidestep,
      fallback: Option.none()
    },
    {
      current: sidestep,
      next: advance,
      fallback: Option.some(backtrack)
    },
    {
      current: advance,
      next: advance,
      fallback: Option.some(sidestep)
    }
  ];
  var go$1 = function (universe, item, mode, direction, rules) {
    var rules = rules !== undefined ? rules : successors;
    var ruleOpt = $_4jja6kk5jfuvixx1.find(rules, function (succ) {
      return succ.current === mode;
    });
    return ruleOpt.bind(function (rule) {
      return rule.current(universe, item, direction, rule.next).orThunk(function () {
        return rule.fallback.bind(function (fb) {
          return go$1(universe, item, fb, direction);
        });
      });
    });
  };
  var $_4n5pkzppjfuviz7k = {
    backtrack: backtrack,
    sidestep: sidestep,
    advance: advance,
    go: go$1
  };

  var left$1 = function () {
    var sibling = function (universe, item) {
      return universe.query().prevSibling(item);
    };
    var first = function (children) {
      return children.length > 0 ? Option.some(children[children.length - 1]) : Option.none();
    };
    return {
      sibling: sibling,
      first: first
    };
  };
  var right$1 = function () {
    var sibling = function (universe, item) {
      return universe.query().nextSibling(item);
    };
    var first = function (children) {
      return children.length > 0 ? Option.some(children[0]) : Option.none();
    };
    return {
      sibling: sibling,
      first: first
    };
  };
  var $_fchu6fpqjfuviz7s = {
    left: left$1,
    right: right$1
  };

  var hone = function (universe, item, predicate, mode, direction, isRoot) {
    var next = $_4n5pkzppjfuviz7k.go(universe, item, mode, direction);
    return next.bind(function (n) {
      if (isRoot(n.item()))
        return Option.none();
      else
        return predicate(n.item()) ? Option.some(n.item()) : hone(universe, n.item(), predicate, n.mode(), direction, isRoot);
    });
  };
  var left$2 = function (universe, item, predicate, isRoot) {
    return hone(universe, item, predicate, $_4n5pkzppjfuviz7k.sidestep, $_fchu6fpqjfuviz7s.left(), isRoot);
  };
  var right$2 = function (universe, item, predicate, isRoot) {
    return hone(universe, item, predicate, $_4n5pkzppjfuviz7k.sidestep, $_fchu6fpqjfuviz7s.right(), isRoot);
  };
  var $_73mh4upojfuviz7g = {
    left: left$2,
    right: right$2
  };

  var isLeaf = function (universe, element) {
    return universe.property().children(element).length === 0;
  };
  var before$2 = function (universe, item, isRoot) {
    return seekLeft(universe, item, $_fdch7uk7jfuvixxb.curry(isLeaf, universe), isRoot);
  };
  var after$3 = function (universe, item, isRoot) {
    return seekRight(universe, item, $_fdch7uk7jfuvixxb.curry(isLeaf, universe), isRoot);
  };
  var seekLeft = function (universe, item, predicate, isRoot) {
    return $_73mh4upojfuviz7g.left(universe, item, predicate, isRoot);
  };
  var seekRight = function (universe, item, predicate, isRoot) {
    return $_73mh4upojfuviz7g.right(universe, item, predicate, isRoot);
  };
  var walkers = function () {
    return {
      left: $_fchu6fpqjfuviz7s.left,
      right: $_fchu6fpqjfuviz7s.right
    };
  };
  var walk = function (universe, item, mode, direction, _rules) {
    return $_4n5pkzppjfuviz7k.go(universe, item, mode, direction, _rules);
  };
  var $_1i6gx7pnjfuviz7c = {
    before: before$2,
    after: after$3,
    seekLeft: seekLeft,
    seekRight: seekRight,
    walkers: walkers,
    walk: walk,
    backtrack: $_4n5pkzppjfuviz7k.backtrack,
    sidestep: $_4n5pkzppjfuviz7k.sidestep,
    advance: $_4n5pkzppjfuviz7k.advance
  };

  var universe$2 = DomUniverse();
  var gather = function (element, prune, transform) {
    return $_1i6gx7pnjfuviz7c.gather(universe$2, element, prune, transform);
  };
  var before$3 = function (element, isRoot) {
    return $_1i6gx7pnjfuviz7c.before(universe$2, element, isRoot);
  };
  var after$4 = function (element, isRoot) {
    return $_1i6gx7pnjfuviz7c.after(universe$2, element, isRoot);
  };
  var seekLeft$1 = function (element, predicate, isRoot) {
    return $_1i6gx7pnjfuviz7c.seekLeft(universe$2, element, predicate, isRoot);
  };
  var seekRight$1 = function (element, predicate, isRoot) {
    return $_1i6gx7pnjfuviz7c.seekRight(universe$2, element, predicate, isRoot);
  };
  var walkers$1 = function () {
    return $_1i6gx7pnjfuviz7c.walkers();
  };
  var walk$1 = function (item, mode, direction, _rules) {
    return $_1i6gx7pnjfuviz7c.walk(universe$2, item, mode, direction, _rules);
  };
  var $_9y9zempmjfuviz78 = {
    gather: gather,
    before: before$3,
    after: after$4,
    seekLeft: seekLeft$1,
    seekRight: seekRight$1,
    walkers: walkers$1,
    walk: walk$1
  };

  var JUMP_SIZE = 5;
  var NUM_RETRIES = 100;
  var adt$2 = $_27harem7jfuviyb3.generate([
    { 'none': [] },
    { 'retry': ['caret'] }
  ]);
  var isOutside = function (caret, box) {
    return caret.left() < box.left() || Math.abs(box.right() - caret.left()) < 1 || caret.left() > box.right();
  };
  var inOutsideBlock = function (bridge, element, caret) {
    return $_eg4f87lbjfuviy37.closest(element, $_fkl6wvmrjfuviyg2.isBlock).fold($_fdch7uk7jfuvixxb.constant(false), function (cell) {
      return $_3hp2alpkjfuviz6e.getEntireBox(bridge, cell).exists(function (box) {
        return isOutside(caret, box);
      });
    });
  };
  var adjustDown = function (bridge, element, guessBox, original, caret) {
    var lowerCaret = $_an24q2pjjfuviz6a.moveDown(caret, JUMP_SIZE);
    if (Math.abs(guessBox.bottom() - original.bottom()) < 1)
      return adt$2.retry(lowerCaret);
    else if (guessBox.top() > caret.bottom())
      return adt$2.retry(lowerCaret);
    else if (guessBox.top() === caret.bottom())
      return adt$2.retry($_an24q2pjjfuviz6a.moveDown(caret, 1));
    else
      return inOutsideBlock(bridge, element, caret) ? adt$2.retry($_an24q2pjjfuviz6a.translate(lowerCaret, JUMP_SIZE, 0)) : adt$2.none();
  };
  var adjustUp = function (bridge, element, guessBox, original, caret) {
    var higherCaret = $_an24q2pjjfuviz6a.moveUp(caret, JUMP_SIZE);
    if (Math.abs(guessBox.top() - original.top()) < 1)
      return adt$2.retry(higherCaret);
    else if (guessBox.bottom() < caret.top())
      return adt$2.retry(higherCaret);
    else if (guessBox.bottom() === caret.top())
      return adt$2.retry($_an24q2pjjfuviz6a.moveUp(caret, 1));
    else
      return inOutsideBlock(bridge, element, caret) ? adt$2.retry($_an24q2pjjfuviz6a.translate(higherCaret, JUMP_SIZE, 0)) : adt$2.none();
  };
  var upMovement = {
    point: $_an24q2pjjfuviz6a.getTop,
    adjuster: adjustUp,
    move: $_an24q2pjjfuviz6a.moveUp,
    gather: $_9y9zempmjfuviz78.before
  };
  var downMovement = {
    point: $_an24q2pjjfuviz6a.getBottom,
    adjuster: adjustDown,
    move: $_an24q2pjjfuviz6a.moveDown,
    gather: $_9y9zempmjfuviz78.after
  };
  var isAtTable = function (bridge, x, y) {
    return bridge.elementFromPoint(x, y).filter(function (elm) {
      return $_6mcqmml6jfuviy2u.name(elm) === 'table';
    }).isSome();
  };
  var adjustForTable = function (bridge, movement, original, caret, numRetries) {
    return adjustTil(bridge, movement, original, movement.move(caret, JUMP_SIZE), numRetries);
  };
  var adjustTil = function (bridge, movement, original, caret, numRetries) {
    if (numRetries === 0)
      return Option.some(caret);
    if (isAtTable(bridge, caret.left(), movement.point(caret)))
      return adjustForTable(bridge, movement, original, caret, numRetries - 1);
    return bridge.situsFromPoint(caret.left(), movement.point(caret)).bind(function (guess) {
      return guess.start().fold(Option.none, function (element, offset) {
        return $_3hp2alpkjfuviz6e.getEntireBox(bridge, element, offset).bind(function (guessBox) {
          return movement.adjuster(bridge, element, guessBox, original, caret).fold(Option.none, function (newCaret) {
            return adjustTil(bridge, movement, original, newCaret, numRetries - 1);
          });
        }).orThunk(function () {
          return Option.some(caret);
        });
      }, Option.none);
    });
  };
  var ieTryDown = function (bridge, caret) {
    return bridge.situsFromPoint(caret.left(), caret.bottom() + JUMP_SIZE);
  };
  var ieTryUp = function (bridge, caret) {
    return bridge.situsFromPoint(caret.left(), caret.top() - JUMP_SIZE);
  };
  var checkScroll = function (movement, adjusted, bridge) {
    if (movement.point(adjusted) > bridge.getInnerHeight())
      return Option.some(movement.point(adjusted) - bridge.getInnerHeight());
    else if (movement.point(adjusted) < 0)
      return Option.some(-movement.point(adjusted));
    else
      return Option.none();
  };
  var retry = function (movement, bridge, caret) {
    var moved = movement.move(caret, JUMP_SIZE);
    var adjusted = adjustTil(bridge, movement, caret, moved, NUM_RETRIES).getOr(moved);
    return checkScroll(movement, adjusted, bridge).fold(function () {
      return bridge.situsFromPoint(adjusted.left(), movement.point(adjusted));
    }, function (delta) {
      bridge.scrollBy(0, delta);
      return bridge.situsFromPoint(adjusted.left(), movement.point(adjusted) - delta);
    });
  };
  var $_7hk4s3pljfuviz6m = {
    tryUp: $_fdch7uk7jfuvixxb.curry(retry, upMovement),
    tryDown: $_fdch7uk7jfuvixxb.curry(retry, downMovement),
    ieTryUp: ieTryUp,
    ieTryDown: ieTryDown,
    getJumpSize: $_fdch7uk7jfuvixxb.constant(JUMP_SIZE)
  };

  var adt$3 = $_27harem7jfuviyb3.generate([
    { 'none': ['message'] },
    { 'success': [] },
    { 'failedUp': ['cell'] },
    { 'failedDown': ['cell'] }
  ]);
  var isOverlapping = function (bridge, before, after) {
    var beforeBounds = bridge.getRect(before);
    var afterBounds = bridge.getRect(after);
    return afterBounds.right > beforeBounds.left && afterBounds.left < beforeBounds.right;
  };
  var verify = function (bridge, before, beforeOffset, after, afterOffset, failure, isRoot) {
    return $_26gnp6lajfuviy35.closest(after, 'td,th', isRoot).bind(function (afterCell) {
      return $_26gnp6lajfuviy35.closest(before, 'td,th', isRoot).map(function (beforeCell) {
        if (!$_g6ztqikojfuviy13.eq(afterCell, beforeCell)) {
          return $_1tw8e1lujfuviy7z.sharedOne(isRow, [
            afterCell,
            beforeCell
          ]).fold(function () {
            return isOverlapping(bridge, beforeCell, afterCell) ? adt$3.success() : failure(beforeCell);
          }, function (sharedRow) {
            return failure(beforeCell);
          });
        } else {
          return $_g6ztqikojfuviy13.eq(after, afterCell) && $_cj2m48lmjfuviy5u.getEnd(afterCell) === afterOffset ? failure(beforeCell) : adt$3.none('in same cell');
        }
      });
    }).getOr(adt$3.none('default'));
  };
  var isRow = function (elem) {
    return $_26gnp6lajfuviy35.closest(elem, 'tr');
  };
  var cata$2 = function (subject, onNone, onSuccess, onFailedUp, onFailedDown) {
    return subject.fold(onNone, onSuccess, onFailedUp, onFailedDown);
  };
  var $_fyt9q7prjfuviz7w = {
    verify: verify,
    cata: cata$2,
    adt: adt$3
  };

  var point = $_96oqrskbjfuvixya.immutable('element', 'offset');
  var delta = $_96oqrskbjfuvixya.immutable('element', 'deltaOffset');
  var range$3 = $_96oqrskbjfuvixya.immutable('element', 'start', 'finish');
  var points = $_96oqrskbjfuvixya.immutable('begin', 'end');
  var text = $_96oqrskbjfuvixya.immutable('element', 'text');
  var $_5ry9ycptjfuviz8v = {
    point: point,
    delta: delta,
    range: range$3,
    points: points,
    text: text
  };

  var inAncestor = $_96oqrskbjfuvixya.immutable('ancestor', 'descendants', 'element', 'index');
  var inParent = $_96oqrskbjfuvixya.immutable('parent', 'children', 'element', 'index');
  var childOf = function (element, ancestor) {
    return $_eg4f87lbjfuviy37.closest(element, function (elem) {
      return $_87w3h3kmjfuviy0m.parent(elem).exists(function (parent) {
        return $_g6ztqikojfuviy13.eq(parent, ancestor);
      });
    });
  };
  var indexInParent = function (element) {
    return $_87w3h3kmjfuviy0m.parent(element).bind(function (parent) {
      var children = $_87w3h3kmjfuviy0m.children(parent);
      return indexOf$1(children, element).map(function (index) {
        return inParent(parent, children, element, index);
      });
    });
  };
  var indexOf$1 = function (elements, element) {
    return $_4jja6kk5jfuvixx1.findIndex(elements, $_fdch7uk7jfuvixxb.curry($_g6ztqikojfuviy13.eq, element));
  };
  var selectorsInParent = function (element, selector) {
    return $_87w3h3kmjfuviy0m.parent(element).bind(function (parent) {
      var children = $_a3hs1bl7jfuviy2w.children(parent, selector);
      return indexOf$1(children, element).map(function (index) {
        return inParent(parent, children, element, index);
      });
    });
  };
  var descendantsInAncestor = function (element, ancestorSelector, descendantSelector) {
    return $_26gnp6lajfuviy35.closest(element, ancestorSelector).bind(function (ancestor) {
      var descendants = $_a3hs1bl7jfuviy2w.descendants(ancestor, descendantSelector);
      return indexOf$1(descendants, element).map(function (index) {
        return inAncestor(ancestor, descendants, element, index);
      });
    });
  };
  var $_e2sgp0pujfuviz8z = {
    childOf: childOf,
    indexOf: indexOf$1,
    indexInParent: indexInParent,
    selectorsInParent: selectorsInParent,
    descendantsInAncestor: descendantsInAncestor
  };

  var isBr = function (elem) {
    return $_6mcqmml6jfuviy2u.name(elem) === 'br';
  };
  var gatherer = function (cand, gather, isRoot) {
    return gather(cand, isRoot).bind(function (target) {
      return $_6mcqmml6jfuviy2u.isText(target) && $_cvagqflnjfuviy5y.get(target).trim().length === 0 ? gatherer(target, gather, isRoot) : Option.some(target);
    });
  };
  var handleBr = function (isRoot, element, direction) {
    return direction.traverse(element).orThunk(function () {
      return gatherer(element, direction.gather, isRoot);
    }).map(direction.relative);
  };
  var findBr = function (element, offset) {
    return $_87w3h3kmjfuviy0m.child(element, offset).filter(isBr).orThunk(function () {
      return $_87w3h3kmjfuviy0m.child(element, offset - 1).filter(isBr);
    });
  };
  var handleParent = function (isRoot, element, offset, direction) {
    return findBr(element, offset).bind(function (br) {
      return direction.traverse(br).fold(function () {
        return gatherer(br, direction.gather, isRoot).map(direction.relative);
      }, function (adjacent) {
        return $_e2sgp0pujfuviz8z.indexInParent(adjacent).map(function (info) {
          return $_8a4rxdovjfuviz0c.on(info.parent(), info.index());
        });
      });
    });
  };
  var tryBr = function (isRoot, element, offset, direction) {
    var target = isBr(element) ? handleBr(isRoot, element, direction) : handleParent(isRoot, element, offset, direction);
    return target.map(function (tgt) {
      return {
        start: $_fdch7uk7jfuvixxb.constant(tgt),
        finish: $_fdch7uk7jfuvixxb.constant(tgt)
      };
    });
  };
  var process = function (analysis) {
    return $_fyt9q7prjfuviz7w.cata(analysis, function (message) {
      return Option.none();
    }, function () {
      return Option.none();
    }, function (cell) {
      return Option.some($_5ry9ycptjfuviz8v.point(cell, 0));
    }, function (cell) {
      return Option.some($_5ry9ycptjfuviz8v.point(cell, $_cj2m48lmjfuviy5u.getEnd(cell)));
    });
  };
  var $_b2f2ygpsjfuviz89 = {
    tryBr: tryBr,
    process: process
  };

  var MAX_RETRIES = 20;
  var platform$1 = $_8chrc7ktjfuviy1m.detect();
  var findSpot = function (bridge, isRoot, direction) {
    return bridge.getSelection().bind(function (sel) {
      return $_b2f2ygpsjfuviz89.tryBr(isRoot, sel.finish(), sel.foffset(), direction).fold(function () {
        return Option.some($_5ry9ycptjfuviz8v.point(sel.finish(), sel.foffset()));
      }, function (brNeighbour) {
        var range = bridge.fromSitus(brNeighbour);
        var analysis = $_fyt9q7prjfuviz7w.verify(bridge, sel.finish(), sel.foffset(), range.finish(), range.foffset(), direction.failure, isRoot);
        return $_b2f2ygpsjfuviz89.process(analysis);
      });
    });
  };
  var scan = function (bridge, isRoot, element, offset, direction, numRetries) {
    if (numRetries === 0)
      return Option.none();
    return tryCursor(bridge, isRoot, element, offset, direction).bind(function (situs) {
      var range = bridge.fromSitus(situs);
      var analysis = $_fyt9q7prjfuviz7w.verify(bridge, element, offset, range.finish(), range.foffset(), direction.failure, isRoot);
      return $_fyt9q7prjfuviz7w.cata(analysis, function () {
        return Option.none();
      }, function () {
        return Option.some(situs);
      }, function (cell) {
        if ($_g6ztqikojfuviy13.eq(element, cell) && offset === 0)
          return tryAgain(bridge, element, offset, $_an24q2pjjfuviz6a.moveUp, direction);
        else
          return scan(bridge, isRoot, cell, 0, direction, numRetries - 1);
      }, function (cell) {
        if ($_g6ztqikojfuviy13.eq(element, cell) && offset === $_cj2m48lmjfuviy5u.getEnd(cell))
          return tryAgain(bridge, element, offset, $_an24q2pjjfuviz6a.moveDown, direction);
        else
          return scan(bridge, isRoot, cell, $_cj2m48lmjfuviy5u.getEnd(cell), direction, numRetries - 1);
      });
    });
  };
  var tryAgain = function (bridge, element, offset, move, direction) {
    return $_3hp2alpkjfuviz6e.getBoxAt(bridge, element, offset).bind(function (box) {
      return tryAt(bridge, direction, move(box, $_7hk4s3pljfuviz6m.getJumpSize()));
    });
  };
  var tryAt = function (bridge, direction, box) {
    if (platform$1.browser.isChrome() || platform$1.browser.isSafari() || platform$1.browser.isFirefox() || platform$1.browser.isEdge())
      return direction.otherRetry(bridge, box);
    else if (platform$1.browser.isIE())
      return direction.ieRetry(bridge, box);
    else
      return Option.none();
  };
  var tryCursor = function (bridge, isRoot, element, offset, direction) {
    return $_3hp2alpkjfuviz6e.getBoxAt(bridge, element, offset).bind(function (box) {
      return tryAt(bridge, direction, box);
    });
  };
  var handle$2 = function (bridge, isRoot, direction) {
    return findSpot(bridge, isRoot, direction).bind(function (spot) {
      return scan(bridge, isRoot, spot.element(), spot.offset(), direction, MAX_RETRIES).map(bridge.fromSitus);
    });
  };
  var $_ga1yakpijfuviz5w = { handle: handle$2 };

  var any$1 = function (predicate) {
    return $_eg4f87lbjfuviy37.first(predicate).isSome();
  };
  var ancestor$3 = function (scope, predicate, isRoot) {
    return $_eg4f87lbjfuviy37.ancestor(scope, predicate, isRoot).isSome();
  };
  var closest$3 = function (scope, predicate, isRoot) {
    return $_eg4f87lbjfuviy37.closest(scope, predicate, isRoot).isSome();
  };
  var sibling$3 = function (scope, predicate) {
    return $_eg4f87lbjfuviy37.sibling(scope, predicate).isSome();
  };
  var child$4 = function (scope, predicate) {
    return $_eg4f87lbjfuviy37.child(scope, predicate).isSome();
  };
  var descendant$3 = function (scope, predicate) {
    return $_eg4f87lbjfuviy37.descendant(scope, predicate).isSome();
  };
  var $_ajfnsppvjfuviz9c = {
    any: any$1,
    ancestor: ancestor$3,
    closest: closest$3,
    sibling: sibling$3,
    child: child$4,
    descendant: descendant$3
  };

  var detection = $_8chrc7ktjfuviy1m.detect();
  var inSameTable = function (elem, table) {
    return $_ajfnsppvjfuviz9c.ancestor(elem, function (e) {
      return $_87w3h3kmjfuviy0m.parent(e).exists(function (p) {
        return $_g6ztqikojfuviy13.eq(p, table);
      });
    });
  };
  var simulate = function (bridge, isRoot, direction, initial, anchor) {
    return $_26gnp6lajfuviy35.closest(initial, 'td,th', isRoot).bind(function (start) {
      return $_26gnp6lajfuviy35.closest(start, 'table', isRoot).bind(function (table) {
        if (!inSameTable(anchor, table))
          return Option.none();
        return $_ga1yakpijfuviz5w.handle(bridge, isRoot, direction).bind(function (range) {
          return $_26gnp6lajfuviy35.closest(range.finish(), 'td,th', isRoot).map(function (finish) {
            return {
              start: $_fdch7uk7jfuvixxb.constant(start),
              finish: $_fdch7uk7jfuvixxb.constant(finish),
              range: $_fdch7uk7jfuvixxb.constant(range)
            };
          });
        });
      });
    });
  };
  var navigate = function (bridge, isRoot, direction, initial, anchor, precheck) {
    if (detection.browser.isIE()) {
      return Option.none();
    } else {
      return precheck(initial, isRoot).orThunk(function () {
        return simulate(bridge, isRoot, direction, initial, anchor).map(function (info) {
          var range = info.range();
          return $_8nrj5cpbjfuviz3g.response(Option.some($_ehlw1upejfuviz45.makeSitus(range.start(), range.soffset(), range.finish(), range.foffset())), true);
        });
      });
    }
  };
  var firstUpCheck = function (initial, isRoot) {
    return $_26gnp6lajfuviy35.closest(initial, 'tr', isRoot).bind(function (startRow) {
      return $_26gnp6lajfuviy35.closest(startRow, 'table', isRoot).bind(function (table) {
        var rows = $_a3hs1bl7jfuviy2w.descendants(table, 'tr');
        if ($_g6ztqikojfuviy13.eq(startRow, rows[0])) {
          return $_9y9zempmjfuviz78.seekLeft(table, function (element) {
            return $_dsfijblljfuviy5q.last(element).isSome();
          }, isRoot).map(function (last) {
            var lastOffset = $_cj2m48lmjfuviy5u.getEnd(last);
            return $_8nrj5cpbjfuviz3g.response(Option.some($_ehlw1upejfuviz45.makeSitus(last, lastOffset, last, lastOffset)), true);
          });
        } else {
          return Option.none();
        }
      });
    });
  };
  var lastDownCheck = function (initial, isRoot) {
    return $_26gnp6lajfuviy35.closest(initial, 'tr', isRoot).bind(function (startRow) {
      return $_26gnp6lajfuviy35.closest(startRow, 'table', isRoot).bind(function (table) {
        var rows = $_a3hs1bl7jfuviy2w.descendants(table, 'tr');
        if ($_g6ztqikojfuviy13.eq(startRow, rows[rows.length - 1])) {
          return $_9y9zempmjfuviz78.seekRight(table, function (element) {
            return $_dsfijblljfuviy5q.first(element).isSome();
          }, isRoot).map(function (first) {
            return $_8nrj5cpbjfuviz3g.response(Option.some($_ehlw1upejfuviz45.makeSitus(first, 0, first, 0)), true);
          });
        } else {
          return Option.none();
        }
      });
    });
  };
  var select = function (bridge, container, isRoot, direction, initial, anchor, selectRange) {
    return simulate(bridge, isRoot, direction, initial, anchor).bind(function (info) {
      return $_9sykywpgjfuviz4r.detect(container, isRoot, info.start(), info.finish(), selectRange);
    });
  };
  var $_bnyc49phjfuviz55 = {
    navigate: navigate,
    select: select,
    firstUpCheck: firstUpCheck,
    lastDownCheck: lastDownCheck
  };

  var findCell = function (target, isRoot) {
    return $_26gnp6lajfuviy35.closest(target, 'td,th', isRoot);
  };
  function MouseSelection (bridge, container, isRoot, annotations) {
    var cursor = Option.none();
    var clearState = function () {
      cursor = Option.none();
    };
    var mousedown = function (event) {
      annotations.clear(container);
      cursor = findCell(event.target(), isRoot);
    };
    var mouseover = function (event) {
      cursor.each(function (start) {
        annotations.clear(container);
        findCell(event.target(), isRoot).each(function (finish) {
          $_8vwxtkltjfuviy76.identify(start, finish, isRoot).each(function (cellSel) {
            var boxes = cellSel.boxes().getOr([]);
            if (boxes.length > 1 || boxes.length === 1 && !$_g6ztqikojfuviy13.eq(start, finish)) {
              annotations.selectRange(container, boxes, cellSel.start(), cellSel.finish());
              bridge.selectContents(finish);
            }
          });
        });
      });
    };
    var mouseup = function () {
      cursor.each(clearState);
    };
    return {
      mousedown: mousedown,
      mouseover: mouseover,
      mouseup: mouseup
    };
  }

  var $_e8neeppxjfuviz9m = {
    down: {
      traverse: $_87w3h3kmjfuviy0m.nextSibling,
      gather: $_9y9zempmjfuviz78.after,
      relative: $_8a4rxdovjfuviz0c.before,
      otherRetry: $_7hk4s3pljfuviz6m.tryDown,
      ieRetry: $_7hk4s3pljfuviz6m.ieTryDown,
      failure: $_fyt9q7prjfuviz7w.adt.failedDown
    },
    up: {
      traverse: $_87w3h3kmjfuviy0m.prevSibling,
      gather: $_9y9zempmjfuviz78.before,
      relative: $_8a4rxdovjfuviz0c.before,
      otherRetry: $_7hk4s3pljfuviz6m.tryUp,
      ieRetry: $_7hk4s3pljfuviz6m.ieTryUp,
      failure: $_fyt9q7prjfuviz7w.adt.failedUp
    }
  };

  var rc = $_96oqrskbjfuvixya.immutable('rows', 'cols');
  var mouse = function (win, container, isRoot, annotations) {
    var bridge = WindowBridge(win);
    var handlers = MouseSelection(bridge, container, isRoot, annotations);
    return {
      mousedown: handlers.mousedown,
      mouseover: handlers.mouseover,
      mouseup: handlers.mouseup
    };
  };
  var keyboard = function (win, container, isRoot, annotations) {
    var bridge = WindowBridge(win);
    var clearToNavigate = function () {
      annotations.clear(container);
      return Option.none();
    };
    var keydown = function (event, start, soffset, finish, foffset, direction) {
      var keycode = event.raw().which;
      var shiftKey = event.raw().shiftKey === true;
      var handler = $_8vwxtkltjfuviy76.retrieve(container, annotations.selectedSelector()).fold(function () {
        if ($_668mt4pcjfuviz3j.isDown(keycode) && shiftKey) {
          return $_fdch7uk7jfuvixxb.curry($_bnyc49phjfuviz55.select, bridge, container, isRoot, $_e8neeppxjfuviz9m.down, finish, start, annotations.selectRange);
        } else if ($_668mt4pcjfuviz3j.isUp(keycode) && shiftKey) {
          return $_fdch7uk7jfuvixxb.curry($_bnyc49phjfuviz55.select, bridge, container, isRoot, $_e8neeppxjfuviz9m.up, finish, start, annotations.selectRange);
        } else if ($_668mt4pcjfuviz3j.isDown(keycode)) {
          return $_fdch7uk7jfuvixxb.curry($_bnyc49phjfuviz55.navigate, bridge, isRoot, $_e8neeppxjfuviz9m.down, finish, start, $_bnyc49phjfuviz55.lastDownCheck);
        } else if ($_668mt4pcjfuviz3j.isUp(keycode)) {
          return $_fdch7uk7jfuvixxb.curry($_bnyc49phjfuviz55.navigate, bridge, isRoot, $_e8neeppxjfuviz9m.up, finish, start, $_bnyc49phjfuviz55.firstUpCheck);
        } else {
          return Option.none;
        }
      }, function (selected) {
        var update = function (attempts) {
          return function () {
            var navigation = $_6i1r9dmvjfuviygw.findMap(attempts, function (delta) {
              return $_9sykywpgjfuviz4r.update(delta.rows(), delta.cols(), container, selected, annotations);
            });
            return navigation.fold(function () {
              return $_8vwxtkltjfuviy76.getEdges(container, annotations.firstSelectedSelector(), annotations.lastSelectedSelector()).map(function (edges) {
                var relative = $_668mt4pcjfuviz3j.isDown(keycode) || direction.isForward(keycode) ? $_8a4rxdovjfuviz0c.after : $_8a4rxdovjfuviz0c.before;
                bridge.setRelativeSelection($_8a4rxdovjfuviz0c.on(edges.first(), 0), relative(edges.table()));
                annotations.clear(container);
                return $_8nrj5cpbjfuviz3g.response(Option.none(), true);
              });
            }, function (_) {
              return Option.some($_8nrj5cpbjfuviz3g.response(Option.none(), true));
            });
          };
        };
        if ($_668mt4pcjfuviz3j.isDown(keycode) && shiftKey)
          return update([rc(+1, 0)]);
        else if ($_668mt4pcjfuviz3j.isUp(keycode) && shiftKey)
          return update([rc(-1, 0)]);
        else if (direction.isBackward(keycode) && shiftKey)
          return update([
            rc(0, -1),
            rc(-1, 0)
          ]);
        else if (direction.isForward(keycode) && shiftKey)
          return update([
            rc(0, +1),
            rc(+1, 0)
          ]);
        else if ($_668mt4pcjfuviz3j.isNavigation(keycode) && shiftKey === false)
          return clearToNavigate;
        else
          return Option.none;
      });
      return handler();
    };
    var keyup = function (event, start, soffset, finish, foffset) {
      return $_8vwxtkltjfuviy76.retrieve(container, annotations.selectedSelector()).fold(function () {
        var keycode = event.raw().which;
        var shiftKey = event.raw().shiftKey === true;
        if (shiftKey === false)
          return Option.none();
        if ($_668mt4pcjfuviz3j.isNavigation(keycode))
          return $_9sykywpgjfuviz4r.sync(container, isRoot, start, soffset, finish, foffset, annotations.selectRange);
        else
          return Option.none();
      }, Option.none);
    };
    return {
      keydown: keydown,
      keyup: keyup
    };
  };
  var $_4r5b80pajfuviz33 = {
    mouse: mouse,
    keyboard: keyboard
  };

  var add$3 = function (element, classes) {
    $_4jja6kk5jfuvixx1.each(classes, function (x) {
      $_c1zp6in6jfuviyjs.add(element, x);
    });
  };
  var remove$7 = function (element, classes) {
    $_4jja6kk5jfuvixx1.each(classes, function (x) {
      $_c1zp6in6jfuviyjs.remove(element, x);
    });
  };
  var toggle$2 = function (element, classes) {
    $_4jja6kk5jfuvixx1.each(classes, function (x) {
      $_c1zp6in6jfuviyjs.toggle(element, x);
    });
  };
  var hasAll = function (element, classes) {
    return $_4jja6kk5jfuvixx1.forall(classes, function (clazz) {
      return $_c1zp6in6jfuviyjs.has(element, clazz);
    });
  };
  var hasAny = function (element, classes) {
    return $_4jja6kk5jfuvixx1.exists(classes, function (clazz) {
      return $_c1zp6in6jfuviyjs.has(element, clazz);
    });
  };
  var getNative = function (element) {
    var classList = element.dom().classList;
    var r = new Array(classList.length);
    for (var i = 0; i < classList.length; i++) {
      r[i] = classList.item(i);
    }
    return r;
  };
  var get$11 = function (element) {
    return $_1jx5cbn8jfuviyjv.supports(element) ? getNative(element) : $_1jx5cbn8jfuviyjv.get(element);
  };
  var $_5j3f8uq0jfuviza8 = {
    add: add$3,
    remove: remove$7,
    toggle: toggle$2,
    hasAll: hasAll,
    hasAny: hasAny,
    get: get$11
  };

  var addClass = function (clazz) {
    return function (element) {
      $_c1zp6in6jfuviyjs.add(element, clazz);
    };
  };
  var removeClass = function (clazz) {
    return function (element) {
      $_c1zp6in6jfuviyjs.remove(element, clazz);
    };
  };
  var removeClasses = function (classes) {
    return function (element) {
      $_5j3f8uq0jfuviza8.remove(element, classes);
    };
  };
  var hasClass = function (clazz) {
    return function (element) {
      return $_c1zp6in6jfuviyjs.has(element, clazz);
    };
  };
  var $_ch4q4lpzjfuviza7 = {
    addClass: addClass,
    removeClass: removeClass,
    removeClasses: removeClasses,
    hasClass: hasClass
  };

  var byClass = function (ephemera) {
    var addSelectionClass = $_ch4q4lpzjfuviza7.addClass(ephemera.selected());
    var removeSelectionClasses = $_ch4q4lpzjfuviza7.removeClasses([
      ephemera.selected(),
      ephemera.lastSelected(),
      ephemera.firstSelected()
    ]);
    var clear = function (container) {
      var sels = $_a3hs1bl7jfuviy2w.descendants(container, ephemera.selectedSelector());
      $_4jja6kk5jfuvixx1.each(sels, removeSelectionClasses);
    };
    var selectRange = function (container, cells, start, finish) {
      clear(container);
      $_4jja6kk5jfuvixx1.each(cells, addSelectionClass);
      $_c1zp6in6jfuviyjs.add(start, ephemera.firstSelected());
      $_c1zp6in6jfuviyjs.add(finish, ephemera.lastSelected());
    };
    return {
      clear: clear,
      selectRange: selectRange,
      selectedSelector: ephemera.selectedSelector,
      firstSelectedSelector: ephemera.firstSelectedSelector,
      lastSelectedSelector: ephemera.lastSelectedSelector
    };
  };
  var byAttr = function (ephemera) {
    var removeSelectionAttributes = function (element) {
      $_2ekobel5jfuviy2m.remove(element, ephemera.selected());
      $_2ekobel5jfuviy2m.remove(element, ephemera.firstSelected());
      $_2ekobel5jfuviy2m.remove(element, ephemera.lastSelected());
    };
    var addSelectionAttribute = function (element) {
      $_2ekobel5jfuviy2m.set(element, ephemera.selected(), '1');
    };
    var clear = function (container) {
      var sels = $_a3hs1bl7jfuviy2w.descendants(container, ephemera.selectedSelector());
      $_4jja6kk5jfuvixx1.each(sels, removeSelectionAttributes);
    };
    var selectRange = function (container, cells, start, finish) {
      clear(container);
      $_4jja6kk5jfuvixx1.each(cells, addSelectionAttribute);
      $_2ekobel5jfuviy2m.set(start, ephemera.firstSelected(), '1');
      $_2ekobel5jfuviy2m.set(finish, ephemera.lastSelected(), '1');
    };
    return {
      clear: clear,
      selectRange: selectRange,
      selectedSelector: ephemera.selectedSelector,
      firstSelectedSelector: ephemera.firstSelectedSelector,
      lastSelectedSelector: ephemera.lastSelectedSelector
    };
  };
  var $_ajwy5tpyjfuviz9u = {
    byClass: byClass,
    byAttr: byAttr
  };

  function CellSelection$1 (editor, lazyResize) {
    var handlerStruct = $_96oqrskbjfuvixya.immutableBag([
      'mousedown',
      'mouseover',
      'mouseup',
      'keyup',
      'keydown'
    ], []);
    var handlers = Option.none();
    var annotations = $_ajwy5tpyjfuviz9u.byAttr($_g37vw7m5jfuviyax);
    editor.on('init', function (e) {
      var win = editor.getWin();
      var body = $_aheu0fnnjfuviymj.getBody(editor);
      var isRoot = $_aheu0fnnjfuviymj.getIsRoot(editor);
      var syncSelection = function () {
        var sel = editor.selection;
        var start = $_4sdhm4kkjfuviy0e.fromDom(sel.getStart());
        var end = $_4sdhm4kkjfuviy0e.fromDom(sel.getEnd());
        var startTable = $_dmqxswkhjfuvixyz.table(start);
        var endTable = $_dmqxswkhjfuvixyz.table(end);
        var sameTable = startTable.bind(function (tableStart) {
          return endTable.bind(function (tableEnd) {
            return $_g6ztqikojfuviy13.eq(tableStart, tableEnd) ? Option.some(true) : Option.none();
          });
        });
        sameTable.fold(function () {
          annotations.clear(body);
        }, $_fdch7uk7jfuvixxb.noop);
      };
      var mouseHandlers = $_4r5b80pajfuviz33.mouse(win, body, isRoot, annotations);
      var keyHandlers = $_4r5b80pajfuviz33.keyboard(win, body, isRoot, annotations);
      var hasShiftKey = function (event) {
        return event.raw().shiftKey === true;
      };
      var handleResponse = function (event, response) {
        if (!hasShiftKey(event)) {
          return;
        }
        if (response.kill()) {
          event.kill();
        }
        response.selection().each(function (ns) {
          var relative = $_cirtsuoujfuviz06.relative(ns.start(), ns.finish());
          var rng = $_6ufympp0jfuviz14.asLtrRange(win, relative);
          editor.selection.setRng(rng);
        });
      };
      var keyup = function (event) {
        var wrappedEvent = wrapEvent(event);
        if (wrappedEvent.raw().shiftKey && $_668mt4pcjfuviz3j.isNavigation(wrappedEvent.raw().which)) {
          var rng = editor.selection.getRng();
          var start = $_4sdhm4kkjfuviy0e.fromDom(rng.startContainer);
          var end = $_4sdhm4kkjfuviy0e.fromDom(rng.endContainer);
          keyHandlers.keyup(wrappedEvent, start, rng.startOffset, end, rng.endOffset).each(function (response) {
            handleResponse(wrappedEvent, response);
          });
        }
      };
      var keydown = function (event) {
        var wrappedEvent = wrapEvent(event);
        lazyResize().each(function (resize) {
          resize.hideBars();
        });
        var rng = editor.selection.getRng();
        var startContainer = $_4sdhm4kkjfuviy0e.fromDom(editor.selection.getStart());
        var start = $_4sdhm4kkjfuviy0e.fromDom(rng.startContainer);
        var end = $_4sdhm4kkjfuviy0e.fromDom(rng.endContainer);
        var direction = $_1g25rnojfuviymn.directionAt(startContainer).isRtl() ? $_668mt4pcjfuviz3j.rtl : $_668mt4pcjfuviz3j.ltr;
        keyHandlers.keydown(wrappedEvent, start, rng.startOffset, end, rng.endOffset, direction).each(function (response) {
          handleResponse(wrappedEvent, response);
        });
        lazyResize().each(function (resize) {
          resize.showBars();
        });
      };
      var isMouseEvent = function (event) {
        return event.hasOwnProperty('x') && event.hasOwnProperty('y');
      };
      var wrapEvent = function (event) {
        var target = $_4sdhm4kkjfuviy0e.fromDom(event.target);
        var stop = function () {
          event.stopPropagation();
        };
        var prevent = function () {
          event.preventDefault();
        };
        var kill = $_fdch7uk7jfuvixxb.compose(prevent, stop);
        return {
          target: $_fdch7uk7jfuvixxb.constant(target),
          x: $_fdch7uk7jfuvixxb.constant(isMouseEvent(event) ? event.x : null),
          y: $_fdch7uk7jfuvixxb.constant(isMouseEvent(event) ? event.y : null),
          stop: stop,
          prevent: prevent,
          kill: kill,
          raw: $_fdch7uk7jfuvixxb.constant(event)
        };
      };
      var isLeftMouse = function (raw) {
        return raw.button === 0;
      };
      var isLeftButtonPressed = function (raw) {
        if (raw.buttons === undefined) {
          return true;
        }
        return (raw.buttons & 1) !== 0;
      };
      var mouseDown = function (e) {
        if (isLeftMouse(e)) {
          mouseHandlers.mousedown(wrapEvent(e));
        }
      };
      var mouseOver = function (e) {
        if (isLeftButtonPressed(e)) {
          mouseHandlers.mouseover(wrapEvent(e));
        }
      };
      var mouseUp = function (e) {
        if (isLeftMouse(e)) {
          mouseHandlers.mouseup(wrapEvent(e));
        }
      };
      editor.on('mousedown', mouseDown);
      editor.on('mouseover', mouseOver);
      editor.on('mouseup', mouseUp);
      editor.on('keyup', keyup);
      editor.on('keydown', keydown);
      editor.on('nodechange', syncSelection);
      handlers = Option.some(handlerStruct({
        mousedown: mouseDown,
        mouseover: mouseOver,
        mouseup: mouseUp,
        keyup: keyup,
        keydown: keydown
      }));
    });
    var destroy = function () {
      handlers.each(function (handlers) {
      });
    };
    return {
      clear: annotations.clear,
      destroy: destroy
    };
  }

  var Selections = function (editor) {
    var get = function () {
      var body = $_aheu0fnnjfuviymj.getBody(editor);
      return $_7o7mthlsjfuviy6v.retrieve(body, $_g37vw7m5jfuviyax.selectedSelector()).fold(function () {
        if (editor.selection.getStart() === undefined) {
          return $_1jmyeim6jfuviyb0.none();
        } else {
          return $_1jmyeim6jfuviyb0.single(editor.selection);
        }
      }, function (cells) {
        return $_1jmyeim6jfuviyb0.multiple(cells);
      });
    };
    return { get: get };
  };

  var each$4 = global$2.each;
  var addButtons = function (editor) {
    var menuItems = [];
    each$4('inserttable tableprops deletetable | cell row column'.split(' '), function (name) {
      if (name === '|') {
        menuItems.push({ text: '-' });
      } else {
        menuItems.push(editor.menuItems[name]);
      }
    });
    editor.addButton('table', {
      type: 'menubutton',
      title: 'Table',
      menu: menuItems
    });
    function cmd(command) {
      return function () {
        editor.execCommand(command);
      };
    }
    editor.addButton('tableprops', {
      title: 'Table properties',
      onclick: $_fdch7uk7jfuvixxb.curry($_53k9g0nzjfuviyoc.open, editor, true),
      icon: 'table'
    });
    editor.addButton('tabledelete', {
      title: 'Delete table',
      onclick: cmd('mceTableDelete')
    });
    editor.addButton('tablecellprops', {
      title: 'Cell properties',
      onclick: cmd('mceTableCellProps')
    });
    editor.addButton('tablemergecells', {
      title: 'Merge cells',
      onclick: cmd('mceTableMergeCells')
    });
    editor.addButton('tablesplitcells', {
      title: 'Split cell',
      onclick: cmd('mceTableSplitCells')
    });
    editor.addButton('tableinsertrowbefore', {
      title: 'Insert row before',
      onclick: cmd('mceTableInsertRowBefore')
    });
    editor.addButton('tableinsertrowafter', {
      title: 'Insert row after',
      onclick: cmd('mceTableInsertRowAfter')
    });
    editor.addButton('tabledeleterow', {
      title: 'Delete row',
      onclick: cmd('mceTableDeleteRow')
    });
    editor.addButton('tablerowprops', {
      title: 'Row properties',
      onclick: cmd('mceTableRowProps')
    });
    editor.addButton('tablecutrow', {
      title: 'Cut row',
      onclick: cmd('mceTableCutRow')
    });
    editor.addButton('tablecopyrow', {
      title: 'Copy row',
      onclick: cmd('mceTableCopyRow')
    });
    editor.addButton('tablepasterowbefore', {
      title: 'Paste row before',
      onclick: cmd('mceTablePasteRowBefore')
    });
    editor.addButton('tablepasterowafter', {
      title: 'Paste row after',
      onclick: cmd('mceTablePasteRowAfter')
    });
    editor.addButton('tableinsertcolbefore', {
      title: 'Insert column before',
      onclick: cmd('mceTableInsertColBefore')
    });
    editor.addButton('tableinsertcolafter', {
      title: 'Insert column after',
      onclick: cmd('mceTableInsertColAfter')
    });
    editor.addButton('tabledeletecol', {
      title: 'Delete column',
      onclick: cmd('mceTableDeleteCol')
    });
  };
  var addToolbars = function (editor) {
    var isTable = function (table) {
      var selectorMatched = editor.dom.is(table, 'table') && editor.getBody().contains(table);
      return selectorMatched;
    };
    var toolbar = getToolbar(editor);
    if (toolbar.length > 0) {
      editor.addContextToolbar(isTable, toolbar.join(' '));
    }
  };
  var $_6ddytyq2jfuvizah = {
    addButtons: addButtons,
    addToolbars: addToolbars
  };

  var addMenuItems = function (editor, selections) {
    var targets = Option.none();
    var tableCtrls = [];
    var cellCtrls = [];
    var mergeCtrls = [];
    var unmergeCtrls = [];
    var noTargetDisable = function (ctrl) {
      ctrl.disabled(true);
    };
    var ctrlEnable = function (ctrl) {
      ctrl.disabled(false);
    };
    var pushTable = function () {
      var self = this;
      tableCtrls.push(self);
      targets.fold(function () {
        noTargetDisable(self);
      }, function (targets) {
        ctrlEnable(self);
      });
    };
    var pushCell = function () {
      var self = this;
      cellCtrls.push(self);
      targets.fold(function () {
        noTargetDisable(self);
      }, function (targets) {
        ctrlEnable(self);
      });
    };
    var pushMerge = function () {
      var self = this;
      mergeCtrls.push(self);
      targets.fold(function () {
        noTargetDisable(self);
      }, function (targets) {
        self.disabled(targets.mergable().isNone());
      });
    };
    var pushUnmerge = function () {
      var self = this;
      unmergeCtrls.push(self);
      targets.fold(function () {
        noTargetDisable(self);
      }, function (targets) {
        self.disabled(targets.unmergable().isNone());
      });
    };
    var setDisabledCtrls = function () {
      targets.fold(function () {
        $_4jja6kk5jfuvixx1.each(tableCtrls, noTargetDisable);
        $_4jja6kk5jfuvixx1.each(cellCtrls, noTargetDisable);
        $_4jja6kk5jfuvixx1.each(mergeCtrls, noTargetDisable);
        $_4jja6kk5jfuvixx1.each(unmergeCtrls, noTargetDisable);
      }, function (targets) {
        $_4jja6kk5jfuvixx1.each(tableCtrls, ctrlEnable);
        $_4jja6kk5jfuvixx1.each(cellCtrls, ctrlEnable);
        $_4jja6kk5jfuvixx1.each(mergeCtrls, function (mergeCtrl) {
          mergeCtrl.disabled(targets.mergable().isNone());
        });
        $_4jja6kk5jfuvixx1.each(unmergeCtrls, function (unmergeCtrl) {
          unmergeCtrl.disabled(targets.unmergable().isNone());
        });
      });
    };
    editor.on('init', function () {
      editor.on('nodechange', function (e) {
        var cellOpt = Option.from(editor.dom.getParent(editor.selection.getStart(), 'th,td'));
        targets = cellOpt.bind(function (cellDom) {
          var cell = $_4sdhm4kkjfuviy0e.fromDom(cellDom);
          var table = $_dmqxswkhjfuvixyz.table(cell);
          return table.map(function (table) {
            return $_fgxiq3lqjfuviy6a.forMenu(selections, table, cell);
          });
        });
        setDisabledCtrls();
      });
    });
    var generateTableGrid = function () {
      var html = '';
      html = '<table role="grid" class="mce-grid mce-grid-border" aria-readonly="true">';
      for (var y = 0; y < 10; y++) {
        html += '<tr>';
        for (var x = 0; x < 10; x++) {
          html += '<td role="gridcell" tabindex="-1"><a id="mcegrid' + (y * 10 + x) + '" href="#" ' + 'data-mce-x="' + x + '" data-mce-y="' + y + '"></a></td>';
        }
        html += '</tr>';
      }
      html += '</table>';
      html += '<div class="mce-text-center" role="presentation">1 x 1</div>';
      return html;
    };
    var selectGrid = function (editor, tx, ty, control) {
      var table = control.getEl().getElementsByTagName('table')[0];
      var x, y, focusCell, cell, active;
      var rtl = control.isRtl() || control.parent().rel === 'tl-tr';
      table.nextSibling.innerHTML = tx + 1 + ' x ' + (ty + 1);
      if (rtl) {
        tx = 9 - tx;
      }
      for (y = 0; y < 10; y++) {
        for (x = 0; x < 10; x++) {
          cell = table.rows[y].childNodes[x].firstChild;
          active = (rtl ? x >= tx : x <= tx) && y <= ty;
          editor.dom.toggleClass(cell, 'mce-active', active);
          if (active) {
            focusCell = cell;
          }
        }
      }
      return focusCell.parentNode;
    };
    var insertTable = hasTableGrid(editor) === false ? {
      text: 'Table',
      icon: 'table',
      context: 'table',
      onclick: $_fdch7uk7jfuvixxb.curry($_53k9g0nzjfuviyoc.open, editor)
    } : {
      text: 'Table',
      icon: 'table',
      context: 'table',
      ariaHideMenu: true,
      onclick: function (e) {
        if (e.aria) {
          this.parent().hideAll();
          e.stopImmediatePropagation();
          $_53k9g0nzjfuviyoc.open(editor);
        }
      },
      onshow: function () {
        selectGrid(editor, 0, 0, this.menu.items()[0]);
      },
      onhide: function () {
        var elements = this.menu.items()[0].getEl().getElementsByTagName('a');
        editor.dom.removeClass(elements, 'mce-active');
        editor.dom.addClass(elements[0], 'mce-active');
      },
      menu: [{
          type: 'container',
          html: generateTableGrid(),
          onPostRender: function () {
            this.lastX = this.lastY = 0;
          },
          onmousemove: function (e) {
            var target = e.target;
            var x, y;
            if (target.tagName.toUpperCase() === 'A') {
              x = parseInt(target.getAttribute('data-mce-x'), 10);
              y = parseInt(target.getAttribute('data-mce-y'), 10);
              if (this.isRtl() || this.parent().rel === 'tl-tr') {
                x = 9 - x;
              }
              if (x !== this.lastX || y !== this.lastY) {
                selectGrid(editor, x, y, e.control);
                this.lastX = x;
                this.lastY = y;
              }
            }
          },
          onclick: function (e) {
            var self = this;
            if (e.target.tagName.toUpperCase() === 'A') {
              e.preventDefault();
              e.stopPropagation();
              self.parent().cancel();
              editor.undoManager.transact(function () {
                $_53bmjpo1jfuviyoi.insert(editor, self.lastX + 1, self.lastY + 1);
              });
              editor.addVisual();
            }
          }
        }]
    };
    function cmd(command) {
      return function () {
        editor.execCommand(command);
      };
    }
    var tableProperties = {
      text: 'Table properties',
      context: 'table',
      onPostRender: pushTable,
      onclick: $_fdch7uk7jfuvixxb.curry($_53k9g0nzjfuviyoc.open, editor, true)
    };
    var deleteTable = {
      text: 'Delete table',
      context: 'table',
      onPostRender: pushTable,
      cmd: 'mceTableDelete'
    };
    var row = {
      text: 'Row',
      context: 'table',
      menu: [
        {
          text: 'Insert row before',
          onclick: cmd('mceTableInsertRowBefore'),
          onPostRender: pushCell
        },
        {
          text: 'Insert row after',
          onclick: cmd('mceTableInsertRowAfter'),
          onPostRender: pushCell
        },
        {
          text: 'Delete row',
          onclick: cmd('mceTableDeleteRow'),
          onPostRender: pushCell
        },
        {
          text: 'Row properties',
          onclick: cmd('mceTableRowProps'),
          onPostRender: pushCell
        },
        { text: '-' },
        {
          text: 'Cut row',
          onclick: cmd('mceTableCutRow'),
          onPostRender: pushCell
        },
        {
          text: 'Copy row',
          onclick: cmd('mceTableCopyRow'),
          onPostRender: pushCell
        },
        {
          text: 'Paste row before',
          onclick: cmd('mceTablePasteRowBefore'),
          onPostRender: pushCell
        },
        {
          text: 'Paste row after',
          onclick: cmd('mceTablePasteRowAfter'),
          onPostRender: pushCell
        }
      ]
    };
    var column = {
      text: 'Column',
      context: 'table',
      menu: [
        {
          text: 'Insert column before',
          onclick: cmd('mceTableInsertColBefore'),
          onPostRender: pushCell
        },
        {
          text: 'Insert column after',
          onclick: cmd('mceTableInsertColAfter'),
          onPostRender: pushCell
        },
        {
          text: 'Delete column',
          onclick: cmd('mceTableDeleteCol'),
          onPostRender: pushCell
        }
      ]
    };
    var cell = {
      separator: 'before',
      text: 'Cell',
      context: 'table',
      menu: [
        {
          text: 'Cell properties',
          onclick: cmd('mceTableCellProps'),
          onPostRender: pushCell
        },
        {
          text: 'Merge cells',
          onclick: cmd('mceTableMergeCells'),
          onPostRender: pushMerge
        },
        {
          text: 'Split cell',
          onclick: cmd('mceTableSplitCells'),
          onPostRender: pushUnmerge
        }
      ]
    };
    editor.addMenuItem('inserttable', insertTable);
    editor.addMenuItem('tableprops', tableProperties);
    editor.addMenuItem('deletetable', deleteTable);
    editor.addMenuItem('row', row);
    editor.addMenuItem('column', column);
    editor.addMenuItem('cell', cell);
  };
  var $_35sgjcq3jfuvizan = { addMenuItems: addMenuItems };

  var getClipboardRows = function (clipboardRows) {
    return clipboardRows.get().fold(function () {
      return;
    }, function (rows) {
      return $_4jja6kk5jfuvixx1.map(rows, function (row) {
        return row.dom();
      });
    });
  };
  var setClipboardRows = function (rows, clipboardRows) {
    var sugarRows = $_4jja6kk5jfuvixx1.map(rows, $_4sdhm4kkjfuviy0e.fromDom);
    clipboardRows.set(Option.from(sugarRows));
  };
  var getApi = function (editor, clipboardRows) {
    return {
      insertTable: function (columns, rows) {
        return $_53bmjpo1jfuviyoi.insert(editor, columns, rows);
      },
      setClipboardRows: function (rows) {
        return setClipboardRows(rows, clipboardRows);
      },
      getClipboardRows: function () {
        return getClipboardRows(clipboardRows);
      }
    };
  };

  function Plugin(editor) {
    var resizeHandler = ResizeHandler(editor);
    var cellSelection = CellSelection$1(editor, resizeHandler.lazyResize);
    var actions = TableActions(editor, resizeHandler.lazyWire);
    var selections = Selections(editor);
    var clipboardRows = Cell(Option.none());
    $_5hhd4unsjfuviyn2.registerCommands(editor, actions, cellSelection, selections, clipboardRows);
    $_jmgm9k4jfuvixw9.registerEvents(editor, selections, actions, cellSelection);
    $_35sgjcq3jfuvizan.addMenuItems(editor, selections);
    $_6ddytyq2jfuvizah.addButtons(editor);
    $_6ddytyq2jfuvizah.addToolbars(editor);
    editor.on('PreInit', function () {
      editor.serializer.addTempAttr($_g37vw7m5jfuviyax.firstSelected());
      editor.serializer.addTempAttr($_g37vw7m5jfuviyax.lastSelected());
    });
    if (hasTabNavigation(editor)) {
      editor.on('keydown', function (e) {
        $_flm1p1orjfuviyzd.handle(e, editor, actions, resizeHandler.lazyWire);
      });
    }
    editor.on('remove', function () {
      resizeHandler.destroy();
      cellSelection.destroy();
    });
    return getApi(editor, clipboardRows);
  }
  global.add('table', Plugin);
  function Plugin$1 () {
  }

  return Plugin$1;

}());
})();
