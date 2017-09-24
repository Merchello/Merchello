(function () {
    var app = angular.module('Umbraco.canvasdesigner', [
        'colorpicker',
        'ui.slider',
        'umbraco.resources',
        'umbraco.services'
    ]).controller('Umbraco.canvasdesignerController', function ($scope, $http, $window, $timeout, $location, dialogService) {
        $scope.isOpen = false;
        $scope.frameLoaded = false;
        $scope.enableCanvasdesigner = 0;
        $scope.googleFontFamilies = {};
        $scope.pageId = $location.search().id;
        $scope.pageUrl = '../dialogs/Preview.aspx?id=' + $location.search().id;
        $scope.valueAreLoaded = false;
        $scope.devices = [
            {
                name: 'desktop',
                css: 'desktop',
                icon: 'icon-display',
                title: 'Desktop'
            },
            {
                name: 'laptop - 1366px',
                css: 'laptop border',
                icon: 'icon-laptop',
                title: 'Laptop'
            },
            {
                name: 'iPad portrait - 768px',
                css: 'iPad-portrait border',
                icon: 'icon-ipad',
                title: 'Tablet portrait'
            },
            {
                name: 'iPad landscape - 1024px',
                css: 'iPad-landscape border',
                icon: 'icon-ipad flip',
                title: 'Tablet landscape'
            },
            {
                name: 'smartphone portrait - 480px',
                css: 'smartphone-portrait border',
                icon: 'icon-iphone',
                title: 'Smartphone portrait'
            },
            {
                name: 'smartphone landscape  - 320px',
                css: 'smartphone-landscape border',
                icon: 'icon-iphone flip',
                title: 'Smartphone landscape'
            }
        ];
        $scope.previewDevice = $scope.devices[0];
        var apiController = '../Api/Canvasdesigner/';
        /*****************************************************************************/
        /* Preview devices */
        /*****************************************************************************/
        // Set preview device
        $scope.updatePreviewDevice = function (device) {
            $scope.previewDevice = device;
        };
        /*****************************************************************************/
        /* Exit Preview */
        /*****************************************************************************/
        $scope.exitPreview = function () {
            window.top.location.href = '../endPreview.aspx?redir=%2f' + $scope.pageId;
        };
        /*****************************************************************************/
        /* UI designer managment */
        /*****************************************************************************/
        // Update all Canvasdesigner config's values from data
        var updateConfigValue = function (data) {
            var fonts = [];
            $.each($scope.canvasdesignerModel.configs, function (indexConfig, config) {
                if (config.editors) {
                    $.each(config.editors, function (indexItem, item) {
                        /* try to get value */
                        try {
                            if (item.values) {
                                angular.forEach(Object.keys(item.values), function (key, indexKey) {
                                    if (key != '\'\'') {
                                        var propertyAlias = key.toLowerCase() + item.alias.toLowerCase();
                                        var newValue = eval('data.' + propertyAlias.replace('@', ''));
                                        if (newValue == '\'\'') {
                                            newValue = '';
                                        }
                                        item.values[key] = newValue;
                                    }
                                });
                            }
                            // TODO: special init for font family picker
                            if (item.type == 'googlefontpicker') {
                                if (item.values.fontType == 'google' && item.values.fontFamily + item.values.fontWeight && $.inArray(item.values.fontFamily + ':' + item.values.fontWeight, fonts) < 0) {
                                    fonts.splice(0, 0, item.values.fontFamily + ':' + item.values.fontWeight);
                                }
                            }
                        } catch (err) {
                            console.info('Style parameter not found ' + item.alias);
                        }
                    });
                }
            });
            // Load google font
            $.each(fonts, function (indexFont, font) {
                loadGoogleFont(font);
                loadGoogleFontInFront(font);
            });
            $scope.valueAreLoaded = true;
        };
        // Load parameters from GetLessParameters and init data of the Canvasdesigner config
        $scope.initCanvasdesigner = function () {
            LazyLoad.js(['https://ajax.googleapis.com/ajax/libs/webfont/1/webfont.js']);
            $http.get(apiController + 'Load', { params: { pageId: $scope.pageId } }).success(function (data) {
                updateConfigValue(data);
                $timeout(function () {
                    $scope.frameLoaded = true;
                }, 200);
            });
        };
        // Refresh all less parameters for every changes watching canvasdesignerModel
        var refreshCanvasdesigner = function () {
            var parameters = [];
            if ($scope.canvasdesignerModel) {
                angular.forEach($scope.canvasdesignerModel.configs, function (config, indexConfig) {
                    // Get currrent selected element
                    // TODO
                    //if ($scope.schemaFocus && angular.lowercase($scope.schemaFocus) == angular.lowercase(config.name)) {
                    //    $scope.currentSelected = config.selector ? config.selector : config.schema;
                    //}
                    if (config.editors) {
                        angular.forEach(config.editors, function (item, indexItem) {
                            // Add new style
                            if (item.values) {
                                angular.forEach(Object.keys(item.values), function (key, indexKey) {
                                    var propertyAlias = key.toLowerCase() + item.alias.toLowerCase();
                                    var value = eval('item.values.' + key);
                                    parameters.splice(parameters.length + 1, 0, '\'@' + propertyAlias + '\':\'' + value + '\'');
                                });
                            }
                        });
                    }
                });
                // Refresh page style
                refreshFrontStyles(parameters);
                // Refresh layout of selected element
                //$timeout(function () {
                $scope.positionSelectedHide();
                if ($scope.currentSelected) {
                    refreshOutlineSelected($scope.currentSelected);
                }    //}, 200);
            }
        };
        $scope.createStyle = function () {
            $scope.saveLessParameters(false);
        };
        $scope.saveStyle = function () {
            $scope.saveLessParameters(true);
        };
        // Save all parameter in CanvasdesignerParameters.less file
        $scope.saveLessParameters = function (inherited) {
            var parameters = [];
            $.each($scope.canvasdesignerModel.configs, function (indexConfig, config) {
                if (config.editors) {
                    $.each(config.editors, function (indexItem, item) {
                        if (item.values) {
                            angular.forEach(Object.keys(item.values), function (key, indexKey) {
                                var propertyAlias = key.toLowerCase() + item.alias.toLowerCase();
                                var value = eval('item.values.' + key);
                                parameters.splice(parameters.length + 1, 0, '@' + propertyAlias + ':' + value + ';');
                            });
                            // TODO: special init for font family picker
                            if (item.type == 'googlefontpicker' && item.values.fontFamily) {
                                var variant = item.values.fontWeight != '' || item.values.fontStyle != '' ? ':' + item.values.fontWeight + item.values.fontStyle : '';
                                var gimport = '@import url(\'https://fonts.googleapis.com/css?family=' + item.values.fontFamily + variant + '\');';
                                if ($.inArray(gimport, parameters) < 0) {
                                    parameters.splice(0, 0, gimport);
                                }
                            }
                        }
                    });
                }
            });
            var resultParameters = {
                parameters: parameters.join(''),
                pageId: $scope.pageId,
                inherited: inherited
            };
            var transform = function (result) {
                return $.param(result);
            };
            $('.btn-default-save').attr('disabled', true);
            $http.defaults.headers.post['Content-Type'] = 'application/x-www-form-urlencoded';
            $http.post(apiController + 'Save', resultParameters, {
                headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' },
                transformRequest: transform
            }).success(function (data) {
                $('.btn-default-save').attr('disabled', false);
                $('#speechbubble').fadeIn('slow').delay(5000).fadeOut('slow');
            });
        };
        // Delete current page Canvasdesigner
        $scope.deleteCanvasdesigner = function () {
            $('.btn-default-delete').attr('disabled', true);
            $http.get(apiController + 'Delete', { params: { pageId: $scope.pageId } }).success(function (data) {
                location.reload();
            });
        };
        /*****************************************************************************/
        /* Preset design */
        /*****************************************************************************/
        // Refresh with selected Canvasdesigner palette
        $scope.refreshCanvasdesignerByPalette = function (palette) {
            updateConfigValue(palette.data);
            refreshCanvasdesigner();
        };
        // Hidden botton to make preset from the current settings
        $scope.makePreset = function () {
            var parameters = [];
            $.each($scope.canvasdesignerModel.configs, function (indexConfig, config) {
                if (config.editors) {
                    $.each(config.editors, function (indexItem, item) {
                        if (item.values) {
                            angular.forEach(Object.keys(item.values), function (key, indexKey) {
                                var propertyAlias = key.toLowerCase() + item.alias.toLowerCase();
                                var value = eval('item.values.' + key);
                                var value = value != 0 && (value == undefined || value == '') ? '\'\'' : value;
                                parameters.splice(parameters.length + 1, 0, '"' + propertyAlias + '":' + ' "' + value + '"');
                            });
                        }
                    });
                }
            });
            $('.btn-group').append('<textarea>{name:"", color1:"", color2:"", color3:"", color4:"", color5:"", data:{' + parameters.join(',') + '}}</textarea>');
        };
        /*****************************************************************************/
        /* Panel managment */
        /*****************************************************************************/
        $scope.openPreviewDevice = function () {
            $scope.showDevicesPreview = true;
            $scope.closeIntelCanvasdesigner();
        };
        $scope.closePreviewDevice = function () {
            $scope.showDevicesPreview = false;
            if ($scope.showStyleEditor) {
                $scope.openIntelCanvasdesigner();
            }
        };
        $scope.openPalettePicker = function () {
            $scope.showPalettePicker = true;
            $scope.showStyleEditor = false;
            $scope.closeIntelCanvasdesigner();
        };
        $scope.openStyleEditor = function () {
            $scope.showStyleEditor = true;
            $scope.showPalettePicker = false;
            $scope.outlineSelectedHide();
            $scope.openIntelCanvasdesigner();
        };
        // Remove value from field
        $scope.removeField = function (field) {
            field.value = '';
        };
        // Check if category existe
        $scope.hasEditor = function (editors, category) {
            var result = false;
            angular.forEach(editors, function (item, index) {
                if (item.category == category) {
                    result = true;
                }
            });
            return result;
        };
        $scope.closeFloatPanels = function () {
            /* hack to hide color picker */
            $('.spectrumcolorpicker input').spectrum('hide');
            dialogService.close();
            $scope.showPalettePicker = false;
            $scope.$apply();
        };
        $scope.clearHighlightedItems = function () {
            $.each($scope.canvasdesignerModel.configs, function (indexConfig, config) {
                config.highlighted = false;
            });
        };
        $scope.setCurrentHighlighted = function (item) {
            $scope.clearHighlightedItems();
            item.highlighted = true;
        };
        $scope.setCurrentSelected = function (item) {
            $scope.currentSelected = item;
            $scope.clearSelectedCategory();
            refreshOutlineSelected($scope.currentSelected);
        };
        /* Editor categories */
        $scope.getCategories = function (item) {
            var propertyCategories = [];
            $.each(item.editors, function (indexItem, editor) {
                if (editor.category) {
                    if ($.inArray(editor.category, propertyCategories) < 0) {
                        propertyCategories.splice(propertyCategories.length + 1, 0, editor.category);
                    }
                }
            });
            return propertyCategories;
        };
        $scope.setSelectedCategory = function (item) {
            $scope.categoriesVisibility = $scope.categoriesVisibility || {};
            $scope.categoriesVisibility[item] = !$scope.categoriesVisibility[item];
        };
        $scope.clearSelectedCategory = function () {
            $scope.categoriesVisibility = null;
        };
        /*****************************************************************************/
        /* Call function into the front-end   */
        /*****************************************************************************/
        var loadGoogleFontInFront = function (font) {
            if (document.getElementById('resultFrame').contentWindow.getFont)
                document.getElementById('resultFrame').contentWindow.getFont(font);
        };
        var refreshFrontStyles = function (parameters) {
            if (document.getElementById('resultFrame').contentWindow.refreshLayout)
                document.getElementById('resultFrame').contentWindow.refreshLayout(parameters);
        };
        var hideUmbracoPreviewBadge = function () {
            var iframe = document.getElementById('resultFrame').contentWindow || document.getElementById('resultFrame').contentDocument;
            if (iframe.document.getElementById('umbracoPreviewBadge'))
                iframe.document.getElementById('umbracoPreviewBadge').style.display = 'none';
        };
        $scope.openIntelCanvasdesigner = function () {
            if (document.getElementById('resultFrame').contentWindow.initIntelCanvasdesigner)
                document.getElementById('resultFrame').contentWindow.initIntelCanvasdesigner($scope.canvasdesignerModel);
        };
        $scope.closeIntelCanvasdesigner = function () {
            if (document.getElementById('resultFrame').contentWindow.closeIntelCanvasdesigner)
                document.getElementById('resultFrame').contentWindow.closeIntelCanvasdesigner($scope.canvasdesignerModel);
            $scope.outlineSelectedHide();
        };
        var refreshOutlineSelected = function (config) {
            var schema = config.selector ? config.selector : config.schema;
            if (document.getElementById('resultFrame').contentWindow.refreshOutlineSelected)
                document.getElementById('resultFrame').contentWindow.refreshOutlineSelected(schema);
        };
        $scope.outlineSelectedHide = function () {
            $scope.currentSelected = null;
            if (document.getElementById('resultFrame').contentWindow.outlineSelectedHide)
                document.getElementById('resultFrame').contentWindow.outlineSelectedHide();
        };
        $scope.refreshOutlinePosition = function (config) {
            var schema = config.selector ? config.selector : config.schema;
            if (document.getElementById('resultFrame').contentWindow.refreshOutlinePosition)
                document.getElementById('resultFrame').contentWindow.refreshOutlinePosition(schema);
        };
        $scope.positionSelectedHide = function () {
            if (document.getElementById('resultFrame').contentWindow.outlinePositionHide)
                document.getElementById('resultFrame').contentWindow.outlinePositionHide();
        };
        /*****************************************************************************/
        /* Google font loader, TODO: put together from directive, front and back */
        /*****************************************************************************/
        var webFontScriptLoaded = false;
        var loadGoogleFont = function (font) {
            if (!webFontScriptLoaded) {
                $.getScript('https://ajax.googleapis.com/ajax/libs/webfont/1/webfont.js').done(function () {
                    webFontScriptLoaded = true;
                    // Recursively call once webfont script is available.
                    loadGoogleFont(font);
                }).fail(function () {
                    console.log('error loading webfont');
                });
            } else {
                WebFont.load({
                    google: { families: [font] },
                    loading: function () {
                    },
                    active: function () {
                    },
                    inactive: function () {
                    }
                });
            }
        };
        /*****************************************************************************/
        /* Init */
        /*****************************************************************************/
        // Preload of the google font
        if ($scope.showStyleEditor) {
            $http.get(apiController + 'GetGoogleFont').success(function (data) {
                $scope.googleFontFamilies = data;
            });
        }
        // watch framLoaded, only if iframe page have enableCanvasdesigner()
        $scope.$watch('enableCanvasdesigner', function () {
            $timeout(function () {
                if ($scope.enableCanvasdesigner > 0) {
                    $scope.$watch('ngRepeatFinished', function (ngRepeatFinishedEvent) {
                        $timeout(function () {
                            $scope.initCanvasdesigner();
                        }, 200);
                    });
                    $scope.$watch('canvasdesignerModel', function () {
                        refreshCanvasdesigner();
                    }, true);
                }
            }, 100);
        }, true);
    }).directive('onFinishRenderFilters', function ($timeout) {
        return {
            restrict: 'A',
            link: function (scope, element, attr) {
                if (scope.$last === true) {
                    $timeout(function () {
                        scope.$emit('ngRepeatFinished');
                    });
                }
            }
        };
    }).directive('iframeIsLoaded', function ($timeout) {
        return {
            restrict: 'A',
            link: function (scope, element, attr) {
                element.load(function () {
                    var iframe = element.context.contentWindow || element.context.contentDocument;
                    if (iframe && iframe.document.getElementById('umbracoPreviewBadge'))
                        iframe.document.getElementById('umbracoPreviewBadge').style.display = 'none';
                    if (!document.getElementById('resultFrame').contentWindow.refreshLayout) {
                        scope.frameLoaded = true;
                        scope.$apply();
                    }
                });
            }
        };
    });
    /*********************************************************************************************************/
    /* Global function and variable for panel/page com */
    /*********************************************************************************************************/
    var currentTarget = undefined;
    var refreshLayout = function (parameters) {
        // hide preview badget
        $('#umbracoPreviewBadge').hide();
        var string = 'less.modifyVars({' + parameters.join(',') + '})';
        eval(string);
    };
    /* Fonts loaded in the Canvasdesigner panel need to be loaded independently in
 * the content iframe to allow live previewing.
 */
    var webFontScriptLoaded = false;
    var getFont = function (font) {
        if (!webFontScriptLoaded) {
            $.getScript('https://ajax.googleapis.com/ajax/libs/webfont/1/webfont.js').done(function () {
                webFontScriptLoaded = true;
                // Recursively call once webfont script is available.
                getFont(font);
            }).fail(function () {
                console.log('error loading webfont');
            });
        } else {
            WebFont.load({
                google: { families: [font] },
                loading: function () {
                },
                active: function () {
                },
                inactive: function () {
                }
            });
        }
    };
    var closeIntelCanvasdesigner = function (canvasdesignerModel) {
        if (canvasdesignerModel) {
            $.each(canvasdesignerModel.configs, function (indexConfig, config) {
                if (config.schema) {
                    $(config.schema).unbind();
                    $(config.schema).removeAttr('canvasdesigner-over');
                }
            });
            initBodyClickEvent();
        }
    };
    var initBodyClickEvent = function () {
        $('body').on('click', function () {
            if (parent.iframeBodyClick) {
                parent.iframeBodyClick();
            }
        });
    };
    var initIntelCanvasdesigner = function (canvasdesignerModel) {
        if (canvasdesignerModel) {
            // Add canvasdesigner-over attr for each schema from config
            $.each(canvasdesignerModel.configs, function (indexConfig, config) {
                var schema = config.selector ? config.selector : config.schema;
                if (schema) {
                    $(schema).attr('canvasdesigner-over', config.schema);
                    $(schema).attr('canvasdesigner-over-name', config.name);
                    $(schema).css('cursor', 'default');
                }
            });
            // Outline canvasdesigner-over
            $(document).mousemove(function (e) {
                e.stopPropagation();
                var target = $(e.target);
                while (target.length > 0 && (target.attr('canvasdesigner-over') == undefined || target.attr('canvasdesigner-over') == '')) {
                    target = target.parent();
                }
                if (target.attr('canvasdesigner-over') != undefined && target.attr('canvasdesigner-over') != '') {
                    target.unbind();
                    outlinePosition(target);
                    parent.onMouseoverCanvasdesignerItem(target.attr('canvasdesigner-over-name'), target);
                    target.click(function (e) {
                        e.stopPropagation();
                        e.preventDefault();
                        //console.info(target.attr('canvasdesigner-over'));
                        currentTarget = target;
                        outlineSelected();
                        parent.onClickCanvasdesignerItem(target.attr('canvasdesigner-over'), target);
                        return false;
                    });
                } else {
                    outlinePositionHide();
                }
            });
        }
    };
    var refreshOutlinePosition = function (schema) {
        outlinePosition($(schema));
    };
    var outlinePosition = function (oTarget) {
        var target = oTarget;
        if (target.length > 0 && target.attr('canvasdesigner-over') != undefined && target.attr('canvasdesigner-over') != '') {
            var localname = target[0].localName;
            var height = $(target).outerHeight();
            var width = $(target).outerWidth();
            var position = $(target).offset();
            var posY = position.top;
            //$(window).scrollTop();
            var posX = position.left;
            //+ $(window).scrollLeft();
            $('.canvasdesigner-overlay').css('display', 'block');
            $('.canvasdesigner-overlay').css('left', posX);
            $('.canvasdesigner-overlay').css('top', posY);
            $('.canvasdesigner-overlay').css('width', width + 'px');
            $('.canvasdesigner-overlay').css('height', height + 'px');
            //console.info("element select " + localname);
            $('.canvasdesigner-overlay span').html(target.attr('canvasdesigner-over-name'));
        } else {
            outlinePositionHide();    //console.info("element not found select");
        }
    };
    var refreshOutlineSelected = function (schema) {
        outlineSelected($(schema));
    };
    var outlineSelected = function (oTarget) {
        var target = currentTarget;
        if (oTarget) {
            currentTarget = oTarget;
            target = oTarget;
        }
        if (target && target.length > 0 && target.attr('canvasdesigner-over') != undefined && target.attr('canvasdesigner-over') != '') {
            var localname = target[0].localName;
            var height = $(target).outerHeight();
            var width = $(target).outerWidth();
            var position = $(target).offset();
            var posY = position.top;
            //$(window).scrollTop();
            var posX = position.left;
            //+ $(window).scrollLeft();
            $('.canvasdesigner-overlay-selected').css('display', 'block');
            $('.canvasdesigner-overlay-selected').css('left', posX);
            $('.canvasdesigner-overlay-selected').css('top', posY);
            $('.canvasdesigner-overlay-selected').css('width', width + 'px');
            $('.canvasdesigner-overlay-selected').css('height', height + 'px');
            //console.info("element select " + localname);
            $('.canvasdesigner-overlay-selected span').html(target.attr('canvasdesigner-over-name'));
        } else {
            outlinePositionHide();    //console.info("element not found select");
        }
    };
    var outlinePositionHide = function () {
        $('.canvasdesigner-overlay').css('display', 'none');
    };
    var outlineSelectedHide = function () {
        currentTarget = undefined;
        $('.canvasdesigner-overlay-selected').css('display', 'none');
    };
    var initCanvasdesignerPanel = function () {
        $('link[data-title="canvasdesignerCss"]').attr('disabled', 'disabled');
        // First load the canvasdesigner config from file
        if (!canvasdesignerConfig) {
            console.info('canvasdesigner config not found');
        }
        // Add canvasdesigner from HTML 5 data tags
        $('[data-canvasdesigner]').each(function (index, value) {
            var tagName = $(value).data('canvasdesigner') ? $(value).data('canvasdesigner') : $(value)[0].nodeName.toLowerCase();
            var tagSchema = $(value).data('schema') ? $(value).data('schema') : $(value)[0].nodeName.toLowerCase();
            var tagSelector = $(value).data('selector') ? $(value).data('selector') : tagSchema;
            var tagEditors = $(value).data('editors');
            //JSON.parse(...);
            canvasdesignerConfig.configs.splice(canvasdesignerConfig.configs.length, 0, {
                name: tagName,
                schema: tagSchema,
                selector: tagSelector,
                editors: tagEditors
            });
        });
        // For each editor config create a composite alias
        $.each(canvasdesignerConfig.configs, function (configIndex, config) {
            if (config.editors) {
                $.each(config.editors, function (editorIndex, editor) {
                    var clearSchema = config.schema.replace(/[^a-zA-Z0-9]+/g, '').toLowerCase();
                    var clearEditor = JSON.stringify(editor).replace(/[^a-zA-Z0-9]+/g, '').toLowerCase();
                    editor.alias = clearSchema + clearEditor;
                });
            }
        });
        // Create or update the less file
        $.ajax({
            url: '/Umbraco/Api/CanvasDesigner/Init',
            type: 'POST',
            dataType: 'json',
            error: function (err) {
                alert(err.responseText);
            },
            data: {
                config: JSON.stringify(canvasdesignerConfig),
                pageId: pageId
            },
            success: function (data) {
                // Add Less link in head
                $('head').append('<link>');
                css = $('head').children(':last');
                css.attr({
                    rel: 'stylesheet/less',
                    type: 'text/css',
                    href: data
                });
                //console.info("Less styles are loaded");
                // Init Less.js
                $.getScript('/Umbraco/lib/Less/less-1.7.0.min.js', function (data, textStatus, jqxhr) {
                    // Init panel
                    if (parent.setFrameIsLoaded) {
                        parent.setFrameIsLoaded(canvasdesignerConfig, canvasdesignerPalette);
                    }
                });
            }
        });
    };
    $(function () {
        if (parent.setFrameIsLoaded) {
            // Overlay background-color: rgba(28, 203, 255, 0.05);
            $('body').append('<div class="canvasdesigner-overlay" style="display:none; pointer-events: none; position: absolute; z-index: 9999; border: 1px solid #2ebdff; border-radius: 3px; "><span style="position:absolute;background: #2ebdff; font-family: Helvetica, Arial, sans-serif; color: #fff; padding: 0 5px 0 6px; font-size: 10px; line-height: 17px; display: inline-block; border-radius: 0 0 3px 0;"></span></div>');
            $('body').append('<div class="canvasdesigner-overlay-selected" style="display:none; pointer-events: none; position: absolute; z-index: 9998; border: 2px solid #2ebdff; border-radius: 3px;"><span style="position:absolute;background: #2ebdff; font-family: Helvetica, Arial, sans-serif; color: #fff; padding: 0 5px; font-size: 10px; line-height: 16px; display: inline-block; border-radius: 0 0 3px 0;"></span></div>');
            // Set event for any body click
            initBodyClickEvent();
            // Init canvasdesigner panel
            initCanvasdesignerPanel();
        }
    });
    /*********************************************************************************************************/
    /* Global function and variable for panel/page com  */
    /*********************************************************************************************************/
    /* Called for every canvasdesigner-over click */
    var onClickCanvasdesignerItem = function (schema) {
        var scope = angular.element($('#canvasdesignerPanel')).scope();
        //if (scope.schemaFocus != schema.toLowerCase()) {
        //var notFound = true;
        $.each(scope.canvasdesignerModel.configs, function (indexConfig, config) {
            if (config.schema && schema.toLowerCase() == config.schema.toLowerCase()) {
                scope.currentSelected = config;
            }
        });
        //}
        scope.clearSelectedCategory();
        scope.closeFloatPanels();
        scope.$apply();
    };
    /* Called for every canvasdesigner-over rollover */
    var onMouseoverCanvasdesignerItem = function (name) {
        var scope = angular.element($('#canvasdesignerPanel')).scope();
        $.each(scope.canvasdesignerModel.configs, function (indexConfig, config) {
            config.highlighted = false;
            if (config.name && name.toLowerCase() == config.name.toLowerCase()) {
                config.highlighted = true;
            }
        });
        scope.$apply();
    };
    /* Called when the iframe is first loaded */
    var setFrameIsLoaded = function (canvasdesignerConfig, canvasdesignerPalette) {
        var scope = angular.element($('#canvasdesignerPanel')).scope();
        scope.canvasdesignerModel = canvasdesignerConfig;
        scope.canvasdesignerPalette = canvasdesignerPalette;
        scope.enableCanvasdesigner++;
        scope.$apply();
    };
    /* Iframe body click */
    var iframeBodyClick = function () {
        var scope = angular.element($('#canvasdesignerPanel')).scope();
        scope.closeFloatPanels();
    };
    /*********************************************************************************************************/
    /* Canvasdesigner setting panel config */
    /*********************************************************************************************************/
    var canvasdesignerConfig = {
        configs: [
            {
                name: 'Body',
                schema: 'body',
                selector: 'body',
                editors: [
                    {
                        type: 'background',
                        category: 'Color',
                        name: 'Background'
                    },
                    {
                        type: 'color',
                        category: 'Font',
                        name: 'Font Color (main)',
                        css: 'color',
                        schema: 'body, h1, h2, h3, h4, h5, h6, h7, #nav li a'
                    },
                    {
                        type: 'color',
                        category: 'Font',
                        name: 'Font Color (secondary)',
                        css: 'color',
                        schema: 'ul.meta, .byline'
                    },
                    {
                        type: 'googlefontpicker',
                        category: 'Font',
                        name: 'Font Family',
                        css: 'color',
                        schema: 'body, h1, h2, h3, h4, h5, h6, h7, .byline, #nav, .button'
                    }
                ]
            },
            {
                name: 'Nav',
                schema: '#nav',
                selector: 'nav',
                editors: [
                    {
                        type: 'background',
                        category: 'Color',
                        name: 'Background'
                    },
                    {
                        type: 'border',
                        category: 'Color',
                        name: 'Border'
                    },
                    {
                        type: 'color',
                        category: 'Nav',
                        name: 'Font Color',
                        css: 'color',
                        schema: '#nav li a'
                    },
                    {
                        type: 'color',
                        category: 'Nav',
                        name: 'Font Color (hover / selected)',
                        css: 'color',
                        schema: '#nav li:hover a'
                    },
                    {
                        type: 'color',
                        category: 'Nav',
                        name: 'Background Color (hover)',
                        css: 'background-color',
                        schema: '#nav li:hover a'
                    },
                    {
                        type: 'color',
                        category: 'Nav',
                        name: 'Background Color (selected)',
                        css: 'background-color',
                        schema: '#nav li.current_page_item a'
                    },
                    {
                        type: 'googlefontpicker',
                        category: 'Font',
                        name: 'Font familly'
                    }
                ]
            },
            {
                name: 'Logo',
                schema: '#header .logo div',
                selector: '#header .logo div',
                editors: [
                    {
                        type: 'color',
                        category: 'Color',
                        name: 'Border color',
                        css: 'border-top-color',
                        schema: '#header .logo'
                    },
                    {
                        type: 'padding',
                        category: 'Position',
                        name: 'Margin',
                        enable: [
                            'top',
                            'bottom'
                        ],
                        schema: '#header'
                    }
                ]
            },
            {
                name: 'h2',
                schema: 'h2',
                selector: 'h2 span',
                editors: [
                    {
                        type: 'color',
                        category: 'Color',
                        name: 'Border color',
                        css: 'border-top-color',
                        schema: 'h2.major'
                    },
                    {
                        type: 'color',
                        category: 'Font',
                        name: 'Font color',
                        css: 'color'
                    }
                ]
            },
            {
                name: 'h3',
                schema: 'h3',
                selector: 'h3',
                editors: [{
                        type: 'color',
                        category: 'Font',
                        name: 'Font color',
                        css: 'color'
                    }]
            },
            {
                name: 'Banner Title',
                schema: '#banner h2',
                selector: '#banner h2',
                editors: [
                    {
                        type: 'color',
                        category: 'Font',
                        name: 'Font color',
                        css: 'color'
                    },
                    {
                        type: 'slider',
                        category: 'Font',
                        name: 'Font size',
                        css: 'font-size',
                        min: 18,
                        max: 100
                    },
                    {
                        type: 'margin',
                        category: 'Position',
                        name: 'Margin'
                    }
                ]
            },
            {
                name: 'Banner Sub-title',
                schema: '#banner .byline',
                selector: '#banner .byline',
                editors: [
                    {
                        type: 'color',
                        category: 'Font',
                        name: 'Font color',
                        css: 'color'
                    },
                    {
                        type: 'slider',
                        category: 'Font',
                        name: 'Font size',
                        css: 'font-size',
                        min: 18,
                        max: 100
                    },
                    {
                        type: 'margin',
                        category: 'Position',
                        name: 'Margin'
                    }
                ]
            },
            {
                name: 'Banner',
                schema: '#banner',
                selector: '#banner',
                editors: [{
                        type: 'background',
                        category: 'Color',
                        name: 'Background',
                        css: 'color'
                    }]
            },
            {
                name: 'Banner-wrapper',
                schema: '#banner-wrapper',
                selector: '#banner-wrapper',
                editors: [
                    {
                        type: 'background',
                        category: 'Color',
                        name: 'Background'
                    },
                    {
                        type: 'padding',
                        category: 'Position',
                        name: 'Padding',
                        enable: [
                            'top',
                            'bottom'
                        ]
                    }
                ]
            },
            {
                name: '#main-wrapper',
                schema: '#main-wrapper',
                selector: '#main-wrapper',
                editors: [{
                        type: 'border',
                        category: 'Styling',
                        name: 'Border',
                        enable: [
                            'top',
                            'bottom'
                        ]
                    }]
            },
            {
                name: 'Image',
                schema: '.image,.image img,.image:before',
                selector: '.image',
                editors: [{
                        type: 'radius',
                        category: 'Styling',
                        name: 'Radius'
                    }]
            },
            {
                name: 'Button',
                schema: '.button',
                selector: '.button',
                editors: [
                    {
                        type: 'color',
                        category: 'Color',
                        name: 'Color',
                        css: 'color'
                    },
                    {
                        type: 'color',
                        category: 'Color',
                        name: 'Background',
                        css: 'background'
                    },
                    {
                        type: 'color',
                        category: 'Color',
                        name: 'Background Hover',
                        css: 'background',
                        schema: '.button:hover'
                    },
                    {
                        type: 'radius',
                        category: 'Styling',
                        name: 'Radius'
                    }
                ]
            },
            {
                name: 'Button Alt',
                schema: '.button-alt',
                selector: '.button-alt',
                editors: [
                    {
                        type: 'color',
                        category: 'Color',
                        name: 'Color',
                        css: 'color'
                    },
                    {
                        type: 'color',
                        category: 'Color',
                        name: 'Background',
                        css: 'background'
                    },
                    {
                        type: 'color',
                        category: 'Color',
                        name: 'Background Hover',
                        css: 'background',
                        schema: '.button-alt:hover'
                    }
                ]
            }
        ]
    };
    /*********************************************************************************************************/
    /* Canvasdesigner palette tab config */
    /*********************************************************************************************************/
    var canvasdesignerPalette = [
        {
            name: 'Default',
            color1: 'rgb(193, 202, 197)',
            color2: 'rgb(231, 234, 232)',
            color3: 'rgb(107, 119, 112)',
            color4: 'rgb(227, 218, 168)',
            color5: 'rgba(21, 28, 23, 0.95)',
            data: {
                'widebodytypewidecategorydimensionnamelayout': 'wide',
                'imageorpatternbodytypebackgroundcategorycolornamebackground': '',
                'colorbodytypebackgroundcategorycolornamebackground': '',
                'colorbodytypecolorcategoryfontnamefontcolormaincsscolorschemabodyh1h2h3h4h5h6h7navlia': 'rgb(107, 119, 112)',
                'colorbodytypecolorcategoryfontnamefontcolorsecondarycsscolorschemaulmetabyline': 'rgb(193, 202, 197)',
                'fontfamilybodytypegooglefontpickercategoryfontnamefontfamilycsscolorschemabodyh1h2h3h4h5h6h7bylinenavbutton': 'Open Sans Condensed',
                'fonttypebodytypegooglefontpickercategoryfontnamefontfamilycsscolorschemabodyh1h2h3h4h5h6h7bylinenavbutton': 'google',
                'fontweightbodytypegooglefontpickercategoryfontnamefontfamilycsscolorschemabodyh1h2h3h4h5h6h7bylinenavbutton': '700',
                'fontstylebodytypegooglefontpickercategoryfontnamefontfamilycsscolorschemabodyh1h2h3h4h5h6h7bylinenavbutton': '',
                'imageorpatternnavtypebackgroundcategorycolornamebackground': '',
                'colornavtypebackgroundcategorycolornamebackground': '',
                'bordersizenavtypebordercategorycolornameborder': '',
                'bordercolornavtypebordercategorycolornameborder': '',
                'bordertypenavtypebordercategorycolornameborder': 'solid',
                'leftbordersizenavtypebordercategorycolornameborder': '',
                'leftbordercolornavtypebordercategorycolornameborder': '',
                'leftbordertypenavtypebordercategorycolornameborder': 'solid',
                'rightbordersizenavtypebordercategorycolornameborder': '',
                'rightbordercolornavtypebordercategorycolornameborder': '',
                'rightbordertypenavtypebordercategorycolornameborder': 'solid',
                'topbordersizenavtypebordercategorycolornameborder': '',
                'topbordercolornavtypebordercategorycolornameborder': '',
                'topbordertypenavtypebordercategorycolornameborder': 'solid',
                'bottombordersizenavtypebordercategorycolornameborder': '',
                'bottombordercolornavtypebordercategorycolornameborder': '',
                'bottombordertypenavtypebordercategorycolornameborder': 'solid',
                'colornavtypecolorcategorynavnamefontcolorcsscolorschemanavlia': 'rgb(107, 119, 112)',
                'colornavtypecolorcategorynavnamefontcolorhoverselectedcsscolorschemanavlihovera': 'rgb(255, 255, 255)',
                'colornavtypecolorcategorynavnamebackgroundcolorhovercssbackgroundcolorschemanavlihovera': 'rgb(193, 202, 197)',
                'colornavtypecolorcategorynavnamebackgroundcolorselectedcssbackgroundcolorschemanavlicurrentpageitema': 'rgb(227, 218, 168)',
                'fontfamilynavtypegooglefontpickercategoryfontnamefontfamilly': '',
                'fonttypenavtypegooglefontpickercategoryfontnamefontfamilly': '',
                'fontweightnavtypegooglefontpickercategoryfontnamefontfamilly': '',
                'fontstylenavtypegooglefontpickercategoryfontnamefontfamilly': '',
                'colorheaderlogodivtypecolorcategorycolornamebordercolorcssbordertopcolorschemaheaderlogo': 'rgb(231, 234, 232)',
                'paddingvalueheaderlogodivtypepaddingcategorypositionnamemarginenabletopbottomschemaheader': '',
                'leftpaddingvalueheaderlogodivtypepaddingcategorypositionnamemarginenabletopbottomschemaheader': '',
                'rightpaddingvalueheaderlogodivtypepaddingcategorypositionnamemarginenabletopbottomschemaheader': '',
                'toppaddingvalueheaderlogodivtypepaddingcategorypositionnamemarginenabletopbottomschemaheader': '172',
                'bottompaddingvalueheaderlogodivtypepaddingcategorypositionnamemarginenabletopbottomschemaheader': '101',
                'colorh2typecolorcategorycolornamebordercolorcssbordertopcolorschemah2major': 'rgb(231, 234, 232)',
                'colorh2typecolorcategoryfontnamefontcolorcsscolor': '',
                'colorh3typecolorcategoryfontnamefontcolorcsscolor': '',
                'colorbannerh2typecolorcategoryfontnamefontcolorcsscolor': '',
                'sliderbannerh2typeslidercategoryfontnamefontsizecssfontsizemin18max100': '45',
                'marginvaluebannerh2typemargincategorypositionnamemargin': '',
                'leftmarginvaluebannerh2typemargincategorypositionnamemargin': '',
                'rightmarginvaluebannerh2typemargincategorypositionnamemargin': '',
                'topmarginvaluebannerh2typemargincategorypositionnamemargin': '',
                'bottommarginvaluebannerh2typemargincategorypositionnamemargin': '',
                'colorbannerbylinetypecolorcategoryfontnamefontcolorcsscolor': '',
                'sliderbannerbylinetypeslidercategoryfontnamefontsizecssfontsizemin18max100': '22',
                'marginvaluebannerbylinetypemargincategorypositionnamemargin': '',
                'leftmarginvaluebannerbylinetypemargincategorypositionnamemargin': '',
                'rightmarginvaluebannerbylinetypemargincategorypositionnamemargin': '',
                'topmarginvaluebannerbylinetypemargincategorypositionnamemargin': '',
                'bottommarginvaluebannerbylinetypemargincategorypositionnamemargin': '',
                'imageorpatternbannertypebackgroundcategorycolornamebackgroundcsscolor': '',
                'colorbannertypebackgroundcategorycolornamebackgroundcsscolor': 'rgba(21, 28, 23, 0.95)',
                'imageorpatternbannerwrappertypebackgroundcategorycolornamebackground': '',
                'colorbannerwrappertypebackgroundcategorycolornamebackground': '',
                'paddingvaluebannerwrappertypepaddingcategorypositionnamepaddingenabletopbottom': '',
                'leftpaddingvaluebannerwrappertypepaddingcategorypositionnamepaddingenabletopbottom': '',
                'rightpaddingvaluebannerwrappertypepaddingcategorypositionnamepaddingenabletopbottom': '',
                'toppaddingvaluebannerwrappertypepaddingcategorypositionnamepaddingenabletopbottom': '123',
                'bottompaddingvaluebannerwrappertypepaddingcategorypositionnamepaddingenabletopbottom': '125',
                'bordersizemainwrappertypebordercategorystylingnameborderenabletopbottom': '',
                'bordercolormainwrappertypebordercategorystylingnameborderenabletopbottom': '',
                'bordertypemainwrappertypebordercategorystylingnameborderenabletopbottom': 'solid',
                'leftbordersizemainwrappertypebordercategorystylingnameborderenabletopbottom': '',
                'leftbordercolormainwrappertypebordercategorystylingnameborderenabletopbottom': '',
                'leftbordertypemainwrappertypebordercategorystylingnameborderenabletopbottom': 'solid',
                'rightbordersizemainwrappertypebordercategorystylingnameborderenabletopbottom': '',
                'rightbordercolormainwrappertypebordercategorystylingnameborderenabletopbottom': '',
                'rightbordertypemainwrappertypebordercategorystylingnameborderenabletopbottom': 'solid',
                'topbordersizemainwrappertypebordercategorystylingnameborderenabletopbottom': '32',
                'topbordercolormainwrappertypebordercategorystylingnameborderenabletopbottom': 'rgb(227, 218, 168)',
                'topbordertypemainwrappertypebordercategorystylingnameborderenabletopbottom': 'solid',
                'bottombordersizemainwrappertypebordercategorystylingnameborderenabletopbottom': '10',
                'bottombordercolormainwrappertypebordercategorystylingnameborderenabletopbottom': 'rgb(193, 202, 197)',
                'bottombordertypemainwrappertypebordercategorystylingnameborderenabletopbottom': 'solid',
                'radiusvalueimageimageimgimagebeforetyperadiuscategorystylingnameradius': '8',
                'topleftradiusvalueimageimageimgimagebeforetyperadiuscategorystylingnameradius': '',
                'toprightradiusvalueimageimageimgimagebeforetyperadiuscategorystylingnameradius': '',
                'bottomleftradiusvalueimageimageimgimagebeforetyperadiuscategorystylingnameradius': '',
                'bottomrightradiusvalueimageimageimgimagebeforetyperadiuscategorystylingnameradius': '',
                'colorbuttontypecolorcategorycolornamecolorcsscolor': 'rgb(255, 255, 255)',
                'colorbuttontypecolorcategorycolornamebackgroundcssbackground': 'rgb(227, 218, 168)',
                'colorbuttontypecolorcategorycolornamebackgroundhovercssbackgroundschemabuttonhover': 'rgb(235, 227, 178)',
                'radiusvaluebuttontyperadiuscategorystylingnameradius': '7',
                'topleftradiusvaluebuttontyperadiuscategorystylingnameradius': '',
                'toprightradiusvaluebuttontyperadiuscategorystylingnameradius': '',
                'bottomleftradiusvaluebuttontyperadiuscategorystylingnameradius': '',
                'bottomrightradiusvaluebuttontyperadiuscategorystylingnameradius': '',
                'colorbuttonalttypecolorcategorycolornamecolorcsscolor': 'rgb(255, 255, 255)',
                'colorbuttonalttypecolorcategorycolornamebackgroundcssbackground': 'rgb(193, 202, 197)',
                'colorbuttonalttypecolorcategorycolornamebackgroundhovercssbackgroundschemabuttonalthover': 'rgb(204, 213, 208)'
            }
        },
        {
            name: 'Blue Alternative',
            color1: 'rgb(193, 202, 197)',
            color2: 'rgb(231, 234, 232)',
            color3: 'rgb(107, 119, 112)',
            color4: 'rgb(68, 187, 204)',
            color5: 'rgba(21, 28, 23, 0.95)',
            data: {
                'widebodytypewidecategorydimensionnamelayout': 'wide',
                'imageorpatternbodytypebackgroundcategorycolornamebackground': '',
                'colorbodytypebackgroundcategorycolornamebackground': '',
                'colorbodytypecolorcategoryfontnamefontcolormaincsscolorschemabodyh1h2h3h4h5h6h7navlia': 'rgb(51, 68, 51)',
                'colorbodytypecolorcategoryfontnamefontcolorsecondarycsscolorschemaulmetabyline': 'rgb(68, 187, 204)',
                'fontfamilybodytypegooglefontpickercategoryfontnamefontfamilycsscolorschemabodyh1h2h3h4h5h6h7bylinenavbutton': 'Alef',
                'fonttypebodytypegooglefontpickercategoryfontnamefontfamilycsscolorschemabodyh1h2h3h4h5h6h7bylinenavbutton': 'google',
                'fontweightbodytypegooglefontpickercategoryfontnamefontfamilycsscolorschemabodyh1h2h3h4h5h6h7bylinenavbutton': 'regular',
                'fontstylebodytypegooglefontpickercategoryfontnamefontfamilycsscolorschemabodyh1h2h3h4h5h6h7bylinenavbutton': '',
                'imageorpatternnavtypebackgroundcategorycolornamebackground': '',
                'colornavtypebackgroundcategorycolornamebackground': '',
                'bordersizenavtypebordercategorycolornameborder': '',
                'bordercolornavtypebordercategorycolornameborder': '',
                'bordertypenavtypebordercategorycolornameborder': 'solid',
                'leftbordersizenavtypebordercategorycolornameborder': '',
                'leftbordercolornavtypebordercategorycolornameborder': '',
                'leftbordertypenavtypebordercategorycolornameborder': 'solid',
                'rightbordersizenavtypebordercategorycolornameborder': '',
                'rightbordercolornavtypebordercategorycolornameborder': '',
                'rightbordertypenavtypebordercategorycolornameborder': 'solid',
                'topbordersizenavtypebordercategorycolornameborder': '',
                'topbordercolornavtypebordercategorycolornameborder': '',
                'topbordertypenavtypebordercategorycolornameborder': 'solid',
                'bottombordersizenavtypebordercategorycolornameborder': '1',
                'bottombordercolornavtypebordercategorycolornameborder': 'rgba(0, 0, 0, 0.05)',
                'bottombordertypenavtypebordercategorycolornameborder': 'solid',
                'colornavtypecolorcategorynavnamefontcolorcsscolorschemanavlia': 'rgb(107, 119, 112)',
                'colornavtypecolorcategorynavnamefontcolorhoverselectedcsscolorschemanavlihovera': 'rgb(255, 255, 255)',
                'colornavtypecolorcategorynavnamebackgroundcolorhovercssbackgroundcolorschemanavlihovera': 'rgb(193, 202, 197)',
                'colornavtypecolorcategorynavnamebackgroundcolorselectedcssbackgroundcolorschemanavlicurrentpageitema': 'rgb(68, 187, 204)',
                'fontfamilynavtypegooglefontpickercategoryfontnamefontfamilly': '',
                'fonttypenavtypegooglefontpickercategoryfontnamefontfamilly': '',
                'fontweightnavtypegooglefontpickercategoryfontnamefontfamilly': '',
                'fontstylenavtypegooglefontpickercategoryfontnamefontfamilly': '',
                'colorheaderlogodivtypecolorcategorycolornamebordercolorcssbordertopcolorschemaheaderlogo': 'rgb(231, 234, 232)',
                'paddingvalueheaderlogodivtypepaddingcategorypositionnamemarginenabletopbottomschemaheader': '',
                'leftpaddingvalueheaderlogodivtypepaddingcategorypositionnamemarginenabletopbottomschemaheader': '',
                'rightpaddingvalueheaderlogodivtypepaddingcategorypositionnamemarginenabletopbottomschemaheader': '',
                'toppaddingvalueheaderlogodivtypepaddingcategorypositionnamemarginenabletopbottomschemaheader': '166',
                'bottompaddingvalueheaderlogodivtypepaddingcategorypositionnamemarginenabletopbottomschemaheader': '91',
                'colorh2typecolorcategorycolornamebordercolorcssbordertopcolorschemah2major': 'rgb(231, 234, 232)',
                'colorh2typecolorcategoryfontnamefontcolorcsscolor': '',
                'colorh3typecolorcategoryfontnamefontcolorcsscolor': '',
                'colorbannerh2typecolorcategoryfontnamefontcolorcsscolor': '',
                'sliderbannerh2typeslidercategoryfontnamefontsizecssfontsizemin18max100': '45',
                'marginvaluebannerh2typemargincategorypositionnamemargin': '',
                'leftmarginvaluebannerh2typemargincategorypositionnamemargin': '',
                'rightmarginvaluebannerh2typemargincategorypositionnamemargin': '',
                'topmarginvaluebannerh2typemargincategorypositionnamemargin': '',
                'bottommarginvaluebannerh2typemargincategorypositionnamemargin': '',
                'colorbannerbylinetypecolorcategoryfontnamefontcolorcsscolor': '',
                'sliderbannerbylinetypeslidercategoryfontnamefontsizecssfontsizemin18max100': '22',
                'marginvaluebannerbylinetypemargincategorypositionnamemargin': '',
                'leftmarginvaluebannerbylinetypemargincategorypositionnamemargin': '',
                'rightmarginvaluebannerbylinetypemargincategorypositionnamemargin': '',
                'topmarginvaluebannerbylinetypemargincategorypositionnamemargin': '',
                'bottommarginvaluebannerbylinetypemargincategorypositionnamemargin': '',
                'imageorpatternbannertypebackgroundcategorycolornamebackgroundcsscolor': '',
                'colorbannertypebackgroundcategorycolornamebackgroundcsscolor': 'rgba(21, 28, 23, 0.95)',
                'imageorpatternbannerwrappertypebackgroundcategorycolornamebackground': '',
                'colorbannerwrappertypebackgroundcategorycolornamebackground': '',
                'paddingvaluebannerwrappertypepaddingcategorypositionnamepaddingenabletopbottom': '',
                'leftpaddingvaluebannerwrappertypepaddingcategorypositionnamepaddingenabletopbottom': '',
                'rightpaddingvaluebannerwrappertypepaddingcategorypositionnamepaddingenabletopbottom': '',
                'toppaddingvaluebannerwrappertypepaddingcategorypositionnamepaddingenabletopbottom': '48',
                'bottompaddingvaluebannerwrappertypepaddingcategorypositionnamepaddingenabletopbottom': '55',
                'bordersizemainwrappertypebordercategorystylingnameborderenabletopbottom': '',
                'bordercolormainwrappertypebordercategorystylingnameborderenabletopbottom': '',
                'bordertypemainwrappertypebordercategorystylingnameborderenabletopbottom': 'solid',
                'leftbordersizemainwrappertypebordercategorystylingnameborderenabletopbottom': '',
                'leftbordercolormainwrappertypebordercategorystylingnameborderenabletopbottom': '',
                'leftbordertypemainwrappertypebordercategorystylingnameborderenabletopbottom': 'solid',
                'rightbordersizemainwrappertypebordercategorystylingnameborderenabletopbottom': '',
                'rightbordercolormainwrappertypebordercategorystylingnameborderenabletopbottom': '',
                'rightbordertypemainwrappertypebordercategorystylingnameborderenabletopbottom': 'solid',
                'topbordersizemainwrappertypebordercategorystylingnameborderenabletopbottom': '10',
                'topbordercolormainwrappertypebordercategorystylingnameborderenabletopbottom': 'rgb(68, 187, 204)',
                'topbordertypemainwrappertypebordercategorystylingnameborderenabletopbottom': 'solid',
                'bottombordersizemainwrappertypebordercategorystylingnameborderenabletopbottom': '10',
                'bottombordercolormainwrappertypebordercategorystylingnameborderenabletopbottom': 'rgb(193, 202, 197)',
                'bottombordertypemainwrappertypebordercategorystylingnameborderenabletopbottom': 'solid',
                'radiusvalueimageimageimgimagebeforetyperadiuscategorystylingnameradius': '0',
                'topleftradiusvalueimageimageimgimagebeforetyperadiuscategorystylingnameradius': '20',
                'toprightradiusvalueimageimageimgimagebeforetyperadiuscategorystylingnameradius': '',
                'bottomleftradiusvalueimageimageimgimagebeforetyperadiuscategorystylingnameradius': '',
                'bottomrightradiusvalueimageimageimgimagebeforetyperadiuscategorystylingnameradius': '20',
                'colorbuttontypecolorcategorycolornamecolorcsscolor': 'rgb(255, 255, 255)',
                'colorbuttontypecolorcategorycolornamebackgroundcssbackground': 'rgb(68, 187, 204)',
                'colorbuttontypecolorcategorycolornamebackgroundhovercssbackgroundschemabuttonhover': 'rgb(133, 220, 232)',
                'radiusvaluebuttontyperadiuscategorystylingnameradius': '7',
                'topleftradiusvaluebuttontyperadiuscategorystylingnameradius': '',
                'toprightradiusvaluebuttontyperadiuscategorystylingnameradius': '',
                'bottomleftradiusvaluebuttontyperadiuscategorystylingnameradius': '',
                'bottomrightradiusvaluebuttontyperadiuscategorystylingnameradius': '',
                'colorbuttonalttypecolorcategorycolornamecolorcsscolor': 'rgb(255, 255, 255)',
                'colorbuttonalttypecolorcategorycolornamebackgroundcssbackground': 'rgb(193, 202, 197)',
                'colorbuttonalttypecolorcategorycolornamebackgroundhovercssbackgroundschemabuttonalthover': 'rgb(204, 213, 208)'
            }
        },
        {
            name: 'Green safe',
            color1: 'rgb(193, 202, 197)',
            color2: 'rgb(240, 240, 240)',
            color3: 'rgb(0, 153, 0)',
            color4: 'rgb(0, 51, 0)',
            color5: 'rgb(51, 51, 51)',
            data: {
                'widebodytypewidecategorydimensionnamelayout': 'box',
                'imageorpatternbodytypebackgroundcategorycolornamebackground': '',
                'colorbodytypebackgroundcategorycolornamebackground': 'rgb(240, 240, 240)',
                'colorbodytypecolorcategoryfontnamefontcolormaincsscolorschemabodyh1h2h3h4h5h6h7navlia': 'rgb(85, 85, 85)',
                'colorbodytypecolorcategoryfontnamefontcolorsecondarycsscolorschemaulmetabyline': 'rgb(0, 153, 0)',
                'fontfamilybodytypegooglefontpickercategoryfontnamefontfamilycsscolorschemabodyh1h2h3h4h5h6h7bylinenavbutton': 'Karma',
                'fonttypebodytypegooglefontpickercategoryfontnamefontfamilycsscolorschemabodyh1h2h3h4h5h6h7bylinenavbutton': 'google',
                'fontweightbodytypegooglefontpickercategoryfontnamefontfamilycsscolorschemabodyh1h2h3h4h5h6h7bylinenavbutton': '300',
                'fontstylebodytypegooglefontpickercategoryfontnamefontfamilycsscolorschemabodyh1h2h3h4h5h6h7bylinenavbutton': '',
                'imageorpatternnavtypebackgroundcategorycolornamebackground': '',
                'colornavtypebackgroundcategorycolornamebackground': 'rgb(0, 51, 0)',
                'bordersizenavtypebordercategorycolornameborder': '',
                'bordercolornavtypebordercategorycolornameborder': '',
                'bordertypenavtypebordercategorycolornameborder': 'solid',
                'leftbordersizenavtypebordercategorycolornameborder': '',
                'leftbordercolornavtypebordercategorycolornameborder': '',
                'leftbordertypenavtypebordercategorycolornameborder': 'solid',
                'rightbordersizenavtypebordercategorycolornameborder': '',
                'rightbordercolornavtypebordercategorycolornameborder': '',
                'rightbordertypenavtypebordercategorycolornameborder': 'solid',
                'topbordersizenavtypebordercategorycolornameborder': '',
                'topbordercolornavtypebordercategorycolornameborder': '',
                'topbordertypenavtypebordercategorycolornameborder': 'solid',
                'bottombordersizenavtypebordercategorycolornameborder': '1',
                'bottombordercolornavtypebordercategorycolornameborder': 'rgba(0, 0, 0, 0.05)',
                'bottombordertypenavtypebordercategorycolornameborder': 'solid',
                'colornavtypecolorcategorynavnamefontcolorcsscolorschemanavlia': 'rgb(187, 187, 187)',
                'colornavtypecolorcategorynavnamefontcolorhoverselectedcsscolorschemanavlihovera': 'rgb(255, 255, 255)',
                'colornavtypecolorcategorynavnamebackgroundcolorhovercssbackgroundcolorschemanavlihovera': 'rgb(0, 153, 0)',
                'colornavtypecolorcategorynavnamebackgroundcolorselectedcssbackgroundcolorschemanavlicurrentpageitema': 'rgb(0, 153, 0)',
                'fontfamilynavtypegooglefontpickercategoryfontnamefontfamilly': '',
                'fonttypenavtypegooglefontpickercategoryfontnamefontfamilly': '',
                'fontweightnavtypegooglefontpickercategoryfontnamefontfamilly': '',
                'fontstylenavtypegooglefontpickercategoryfontnamefontfamilly': '',
                'colorheaderlogodivtypecolorcategorycolornamebordercolorcssbordertopcolorschemaheaderlogo': 'rgb(231, 234, 232)',
                'paddingvalueheaderlogodivtypepaddingcategorypositionnamemarginenabletopbottomschemaheader': '',
                'leftpaddingvalueheaderlogodivtypepaddingcategorypositionnamemarginenabletopbottomschemaheader': '',
                'rightpaddingvalueheaderlogodivtypepaddingcategorypositionnamemarginenabletopbottomschemaheader': '',
                'toppaddingvalueheaderlogodivtypepaddingcategorypositionnamemarginenabletopbottomschemaheader': '151',
                'bottompaddingvalueheaderlogodivtypepaddingcategorypositionnamemarginenabletopbottomschemaheader': '57',
                'colorh2typecolorcategorycolornamebordercolorcssbordertopcolorschemah2major': 'rgb(231, 234, 232)',
                'colorh2typecolorcategoryfontnamefontcolorcsscolor': '',
                'colorh3typecolorcategoryfontnamefontcolorcsscolor': '',
                'colorbannerh2typecolorcategoryfontnamefontcolorcsscolor': 'rgb(0, 153, 0)',
                'sliderbannerh2typeslidercategoryfontnamefontsizecssfontsizemin18max100': '54',
                'marginvaluebannerh2typemargincategorypositionnamemargin': '',
                'leftmarginvaluebannerh2typemargincategorypositionnamemargin': '',
                'rightmarginvaluebannerh2typemargincategorypositionnamemargin': '',
                'topmarginvaluebannerh2typemargincategorypositionnamemargin': '33',
                'bottommarginvaluebannerh2typemargincategorypositionnamemargin': '',
                'colorbannerbylinetypecolorcategoryfontnamefontcolorcsscolor': 'rgb(255, 255, 255)',
                'sliderbannerbylinetypeslidercategoryfontnamefontsizecssfontsizemin18max100': '26',
                'marginvaluebannerbylinetypemargincategorypositionnamemargin': '',
                'leftmarginvaluebannerbylinetypemargincategorypositionnamemargin': '',
                'rightmarginvaluebannerbylinetypemargincategorypositionnamemargin': '',
                'topmarginvaluebannerbylinetypemargincategorypositionnamemargin': '',
                'bottommarginvaluebannerbylinetypemargincategorypositionnamemargin': '30',
                'imageorpatternbannertypebackgroundcategorycolornamebackgroundcsscolor': '',
                'colorbannertypebackgroundcategorycolornamebackgroundcsscolor': 'rgb(51, 51, 51)',
                'imageorpatternbannerwrappertypebackgroundcategorycolornamebackground': '',
                'colorbannerwrappertypebackgroundcategorycolornamebackground': 'rgba(0, 153, 0, 0.15)',
                'paddingvaluebannerwrappertypepaddingcategorypositionnamepaddingenabletopbottom': '',
                'leftpaddingvaluebannerwrappertypepaddingcategorypositionnamepaddingenabletopbottom': '',
                'rightpaddingvaluebannerwrappertypepaddingcategorypositionnamepaddingenabletopbottom': '',
                'toppaddingvaluebannerwrappertypepaddingcategorypositionnamepaddingenabletopbottom': '21',
                'bottompaddingvaluebannerwrappertypepaddingcategorypositionnamepaddingenabletopbottom': '21',
                'bordersizemainwrappertypebordercategorystylingnameborderenabletopbottom': '',
                'bordercolormainwrappertypebordercategorystylingnameborderenabletopbottom': '',
                'bordertypemainwrappertypebordercategorystylingnameborderenabletopbottom': 'solid',
                'leftbordersizemainwrappertypebordercategorystylingnameborderenabletopbottom': '',
                'leftbordercolormainwrappertypebordercategorystylingnameborderenabletopbottom': '',
                'leftbordertypemainwrappertypebordercategorystylingnameborderenabletopbottom': 'solid',
                'rightbordersizemainwrappertypebordercategorystylingnameborderenabletopbottom': '',
                'rightbordercolormainwrappertypebordercategorystylingnameborderenabletopbottom': '',
                'rightbordertypemainwrappertypebordercategorystylingnameborderenabletopbottom': 'solid',
                'topbordersizemainwrappertypebordercategorystylingnameborderenabletopbottom': '1',
                'topbordercolormainwrappertypebordercategorystylingnameborderenabletopbottom': 'rgba(68, 187, 204, 0)',
                'topbordertypemainwrappertypebordercategorystylingnameborderenabletopbottom': 'solid',
                'bottombordersizemainwrappertypebordercategorystylingnameborderenabletopbottom': '10',
                'bottombordercolormainwrappertypebordercategorystylingnameborderenabletopbottom': 'rgb(193, 202, 197)',
                'bottombordertypemainwrappertypebordercategorystylingnameborderenabletopbottom': 'solid',
                'radiusvalueimageimageimgimagebeforetyperadiuscategorystylingnameradius': '8',
                'topleftradiusvalueimageimageimgimagebeforetyperadiuscategorystylingnameradius': '0',
                'toprightradiusvalueimageimageimgimagebeforetyperadiuscategorystylingnameradius': '0',
                'bottomleftradiusvalueimageimageimgimagebeforetyperadiuscategorystylingnameradius': '0',
                'bottomrightradiusvalueimageimageimgimagebeforetyperadiuscategorystylingnameradius': '0',
                'colorbuttontypecolorcategorycolornamecolorcsscolor': 'rgb(255, 255, 255)',
                'colorbuttontypecolorcategorycolornamebackgroundcssbackground': 'rgb(0, 51, 0)',
                'colorbuttontypecolorcategorycolornamebackgroundhovercssbackgroundschemabuttonhover': 'rgba(0, 51, 0, 0.62)',
                'radiusvaluebuttontyperadiuscategorystylingnameradius': '7',
                'topleftradiusvaluebuttontyperadiuscategorystylingnameradius': '',
                'toprightradiusvaluebuttontyperadiuscategorystylingnameradius': '',
                'bottomleftradiusvaluebuttontyperadiuscategorystylingnameradius': '',
                'bottomrightradiusvaluebuttontyperadiuscategorystylingnameradius': '',
                'colorbuttonalttypecolorcategorycolornamecolorcsscolor': 'rgb(255, 255, 255)',
                'colorbuttonalttypecolorcategorycolornamebackgroundcssbackground': 'rgb(193, 202, 197)',
                'colorbuttonalttypecolorcategorycolornamebackgroundhovercssbackgroundschemabuttonalthover': 'rgb(204, 213, 208)'
            }
        },
        {
            name: 'Orange',
            color1: 'rgb(193, 202, 197)',
            color2: 'rgb(231, 234, 232)',
            color3: 'rgb(230, 126, 34)',
            color4: 'rgb(211, 84, 0)',
            color5: 'rgb(51, 51, 51)',
            data: {
                'widebodytypewidecategorydimensionnamelayout': 'wide',
                'imageorpatternbodytypebackgroundcategorycolornamebackground': '',
                'colorbodytypebackgroundcategorycolornamebackground': 'rgb(240, 240, 240)',
                'colorbodytypecolorcategoryfontnamefontcolormaincsscolorschemabodyh1h2h3h4h5h6h7navlia': 'rgb(85, 85, 85)',
                'colorbodytypecolorcategoryfontnamefontcolorsecondarycsscolorschemaulmetabyline': 'rgb(230, 126, 34)',
                'fontfamilybodytypegooglefontpickercategoryfontnamefontfamilycsscolorschemabodyh1h2h3h4h5h6h7bylinenavbutton': 'Lato',
                'fonttypebodytypegooglefontpickercategoryfontnamefontfamilycsscolorschemabodyh1h2h3h4h5h6h7bylinenavbutton': 'google',
                'fontweightbodytypegooglefontpickercategoryfontnamefontfamilycsscolorschemabodyh1h2h3h4h5h6h7bylinenavbutton': '300',
                'fontstylebodytypegooglefontpickercategoryfontnamefontfamilycsscolorschemabodyh1h2h3h4h5h6h7bylinenavbutton': '',
                'imageorpatternnavtypebackgroundcategorycolornamebackground': '',
                'colornavtypebackgroundcategorycolornamebackground': 'rgb(51, 51, 51)',
                'bordersizenavtypebordercategorycolornameborder': '',
                'bordercolornavtypebordercategorycolornameborder': '',
                'bordertypenavtypebordercategorycolornameborder': 'solid',
                'leftbordersizenavtypebordercategorycolornameborder': '',
                'leftbordercolornavtypebordercategorycolornameborder': '',
                'leftbordertypenavtypebordercategorycolornameborder': 'solid',
                'rightbordersizenavtypebordercategorycolornameborder': '',
                'rightbordercolornavtypebordercategorycolornameborder': '',
                'rightbordertypenavtypebordercategorycolornameborder': 'solid',
                'topbordersizenavtypebordercategorycolornameborder': '',
                'topbordercolornavtypebordercategorycolornameborder': '',
                'topbordertypenavtypebordercategorycolornameborder': 'solid',
                'bottombordersizenavtypebordercategorycolornameborder': '1',
                'bottombordercolornavtypebordercategorycolornameborder': 'rgba(0, 0, 0, 0.05)',
                'bottombordertypenavtypebordercategorycolornameborder': 'solid',
                'colornavtypecolorcategorynavnamefontcolorcsscolorschemanavlia': 'rgb(187, 187, 187)',
                'colornavtypecolorcategorynavnamefontcolorhoverselectedcsscolorschemanavlihovera': 'rgb(255, 255, 255)',
                'colornavtypecolorcategorynavnamebackgroundcolorhovercssbackgroundcolorschemanavlihovera': 'rgb(181, 181, 181)',
                'colornavtypecolorcategorynavnamebackgroundcolorselectedcssbackgroundcolorschemanavlicurrentpageitema': 'rgb(211, 84, 0)',
                'fontfamilynavtypegooglefontpickercategoryfontnamefontfamilly': '',
                'fonttypenavtypegooglefontpickercategoryfontnamefontfamilly': '',
                'fontweightnavtypegooglefontpickercategoryfontnamefontfamilly': '',
                'fontstylenavtypegooglefontpickercategoryfontnamefontfamilly': '',
                'colorheaderlogodivtypecolorcategorycolornamebordercolorcssbordertopcolorschemaheaderlogo': 'rgb(231, 234, 232)',
                'paddingvalueheaderlogodivtypepaddingcategorypositionnamemarginenabletopbottomschemaheader': '',
                'leftpaddingvalueheaderlogodivtypepaddingcategorypositionnamemarginenabletopbottomschemaheader': '',
                'rightpaddingvalueheaderlogodivtypepaddingcategorypositionnamemarginenabletopbottomschemaheader': '',
                'toppaddingvalueheaderlogodivtypepaddingcategorypositionnamemarginenabletopbottomschemaheader': '151',
                'bottompaddingvalueheaderlogodivtypepaddingcategorypositionnamemarginenabletopbottomschemaheader': '57',
                'colorh2typecolorcategorycolornamebordercolorcssbordertopcolorschemah2major': 'rgb(231, 234, 232)',
                'colorh2typecolorcategoryfontnamefontcolorcsscolor': '',
                'colorh3typecolorcategoryfontnamefontcolorcsscolor': '',
                'colorbannerh2typecolorcategoryfontnamefontcolorcsscolor': '',
                'sliderbannerh2typeslidercategoryfontnamefontsizecssfontsizemin18max100': '54',
                'marginvaluebannerh2typemargincategorypositionnamemargin': '',
                'leftmarginvaluebannerh2typemargincategorypositionnamemargin': '',
                'rightmarginvaluebannerh2typemargincategorypositionnamemargin': '',
                'topmarginvaluebannerh2typemargincategorypositionnamemargin': '33',
                'bottommarginvaluebannerh2typemargincategorypositionnamemargin': '',
                'colorbannerbylinetypecolorcategoryfontnamefontcolorcsscolor': 'rgb(225, 225, 225)',
                'sliderbannerbylinetypeslidercategoryfontnamefontsizecssfontsizemin18max100': '26',
                'marginvaluebannerbylinetypemargincategorypositionnamemargin': '',
                'leftmarginvaluebannerbylinetypemargincategorypositionnamemargin': '',
                'rightmarginvaluebannerbylinetypemargincategorypositionnamemargin': '',
                'topmarginvaluebannerbylinetypemargincategorypositionnamemargin': '',
                'bottommarginvaluebannerbylinetypemargincategorypositionnamemargin': '30',
                'imageorpatternbannertypebackgroundcategorycolornamebackgroundcsscolor': '',
                'colorbannertypebackgroundcategorycolornamebackgroundcsscolor': 'rgb(230, 126, 34)',
                'imageorpatternbannerwrappertypebackgroundcategorycolornamebackground': '',
                'colorbannerwrappertypebackgroundcategorycolornamebackground': 'rgb(255, 255, 255)',
                'paddingvaluebannerwrappertypepaddingcategorypositionnamepaddingenabletopbottom': '',
                'leftpaddingvaluebannerwrappertypepaddingcategorypositionnamepaddingenabletopbottom': '',
                'rightpaddingvaluebannerwrappertypepaddingcategorypositionnamepaddingenabletopbottom': '',
                'toppaddingvaluebannerwrappertypepaddingcategorypositionnamepaddingenabletopbottom': '21',
                'bottompaddingvaluebannerwrappertypepaddingcategorypositionnamepaddingenabletopbottom': '21',
                'bordersizemainwrappertypebordercategorystylingnameborderenabletopbottom': '',
                'bordercolormainwrappertypebordercategorystylingnameborderenabletopbottom': '',
                'bordertypemainwrappertypebordercategorystylingnameborderenabletopbottom': 'solid',
                'leftbordersizemainwrappertypebordercategorystylingnameborderenabletopbottom': '',
                'leftbordercolormainwrappertypebordercategorystylingnameborderenabletopbottom': '',
                'leftbordertypemainwrappertypebordercategorystylingnameborderenabletopbottom': 'solid',
                'rightbordersizemainwrappertypebordercategorystylingnameborderenabletopbottom': '',
                'rightbordercolormainwrappertypebordercategorystylingnameborderenabletopbottom': '',
                'rightbordertypemainwrappertypebordercategorystylingnameborderenabletopbottom': 'solid',
                'topbordersizemainwrappertypebordercategorystylingnameborderenabletopbottom': '1',
                'topbordercolormainwrappertypebordercategorystylingnameborderenabletopbottom': 'rgba(68, 187, 204, 0)',
                'topbordertypemainwrappertypebordercategorystylingnameborderenabletopbottom': 'solid',
                'bottombordersizemainwrappertypebordercategorystylingnameborderenabletopbottom': '10',
                'bottombordercolormainwrappertypebordercategorystylingnameborderenabletopbottom': 'rgb(193, 202, 197)',
                'bottombordertypemainwrappertypebordercategorystylingnameborderenabletopbottom': 'solid',
                'radiusvalueimageimageimgimagebeforetyperadiuscategorystylingnameradius': '8',
                'topleftradiusvalueimageimageimgimagebeforetyperadiuscategorystylingnameradius': '0',
                'toprightradiusvalueimageimageimgimagebeforetyperadiuscategorystylingnameradius': '0',
                'bottomleftradiusvalueimageimageimgimagebeforetyperadiuscategorystylingnameradius': '0',
                'bottomrightradiusvalueimageimageimgimagebeforetyperadiuscategorystylingnameradius': '0',
                'colorbuttontypecolorcategorycolornamecolorcsscolor': 'rgb(255, 255, 255)',
                'colorbuttontypecolorcategorycolornamebackgroundcssbackground': 'rgb(230, 126, 34)',
                'colorbuttontypecolorcategorycolornamebackgroundhovercssbackgroundschemabuttonhover': 'rgba(230, 126, 34, 0.55)',
                'radiusvaluebuttontyperadiuscategorystylingnameradius': '7',
                'topleftradiusvaluebuttontyperadiuscategorystylingnameradius': '',
                'toprightradiusvaluebuttontyperadiuscategorystylingnameradius': '',
                'bottomleftradiusvaluebuttontyperadiuscategorystylingnameradius': '',
                'bottomrightradiusvaluebuttontyperadiuscategorystylingnameradius': '',
                'colorbuttonalttypecolorcategorycolornamecolorcsscolor': 'rgb(255, 255, 255)',
                'colorbuttonalttypecolorcategorycolornamebackgroundcssbackground': 'rgb(193, 202, 197)',
                'colorbuttonalttypecolorcategorycolornamebackgroundhovercssbackgroundschemabuttonalthover': 'rgb(204, 213, 208)'
            }
        }
    ];
    /*********************************************************************************************************/
    /* Background editor */
    /*********************************************************************************************************/
    angular.module('Umbraco.canvasdesigner').controller('Umbraco.canvasdesigner.background', function ($scope, dialogService) {
        if (!$scope.item.values) {
            $scope.item.values = {
                imageorpattern: '',
                color: ''
            };
        }
        $scope.open = function (field) {
            var config = {
                template: 'mediaPickerModal.html',
                change: function (data) {
                    $scope.item.values.imageorpattern = data;
                },
                callback: function (data) {
                    $scope.item.values.imageorpattern = data;
                },
                cancel: function (data) {
                    $scope.item.values.imageorpattern = data;
                },
                dialogData: $scope.googleFontFamilies,
                dialogItem: $scope.item.values.imageorpattern
            };
            dialogService.open(config);
        };
    }).controller('canvasdesigner.mediaPickerModal', function ($scope, $http, mediaResource, umbRequestHelper, entityResource, mediaHelper) {
        if (mediaHelper && mediaHelper.registerFileResolver) {
            mediaHelper.registerFileResolver('Umbraco.UploadField', function (property, entity, thumbnail) {
                if (thumbnail) {
                    if (mediaHelper.detectIfImageByExtension(property.value)) {
                        var thumbnailUrl = umbRequestHelper.getApiUrl('imagesApiBaseUrl', 'GetBigThumbnail', [{ originalImagePath: property.value }]);
                        return thumbnailUrl;
                    } else {
                        return null;
                    }
                } else {
                    return property.value;
                }
            });
        }
        var modalFieldvalue = $scope.dialogItem;
        $scope.currentFolder = {};
        $scope.currentFolder.children = [];
        $scope.currentPath = [];
        $scope.startNodeId = -1;
        $scope.options = {
            url: umbRequestHelper.getApiUrl('mediaApiBaseUrl', 'PostAddFile'),
            formData: { currentFolder: $scope.startNodeId }
        };
        //preload selected item
        $scope.selectedMedia = undefined;
        $scope.submitFolder = function (e) {
            if (e.keyCode === 13) {
                e.preventDefault();
                $scope.$parent.data.showFolderInput = false;
                if ($scope.$parent.data.newFolder && $scope.$parent.data.newFolder != '') {
                    mediaResource.addFolder($scope.$parent.data.newFolder, $scope.currentFolder.id).then(function (data) {
                        $scope.$parent.data.newFolder = undefined;
                        $scope.gotoFolder(data);
                    });
                }
            }
        };
        $scope.gotoFolder = function (folder) {
            if (!folder) {
                folder = {
                    id: $scope.startNodeId,
                    name: 'Media',
                    icon: 'icon-folder'
                };
            }
            if (folder.id > 0) {
                var matches = _.filter($scope.currentPath, function (value, index) {
                    if (value.id == folder.id) {
                        value.indexInPath = index;
                        return value;
                    }
                });
                if (matches && matches.length > 0) {
                    $scope.currentPath = $scope.currentPath.slice(0, matches[0].indexInPath + 1);
                } else {
                    $scope.currentPath.push(folder);
                }
            } else {
                $scope.currentPath = [];
            }
            //mediaResource.rootMedia()
            mediaResource.getChildren(folder.id).then(function (data) {
                folder.children = data.items ? data.items : [];
                angular.forEach(folder.children, function (child) {
                    child.isFolder = child.contentTypeAlias == 'Folder' ? true : false;
                    if (!child.isFolder) {
                        angular.forEach(child.properties, function (property) {
                            if (property.alias == 'umbracoFile' && property.value) {
                                child.thumbnail = mediaHelper.resolveFile(child, true);
                                child.image = property.value;
                            }
                        });
                    }
                });
                $scope.options.formData.currentFolder = folder.id;
                $scope.currentFolder = folder;
            });
        };
        $scope.iconFolder = 'glyphicons-icon folder-open';
        $scope.selectMedia = function (media) {
            if (!media.isFolder) {
                //we have 3 options add to collection (if multi) show details, or submit it right back to the callback
                $scope.selectedMedia = media;
                modalFieldvalue = 'url(' + $scope.selectedMedia.image + ')';
                $scope.change(modalFieldvalue);
            } else {
                $scope.gotoFolder(media);
            }
        };
        //default root item
        if (!$scope.selectedMedia) {
            $scope.gotoFolder();
        }
        $scope.submitAndClose = function () {
            if (modalFieldvalue != '') {
                $scope.submit(modalFieldvalue);
            } else {
                $scope.cancel();
            }
        };
        $scope.cancelAndClose = function () {
            $scope.cancel();
        };
    });
    /*********************************************************************************************************/
    /* Background editor */
    /*********************************************************************************************************/
    angular.module('Umbraco.canvasdesigner').controller('Umbraco.canvasdesigner.border', function ($scope, dialogService) {
        $scope.defaultBorderList = [
            'all',
            'left',
            'right',
            'top',
            'bottom'
        ];
        $scope.borderList = [];
        $scope.bordertypes = [
            'solid',
            'dashed',
            'dotted'
        ];
        $scope.selectedBorder = {
            name: 'all',
            size: 0,
            color: '',
            type: ''
        };
        $scope.setselectedBorder = function (bordertype) {
            if (bordertype == 'all') {
                $scope.selectedBorder.name = 'all';
                $scope.selectedBorder.size = $scope.item.values.bordersize;
                $scope.selectedBorder.color = $scope.item.values.bordercolor;
                $scope.selectedBorder.type = $scope.item.values.bordertype;
            }
            if (bordertype == 'left') {
                $scope.selectedBorder.name = 'left';
                $scope.selectedBorder.size = $scope.item.values.leftbordersize;
                $scope.selectedBorder.color = $scope.item.values.leftbordercolor;
                $scope.selectedBorder.type = $scope.item.values.leftbordertype;
            }
            if (bordertype == 'right') {
                $scope.selectedBorder.name = 'right';
                $scope.selectedBorder.size = $scope.item.values.rightbordersize;
                $scope.selectedBorder.color = $scope.item.values.rightbordercolor;
                $scope.selectedBorder.type = $scope.item.values.rightbordertype;
            }
            if (bordertype == 'top') {
                $scope.selectedBorder.name = 'top';
                $scope.selectedBorder.size = $scope.item.values.topbordersize;
                $scope.selectedBorder.color = $scope.item.values.topbordercolor;
                $scope.selectedBorder.type = $scope.item.values.topbordertype;
            }
            if (bordertype == 'bottom') {
                $scope.selectedBorder.name = 'bottom';
                $scope.selectedBorder.size = $scope.item.values.bottombordersize;
                $scope.selectedBorder.color = $scope.item.values.bottombordercolor;
                $scope.selectedBorder.type = $scope.item.values.bottombordertype;
            }
        };
        if (!$scope.item.values) {
            $scope.item.values = {
                bordersize: '',
                bordercolor: '',
                bordertype: 'solid',
                leftbordersize: '',
                leftbordercolor: '',
                leftbordertype: 'solid',
                rightbordersize: '',
                rightbordercolor: '',
                rightbordertype: 'solid',
                topbordersize: '',
                topbordercolor: '',
                topbordertype: 'solid',
                bottombordersize: '',
                bottombordercolor: '',
                bottombordertype: 'solid'
            };
        }
        if ($scope.item.enable) {
            angular.forEach($scope.defaultBorderList, function (key, indexKey) {
                if ($.inArray(key, $scope.item.enable) >= 0) {
                    $scope.borderList.splice($scope.borderList.length + 1, 0, key);
                }
            });
        } else {
            $scope.borderList = $scope.defaultBorderList;
        }
        $scope.$watch('valueAreLoaded', function () {
            $scope.setselectedBorder($scope.borderList[0]);
        }, false);
        $scope.$watch('selectedBorder', function () {
            if ($scope.selectedBorder.name == 'all') {
                $scope.item.values.bordersize = $scope.selectedBorder.size;
                $scope.item.values.bordertype = $scope.selectedBorder.type;
            }
            if ($scope.selectedBorder.name == 'left') {
                $scope.item.values.leftbordersize = $scope.selectedBorder.size;
                $scope.item.values.leftbordertype = $scope.selectedBorder.type;
            }
            if ($scope.selectedBorder.name == 'right') {
                $scope.item.values.rightbordersize = $scope.selectedBorder.size;
                $scope.item.values.rightbordertype = $scope.selectedBorder.type;
            }
            if ($scope.selectedBorder.name == 'top') {
                $scope.item.values.topbordersize = $scope.selectedBorder.size;
                $scope.item.values.topbordertype = $scope.selectedBorder.type;
            }
            if ($scope.selectedBorder.name == 'bottom') {
                $scope.item.values.bottombordersize = $scope.selectedBorder.size;
                $scope.item.values.bottombordertype = $scope.selectedBorder.type;
            }
        }, true);
    });
    /*********************************************************************************************************/
    /* color editor */
    /*********************************************************************************************************/
    angular.module('Umbraco.canvasdesigner').controller('Umbraco.canvasdesigner.color', function ($scope) {
        if (!$scope.item.values) {
            $scope.item.values = { color: '' };
        }
    });
    /*********************************************************************************************************/
    /* google font editor */
    /*********************************************************************************************************/
    angular.module('Umbraco.canvasdesigner').controller('Umbraco.canvasdesigner.googlefontpicker', function ($scope, dialogService) {
        if (!$scope.item.values) {
            $scope.item.values = {
                fontFamily: '',
                fontType: '',
                fontWeight: '',
                fontStyle: ''
            };
        }
        $scope.setStyleVariant = function () {
            if ($scope.item.values != undefined) {
                return {
                    'font-family': $scope.item.values.fontFamily,
                    'font-weight': $scope.item.values.fontWeight,
                    'font-style': $scope.item.values.fontStyle
                };
            }
        };
        $scope.open = function (field) {
            var config = {
                template: 'googlefontdialog.html',
                change: function (data) {
                    $scope.item.values = data;
                },
                callback: function (data) {
                    $scope.item.values = data;
                },
                cancel: function (data) {
                    $scope.item.values = data;
                },
                dialogData: $scope.googleFontFamilies,
                dialogItem: $scope.item.values
            };
            dialogService.open(config);
        };
    }).controller('googlefontdialog.controller', function ($scope) {
        $scope.safeFonts = [
            'Arial, Helvetica',
            'Impact',
            'Lucida Sans Unicode',
            'Tahoma',
            'Trebuchet MS',
            'Verdana',
            'Georgia',
            'Times New Roman',
            'Courier New, Courier'
        ];
        $scope.fonts = [];
        $scope.selectedFont = {};
        var googleGetWeight = function (googleVariant) {
            return googleVariant != undefined && googleVariant != '' ? googleVariant.replace('italic', '') : '';
        };
        var googleGetStyle = function (googleVariant) {
            var variantStyle = '';
            if (googleVariant != undefined && googleVariant != '' && googleVariant.indexOf('italic') >= 0) {
                variantWeight = googleVariant.replace('italic', '');
                variantStyle = 'italic';
            }
            return variantStyle;
        };
        angular.forEach($scope.safeFonts, function (value, key) {
            $scope.fonts.push({
                groupName: 'Safe fonts',
                fontType: 'safe',
                fontFamily: value,
                fontWeight: 'normal',
                fontStyle: 'normal'
            });
        });
        angular.forEach($scope.dialogData.items, function (value, key) {
            var variants = value.variants;
            var variant = value.variants.length > 0 ? value.variants[0] : '';
            var fontWeight = googleGetWeight(variant);
            var fontStyle = googleGetStyle(variant);
            $scope.fonts.push({
                groupName: 'Google fonts',
                fontType: 'google',
                fontFamily: value.family,
                variants: value.variants,
                variant: variant,
                fontWeight: fontWeight,
                fontStyle: fontStyle
            });
        });
        $scope.setStyleVariant = function () {
            if ($scope.dialogItem != undefined) {
                return {
                    'font-family': $scope.selectedFont.fontFamily,
                    'font-weight': $scope.selectedFont.fontWeight,
                    'font-style': $scope.selectedFont.fontStyle
                };
            }
        };
        function loadFont(font, variant) {
            WebFont.load({
                google: { families: [font.fontFamily + ':' + variant] },
                loading: function () {
                    console.log('loading');
                },
                active: function () {
                    $scope.selectedFont = font;
                    $scope.selectedFont.fontWeight = googleGetWeight(variant);
                    $scope.selectedFont.fontStyle = googleGetStyle(variant);
                    // If $apply isn't called, the new font family isn't applied until the next user click.
                    $scope.change({
                        fontFamily: $scope.selectedFont.fontFamily,
                        fontType: $scope.selectedFont.fontType,
                        fontWeight: $scope.selectedFont.fontWeight,
                        fontStyle: $scope.selectedFont.fontStyle
                    });
                }
            });
        }
        var webFontScriptLoaded = false;
        $scope.showFontPreview = function (font, variant) {
            if (!variant)
                variant = font.variant;
            if (font != undefined && font.fontFamily != '' && font.fontType == 'google') {
                // Font needs to be independently loaded in the iframe for live preview to work.
                document.getElementById('resultFrame').contentWindow.getFont(font.fontFamily + ':' + variant);
                if (!webFontScriptLoaded) {
                    $.getScript('https://ajax.googleapis.com/ajax/libs/webfont/1/webfont.js').done(function () {
                        webFontScriptLoaded = true;
                        loadFont(font, variant);
                    }).fail(function () {
                        console.log('error loading webfont');
                    });
                } else {
                    loadFont(font, variant);
                }
            } else {
                // Font is available, apply it immediately in modal preview.
                $scope.selectedFont = font;
                // If $apply isn't called, the new font family isn't applied until the next user click.
                $scope.change({
                    fontFamily: $scope.selectedFont.fontFamily,
                    fontType: $scope.selectedFont.fontType,
                    fontWeight: $scope.selectedFont.fontWeight,
                    fontStyle: $scope.selectedFont.fontStyle
                });
            }
        };
        $scope.cancelAndClose = function () {
            $scope.cancel();
        };
        $scope.submitAndClose = function () {
            $scope.submit({
                fontFamily: $scope.selectedFont.fontFamily,
                fontType: $scope.selectedFont.fontType,
                fontWeight: $scope.selectedFont.fontWeight,
                fontStyle: $scope.selectedFont.fontStyle
            });
        };
        if ($scope.dialogItem != undefined) {
            angular.forEach($scope.fonts, function (value, key) {
                if (value.fontFamily == $scope.dialogItem.fontFamily) {
                    $scope.selectedFont = value;
                    $scope.selectedFont.variant = $scope.dialogItem.fontWeight + $scope.dialogItem.fontStyle;
                    $scope.selectedFont.fontWeight = $scope.dialogItem.fontWeight;
                    $scope.selectedFont.fontStyle = $scope.dialogItem.fontStyle;
                }
            });
        }
    });
    /*********************************************************************************************************/
    /* grid row editor */
    /*********************************************************************************************************/
    angular.module('Umbraco.canvasdesigner').controller('Umbraco.canvasdesigner.gridRow', function ($scope) {
        if (!$scope.item.values) {
            $scope.item.values = { fullsize: false };
        }
    });
    /*********************************************************************************************************/
    /* Layout */
    /*********************************************************************************************************/
    angular.module('Umbraco.canvasdesigner').controller('Umbraco.canvasdesigner.layout', function ($scope) {
        if (!$scope.item.values) {
            $scope.item.values = { layout: '' };
        }
    });
    /*********************************************************************************************************/
    /* margin editor */
    /*********************************************************************************************************/
    angular.module('Umbraco.canvasdesigner').controller('Umbraco.canvasdesigner.margin', function ($scope, dialogService) {
        $scope.defaultmarginList = [
            'all',
            'left',
            'right',
            'top',
            'bottom'
        ];
        $scope.marginList = [];
        $scope.selectedmargin = {
            name: '',
            value: 0
        };
        $scope.setSelectedmargin = function (margintype) {
            if (margintype == 'all') {
                $scope.selectedmargin.name = 'all';
                $scope.selectedmargin.value = $scope.item.values.marginvalue;
            }
            if (margintype == 'left') {
                $scope.selectedmargin.name = 'left';
                $scope.selectedmargin.value = $scope.item.values.leftmarginvalue;
            }
            if (margintype == 'right') {
                $scope.selectedmargin.name = 'right';
                $scope.selectedmargin.value = $scope.item.values.rightmarginvalue;
            }
            if (margintype == 'top') {
                $scope.selectedmargin.name = 'top';
                $scope.selectedmargin.value = $scope.item.values.topmarginvalue;
            }
            if (margintype == 'bottom') {
                $scope.selectedmargin.name = 'bottom';
                $scope.selectedmargin.value = $scope.item.values.bottommarginvalue;
            }
        };
        if (!$scope.item.values) {
            $scope.item.values = {
                marginvalue: $scope.item.defaultValue && $scope.item.defaultValue.length > 0 ? $scope.item.defaultValue[0] : '',
                leftmarginvalue: $scope.item.defaultValue && $scope.item.defaultValue.length > 1 ? $scope.item.defaultValue[1] : '',
                rightmarginvalue: $scope.item.defaultValue && $scope.item.defaultValue.length > 2 ? $scope.item.defaultValue[2] : '',
                topmarginvalue: $scope.item.defaultValue && $scope.item.defaultValue.length > 3 ? $scope.item.defaultValue[3] : '',
                bottommarginvalue: $scope.item.defaultValue && $scope.item.defaultValue.length > 4 ? $scope.item.defaultValue[4] : ''
            };
        }
        if ($scope.item.enable) {
            angular.forEach($scope.defaultmarginList, function (key, indexKey) {
                if ($.inArray(key, $scope.item.enable) >= 0) {
                    $scope.marginList.splice($scope.marginList.length + 1, 0, key);
                }
            });
        } else {
            $scope.marginList = $scope.defaultmarginList;
        }
        $scope.$watch('valueAreLoaded', function () {
            $scope.setSelectedmargin($scope.marginList[0]);
        }, false);
        $scope.$watch('selectedmargin', function () {
            if ($scope.selectedmargin.name == 'all') {
                $scope.item.values.marginvalue = $scope.selectedmargin.value;
            }
            if ($scope.selectedmargin.name == 'left') {
                $scope.item.values.leftmarginvalue = $scope.selectedmargin.value;
            }
            if ($scope.selectedmargin.name == 'right') {
                $scope.item.values.rightmarginvalue = $scope.selectedmargin.value;
            }
            if ($scope.selectedmargin.name == 'top') {
                $scope.item.values.topmarginvalue = $scope.selectedmargin.value;
            }
            if ($scope.selectedmargin.name == 'bottom') {
                $scope.item.values.bottommarginvalue = $scope.selectedmargin.value;
            }
        }, true);
    });
    /*********************************************************************************************************/
    /* padding editor */
    /*********************************************************************************************************/
    angular.module('Umbraco.canvasdesigner').controller('Umbraco.canvasdesigner.padding', function ($scope, dialogService) {
        $scope.defaultPaddingList = [
            'all',
            'left',
            'right',
            'top',
            'bottom'
        ];
        $scope.paddingList = [];
        $scope.selectedpadding = {
            name: '',
            value: 0
        };
        $scope.setSelectedpadding = function (paddingtype) {
            if (paddingtype == 'all') {
                $scope.selectedpadding.name = 'all';
                $scope.selectedpadding.value = $scope.item.values.paddingvalue;
            }
            if (paddingtype == 'left') {
                $scope.selectedpadding.name = 'left';
                $scope.selectedpadding.value = $scope.item.values.leftpaddingvalue;
            }
            if (paddingtype == 'right') {
                $scope.selectedpadding.name = 'right';
                $scope.selectedpadding.value = $scope.item.values.rightpaddingvalue;
            }
            if (paddingtype == 'top') {
                $scope.selectedpadding.name = 'top';
                $scope.selectedpadding.value = $scope.item.values.toppaddingvalue;
            }
            if (paddingtype == 'bottom') {
                $scope.selectedpadding.name = 'bottom';
                $scope.selectedpadding.value = $scope.item.values.bottompaddingvalue;
            }
        };
        if (!$scope.item.values) {
            $scope.item.values = {
                paddingvalue: $scope.item.defaultValue && $scope.item.defaultValue.length > 0 ? $scope.item.defaultValue[0] : '',
                leftpaddingvalue: $scope.item.defaultValue && $scope.item.defaultValue.length > 1 ? $scope.item.defaultValue[1] : '',
                rightpaddingvalue: $scope.item.defaultValue && $scope.item.defaultValue.length > 2 ? $scope.item.defaultValue[2] : '',
                toppaddingvalue: $scope.item.defaultValue && $scope.item.defaultValue.length > 3 ? $scope.item.defaultValue[3] : '',
                bottompaddingvalue: $scope.item.defaultValue && $scope.item.defaultValue.length > 4 ? $scope.item.defaultValue[4] : ''
            };
        }
        if ($scope.item.enable) {
            angular.forEach($scope.defaultPaddingList, function (key, indexKey) {
                if ($.inArray(key, $scope.item.enable) >= 0) {
                    $scope.paddingList.splice($scope.paddingList.length + 1, 0, key);
                }
            });
        } else {
            $scope.paddingList = $scope.defaultPaddingList;
        }
        $scope.$watch('valueAreLoaded', function () {
            $scope.setSelectedpadding($scope.paddingList[0]);
        }, false);
        $scope.$watch('selectedpadding', function () {
            if ($scope.selectedpadding.name == 'all') {
                $scope.item.values.paddingvalue = $scope.selectedpadding.value;
            }
            if ($scope.selectedpadding.name == 'left') {
                $scope.item.values.leftpaddingvalue = $scope.selectedpadding.value;
            }
            if ($scope.selectedpadding.name == 'right') {
                $scope.item.values.rightpaddingvalue = $scope.selectedpadding.value;
            }
            if ($scope.selectedpadding.name == 'top') {
                $scope.item.values.toppaddingvalue = $scope.selectedpadding.value;
            }
            if ($scope.selectedpadding.name == 'bottom') {
                $scope.item.values.bottompaddingvalue = $scope.selectedpadding.value;
            }
        }, true);
    });
    /*********************************************************************************************************/
    /* radius editor */
    /*********************************************************************************************************/
    angular.module('Umbraco.canvasdesigner').controller('Umbraco.canvasdesigner.radius', function ($scope, dialogService) {
        $scope.defaultRadiusList = [
            'all',
            'topleft',
            'topright',
            'bottomleft',
            'bottomright'
        ];
        $scope.radiusList = [];
        $scope.selectedradius = {
            name: '',
            value: 0
        };
        $scope.setSelectedradius = function (radiustype) {
            if (radiustype == 'all') {
                $scope.selectedradius.name = 'all';
                $scope.selectedradius.value = $scope.item.values.radiusvalue;
            }
            if (radiustype == 'topleft') {
                $scope.selectedradius.name = 'topleft';
                $scope.selectedradius.value = $scope.item.values.topleftradiusvalue;
            }
            if (radiustype == 'topright') {
                $scope.selectedradius.name = 'topright';
                $scope.selectedradius.value = $scope.item.values.toprightradiusvalue;
            }
            if (radiustype == 'bottomleft') {
                $scope.selectedradius.name = 'bottomleft';
                $scope.selectedradius.value = $scope.item.values.bottomleftradiusvalue;
            }
            if (radiustype == 'bottomright') {
                $scope.selectedradius.name = 'bottomright';
                $scope.selectedradius.value = $scope.item.values.bottomrightradiusvalue;
            }
        };
        if (!$scope.item.values) {
            $scope.item.values = {
                radiusvalue: $scope.item.defaultValue && $scope.item.defaultValue.length > 0 ? $scope.item.defaultValue[0] : '',
                topleftradiusvalue: $scope.item.defaultValue && $scope.item.defaultValue.length > 1 ? $scope.item.defaultValue[1] : '',
                toprightradiusvalue: $scope.item.defaultValue && $scope.item.defaultValue.length > 2 ? $scope.item.defaultValue[2] : '',
                bottomleftradiusvalue: $scope.item.defaultValue && $scope.item.defaultValue.length > 3 ? $scope.item.defaultValue[3] : '',
                bottomrightradiusvalue: $scope.item.defaultValue && $scope.item.defaultValue.length > 4 ? $scope.item.defaultValue[4] : ''
            };
        }
        if ($scope.item.enable) {
            angular.forEach($scope.defaultRadiusList, function (key, indexKey) {
                if ($.inArray(key, $scope.item.enable) >= 0) {
                    $scope.radiusList.splice($scope.radiusList.length + 1, 0, key);
                }
            });
        } else {
            $scope.radiusList = $scope.defaultRadiusList;
        }
        $scope.$watch('valueAreLoaded', function () {
            $scope.setSelectedradius($scope.radiusList[0]);
        }, false);
        $scope.$watch('selectedradius', function () {
            if ($scope.selectedradius.name == 'all') {
                $scope.item.values.radiusvalue = $scope.selectedradius.value;
            }
            if ($scope.selectedradius.name == 'topleft') {
                $scope.item.values.topleftradiusvalue = $scope.selectedradius.value;
            }
            if ($scope.selectedradius.name == 'topright') {
                $scope.item.values.toprightradiusvalue = $scope.selectedradius.value;
            }
            if ($scope.selectedradius.name == 'bottomleft') {
                $scope.item.values.bottomleftradiusvalue = $scope.selectedradius.value;
            }
            if ($scope.selectedradius.name == 'bottomright') {
                $scope.item.values.bottomrightradiusvalue = $scope.selectedradius.value;
            }
        }, true);
    });
    /*********************************************************************************************************/
    /* shadow editor */
    /*********************************************************************************************************/
    angular.module('Umbraco.canvasdesigner').controller('Umbraco.canvasdesigner.shadow', function ($scope) {
        if (!$scope.item.values) {
            $scope.item.values = { shadow: '' };
        }
    });
    /*********************************************************************************************************/
    /* slider editor */
    /*********************************************************************************************************/
    angular.module('Umbraco.canvasdesigner').controller('Umbraco.canvasdesigner.slider', function ($scope) {
        if (!$scope.item.values) {
            $scope.item.values = { slider: '' };
        }
    });
    /*********************************************************************************************************/
    /* spectrum color picker directive */
    /*********************************************************************************************************/
    angular.module('colorpicker', ['spectrumcolorpicker']).directive('colorpicker', [
        'dialogService',
        function (dialogService) {
            return {
                restrict: 'EA',
                scope: { ngModel: '=' },
                link: function (scope, $element) {
                    scope.openColorDialog = function () {
                        var config = {
                            template: 'colorModal.html',
                            change: function (data) {
                                scope.ngModel = data;
                            },
                            callback: function (data) {
                                scope.ngModel = data;
                            },
                            cancel: function (data) {
                                scope.ngModel = data;
                            },
                            dialogItem: scope.ngModel,
                            scope: scope
                        };
                        dialogService.open(config);
                    };
                    scope.setColor = false;
                    scope.submitAndClose = function () {
                        if (scope.ngModel != '') {
                            scope.setColor = true;
                            scope.submit(scope.ngModel);
                        } else {
                            scope.cancel();
                        }
                    };
                    scope.cancelAndClose = function () {
                        scope.cancel();
                    };
                },
                template: '<div>' + '<div class="color-picker-preview" ng-click="openColorDialog()" style="background: {{ngModel}} !important;"></div>' + '<script type="text/ng-template" id="colorModal.html">' + '<div class="modal-header">' + '<h1>Header</h1>' + '</div>' + '<div class="modal-body">' + '<spectrum colorselected="ngModel" set-color="setColor" flat="true" show-palette="true"></spectrum>' + '</div>' + '<div class="right">' + '<a class="btn" href="#" ng-click="cancelAndClose()">Cancel</a>' + '<a class="btn btn-success" href="#" ng-click="submitAndClose()">Done</a>' + '</div>' + '</script>' + '</div>',
                replace: true
            };
        }
    ]);
    /*********************************************************************************************************/
    /* jQuery UI Slider plugin wrapper */
    /*********************************************************************************************************/
    angular.module('Umbraco.canvasdesigner').factory('dialogService', function ($rootScope, $q, $http, $timeout, $compile, $templateCache) {
        function closeDialog(dialog, destroyScope) {
            if (dialog.element) {
                dialog.element.removeClass('selected');
                dialog.element.html('');
                if (destroyScope) {
                    dialog.scope.$destroy();
                }
            }
        }
        function open() {
        }
        return {
            open: function (options) {
                var defaults = {
                    template: '',
                    callback: undefined,
                    change: undefined,
                    cancel: undefined,
                    element: undefined,
                    dialogItem: undefined,
                    dialogData: undefined
                };
                var dialog = angular.extend(defaults, options);
                var destroyScope = true;
                if (options && options.scope) {
                    destroyScope = false;
                }
                var scope = options && options.scope || $rootScope.$new();
                // Save original value for cancel action
                var originalDialogItem = angular.copy(dialog.dialogItem);
                dialog.element = $('.float-panel');
                /************************************/
                // Close dialog if the user clicks outside the dialog. (Not working well with colorpickers and datepickers)
                $(document).mousedown(function (e) {
                    var container = dialog.element;
                    if (!container.is(e.target) && container.has(e.target).length === 0) {
                        closeDialog(dialog, destroyScope);
                    }
                });
                /************************************/
                $q.when($templateCache.get(dialog.template) || $http.get(dialog.template, { cache: true }).then(function (res) {
                    return res.data;
                })).then(function onSuccess(template) {
                    dialog.element.html(template);
                    $timeout(function () {
                        $compile(dialog.element)(scope);
                    });
                    dialog.element.addClass('selected');
                    scope.cancel = function () {
                        if (dialog.cancel) {
                            dialog.cancel(originalDialogItem);
                        }
                        closeDialog(dialog, destroyScope);
                    };
                    scope.change = function (data) {
                        if (dialog.change) {
                            dialog.change(data);
                        }
                    };
                    scope.submit = function (data) {
                        if (dialog.callback) {
                            dialog.callback(data);
                        }
                        closeDialog(dialog, destroyScope);
                    };
                    scope.close = function () {
                        closeDialog(dialog, destroyScope);
                    };
                    scope.dialogData = dialog.dialogData;
                    scope.dialogItem = dialog.dialogItem;
                    dialog.scope = scope;
                });
                return dialog;
            },
            close: function () {
                var modal = $('.float-panel');
                modal.removeClass('selected');
            }
        };
    });
    /*********************************************************************************************************/
    /* jQuery UI Slider plugin wrapper */
    /*********************************************************************************************************/
    angular.module('ui.slider', []).value('uiSliderConfig', {}).directive('uiSlider', [
        'uiSliderConfig',
        '$timeout',
        function (uiSliderConfig, $timeout) {
            uiSliderConfig = uiSliderConfig || {};
            return {
                require: 'ngModel',
                template: '<div><div class="slider" /><input class="slider-input" style="display:none" ng-model="value"></div>',
                replace: true,
                compile: function () {
                    return function (scope, elm, attrs, ngModel) {
                        scope.value = ngModel.$viewValue;
                        function parseNumber(n, decimals) {
                            return decimals ? parseFloat(n) : parseInt(n);
                        }
                        ;
                        var options = angular.extend(scope.$eval(attrs.uiSlider) || {}, uiSliderConfig);
                        // Object holding range values
                        var prevRangeValues = {
                            min: null,
                            max: null
                        };
                        // convenience properties
                        var properties = [
                            'min',
                            'max',
                            'step'
                        ];
                        var useDecimals = !angular.isUndefined(attrs.useDecimals) ? true : false;
                        var init = function () {
                            // When ngModel is assigned an array of values then range is expected to be true.
                            // Warn user and change range to true else an error occurs when trying to drag handle
                            if (angular.isArray(ngModel.$viewValue) && options.range !== true) {
                                console.warn('Change your range option of ui-slider. When assigning ngModel an array of values then the range option should be set to true.');
                                options.range = true;
                            }
                            // Ensure the convenience properties are passed as options if they're defined
                            // This avoids init ordering issues where the slider's initial state (eg handle
                            // position) is calculated using widget defaults
                            // Note the properties take precedence over any duplicates in options
                            angular.forEach(properties, function (property) {
                                if (angular.isDefined(attrs[property])) {
                                    options[property] = parseNumber(attrs[property], useDecimals);
                                }
                            });
                            elm.find('.slider').slider(options);
                            init = angular.noop;
                        };
                        // Find out if decimals are to be used for slider
                        angular.forEach(properties, function (property) {
                            // support {{}} and watch for updates
                            attrs.$observe(property, function (newVal) {
                                if (!!newVal) {
                                    init();
                                    elm.find('.slider').slider('option', property, parseNumber(newVal, useDecimals));
                                }
                            });
                        });
                        attrs.$observe('disabled', function (newVal) {
                            init();
                            elm.find('.slider').slider('option', 'disabled', !!newVal);
                        });
                        // Watch ui-slider (byVal) for changes and update
                        scope.$watch(attrs.uiSlider, function (newVal) {
                            init();
                            if (newVal != undefined) {
                                elm.find('.slider').slider('option', newVal);
                                elm.find('.ui-slider-handle').html('<span>' + ui.value + 'px</span>');
                            }
                        }, true);
                        // Late-bind to prevent compiler clobbering
                        $timeout(init, 0, true);
                        // Update model value from slider
                        elm.find('.slider').bind('slidestop', function (event, ui) {
                            ngModel.$setViewValue(ui.values || ui.value);
                            scope.$apply();
                        });
                        elm.bind('slide', function (event, ui) {
                            event.stopPropagation();
                            elm.find('.slider-input').val(ui.value);
                            elm.find('.ui-slider-handle').html('<span>' + ui.value + 'px</span>');
                        });
                        // Update slider from model value
                        ngModel.$render = function () {
                            init();
                            var method = options.range === true ? 'values' : 'value';
                            if (isNaN(ngModel.$viewValue) && !(ngModel.$viewValue instanceof Array))
                                ngModel.$viewValue = 0;
                            if (ngModel.$viewValue == '')
                                ngModel.$viewValue = 0;
                            scope.value = ngModel.$viewValue;
                            // Do some sanity check of range values
                            if (options.range === true) {
                                // Check outer bounds for min and max values
                                if (angular.isDefined(options.min) && options.min > ngModel.$viewValue[0]) {
                                    ngModel.$viewValue[0] = options.min;
                                }
                                if (angular.isDefined(options.max) && options.max < ngModel.$viewValue[1]) {
                                    ngModel.$viewValue[1] = options.max;
                                }
                                // Check min and max range values
                                if (ngModel.$viewValue[0] >= ngModel.$viewValue[1]) {
                                    // Min value should be less to equal to max value
                                    if (prevRangeValues.min >= ngModel.$viewValue[1])
                                        ngModel.$viewValue[0] = prevRangeValues.min;
                                    // Max value should be less to equal to min value
                                    if (prevRangeValues.max <= ngModel.$viewValue[0])
                                        ngModel.$viewValue[1] = prevRangeValues.max;
                                }
                                // Store values for later user
                                prevRangeValues.min = ngModel.$viewValue[0];
                                prevRangeValues.max = ngModel.$viewValue[1];
                            }
                            elm.find('.slider').slider(method, ngModel.$viewValue);
                            elm.find('.ui-slider-handle').html('<span>' + ngModel.$viewValue + 'px</span>');
                        };
                        scope.$watch('value', function () {
                            ngModel.$setViewValue(scope.value);
                        }, true);
                        scope.$watch(attrs.ngModel, function () {
                            if (options.range === true) {
                                ngModel.$render();
                            }
                        }, true);
                        function destroy() {
                            elm.find('.slider').slider('destroy');
                        }
                        elm.find('.slider').bind('$destroy', destroy);
                    };
                }
            };
        }
    ]);
    /*********************************************************************************************************/
    /* spectrum color picker directive */
    /*********************************************************************************************************/
    angular.module('spectrumcolorpicker', []).directive('spectrum', function () {
        return {
            restrict: 'E',
            transclude: true,
            scope: {
                colorselected: '=',
                setColor: '=',
                flat: '=',
                showPalette: '='
            },
            link: function (scope, $element) {
                var initColor;
                $element.find('input').spectrum({
                    color: scope.colorselected,
                    allowEmpty: true,
                    preferredFormat: 'hex',
                    showAlpha: true,
                    showInput: true,
                    flat: scope.flat,
                    localStorageKey: 'spectrum.panel',
                    showPalette: scope.showPalette,
                    palette: [],
                    change: function (color) {
                        if (color) {
                            scope.colorselected = color.toRgbString();
                        } else {
                            scope.colorselected = '';
                        }
                        scope.$apply();
                    },
                    move: function (color) {
                        scope.colorselected = color.toRgbString();
                        scope.$apply();
                    },
                    beforeShow: function (color) {
                        initColor = angular.copy(scope.colorselected);
                        $(this).spectrum('container').find('.sp-cancel').click(function (e) {
                            scope.colorselected = initColor;
                            scope.$apply();
                        });
                    }
                });
                scope.$watch('setcolor', function (setColor) {
                    if (scope.$eval(setColor) === true) {
                        $element.find('input').spectrum('set', scope.colorselected);
                    }
                }, true);
            },
            template: '      <div class="spectrumcolorpicker"><div class="real-color-preview" style="background-color:{{colorselected}}"></div><input type=\'text\' ng-model=\'colorselected\' /></div>',
            replace: true
        };
    });
}());