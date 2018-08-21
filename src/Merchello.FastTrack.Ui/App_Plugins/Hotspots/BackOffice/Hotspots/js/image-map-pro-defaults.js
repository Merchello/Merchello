(function($, window, document, undefined) {
	
    $.imageMapProShapeDefaults = {
		id: 'spot-0',
		title: '',
		type: 'spot',
		x: -1,
		y: -1,
		width: 44,
		height: 44,
		actions: {
			mouseover: 'show-tooltip',
			click: 'no-action',
			link: '#',
			open_link_in_new_window: 1
		},
		default_style: {
			opacity: 1,
			border_radius: 50,
			background_color: '#000000',
			background_opacity: 0.4,
			border_width: 0,
			border_style: 'solid',
			border_color: '#ffffff',
			border_opacity: 1,

			// poly-specific
			fill: '#000000',
			fill_opacity: 0.4,
			stroke_color: '#ffffff',
			stroke_opacity: 0.75,
			stroke_width: 0,
			stroke_dasharray: '10 10',
			stroke_linecap: 'round',

			// spot-specific
			use_icon: 1,
			icon_type: 'library', // or 'custom'
			icon_svg_path: 'M409.81,160.113C409.79,71.684,338.136,0,249.725,0C161.276,0,89.583,71.684,89.583,160.113     c0,76.325,119.274,280.238,151.955,334.638c1.72,2.882,4.826,4.641,8.178,4.641c3.351,0,6.468-1.759,8.168-4.631     C290.545,440.361,409.81,236.438,409.81,160.113z M249.716,283.999c-68.303,0-123.915-55.573-123.915-123.895     c0-68.313,55.592-123.895,123.915-123.895s123.876,55.582,123.876,123.895S318.029,283.999,249.716,283.999z',
			icon_svg_viewbox: '0 0 499.392 499.392',
			icon_fill: '#000000',
			icon_url: '',
			icon_is_pin: 1,
			icon_shadow: 0
		},
		mouseover_style: {
			opacity: 1,
			border_radius: 50,
			background_color: '#ffffff',
			background_opacity: 0.4,
			border_width: 0,
			border_style: 'solid',
			border_color: '#ffffff',
			border_opacity: 1,

			// poly-specific
			fill: '#ffffff',
			fill_opacity: 0.4,
			stroke_color: '#ffffff',
			stroke_opacity: 0.75,
			stroke_width: 0,
			stroke_dasharray: '10 10',
			stroke_linecap: 'round',

			// spot-specific
			icon_fill: '#000000'
		},
		tooltip_style: {
			border_radius: 5,
			padding: 20,
			background_color: '#000000',
			background_opacity: 0.9,
			position: 'top',
			width: 300,
			auto_width: 0
		},
		tooltip_content: {
			content_type: 'plain-text',
			plain_text: 'Lorem Ipsum',
			plain_text_color: '#ffffff',
			squares_settings: {
				containers: [{
					id: "sq-container-403761",
					settings: {
						elements: [{
							settings: {
								name: "Paragraph",
								iconClass: "fa fa-paragraph"
							}
						}]
					}
				}]
			}
		},
		points: [],
		vs: []
	};

    $.imageMapProEditorDefaults = {
		id: 0,
		editor: {
			previewMode: 0,
			selected_shape: -1,
			tool: 'spot',
			state: {
				dragging: false
			}
		},
		runtime: {
			is_fullscreen: 0
		},
		general: {
			name: '',
			shortcode: '',
			width: 1280,
			height: 776,
			naturalWidth: 1280,
			naturalHeight: 776,
			responsive: 1,
			preserve_quality: 1,
			pageload_animation: 'none',
			late_initialization: 0,
			center_image_map: 0
		},
		image: {
			url: 'https://webcraftplugins.com/uploads/image-map-pro/demo.jpg',
		},
		fullscreen: {
			enable_fullscreen_mode: 0,
			start_in_fullscreen_mode: 0,
			fullscreen_background: '#000000',
			fullscreen_button_position: 1,
			fullscreen_button_type: 'icon_and_text',
			fullscreen_button_color: '#ffffff',
			fullscreen_button_text_color: '#222222'
		},
		tooltips: {
			sticky_tooltips: 0,
			constrain_tooltips: 1,
			tooltip_animation: 'grow',
			fullscreen_tooltips: 'none', // none / mobile / always,
		}, spots: []
	};
})(jQuery, window, document);