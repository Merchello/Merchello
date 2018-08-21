// Squares
// Description: Interactive and embeddable HTML content builder.
// Author: Nikolay Dyankov
// License: MIT

;(function ($, window, document, undefined) {
    $.squaresRegisterControl({
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
                self.valueChanged();
            });
        }
    });
    $.squaresRegisterControl({
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
                self.valueChanged();
            });
        }
    });
    $.squaresRegisterControl({
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
    $.squaresRegisterControl({
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
    $.squaresRegisterControl({
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
    $.squaresRegisterControl({
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
    $.squaresRegisterControl({
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
                html += '<option value="'+ this.options[i] +'">'+ this.options[i] +'</option>';
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
    $.squaresRegisterControl({
        type: 'box model',
        getValue: function() {
            return {
                margin: {
                    top: parseInt($('#sq-element-option-boxmodel-margin-top').val(), 10),
                    bottom: parseInt($('#sq-element-option-boxmodel-margin-bottom').val(), 10),
                    left: parseInt($('#sq-element-option-boxmodel-margin-left').val(), 10),
                    right: parseInt($('#sq-element-option-boxmodel-margin-right').val(), 10)
                },
                padding: {
                    top: parseInt($('#sq-element-option-boxmodel-padding-top').val(), 10),
                    bottom: parseInt($('#sq-element-option-boxmodel-padding-bottom').val(), 10),
                    left: parseInt($('#sq-element-option-boxmodel-padding-left').val(), 10),
                    right: parseInt($('#sq-element-option-boxmodel-padding-right').val(), 10)
                }
            }
        },
        setValue: function(v) {
            $('#sq-element-option-boxmodel-margin-top').val(this._value.margin.top);
            $('#sq-element-option-boxmodel-margin-bottom').val(this._value.margin.bottom);
            $('#sq-element-option-boxmodel-margin-left').val(this._value.margin.left);
            $('#sq-element-option-boxmodel-margin-right').val(this._value.margin.right);

            $('#sq-element-option-boxmodel-padding-top').val(this._value.padding.top);
            $('#sq-element-option-boxmodel-padding-bottom').val(this._value.padding.bottom);
            $('#sq-element-option-boxmodel-padding-left').val(this._value.padding.left);
            $('#sq-element-option-boxmodel-padding-right').val(this._value.padding.right);
        },
        HTML: function() {
            var html = '';

            html += '<div class="sq-boxmodel-margin" id="'+ this.elementID +'">';
            html += '   <div id="sq-boxmodel-label-margin">margin</div>';
            html += '   <input type="text" class="sq-boxmodel-input" id="sq-element-option-boxmodel-margin-top">';
            html += '   <input type="text" class="sq-boxmodel-input" id="sq-element-option-boxmodel-margin-bottom">';
            html += '   <input type="text" class="sq-boxmodel-input" id="sq-element-option-boxmodel-margin-left">';
            html += '   <input type="text" class="sq-boxmodel-input" id="sq-element-option-boxmodel-margin-right">';
            html += '   <div class="sq-boxmodel-padding">';
            html += '       <div id="sq-boxmodel-label-padding">padding</div>';
            html += '       <input type="text" class="sq-boxmodel-input" id="sq-element-option-boxmodel-padding-top">';
            html += '       <input type="text" class="sq-boxmodel-input" id="sq-element-option-boxmodel-padding-bottom">';
            html += '       <input type="text" class="sq-boxmodel-input" id="sq-element-option-boxmodel-padding-left">';
            html += '       <input type="text" class="sq-boxmodel-input" id="sq-element-option-boxmodel-padding-right">';
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
    $.squaresRegisterControl({
        type: 'slider',
        getValue: function() {
            var v = 0;

            // Get the ball position
            var ball = $('#' + this.elementID).find('.sq-control-slider-ball');
            var ballPosition = ball.position().left;

            // Get the track width
            var track = $('#' + this.elementID).find('.sq-control-slider-track');
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

            var ball = $('#' + this.elementID).find('.sq-control-slider-ball');

            // Get the track width
            var track = $('#' + this.elementID).find('.sq-control-slider-track');
            var trackWidth = track.outerWidth();

            ball.css({
                left: trackWidth * progress
            });
        },
        HTML: function() {
            var html = '';

            html += '<div class="sq-control-slider" id="'+ this.elementID +'">';
            html += '   <div class="sq-control-slider-bubble"></div>';
            html += '   <div class="sq-control-slider-track"></div>';
            html += '   <div class="sq-control-slider-ball"></div>';
            html += '</div>';

            return html;
        },
        init: function() {
            var self = this;
            var ix = 0, iex = 0, dragging = false, ball = undefined, track = undefined, bubble = undefined;

            // Ball dragging
            $(document).on('mousedown', '#' + self.elementID + ' .sq-control-slider-ball', function(e) {
                ball = $('#' + self.elementID).find('.sq-control-slider-ball');
                track = $('#' + self.elementID).find('.sq-control-slider-track');
                bubble = $('#' + self.elementID).find('.sq-control-slider-bubble');
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
            $(document).on('mousedown', '#' + self.elementID + ' .sq-control-slider-track', function(e) {
                ball = $('#' + self.elementID).find('.sq-control-slider-ball');
                track = $('#' + self.elementID).find('.sq-control-slider-track');
                bubble = $('#' + self.elementID).find('.sq-control-slider-bubble');

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
    $.squaresRegisterControl({
        type: 'grid system',
        getValue: function() {
            // tmp
            var res = {
                xs: {
                    use: 1,
                    class: 'sq-col-xs-1',
                    visible: 1
                },
                sm: {
                    use: 1,
                    class: 'sq-col-sm-1',
                    visible: 1
                },
                md: {
                    use: 1,
                    class: 'sq-col-md-1',
                    visible: 1
                },
                lg: {
                    use: 1,
                    class: 'sq-col-lg-1',
                    visible: 1
                },
            };

            var root = $('#' + this.elementID);

            // XS ---------
            var xsGroup = root.find('.sq-grid-system-control-res-group-xs');

            // Use
            if (xsGroup.find('.sq-grid-system-control-res-use-checkbox').get(0).checked) {
                res.xs.use = 1;
            } else {
                res.xs.use = 0;
            }

            // Class
            res.xs.class = xsGroup.find('.sq-grid-system-control-select-colspan').val();

            // Visible
            if (xsGroup.find('.sq-grid-system-control-visible').hasClass('sq-grid-system-control-visible-not')) {
                res.xs.visible = 0;
            } else {
                res.xs.visible = 1;
            }

            // SM ---------
            var smGroup = root.find('.sq-grid-system-control-res-group-sm');

            // Use
            if (smGroup.find('.sq-grid-system-control-res-use-checkbox').get(0).checked) {
                res.sm.use = 1;
            } else {
                res.sm.use = 0;
            }

            // Class
            res.sm.class = smGroup.find('.sq-grid-system-control-select-colspan').val();

            // Visible
            if (smGroup.find('.sq-grid-system-control-visible').hasClass('sq-grid-system-control-visible-not')) {
                res.sm.visible = 0;
            } else {
                res.sm.visible = 1;
            }

            // MD ---------
            var mdGroup = root.find('.sq-grid-system-control-res-group-md');

            // Use
            if (mdGroup.find('.sq-grid-system-control-res-use-checkbox').get(0).checked) {
                res.md.use = 1;
            } else {
                res.md.use = 0;
            }

            // Class
            res.md.class = mdGroup.find('.sq-grid-system-control-select-colspan').val();

            // Visible
            if (mdGroup.find('.sq-grid-system-control-visible').hasClass('sq-grid-system-control-visible-not')) {
                res.md.visible = 0;
            } else {
                res.md.visible = 1;
            }

            // LG ---------
            var lgGroup = root.find('.sq-grid-system-control-res-group-lg');

            // Use
            if (lgGroup.find('.sq-grid-system-control-res-use-checkbox').get(0).checked) {
                res.lg.use = 1;
            } else {
                res.lg.use = 0;
            }

            // Class
            res.lg.class = lgGroup.find('.sq-grid-system-control-select-colspan').val();

            // Visible
            if (lgGroup.find('.sq-grid-system-control-visible').hasClass('sq-grid-system-control-visible-not')) {
                res.lg.visible = 0;
            } else {
                res.lg.visible = 1;
            }

            return res;
        },
        setValue: function(v) {
            var root = $('#' + this.elementID);

            // XS ---------
            var xsGroup = root.find('.sq-grid-system-control-res-group-xs');

            // Use
            if (parseInt(v.xs.use, 10) == 1) {
                xsGroup.find('.sq-grid-system-control-res-use-checkbox').get(0).checked = true;
                xsGroup.find('select').removeAttr('disabled');
                xsGroup.find('.sq-grid-system-control-visible').removeClass('sq-control-disabled');
            } else {
                xsGroup.find('.sq-grid-system-control-res-use-checkbox').get(0).checked = false;
                xsGroup.find('select').attr('disabled', 'disabled');
                xsGroup.find('.sq-grid-system-control-visible').addClass('sq-control-disabled');
            }

            // Class
            xsGroup.find('.sq-grid-system-control-select-colspan').val(v.xs.class);

            // Visible
            if (parseInt(v.xs.visible, 10) == 1) {
                xsGroup.find('.sq-grid-system-control-visible').removeClass('sq-grid-system-control-visible-not');
            } else {
                xsGroup.find('.sq-grid-system-control-visible').addClass('sq-grid-system-control-visible-not');
            }

            // SM ---------
            var smGroup = root.find('.sq-grid-system-control-res-group-sm');

            // Use
            if (parseInt(v.sm.use, 10) == 1) {
                smGroup.find('.sq-grid-system-control-res-use-checkbox').get(0).checked = true;
                smGroup.find('select').removeAttr('disabled');
                smGroup.find('.sq-grid-system-control-visible').removeClass('sq-control-disabled');
            } else {
                smGroup.find('.sq-grid-system-control-res-use-checkbox').get(0).checked = false;
                smGroup.find('select').attr('disabled', 'disabled');
                smGroup.find('.sq-grid-system-control-visible').addClass('sq-control-disabled');
            }

            // Class
            smGroup.find('.sq-grid-system-control-select-colspan').val(v.sm.class);

            // Visible
            if (parseInt(v.sm.visible, 10) == 1) {
                smGroup.find('.sq-grid-system-control-visible').removeClass('sq-grid-system-control-visible-not');
            } else {
                smGroup.find('.sq-grid-system-control-visible').addClass('sq-grid-system-control-visible-not');
            }

            // MD ---------
            var mdGroup = root.find('.sq-grid-system-control-res-group-md');

            // Use
            if (parseInt(v.md.use, 10) == 1) {
                mdGroup.find('.sq-grid-system-control-res-use-checkbox').get(0).checked = true;
                mdGroup.find('select').removeAttr('disabled');
                mdGroup.find('.sq-grid-system-control-visible').removeClass('sq-control-disabled');
            } else {
                mdGroup.find('.sq-grid-system-control-res-use-checkbox').get(0).checked = false;
                mdGroup.find('select').attr('disabled', 'disabled');
                mdGroup.find('.sq-grid-system-control-visible').addClass('sq-control-disabled');
            }

            // Class
            mdGroup.find('.sq-grid-system-control-select-colspan').val(v.md.class);

            // Visible
            if (parseInt(v.md.visible, 10) == 1) {
                mdGroup.find('.sq-grid-system-control-visible').removeClass('sq-grid-system-control-visible-not');
            } else {
                mdGroup.find('.sq-grid-system-control-visible').addClass('sq-grid-system-control-visible-not');
            }

            // LG ---------
            var lgGroup = root.find('.sq-grid-system-control-res-group-lg');

            // Use
            if (parseInt(v.lg.use, 10) == 1) {
                lgGroup.find('.sq-grid-system-control-res-use-checkbox').get(0).checked = true;
                lgGroup.find('select').removeAttr('disabled');
                lgGroup.find('.sq-grid-system-control-visible').removeClass('sq-control-disabled');
            } else {
                lgGroup.find('.sq-grid-system-control-res-use-checkbox').get(0).checked = false;
                lgGroup.find('select').attr('disabled', 'disabled');
                lgGroup.find('.sq-grid-system-control-visible').addClass('sq-control-disabled');
            }

            // Class
            lgGroup.find('.sq-grid-system-control-select-colspan').val(v.lg.class);

            // Visible
            if (parseInt(v.lg.visible, 10) == 1) {
                lgGroup.find('.sq-grid-system-control-visible').removeClass('sq-grid-system-control-visible-not');
            } else {
                lgGroup.find('.sq-grid-system-control-visible').addClass('sq-grid-system-control-visible-not');
            }
        },
        HTML: function() {
            var html = '';

            html += '<div class="sq-grid-system-control" id="'+ this.elementID +'">';

            // LG
            html += '   <div class="sq-grid-system-control-res-group sq-grid-system-control-res-group-lg">';
            html += '       <div class="sq-grid-system-control-res-name">LG</div>';
            html += '       <div class="sq-grid-system-control-res-use">';
            html += '           <input type="checkbox" class="sq-grid-system-control-res-use-checkbox">';
            html += '       </div>';
            html += '       <div class="sq-grid-system-control-colspan">';
            html += '           <select class="sq-grid-system-control-select-colspan">';
            html += '               <option value="sq-col-lg-1">1 Column</option>';
            html += '               <option value="sq-col-lg-2">2 Columns</option>';
            html += '               <option value="sq-col-lg-3">3 Columns</option>';
            html += '               <option value="sq-col-lg-4">4 Columns</option>';
            html += '               <option value="sq-col-lg-5">5 Columns</option>';
            html += '               <option value="sq-col-lg-6">6 Column</option>';
            html += '               <option value="sq-col-lg-7">7 Columns</option>';
            html += '               <option value="sq-col-lg-8">8 Columns</option>';
            html += '               <option value="sq-col-lg-9">9 Columns</option>';
            html += '               <option value="sq-col-lg-10">10 Columns</option>';
            html += '               <option value="sq-col-lg-11">11 Columns</option>';
            html += '               <option value="sq-col-lg-12">12 Columns</option>';
            html += '           </select>';
            html += '       </div>';
            html += '       <div class="sq-grid-system-control-visible">';
            html += '           <i class="fa fa-eye" aria-hidden="true"></i>';
            html += '           <i class="fa fa-eye-slash" aria-hidden="true"></i>';
            html += '       </div>';
            html += '   </div>';

            // MD
            html += '   <div class="sq-grid-system-control-res-group sq-grid-system-control-res-group-md">';
            html += '       <div class="sq-grid-system-control-res-name">MD</div>';
            html += '       <div class="sq-grid-system-control-res-use">';
            html += '           <input type="checkbox" class="sq-grid-system-control-res-use-checkbox">';
            html += '       </div>';
            html += '       <div class="sq-grid-system-control-colspan">';
            html += '           <select class="sq-grid-system-control-select-colspan">';
            html += '               <option value="sq-col-md-1">1 Column</option>';
            html += '               <option value="sq-col-md-2">2 Columns</option>';
            html += '               <option value="sq-col-md-3">3 Columns</option>';
            html += '               <option value="sq-col-md-4">4 Columns</option>';
            html += '               <option value="sq-col-md-5">5 Columns</option>';
            html += '               <option value="sq-col-md-6">6 Column</option>';
            html += '               <option value="sq-col-md-7">7 Columns</option>';
            html += '               <option value="sq-col-md-8">8 Columns</option>';
            html += '               <option value="sq-col-md-9">9 Columns</option>';
            html += '               <option value="sq-col-md-10">10 Columns</option>';
            html += '               <option value="sq-col-md-11">11 Columns</option>';
            html += '               <option value="sq-col-md-12">12 Columns</option>';
            html += '           </select>';
            html += '       </div>';
            html += '       <div class="sq-grid-system-control-visible">';
            html += '           <i class="fa fa-eye" aria-hidden="true"></i>';
            html += '           <i class="fa fa-eye-slash" aria-hidden="true"></i>';
            html += '       </div>';
            html += '   </div>';

            // SM
            html += '   <div class="sq-grid-system-control-res-group sq-grid-system-control-res-group-sm">';
            html += '       <div class="sq-grid-system-control-res-name">SM</div>';
            html += '       <div class="sq-grid-system-control-res-use">';
            html += '           <input type="checkbox" class="sq-grid-system-control-res-use-checkbox">';
            html += '       </div>';
            html += '       <div class="sq-grid-system-control-colspan">';
            html += '           <select class="sq-grid-system-control-select-colspan">';
            html += '               <option value="sq-col-sm-1">1 Column</option>';
            html += '               <option value="sq-col-sm-2">2 Columns</option>';
            html += '               <option value="sq-col-sm-3">3 Columns</option>';
            html += '               <option value="sq-col-sm-4">4 Columns</option>';
            html += '               <option value="sq-col-sm-5">5 Columns</option>';
            html += '               <option value="sq-col-sm-6">6 Column</option>';
            html += '               <option value="sq-col-sm-7">7 Columns</option>';
            html += '               <option value="sq-col-sm-8">8 Columns</option>';
            html += '               <option value="sq-col-sm-9">9 Columns</option>';
            html += '               <option value="sq-col-sm-10">10 Columns</option>';
            html += '               <option value="sq-col-sm-11">11 Columns</option>';
            html += '               <option value="sq-col-sm-12">12 Columns</option>';
            html += '           </select>';
            html += '       </div>';
            html += '       <div class="sq-grid-system-control-visible">';
            html += '           <i class="fa fa-eye" aria-hidden="true"></i>';
            html += '           <i class="fa fa-eye-slash" aria-hidden="true"></i>';
            html += '       </div>';
            html += '   </div>';

            // XS
            html += '   <div class="sq-grid-system-control-res-group sq-grid-system-control-res-group-xs">';
            html += '       <div class="sq-grid-system-control-res-name">XS</div>';
            html += '       <div class="sq-grid-system-control-res-use">';
            html += '           <input type="checkbox" class="sq-grid-system-control-res-use-checkbox">';
            html += '       </div>';
            html += '       <div class="sq-grid-system-control-colspan">';
            html += '           <select class="sq-grid-system-control-select-colspan">';
            html += '               <option value="sq-col-xs-1">1 Column</option>';
            html += '               <option value="sq-col-xs-2">2 Columns</option>';
            html += '               <option value="sq-col-xs-3">3 Columns</option>';
            html += '               <option value="sq-col-xs-4">4 Columns</option>';
            html += '               <option value="sq-col-xs-5">5 Columns</option>';
            html += '               <option value="sq-col-xs-6">6 Column</option>';
            html += '               <option value="sq-col-xs-7">7 Columns</option>';
            html += '               <option value="sq-col-xs-8">8 Columns</option>';
            html += '               <option value="sq-col-xs-9">9 Columns</option>';
            html += '               <option value="sq-col-xs-10">10 Columns</option>';
            html += '               <option value="sq-col-xs-11">11 Columns</option>';
            html += '               <option value="sq-col-xs-12">12 Columns</option>';
            html += '           </select>';
            html += '       </div>';
            html += '       <div class="sq-grid-system-control-visible">';
            html += '           <i class="fa fa-eye" aria-hidden="true"></i>';
            html += '           <i class="fa fa-eye-slash" aria-hidden="true"></i>';
            html += '       </div>';
            html += '   </div>';

            // end
            html += '   <div class="squares-clear"></div>';
            html += '</div>';

            return html;
        },
        init: function() {
            var self = this;
            // self.valueChanged();

            // "Use" checkboxes
            $(document).on('change', '#' + this.elementID + ' .sq-grid-system-control-res-use-checkbox', function() {
                // Enable/disable the other inputs from this resolution group

                if ($(this).get(0).checked) {
                    $(this).closest('.sq-grid-system-control-res-group').find('select').removeAttr('disabled');
                    $(this).closest('.sq-grid-system-control-res-group').find('.sq-grid-system-control-visible').removeClass('sq-control-disabled');
                } else {
                    $(this).closest('.sq-grid-system-control-res-group').find('select').attr('disabled', 'disabled');
                    $(this).closest('.sq-grid-system-control-res-group').find('.sq-grid-system-control-visible').addClass('sq-control-disabled');
                }

                self.valueChanged();
            });

            // Toggle visibility
            $(document).on('click', '#' + this.elementID + ' .sq-grid-system-control-visible', function() {
                $(this).toggleClass('sq-grid-system-control-visible-not');
                self.valueChanged();
            });

            // Select colspan
            $(document).on('change', '#' + this.elementID + ' .sq-grid-system-control-select-colspan', function() {
                self.valueChanged();
            });
        }
    });
    $.squaresRegisterControl({
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

            html += '<div class="sq-control-switch" id="'+ this.elementID +'">';
            html += '   <div class="sq-control-switch-ball"></div>';
            html += '</div>';

            html += '<div class="sq-control-switch-label" id="'+ this.elementID +'-label">'+ this.name +'</div>';
            html += '<div class="squares-clear"></div>';

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
    $.squaresRegisterControl({
        type: 'button group',
        getValue: function() {
            var v = $('#' + this.elementID).find('.active[data-button-value]').data('button-value');

            return v;
        },
        setValue: function(v) {
            $('#' + this.elementID).find('[data-button-value]').removeClass('active');
            $('#' + this.elementID).find('[data-button-value="'+ v +'"]').addClass('active');
        },
        HTML: function() {
            var html = '';

            html += '<div class="sq-control-button-group" id="'+ this.elementID +'">';

            for (var i=0; i<this.options.length; i++) {
                html += '<div class="sq-control-button-group-button" data-button-value="'+ this.options[i] +'">'+ this.options[i] +'</div>';
            }

            html += '</div>';

            return html;
        },
        init: function() {
            var self = this;

            $(document).on('click', '#' + this.elementID + ' .sq-control-button-group-button', function() {
                $(this).siblings().removeClass('active').removeClass('no-border-right');
                $(this).prev().addClass('no-border-right');
                $(this).addClass('active');
                self.valueChanged();
            });
        }
    });
    $.squaresRegisterControl({
        type: 'wp media upload',
        getValue: function() {
            return $('#' + this.elementID + ' input').val();
        },
        setValue: function(v) {
            $('#' + this.elementID + ' input').val(v);
        },
        HTML: function() {
            return '<div class="sq-input-with-button" id="'+ this.elementID +'"><input type="text"><div class="sq-control-button">Choose Media</div></div>';
        },
        init: function() {
            var self = this;

            var inputSelector = '#' + this.elementID + ' input';
            var buttonSelector = '#' + this.elementID + ' .sq-control-button';

            $.wcpWPMedia(inputSelector, buttonSelector, function() {
                self.valueChanged();
            });
        }
    });
})(jQuery, window, document);
