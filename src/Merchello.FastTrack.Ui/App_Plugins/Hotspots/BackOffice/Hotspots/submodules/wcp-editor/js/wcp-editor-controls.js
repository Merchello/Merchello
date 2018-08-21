// Webcraft Plugins Ltd.
// Author: Nikolay Dyankov

/* 

To be rewritten

- All controls must share properties
- Inheritance?
- New: tooltip functionality
- New: colspan functionality

*/

;(function ($, window, document, undefined) {
    $.wcpEditorRegisterControl({
        type: 'int',
        getValue: function() {
            return parseInt($('#' + this.elementID).val(), 10);
        },
        setValue: function(v) {
            $('#' + this.elementID).val(parseInt(v, 10));
        },
        HTML: function() {
            return '<input type="text" id="'+ this.elementID +'">';
        },
        init: function() {
            var self = this;
            $(document).on('change', '#' + this.elementID, function() {
                var parsedValue = parseInt($(this).val(), 10);

                if (isNaN(parsedValue)) {
                    parsedValue = 0;
                }

                $(this).val(parsedValue);

                self.valueChanged();
            });
        }
    });
    $.wcpEditorRegisterControl({
        type: 'float',
        getValue: function() {
            return parseFloat($('#' + this.elementID).val());
        },
        setValue: function(v) {
            $('#' + this.elementID).val(parseFloat(v));
        },
        HTML: function() {
            return '<input type="text" id="'+ this.elementID +'">';
        },
        init: function() {
            var self = this;
            $(document).on('change', '#' + this.elementID, function() {
                var parsedValue = parseFloat($(this).val());

                if (isNaN(parsedValue)) {
                    parsedValue = 0;
                }

                $(this).val(parsedValue);

                self.valueChanged();
            });
        }
    });
    $.wcpEditorRegisterControl({
        type: 'text',
        getValue: function() {
            return $('#' + this.elementID).val();
        },
        setValue: function(v) {
            $('#' + this.elementID).val(v);
        },
        HTML: function() {
            return '<input type="text" id="'+ this.elementID +'">';
        },
        init: function() {
            var self = this;
            $(document).on('change', '#' + this.elementID, function() {
                self.valueChanged();
            });
        }
    });
    $.wcpEditorRegisterControl({
        type: 'textarea',
        getValue: function() {
            return $('#' + this.elementID).val();
        },
        setValue: function(v) {
            $('#' + this.elementID).val(v);
        },
        HTML: function() {
            return '<textarea id="'+ this.elementID +'" rows="5"></textarea>';
        },
        init: function() {
            var self = this;
            $(document).on('change', '#' + this.elementID, function() {
                self.valueChanged();
            });
        }
    });
    $.wcpEditorRegisterControl({
        type: 'checkbox',
        getValue: function() {
            if ($('#' + this.elementID).get(0).checked == true) {
                return 1;
            } else {
                return 0;
            }
        },
        setValue: function(v) {
            if (parseInt(v, 10) === 1) {
                $('#' + this.elementID).get(0).checked = true;
            } else {
                $('#' + this.elementID).get(0).checked = false;
            }
        },
        HTML: function() {
            return '<input type="checkbox" id="'+ this.elementID +'">';
        },
        init: function() {
            var self = this;
            $(document).on('change', '#' + this.elementID, function() {
                self.valueChanged();
            });
        }
    });
    $.wcpEditorRegisterControl({
        type: 'color',
        getValue: function() {
            return $('#' + this.elementID).val();
        },
        setValue: function(v) {
            $('#' + this.elementID).val(v);
        },
        HTML: function() {
            return '<input type="color" id="'+ this.elementID +'">';
        },
        init: function() {
            var self = this;
            $(document).on('change', '#' + this.elementID, function() {
                self.valueChanged();
            });
        }
    });
    $.wcpEditorRegisterControl({
        type: 'select',
        getValue: function() {
            return $('#' + this.elementID).val();
        },
        setValue: function(v) {
            $('#' + this.elementID).val(v);
        },
        HTML: function() {
            var html = '';

            html += '<select id="'+ this.elementID +'">';

            for (var i=0; i<this.options.length; i++) {
                html += '<option value="'+ this.options[i].value +'">'+ this.options[i].title +'</option>';
            }

            html += '</select>';

            return html;
        },
        init: function() {
            var self = this;
            $(document).on('change', '#' + this.elementID, function() {
                self.valueChanged();
            });
        }
    });
    $.wcpEditorRegisterControl({
        type: 'box model',
        getValue: function() {
            return {
                margin: {
                    top: parseInt($('#wcp-editor-element-option-boxmodel-margin-top').val(), 10),
                    bottom: parseInt($('#wcp-editor-element-option-boxmodel-margin-bottom').val(), 10),
                    left: parseInt($('#wcp-editor-element-option-boxmodel-margin-left').val(), 10),
                    right: parseInt($('#wcp-editor-element-option-boxmodel-margin-right').val(), 10)
                },
                padding: {
                    top: parseInt($('#wcp-editor-element-option-boxmodel-padding-top').val(), 10),
                    bottom: parseInt($('#wcp-editor-element-option-boxmodel-padding-bottom').val(), 10),
                    left: parseInt($('#wcp-editor-element-option-boxmodel-padding-left').val(), 10),
                    right: parseInt($('#wcp-editor-element-option-boxmodel-padding-right').val(), 10)
                }
            }
        },
        setValue: function(v) {
            $('#wcp-editor-element-option-boxmodel-margin-top').val(v.margin.top);
            $('#wcp-editor-element-option-boxmodel-margin-bottom').val(v.margin.bottom);
            $('#wcp-editor-element-option-boxmodel-margin-left').val(v.margin.left);
            $('#wcp-editor-element-option-boxmodel-margin-right').val(v.margin.right);

            $('#wcp-editor-element-option-boxmodel-padding-top').val(v.padding.top);
            $('#wcp-editor-element-option-boxmodel-padding-bottom').val(v.padding.bottom);
            $('#wcp-editor-element-option-boxmodel-padding-left').val(v.padding.left);
            $('#wcp-editor-element-option-boxmodel-padding-right').val(v.padding.right);
        },
        HTML: function() {
            var html = '';

            html += '<div class="wcp-editor-boxmodel-margin" id="'+ this.elementID +'">';
            html += '   <div id="wcp-editor-boxmodel-label-margin">margin</div>';
            html += '   <input type="text" class="wcp-editor-boxmodel-input" id="wcp-editor-element-option-boxmodel-margin-top">';
            html += '   <input type="text" class="wcp-editor-boxmodel-input" id="wcp-editor-element-option-boxmodel-margin-bottom">';
            html += '   <input type="text" class="wcp-editor-boxmodel-input" id="wcp-editor-element-option-boxmodel-margin-left">';
            html += '   <input type="text" class="wcp-editor-boxmodel-input" id="wcp-editor-element-option-boxmodel-margin-right">';
            html += '   <div class="wcp-editor-boxmodel-padding">';
            html += '       <div id="wcp-editor-boxmodel-label-padding">padding</div>';
            html += '       <input type="text" class="wcp-editor-boxmodel-input" id="wcp-editor-element-option-boxmodel-padding-top">';
            html += '       <input type="text" class="wcp-editor-boxmodel-input" id="wcp-editor-element-option-boxmodel-padding-bottom">';
            html += '       <input type="text" class="wcp-editor-boxmodel-input" id="wcp-editor-element-option-boxmodel-padding-left">';
            html += '       <input type="text" class="wcp-editor-boxmodel-input" id="wcp-editor-element-option-boxmodel-padding-right">';
            html += '   </div>';
            html += '</div>';

            return html;
        },
        init: function() {
            var self = this;
            $(document).on('change', '#' + this.elementID + ' input', function() {
                self.valueChanged();
            });
        }
    });
    $.wcpEditorRegisterControl({
        type: 'slider',
        getValue: function() {
            var v = 0;

            // Get the ball position
            var ball = $('#' + this.elementID).find('.wcp-editor-control-slider-ball');
            var ballPosition = ball.position().left;

            // Get the track width
            var track = $('#' + this.elementID).find('.wcp-editor-control-slider-track');
            var trackWidth = track.outerWidth();

            // Calculate value
            var progress = ballPosition / trackWidth;
            v = this.options.min + (this.options.max - this.options.min) * progress;

            if (this.options.type == 'int') v = Math.round(v);

            return v;
        },
        setValue: function(v) {
            if (this.options.type == 'int') v = Math.round(v);

            var progress = (v - this.options.min) / (this.options.max - this.options.min);

            var ball = $('#' + this.elementID).find('.wcp-editor-control-slider-ball');

            // Get the track width
            var track = $('#' + this.elementID).find('.wcp-editor-control-slider-track');
            var trackWidth = track.outerWidth();

            ball.css({
                left: trackWidth * progress
            });
        },
        HTML: function() {
            var html = '';

            html += '<div class="wcp-editor-control-slider" id="'+ this.elementID +'">';
            html += '   <div class="wcp-editor-control-slider-bubble"></div>';
            html += '   <div class="wcp-editor-control-slider-track"></div>';
            html += '   <div class="wcp-editor-control-slider-ball"></div>';
            html += '</div>';

            return html;
        },
        init: function() {
            var self = this;
            var ix = 0, iex = 0, dragging = false, ball = undefined, track = undefined, bubble = undefined;

            // Ball dragging
            $(document).on('mousedown', '#' + self.elementID + ' .wcp-editor-control-slider-ball', function(e) {
                ball = $('#' + self.elementID).find('.wcp-editor-control-slider-ball');
                track = $('#' + self.elementID).find('.wcp-editor-control-slider-track');
                bubble = $('#' + self.elementID).find('.wcp-editor-control-slider-bubble');
                ix = ball.position().left;
                iex = e.pageX;

                dragging = true;

                if ($.wcpEditorSliderStartedDragging) {
                    $.wcpEditorSliderStartedDragging();
                }

                // Show value bubble
                bubble.show();
            });
            $(document).on('mousemove.' + this.elementID, function(e) {
                if (dragging) {
                    var o = ix - iex + e.pageX;

                    if (o < 0) o = 0;
                    if (o > track.outerWidth()) o = track.outerWidth();

                    if (self.options.type == 'int') {
                        var step = track.outerWidth() / (self.options.max + 1);

                        o = o - (o % step);
                    }

                    ball.css({
                        left: o
                    });

                    self.valueChanged();

                    // Update value bubble
                    var rounded = Math.round(self.getValue() * 10)/10;

                    if (self.options.type == 'int') {
                        rounded = self.getValue();
                    }

                    bubble.html(rounded);
                    bubble.css({
                        left: o
                    });
                }
            });
            $(document).on('mouseup.' + this.elementID, function(e) {
                if (dragging) {
                    if ($.wcpEditorSliderFinishedDragging) {
                        $.wcpEditorSliderFinishedDragging();
                    }

                    dragging = false;
                    self.valueChanged();

                    // Hide value bubble
                    bubble.hide();
                }
            });

            // Click on the track
            $(document).on('mousedown', '#' + self.elementID + ' .wcp-editor-control-slider-track', function(e) {
                ball = $('#' + self.elementID).find('.wcp-editor-control-slider-ball');
                track = $('#' + self.elementID).find('.wcp-editor-control-slider-track');
                bubble = $('#' + self.elementID).find('.wcp-editor-control-slider-bubble');

                // Set the ball to the mouse position
                var o = e.pageX - track.offset().left;

                if (o < 0) o = 0;
                if (o > track.outerWidth()) o = track.outerWidth();

                ball.css({
                    left: o
                });

                // Start dragging
                ix = ball.position().left;
                iex = e.pageX;

                dragging = true;

                // Show value bubble
                bubble.show();
            });
        }
    });
    $.wcpEditorRegisterControl({
        type: 'grid system',
        getValue: function() {
            // tmp
            var res = {
                xs: {
                    use: 1,
                    class: 'col-xs-1',
                    visible: 1
                },
                sm: {
                    use: 1,
                    class: 'col-sm-1',
                    visible: 1
                },
                md: {
                    use: 1,
                    class: 'col-md-1',
                    visible: 1
                },
                lg: {
                    use: 1,
                    class: 'col-lg-1',
                    visible: 1
                },
            };

            var root = $('#' + this.elementID);

            // XS ---------
            var xsGroup = root.find('.wcp-editor-grid-system-control-res-group-xs');

            // Use
            if (xsGroup.find('.wcp-editor-grid-system-control-res-use-checkbox').get(0).checked) {
                res.xs.use = 1;
            } else {
                res.xs.use = 0;
            }

            // Class
            res.xs.class = xsGroup.find('.wcp-editor-grid-system-control-select-colspan').val();

            // Visible
            if (xsGroup.find('.wcp-editor-grid-system-control-visible').hasClass('wcp-editor-grid-system-control-visible-not')) {
                res.xs.visible = 0;
            } else {
                res.xs.visible = 1;
            }

            // SM ---------
            var smGroup = root.find('.wcp-editor-grid-system-control-res-group-sm');

            // Use
            if (smGroup.find('.wcp-editor-grid-system-control-res-use-checkbox').get(0).checked) {
                res.sm.use = 1;
            } else {
                res.sm.use = 0;
            }

            // Class
            res.sm.class = smGroup.find('.wcp-editor-grid-system-control-select-colspan').val();

            // Visible
            if (smGroup.find('.wcp-editor-grid-system-control-visible').hasClass('wcp-editor-grid-system-control-visible-not')) {
                res.sm.visible = 0;
            } else {
                res.sm.visible = 1;
            }

            // MD ---------
            var mdGroup = root.find('.wcp-editor-grid-system-control-res-group-md');

            // Use
            if (mdGroup.find('.wcp-editor-grid-system-control-res-use-checkbox').get(0).checked) {
                res.md.use = 1;
            } else {
                res.md.use = 0;
            }

            // Class
            res.md.class = mdGroup.find('.wcp-editor-grid-system-control-select-colspan').val();

            // Visible
            if (mdGroup.find('.wcp-editor-grid-system-control-visible').hasClass('wcp-editor-grid-system-control-visible-not')) {
                res.md.visible = 0;
            } else {
                res.md.visible = 1;
            }

            // LG ---------
            var lgGroup = root.find('.wcp-editor-grid-system-control-res-group-lg');

            // Use
            if (lgGroup.find('.wcp-editor-grid-system-control-res-use-checkbox').get(0).checked) {
                res.lg.use = 1;
            } else {
                res.lg.use = 0;
            }

            // Class
            res.lg.class = lgGroup.find('.wcp-editor-grid-system-control-select-colspan').val();

            // Visible
            if (lgGroup.find('.wcp-editor-grid-system-control-visible').hasClass('wcp-editor-grid-system-control-visible-not')) {
                res.lg.visible = 0;
            } else {
                res.lg.visible = 1;
            }

            return res;
        },
        setValue: function(v) {
            var root = $('#' + this.elementID);

            // XS ---------
            var xsGroup = root.find('.wcp-editor-grid-system-control-res-group-xs');

            // Use
            if (parseInt(v.xs.use, 10) == 1) {
                xsGroup.find('.wcp-editor-grid-system-control-res-use-checkbox').get(0).checked = true;
                xsGroup.find('select').removeAttr('disabled');
                xsGroup.find('.wcp-editor-grid-system-control-visible').removeClass('wcp-editor-control-disabled');
            } else {
                xsGroup.find('.wcp-editor-grid-system-control-res-use-checkbox').get(0).checked = false;
                xsGroup.find('select').attr('disabled', 'disabled');
                xsGroup.find('.wcp-editor-grid-system-control-visible').addClass('wcp-editor-control-disabled');
            }

            // Class
            xsGroup.find('.wcp-editor-grid-system-control-select-colspan').val(v.xs.class);

            // Visible
            if (parseInt(v.xs.visible, 10) == 1) {
                xsGroup.find('.wcp-editor-grid-system-control-visible').removeClass('wcp-editor-grid-system-control-visible-not');
            } else {
                xsGroup.find('.wcp-editor-grid-system-control-visible').addClass('wcp-editor-grid-system-control-visible-not');
            }

            // SM ---------
            var smGroup = root.find('.wcp-editor-grid-system-control-res-group-sm');

            // Use
            if (parseInt(v.sm.use, 10) == 1) {
                smGroup.find('.wcp-editor-grid-system-control-res-use-checkbox').get(0).checked = true;
                smGroup.find('select').removeAttr('disabled');
                smGroup.find('.wcp-editor-grid-system-control-visible').removeClass('wcp-editor-control-disabled');
            } else {
                smGroup.find('.wcp-editor-grid-system-control-res-use-checkbox').get(0).checked = false;
                smGroup.find('select').attr('disabled', 'disabled');
                smGroup.find('.wcp-editor-grid-system-control-visible').addClass('wcp-editor-control-disabled');
            }

            // Class
            smGroup.find('.wcp-editor-grid-system-control-select-colspan').val(v.sm.class);

            // Visible
            if (parseInt(v.sm.visible, 10) == 1) {
                smGroup.find('.wcp-editor-grid-system-control-visible').removeClass('wcp-editor-grid-system-control-visible-not');
            } else {
                smGroup.find('.wcp-editor-grid-system-control-visible').addClass('wcp-editor-grid-system-control-visible-not');
            }

            // MD ---------
            var mdGroup = root.find('.wcp-editor-grid-system-control-res-group-md');

            // Use
            if (parseInt(v.md.use, 10) == 1) {
                mdGroup.find('.wcp-editor-grid-system-control-res-use-checkbox').get(0).checked = true;
                mdGroup.find('select').removeAttr('disabled');
                mdGroup.find('.wcp-editor-grid-system-control-visible').removeClass('wcp-editor-control-disabled');
            } else {
                mdGroup.find('.wcp-editor-grid-system-control-res-use-checkbox').get(0).checked = false;
                mdGroup.find('select').attr('disabled', 'disabled');
                mdGroup.find('.wcp-editor-grid-system-control-visible').addClass('wcp-editor-control-disabled');
            }

            // Class
            mdGroup.find('.wcp-editor-grid-system-control-select-colspan').val(v.md.class);

            // Visible
            if (parseInt(v.md.visible, 10) == 1) {
                mdGroup.find('.wcp-editor-grid-system-control-visible').removeClass('wcp-editor-grid-system-control-visible-not');
            } else {
                mdGroup.find('.wcp-editor-grid-system-control-visible').addClass('wcp-editor-grid-system-control-visible-not');
            }

            // LG ---------
            var lgGroup = root.find('.wcp-editor-grid-system-control-res-group-lg');

            // Use
            if (parseInt(v.lg.use, 10) == 1) {
                lgGroup.find('.wcp-editor-grid-system-control-res-use-checkbox').get(0).checked = true;
                lgGroup.find('select').removeAttr('disabled');
                lgGroup.find('.wcp-editor-grid-system-control-visible').removeClass('wcp-editor-control-disabled');
            } else {
                lgGroup.find('.wcp-editor-grid-system-control-res-use-checkbox').get(0).checked = false;
                lgGroup.find('select').attr('disabled', 'disabled');
                lgGroup.find('.wcp-editor-grid-system-control-visible').addClass('wcp-editor-control-disabled');
            }

            // Class
            lgGroup.find('.wcp-editor-grid-system-control-select-colspan').val(v.lg.class);

            // Visible
            if (parseInt(v.lg.visible, 10) == 1) {
                lgGroup.find('.wcp-editor-grid-system-control-visible').removeClass('wcp-editor-grid-system-control-visible-not');
            } else {
                lgGroup.find('.wcp-editor-grid-system-control-visible').addClass('wcp-editor-grid-system-control-visible-not');
            }
        },
        HTML: function() {
            var html = '';

            html += '<div class="wcp-editor-grid-system-control" id="'+ this.elementID +'">';

            // LG
            html += '   <div class="wcp-editor-grid-system-control-res-group wcp-editor-grid-system-control-res-group-lg">';
            html += '       <div class="wcp-editor-grid-system-control-res-name">LG</div>';
            html += '       <div class="wcp-editor-grid-system-control-res-use">';
            html += '           <input type="checkbox" class="wcp-editor-grid-system-control-res-use-checkbox">';
            html += '       </div>';
            html += '       <div class="wcp-editor-grid-system-control-colspan">';
            html += '           <select class="wcp-editor-grid-system-control-select-colspan">';
            html += '               <option value="col-lg-1">1 Column</option>';
            html += '               <option value="col-lg-2">2 Columns</option>';
            html += '               <option value="col-lg-3">3 Columns</option>';
            html += '               <option value="col-lg-4">4 Columns</option>';
            html += '               <option value="col-lg-5">5 Columns</option>';
            html += '               <option value="col-lg-6">6 Column</option>';
            html += '               <option value="col-lg-7">7 Columns</option>';
            html += '               <option value="col-lg-8">8 Columns</option>';
            html += '               <option value="col-lg-9">9 Columns</option>';
            html += '               <option value="col-lg-10">10 Columns</option>';
            html += '               <option value="col-lg-11">11 Columns</option>';
            html += '               <option value="col-lg-12">12 Columns</option>';
            html += '           </select>';
            html += '       </div>';
            html += '       <div class="wcp-editor-grid-system-control-visible">';
            html += '           <i class="fa fa-eye" aria-hidden="true"></i>';
            html += '           <i class="fa fa-eye-slash" aria-hidden="true"></i>';
            html += '       </div>';
            html += '   </div>';

            // MD
            html += '   <div class="wcp-editor-grid-system-control-res-group wcp-editor-grid-system-control-res-group-md">';
            html += '       <div class="wcp-editor-grid-system-control-res-name">MD</div>';
            html += '       <div class="wcp-editor-grid-system-control-res-use">';
            html += '           <input type="checkbox" class="wcp-editor-grid-system-control-res-use-checkbox">';
            html += '       </div>';
            html += '       <div class="wcp-editor-grid-system-control-colspan">';
            html += '           <select class="wcp-editor-grid-system-control-select-colspan">';
            html += '               <option value="col-md-1">1 Column</option>';
            html += '               <option value="col-md-2">2 Columns</option>';
            html += '               <option value="col-md-3">3 Columns</option>';
            html += '               <option value="col-md-4">4 Columns</option>';
            html += '               <option value="col-md-5">5 Columns</option>';
            html += '               <option value="col-md-6">6 Column</option>';
            html += '               <option value="col-md-7">7 Columns</option>';
            html += '               <option value="col-md-8">8 Columns</option>';
            html += '               <option value="col-md-9">9 Columns</option>';
            html += '               <option value="col-md-10">10 Columns</option>';
            html += '               <option value="col-md-11">11 Columns</option>';
            html += '               <option value="col-md-12">12 Columns</option>';
            html += '           </select>';
            html += '       </div>';
            html += '       <div class="wcp-editor-grid-system-control-visible">';
            html += '           <i class="fa fa-eye" aria-hidden="true"></i>';
            html += '           <i class="fa fa-eye-slash" aria-hidden="true"></i>';
            html += '       </div>';
            html += '   </div>';

            // SM
            html += '   <div class="wcp-editor-grid-system-control-res-group wcp-editor-grid-system-control-res-group-sm">';
            html += '       <div class="wcp-editor-grid-system-control-res-name">SM</div>';
            html += '       <div class="wcp-editor-grid-system-control-res-use">';
            html += '           <input type="checkbox" class="wcp-editor-grid-system-control-res-use-checkbox">';
            html += '       </div>';
            html += '       <div class="wcp-editor-grid-system-control-colspan">';
            html += '           <select class="wcp-editor-grid-system-control-select-colspan">';
            html += '               <option value="col-sm-1">1 Column</option>';
            html += '               <option value="col-sm-2">2 Columns</option>';
            html += '               <option value="col-sm-3">3 Columns</option>';
            html += '               <option value="col-sm-4">4 Columns</option>';
            html += '               <option value="col-sm-5">5 Columns</option>';
            html += '               <option value="col-sm-6">6 Column</option>';
            html += '               <option value="col-sm-7">7 Columns</option>';
            html += '               <option value="col-sm-8">8 Columns</option>';
            html += '               <option value="col-sm-9">9 Columns</option>';
            html += '               <option value="col-sm-10">10 Columns</option>';
            html += '               <option value="col-sm-11">11 Columns</option>';
            html += '               <option value="col-sm-12">12 Columns</option>';
            html += '           </select>';
            html += '       </div>';
            html += '       <div class="wcp-editor-grid-system-control-visible">';
            html += '           <i class="fa fa-eye" aria-hidden="true"></i>';
            html += '           <i class="fa fa-eye-slash" aria-hidden="true"></i>';
            html += '       </div>';
            html += '   </div>';

            // XS
            html += '   <div class="wcp-editor-grid-system-control-res-group wcp-editor-grid-system-control-res-group-xs">';
            html += '       <div class="wcp-editor-grid-system-control-res-name">XS</div>';
            html += '       <div class="wcp-editor-grid-system-control-res-use">';
            html += '           <input type="checkbox" class="wcp-editor-grid-system-control-res-use-checkbox">';
            html += '       </div>';
            html += '       <div class="wcp-editor-grid-system-control-colspan">';
            html += '           <select class="wcp-editor-grid-system-control-select-colspan">';
            html += '               <option value="col-xs-1">1 Column</option>';
            html += '               <option value="col-xs-2">2 Columns</option>';
            html += '               <option value="col-xs-3">3 Columns</option>';
            html += '               <option value="col-xs-4">4 Columns</option>';
            html += '               <option value="col-xs-5">5 Columns</option>';
            html += '               <option value="col-xs-6">6 Column</option>';
            html += '               <option value="col-xs-7">7 Columns</option>';
            html += '               <option value="col-xs-8">8 Columns</option>';
            html += '               <option value="col-xs-9">9 Columns</option>';
            html += '               <option value="col-xs-10">10 Columns</option>';
            html += '               <option value="col-xs-11">11 Columns</option>';
            html += '               <option value="col-xs-12">12 Columns</option>';
            html += '           </select>';
            html += '       </div>';
            html += '       <div class="wcp-editor-grid-system-control-visible">';
            html += '           <i class="fa fa-eye" aria-hidden="true"></i>';
            html += '           <i class="fa fa-eye-slash" aria-hidden="true"></i>';
            html += '       </div>';
            html += '   </div>';

            // end
            html += '   <div class="wcp-editor-controls-clear"></div>';
            html += '</div>';

            return html;
        },
        init: function() {
            var self = this;
            // self.valueChanged();

            // "Use" checkboxes
            $(document).on('change', '#' + this.elementID + ' .wcp-editor-grid-system-control-res-use-checkbox', function() {
                // Enable/disable the other inputs from this resolution group

                if ($(this).get(0).checked) {
                    $(this).closest('.wcp-editor-grid-system-control-res-group').find('select').removeAttr('disabled');
                    $(this).closest('.wcp-editor-grid-system-control-res-group').find('.wcp-editor-grid-system-control-visible').removeClass('wcp-editor-control-disabled');
                } else {
                    $(this).closest('.wcp-editor-grid-system-control-res-group').find('select').attr('disabled', 'disabled');
                    $(this).closest('.wcp-editor-grid-system-control-res-group').find('.wcp-editor-grid-system-control-visible').addClass('wcp-editor-control-disabled');
                }

                self.valueChanged();
            });

            // Toggle visibility
            $(document).on('click', '#' + this.elementID + ' .wcp-editor-grid-system-control-visible', function() {
                $(this).toggleClass('wcp-editor-grid-system-control-visible-not');
                self.valueChanged();
            });

            // Select colspan
            $(document).on('change', '#' + this.elementID + ' .wcp-editor-grid-system-control-select-colspan', function() {
                self.valueChanged();
            });
        }
    });
    $.wcpEditorRegisterControl({
        type: 'switch',
        customLabel: true,
        getValue: function() {
            var v = 0;

            if ($('#' + this.elementID).hasClass('active')) {
                v = 1;
            }

            return v;
        },
        setValue: function(v) {
            if (parseInt(v, 10) == 1) {
                $('#' + this.elementID).addClass('active');
            } else {
                $('#' + this.elementID).removeClass('active');
            }
        },
        HTML: function() {
            var html = '';

            html += '<div class="wcp-editor-control-switch" id="'+ this.elementID +'">';
            html += '   <div class="wcp-editor-control-switch-ball"></div>';
            html += '</div>';

            html += '<div class="wcp-editor-control-switch-label" id="'+ this.elementID +'-label">'+ this.title +'</div>';
            html += '<div class="wcp-editor-controls-clear"></div>';

            return html;
        },
        init: function() {
            var self = this;

            $(document).on('click', '#' + this.elementID, function() {
                $(this).toggleClass('active');
                self.valueChanged();
            });
            $(document).on('click', '#' + this.elementID + '-label', function() {
                $('#' + self.elementID).toggleClass('active');
                self.valueChanged();
            });
        }
    });
    $.wcpEditorRegisterControl({
        type: 'button group',
        getValue: function() {
            var v = $('#' + this.elementID).find('.active[data-button-value]').data('button-value');

            return v;
        },
        setValue: function(v) {
            $('#' + this.elementID).find('[data-button-value]').removeClass('active');
            $('#' + this.elementID).find('[data-button-value="'+ v +'"]').addClass('active');

            $('#' + this.elementID).find('[data-button-value="'+ v +'"]').siblings().removeClass('no-border-right');
            $('#' + this.elementID).find('[data-button-value="'+ v +'"]').prev().addClass('no-border-right');
        },
        HTML: function() {
            var html = '';

            html += '<div class="wcp-editor-control-button-group" id="'+ this.elementID +'">';

            for (var i=0; i<this.options.length; i++) {
                html += '<div class="wcp-editor-control-button-group-button" data-button-value="'+ this.options[i].value +'">'+ this.options[i].title +'</div>';
            }

            html += '</div>';

            return html;
        },
        init: function() {
            var self = this;

            $(document).on('click', '#' + this.elementID + ' .wcp-editor-control-button-group-button', function() {
                $(this).siblings().removeClass('active').removeClass('no-border-right');
                $(this).prev().addClass('no-border-right');
                $(this).addClass('active');
                self.valueChanged();
            });
        }
    });
    $.wcpEditorRegisterControl({
        type: 'button',
        customLabel: true,
        getValue: function() {
            return undefined;
        },
        setValue: function() {

        },
        HTML: function() {
            return '<div id="'+ this.elementID +'" class="wcp-editor-control-button">'+ this.title +'</div>';
        },
        init: function() {
            var self = this;

            $(document).on('click', '#' + this.elementID, function() {
                self.valueChanged();
                $(document).trigger(self.options.event_name);
            });
        }
    });
    $.wcpEditorRegisterControl({
        type: 'wp media upload',
        getValue: function() {
            return $('#' + this.elementID + ' input').val();
        },
        setValue: function(v) {
            $('#' + this.elementID + ' input').val(v);
        },
        HTML: function() {
            return '<div class="wcp-editor-input-with-button" id="'+ this.elementID +'"><input type="text"><div class="wcp-editor-control-button">Choose Image</div></div>';
        },
        init: function() {
            var self = this;

            var inputSelector = '#' + this.elementID + ' input';
            var buttonSelector = '#' + this.elementID + ' .wcp-editor-control-button';

            if ($.wcpWPMedia) {
                $.wcpWPMedia(inputSelector, buttonSelector, function() {
                    self.valueChanged();
                });
            }

            $(document).on('change', inputSelector, function() {
                self.valueChanged();
            });
        }
    });
    $.wcpEditorRegisterControl({
        type: 'object',
        getValue: function() {
            return this.val;
        },
        setValue: function(v) {
            this.val = v;
        },
        HTML: function() {
            return '';
        },
        init: function() {
            var self = this;

			// Custom var
			this.val = undefined;
        }
    });
    $.wcpEditorRegisterControl({
        type: 'fullscreen button position',
        getValue: function() {
            return $('#' + this.elementID).find('.wcp-editor-control-fullscreen-button-position-selected').data('wcp-button-position');
        },
        setValue: function(v) {
            this.val = v;

            $('#' + this.elementID).find('.wcp-editor-control-fullscreen-button-position-selected').removeClass('wcp-editor-control-fullscreen-button-position-selected');
            $('#' + this.elementID).find('[data-wcp-button-position="'+ v +'"]').addClass('wcp-editor-control-fullscreen-button-position-selected');
        },
        HTML: function() {
            var html = '';

            html += '<div class="wcp-editor-control-fullscreen-button-position" id="'+ this.elementID +'">';
            html += '<div class="wcp-editor-control-fullscreen-button-position-location wcp-editor-control-fullscreen-button-position-location-0" data-wcp-button-position="0"></div>';
            html += '<div class="wcp-editor-control-fullscreen-button-position-location wcp-editor-control-fullscreen-button-position-location-1 wcp-editor-control-fullscreen-button-position-selected" data-wcp-button-position="1"></div>';
            html += '<div class="wcp-editor-control-fullscreen-button-position-location wcp-editor-control-fullscreen-button-position-location-2" data-wcp-button-position="2"></div>';
            html += '<div class="wcp-editor-control-fullscreen-button-position-location wcp-editor-control-fullscreen-button-position-location-3" data-wcp-button-position="3"></div>';
            html += '<div class="wcp-editor-control-fullscreen-button-position-location wcp-editor-control-fullscreen-button-position-location-4" data-wcp-button-position="4"></div>';
            html += '<div class="wcp-editor-control-fullscreen-button-position-location wcp-editor-control-fullscreen-button-position-location-5" data-wcp-button-position="5"></div>';
            html += '</div>';

            return html;
        },
        init: function() {
            var self = this;

			$(document).off('click.fullscreen_button_position');
			$(document).on('click.fullscreen_button_position', '.wcp-editor-control-fullscreen-button-position-location', function() {
                $('#' + self.elementID).find('.wcp-editor-control-fullscreen-button-position-selected').removeClass('wcp-editor-control-fullscreen-button-position-selected');
                $(this).addClass('wcp-editor-control-fullscreen-button-position-selected');


                self.valueChanged();
            });
        }
    });
})(jQuery, window, document);
