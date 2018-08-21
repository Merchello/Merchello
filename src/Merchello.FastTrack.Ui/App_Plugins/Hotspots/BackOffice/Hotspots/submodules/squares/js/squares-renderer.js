;(function($, window, document, undefined) {

	var registeredElements = new Array();
	$.squaresRendererRegisterElement = function(options) {
		registeredElements[options.name] = options;
	};

	$.squaresRendererRenderObject = function(settings) {
		if (typeof(settings) != 'object') {
			try {
				settings = JSON.parse(settings);
			} catch(err) {
				console.log(err);
				console.log('Squares renderer failed to parse JSON: ');
				console.log(settings);

				return '';
			}
		}

		var renderer = new Renderer(settings);
		var html = renderer.render();

		return html;
	}

	function Renderer(settings) {
		this.settings = settings;
		this.containers = [];

		this.init();
	}
	Renderer.prototype.init = function() {
		// Create containers
		for (var i=0; i<this.settings.containers.length; i++) {
			this.containers[i] = new Container(this.settings.containers[i]);
		}
	};
	Renderer.prototype.render = function() {
		var html = '';

		for (var i=0; i<this.containers.length; i++) {
			html += this.containers[i].render();
		}

		return html;
	}

	function Container(settings) {
		this.settings = settings;
		this.elements = [];

		this.init();
	}
	Container.prototype.init = function() {
		// Create elements
		if (this.settings.settings) {
			for (var i=0; i<this.settings.settings.elements.length; i++) {
				this.elements[i] = new Element(this.settings.settings.elements[i]);
			}
		}
	};
	Container.prototype.render = function() {
		var html = '';

		html += '<div class="squares-container">';

		if (this.settings.settings) {
			for (var i=0; i<this.settings.settings.elements.length; i++) {
				html += this.elements[i].render();
			}
		}

		html += '	<div class="squares-clear"></div>'
		html += '</div>';

		return html;
	}

	var elementDefaultOptions = {
		layout: {
			box_model: {
				margin: {
					top: 0,
					bottom: 0,
					left: 0,
					right: 0
				},
				padding: {
					top: 10,
					bottom: 10,
					left: 10,
					right: 10
				}
			},
			use_grid: 1,
			column_span: {
				xs: {
					use: 0,
					class: 'sq-col-xs-12',
					visible: 0
				},
				sm: {
					use: 0,
					class: 'sq-col-sm-12',
					visible: 0
				},
				md: {
					use: 0,
					class: 'sq-col-md-12',
					visible: 1
				},
				lg: {
					use: 1,
					class: 'sq-col-lg-12',
					visible: 1
				},
			},
			width: '100',
			auto_width: 1,
			height: '100',
			auto_height: 1
		},
		style: {
			background_color: '#ffffff',
			background_opacity: '0',
			opacity: '1',
			box_shadow: 'none',
			border_width: '0',
			border_style: 'none',
			border_color: '#000000',
			border_opacity: '1',
			border_radius: '0',
		},
		font: {
			font_family: 'sans-serif',
			font_size: '14',
			font_weight: 'normal',
			font_style: 'normal',
			line_height: '22',
			text_color: '#ffffff',
			text_align: 'left',
			text_decoration: 'none',
			text_transform: 'none',
			text_shadow: ''
		},
		general: {
			id: '',
			classes: '',
			css: ''
		}
	};

	function Element(e) {
		this.settings = e.settings;

		this.defaults = $.extend(true, {}, elementDefaultOptions);
		this.elementSpecificDefaults = {};
		this.options = undefined;

		this.init(e);
	}
	Element.prototype.init = function(e) {
		// Merge defaults with element-specific options defaults
		var elementSpecificControls = $.extend(true, {}, registeredElements[this.settings.name].controls);

		for (var controlsGroupKey in elementSpecificControls) {
			var controlsRootObj = elementSpecificControls[controlsGroupKey];
			this.elementSpecificDefaults[controlsGroupKey] = {};

			for (var key in controlsRootObj) {
				var control = controlsRootObj[key];

				this.elementSpecificDefaults[controlsGroupKey][key] = control.default;
			}
		}

		this.defaults = $.extend(true, {}, this.defaults, this.elementSpecificDefaults);

		// Merge defaults with the provided options
		this.options = $.extend(true, {}, this.defaults, e.options);
	}
	Element.prototype.render = function() {
		var html = '';
		html += '<div class="squares-element '+ this.generateLayoutClass(this.options['layout']) +'" style="'+ this.generateCSS(this.options) +'">';
		html += registeredElements[this.settings.name].render(this.options);
		html += '</div>';

		return html;
	}
	Element.prototype.generateLayoutClass = function() {
		var o = this.options['layout'];

		if (parseInt(o['use_grid'], 10) == 1) {
			var classes = '';
			var v = o['column_span'];

			if (parseInt(v.xs.use, 10) == 1) {
				classes += v.xs.class + ' ';

				if (parseInt(v.xs.visible, 10) == 0) {
					classes += 'sq-hidden-xs ';
				}
			}
			if (parseInt(v.sm.use, 10) == 1) {
				classes += v.sm.class + ' ';

				if (parseInt(v.sm.visible, 10) == 0) {
					classes += 'sq-hidden-sm ';
				}
			}
			if (parseInt(v.md.use, 10) == 1) {
				classes += v.md.class + ' ';

				if (parseInt(v.md.visible, 10) == 0) {
					classes += 'sq-hidden-md ';
				}
			}
			if (parseInt(v.lg.use, 10) == 1) {
				classes += v.lg.class + ' ';

				if (parseInt(v.lg.visible, 10) == 0) {
					classes += 'sq-hidden-lg ';
				}
			}
			return classes;
		} else {
			return '';
		}
	}
	Element.prototype.generateCSS = function() {
		var css = '';

		// =====================================================================
		// Layout
		// =====================================================================

		var o = this.options['layout'];

		// Box Model
		css += 'margin-top: ' + o['box_model'].margin.top + 'px; ';
		css += 'margin-bottom: ' + o['box_model'].margin.bottom + 'px; ';
		css += 'margin-left: ' + o['box_model'].margin.left + 'px; ';
		css += 'margin-right: ' + o['box_model'].margin.right + 'px; ';
		css += 'padding-top: ' + o['box_model'].padding.top + 'px; ';
		css += 'padding-bottom: ' + o['box_model'].padding.bottom + 'px; ';
		css += 'padding-left: ' + o['box_model'].padding.left + 'px; ';
		css += 'padding-right: ' + o['box_model'].padding.right + 'px; ';

		if (parseInt(o['use_grid'], 10) == 1) {
			// Grid system

		} else {
			// Width
			if (parseInt(o['auto_width'], 10) == 1) {
				css += 'width: auto; ';
			} else {
				if (o['width'] !== '' && !isNaN(o['width'])) {
					css += 'width: '+ o['width'] +'px; ';
				}
			}

			// Height
			if (parseInt(o['auto_height'], 10) == 1) {
				css += 'height: auto; ';
			} else {
				if (o['height'] !== '' && !isNaN(o['height'])) {
					css += 'height: '+ o['height'] +'px; ';
				}
			}
		}

		css += 'float: left; ';

		// =====================================================================
		// Text
		// =====================================================================
		var o = this.options['font'];
		this.options.fontStyles = '';

		if (o) {
			// Font Family
			css += 'font-family: ' + o['font_family'] + '; ';
			this.options.fontStyles += 'font-family: ' + o['font_family'] + '; ';

			// Font Size
			css += 'font-size: ' + o['font_size'] + 'px; ';
			this.options.fontStyles += 'font-size: ' + o['font_size'] + 'px; ';

			// Font Weight
			css += 'font-weight: ' + o['font_weight'] + '; ';
			this.options.fontStyles += 'font-weight: ' + o['font_weight'] + '; ';

			// Font Style
			css += 'font-style: ' + o['font_style'] + '; ';
			this.options.fontStyles += 'font-style: ' + o['font_style'] + '; ';

			// Line Height
			css += 'line-height: ' + o['line_height'] + 'px; ';
			this.options.fontStyles += 'line-height: ' + o['line_height'] + 'px; ';

			// Text Color
			css += 'color: ' + o['text_color'] + '; ';
			this.options.fontStyles += 'color: ' + o['text_color'] + '; ';

			// Text Align
			css += 'text-align: ' + o['text_align'] + '; ';
			this.options.fontStyles += 'text-align: ' + o['text_align'] + '; ';

			// Text Decoration
			css += 'text-decoration: ' + o['text_decoration'] + '; ';
			this.options.fontStyles += 'text-decoration: ' + o['text_decoration'] + '; ';

			// Text Transform
			css += 'text-transform: ' + o['text_transform'] + '; ';
			this.options.fontStyles += 'text-transform: ' + o['text_transform'] + '; ';

			// Text Shadow
			css += 'text-shadow: ' + o['text_shadow'] + '; ';
			this.options.fontStyles += 'text-shadow: ' + o['text_shadow'] + '; ';
		}

		// =====================================================================
		// Style
		// =====================================================================
		var o = this.options['style'];

		if (o) {
			// Background Color
			var c_bg = hexToRgb(o['background_color']);
			css += 'background-color: rgba('+ c_bg.r +', '+ c_bg.g +', '+ c_bg.b +', '+ o['background_opacity'] +'); ';

			// Opacity
			css += 'opacity: ' + o['opacity'] + '; ';

			// Box Shadow
			css += 'box-shadow: ' + o['box_shadow'] + '; ';

			// Border Width
			css += 'border-width: ' + o['border_width'] + 'px; ';

			// Border Style
			css += 'border-style: ' + o['border_style'] + '; ';

			// Border Color
			var c_bg = hexToRgb(o['border_color']);
			css += 'border-color: rgba('+ c_bg.r +', '+ c_bg.g +', '+ c_bg.b +', '+ o['border_opacity'] +'); ';

			// Border Radius
			css += 'border-radius: ' + o['border_radius'] + 'px; ';
		}

		return css;
	}

	function hexToRgb(hex) {
		var result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hex);
		return result ? {
			r: parseInt(result[1], 16),
			g: parseInt(result[2], 16),
			b: parseInt(result[3], 16)
		} : null;
	}

})(jQuery, window, document);
