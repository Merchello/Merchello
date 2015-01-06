/*
Copyright (c) 2013, Goldark SS LTDA <http://www.goldark.co>
All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
-Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
-Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the 
documentation and/or other materials provided with the distribution.
-Neither the name of the Goldark nor the names of its contributors may be used to endorse or promote products derived from this software 
without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE 
LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT 
LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
OF SUCH DAMAGE.
*/

/*
This module is based in the firefox jsonview extenrsion made by Ben Hollis: https://github.com/bhollis/jsonview/
*/
'use strict';

//angular.module('gd.ui.jsonexplorer', [])
angular.module('umbraco')
.directive('jsonExplorer', function ($http) {
	return {
		restrict: 'E',
		scope: {
			jsonData: '@',
		},
		link: function (scope, elem, attrs) {
			attrs.$observe('jsonData', function (val) {
				var output = '';
				var formatter = {};
				formatter.jsString = function (s) {
					var has = {
      					'\b': 'b',
      					'\f': 'f',
      					'\r': 'r',
      					'\n': 'n',
      					'\t': 't'
    				}, ws;
    				for (ws in has) {
      					if (-1 === s.indexOf(ws)) {
        					delete has[ws];
      					}
    				}
    				
    				s = JSON.stringify({a:s});
    				s = s.slice(6, -2);
    				for (ws in has) {
      					s = s.replace(new RegExp('\\\\u000' + (ws.charCodeAt().toString(16)), 'ig'),
                    		'\\' + has[ws]);
    				}

    				return this.htmlEncode(s);
				};
				formatter.htmlEncode =  function (t) {
						if (t == null) {
							return '';
						}
    					return t.toString().replace(/&/g,"&amp;")
    						.replace(/"/g,"&quot;").replace(/</g,"&lt;").replace(/>/g,"&gt;");
  					};
				formatter.decorateWithSpan = function (value, className) {
					return '<span class="' + className + '">' + this.htmlEncode(value) + '</span>';
				};
				formatter.arrayToHtml = function (json) {
					var hasContents = false;
    				var output = '';
    				var numProps = 0;
    
    				for (var prop in json ) {
      					numProps++;
    				}

    				for (var prop in json) {
      					hasContents = true;
      					output += '<li>' + this.valueToHtml(json[prop]);
      					if (numProps > 1) {
       						output += ',';
      					}
      					output += '</li>';
      					numProps--;
    				}
    
    				if (hasContents) {
      					output = '[<ul class="array collapsible">' + output + '</ul>]';
    				} else {
      					output = '[ ]';
    				}
        			return output;
				};

				formatter.objectToHtml = function (json) {
					var hasContents = false;
    				var output = '';
    				var numProps = 0;
    				for (var prop in json ) {
      					numProps++;
    				}

    				for (var prop in json) {
      					hasContents = true;
      					output += '<li>' + 
      					'<span class="prop"><span class="q">"</span>' + this.jsString(prop) +
                			'<span class="q">"</span></span>: ' + this.valueToHtml(json[prop]);
	      				if (numProps > 1) {
	        				output += ',';
	      				}
	      				output += '</li>';
	      				numProps--;
    				}
    
	    			if (hasContents) {
	      				output = '{<ul class="obj collapsible">' + output + '</ul>}';
	    			} else {
	      				output = '{ }';
	    			}
	    
	    			return output;
				};
				
				formatter.valueToHtml = function (value) {
					var type = value && value.constructor;
					var output = '';

					if (value == null) {
						output += this.decorateWithSpan('null', 'null');
					}

					if (value && type == Array) {
						output += this.arrayToHtml(value);
					}

					if (value && type == Object) {
						output += this.objectToHtml(value);
					}

					if (type == Number) {
						output += this.decorateWithSpan(value, 'num');
					}

					if (type == String) {
						if (/^(http|https|file):\/\/[^\s]+$/i.test(value)) {
        					output += '<a href="' + value + '"><span class="q">"</span>' + 
        						this.jsString(value) + '<span class="q">"</span></a>';
      					} else {
        					output += '<span class="string">"' + this.jsString(value) + '"</span>';
      					}
					}

					if (type == Boolean) {
						output += this.decorateWithSpan(value, 'bool');
					}

					return output;
				};
				formatter.jsonToHtml = function (json) {
					return '<div class="gd-ui-json-explorer">' + this.valueToHtml(json) + '</div>';
				};
				
				var json = JSON.parse(val);
				var x = formatter.jsonToHtml(json);
				elem.html(x);
				function collapse (evt) {
					var collapser = evt.target;
    				var target = collapser.parentNode.getElementsByClassName('collapsible');
    
    				if (!target.length) {
      					return;
    				}

    				target = target[0];

    				if (target.style.display == 'none') {
				      var ellipsis = target.parentNode.getElementsByClassName('ellipsis')[0];
				      target.parentNode.removeChild(ellipsis);
				      target.style.display = '';
				      collapser.innerHTML = '-';
    				} else {
    				  target.style.display = 'none';
				   	  var ellipsis = document.createElement('span');
				      ellipsis.className = 'ellipsis';
				      ellipsis.innerHTML = ' &hellip; ';
				      target.parentNode.insertBefore(ellipsis, target);
				      collapser.innerHTML = '+';
    				}
				}
				
				var collections = angular.element(elem)[0].getElementsByTagName('ul');
				for (var i = 0; i < collections.length; i++) {
					var collectionItem = collections[i];
					if (collectionItem.className.indexOf('collapsible') != -1) {
						if (collectionItem.parentNode.nodeName == 'LI') {
							var collapser = document.createElement('div');
							collapser.className = 'collapser';
							collapser.innerHTML = '-';
							collapser.addEventListener('click', collapse, false);
							collectionItem.parentNode.insertBefore(collapser, collectionItem.parentNode.firstChild);
						}						
					}
				}
			});
      	}
    }
});