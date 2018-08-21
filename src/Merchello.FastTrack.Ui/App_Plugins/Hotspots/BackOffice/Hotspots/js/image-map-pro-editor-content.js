;(function ( $, window, document, undefined ) {
    $.image_map_pro_editor_content = function() {
        var settings = $.image_map_pro_editor_current_settings();
        var html = '';

        if (settings.editor.tool == 'zoom-in') {
            html += '<div class="imp-editor-canvas-overlay" id="imp-editor-canvas-overlay-zoom-in"></div>';
        }
        if (settings.editor.tool == 'zoom-out') {
            html += '<div class="imp-editor-canvas-overlay" id="imp-editor-canvas-overlay-zoom-out"></div>';
        }
        if (settings.editor.tool == 'drag' || settings.editor.state.dragging) {
            html += '<div class="imp-editor-canvas-overlay" id="imp-editor-canvas-overlay-drag"></div>';
        }

        html += '<img id="imp-editor-image" src="'+ settings.image.url +'">'
        html += '<div id="imp-editor-shapes-container">';

        for (var i=0; i<settings.spots.length; i++) {
            var s = settings.spots[i];

            if (s.type == 'spot') {
                if (parseInt(s.default_style.use_icon, 10) == 1) {
                    var style = '';
                    style += 'left: ' + s.x + '%;';
                    style += 'top: ' + s.y + '%;';

                    style += 'width: ' + s.width + 'px;';
                    style += 'height: ' + s.height + 'px;';
                    style += 'margin-left: -' + (s.width/2) + 'px;';
                    style += 'margin-top: -' + (s.height/2) + 'px;';
                    style += 'background-image: url('+ s.default_style.icon_url +')';
                    style += 'background-position: center;';
                    style += 'background-repeat: no-repeat;';

                    var iconStyle = '';
                    if (parseInt(s.default_style.icon_is_pin, 10) == 1) {
                        iconStyle += 'top: -50%;';
                        iconStyle += 'position: absolute;';
                    }

                    html += '<div class="imp-editor-shape imp-editor-spot" data-id="'+ s.id +'" data-editor-object-type="1" style="'+ style +'"><div class="imp-selection" style="border-radius: '+ s.default_style.border_radius +'px;"></div>';

                    if (s.default_style.icon_type == 'library') {
                        html += '   <svg style="'+ iconStyle +'" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" version="1.1" viewBox="'+ s.default_style.icon_svg_viewbox +'" xml:space="preserve" width="'+ s.width +'px" height="'+ s.height +'px">';
                        html += '       <path style="fill:'+ s.default_style.icon_fill +'" d="'+ s.default_style.icon_svg_path +'"></path>';
                        html += '   </svg>';
                    } else {
                        if (s.default_style.icon_url.length > 0) {
                            html += '<img style="'+ iconStyle +'" src="'+ s.default_style.icon_url +'">';
                        }
                    }
                    if (parseInt(s.default_style.icon_shadow, 10) == 1) {
                        var shadowStyle = '';
                        shadowStyle += 'width: ' + s.width + 'px;';
                        shadowStyle += 'height: ' + s.height + 'px;';
                        if (parseInt(s.default_style.icon_is_pin, 10) == 0) {
                            shadowStyle += 'top: '+ s.height/2 +'px;';
                        }
                        html += '<div style="'+ shadowStyle +'" class="imp-editor-shape-icon-shadow"></div>';
                    }

                    html += '</div>';
                } else {
                    var c_bg = hexToRgb(s.default_style.background_color);
                    var c_bo = hexToRgb(s.default_style.border_color);

                    var style = '';

                    style += 'left: ' + s.x + '%;';
                    style += 'top: ' + s.y + '%;';

                    style += 'width: ' + s.width + 'px;';
                    style += 'height: ' + s.height + 'px;';
                    style += 'margin-left: -' + (s.width/2) + 'px;';
                    style += 'margin-top: -' + (s.height/2) + 'px;';

                    style += 'background: rgba('+ c_bg.r +', '+ c_bg.g +', '+ c_bg.b +', '+ s.default_style.background_opacity +');';
                    // style += 'opacity: ' + s.default_style.opacity + ';';
                    style += 'border-color: rgba('+ c_bo.r +', '+ c_bo.g +', '+ c_bo.b +', '+ s.default_style.border_opacity +');';
                    style += 'border-width: ' + s.default_style.border_width + 'px;';
                    style += 'border-style: ' + s.default_style.border_style + ';';
                    style += 'border-radius: ' + s.default_style.border_radius + 'px;';

                    html += '<div class="imp-editor-shape imp-editor-spot" data-id="'+ s.id +'" data-editor-object-type="1" style="'+ style +'"><div class="imp-selection" style="border-radius: '+ s.default_style.border_radius +'px;"></div></div>';
                }
            }
            if (s.type == 'rect') {
                var c_bg = hexToRgb(s.default_style.background_color);
                var c_bo = hexToRgb(s.default_style.border_color);

                var style = '';

                style += 'left: ' + s.x + '%;';
                style += 'top: ' + s.y + '%;';

                style += 'width: ' + s.width + '%;';
                style += 'height: ' + s.height + '%;';

                style += 'background: rgba('+ c_bg.r +', '+ c_bg.g +', '+ c_bg.b +', '+ s.default_style.background_opacity +');';
                // style += 'opacity: ' + s.default_style.opacity + ';';
                style += 'border-color: rgba('+ c_bo.r +', '+ c_bo.g +', '+ c_bo.b +', '+ s.default_style.border_opacity +');';
                style += 'border-width: ' + s.default_style.border_width + 'px;';
                style += 'border-style: ' + s.default_style.border_style + ';';
                style += 'border-radius: ' + s.default_style.border_radius + 'px;';

                html += '<div class="imp-editor-shape imp-editor-rect" data-id="'+ s.id +'" data-editor-object-type="3" style="'+ style +'">';
                html += '   <div class="imp-selection" style="border-radius: '+ s.default_style.border_radius +'px;">';
                html += '       <div class="imp-selection-translate-boxes">';
                html += '           <div class="imp-selection-translate-box imp-selection-translate-box-1" data-transform-direction="1" data-editor-object-type="5"></div>';
                html += '           <div class="imp-selection-translate-box imp-selection-translate-box-2" data-transform-direction="2" data-editor-object-type="5"></div>';
                html += '           <div class="imp-selection-translate-box imp-selection-translate-box-3" data-transform-direction="3" data-editor-object-type="5"></div>';
                html += '           <div class="imp-selection-translate-box imp-selection-translate-box-4" data-transform-direction="4" data-editor-object-type="5"></div>';
                html += '           <div class="imp-selection-translate-box imp-selection-translate-box-5" data-transform-direction="5" data-editor-object-type="5"></div>';
                html += '           <div class="imp-selection-translate-box imp-selection-translate-box-6" data-transform-direction="6" data-editor-object-type="5"></div>';
                html += '           <div class="imp-selection-translate-box imp-selection-translate-box-7" data-transform-direction="7" data-editor-object-type="5"></div>';
                html += '           <div class="imp-selection-translate-box imp-selection-translate-box-8" data-transform-direction="8" data-editor-object-type="5"></div>';
                html += '       </div>';
                html += '   </div>';
                html += '</div>';
            }
            if (s.type == 'oval') {
                var c_bg = hexToRgb(s.default_style.background_color);
                var c_bo = hexToRgb(s.default_style.border_color);

                var style = '';

                style += 'left: ' + s.x + '%;';
                style += 'top: ' + s.y + '%;';

                style += 'width: ' + s.width + '%;';
                style += 'height: ' + s.height + '%;';

                style += 'background: rgba('+ c_bg.r +', '+ c_bg.g +', '+ c_bg.b +', '+ s.default_style.background_opacity +');';
                // style += 'opacity: ' + s.default_style.opacity + ';';
                style += 'border-color: rgba('+ c_bo.r +', '+ c_bo.g +', '+ c_bo.b +', '+ s.default_style.border_opacity +');';
                style += 'border-width: ' + s.default_style.border_width + 'px;';
                style += 'border-style: ' + s.default_style.border_style + ';';
                style += 'border-radius: 100% 100%;';

                html += '<div class="imp-editor-shape imp-editor-oval" data-id="'+ s.id +'" data-editor-object-type="2" style="'+ style +'">';
                html += '   <div class="imp-selection" style="border-radius: 100% 100%;">';
                html += '       <div class="imp-selection-translate-boxes">';
                html += '           <div class="imp-selection-translate-box imp-selection-translate-box-1" data-transform-direction="1" data-editor-object-type="5"></div>';
                html += '           <div class="imp-selection-translate-box imp-selection-translate-box-2" data-transform-direction="2" data-editor-object-type="5"></div>';
                html += '           <div class="imp-selection-translate-box imp-selection-translate-box-3" data-transform-direction="3" data-editor-object-type="5"></div>';
                html += '           <div class="imp-selection-translate-box imp-selection-translate-box-4" data-transform-direction="4" data-editor-object-type="5"></div>';
                html += '           <div class="imp-selection-translate-box imp-selection-translate-box-5" data-transform-direction="5" data-editor-object-type="5"></div>';
                html += '           <div class="imp-selection-translate-box imp-selection-translate-box-6" data-transform-direction="6" data-editor-object-type="5"></div>';
                html += '           <div class="imp-selection-translate-box imp-selection-translate-box-7" data-transform-direction="7" data-editor-object-type="5"></div>';
                html += '           <div class="imp-selection-translate-box imp-selection-translate-box-8" data-transform-direction="8" data-editor-object-type="5"></div>';
                html += '       </div>';
                html += '   </div>';
                html += '</div>';
            }
            if (s.type == 'poly' && s.points) {
                var c_fill = hexToRgb(s.default_style.fill);
                var c_stroke = hexToRgb(s.default_style.stroke_color);

                var style = '';
                style += 'left: ' + s.x + '%;';
                style += 'top: ' + s.y + '%;';
                style += 'width: ' + s.width + '%;';
                style += 'height: ' + s.height + '%;';
                // style += 'opacity: ' + s.default_style.opacity + ';';


                var svgStyle = '';
                svgStyle += 'width: 100%;';
                svgStyle += 'height: 100%;';
                svgStyle += 'fill: rgba('+ c_fill.r +', '+ c_fill.g +', '+ c_fill.b +', '+ s.default_style.fill_opacity +');';
                svgStyle += 'stroke: rgba('+ c_stroke.r +', '+ c_stroke.g +', '+ c_stroke.b +', '+ s.default_style.stroke_opacity +');';
                svgStyle += 'stroke-width: ' + s.default_style.stroke_width + 'px;';
                svgStyle += 'stroke-dasharray: ' + s.default_style.stroke_dasharray + ';';
                svgStyle += 'stroke-linecap: ' + s.default_style.stroke_linecap + ';';

                html += '<div class="imp-editor-shape imp-editor-poly" data-id="'+ s.id +'" data-editor-object-type="4" style="'+ style +'">';
                html += '   <div class="imp-editor-poly-svg-temp-control-point" data-editor-object-type="6"></div>';

                var shapeWidthPx = settings.general.width * (s.width/100);
                var shapeHeightPx = settings.general.height * (s.height/100);
                html += '   <div class="imp-editor-svg-wrap" style="padding: '+ (s.default_style.stroke_width) +'px; left: -'+ (s.default_style.stroke_width) +'px; top: -'+ (s.default_style.stroke_width) +'px;">'
                html += '       <svg class="imp-editor-poly-svg" viewBox="0 0 '+ shapeWidthPx +' '+ shapeHeightPx +'" preserveAspectRatio="none" style="'+ svgStyle +'">';
                html += '           <polygon points="';

                for (var j=0; j<s.points.length; j++) {
                    var x = s.default_style.stroke_width + (s.points[j].x / 100) * (shapeWidthPx - s.default_style.stroke_width*2);
                    var y = s.default_style.stroke_width + (s.points[j].y / 100) * (shapeHeightPx - s.default_style.stroke_width*2);
                    html += x + ',' + y + ' ';
                }

                html += '           "></polygon>';
                html += '       </svg>';
                html += '   </div>'; // end svg wrap
                html += '       <svg class="imp-editor-shape-poly-svg-overlay" viewBox="0 0 '+ shapeWidthPx +' '+ shapeHeightPx +'" preserveAspectRatio="none">';
                html += '           <polygon points="';

                for (var j=0; j<s.points.length; j++) {
                    var x = (s.points[j].x / 100) * shapeWidthPx;
                    var y = (s.points[j].y / 100) * shapeHeightPx;
                    html += x + ',' + y + ' ';
                }

                html += '           "></polygon>';
                html += '       </svg>';
                html += '   <div class="imp-selection imp-expanded-selection">';
                html += '       <div class="imp-selection-translate-boxes">';
                html += '           <div class="imp-selection-translate-box imp-selection-translate-box-1" data-transform-direction="1" data-editor-object-type="5"></div>';
                html += '           <div class="imp-selection-translate-box imp-selection-translate-box-2" data-transform-direction="2" data-editor-object-type="5"></div>';
                html += '           <div class="imp-selection-translate-box imp-selection-translate-box-3" data-transform-direction="3" data-editor-object-type="5"></div>';
                html += '           <div class="imp-selection-translate-box imp-selection-translate-box-4" data-transform-direction="4" data-editor-object-type="5"></div>';
                html += '           <div class="imp-selection-translate-box imp-selection-translate-box-5" data-transform-direction="5" data-editor-object-type="5"></div>';
                html += '           <div class="imp-selection-translate-box imp-selection-translate-box-6" data-transform-direction="6" data-editor-object-type="5"></div>';
                html += '           <div class="imp-selection-translate-box imp-selection-translate-box-7" data-transform-direction="7" data-editor-object-type="5"></div>';
                html += '           <div class="imp-selection-translate-box imp-selection-translate-box-8" data-transform-direction="8" data-editor-object-type="5"></div>';
                html += '       </div>';
                html += '   </div>';

                for (var j=0; j<s.points.length; j++) {
                    html += '       <div class="imp-poly-control-point" data-editor-object-type="7" data-index="'+ j +'" style="left: '+ s.points[j].x +'%; top: ' + s.points[j].y + '%;"></div>';
                }

                html += '</div>';
            }
        }

        // Close shapes container
        html += '</div>';

        // Close canvas inner wrap
        html += '</div>';

        return html;
    }

    function hexToRgb(hex) {
        var result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hex);
        return result ? {
            r: parseInt(result[1], 16),
            g: parseInt(result[2], 16),
            b: parseInt(result[3], 16)
        } : { r: 0, g: 0, b: 0 };
    }
})( jQuery, window, document );
