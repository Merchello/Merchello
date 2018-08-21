;(function ( $, window, document, undefined) {
    // Register Forms
    $.wcpEditorCreateForm({
        name: 'Image Map Settings',
        controlGroups: [
            {
                groupName: 'general',
                groupTitle: 'General',
                groupIcon: 'fa fa-cog',
                controls: [
                    //{
                    //    type: 'text',
                    //    name: 'image_map_name',
                    //    title: 'Image Map Name',
                    //    value: $.imageMapProDefaultSettings.general.name
                    //},
                    {
                        type: 'switch',
                        name: 'responsive',
                        title: 'Responsive',
                        value: $.imageMapProDefaultSettings.general.responsive,
                    },
                    {
                        type: 'switch',
                        name: 'preserve_quality',
                        title: 'Preserve Quality',
                        value: $.imageMapProDefaultSettings.general.preserve_quality,
                    },
                    {
                        type: 'int',
                        name: 'image_map_width',
                        title: 'Width',
                        value: $.imageMapProDefaultSettings.general.width,
                    },
                    {
                        type: 'int',
                        name: 'image_map_height',
                        title: 'Height',
                        value: $.imageMapProDefaultSettings.general.height
                        },
                    {
                        type: 'button',
                        name: 'reset_size',
                        title: 'Reset Size',
                        options: { event_name: 'button-reset-size-clicked' },
                        value: undefined
                    },
                    {
                        type: 'select',
                        name: 'pageload_animation',
                        title: 'Page Load Animation',
                        options: [
                            { value: 'none', title: 'None' },
                            { value: 'grow', title: 'Grow' },
                            { value: 'fade', title: 'Fade' },
                        ],
                        value: $.imageMapProDefaultSettings.general.pageload_animation
                    },
                    {
                        type: 'switch',
                        name: 'late_initialization',
                        title: 'Late Initialization',
                        value: $.imageMapProDefaultSettings.general.late_initialization,
                    },
                    {
                        type: 'switch',
                        name: 'center_image_map',
                        title: 'Center Image Map',
                        value: $.imageMapProDefaultSettings.general.center_image_map,
                    },
                ]
            },
            {
                groupName: 'image',
                groupTitle: 'Image',
                groupIcon: 'fa fa-photo',
                controls: [
                    {
                        type: 'wp media upload',
                        name: 'image_url',
                        title: 'Image URL',
                        value: $.imageMapProDefaultSettings.general.image_url
                    },
                ]
            },
            {
                groupName: 'tooltips',
                groupTitle: 'Tooltips',
                groupIcon: 'fa fa-comment',
                controls: [
                    {
                        type: 'switch',
                        name: 'sticky_tooltips',
                        title: 'Sticky Tooltips',
                        value: $.imageMapProDefaultSettings.general.sticky_tooltips,
                    },
                    {
                        type: 'switch',
                        name: 'constrain_tooltips',
                        title: 'Constrain Tooltips',
                        value: $.imageMapProDefaultSettings.general.constrain_tooltips,
                    },
                    {
                        type: 'select',
                        name: 'tooltip_animation',
                        title: 'Tooltip Animation',
                        options: [
                            { value: 'none', title: 'None' },
                            { value: 'grow', title: 'Grow' },
                            { value: 'fade', title: 'Fade' },
                        ],
                        value: $.imageMapProDefaultSettings.general.tooltip_animation
                    },
                    {
                        type: 'select',
                        name: 'fullscreen_tooltips',
                        title: 'Fullscreen Tooltips',
                        options: [
                            { value: 'none', title: 'None' },
                            { value: 'mobile-only', title: 'Mobile Only' },
                            { value: 'always', title: 'Always' },
                        ],
                        value: $.imageMapProDefaultSettings.general.fullscreen_tooltips
                    },
                ]
            },
            {
                groupName: 'fullscreen',
                groupTitle: 'Fullscreen',
                groupIcon: 'fa fa-arrows-alt',
                controls: [
                    {
                        type: 'switch',
                        name: 'enable_fullscreen_mode',
                        title: 'Enable Fullscreen Mode',
                        value: $.imageMapProDefaultSettings.fullscreen.enable_fullscreen_mode,
                    },
                    {
                        type: 'switch',
                        name: 'start_in_fullscreen_mode',
                        title: 'Start in Fullscreen Mode',
                        value: $.imageMapProDefaultSettings.fullscreen.start_in_fullscreen_mode,
                    },
                    {
                        type: 'color',
                        name: 'fullscreen_background',
                        title: 'Fullscreen Background',
                        value: $.imageMapProDefaultSettings.fullscreen.fullscreen_background,
                    },
                    {
                        type: 'fullscreen button position',
                        name: 'fullscreen_button_position',
                        title: 'Fullscreen Button Position',
                        value: 1 // 0 = top left, 1 = top center, 2 = top right, 3 = bottom right, 4 = bottom center, 5 = bottom left
                    },
                    {
                        type: 'button group',
                        name: 'fullscreen_button_type',
                        title: 'Button Type',
                        options: [
                            { value: 'icon', title: 'Icon' },
                            { value: 'text', title: 'Text' },
                            { value: 'icon_and_text', title: 'Icon and Text' }
                        ],
                        value: $.imageMapProDefaultSettings.fullscreen.fullscreen_button_type,
                    },
                    {
                        type: 'color',
                        name: 'fullscreen_button_color',
                        title: 'Button Color',
                        value: $.imageMapProDefaultSettings.fullscreen.fullscreen_button_color,
                    },
                    {
                        type: 'color',
                        name: 'fullscreen_button_text_color',
                        title: 'Button Icon/Text Color',
                        value: $.imageMapProDefaultSettings.fullscreen.fullscreen_button_text_color,
                    }
                ]
            }
        ]
    });
    $.wcpEditorCreateForm({
        name: 'Shape Settings',
        controlGroups: [
            {
                groupName: 'general',
                groupTitle: 'General',
                groupIcon: 'fa fa-cog',
                controls: [
                    {
                        type: 'float',
                        name: 'x',
                        title: 'X',
                        value: $.imageMapProDefaultSpotSettings.x
                    },
                    {
                        type: 'float',
                        name: 'y',
                        title: 'Y',
                        value: $.imageMapProDefaultSpotSettings.y
                    },
                    {
                        type: 'float',
                        name: 'width',
                        title: 'Width',
                        value: $.imageMapProDefaultSpotSettings.width
                    },
                    {
                        type: 'float',
                        name: 'height',
                        title: 'Height',
                        value: $.imageMapProDefaultSpotSettings.height
                    },
                ]
            },
            {
                groupName: 'actions',
                groupTitle: 'Actions',
                groupIcon: 'fa fa-bolt',
                controls: [
                    {
                        type: 'select',
                        name: 'mouseover',
                        title: 'Mouseover Action',
                        options: [
                            { value: 'no-action', title: 'No Action' },
                            { value: 'show-tooltip', title: 'Show Tooltip' },
                        ],
                        value: $.imageMapProDefaultSpotSettings.actions.mouseover
                    },
                    {
                        type: 'select',
                        name: 'click',
                        title: 'Click Action',
                        options: [
                            { value: 'no-action', title: 'No Action' },
                            { value: 'show-tooltip', title: 'Show Tooltip' },
                            { value: 'follow-link', title: 'Follow Link' },
                        ],
                        value: $.imageMapProDefaultSpotSettings.actions.click
                    },
                    {
                        type: 'text',
                        name: 'link',
                        title: 'Link URL',
                        value: $.imageMapProDefaultSpotSettings.actions.link
                    },
                    {
                        type: 'switch',
                        name: 'open_link_in_new_window',
                        title: 'Open Link in New Window',
                        value: $.imageMapProDefaultSpotSettings.actions.open_link_in_new_window
                    },
                ]
            },
            {
                groupName: 'icon',
                groupTitle: 'Icon',
                groupIcon: 'fa fa-map-marker',
                controls: [
                    {
                        type: 'switch',
                        name: 'use_icon',
                        title: 'Use Icon',
                        value: $.imageMapProDefaultSpotSettings.default_style.use_icon
                    },
                    {
                        type: 'button group',
                        name: 'icon_type',
                        title: 'Icon Type',
                        options: [
                            { value: 'library', title: 'Library' },
                            { value: 'custom', title: 'Custom' },
                        ],
                        value: $.imageMapProDefaultSpotSettings.default_style.icon_type
                    },
                    {
                        type: 'button',
                        name: 'choose_icon_from_library',
                        title: 'Choose from Library',
                        options: { event_name: 'button-choose-icon-clicked' },
                        value: undefined
                    },
                    {
                        type: 'text',
                        name: 'icon_svg_path',
                        title: 'Icon SVG Path',
                        value: $.imageMapProDefaultSpotSettings.default_style.icon_svg_path,
                        render: false
                    },
                    {
                        type: 'text',
                        name: 'icon_svg_viewbox',
                        title: 'Icon SVG Viewbox',
                        value: $.imageMapProDefaultSpotSettings.default_style.icon_svg_viewbox,
                        render: false
                    },
                    {
                        type: 'text',
                        name: 'icon_url',
                        title: 'Icon URL',
                        value: $.imageMapProDefaultSpotSettings.default_style.icon_url
                    },
                    {
                        type: 'switch',
                        name: 'icon_is_pin',
                        title: 'Icon is a Pin',
                        value: $.imageMapProDefaultSpotSettings.default_style.icon_is_pin
                    },
                    {
                        type: 'switch',
                        name: 'icon_shadow',
                        title: 'Icon Shadow',
                        value: $.imageMapProDefaultSpotSettings.default_style.icon_shadow
                    },
                ]
            },
            {
                groupName: 'default_style',
                groupTitle: 'Default Style',
                groupIcon: 'fa fa-paint-brush',
                controls: [
                    {
                        type: 'slider',
                        name: 'opacity',
                        title: 'Opacity',
                        options: { min: 0, max: 1 },
                        value: $.imageMapProDefaultSpotSettings.default_style.opacity
                    },
                    {
                        type: 'color',
                        name: 'icon_fill',
                        title: 'SVG Icon Fill Color',
                        value: $.imageMapProDefaultSpotSettings.default_style.icon_fill
                    },
                    {
                        type: 'int',
                        name: 'border_radius',
                        title: 'Border Radius',
                        value: $.imageMapProDefaultSpotSettings.default_style.border_radius
                    },
                    {
                        type: 'color',
                        name: 'background_color',
                        title: 'Background Color',
                        value: $.imageMapProDefaultSpotSettings.default_style.background_color
                    },
                    {
                        type: 'slider',
                        name: 'background_opacity',
                        title: 'Background Opacity',
                        options: { min: 0, max: 1 },
                        value: $.imageMapProDefaultSpotSettings.default_style.background_opacity
                    },
                    {
                        type: 'slider',
                        name: 'border_width',
                        title: 'Border Width',
                        options: { min: 0, max: 20, type: 'int' },
                        value: $.imageMapProDefaultSpotSettings.default_style.border_width
                    },
                    {
                        type: 'select',
                        name: 'border_style',
                        title: 'Border Style',
                        options: [
                            { value: 'none', title: 'None' },
                            { value: 'hidden', title: 'Hidden' },
                            { value: 'solid', title: 'Solid' },
                            { value: 'dotted', title: 'Dotted' },
                            { value: 'dashed', title: 'Dashed' },
                            { value: 'double', title: 'Double' },
                            { value: 'groove', title: 'Groove' },
                            { value: 'inset', title: 'Inset' },
                            { value: 'outset', title: 'Outset' },
                        ],
                        value: $.imageMapProDefaultSpotSettings.default_style.border_style
                    },
                    {
                        type: 'color',
                        name: 'border_color',
                        title: 'Border Color',
                        value: $.imageMapProDefaultSpotSettings.default_style.border_color
                    },
                    {
                        type: 'slider',
                        name: 'border_opacity',
                        title: 'Border Opacity',
                        options: { min: 0, max: 1 },
                        value: $.imageMapProDefaultSpotSettings.default_style.border_opacity
                    },
                    {
                        type: 'color',
                        name: 'fill',
                        title: 'Fill',
                        value: $.imageMapProDefaultSpotSettings.default_style.fill
                    },
                    {
                        type: 'slider',
                        name: 'fill_opacity',
                        title: 'Fill Opacity',
                        options: { min: 0, max: 1 },
                        value: $.imageMapProDefaultSpotSettings.default_style.fill_opacity
                    },
                    {
                        type: 'color',
                        name: 'stroke_color',
                        title: 'Stroke Color',
                        value: $.imageMapProDefaultSpotSettings.default_style.stroke_color
                    },
                    {
                        type: 'slider',
                        name: 'stroke_opacity',
                        title: 'Stroke Opacity',
                        options: { min: 0, max: 1 },
                        value: $.imageMapProDefaultSpotSettings.default_style.stroke_opacity
                    },
                    {
                        type: 'slider',
                        name: 'stroke_width',
                        title: 'Stroke Width',
                        options: { min: 0, max: 20, type: 'int' },
                        value: $.imageMapProDefaultSpotSettings.default_style.stroke_width
                    },
                    {
                        type: 'text',
                        name: 'stroke_dasharray',
                        title: 'Stroke Dasharray',
                        value: $.imageMapProDefaultSpotSettings.default_style.stroke_dasharray
                    },
                    {
                        type: 'select',
                        name: 'stroke_linecap',
                        title: 'Stroke Linecap',
                        options: [
                            { value: 'butt', title: 'Butt' },
                            { value: 'round', title: 'Round' },
                            { value: 'square', title: 'Square' },
                        ],
                        value: $.imageMapProDefaultSpotSettings.default_style.stroke_linecap
                    },
                ]
            },
            {
                groupName: 'mouseover_style',
                groupTitle: 'Mouseover Style',
                groupIcon: 'fa fa-paint-brush',
                controls: [
                    {
                        type: 'button',
                        name: 'copy_from_default_styles',
                        title: 'Copy from Default Styles',
                        options: { event_name: 'button-copy-from-default-styles-clicked' },
                        value: undefined
                    },
                    {
                        type: 'slider',
                        name: 'mouseover_opacity',
                        title: 'Opacity',
                        options: { min: 0, max: 1 },
                        value: $.imageMapProDefaultSpotSettings.mouseover_style.opacity
                    },
                    {
                        type: 'color',
                        name: 'mouseover_icon_fill',
                        title: 'SVG Icon Fill Color',
                        value: $.imageMapProDefaultSpotSettings.mouseover_style.icon_fill
                    },
                    {
                        type: 'int',
                        name: 'mouseover_border_radius',
                        title: 'Border Radius',
                        value: $.imageMapProDefaultSpotSettings.mouseover_style.border_radius
                    },
                    {
                        type: 'color',
                        name: 'mouseover_background_color',
                        title: 'Background Color',
                        value: $.imageMapProDefaultSpotSettings.mouseover_style.background_color
                    },
                    {
                        type: 'slider',
                        name: 'mouseover_background_opacity',
                        title: 'Background Opacity',
                        options: { min: 0, max: 1 },
                        value: $.imageMapProDefaultSpotSettings.mouseover_style.background_opacity
                    },
                    {
                        type: 'slider',
                        name: 'mouseover_border_width',
                        title: 'Border Width',
                        options: { min: 0, max: 20, type: 'int' },
                        value: $.imageMapProDefaultSpotSettings.mouseover_style.border_width
                    },
                    {
                        type: 'select',
                        name: 'mouseover_border_style',
                        title: 'Border Style',
                        options: [
                            { value: 'none', title: 'None' },
                            { value: 'hidden', title: 'Hidden' },
                            { value: 'solid', title: 'Solid' },
                            { value: 'dotted', title: 'Dotted' },
                            { value: 'dashed', title: 'Dashed' },
                            { value: 'double', title: 'Double' },
                            { value: 'groove', title: 'Groove' },
                            { value: 'inset', title: 'Inset' },
                            { value: 'outset', title: 'Outset' },
                        ],
                        value: $.imageMapProDefaultSpotSettings.mouseover_style.border_style
                    },
                    {
                        type: 'color',
                        name: 'mouseover_border_color',
                        title: 'Border Color',
                        value: $.imageMapProDefaultSpotSettings.mouseover_style.border_color
                    },
                    {
                        type: 'slider',
                        name: 'mouseover_border_opacity',
                        title: 'Border Opacity',
                        options: { min: 0, max: 1 },
                        value: $.imageMapProDefaultSpotSettings.mouseover_style.border_opacity
                    },
                    {
                        type: 'color',
                        name: 'mouseover_fill',
                        title: 'Fill',
                        value: $.imageMapProDefaultSpotSettings.mouseover_style.fill
                    },
                    {
                        type: 'slider',
                        name: 'mouseover_fill_opacity',
                        title: 'Fill Opacity',
                        options: { min: 0, max: 1 },
                        value: $.imageMapProDefaultSpotSettings.mouseover_style.fill_opacity
                    },
                    {
                        type: 'color',
                        name: 'mouseover_stroke_color',
                        title: 'Stroke Color',
                        value: $.imageMapProDefaultSpotSettings.mouseover_style.stroke_color
                    },
                    {
                        type: 'slider',
                        name: 'mouseover_stroke_opacity',
                        title: 'Stroke Opacity',
                        options: { min: 0, max: 1 },
                        value: $.imageMapProDefaultSpotSettings.mouseover_style.stroke_opacity
                    },
                    {
                        type: 'slider',
                        name: 'mouseover_stroke_width',
                        title: 'Stroke Width',
                        options: { min: 0, max: 20, type: 'int' },
                        value: $.imageMapProDefaultSpotSettings.mouseover_style.stroke_width
                    },
                    {
                        type: 'text',
                        name: 'mouseover_stroke_dasharray',
                        title: 'Stroke Dasharray',
                        value: $.imageMapProDefaultSpotSettings.mouseover_style.stroke_dasharray
                    },
                    {
                        type: 'select',
                        name: 'mouseover_stroke_linecap',
                        title: 'Stroke Linecap',
                        options: [
                            { value: 'butt', title: 'Butt' },
                            { value: 'round', title: 'Round' },
                            { value: 'square', title: 'Square' },
                        ],
                        value: $.imageMapProDefaultSpotSettings.mouseover_style.stroke_linecap
                    },
                ]
            },
            {
                groupName: 'tooltip_settings',
                groupTitle: 'Tooltip Settings',
                groupIcon: 'fa fa-comment',
                controls: [
                    {
                        type: 'int',
                        name: 'tooltip_border_radius',
                        title: 'Border Radius',
                        value: $.imageMapProDefaultSpotSettings.tooltip_style.border_radius
                    },
                    {
                        type: 'int',
                        name: 'tooltip_padding',
                        title: 'Padding',
                        value: $.imageMapProDefaultSpotSettings.tooltip_style.padding
                    },
                    {
                        type: 'color',
                        name: 'tooltip_background_color',
                        title: 'Background Color',
                        value: $.imageMapProDefaultSpotSettings.tooltip_style.background_color
                    },
                    {
                        type: 'slider',
                        name: 'tooltip_background_opacity',
                        title: 'Background Opacity',
                        options: { min: 0, max: 1 },
                        value: $.imageMapProDefaultSpotSettings.tooltip_style.background_opacity
                    },
                    {
                        type: 'select',
                        name: 'tooltip_position',
                        title: 'Position',
                        options: [
                            { value: 'top', title: 'Top' },
                            { value: 'bottom', title: 'Bottom' },
                            { value: 'left', title: 'Left' },
                            { value: 'right', title: 'Right' },
                        ],
                        value: $.imageMapProDefaultSpotSettings.tooltip_style.position
                    },
                    {
                        type: 'switch',
                        name: 'tooltip_auto_width',
                        title: 'Auto Width',
                        value: $.imageMapProDefaultSpotSettings.tooltip_style.auto_width
                    },
                    {
                        type: 'int',
                        name: 'tooltip_width',
                        title: 'Width',
                        value: $.imageMapProDefaultSpotSettings.tooltip_style.width
                    },
                ]
            },
            {
                groupName: 'tooltip_content',
                groupTitle: 'Tooltip Content',
                groupIcon: 'fa fa-paragraph',
                controls: [
                    {
                        type: 'button group',
                        name: 'tooltip_content_type',
                        title: 'Tooltip Content',
                        options: [
                            { value: 'plain-text', title: 'Plain Text' },
                            { value: 'content-builder', title: 'Content Builder' },
                        ]
                    },
                    {
                        type: 'textarea',
                        name: 'plain_text',
                        title: 'Tooltip Content',
                        value: $.imageMapProDefaultSpotSettings.tooltip_content.plain_text
                    },
                    {
                        type: 'color',
                        name: 'plain_text_color',
                        title: 'Text Color',
                        value: $.imageMapProDefaultSpotSettings.tooltip_content.plain_text_color
                    },
                    {
                        type: 'object',
                        name: 'squares_settings',
                        title: 'Squares Settings',
                        value: $.imageMapProDefaultSpotSettings.tooltip_content.squares_settings,
						render: false
                    },
                    {
                        type: 'button',
                        name: 'launch_content_builder',
                        title: 'Launch Content Builder',
                        options: { event_name: 'button-launch-content-builder-clicked' },
                        value: undefined
                    },
                ]
            },
        ]
    });
	   
    var extraMainButtons = [
        {
            name: 'code',
            icon: 'fa fa-code',
            title: 'Code'
        },
        {
            name: 'import',
            icon: 'fa fa-arrow-down',
            title: 'Import'
        },
        {
            name: 'export',
            icon: 'fa fa-arrow-up',
            title: 'Export'
        }
    ];
    
    $.WCPEditorSettings = {
        mainTabs: [
            {
                name: 'Image Map',
                icon: 'fa fa-cog',
                title: 'Image Map Settings'
            },
            {
                name: 'Shape',
                icon: 'fa fa-object-ungroup',
                title: 'Selected Shape Settings'
            }
        ],
        toolbarButtons: [
            [
                {
                    name: 'spot',
                    icon: 'fa fa-map-marker',
                    title: 'Icon'
                },
                {
                    name: 'oval',
                    customIcon: '<div style="width: 14px; height: 14px; border: 2px solid #222; border-radius: 50px;"></div>',
                    title: 'Ellipse'
                },
                {
                    name: 'rect',
                    customIcon: '<div style="width: 20px; height: 14px; border: 2px solid #222; border-radius: 5px;"></div>',
                    title: 'Rectangle'
                },
                {
                    name: 'poly',
                    customIcon: '<svg width="24px" height="24px" viewport="0 0 24 24" version="1.1" xmlns="http://www.w3.org/2000/svg"><polygon fill="none" style="stroke: black; stroke-width: 2px;" points="20,20 18,4 7,7 4,20"></polygon><ellipse cx="20" cy="20" rx="3" ry="3"></ellipse><ellipse cx="18" cy="4" rx="3" ry="3"></ellipse><ellipse cx="7" cy="7" rx="3" ry="3"></ellipse><ellipse cx="4" cy="20" rx="3" ry="3"></ellipse></svg>',
                    title: 'Polygon'
                },
            ],
            [
                {
                    name: 'select',
                    icon: 'fa fa-mouse-pointer',
                    title: 'Select'
                },
                {
                    name: 'zoom-in',
                    icon: 'fa fa-search-plus',
                    title: 'Zoom In (CTRL +)',
                },
                {
                    name: 'zoom-out',
                    icon: 'fa fa-search-minus',
                    title: 'Zoom Out (CTRL -)'
                },
                {
                    name: 'drag',
                    icon: 'fa fa-hand-paper-o',
                    title: 'Drag Canvas (Hold Spacebar and Drag)'
                },
                {
                    name: 'reset',
                    customIcon: '1:1',
                    title: 'Reset Canvas Zoom & Pan (CTRL + 0)',
                    kind: 'button'
                },
            ]
        ], 
        extraMainButtons: extraMainButtons,
        listItemButtons: [
               
        ],
        listItemTitle: 'Shapes',
        listItemTitleButtons: [
            {
                name: 'rename',
                icon: 'fa fa-pencil',
                title: 'Rename'
            },
            {
                name: 'copy',
                icon: 'fa fa-files-o',
                title: 'Copy Style'
            },
            {
                name: 'paste',
                icon: 'fa fa-clipboard',
                title: 'Paste Style'
            },
            {
                name: 'duplicate',
                icon: 'fa fa-clone',
                title: 'Duplicate'
            },
            {
                name: 'delete',
                icon: 'fa fa-trash-o',
                title: 'Delete'
            },
        ],
        fullscreenButton: false,
        newButton: true,
        helpButton: false,
        previewToggle: true
	};
	   
})( jQuery, window, document );
