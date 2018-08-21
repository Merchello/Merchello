;(function ( $, window, document, undefined ) {

    // Variable to hold the currently fullscreen image map
    var fullscreenMap = undefined;

    // API

    /*
        HTML API
        ---------------------------------------
        data-imp-highlight-shape-on-mouseover
        data-imp-highlight-shape-on-click
        data-imp-unhighlight-shape-on-mouseover
        data-imp-unhighlight-shape-on-click

        data-imp-open-tooltip-on-mouseover
        data-imp-open-tooltip-on-click
        data-imp-close-tooltip-on-mouseover
        data-imp-close-tooltip-on-click

        data-trigger-shape-on-mouseover
        data-trigger-shape-on-click
        data-untrigger-shape-on-mouseover
        data-untrigger-shape-on-click

        EXAMPLE
        ---------------------------------------
        <div data-imp-highlight-shape-on-mouseover="myshape1" data-imp-image-map-name="map1">Click</div>
    */

    // Events (called by the plugin, need implementation)
    $.imageMapProInitialized = function(imageMapName) {
        
    }
    $.imageMapProEventHighlightedShape = function(imageMapName, shapeName) {

    }
    $.imageMapProEventUnhighlightedShape = function(imageMapName, shapeName) {

    }
    $.imageMapProEventClickedShape = function(imageMapName, shapeName) {

    }
    $.imageMapProEventOpenedTooltip = function(imageMapName, shapeName) {

    }
    $.imageMapProEventClosedTooltip = function(imageMapName) {

    }
    // Actions (called by a third party, implemented here)
    $.imageMapProHighlightShape = function(imageMapName, shapeName) {
        var i = $('[data-shape-title="' + shapeName + '"]').data('index');

        if (instances[imageMapName].highlightedShapeIndex == i) return;

        if (instances[imageMapName].highlightedShape) {
            instances[imageMapName].unhighlightShape();
        }

        instances[imageMapName].manuallyHighlightedShape = true;
        instances[imageMapName].highlightShape(i, false);
    }
    $.imageMapProUnhighlightShape = function(imageMapName) {
        if (instances[imageMapName].highlightedShape) {
            instances[imageMapName].unhighlightShape();
        }
    }
    $.imageMapProOpenTooltip = function(imageMapName, shapeName) {
        var i = $('[data-shape-title="' + shapeName + '"]').data('index');

        instances[imageMapName].manuallyShownTooltip = true;
        instances[imageMapName].showTooltip(i);
        instances[imageMapName].updateTooltipPosition(i);
    }
    $.imageMapProHideTooltip = function(imageMapName) {
        instances[imageMapName].hideTooltip();
    }
    $.imageMapProReInitMap = function(imageMapName) {
        instances[imageMapName].init();
    }
    $.imageMapProIsMobile = function() {
        return isMobile();
    }

    "use strict";

    // undefined is used here as the undefined global variable in ECMAScript 3 is
    // mutable (ie. it can be changed by someone else). undefined isn't really being
    // passed in so we can ensure the value of it is truly undefined. In ES5, undefined
    // can no longer be modified.

    // window and document are passed through as local variable rather than global
    // as this (slightly) quickens the resolution process and can be more efficiently
    // minified (especially when both are regularly referenced in your plugin).

    // Create the defaults once
    var pluginName = "imageMapPro";
    var default_settings = $.imageMapProEditorDefaults;
	var default_spot_settings = $.imageMapProShapeDefaults;

    var instances = new Array();
    MutationObserver = window.MutationObserver || window.WebKitMutationObserver;
    var mutationObserver;


    // The actual plugin constructor
    function Plugin ( element, options ) {
        this.element = element;
        // jQuery has an extend method which merges the contents of two or
        // more objects, storing the result in the first object. The first object
        // is generally empty as we don't want to alter the default options for
        // future instances of the plugin
        this.settings = $.extend(true, {}, default_settings, options);

        this.root = $(element);
        this.wrap = undefined;
        this.ui = undefined;
        this.shapeContainer = undefined;
        this.shapeSvgContainer = undefined;
        this.fullscreenTooltipsContainer = undefined;

        // Cache
        this.visibleTooltip = undefined;
        this.visibleTooltipIndex = undefined;
        this.highlightedShape = undefined;
        this.highlightedShapeIndex = undefined;
        this.clickedShape = undefined;
        this.clickedShapeIndex = undefined;
        this.bodyOverflow = undefined;

        // Mutations observer
        this.initTimeout = undefined;

        // Flags
        this.touch = false;
        this.fullscreenTooltipVisible = false;

        this.init();
    }

    // Avoid Plugin.prototype conflicts
    $.extend(Plugin.prototype, {
        init: function () {
            var self = this;

            self.parseSettings();

            instances[this.settings.general.name] = this;

            this.id = Math.random()*100;

            // Fill out any missing properties
            for (var i=0; i<self.settings.spots.length; i++) {
                var s = self.settings.spots[i];
                var d = $.extend(true, {}, default_spot_settings);
                s = $.extend(true, d, s);
                self.settings.spots[i] = $.extend(true, {}, s);

                // Support for image maps created before 3.1.0
                if (!self.settings.spots[i].title || self.settings.spots[i].title.length == 0) {
                    self.settings.spots[i].title = self.settings.spots[i].id;
                }
            }

            self.root.addClass('imp-initialized');
            self.root.attr('data-image-map-pro-id', self.settings.id);
            self.root.html('<div class="imp-wrap"></div>');
            self.wrap = self.root.find('.imp-wrap');

            var img = new Image();
            img.src = self.settings.image.url;

            self.loadImage(img, function() {
                // Image loading
            }, function() {
                // Image loaded
                var html = '';

                html += '<img src="'+ self.settings.image.url +'">';

                self.wrap.html(html);

                self.centerImageMap();
                self.adjustSize();
                self.drawShapes();
                self.addTooltips();
                self.initFullscreen();
                self.events();
                self.animateShapesLoop();

                $.imageMapProInitialized(self.settings.general.name);
            });

            $(window).on('resize', function() {
                if (self.visibleTooltip) {
                    self.updateTooltipPosition(self.highlightedShapeIndex);
                }
            });
        },
        parseSettings: function() {
            // If there is a value for the old image URL in the settings, use that instead
            // This happens when the user updates from an older version using the old format and the
            // image map has not been saved yet.
            if (this.settings.general.image_url) {
                this.settings.image.url = this.settings.general.image_url;
            }
        },
        loadImage: function(image, cbLoading, cbComplete) {
            if (!image.complete || image.naturalWidth === undefined || image.naturalHeight === undefined) {
                cbLoading();
                $(image).on('load', function() {
                    $(image).off('load');
                    cbComplete();
                });
            } else {
                cbComplete();
            }
        },

        centerImageMap: function() {
            var self = this;

            if (parseInt(self.settings.general.center_image_map, 10) == 1) {
                self.wrap.css({
                    margin: '0 auto'
                });
            }
        },
        adjustSize: function() {
            var self = this;

            // If the image map is in fullscreen mode, adjust according to the window and return
            if (parseInt(self.settings.runtime.is_fullscreen, 10) == 1) {
                var screenRatio = $(window).width() / $(window).height();
                var imageRatio = self.settings.general.width / self.settings.general.height;
                
                if (imageRatio < screenRatio) {
                    self.settings.general.width = $(window).height() * imageRatio;
                    self.settings.general.height = $(window).height();
                } else {
                    self.settings.general.width = $(window).width();
                    self.settings.general.height = $(window).width() / imageRatio;
                }

                self.wrap.css({
                    width: self.settings.general.width,
                    height: self.settings.general.height,
                });
                
                return;
            }

            // If the image map is responsive, fit the map to its parent element
            if (parseInt(self.settings.general.responsive, 10) == 1) {
                if (parseInt(self.settings.general.preserve_quality, 10) == 1) {
                    var width = self.settings.general.naturalWidth || self.settings.general.width;
                    self.wrap.css({
                        'max-width': self.settings.general.naturalWidth
                    });
                }
            } else {
                self.wrap.css({
                    width: self.settings.general.width,
                    height: self.settings.general.height,
                });
            }
        },
        drawShapes: function() {
            var self = this;

            // Make sure spot coordinates are numbers
            for (var i=0; i<self.settings.spots.length; i++) {
                var s = self.settings.spots[i];

                s.x = parseFloat(s.x);
                s.y = parseFloat(s.y);
                s.width = parseFloat(s.width);
                s.height = parseFloat(s.height);
                s.default_style.stroke_width = parseInt(s.default_style.stroke_width);
                s.mouseover_style.stroke_width = parseInt(s.mouseover_style.stroke_width);

                if (s.type == 'poly') {
                    for (var j=0; j<s.points.length; j++) {
                        s.points[j].x = parseFloat(s.points[j].x);
                        s.points[j].y = parseFloat(s.points[j].y);
                    }
                }
            }
            self.settings.general.width = parseInt(self.settings.general.width);
            self.settings.general.height = parseInt(self.settings.general.height);

            self.wrap.prepend('<div class="imp-shape-container"></div>');
            self.shapeContainer = self.wrap.find('.imp-shape-container');

            var html = '';
            var hasPolygons = false;

            // If the image map is responsive, use natural width and height
            // Otherwise, use the width/height set from the editor
            var imageMapWidth = self.settings.general.width;
            var imageMapHeight = self.settings.general.height;
            if (parseInt(self.settings.general.responsive, 10) == 1) {
                imageMapWidth = self.settings.general.naturalWidth;
                imageMapHeight = self.settings.general.naturalHeight;
            }
            var svgHtml = '<svg class="hs-poly-svg" viewBox="0 0 '+ imageMapWidth +' '+ imageMapHeight +'" preserveAspectRatio="none">';

            for (var i=0; i<self.settings.spots.length; i++) {
                var s = self.settings.spots[i];
                if (s.type == 'spot') {
                    if (parseInt(s.default_style.use_icon, 10) == 1) {
                        var style = '';

                        var wrapperWidth = (s.width < 44) ? 44 : s.width;
                        var wrapperHeight = (s.height < 44) ? 44 : s.height;

                        style += 'left: '+ s.x +'%;';
                        style += 'top: '+ s.y +'%;';
                        style += 'width: '+ wrapperWidth +'px;';
                        style += 'height: '+ wrapperHeight +'px;';

                        // If the icon is a pin, center it on the bottom edge
                        var marginTop = -wrapperHeight/2;
                        var marginLeft = -wrapperWidth/2;

                        if (parseInt(s.default_style.icon_is_pin, 10) == 1) {
                            marginTop = -wrapperHeight;

                            if (s.height < 44) {
                                marginTop = -wrapperHeight/2 -s.height/2;
                            }
                        }

                        style += 'margin-left: '+ marginLeft +'px;';
                        style += 'margin-top: '+ marginTop +'px;';

                        style += 'background-image: url('+ s.default_style.icon_url +')';
                        style += 'background-position: center;';
                        style += 'background-repeat: no-repeat;';

                        // Page load animations
                        if (self.settings.general.pageload_animation == 'fade') {
                            style += 'opacity: 0;';
                        }
                        if (self.settings.general.pageload_animation == 'grow') {
                            style += 'opacity: ' + s.default_style.opacity + ';';
                            style += 'transform: scale(0, 0);-moz-transform: scale(0, 0);-webkit-transform: scale(0, 0);';
                            if (parseInt(s.default_style.icon_is_pin, 10) == 1) {
                                style += 'transform-origin: 50% 100%;-moz-transform-origin: 50% 100%;-webkit-transform-origin: 50% 100%;';
                            }
                        }
                        if (self.settings.general.pageload_animation == 'none') {
                            style += 'opacity: ' + s.default_style.opacity + ';';
                        }

                        html += '<div class="imp-shape imp-shape-spot" id="'+ s.id +'" data-shape-title="'+ s.title +'" style="'+ style +'" data-index='+ i +'>';

                        // Icon
                        if (s.default_style.icon_type == 'library') {
                            html += '   <svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" version="1.1" viewBox="'+ s.default_style.icon_svg_viewbox +'" xml:space="preserve" width="'+ s.width +'px" height="'+ s.height +'px">';
                            html += '       <path style="fill:'+ s.default_style.icon_fill +'" d="'+ s.default_style.icon_svg_path +'"></path>';
                            html += '   </svg>';
                        } else {
                            html += '<img src="'+ s.default_style.icon_url +'">';
                        }

                        // Shadow
                        if (parseInt(s.default_style.icon_shadow, 10) == 1) {
                            var shadowStyle = '';

                            shadowStyle += 'width: ' + s.width + 'px;';
                            shadowStyle += 'height: ' + s.height + 'px;';
                            shadowStyle += 'top: '+ s.height/2 +'px;';

                            html += '<div style="'+ shadowStyle +'" class="imp-shape-icon-shadow"></div>';
                        }

                        html += '</div>';
                    } else {
                        var style = '';
                        var color_bg = hexToRgb(s.default_style.background_color) || { r: 0, b: 0, g: 0 };
                        var color_border = hexToRgb(s.default_style.border_color) || { r: 0, b: 0, g: 0 };

                        style += 'left: '+ s.x +'%;';
                        style += 'top: '+ s.y +'%;';
                        style += 'width: '+ s.width +'px;';
                        style += 'height: '+ s.height +'px;';
                        style += 'margin-left: -'+ s.width/2 +'px;';
                        style += 'margin-top: -'+ s.height/2 +'px;';

                        style += 'border-radius: ' + s.default_style.border_radius + 'px;';
                        style += 'background: rgba('+ color_bg.r +', '+ color_bg.g +', '+ color_bg.b +', '+ s.default_style.background_opacity +');';
                        style += 'border-width: ' + s.default_style.border_width + 'px;';
                        style += 'border-style: ' + s.default_style.border_style + ';';
                        style += 'border-color: rgba('+ color_border.r +', '+ color_border.g +', '+ color_border.b +', '+ s.default_style.border_opacity +');';

                        if (self.settings.general.pageload_animation == 'fade') {
                            style += 'opacity: 0;';
                        }
                        if (self.settings.general.pageload_animation == 'grow') {
                            style += 'opacity: ' + s.default_style.opacity + ';';
                            style += 'transform: scale(0, 0);-moz-transform: scale(0, 0);-webkit-transform: scale(0, 0);';
                        }
                        if (self.settings.general.pageload_animation == 'none') {
                            style += 'opacity: ' + s.default_style.opacity + ';';
                        }

                        html += '<div class="imp-shape imp-shape-spot" id="'+ s.id +'" data-shape-title="'+ s.title +'" style="'+ style +'" data-index='+ i +'></div>';
                    }
                }
                if (s.type == 'rect') {
                    var style = '';
                    var color_bg = hexToRgb(s.default_style.background_color) || { r: 0, b: 0, g: 0 };
                    var color_border = hexToRgb(s.default_style.border_color) || { r: 0, b: 0, g: 0 };

                    style += 'left: '+ s.x +'%;';
                    style += 'top: '+ s.y +'%;';
                    style += 'width: '+ s.width +'%;';
                    style += 'height: '+ s.height +'%;';

                    style += 'border-radius: ' + s.default_style.border_radius + 'px;';
                    style += 'background: rgba('+ color_bg.r +', '+ color_bg.g +', '+ color_bg.b +', '+ s.default_style.background_opacity +');';
                    style += 'border-width: ' + s.default_style.border_width + 'px;';
                    style += 'border-style: ' + s.default_style.border_style + ';';
                    style += 'border-color: rgba('+ color_border.r +', '+ color_border.g +', '+ color_border.b +', '+ s.default_style.border_opacity +');';

                    if (self.settings.general.pageload_animation == 'fade') {
                        style += 'opacity: 0;';
                    }
                    if (self.settings.general.pageload_animation == 'grow') {
                        style += 'opacity: ' + s.default_style.opacity + ';';
                        style += 'transform: scale(0, 0);-moz-transform: scale(0, 0);-webkit-transform: scale(0, 0);';
                    }
                    if (self.settings.general.pageload_animation == 'none') {
                        style += 'opacity: ' + s.default_style.opacity + ';';
                    }

                    html += '<div class="imp-shape imp-shape-rect" id="'+ s.id +'" data-shape-title="'+ s.title +'" style="'+ style +'" data-index='+ i +'></div>';
                }
                if (s.type == 'oval') {
                    var style = '';
                    var color_bg = hexToRgb(s.default_style.background_color) || { r: 0, b: 0, g: 0 };
                    var color_border = hexToRgb(s.default_style.border_color) || { r: 0, b: 0, g: 0 };

                    style += 'left: '+ s.x +'%;';
                    style += 'top: '+ s.y +'%;';
                    style += 'width: '+ s.width +'%;';
                    style += 'height: '+ s.height +'%;';

                    style += 'border-radius: 50% 50%;';
                    style += 'background: rgba('+ color_bg.r +', '+ color_bg.g +', '+ color_bg.b +', '+ s.default_style.background_opacity +');';
                    style += 'border-width: ' + s.default_style.border_width + 'px;';
                    style += 'border-style: ' + s.default_style.border_style + ';';
                    style += 'border-color: rgba('+ color_border.r +', '+ color_border.g +', '+ color_border.b +', '+ s.default_style.border_opacity +');';

                    if (self.settings.general.pageload_animation == 'fade') {
                        style += 'opacity: 0;';
                    }
                    if (self.settings.general.pageload_animation == 'grow') {
                        style += 'opacity: ' + s.default_style.opacity + ';';
                        style += 'transform: scale(0, 0);-moz-transform: scale(0, 0);-webkit-transform: scale(0, 0);';
                    }
                    if (self.settings.general.pageload_animation == 'none') {
                        style += 'opacity: ' + s.default_style.opacity + ';';
                    }

                    html += '<div class="imp-shape imp-shape-oval" id="'+ s.id +'" data-shape-title="'+ s.title +'" style="'+ style +'" data-index='+ i +'></div>';
                }
                if (s.type == 'poly') {
                    hasPolygons = true;
                    var c_fill = hexToRgb(s.default_style.fill) || { r: 0, b: 0, g: 0 };
                    var c_stroke = hexToRgb(s.default_style.stroke_color) || { r: 0, b: 0, g: 0 };

                    var svgStyle = '';
                    svgStyle += 'width: 100%;';
                    svgStyle += 'height: 100%;';
                    svgStyle += 'fill: rgba('+ c_fill.r +', '+ c_fill.g +', '+ c_fill.b +', '+ s.default_style.fill_opacity +');';
                    svgStyle += 'stroke: rgba('+ c_stroke.r +', '+ c_stroke.g +', '+ c_stroke.b +', '+ s.default_style.stroke_opacity +');';
                    svgStyle += 'stroke-width: ' + s.default_style.stroke_width + 'px;';
                    svgStyle += 'stroke-dasharray: ' + s.default_style.stroke_dasharray + ';';
                    svgStyle += 'stroke-linecap: ' + s.default_style.stroke_linecap + ';';

                    if (self.settings.general.pageload_animation == 'fade') {
                        svgStyle += 'opacity: 0;';
                    }
                    if (self.settings.general.pageload_animation == 'grow') {
                        svgStyle += 'opacity: ' + s.default_style.opacity + ';';
                        svgStyle += 'transform: scale(0, 0);-moz-transform: scale(0, 0);-webkit-transform: scale(0, 0);';
                        var originX = s.x + s.width/2;
                        var originY = s.y + s.height/2;
                        svgStyle += 'transform-origin: '+ originX +'% '+ originY +'%;-moz-transform-origin: '+ originX +'% '+ originY +'%;-webkit-transform-origin: '+ originX +'% '+ originY +'%;';
                    }
                    if (self.settings.general.pageload_animation == 'none') {
                        svgStyle += 'opacity: ' + s.default_style.opacity + ';';
                    }

                    var shapeWidthPx = imageMapWidth * (s.width/100);
                    var shapeHeightPx = imageMapHeight * (s.height/100);

                    svgHtml += '           <polygon class="imp-shape imp-shape-poly" style="'+ svgStyle +'" data-index='+ i +' id="'+ s.id +'" data-shape-title="'+ s.title +'" points="';

                    s.vs = new Array();
                    for (var j=0; j<s.points.length; j++) {
                        var x = (imageMapWidth * (s.x/100)) + (s.points[j].x / 100) * (shapeWidthPx);
                        var y = (imageMapHeight * (s.y/100)) + (s.points[j].y / 100) * (shapeHeightPx);

                        svgHtml += x + ',' + y + ' ';

                        // Cache an array of coordinates for later use in mouse events
                        s.vs.push([ x, y ]);
                    }

                    svgHtml += '           "></polygon>';
                }
            }
            svgHtml += '</svg>';

            if (hasPolygons) {
                self.shapeContainer.html(html + svgHtml);
            } else {
                self.shapeContainer.html(html);
            }
        },
        addTooltips: function() {
            var self = this;

            if (self.settings.tooltips.fullscreen_tooltips == 'always' || (self.settings.tooltips.fullscreen_tooltips == 'mobile-only' && isMobile())) {
                // Fullscreen tooltips
                if (!self.fullscreenTooltipsContainer) {
                    $('.imp-fullscreen-tooltips-container[data-image-map-id="'+ self.settings.id +'"]').remove();
                    $('body').prepend('<div class="imp-fullscreen-tooltips-container" data-image-map-id="'+ self.settings.id +'"></div>');
                    self.fullscreenTooltipsContainer = $('.imp-fullscreen-tooltips-container[data-image-map-id="'+ self.settings.id +'"]');
                }

                var html = '';

                for (var i=0; i<self.settings.spots.length; i++) {
                    var s = self.settings.spots[i];

                    // Fix new lines
                    s.tooltip_content.plain_text = s.tooltip_content.plain_text.replace(/\n/g, '<br>');

                    var style = '';
                    var color_bg = hexToRgb(s.tooltip_style.background_color) || { r: 0, b: 0, g: 0 };

                    style += 'padding: '+ s.tooltip_style.padding +'px;';
                    style += 'background: rgba('+ color_bg.r +', '+ color_bg.g +', '+ color_bg.b +', '+ s.tooltip_style.background_opacity +');';

                    if (self.settings.tooltips.tooltip_animation == 'none') {
                        style += 'opacity: 0;';
                    }
                    if (self.settings.tooltips.tooltip_animation == 'fade') {
                        style += 'opacity: 0;';
                        style += 'transition-property: opacity;-moz-transition-property: opacity;-webkit-transition-property: opacity;';
                    }
                    if (self.settings.tooltips.tooltip_animation == 'grow') {
                        style += 'transform: scale(0, 0);-moz-transform: scale(0, 0);-webkit-transform: scale(0, 0);';
                        style += 'transition-property: transform;-moz-transition-property: -moz-transform;-webkit-transition-property: -webkit-transform;';
                        style += 'transform-origin: 50% 50%;-moz-transform-origin: 50% 50%;-webkit-transform-origin: 50% 50%;';
                    }

                    html += '<div class="imp-fullscreen-tooltip" style="'+ style +'" data-index="'+ i +'">';
                    html += '   <div class="imp-tooltip-close-button" data-index="'+ i +'"><i class="fa fa-times" aria-hidden="true"></i></div>';

                    if (s.tooltip_content.content_type == 'plain-text') {
                        var style = '';
                        style += 'color: ' + s.tooltip_content.plain_text_color + ';';

                        html += '<div class="imp-tooltip-plain-text" style="'+ style +'">' + s.tooltip_content.plain_text + '</div>';
                    } else {
						if (s.tooltip_content.squares_json) {
							html += $.squaresRendererRenderObject(s.tooltip_content.squares_json);
						} else {
							html += $.squaresRendererRenderObject(s.tooltip_content.squares_settings);
						}
                    }

                    html += '</div>';
                }

                self.fullscreenTooltipsContainer.html(html);
            } else {
                // Regular tooltips
                var html = '';

                for (var i=0; i<self.settings.spots.length; i++) {
                    var s = self.settings.spots[i];

                    // Fix new lines
                    s.tooltip_content.plain_text = s.tooltip_content.plain_text.replace(/\n/g, '<br>');

                    var style = '';
                    var color_bg = hexToRgb(s.tooltip_style.background_color) || { r: 0, b: 0, g: 0 };
                    var tooltipBufferPolyClass = (s.type == 'poly') ? 'imp-tooltip-buffer-large' : '';

                    style += 'border-radius: '+ s.tooltip_style.border_radius +'px;';
                    style += 'padding: '+ s.tooltip_style.padding +'px;';
                    style += 'background: rgba('+ color_bg.r +', '+ color_bg.g +', '+ color_bg.b +', '+ s.tooltip_style.background_opacity +');';

                    if (self.settings.tooltips.tooltip_animation == 'none') {
                        style += 'opacity: 0;';
                    }
                    if (self.settings.tooltips.tooltip_animation == 'fade') {
                        style += 'opacity: 0;';
                        style += 'transition-property: opacity;-moz-transition-property: opacity;-webkit-transition-property: opacity;';
                    }
                    if (self.settings.tooltips.tooltip_animation == 'grow') {
                        style += 'transform: scale(0, 0);-moz-transform: scale(0, 0);-webkit-transform: scale(0, 0);';
                        style += 'transition-property: transform;-moz-transition-property: -moz-transform;-webkit-transition-property: -webkit-transform;';

                        if (s.tooltip_style.position == 'top') {
                            style += 'transform-origin: 50% 100%;-moz-transform-origin: 50% 100%;-webkit-transform-origin: 50% 100%;';
                        }
                        if (s.tooltip_style.position == 'bottom') {
                            style += 'transform-origin: 50% 0%;-moz-transform-origin: 50% 0%;-webkit-transform-origin: 50% 0%;';
                        }
                        if (s.tooltip_style.position == 'left') {
                            style += 'transform-origin: 100% 50%;-moz-transform-origin: 100% 50%;-webkit-transform-origin: 100% 50%;';
                        }
                        if (s.tooltip_style.position == 'right') {
                            style += 'transform-origin: 0% 50%;-moz-transform-origin: 0% 50%;-webkit-transform-origin: 0% 50%;';
                        }
                    }

                    html += '<div class="imp-tooltip" style="'+ style +'" data-index="'+ i +'">';

                    if (s.tooltip_style.position == 'top') {
                        html += '   <div class="hs-arrow hs-arrow-bottom" style="border-top-color: rgba('+ color_bg.r +', '+ color_bg.g +', '+ color_bg.b +', '+ s.tooltip_style.background_opacity +');"></div>';
                        if (parseInt(self.settings.tooltips.sticky_tooltips, 10) == 0) {
                            html += '   <div class="imp-tooltip-buffer imp-tooltip-buffer-bottom '+ tooltipBufferPolyClass +'"></div>';
                        }
                    }
                    if (s.tooltip_style.position == 'bottom') {
                        html += '   <div class="hs-arrow hs-arrow-top" style="border-bottom-color: rgba('+ color_bg.r +', '+ color_bg.g +', '+ color_bg.b +', '+ s.tooltip_style.background_opacity +');"></div>';
                        if (parseInt(self.settings.tooltips.sticky_tooltips, 10) == 0) {
                            html += '   <div class="imp-tooltip-buffer imp-tooltip-buffer-top '+ tooltipBufferPolyClass +'"></div>';
                        }
                    }
                    if (s.tooltip_style.position == 'left') {
                        html += '   <div class="hs-arrow hs-arrow-right" style="border-left-color: rgba('+ color_bg.r +', '+ color_bg.g +', '+ color_bg.b +', '+ s.tooltip_style.background_opacity +');"></div>';
                        if (parseInt(self.settings.tooltips.sticky_tooltips, 10) == 0) {
                            html += '   <div class="imp-tooltip-buffer imp-tooltip-buffer-right '+ tooltipBufferPolyClass +'"></div>';
                        }
                    }
                    if (s.tooltip_style.position == 'right') {
                        html += '   <div class="hs-arrow hs-arrow-left" style="border-right-color: rgba('+ color_bg.r +', '+ color_bg.g +', '+ color_bg.b +', '+ s.tooltip_style.background_opacity +');"></div>';
                        if (parseInt(self.settings.tooltips.sticky_tooltips, 10) == 0) {
                            html += '   <div class="imp-tooltip-buffer imp-tooltip-buffer-left '+ tooltipBufferPolyClass +'"></div>';
                        }
                    }

                    if (s.tooltip_content.content_type == 'plain-text') {
                        var style = '';
                        style += 'color: ' + s.tooltip_content.plain_text_color + ';';

                        html += '<div class="imp-tooltip-plain-text" style="'+ style +'">' + s.tooltip_content.plain_text + '</div>';
                    } else {
						if (s.tooltip_content.squares_json) {
							html += $.squaresRendererRenderObject(s.tooltip_content.squares_json);
						} else {
							html += $.squaresRendererRenderObject(s.tooltip_content.squares_settings);
						}
                    }

                    html += '</div>';
                }
                
                self.wrap.prepend(html);
            }
        },
        initFullscreen: function() {
            var self = this;

            if (parseInt(self.settings.fullscreen.enable_fullscreen_mode, 10) == 1) {
                // Button style
                var style = '';
                style += 'background: ' + self.settings.fullscreen.fullscreen_button_color + '; ';
                style += 'color: ' + self.settings.fullscreen.fullscreen_button_text_color + '; ';

                // Button content
                var icon = '<i class="fa fa-arrows-alt" aria-hidden="true"></i>';
                if (parseInt(self.settings.runtime.is_fullscreen, 10) == 1) {
                    icon = '<i class="fa fa-times" aria-hidden="true"></i>';
                }

                var text = 'Go Fullscreen';
                if (parseInt(self.settings.runtime.is_fullscreen, 10) == 1) {
                    text = 'Close Fullscreen';
                }

                var buttonContent = '';
                if (self.settings.fullscreen.fullscreen_button_type == 'icon') {
                    buttonContent += icon;
                }
                if (self.settings.fullscreen.fullscreen_button_type == 'text') {
                    buttonContent += text;
                }
                if (self.settings.fullscreen.fullscreen_button_type == 'icon_and_text') {
                    buttonContent += icon + ' ' + text;
                }

                // Button classes
                var classes = '';
                if (self.settings.fullscreen.fullscreen_button_type == 'icon') {
                    classes += 'imp-fullscreen-button-icon-only';
                }

                // Button html
                var html = '';
                html += '<div style="'+ style +'" class="'+ classes +' imp-fullscreen-button imp-fullscreen-button-position-'+ self.settings.fullscreen.fullscreen_button_position +'">';
                html += buttonContent;
                html += '</div>';
                
                // Append
                self.wrap.append('<div class="imp-ui"></div>');
                self.ui = self.wrap.find('.imp-ui');
                self.ui.append(html);

                // Correct the button's position
                var btn = self.ui.find('.imp-fullscreen-button');

                if (parseInt(self.settings.fullscreen.fullscreen_button_position, 10) == 1 || parseInt(self.settings.fullscreen.fullscreen_button_position, 10) == 4) {
                    btn.css({ "margin-left" : - btn.outerWidth()/2 });
                }

                // Start in fullscreen mode
                if (parseInt(self.settings.fullscreen.start_in_fullscreen_mode, 10) == 1 && self.settings.runtime.is_fullscreen == 0) {
                    self.toggleFullscreen();
                }
            }
        },
        measureTooltipSize: function(i) {
            // Size needs to be calculated just before
            // the tooltip displays, and for the specific tooltip only.
            // No calculations needed if in fullscreen mode

            // 1. Does size need to be calculated?
            if (this.settings.tooltips.fullscreen_tooltips == 'always' || (this.settings.tooltips.fullscreen_tooltips == 'mobile' && isMobile())) return;

            var s = this.settings.spots[i];
            var t = this.wrap.find('.imp-tooltip[data-index="'+ i +'"]');

            // 2. If the tooltip has manual width, set it
            if (parseInt(s.tooltip_style.auto_width, 10) == 0) {
                t.css({
                    width: s.tooltip_style.width
                });
            }

            // 3. Measure width/height
            t.data('imp-measured-width', t.outerWidth());
            t.data('imp-measured-height', t.outerHeight());
        },
        animateShapesLoop: function() {
            if (this.settings.general.pageload_animation == 'none') return;

            var interval = 750 / this.settings.spots.length;
            var shapesRandomOrderArray = shuffle(this.settings.spots.slice());

            for (var i=0; i<shapesRandomOrderArray.length; i++) {
                this.animateShape(shapesRandomOrderArray[i], interval * i);
            }
        },
        animateShape: function(shape, delay) {
            var self = this;
            var spotEl = $('#' + shape.id);

            setTimeout(function() {
                if (self.settings.general.pageload_animation == 'fade') {
                    spotEl.css({
                        opacity: shape.default_style.opacity
                    });
                }
                if (self.settings.general.pageload_animation == 'grow') {
                    spotEl.css({
                        transform: 'scale(1, 1)',
                        '-moz-transform': 'scale(1, 1)',
                        '-webkit-transform': 'scale(1, 1)'
                    });
                }
            }, delay);
        },
        events: function() {
            // to do - complete rewrite
            // events will search for the first shape, which is within the event
            // coordinates.
            var self = this;

            // Mouse events
            this.wrap.off('mousemove');
            this.wrap.on('mousemove', function(e) {
                if (self.touch) return;
                self.handleEventMove(e);
            });
            this.wrap.off('mouseup');
            this.wrap.on('mouseup', function(e) {
                if (self.touch) return;
                self.handleEventEnd(e);
            });

            // Touch events
            this.wrap.off('touchstart');
            this.wrap.on('touchstart', function(e) {
                self.touch = true;
                self.handleEventMove(e);
            });
            this.wrap.off('touchmove');
            this.wrap.on('touchmove', function(e) {
                self.handleEventMove(e);
            });
            this.wrap.off('touchend');
            this.wrap.on('touchend', function(e) {
                self.handleEventEnd(e);
            });

            // Hide tooltips when mouse leaves the image map container
            $(document).off('mousemove.' + this.settings.id);
            $(document).on('mousemove.' + this.settings.id, function(e) {
                if (self.touch) return;

                if (self.manuallyHighlightedShape || self.manuallyShownTooltip) return;

                if ($(e.target).closest('.imp-wrap').length == 0 && $(e.target).closest('.imp-fullscreen-tooltips-container').length == 0) {
                    if (self.visibleTooltip) {
                        self.hideTooltip();
                    }
                    if (self.clickedShape) {
                        self.unclickShape();
                    }
                    if (self.highlightedShape) {
                        self.unhighlightShape();
                    }
                }
            });
            $(document).off('touchstart.' + this.settings.id);
            $(document).on('touchstart.' + this.settings.id, function(e) {
                if (self.manuallyHighlightedShape || self.manuallyShownTooltip) return;

                if ($(e.target).closest('.imp-wrap').length == 0 && $(e.target).closest('.imp-fullscreen-tooltips-container').length == 0) {
                    if (self.visibleTooltip) {
                        self.hideTooltip();
                    }
                    if (self.clickedShape) {
                        self.unclickShape();
                    }
                    if (self.highlightedShape) {
                        self.unhighlightShape();
                    }
                }
            });

            // Tooltips close button
            $(document).off('click.' + this.settings.id, '.imp-tooltip-close-button');
            $(document).on('click.' + this.settings.id, '.imp-tooltip-close-button', function() {
                self.hideTooltip();

                if (self.clickedShape) {
                    self.unclickShape();
                }
                if (self.highlightedShape) {
                    self.unhighlightShape();
                }
            });

            if (parseInt(this.settings.general.late_initialization, 10) == 1) {
                if (!mutationObserver) {
                    mutationObserver = new MutationObserver(function(mutations, observer) {
                        clearTimeout(self.initTimeout);
                        self.initTimeout = setTimeout(function() {
                            for (var i=0; i<mutations.length; i++) {
                                // Check if the mutation is not in an image map
                                if ($(mutations[i].target).closest('.imp-initialized').length == 0 && !$(mutations[i].target).hasClass('imp-initialized')) {
                                    self.init();
                                    break;
                                }
                            }
                        }, 50);
                    });

                    mutationObserver.observe(document, {
                        subtree: true,
                        attributes: true
                    });
                }
            } else {
                if (mutationObserver) {
                    mutationObserver.disconnect();
                    mutationObserver = undefined;
                }
            }

            // Fullscreen button
            $(document).off('click.' + this.settings.id, '[data-image-map-pro-id="' + this.settings.id + '"] .imp-fullscreen-button');
            $(document).on('click.' + this.settings.id, '[data-image-map-pro-id="' + this.settings.id + '"] .imp-fullscreen-button', function() {
                self.toggleFullscreen();
            });

            /*

            HTML API
            ---------------------------------------
            data-imp-highlight-shape-on-mouseover
            data-imp-highlight-shape-on-click
            data-imp-unhighlight-shape-on-mouseover
            data-imp-unhighlight-shape-on-click

            data-imp-open-tooltip-on-mouseover
            data-imp-open-tooltip-on-click
            data-imp-close-tooltip-on-mouseover
            data-imp-close-tooltip-on-click

            data-trigger-shape-on-mouseover
            data-trigger-shape-on-click
            data-untrigger-shape-on-mouseover
            data-untrigger-shape-on-click

            */

            // HTML API - SHAPE

            // [data-imp-highlight-shape-on-mouseover]
            $(document).on('mouseover', '[data-imp-highlight-shape-on-mouseover]', function() {
                var shapeName = $(this).data('imp-highlight-shape-on-mouseover');
                var mapName = $(this).data('imp-image-map-name');
                
                if (!mapName || mapName == self.settings.general.name) {
                    var i = $('[data-shape-title="' + shapeName + '"]').data('index');
                    
                    self.manuallyHighlightedShape = true;
                    self.highlightShape(i, true);
                }
            });
            $(document).on('mouseout', '[data-imp-highlight-shape-on-mouseover]', function() {
                var shapeName = $(this).data('imp-highlight-shape-on-mouseover');
                var mapName = $(this).data('imp-image-map-name');
                
                if (!mapName || mapName == self.settings.general.name) {
                    self.unhighlightShape();
                }
            });

            // [data-imp-highlight-shape-on-click]
            $(document).on('click', '[data-imp-highlight-shape-on-click]', function() {
                var shapeName = $(this).data('imp-highlight-shape-on-click');
                var mapName = $(this).data('imp-image-map-name');
                
                if (!mapName || mapName == self.settings.general.name) {
                    var i = $('[data-shape-title="' + shapeName + '"]').data('index');
                    
                    if (self.highlightedShape) self.unhighlightShape();

                    self.manuallyHighlightedShape = true;
                    self.highlightShape(i, true);
                }
            });

            // [data-imp-unhighlight-shape-on-mouseover]
            $(document).on('mouseover', '[data-imp-unhighlight-shape-on-mouseover]', function() {
                var shapeName = $(this).data('imp-unhighlight-shape-on-mouseover');
                var mapName = $(this).data('imp-image-map-name');
                
                if (!mapName || mapName == self.settings.general.name) {
                    if (self.highlightedShape) self.unhighlightShape();
                }
            });

            // [data-imp-unhighlight-shape-on-click]
            $(document).on('mouseover', '[data-imp-unhighlight-shape-on-click]', function() {
                var shapeName = $(this).data('imp-unhighlight-shape-on-click');
                var mapName = $(this).data('imp-image-map-name');
                
                if (!mapName || mapName == self.settings.general.name) {
                    if (self.highlightedShape) self.unhighlightShape();
                }
            });

            // HTML API - TOOLTIP

            // [data-imp-open-tooltip-on-mouseover]
            $(document).on('mouseover', '[data-imp-open-tooltip-on-mouseover]', function() {
                var shapeName = $(this).data('imp-open-tooltip-on-mouseover');
                var mapName = $(this).data('imp-image-map-name');
                
                if (!mapName || mapName == self.settings.general.name) {
                    var i = $('[data-shape-title="' + shapeName + '"]').data('index');
                    
                    self.manuallyShownTooltip = true;
                    self.showTooltip(i);
                    self.updateTooltipPosition(i);
                }
            });
            $(document).on('mouseout', '[data-imp-open-tooltip-on-mouseover]', function() {
                var shapeName = $(this).data('imp-open-tooltip-on-mouseover');
                var mapName = $(this).data('imp-image-map-name');
                
                if (!mapName || mapName == self.settings.general.name) {
                    self.hideTooltip();
                }
            });

            // [data-imp-open-tooltip-on-click]
            $(document).on('click', '[data-imp-open-tooltip-on-click]', function() {
                var shapeName = $(this).data('imp-open-tooltip-on-click');
                var mapName = $(this).data('imp-image-map-name');
                
                if (!mapName || mapName == self.settings.general.name) {
                    var i = $('[data-shape-title="' + shapeName + '"]').data('index');
                    
                    self.manuallyShownTooltip = true;
                    self.showTooltip(i);
                    self.updateTooltipPosition(i);
                }
            });

            // [data-imp-close-tooltip-on-mouseover]
            $(document).on('mouseover', '[data-imp-close-tooltip-on-mouseover]', function() {
                var shapeName = $(this).data('imp-close-tooltip-on-mouseover');
                var mapName = $(this).data('imp-image-map-name');
                
                if (!mapName || mapName == self.settings.general.name) {
                    self.hideTooltip();
                }
            });

            // [data-imp-close-tooltip-on-click]
            $(document).on('click', '[data-imp-close-tooltip-on-click]', function() {
                var shapeName = $(this).data('imp-close-tooltip-on-click');
                var mapName = $(this).data('imp-image-map-name');
                
                if (!mapName || mapName == self.settings.general.name) {
                    self.hideTooltip();
                }
            });

            // HTML API - TRIGGER

            // [data-imp-trigger-shape-on-mouseover]
            $(document).on('mouseover', '[data-imp-trigger-shape-on-mouseover]', function() {
                var shapeName = $(this).data('imp-trigger-shape-on-mouseover');
                var mapName = $(this).data('imp-image-map-name');
                
                if (!mapName || mapName == self.settings.general.name) {
                    var i = $('[data-shape-title="' + shapeName + '"]').data('index');
                    
                    self.manuallyHighlightedShape = true;
                    self.manuallyShownTooltip = true;
                    self.highlightShape(i, true);
                    self.showTooltip(i);
                    self.updateTooltipPosition(i);
                }
            });
            $(document).on('mouseout', '[data-imp-trigger-shape-on-mouseover]', function() {
                var shapeName = $(this).data('imp-trigger-shape-on-mouseover');
                var mapName = $(this).data('imp-image-map-name');
                
                if (!mapName || mapName == self.settings.general.name) {
                    self.unhighlightShape();
                    self.hideTooltip();
                }
            });

            // [data-imp-trigger-shape-on-click]
            $(document).on('click', '[data-imp-trigger-shape-on-click]', function() {
                var shapeName = $(this).data('imp-trigger-shape-on-click');
                var mapName = $(this).data('imp-image-map-name');
                
                if (!mapName || mapName == self.settings.general.name) {
                    var i = $('[data-shape-title="' + shapeName + '"]').data('index');
                    
                    if (self.highlightedShape) self.unhighlightShape();

                    self.manuallyHighlightedShape = true;
                    self.manuallyShownTooltip = true;
                    self.highlightShape(i, true);
                    self.showTooltip(i);
                    self.updateTooltipPosition(i);
                }
            });

            // [data-imp-untrigger-shape-on-mouseover]
            $(document).on('mouseover', '[data-imp-untrigger-shape-on-mouseover]', function() {
                var shapeName = $(this).data('imp-untrigger-shape-on-mouseover');
                var mapName = $(this).data('imp-image-map-name');
                
                if (!mapName || mapName == self.settings.general.name) {
                    if (self.highlightedShape) self.unhighlightShape();

                    self.hideTooltip();
                }
            });

            // [data-imp-untrigger-shape-on-click]
            $(document).on('mouseover', '[data-imp-untrigger-shape-on-click]', function() {
                var shapeName = $(this).data('imp-untrigger-shape-on-click');
                var mapName = $(this).data('imp-image-map-name');
                
                if (!mapName || mapName == self.settings.general.name) {
                    if (self.highlightedShape) self.unhighlightShape();

                    self.hideTooltip();
                }
            });
        },
        disableEvents: function() {
            this.wrap.off('mousemove');
            this.wrap.off('mouseup');

            // Touch events
            this.wrap.off('touchstart');
            this.wrap.off('touchmove');
            this.wrap.off('touchend');

            // Hide tooltips when mouse leaves the image map container
            $(document).off('mousemove.' + this.settings.id);
            $(document).off('touchstart.' + this.settings.id);

            // Tooltips close button
            $(document).off('click.' + this.settings.id, '.imp-tooltip-close-button');

            // Fullscreen button
            $(document).off('click.' + this.settings.id, '.imp-fullscreen-button');
        },
        handleEventMove: function(e) {
            // If there is a visible fullscreen tooltip, return
            if (this.fullscreenTooltipVisible) return;

            // If the mouse is over a tooltip AND sticky tooltips are OFF, return
            if (($(e.target).closest('.imp-tooltip').length != 0 || $(e.target).hasClass('imp-tooltip')) && parseInt(this.settings.tooltips.sticky_tooltips, 10) == 0) return;

            // If there is a manually highlightedShape / visible tooltip, remove the flags
            if (this.manuallyHighlightedShape || this.manuallyShownTooltip) {
                this.manuallyHighlightedShape = false;
                this.manuallyShownTooltip = false;
            }

            // Get event data
            var c = this.getEventRelativeCoordinates(e);
            var i = this.matchShapeToCoords(c);

            // Highlight logic
            if (i != -1 && i != this.highlightedShapeIndex) {
                if (this.highlightedShape) {
                    this.unhighlightShape();
                }
                
                this.highlightShape(i, true);
            } else if (i == -1 && this.highlightedShape && this.highlightedShapeIndex != this.clickedShapeIndex) {
                this.unhighlightShape();
            }

            // Tooltips logic
            if (this.highlightedShape && this.visibleTooltipIndex != this.highlightedShapeIndex) {
                if (this.highlightedShape.actions.mouseover == 'show-tooltip') {
                    if (this.visibleTooltip) {
                        this.hideTooltip();
                    }

                    if (this.clickedShape) {
                        this.unclickShape();
                    }

                    this.showTooltip(this.highlightedShapeIndex);
                    this.updateTooltipPosition(i, e);
                }
            } else if (this.visibleTooltip && !this.highlightedShape && !this.clickedShape) {
                if (this.visibleTooltip) {
                    this.hideTooltip();
                }
            }

            // If there is a shape active and sticky tooltips is on,
            // update the position of the visible tooltip
            if (this.visibleTooltip && this.highlightedShape && parseInt(this.settings.tooltips.sticky_tooltips, 10) == 1 && this.highlightedShape.actions.mouseover == 'show-tooltip') {
                this.updateTooltipPosition(this.highlightedShapeIndex, e);
            }
        },
        handleEventEnd: function(e) {
			// Did the user click on a tooltip?
			if ($(e.target).closest('.imp-tooltip').length != 0 && !$(e.target).hasClass('imp-tooltip-buffer')) {
				return;
			}

            // If there is a visible fullscreen tooltip, return
            if (this.fullscreenTooltipVisible) return;

            // If there is a manually highlightedShape / visible tooltip, remove the flags
            if (this.manuallyHighlightedShape || this.manuallyShownTooltip) {
                this.manuallyHighlightedShape = false;
                this.manuallyShownTooltip = false;
            }

            // Get event data
            var c = this.getEventRelativeCoordinates(e);
            var i = this.matchShapeToCoords(c);

            // Click logic
            if (i != -1 && i != this.clickedShapeIndex) {
                if (this.clickedShape) {
                    this.unclickShape();
                }

                this.clickShape(i, e);
            } else if (i == -1 && this.clickedShape) {
                this.unclickShape();
            }
        },

        getEventRelativeCoordinates: function(e) {
            var x, y;

            if (e.type == 'touchstart' || e.type == 'touchmove' || e.type == 'touchend' || e.type == 'touchcancel') {
                var touch = e.originalEvent.touches[0] || e.originalEvent.changedTouches[0];
                x = touch.pageX;
                y = touch.pageY;
            } else if (e.type == 'mousedown' || e.type == 'mouseup' || e.type == 'mousemove' || e.type == 'mouseover'|| e.type=='mouseout' || e.type=='mouseenter' || e.type=='mouseleave') {
                x = e.pageX;
                y = e.pageY;
            }

            // Make coordinates relative to the container
            x -= this.wrap.offset().left;
            y -= this.wrap.offset().top;

            // Take window scroll into account
            // x += $(window).scrollLeft();
            // y += $(window).scrollTop();

            // Convert coordinates to %
            x = (x / this.wrap.width()) * 100;
            y = (y / this.wrap.height()) * 100;

            return { x: x, y: y };
        },
        getEventCoordinates: function(e) {
            var x, y;

            if (e.type == 'touchstart' || e.type == 'touchmove' || e.type == 'touchend' || e.type == 'touchcancel') {
                var touch = e.originalEvent.touches[0] || e.originalEvent.changedTouches[0];
                x = touch.pageX;
                y = touch.pageY;
            } else if (e.type == 'mousedown' || e.type == 'mouseup' || e.type == 'mousemove' || e.type == 'mouseover'|| e.type=='mouseout' || e.type=='mouseenter' || e.type=='mouseleave') {
                x = e.pageX;
                y = e.pageY;
            }

            return { x: x, y: y };
        },
        matchShapeToCoords: function(c) {
            for (var i=this.settings.spots.length - 1; i>=0; i--) {
                var s = this.settings.spots[i];

                if (s.type == 'poly') {
                    var x = (c.x / 100) * this.wrap.width();
                    var y = (c.y / 100) * this.wrap.height();

                    x = (x * this.settings.general.width) / this.wrap.width();
                    y = (y * this.settings.general.height) / this.wrap.height();

                    if (isPointInsidePolygon(x, y, s.vs)) {
                        return i;
                        break;
                    }
                }

                if (s.type == 'spot') {
                    var shapeWidth = (s.width < 44) ? 44 : s.width;
                    var shapeHeight = (s.height < 44) ? 44 : s.height;

                    var x = (c.x/100) * this.wrap.width();
                    var y = (c.y/100) * this.wrap.height();
                    var rx = (s.x/100) * this.wrap.width() - shapeWidth/2;
                    var ry = (s.y/100) * this.wrap.height() - shapeHeight/2;
                    var rw = shapeWidth;
                    var rh = shapeHeight;

                    if (parseInt(s.default_style.icon_is_pin, 10) == 1 && parseInt(s.default_style.use_icon, 10) == 1) {
                        ry -= shapeHeight/2;

                        if (s.height < 44) {
                            ry += s.height/2;
                        }
                    }

                    if (isPointInsideRect(x, y, rx, ry, rw, rh)) {
                        return i;
                        break;
                    }
                }

                if (s.type == 'rect') {
                    if (isPointInsideRect(c.x, c.y, s.x, s.y, s.width, s.height)) {
                        return i;
                        break;
                    }
                }

                if (s.type == 'oval') {
                    var x = c.x;
                    var y = c.y;
                    var ex = s.x + s.width/2;
                    var ey = s.y + s.height/2;
                    var rx = s.width/2;
                    var ry = s.height/2;

                    if (isPointInsideEllipse(x, y, ex, ey, rx, ry)) {
                        return i;
                        break;
                    }
                }
            }

            return -1;
        },

        applyMouseoverStyles: function(i) {
            var self = this;

            var s = self.settings.spots[i];
            // If it's an icon, return
            // if (s.type == 'spot' && parseInt(s.default_style.use_icon, 10) == 1) return;

            var el = this.wrap.find('#' + s.id);

            var style = '';

            if (s.type == 'spot' && parseInt(s.default_style.use_icon, 10) == 0) {
                var color_bg = hexToRgb(s.mouseover_style.background_color) || { r: 0, b: 0, g: 0 };
                var color_border = hexToRgb(s.mouseover_style.border_color) || { r: 0, b: 0, g: 0 };

                style += 'left: '+ s.x +'%;';
                style += 'top: '+ s.y +'%;';
                
                style += 'width: '+ s.width +'px;';
                style += 'height: '+ s.height +'px;';
                style += 'margin-left: -'+ s.width/2 +'px;';
                style += 'margin-top: -'+ s.height/2 +'px;';

                style += 'opacity: ' + s.mouseover_style.opacity + ';';
                style += 'border-radius: ' + s.mouseover_style.border_radius + 'px;';
                style += 'background: rgba('+ color_bg.r +', '+ color_bg.g +', '+ color_bg.b +', '+ s.mouseover_style.background_opacity +');';
                style += 'border-width: ' + s.mouseover_style.border_width + 'px;';
                style += 'border-style: ' + s.mouseover_style.border_style + ';';
                style += 'border-color: rgba('+ color_border.r +', '+ color_border.g +', '+ color_border.b +', '+ s.mouseover_style.border_opacity +');';
            }
            if (s.type == 'spot' && parseInt(s.default_style.use_icon, 10) == 1) {
                // If it's an icon, apply opacity
                var wrapperWidth = (s.width < 44) ? 44 : s.width;
                var wrapperHeight = (s.height < 44) ? 44 : s.height;

                style += 'left: '+ s.x +'%;';
                style += 'top: '+ s.y +'%;';
                style += 'width: '+ wrapperWidth +'px;';
                style += 'height: '+ wrapperHeight +'px;';

                // If the icon is a pin, center it on the bottom edge
                var marginTop = -wrapperHeight/2;
                var marginLeft = -wrapperWidth/2;

                if (parseInt(s.default_style.icon_is_pin, 10) == 1) {
                    marginTop = -wrapperHeight;

                    if (s.height < 44) {
                        marginTop = -wrapperHeight/2 -s.height/2;
                    }
                }

                style += 'margin-left: '+ marginLeft +'px;';
                style += 'margin-top: '+ marginTop +'px;';

                style += 'opacity: ' + s.mouseover_style.opacity + ';';
            }
            if (s.type == 'spot' && parseInt(s.default_style.use_icon, 10) == 1 && s.default_style.icon_type == 'library') {
                // If it's an icon and it's from library, apply SVG fill
                el.find('path').attr('style', 'fill:' + s.mouseover_style.icon_fill);
            }
            if (s.type == 'rect') {
                var color_bg = hexToRgb(s.mouseover_style.background_color) || { r: 0, b: 0, g: 0 };
                var color_border = hexToRgb(s.mouseover_style.border_color) || { r: 0, b: 0, g: 0 };

                style += 'left: '+ s.x +'%;';
                style += 'top: '+ s.y +'%;';
                style += 'width: '+ s.width +'%;';
                style += 'height: '+ s.height +'%;';

                style += 'opacity: ' + s.mouseover_style.opacity + ';';
                style += 'border-radius: ' + s.mouseover_style.border_radius + 'px;';
                style += 'background: rgba('+ color_bg.r +', '+ color_bg.g +', '+ color_bg.b +', '+ s.mouseover_style.background_opacity +');';
                style += 'border-width: ' + s.mouseover_style.border_width + 'px;';
                style += 'border-style: ' + s.mouseover_style.border_style + ';';
                style += 'border-color: rgba('+ color_border.r +', '+ color_border.g +', '+ color_border.b +', '+ s.mouseover_style.border_opacity +');';
            }
            if (s.type == 'oval') {
                var color_bg = hexToRgb(s.mouseover_style.background_color) || { r: 0, b: 0, g: 0 };
                var color_border = hexToRgb(s.mouseover_style.border_color) || { r: 0, b: 0, g: 0 };

                style += 'left: '+ s.x +'%;';
                style += 'top: '+ s.y +'%;';
                style += 'width: '+ s.width +'%;';
                style += 'height: '+ s.height +'%;';

                style += 'opacity: ' + s.mouseover_style.opacity + ';';
                style += 'border-radius: 50% 50%;';
                style += 'background: rgba('+ color_bg.r +', '+ color_bg.g +', '+ color_bg.b +', '+ s.mouseover_style.background_opacity +');';
                style += 'border-width: ' + s.mouseover_style.border_width + 'px;';
                style += 'border-style: ' + s.mouseover_style.border_style + ';';
                style += 'border-color: rgba('+ color_border.r +', '+ color_border.g +', '+ color_border.b +', '+ s.mouseover_style.border_opacity +');';
            }
            if (s.type == 'poly') {
                var c_fill = hexToRgb(s.mouseover_style.fill) || { r: 0, b: 0, g: 0 };
                var c_stroke = hexToRgb(s.mouseover_style.stroke_color) || { r: 0, b: 0, g: 0 };

                style += 'opacity: ' + s.mouseover_style.opacity + ';';
                style += 'fill: rgba('+ c_fill.r +', '+ c_fill.g +', '+ c_fill.b +', '+ s.mouseover_style.fill_opacity +');';
                style += 'stroke: rgba('+ c_stroke.r +', '+ c_stroke.g +', '+ c_stroke.b +', '+ s.mouseover_style.stroke_opacity +');';
                style += 'stroke-width: ' + s.mouseover_style.stroke_width + 'px;';
                style += 'stroke-dasharray: ' + s.mouseover_style.stroke_dasharray + ';';
                style += 'stroke-linecap: ' + s.mouseover_style.stroke_linecap + ';';
            }

            el.attr('style', style);
        },
        applyDefaultStyles: function(i) {
            var self = this;
            var s = self.settings.spots[i];

            // If it's an icon, return
            // if (s.type == 'spot' && parseInt(s.default_style.use_icon, 10) == 1) return;
            var el = this.wrap.find('#' + s.id);
            var style = '';

            if (s.type == 'spot' && parseInt(s.default_style.use_icon, 10) == 0) {
                var color_bg = hexToRgb(s.default_style.background_color) || { r: 0, b: 0, g: 0 };
                var color_border = hexToRgb(s.default_style.border_color) || { r: 0, b: 0, g: 0 };

                style += 'left: '+ s.x +'%;';
                style += 'top: '+ s.y +'%;';
                style += 'width: '+ s.width +'px;';
                style += 'height: '+ s.height +'px;';
                style += 'margin-left: -'+ s.width/2 +'px;';
                style += 'margin-top: -'+ s.height/2 +'px;';

                style += 'opacity: ' + s.default_style.opacity + ';';
                style += 'border-radius: ' + s.default_style.border_radius + 'px;';
                style += 'background: rgba('+ color_bg.r +', '+ color_bg.g +', '+ color_bg.b +', '+ s.default_style.background_opacity +');';
                style += 'border-width: ' + s.default_style.border_width + 'px;';
                style += 'border-style: ' + s.default_style.border_style + ';';
                style += 'border-color: rgba('+ color_border.r +', '+ color_border.g +', '+ color_border.b +', '+ s.default_style.border_opacity +');';
            }
            if (s.type == 'spot' && parseInt(s.default_style.use_icon, 10) == 1) {
                // If it's an icon, apply opacity
                var wrapperWidth = (s.width < 44) ? 44 : s.width;
                var wrapperHeight = (s.height < 44) ? 44 : s.height;

                style += 'left: '+ s.x +'%;';
                style += 'top: '+ s.y +'%;';
                style += 'width: '+ wrapperWidth +'px;';
                style += 'height: '+ wrapperHeight +'px;';

                // If the icon is a pin, center it on the bottom edge
                var marginTop = -wrapperHeight/2;
                var marginLeft = -wrapperWidth/2;

                if (parseInt(s.default_style.icon_is_pin, 10) == 1) {
                    marginTop = -wrapperHeight;

                    if (s.height < 44) {
                        marginTop = -wrapperHeight/2 -s.height/2;
                    }
                }

                style += 'margin-left: '+ marginLeft +'px;';
                style += 'margin-top: '+ marginTop +'px;';

                style += 'opacity: ' + s.default_style.opacity + ';';
            }
            if (s.type == 'spot' && parseInt(s.default_style.use_icon, 10) == 1 && s.default_style.icon_type == 'library') {
                // If it's an icon and it's from library, apply SVG fill

                el.find('path').attr('style', 'fill:' + s.default_style.icon_fill);
            }
            if (s.type == 'rect') {
                var color_bg = hexToRgb(s.default_style.background_color) || { r: 0, b: 0, g: 0 };
                var color_border = hexToRgb(s.default_style.border_color) || { r: 0, b: 0, g: 0 };

                style += 'left: '+ s.x +'%;';
                style += 'top: '+ s.y +'%;';
                style += 'width: '+ s.width +'%;';
                style += 'height: '+ s.height +'%;';

                style += 'opacity: ' + s.default_style.opacity + ';';
                style += 'border-radius: ' + s.default_style.border_radius + 'px;';
                style += 'background: rgba('+ color_bg.r +', '+ color_bg.g +', '+ color_bg.b +', '+ s.default_style.background_opacity +');';
                style += 'border-width: ' + s.default_style.border_width + 'px;';
                style += 'border-style: ' + s.default_style.border_style + ';';
                style += 'border-color: rgba('+ color_border.r +', '+ color_border.g +', '+ color_border.b +', '+ s.default_style.border_opacity +');';
            }
            if (s.type == 'oval') {
                var color_bg = hexToRgb(s.default_style.background_color) || { r: 0, b: 0, g: 0 };
                var color_border = hexToRgb(s.default_style.border_color) || { r: 0, b: 0, g: 0 };

                style += 'left: '+ s.x +'%;';
                style += 'top: '+ s.y +'%;';
                style += 'width: '+ s.width +'%;';
                style += 'height: '+ s.height +'%;';

                style += 'opacity: ' + s.default_style.opacity + ';';
                style += 'border-radius: 50% 50%;';
                style += 'background: rgba('+ color_bg.r +', '+ color_bg.g +', '+ color_bg.b +', '+ s.default_style.background_opacity +');';
                style += 'border-width: ' + s.default_style.border_width + 'px;';
                style += 'border-style: ' + s.default_style.border_style + ';';
                style += 'border-color: rgba('+ color_border.r +', '+ color_border.g +', '+ color_border.b +', '+ s.default_style.border_opacity +');';
            }
            if (s.type == 'poly') {
                var c_fill = hexToRgb(s.default_style.fill) || { r: 0, b: 0, g: 0 };
                var c_stroke = hexToRgb(s.default_style.stroke_color) || { r: 0, b: 0, g: 0 };

                style += 'opacity: ' + s.default_style.opacity + ';';
                style += 'fill: rgba('+ c_fill.r +', '+ c_fill.g +', '+ c_fill.b +', '+ s.default_style.fill_opacity +');';
                style += 'stroke: rgba('+ c_stroke.r +', '+ c_stroke.g +', '+ c_stroke.b +', '+ s.default_style.stroke_opacity +');';
                style += 'stroke-width: ' + s.default_style.stroke_width + 'px;';
                style += 'stroke-dasharray: ' + s.default_style.stroke_dasharray + ';';
                style += 'stroke-linecap: ' + s.default_style.stroke_linecap + ';';
            }

            el.attr('style', style);
        },

        highlightShape: function(i, sendEvent) {
            this.applyMouseoverStyles(i);
            this.highlightedShapeIndex = i;
            this.highlightedShape = this.settings.spots[i];

            if (sendEvent) {
                $.imageMapProEventHighlightedShape(this.settings.general.name, this.highlightedShape.title);
            }
        },
        unhighlightShape: function() {
            this.applyDefaultStyles(this.highlightedShapeIndex);

            // Send event
            $.imageMapProEventUnhighlightedShape(this.settings.general.name, this.highlightedShape.title);

            // Reset vars
            this.highlightedShapeIndex = undefined;
            this.highlightedShape = undefined;
        },
        clickShape: function(i, e) {
            if (this.settings.spots[i].actions.click == 'show-tooltip') {
                this.applyMouseoverStyles(i);
                this.showTooltip(i);
                this.updateTooltipPosition(i, e);

                this.clickedShapeIndex = i;
                this.clickedShape = this.settings.spots[i];
            }
            if (this.settings.spots[i].actions.click == 'follow-link') {
                if ($('#imp-temp-link').length == 0) {
                    $('body').append('<a href="" id="imp-temp-link" target="_blank"></a>');
                }
                $('#imp-temp-link').attr('href', this.settings.spots[i].actions.link);

                if (parseInt(this.settings.spots[i].actions.open_link_in_new_window, 10) == 1) {
                    $('#imp-temp-link').attr('target', '_blank');
                } else {
                    $('#imp-temp-link').removeAttr('target');
                }

                $('#imp-temp-link')[0].click();
            }

            $.imageMapProEventClickedShape(this.settings.general.name, this.settings.spots[i].title);
        },
        unclickShape: function() {
            this.applyDefaultStyles(this.clickedShapeIndex);

            if (this.clickedShape.actions.click == 'show-tooltip') {
                this.hideTooltip();
            }

            this.clickedShapeIndex = undefined;
            this.clickedShape = undefined;
        },

        showTooltip: function(i) {
            if ((this.settings.tooltips.fullscreen_tooltips == 'mobile-only' && isMobile()) || this.settings.tooltips.fullscreen_tooltips == 'always') {
                // Fullscreen tooltips
                this.visibleTooltip = $('.imp-fullscreen-tooltip[data-index="'+ i +'"]');
                this.visibleTooltipIndex = i;
                this.fullscreenTooltipsContainer.show();

                var self = this;
                setTimeout(function() {
                    self.visibleTooltip.addClass('imp-tooltip-visible');
                }, 20);

                this.fullscreenTooltipVisible = true;

                // Prevent scrolling of the body and store the original overflow attribute value
                this.bodyOverflow = $('body').css('overflow');
                $('body').css({
                    overflow: 'hidden'
                });
            } else {
                // Normal tooltips
                $('.imp-tooltip-visible').removeClass('imp-tooltip-visible');
                this.visibleTooltip = this.wrap.find('.imp-tooltip[data-index="'+ i +'"]');
                this.visibleTooltipIndex = i;
                this.visibleTooltip.addClass('imp-tooltip-visible');

                this.measureTooltipSize(i);
            }

            $.imageMapProEventOpenedTooltip(this.settings.general.name, this.settings.spots[i].title);
        },
        hideTooltip: function() {
            $('.imp-tooltip-visible').removeClass('imp-tooltip-visible');
            this.visibleTooltip = undefined;
            this.visibleTooltipIndex = undefined;

            if ((this.settings.tooltips.fullscreen_tooltips == 'mobile-only' && isMobile()) || this.settings.tooltips.fullscreen_tooltips == 'always') {
                var self = this;

                setTimeout(function() {
                    self.fullscreenTooltipsContainer.hide();
                }, 200);

                this.fullscreenTooltipVisible = false;

                // Restore the body overflow to allow scrolling
                $('body').css({
                    overflow: this.bodyOverflow
                });
            }

            $.imageMapProEventClosedTooltip(this.settings.general.name);
        },
        updateTooltipPosition: function(i, e) {
            // t = tooltip element
            // tw/th = tooltip width/height
            // sx/sy/sw/sh = spot x/y/width/height
            // p = padding
            // ex/ey = event x/y
            // s = target shape

            // If fullscreen tooltips are on, then do nothing
            if (this.fullscreenTooltipVisible) return;

            var t, tw, th, sx, sy, sw, sh, p = 20, ex, ey, s;

            t = this.visibleTooltip;
            tw = this.visibleTooltip.data('imp-measured-width');
            th = this.visibleTooltip.data('imp-measured-height');
            s = this.settings.spots[i];

            if (parseInt(this.settings.tooltips.sticky_tooltips, 10) == 1 && s.actions.mouseover == 'show-tooltip' && e) {
                // Sticky tooltips
                // Set width/height of the spot to 0
                // and X and Y to the mouse coordinates
                // Get the event coordinates
                var c = this.getEventCoordinates(e);
                ex = c.x;
                ey = c.y;

                sx = ex - this.wrap.offset().left;
                sy = ey - this.wrap.offset().top;
                sw = 0;
                sh = 0;
            } else {
                // Calculate the position and dimentions of the spot
                if (s.type == 'spot') {
                    sw = s.width;
                    sh = s.height;
                    sx = ((Math.round(s.x*10)/10)/100)*this.wrap.width() - sw/2;
                    sy = ((Math.round(s.y*10)/10)/100)*this.wrap.height() - sh/2;
                } else {
                    sw = (s.width/100)*this.wrap.width();
                    sh = (s.height/100)*this.wrap.height();
                    sx = ((Math.round(s.x*10)/10)/100)*this.wrap.width();
                    sy = ((Math.round(s.y*10)/10)/100)*this.wrap.height();
                }
            }

            // Calculate and set the position
            var x, y;
            if (s.tooltip_style.position == 'left') {
                x = sx - tw - p;
                y = sy + sh/2 - th/2;
            }
            if (s.tooltip_style.position == 'right') {
                x = sx + sw + p;
                y = sy + sh/2 - th/2;
            }
            if (s.tooltip_style.position == 'top') {
                x = sx + sw/2 - tw/2
                y = sy - th - p;
            }
            if (s.tooltip_style.position == 'bottom') {
                x = sx + sw/2 - tw/2;
                y = sy + sh + p;
            }

            // If the spot is a pin, offset it to the top
            if (s.type == 'spot' && parseInt(s.default_style.icon_is_pin, 10) == 1 && s.type == 'spot' && parseInt(s.default_style.use_icon, 10) == 1) {
                y -= sh/2;
            }

            var pos = { x: x, y: y };
            if (parseInt(this.settings.tooltips.constrain_tooltips, 10) == 1) {
                var wrapOffsetLeft = this.wrap.offset().left - $(window).scrollLeft();
                var wrapOffsetTop = this.wrap.offset().top - $(window).scrollTop();

                pos = fitRectToScreen(x + wrapOffsetLeft, y + wrapOffsetTop, tw, th);
                pos.x -= wrapOffsetLeft;
                pos.y -= wrapOffsetTop;
            }

            t.css({
                left: pos.x,
                top: pos.y
            });
        },

        toggleFullscreen: function() {
            if (parseInt(this.settings.runtime.is_fullscreen, 10) == 0) {
                // Go fullscreen
                $('body').addClass('imp-fullscreen-mode');

                var fullscreenSettings = $.extend(true, {}, this.settings);
                fullscreenSettings.runtime.is_fullscreen = 1;
                fullscreenSettings.id = '999999';
                fullscreenSettings.general.responsive = 0;

                var style = '';
                style += 'background: ' + this.settings.fullscreen.fullscreen_background;
                $('body').append('<div id="imp-fullscreen-wrap" style="'+ style +'"><div id="image-map-pro-'+ fullscreenSettings.id +'"></div></div>');

                $('#image-map-pro-'+ fullscreenSettings.id).imageMapPro(fullscreenSettings);

                // Disable current image map
                this.disableEvents();

                fullscreenMap = this;
            } else {
                // Close fullscreen
                $('body').removeClass('imp-fullscreen-mode');
                $('#imp-fullscreen-wrap').remove();
                this.disableEvents();

                fullscreenMap.events();
            }
        },
    });

    // A really lightweight plugin wrapper around the constructor,
    // preventing against multiple instantiations
    $.fn[ pluginName ] = function ( options ) {
        return this.each(function() {
            $.data( this, "plugin_" + pluginName, new Plugin( this, options ) );
        });
    };

    function hexToRgb(hex) {
        var result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hex);
        return result ? {
            r: parseInt(result[1], 16),
            g: parseInt(result[2], 16),
            b: parseInt(result[3], 16)
        } : null;
    }
    function isPointInsideRect(x, y, rx, ry, rw, rh) {
        if (x>=rx && x<=rx+rw && y>=ry && y<=ry+rh) return true;
        return false;
    }
    function isPointInsidePolygon(x, y, vs) {
        // ray-casting algorithm based on
        // http://www.ecse.rpi.edu/Homepages/wrf/Research/Short_Notes/pnpoly.html

        var inside = false;
        for (var i = 0, j = vs.length - 1; i < vs.length; j = i++) {
            var xi = vs[i][0], yi = vs[i][1];
            var xj = vs[j][0], yj = vs[j][1];

            var intersect = ((yi > y) != (yj > y))
            && (x < (xj - xi) * (y - yi) / (yj - yi) + xi);
            if (intersect) inside = !inside;
        }

        return inside;
    }
    function isPointInsideEllipse(x, y, ex, ey, rx, ry) {
        var a = (x - ex)*(x - ex);
        var b = rx*rx;
        var c = (y - ey)*(y - ey);
        var d = ry*ry;

        if (a/b + c/d <= 1) return true;

        return false;
    }
    function fitRectToScreen(x, y, w, h) {
        if (x < 0) x = 0;
        if (y < 0) y = 0;
        if (x > $(document).width() - w) x = $(document).width() - w;
        if (y > $(document).height() - h) y = $(document).height() - h;

        return { x: x, y: y };
    }
    function shuffle(array) {
        var currentIndex = array.length, temporaryValue, randomIndex ;

        // While there remain elements to shuffle...
        while (0 !== currentIndex) {

            // Pick a remaining element...
            randomIndex = Math.floor(Math.random() * currentIndex);
            currentIndex -= 1;

            // And swap it with the current element.
            temporaryValue = array[currentIndex];
            array[currentIndex] = array[randomIndex];
            array[randomIndex] = temporaryValue;
        }

        return array;
    }
    function isMobile() {
        if ( /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)) {
            return true;
        }

        return false;
    }

})( jQuery, window, document );
