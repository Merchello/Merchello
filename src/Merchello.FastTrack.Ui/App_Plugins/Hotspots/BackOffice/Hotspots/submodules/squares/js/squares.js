/*

TO DO:

- add lots of elements (as many as possible from Bootstrap)
- create test file with empty editor
- create test file for saving/loading editor state
- create test file for editor and content side-by-side
- create how to use guide
- create API docs

*/

// Squares
// Description: Interactive and embeddable HTML content builder.
// Author: Nikolay Dyankov
// License: MIT

/*

Usage

This script is meant to be embedded in a back-end site builder or similar project.
The usage scenario is the following (for now):

1. Add a class "squares" to the containers that should have editable content
2. Call an API to get the current state of the editor to store it.
3. Call an API to get the generated HTML content for the front-end
4. Include the "squares.css" file in the front-end
5. Insert the previously generated HTML code

*/

;(function ($, window, document, undefined) {

    var editorWindow = undefined, registeredElements = new Array(), registeredControls = new Array(), editors = new Array();

    // =========================================================================
    // [API]

    // Create an editor with previously stored settings in JSON format.
    // The "host" parameter is the root element of the editor. It contains
    // (or will contain a reference to the JS class instance).
    $.squaresInitWithSettings = function(host, settings) {
        // If the host already has an editor attached, remove the editor from the editors array
        if (host.data.editor) {
            for (var i=0; i<editors.length; i++) {
                if (editors[i].id == host.data.editor.id) {
                    editors.splice(i, 1);
                }
            }
        }

        // Init the new editor
        var squaresInstance = new Squares(host, settings);
        editors.push(squaresInstance);
    };

    // Gets the current state as JS object of an editor, selected by its host
    $.squaresGetCurrentSettings = function(host) {
        return host.data.editor.getCompressedSettings();
    };

    // Called at the end to generate the final HTML code to be inserted in the
    // front-end.
    $.squaresGenerateHTML = function(host) {
        return host.data.editor.generateHTML();
    };

    /*
    Adds a new element to the catalog.
    Required options for Element registration:
    - name: sematic name for the Element
    - iconClass: complete class name from Font Awesome
    - content(): callback function which returns HTML code to be rendered
    - (optional) extendOptions - array containing additional controls for
    the element. For example:

    */
    $.squaresRegisterElement = function(options) {
        registeredElements.push(options);
    };

    /*
    Registers a control that can be added to the element settings window
    Required options for Control registration:
    - type: int, float, text, color, etc
    - getValue: getter for the value of the control
    - setValue: setter for the value of the control
    - HTML: returns the HTML of the control
    - init: create events associated with this specific control element, etc
    */

    $.squaresRegisterControl = function(options) {
        registeredControls.push(options);
    }

    $.squaresShowEditorWindow = function(x, y) {
        editorWindow.show(x, y);
    }
    $.squaresHideEditorWindow = function() {
        editorWindow.hide();
    }
    $.squaresExtendElementDefaults = function(extendedDefaults) {
        elementDefaultSettings = $.extend(true, {}, elementDefaultSettings, extendedDefaults);
    }

    // [END API]
    // =========================================================================

    $(document).ready(function() {
        // On document load, loop over all elements with the "squares" class
        // and initialize a Squares editor on them.
        $('.squares').each(function() {
            var squaresInstance = new Squares(this);
            editors.push(squaresInstance);
            $(this).data.editor = squaresInstance;
        });

        // Create the editor window


        // Test initWithSettings
        // var s = '{"containers":[{"id":"sq-container-220041","settings":{"elements":[{"settings":{"name":"Heading","iconClass":"fa fa-header"},"options":{"heading":{"heading":"h1"}}}]}},{"id":"sq-container-352351","settings":{"elements":[{"settings":{"name":"Paragraph","iconClass":"fa fa-font"},"options":{"layout":{"column_span":"6"},"text":{"font_size":"18"}}},{"settings":{"name":"Paragraph","iconClass":"fa fa-font"},"options":{"layout":{"column_span":"6"},"style":{"background_color":"#75fa00","opacity":0.6321428571428571,"border_opacity":0.8571428571428571}}},{"settings":{"name":"Button","iconClass":"fa fa-hand-pointer-o"}}]}},{"id":"sq-container-307581","settings":{"elements":[{"settings":{"name":"Image","iconClass":"fa fa-picture-o"}},{"settings":{"name":"Video","iconClass":"fa fa-video-camera"}},{"settings":{"name":"YouTube","iconClass":"fa fa-youtube"}}]}}]}';
        // var s = '{"containers":[{"id":"sq-container-229951","settings":{"elements":[{"settings":{"name":"Heading","iconClass":"fa fa-header"}}]}}]}';
        // var s = '{"containers":[{"id":"sq-container-718651","settings":{"elements":[{"settings":{"name":"Heading","iconClass":"fa fa-header"},"options":{"heading":{"text":"Lorem Ipsum31231","heading":"h2"}}},{"settings":{"name":"Paragraph","iconClass":"fa fa-font"},"options":{"text":{"text":"Pellentes2131231ue habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas."}}}]}}]}';
        // var s = '{"containers":[{"id":"sq-container-298901","settings":{"elements":[{"settings":{"name":"Heading","iconClass":"fa fa-header"}},{"settings":{"name":"Image","iconClass":"fa fa-picture-o"},"options":{"layout":{"column_span":{"lg":{"class":"sq-col-lg-6"}}}}},{"settings":{"name":"Paragraph","iconClass":"fa fa-font"},"options":{"layout":{"column_span":{"lg":{"class":"sq-col-lg-6"}}}}}]}}]}';
        // var s = '{"containers":[{"id":"sq-container-335181","settings":{"elements":[{"settings":{"name":"Heading","iconClass":"fa fa-header"},"options":{"general":{"id":"element-1-id","classes":"some-class","css":"background: red;"},"layout":{"box_model":{"margin":{"top":20,"bottom":20}},"use_grid":0},"font":{"font_family":"serif","font_size":"39","font_style":"italic","line_height":"auto","text_color":"#ffffff","text_align":"center","text_decoration":"underline","text_transform":"uppercase"},"style":{"background_color":"#f5fc58","background_opacity":0.5571428571428572,"opacity":0.29642857142857143,"box_shadow":"0 0 10px black","border_width":2,"border_style":"dashed","border_color":"#00f92b","border_opacity":0.5285714285714286,"border_radius":100},"heading":{"heading":"h1"}}},{"settings":{"name":"Paragraph","iconClass":"fa fa-paragraph"},"options":{"layout":{"column_span":{"lg":{"class":"sq-col-lg-6"}}}}},{"settings":{"name":"Image","iconClass":"fa fa-camera"},"options":{"layout":{"column_span":{"lg":{"class":"sq-col-lg-6"}}}}},{"settings":{"name":"Button","iconClass":"fa fa-link"}}]}}]}';
        // $.squaresInitWithSettings($('.squares').first(), s);
        // $.squaresInitWithSettings($('.squares').first());
    });

    // The bulk of the functionality goes here.
    // Squares is the "root" class.
    var squaresDefaultSettings = {
        containers: []
    };

    function Squares(host, settings) {
        var that = this;

        // "host" is the direct parent of the embedded editor
        this.host = $(host);
        this.id = Math.floor(Math.random() * 9999) + 1;
        this.settings = $.extend(true, {}, squaresDefaultSettings);

        this.contentRoot = undefined;
        this.root = undefined;
        this.Window = undefined;

        // Drag general flags
        this.ix = 0; // initial dragged object x
        this.iy = 0; // initial dragged object x
        this.iex = 0; // initial event x
        this.iey = 0; // initial event y

        // Drag container flags
        this.shouldStartDraggingContainer = false;
        this.didStartDraggingContainer = false;
        this.draggingContainer = false;

        // Drag container vars
        this.draggedContainerIndex = 0;
        this.draggedContainer = undefined;
        this.dummyContainer = undefined;
        this.containerReorderMap = undefined;
        this.newIndexOfDraggedContainer = 0;

        // Reorder elements
        this.shouldStartDraggingElement = false;
        this.didStartDraggingElement = false;
        this.draggingElement = false;
        this.draggedElementIndex = -1;
        this.draggedElementContainerIndex = -1;
        this.elementDragMap = undefined;
        this.dummyElement = undefined;
        this.newIndexOfDraggedElement = -1;
        this.draggedElementWidth = -1;

        // Selected Element ID
        this.selectedElementID = undefined;

        this.loadSettings(settings);
        this.init();
    };
    Squares.prototype.loadSettings = function(settings) {
        // When settings are loaded, we make sure containers and elements
        // have the correct prototype.
        if (settings) {
            // Iterate over all containers
            if (settings.containers) {
                for (var i=0; i<settings.containers.length; i++) {
                    var c = settings.containers[i];

                    // Add a container and store a reference
                    var newContainer = this.appendContainer();

                    // Iterate over all elements of the container
                    if (c.settings) {
                        for (var j=0; j<c.settings.elements.length; j++) {
                            var e = c.settings.elements[j];

                            // Get the catalog index of the element with the same name
                            // and insert it in the container
                            for (var k=0; k<registeredElements.length; k++) {
                                if (e.settings.name == registeredElements[k].name) {
                                    newContainer.insertElement(k, j, e.options);
                                }
                            }
                        }
                    }
                }
            }
        }
    };
    Squares.prototype.init = function () {
        // Save a reference in the host to the Editor
        this.host.data.editor = this;

        // Insert a container to hold everything
        this.host.html('');
        this.host.append('<div class="sq-root-container"></div>');
        this.root = this.host.find('.sq-root-container');

        // Insert a container to hold all the user generated content
        this.host.find('.sq-root-container').append('<div class="sq-content"></div>');
        this.contentRoot = this.host.find('.sq-content');

        this.contentRoot.attr('id', 'sq-editor-' + this.id);

        this.addUI();
        this.addEvents();
        this.redraw();

        // Editor window
        editorWindow = new EditorWindow();
        editorWindow.hide();
    };
    Squares.prototype.redraw = function () {
        // This is the global redraw function.
        // It is called only when a change in hierarchy is made.
        // It is responsible for creating the root element for each
        //      container and element, telling those objects that they have a new
        //      root element, and calling the "render" function on them.

        this.contentRoot.html('');

        for (var i=0; i<this.settings.containers.length; i++) {
            var c = this.settings.containers[i];

            // Append a container
            var html = '<div class="sq-container" data-index="'+ i +'" id="'+ c.id +'"></div>';

            this.contentRoot.append(html);

            // Set the container's "root" object
            var containerRoot = $('#' + c.id);

            // Call the render() function of the container
            c.render();
            c.appendEditorControls();

            for (var j=0; j<c.settings.elements.length; j++) {
                var e = c.settings.elements[j];

                // Append an element to the container
                var html = '<div class="sq-element" data-index="' + j + '" id="'+ e.id +'"></div>';
                containerRoot.append(html);

                // Call the render() function of the element
                e.render();
                e.appendEditorControls();
            }

            containerRoot.append('<div class="squares-clear"></div>');
        }

        // If there are no containers, hide the "elements button"
        if (this.settings.containers.length == 0) {
            this.root.find('.sq-add-elements').hide();
        } else {
            this.root.find('.sq-add-elements').show();
        }

        // Re-select the currently selected element
        if (this.selectedElementID) {
            this.selectElement(this.selectedElementID);
        }
    };
    Squares.prototype.addEvents = function() {
        var self = this;

        // Button for appending a new container
        this.host.find('.sq-add-container').off('click');
        this.host.find('.sq-add-container').on('click', function() {
            self.appendContainer();
            self.redraw();
        });

        // Delete container button
        $(document).off('mouseout', '#sq-editor-' + this.id + ' .sq-container');
        $(document).on('mouseout', '#sq-editor-' + this.id + ' .sq-container', function(e) {
            if ($(e.target).closest('.sq-container-confirm-delete').length == 0 && !$(e.target).hasClass('sq-container-confirm-delete') &&
                $(e.target).closest('.sq-container-delete').length == 0 && !$(e.target).hasClass('sq-container-delete')) {
                $('.sq-container-confirm-delete').hide();
            }
        });
        $(document).off('click', '#sq-editor-' + this.id + ' .sq-container-delete');
        $(document).on('click', '#sq-editor-' + this.id + ' .sq-container-delete', function() {
            $(this).siblings('.sq-container-confirm-delete').show();
        });
        $(document).off('click', '#sq-editor-' + this.id + ' .sq-container-confirm-delete');
        $(document).on('click', '#sq-editor-' + this.id + ' .sq-container-confirm-delete', function() {
            self.deleteContainer($(this).data('container-id'));
            self.redraw();
        });

        // Reorder containers and elements functionality

        // Containers
        $(document).off('mousedown', '#sq-editor-'+ self.id +' .sq-container-move');
        $(document).on('mousedown', '#sq-editor-'+ self.id +' .sq-container-move', function(e) {
            // If there is just one container, then don't do anything
            if (self.settings.containers.length <= 1) return;

            self.iex = e.pageX;
            self.iey = e.pageY;
            self.shouldStartDraggingContainer = true;
            self.draggedContainerIndex = $(e.target).closest('.sq-container').data('index');
            self.draggedContainer = self.host.find('.sq-container[data-index='+ self.draggedContainerIndex +']');
        });


        // Elements
        $(document).off('mousedown', '#sq-editor-'+ self.id +' .sq-element');
        $(document).on('mousedown', '#sq-editor-'+ self.id +' .sq-element', function(e) {
            // If there is just one container with one element, then don't do anything
            if (self.settings.containers.length == 1 && self.settings.containers[0].settings.elements.length == 1) return;

            self.iex = e.pageX;
            self.iey = e.pageY;
            self.shouldStartDraggingElement = true;
            self.draggedElement = $(this);
            self.draggedElementIndex = $(this).data('index');
            self.draggedElementContainerIndex = $(this).closest('.sq-container').data('index');
        });

        $(document).off('mousemove.'+ self.id);
        $(document).on('mousemove.'+ self.id, function(e) {
            // Drag container
            if (self.shouldStartDraggingContainer && !self.didStartDraggingContainer) {
                self.startDraggingContainer(e);
            }

            if (self.draggingContainer) {
                self.dragContainer(e);
            }

            // Drag element
            if (self.shouldStartDraggingElement && !self.didStartDraggingElement) {
                self.startDraggingElement(e);
            }

            if (self.draggingElement) {
                self.dragElement(e);
            }
        });
        $(document).off('mouseup.'+ self.id);
        $(document).on('mouseup.'+ self.id, function(e) {
            if (self.draggingContainer) {
                self.endDraggingContainer(e);
            }
            if (self.draggingElement) {
                self.endDraggingElement(e);
            }

            // Clean up
            self.shouldStartDraggingContainer = false;
            self.didStartDraggingContainer = false;
            self.draggingContainer = false;

            self.draggedContainerIndex = 0;
            self.draggedContainer = undefined;
            self.dummyContainer = undefined;

            self.shouldStartDraggingElement = false;
            self.didStartDraggingElement = false;
            self.draggingElement = false;
            self.draggedElementIndex = -1;
            self.draggedElementContainerIndex = -1;
        });

        // [end] Reorder containers functionality

        // Delete element button
        $(document).off('click.' + this.id, '#sq-delete-element-button');
        $(document).on('click.' + this.id, '#sq-delete-element-button', function() {
            var elementID = $(this).data('element-id');

            // Search for the element
            for (var i=0; i<self.settings.containers.length; i++) {
                var c = self.settings.containers[i];

                for (var j=0; j<c.settings.elements.length; j++) {
                    if (c.settings.elements[j].id == elementID) {
                        c.removeElementAtIndex(j);
                        self.redraw();
                    }
                }
            }
        });
    };
    Squares.prototype.startDraggingContainer = function(e) {
        if (Math.abs(e.pageX - this.iex) > 5 || Math.abs(e.pageY - this.iey) > 5) {
            this.draggingContainer = true;
            this.didStartDraggingContainer = true;

            // Create a virtual map of the current containers, where
            // every possible position of the dragged container is
            // precalculated
            this.containerReorderMap = new Array();
            var draggedContainerY = this.draggedContainer.outerHeight()/2;

            for (var i=0; i<this.settings.containers.length; i++) {
                var y = draggedContainerY;

                // Add the height of all previous containers to calculate
                // the new virtual Y position of the dragged container
                // for the current index
                for (var j=i-1; j>=0; j--) {
                    var index = j;

                    // The height of the dragged container must not be
                    // included in the calculation.
                    // If the current index is the index of the dragged
                    // container, then increase the index
                    if (j >= this.draggedContainerIndex) {
                        index++;
                    }

                    var c = this.host.find('.sq-container[data-index='+ index +']');
                    y += c.outerHeight();
                }

                this.containerReorderMap.push(y);
            }

            // Position the container absolutely
            this.ix = this.draggedContainer.position().left;
            this.iy = this.draggedContainer.position().top;

            this.draggedContainer.css({
                position: 'absolute',
                left: this.ix,
                top: this.iy,
                width: this.draggedContainer.width()
            });

            this.draggedContainer.addClass('sq-dragging');

            // Insert a dummy container
            this.draggedContainer.after('<div id="sq-dummy-container"></div>');
            this.dummyContainer = $('#sq-dummy-container');
            this.dummyContainer.css({
                width: this.draggedContainer.outerWidth(),
                height: this.draggedContainer.outerHeight()
            });
        }
    }
    Squares.prototype.dragContainer = function(e) {
        this.draggedContainer.css({
            left: this.ix + e.pageX - this.iex,
            top: this.iy + e.pageY - this.iey
        });

        var y = this.draggedContainer.position().top + this.draggedContainer.outerHeight()/2;
        var closestDeltaY = 999999;
        var closestIndex = undefined;

        for (var i=0; i<this.containerReorderMap.length; i++) {
            if (Math.abs(y - this.containerReorderMap[i]) < closestDeltaY) {
                closestDeltaY = Math.abs(y - this.containerReorderMap[i]);
                closestIndex = i;
            }
        }

        // If the closest index changed, move the dummy container to the
        // new position.
        if (closestIndex != this.newIndexOfDraggedContainer) {
            this.newIndexOfDraggedContainer = closestIndex;

            this.dummyContainer.remove();

            if (this.newIndexOfDraggedContainer < this.draggedContainerIndex) {
                this.host.find('.sq-container[data-index='+ this.newIndexOfDraggedContainer +']').before('<div id="sq-dummy-container"></div>');
            } else {
                this.host.find('.sq-container[data-index='+ this.newIndexOfDraggedContainer +']').after('<div id="sq-dummy-container"></div>');
            }

            this.dummyContainer = $('#sq-dummy-container');
            this.dummyContainer.css({
                width: this.draggedContainer.outerWidth(),
                height: this.draggedContainer.outerHeight()
            });
        }
    }
    Squares.prototype.endDraggingContainer = function(e) {
        // Switch places of containers
        if (this.draggedContainerIndex != this.newIndexOfDraggedContainer) {
            var a = this.settings.containers[this.draggedContainerIndex];
            this.settings.containers.splice(this.draggedContainerIndex, 1);
            this.settings.containers.splice(this.newIndexOfDraggedContainer, 0, a);
        }

        this.redraw();
    }
    Squares.prototype.startDraggingElement = function(e) {
        if (Math.abs(e.pageX - this.iex) > 5 || Math.abs(e.pageY - this.iey) > 5) {
            this.draggingElement = true;
            this.didStartDraggingElement = true;

            // Save the starting posiiton of the draggedElement
            this.ix = this.draggedElement.offset().left;
            this.iy = this.draggedElement.offset().top;

            // Create a virtual map of all possible positions of the element
            // in each container
            this.elementDragMap = new Array();

            var draggedElementObject = this.settings.containers[this.draggedElementContainerIndex].settings.elements[this.draggedElementIndex];

            this.draggedElementWidth = getWidthOfElementInGrid(draggedElementObject.controls['layout']['column_span'].getVal());
            this.draggedElementWidth = this.draggedElement.outerWidth();

            var dummyElementHTML = '<div id="sq-dummy-element-tmp" style="width: '+ this.draggedElementWidth +'px; height: '+ this.draggedElement.outerHeight() +'px;"></div>';

            this.draggedElement.hide();
            for (var i=0; i<this.settings.containers.length; i++) {
                var c = this.settings.containers[i];

                // If the container doesn't have any elements, insert just one
                // dummy element and move on to next container
                if (c.settings.elements.length == 0) {
                    this.host.find('.sq-container[data-index='+i+']').append(dummyElementHTML);
                    var el = $('#sq-dummy-element-tmp');
                    this.elementDragMap.push({ x: el.offset().left + el.width()/2, y: el.offset().top + el.height()/2, containerIndex: i, elementIndex: 0 });
                    $('#sq-dummy-element-tmp').remove();
                }

                for (var j=0; j<c.settings.elements.length; j++) {
                    this.host.find('.sq-container[data-index='+i+']').find('.sq-element[data-index='+j+']').before(dummyElementHTML);
                    var el = $('#sq-dummy-element-tmp');
                    this.elementDragMap.push({ x: el.offset().left + el.width()/2, y: el.offset().top + el.height()/2, containerIndex: i, elementIndex: j });
                    $('#sq-dummy-element-tmp').remove();

                    if (j == c.settings.elements.length - 1) {
                        this.host.find('.sq-container[data-index='+i+']').find('.sq-element[data-index='+j+']').after(dummyElementHTML);
                        var el = $('#sq-dummy-element-tmp');
                        this.elementDragMap.push({ x: el.offset().left + el.width()/2, y: el.offset().top + el.height()/2, containerIndex: i, elementIndex: j + 1 });
                        $('#sq-dummy-element-tmp').remove();
                    }
                }
            }

            this.draggedElement.show();

            // Insert a dummy element
            this.draggedElement.after('<div id="sq-dummy-element"><div id="sq-dummy-element-inner"></div></div>');
            this.dummyElement = $('#sq-dummy-element');
            this.dummyElement.css({
                width: this.draggedElementWidth,
                height: this.draggedElement.outerHeight(),
                margin: this.draggedElement.css('margin'),
                padding: 0
            });

            // Position the element absolutely

            var draggedElementWidth = this.draggedElement.width();
            var draggedElementHeight = this.draggedElement.height();
            var draggedElementHTML = this.draggedElement.clone().attr('id', 'sq-dragged-element').wrap('<div>').parent().html();

            this.draggedElement.hide();

            $('body').prepend(draggedElementHTML);
            this.draggedElement = $('#sq-dragged-element');

            this.draggedElement.css({
                position: 'absolute',
                left: this.ix,
                top: this.iy,
                width: draggedElementWidth,
                height: draggedElementHeight
            });
            this.draggedElement.addClass('sq-dragging');
        }
    }
    Squares.prototype.dragElement = function(e) {
        this.draggedElement.css({
            left: this.ix + e.pageX - this.iex,
            top: this.iy + e.pageY - this.iey
        });

        // Find the closest virtual position to the mouse position
        var closestIndex = 0;
        var closestDistance = 999999;

        for (var i=0; i<this.elementDragMap.length; i++) {
            var d = Math.abs(e.pageX - this.elementDragMap[i].x) + Math.abs(e.pageY - this.elementDragMap[i].y);
            if (d < closestDistance) {
                closestDistance = d;
                closestIndex = i;
            }
        }

        if (closestIndex != this.newIndexOfDraggedElement) {
            this.newIndexOfDraggedElement = closestIndex;

            // Remove the current dummy element
            $('#sq-dummy-element').remove();

            // Insert a new dummy element at the container/element index
            var containerIndex = this.elementDragMap[this.newIndexOfDraggedElement].containerIndex;
            var elementIndex = this.elementDragMap[this.newIndexOfDraggedElement].elementIndex;
            var c = this.host.find('.sq-container[data-index='+ containerIndex +']');
            // If the index of the dummy element is bigger than the number
            // of elements in that container, insert the dummy at the end
            if (this.settings.containers[containerIndex].settings.elements.length == 0) {
                c.prepend('<div id="sq-dummy-element"><div id="sq-dummy-element-inner"></div></div>');
            } else if (elementIndex == this.settings.containers[containerIndex].settings.elements.length) {
                var lastElementIndex = this.settings.containers[containerIndex].settings.elements.length - 1;
                var el = c.find('.sq-element[data-index='+ lastElementIndex +']');
                el.after('<div id="sq-dummy-element"><div id="sq-dummy-element-inner"></div></div>');
            } else {
                var el = c.find('.sq-element[data-index='+ elementIndex +']');
                el.before('<div id="sq-dummy-element"><div id="sq-dummy-element-inner"></div></div>');
            }

            this.dummyElement = $('#sq-dummy-element');
            this.dummyElement.css({
                width: this.draggedElementWidth,
                height: this.draggedElement.outerHeight(),
                margin: this.draggedElement.css('margin'),
            });
        }
    }
    Squares.prototype.endDraggingElement = function(e) {
        this.draggedElement.remove();

        // Move the element to the new index
        var newContainerIndex = this.elementDragMap[this.newIndexOfDraggedElement].containerIndex;
        var newElementIndex = this.elementDragMap[this.newIndexOfDraggedElement].elementIndex;

        var oldElementIndex = this.draggedElementIndex;
        var oldContainerIndex = this.draggedElementContainerIndex;

        var el = this.settings.containers[oldContainerIndex].settings.elements[oldElementIndex];
        this.settings.containers[oldContainerIndex].settings.elements.splice(oldElementIndex, 1);
        this.settings.containers[newContainerIndex].settings.elements.splice(newElementIndex, 0, el);

        this.redraw();
    }
    Squares.prototype.addUI = function() {
        this.appendAddContainerButton();
        this.appendAddElementsButton();
    };
    Squares.prototype.appendAddContainerButton = function() {
        var addContainerButtonHTML = '<div class="sq-add-container"><i class="fa fa-plus"></i> <span>Add Container</span></div>';

        this.root.append(addContainerButtonHTML);
    };
    Squares.prototype.appendAddElementsButton = function() {
        var addElementsButtonHTML = '<div class="sq-add-elements"><i class="fa fa-cube"></i></div>';

        this.root.append(addElementsButtonHTML);
    };
    Squares.prototype.appendContainer = function() {
        var c = new Container();
        this.settings.containers.push(c);

        return c;
    };
    Squares.prototype.deleteContainer = function(id) {
        // Find out the index of the container
        var index = 0;

        for (var i=0; i<this.settings.containers.length; i++) {
            if (this.settings.containers[i].id == id) {
                index = i;
            }
        }

        this.settings.containers.splice(index, 1);
    };
    Squares.prototype.addElement = function(containerIndex, elementIndex, elementCatalogIndex, elementControlOptions) {
        var self = this;

        // Add element to container at index
        self.settings.containers[containerIndex].insertElement(elementCatalogIndex, elementIndex, elementControlOptions);

        // Redraw
        self.redraw();
    };
    Squares.prototype.getCompressedSettings = function() {
        var settings = $.extend(true, {}, this.settings);

        // Compress element settings
        for (var i=0; i<settings.containers.length; i++) {
            // var c = $.extend(true, {}, settings.containers[i]);
			var c = {
				id: settings.containers[i].id,
				settings: $.extend(true, {}, settings.containers[i].settings)
			};

            for (var j=0; j<c.settings.elements.length; j++) {
                var e = $.extend(true, {}, c.settings.elements[j]);

                e.settings = subtract(e.settings, elementDefaultSettings);
                e.settings = clean(e.settings);

                // Get the current values of the controls
                var options = e.getCurrentOptions();
                options = subtract(options, e.defaults);
                options = clean(options);

                c.settings.elements[j] = {
                    settings: e.settings,
                    options: options
                };
            }

            settings.containers[i] = c;
        }

        return settings;
    }
    Squares.prototype.generateHTML = function() {
        // function generating the HTML code that will be used in the end product

        var html = '';

        for (var i=0; i<this.settings.containers.length; i++) {
            var c = this.settings.containers[i];

            html += c.generateHTML();
        }

        // Strip slashes
        html = html.replace(/\\(.)/mg, "$1");

        // Replace line breaks with <br>
        html = html.replace(/\n/mg, "<br>");

        return html;
    }
    Squares.prototype.selectElement = function(elementID) {
        this.selectedElementID = elementID;

        $('.sq-element-selected').removeClass('sq-element-selected');
        $('#' + this.selectedElementID).addClass('sq-element-selected');
    }

    // The "Container" class servs literally as a container
    // for Element objects, similar to Bootstrap's "row" class.
    // It will have settings only for layout.

    var containerDefaultSettings = {
        elements: []
    };

    function Container() {
        // this.root is the highest element in the container's hierarchy.
        // it will contain data-index attribute, used to reference this element
        this.id = 'sq-container-' + Math.floor(Math.random() * 99999) + 1;

        this.settings = $.extend(true, {}, containerDefaultSettings);
    }
    Container.prototype.insertElement = function(elementCatalogIndex, index, options) {
        var e = new Element(registeredElements[elementCatalogIndex], options);
        this.settings.elements.splice(index, 0, e);
    }
    Container.prototype.removeElementAtIndex = function(i) {
        this.settings.elements.splice(i, 1);
        editorWindow.openFirstTab();
        editorWindow.removeElementSettings();
    }
    Container.prototype.render = function() {
        // Nothing to render for now
    }
    Container.prototype.appendEditorControls = function() {
        var html = '';
        html += '     <div class="sq-container-move"></div>';
        html += '     <div class="sq-container-delete"><i class="fa fa-times" aria-hidden="true"></i></div>';
        html += '     <div class="sq-container-confirm-delete" data-container-id="'+ this.id +'">Delete</div>';

        $('#' + this.id).append(html);
    }
    Container.prototype.generateHTML = function() {
        // function generating the HTML code that will be used in the end product

        var html = '';

        html += '<div class="squares-container">';

        for (var i=0; i<this.settings.elements.length; i++) {
            var e = this.settings.elements[i];
            html += e.generateHTML();
        }

        html += '<div class="squares-clear"></div>'
        html += '</div>';

        return html;
    }

    // The element object will represent a single piece of content.
    // Image, text, video, etc.
    // It will have settings for layout and styling

    var elementDefaultSettings = {
        name: 'Untitled Element',
        iconClass: 'fa fa-cube',
        controls: [],
        defaultControls: {
            layout: {
                box_model: {
                    name: 'Box Model',
                    type: 'box model',
                    default: {
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
                    }
                },
                use_grid: {
                    name: 'Use Grid System',
                    type: 'switch',
                    default: 1
                },
                column_span: {
                    name: 'Grid Settings',
                    type: 'grid system',
                    group: 'Layout Grid',
                    default: {
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
                    }
                },
                width: {
                    name: 'Width',
                    type: 'int',
                    group: 'Layout Manual',
                    default: '100'
                },
                auto_width: {
                    name: 'Auto Width',
                    type: 'switch',
                    group: 'Layout Manual',
                    default: 1
                },
                height: {
                    name: 'Height',
                    type: 'int',
                    group: 'Layout Manual',
                    default: '100'
                },
                auto_height: {
                    name: 'Auto Height',
                    type: 'switch',
                    group: 'Layout Manual',
                    default: 1
                }
            },
            style: {
                background_color: {
                    name: 'Background Color',
                    type: 'color',
                    default: '#ffffff'
                },
                background_opacity: {
                    name: 'Background Opacity',
                    type: 'slider',
                    options: {
                        min: 0,
                        max: 1
                    },
                    default: '0'
                },
                opacity: {
                    name: 'Opacity',
                    type: 'slider',
                    options: {
                        min: 0,
                        max: 1
                    },
                    default: '1'
                },
                box_shadow: {
                    name: 'Box Shadow',
                    type: 'text',
                    default: 'none'
                },
                border_width: {
                    name: 'Border Width',
                    type: 'slider',
                    options: { min: 0, max: 20, type: 'int' },
                    default: '0'
                },
                border_style: {
                    name: 'Border Style',
                    type: 'select',
                    options: [ 'none', 'hidden', 'dotted', 'dashed', 'solid', 'double', 'groove', 'ridge', 'inset', 'outset' ],
                    default: 'none'
                },
                border_color: {
                    name: 'Border Color',
                    type: 'color',
                    default: '#000000'
                },
                border_opacity: {
                    name: 'Border Opacity',
                    type: 'slider',
                    options: {
                        min: 0,
                        max: 1
                    },
                    default: '1'
                },
                border_radius: {
                    name: 'Border Radius',
                    type: 'slider',
                    options: { min: 0, max: 100, type: 'int' },
                    default: '0'
                },
            },
            font: {
                font_family: {
                    name: 'Font Family',
                    type: 'text',
                    default: 'sans-serif'
                },
                font_size: {
                    name: 'Font Size',
                    type: 'text',
                    format: 'int',
                    default: '14'
                },
                font_weight: {
                    name: 'Font Weight',
                    type: 'text',
                    default: 'normal'
                },
                font_style: {
                    name: 'Font Style',
                    type: 'select',
                    options: [ 'normal', 'italic', 'oblique', 'initial', 'inherit' ],
                    default: 'normal'
                },
                line_height: {
                    name: 'Line Height',
                    type: 'text',
                    format: 'int',
                    default: '22'
                },
                text_color: {
                    name: 'Text Color',
                    type: 'color',
                    default: '#000000'
                },
                text_align: {
                    name: 'Text Align',
                    type: 'select',
                    options: ['left', 'right', 'center', 'justify', 'justify-all', 'start', 'end', 'match-parent', 'inherit', 'initial', 'unset'],
                    default: 'left'
                },
                text_decoration: {
                    name: 'Text Decoration',
                    type: 'select',
                    options: ['none', 'underline'],
                    default: 'none'
                },
                text_transform: {
                    name: 'Text Transform',
                    type: 'select',
                    options: [ 'none', 'capitalize', 'uppercase', 'lowercase', 'initial', 'inherit' ],
                    default: 'none'
                },
                text_shadow: {
                    name: 'Text Shadow',
                    type: 'text',
                    default: ''
                }
            },
            general: {
                id: {
                    name: 'ID',
                    type: 'text',
                    default: ''
                },
                classes: {
                    name: 'Classes',
                    type: 'text',
                    default: ''
                },
                css: {
                    name: 'CSS',
                    type: 'text',
                    default: ''
                }
            }
        },
        defaultControlGroupIcons: {
            general: 'fa fa-wrench',
            layout: 'fa fa-th-large',
            font: 'fa fa-font',
            style: 'fa fa-paint-brush'
        },
        content: function() {
            return '';
        }
    };

    function Element(settings, options) {
        // this.root is the highest element in the container's hierarchy.
        // it will contain data-index attribute, used to reference this element
        this.id = 'sq-element-' + Math.floor(Math.random() * 99999) + 1;

        // Settings are used only for initialization
        this.settings = $.extend(true, {}, elementDefaultSettings, settings);

        // This array will contain only the default values for each option and
        // it will be used only for compressing the generated JSON
        this.defaults = new Array();

        // Array containing all control objects
        // all options of this element should be accessed from here
        this.controls = new Array();

        // Create a reference to the content() function, so 'this' within that function
        // refers to the Element object and it has access to its controls
        this.content = this.settings.content;

        // Temporary variable until a better solution is found
        this.fontStyles = '';

        this.init(options);
    }
    Element.prototype.init = function(options) {
        // Merge the custom controls with the default controls
        this.settings.controls = $.extend(true, {}, this.settings.controls, this.settings.defaultControls);

        // Merge the custom control group icons with the default control group icons
        this.settings.controlGroupIcons = $.extend(true, {}, this.settings.controlGroupIcons, this.settings.defaultControlGroupIcons);

        // Remove the default style controls if the option is specified
        if (this.settings.useStyleControls === false) {
            this.settings.controls.style = undefined;
        }
        // Remove the default text style controls if the option is specified
        if (this.settings.useFontControls === false) {
            this.settings.controls.font = undefined;
        }

        // Create associative array from this.settings.controls containing default values
        // Used only for compression
        for (var g in this.settings.controls) {
            if (this.settings.controls.hasOwnProperty(g)) {
                var group = this.settings.controls[g];

                if (!this.defaults[g]) {
                    this.defaults[g] = {};
                }

                for (var op in group) {
                    if (group.hasOwnProperty(op)) {
                        var option = group[op];

                        this.defaults[g][op] = option.default;
                    }
                }
            }
        }

        // Create controls
        for (var g in this.settings.controls) {
            if (this.settings.controls.hasOwnProperty(g)) {
                var group = this.settings.controls[g];

                for (var op in group) {
                    if (group.hasOwnProperty(op)) {
                        var option = group[op];

                        // Get a control from the registered controls
                        // of the corresponding type
                        var controlOptions = undefined;

                        for (var i=0; i<registeredControls.length; i++) {
                            if (registeredControls[i].type == option.type) {
                                controlOptions = registeredControls[i];
                            }
                        }

                        // Check if there is a value in the init options
                        var v = option.default;

                        if (options !== undefined && options[g] !== undefined && options[g][op] !== undefined) {
                            if (typeof(options[g][op]) == 'object') {
                                v = $.extend(true, {}, option.default, options[g][op]);
                            } else {
                                v = options[g][op];
                            }
                        }

                        if (this.controls[g] === undefined) {
                            this.controls[g] = {};
                        }

                        var self = this;

                        this.controls[g][op] = new SquaresControl(controlOptions, option.name, option.group, g, option.options, function() {
                            self.updateForm();
                            self.render();
                            self.appendEditorControls();
                        });

                        this.controls[g][op].setVal(v);
                    }
                }
            }
        }
    }
    Element.prototype.getSettingsForm = function() {
        // Loop over all controls and get the HTML from each control
        // Also add a label with the name of the control
        var html = '';

        // Create tabs
        html += '<div id="sq-window-settings-sidebar">';
        var groupCount = 0;
        for (var g in this.controls) {
            var icon = '<i class="fa fa-toggle-on" aria-hidden="true"></i>';

            if (this.settings.controlGroupIcons[g]) {
                icon = '<i class="'+ this.settings.controlGroupIcons[g] +'" aria-hidden="true"></i>';
            }

            html += '<div class="sq-window-settings-sidebar-button" data-tab-index="'+ groupCount +'" data-tab-group="sq-element-settings-tab-group" data-tab-button>';
            html += '   <div class="sq-window-settings-sidebar-button-icon">'+ icon +'</div>';
            html += '   <div class="sq-window-settings-sidebar-button-title">'+ g +'</div>';
            html += '</div>';
            groupCount++;
        }

        // Append delete element tab button
        html += '<div class="sq-window-settings-sidebar-button" data-tab-index="'+ groupCount +'" data-tab-group="sq-element-settings-tab-group" data-tab-button>';
        html += '   <div class="sq-window-settings-sidebar-button-icon"><i class="fa fa-trash-o" aria-hidden="true"></i></div>';
        html += '   <div class="sq-window-settings-sidebar-button-title">Delete</div>';
        html += '</div>';

        html += '</div>';


        // Create content for each tab
        html += '<div class="sq-settings-window-content-wrap">';

        var groupCount = 0;
        for (var g in this.controls) {
            html += '<div class="sq-window-content" data-tab-content data-tab-index="'+ groupCount +'" data-tab-group="sq-element-settings-tab-group">';

            var tabGroup = this.controls[g];
            groupCount++;

            for (var c in tabGroup) {
                var control = tabGroup[c];

                html += '<div class="sq-form-control '+ control.elementClass +'">';

                if (control.customLabel) {
                    html += control.HTML();
                } else {
                    html += '<label for="'+ control.elementID +'">'+ control.name +'</label>';
                    html += control.HTML();
                }

                html += '</div>';
            }

            html += '</div>';
        }

        // Create content for the delete element tab
        html += '<div class="sq-window-content" data-tab-content data-tab-index="'+ groupCount +'" data-tab-group="sq-element-settings-tab-group">';
        html += '   <div class="sq-form-control">';
        html += '       <p>Delete Element?</p>';
        html += '       <div id="sq-delete-element-button" data-element-id="'+ this.id +'">Delete</div>';
        html += '   </div>';
        html += '</div>';

        html += '</div>';

        return html;
    }
    Element.prototype.loadOptions = function() {
        // Load the current options of the element in the settings window

        for (var g in this.controls) {
            var tabGroup = this.controls[g];

            for (var c in tabGroup) {
                var control = tabGroup[c];
                control.loadVal();
            }
        }

        this.updateForm();
    }
    Element.prototype.updateForm = function() {
        if (this.controls['layout']['use_grid'].getVal() == 1) {
            $('.' + this.controls['layout']['width'].elementClass).hide();
            $('.' + this.controls['layout']['column_span'].elementClass).show();
        } else {
            $('.' + this.controls['layout']['width'].elementClass).show();
            $('.' + this.controls['layout']['column_span'].elementClass).hide();
        }
    }
    Element.prototype.generateStyles = function() {
        var css = '';
        // =====================================================================
        // Layout
        // =====================================================================

        var o = this.controls['layout'];

        // Box Model
        if (isNumeric(o['box_model'].getVal().margin.top)) {
            css += 'margin-top: ' + o['box_model'].getVal().margin.top + 'px; ';
        }
        if (isNumeric(o['box_model'].getVal().margin.bottom)) {
            css += 'margin-bottom: ' + o['box_model'].getVal().margin.bottom + 'px; ';
        }
        if (isNumeric(o['box_model'].getVal().margin.left)) {
            css += 'margin-left: ' + o['box_model'].getVal().margin.left + 'px; ';
        }
        if (isNumeric(o['box_model'].getVal().margin.right)) {
            css += 'margin-right: ' + o['box_model'].getVal().margin.right + 'px; ';
        }

        if (isNumeric(o['box_model'].getVal().padding.top)) {
            css += 'padding-top: ' + o['box_model'].getVal().padding.top + 'px; ';
        }
        if (isNumeric(o['box_model'].getVal().padding.bottom)) {
            css += 'padding-bottom: ' + o['box_model'].getVal().padding.bottom + 'px; ';
        }
        if (isNumeric(o['box_model'].getVal().padding.left)) {
            css += 'padding-left: ' + o['box_model'].getVal().padding.left + 'px; ';
        }
        if (isNumeric(o['box_model'].getVal().padding.right)) {
            css += 'padding-right: ' + o['box_model'].getVal().padding.right + 'px; ';
        }

        if (parseInt(o['use_grid'].getVal(), 10) == 1) {
            // Grid system

        } else {
            // Width
            if (parseInt(o['auto_width'].getVal(), 10) == 1) {
                css += 'width: auto; ';
            } else {
                if (o['width'].getVal() !== '' && !isNaN(o['width'].getVal())) {
                    css += 'width: '+ o['width'].getVal() +'px; ';
                }
            }

            // Height
            if (parseInt(o['auto_height'].getVal(), 10) == 1) {
                css += 'height: auto; ';
            } else {
                if (o['height'].getVal() !== '' && !isNaN(o['height'].getVal())) {
                    css += 'height: '+ o['height'].getVal() +'px; ';
                }
            }
        }

        css += 'float: left; ';

        // =====================================================================
        // Text
        // =====================================================================
        var o = this.controls['font'];

        if (o) {
            // Font Family
            if (o['font_family'].getVal() !== '') {
                css += 'font-family: ' + o['font_family'].getVal() + '; ';
                this.fontStyles += 'font-family: ' + o['font_family'].getVal() + '; ';
            }

            // Font Size
            if (isNumeric(o['font_size'].getVal())) {
                css += 'font-size: ' + o['font_size'].getVal() + 'px; ';
                this.fontStyles += 'font-size: ' + o['font_size'].getVal() + 'px; ';
            }

            // Font Weight
            if (o['font_weight'].getVal() !== '') {
                css += 'font-weight: ' + o['font_weight'].getVal() + '; ';
                this.fontStyles += 'font-weight: ' + o['font_weight'].getVal() + '; ';
            }

            // Font Style
            if (o['font_style'].getVal() !== '') {
                css += 'font-style: ' + o['font_style'].getVal() + '; ';
                this.fontStyles += 'font-style: ' + o['font_style'].getVal() + '; ';
            }

            // Line Height
            if (isNumeric(o['line_height'].getVal())) {
                css += 'line-height: ' + o['line_height'].getVal() + 'px; ';
                this.fontStyles += 'line-height: ' + o['line_height'].getVal() + 'px; ';
            }

            // Text Color
            if (o['text_color'].getVal() !== '') {
                css += 'color: ' + o['text_color'].getVal() + '; ';
                this.fontStyles += 'color: ' + o['text_color'].getVal() + '; ';
            }

            // Text Align
            if (o['text_align'].getVal() !== '') {
                css += 'text-align: ' + o['text_align'].getVal() + '; ';
                this.fontStyles += 'text-align: ' + o['text_align'].getVal() + '; ';
            }

            // Text Decoration
            if (o['text_decoration'].getVal() !== '') {
                css += 'text-decoration: ' + o['text_decoration'].getVal() + '; ';
                this.fontStyles += 'text-decoration: ' + o['text_decoration'].getVal() + '; ';
            }

            // Text Transform
            if (o['text_transform'].getVal() !== '') {
                css += 'text-transform: ' + o['text_transform'].getVal() + '; ';
                this.fontStyles += 'text-transform: ' + o['text_transform'].getVal() + '; ';
            }

            // Text Shadow
            if (o['text_shadow'].getVal() !== '') {
                css += 'text-shadow: ' + o['text_shadow'].getVal() + '; ';
                this.fontStyles += 'text-shadow: ' + o['text_shadow'].getVal() + '; ';
            }
        }

        // =====================================================================
        // Style
        // =====================================================================
        var o = this.controls['style'];

        if (o) {
            // Background Color
            var c_bg = hexToRgb(o['background_color'].getVal());
            css += 'background-color: rgba('+ c_bg.r +', '+ c_bg.g +', '+ c_bg.b +', '+ o['background_opacity'].getVal() +'); ';

            // Opacity
            if (isNumeric(o['opacity'].getVal())) {
                css += 'opacity: ' + o['opacity'].getVal() + '; ';
            }

            // Box Shadow
            if (o['box_shadow'].getVal() !== '') {
                css += 'box-shadow: ' + o['box_shadow'].getVal() + '; ';
            }

            // Border Width
            if (isNumeric(o['border_width'].getVal())) {
                css += 'border-width: ' + o['border_width'].getVal() + 'px; ';
            }

            // Border Style
            if (o['border_style'].getVal() !== '') {
                css += 'border-style: ' + o['border_style'].getVal() + '; ';
            }

            // Border Color
            var c_bg = hexToRgb(o['border_color'].getVal());
            css += 'border-color: rgba('+ c_bg.r +', '+ c_bg.g +', '+ c_bg.b +', '+ o['border_opacity'].getVal() +'); ';

            // Border Radius
            if (isNumeric(o['border_radius'].getVal())) {
                css += 'border-radius: ' + o['border_radius'].getVal() + 'px; ';
            }
        }

        return css;
    }
    Element.prototype.generateLayoutClass = function() {
        var o = this.controls['layout'];

        if (parseInt(o['use_grid'].getVal(), 10) == 1) {
            var classes = '';
            var v = o['column_span'].getVal();

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
    Element.prototype.render = function() {
        // Preserve selection
        var selected = false;
        if ($('#' + this.id).hasClass('sq-element-selected')) {
            selected = true;
        }

        // Update the element's style
        $('#' + this.id).attr('style', this.generateStyles());

        // Add layout classes to the element
        $('#' + this.id).attr('class', 'sq-element ' + this.generateLayoutClass());

        // Update the element's user set content
        $('#' + this.id).html(this.content());

        if (selected) {
            $('#' + this.id).addClass('sq-element-selected');
        }
    }
    Element.prototype.appendEditorControls = function() {
        var html = '';

        html += '     <div class="sq-element-controls">';
        html += '         <div class="sq-element-control-drag"></div>';
        html += '     </div>';

        $('#' + this.id).append(html);
    }
    Element.prototype.getCurrentOptions = function() {
        // Loop over all controls and put their values in an associative array

        var options = {};

        for (var controlGroupName in this.controls) {
            for (var controlName in this.controls[controlGroupName]) {
                var c = this.controls[controlGroupName][controlName];
                if (!options[controlGroupName]) {
                    options[controlGroupName] = {};
                }

                options[controlGroupName][controlName] = c.getVal();
            }
        }

        return options;
    }
    Element.prototype.generateHTML = function() {
        // function generating the HTML code that will be used in the end product

        var html = '';

        html += '<div id="'+ this.id +'" class="squares-element '+ this.generateLayoutClass() +'" style="'+ this.generateStyles() +'">';
        html += this.content();
        html += '</div>';

        return html;
    }

    function EditorWindow() {
        this.root = undefined;
        this.id = Math.floor(Math.random() * 10000) + 1;

        this.visible = false;

        // flags for dragging the window
        this.shouldStartDragging = false;
        this.didStartDragging = false;
        this.dragging = false;
        this.iex = 0; // initial event x
        this.iey = 0; // initial event y
        this.ix = 0; // initial window x
        this.iy = 0; // initial window y

        this.init();
        this.events();
        this.show(600, 100);
    }
    EditorWindow.prototype.init = function() {
        var WindowHTML = '';

        WindowHTML += ' <div class="sq-window" id="sq-window-'+ this.id +'">';
        WindowHTML += '     <div class="sq-window-header">';
        WindowHTML += '         <div class="sq-window-main-nav">';
        WindowHTML += '             <div id="sq-window-main-nav-button-elements" class="sq-window-main-nav-button" data-tab-group="sq-window-main-tab-group" data-tab-index="0" data-tab-button><i class="fa fa-cube" aria-hidden="true"></i></div>';
        WindowHTML += '             <div id="sq-window-main-nav-button-settings" class="sq-window-main-nav-button" data-tab-group="sq-window-main-tab-group" data-tab-index="1" data-tab-button><i class="fa fa-cog" aria-hidden="true"></i></div>';
        WindowHTML += '         </div>';
        WindowHTML += '         <div class="sq-window-close"><i class="fa fa-times"></i></div>';
        WindowHTML += '     </div>';
        WindowHTML += '     <div class="sq-window-container">';

        // Elements tab
        WindowHTML += '         <div class="sq-window-tab-content" data-tab-group="sq-window-main-tab-group" data-tab-index="0" data-tab-content id="sq-window-elements-tab-content">';
        WindowHTML += '             <div class="sq-window-main-tab-header">';
        WindowHTML += '                 <h1>Elements</h1>';
        WindowHTML += '                 <div id="sq-window-elements-search">';
        WindowHTML += '                     <i class="fa fa-search" aria-hidden="true"></i>';
        WindowHTML += '                     <input type="text" id="sq-window-elements-search-input">';
        WindowHTML += '                 </div>';
        WindowHTML += '             </div>';
        WindowHTML += '             <div class="sq-window-content">';
        WindowHTML += '                 <div id="sq-no-elements-found">No elements found.</div>';
        for (var i=0; i<registeredElements.length; i++) {
            WindowHTML += '             <div class="sq-element-thumb" data-index="' + i + '">';
            WindowHTML += '                 <div class="sq-element-thumb-icon">';
            WindowHTML += '                     <i class="' + registeredElements[i].iconClass + '"></i>';
            WindowHTML += '                 </div>';
            WindowHTML += '                 <div class="sq-element-thumb-title">';
            WindowHTML += '                     <h2>'+ registeredElements[i].name +'</h2>';
            WindowHTML += '                 </div>';
            WindowHTML += '             </div>';
        }
        WindowHTML += '                 <div class="squares-clear"></div>';
        WindowHTML += '             </div>';
        WindowHTML += '         </div>';

        // Settings tab
        WindowHTML += '         <div class="sq-window-tab-content" data-tab-group="sq-window-main-tab-group" data-tab-index="1" data-tab-content id="sq-window-settings-tab-content">';
        WindowHTML += '             <div class="sq-window-main-tab-header"><h1>Settings</h1></div>';
        WindowHTML += '             <div id="sq-window-settings-tab-inner-content"></div>';
        WindowHTML += '         </div>';

        WindowHTML += '     </div>';
        WindowHTML += ' </div>';

        if ($('.sq-windows-root').length == 0) {
            $('body').prepend('<div class="sq-windows-root"></div>');
        }

        $('.sq-windows-root').html(WindowHTML);

        this.root = $('#sq-window-' + this.id);

        this.openFirstTab();
        this.removeElementSettings();
    }
    EditorWindow.prototype.events = function() {
        var self = this;

        // Search field
        $(document).on('keyup', '#sq-window-elements-search-input', function() {
            var v = $(this).val().toLowerCase();

            var elementsFound = false;

            $('.sq-element-thumb').each(function() {
                var elementTitle = $(this).find('h2').html();

                if (elementTitle.toLowerCase().indexOf(v) >= 0) {
                    elementsFound = true;
                    $(this).show();
                } else {
                    $(this).hide();
                }
            });

            if (!elementsFound) {
                $('#sq-no-elements-found').show();
            } else {
                $('#sq-no-elements-found').hide();
            }
        });

        // Open the editor window when click on element
        $(document).on('click', '.sq-element', function() {
            if (!self.visible) {
                var x = $(this).offset().left + $(this).closest('.sq-root-container').width() + 40;
                var y = $(this).offset().top;
                self.show(x, y);
            }

            var editor = $(this).closest('.sq-root-container').data.editor;
            var containerIndex = $(this).closest('.sq-container').data('index');
            var elementIndex = $(this).data('index');
            var el = editor.settings.containers[containerIndex].settings.elements[elementIndex];

            // Open the settings tab
            $('#sq-window-elements-tab-content').hide();
            $('#sq-window-settings-tab-content').show();

            // Highlight the settings tab
            $('.sq-window-main-nav-button').removeClass('active');
            $('#sq-window-main-nav-button-settings').addClass('active');

            // Load the element settings
            $('#sq-window-settings-tab-inner-content').html(el.getSettingsForm());
            el.loadOptions();

            // Go to the first tab of the settings
            $('[data-tab-content][data-tab-group="sq-element-settings-tab-group"]').hide();
            $('[data-tab-content][data-tab-group="sq-element-settings-tab-group"][data-tab-index="0"]').show();

            // Highlight the first tab button
            $('[data-tab-button][data-tab-group="sq-element-settings-tab-group"]').removeClass('active').first().addClass('active');

            // Select the element
            editor.selectElement(el.id);
        });

        // Open the window when clicked on the add elements button
        $(document).on('click', '.sq-add-elements', function() {
            if (!self.visible) {
                var x = $(this).closest('.sq-root-container').offset().left + $(this).closest('.sq-root-container').width() + 40;
                var y = $(this).closest('.sq-root-container').offset().top;
                self.show(x + 20, y + 20);
            }

            // Show the elements tab
            $('#sq-window-elements-tab-content').show();
            $('#sq-window-settings-tab-content').hide();

            // Tabs
            $('.sq-window-main-nav-button').removeClass('active');
            $('#sq-window-main-nav-button-elements').addClass('active');
        });

        // Generic Tab functionality
        $(document).on('click', '[data-tab-button]', function() {
            var index = $(this).data('tab-index');
            var tabGroup = $(this).data('tab-group');

            $('[data-tab-button][data-tab-group="'+ tabGroup +'"]').removeClass('active');
            $(this).addClass('active');

            $('[data-tab-content][data-tab-group="'+ tabGroup +'"]').hide();
            $('[data-tab-content][data-tab-group="'+ tabGroup +'"][data-tab-index="'+ index +'"]').show();
        });

        // Button for closing the elements window
        self.root.find('.sq-window-close').on('click', function(e) {
            self.hide();
        });

        // Move the window by dragging its header
        self.root.find('.sq-window-header').off('mousedown');
        self.root.find('.sq-window-header').on('mousedown', function(e) {
            if ($(e.target).hasClass('sq-window-close') || $(e.target).closest('.sq-window-close').length > 0) return;

            self.shouldStartDragging = true;

            self.iex = e.pageX;
            self.iey = e.pageY;

            $('.sq-window-active').removeClass('sq-window-active');
            self.root.addClass('sq-window-active');
        });
        $(document).on('mousemove.' + self.id, function(e) {
            // Start moving the window only if the user drags it by 5 pixels or
            // more, to prevent accidental drag
            if (self.shouldStartDragging && !self.didStartDragging) {
                if (Math.abs(e.pageX - self.iex) > 5 || Math.abs(e.pageY - self.iey) > 5) {
                    self.ix = self.root.offset().left;
                    self.iy = self.root.offset().top;
                    self.dragging = true;
                    self.didStartDragging = true;
                }

            }

            if (self.dragging) {
                self.root.css({
                    left: self.ix + e.pageX - self.iex,
                    top: self.iy + e.pageY - self.iey,
                });
            }
        });

        $(document).on('mouseup.' + self.id, function(e) {
            self.shouldStartDragging = false;
            self.didStartDragging = false;
            self.dragging = false;
        });

        // =====================================================================
        // Needs tidying up
        // Drag elements from window to container functionality
        var shouldStartDraggingElementToContainer = false,
        didStartDraggingElementToContainer = false,
        draggingElementToContainer = false,
        virtualIndexOfDraggedElement = -1,
        draggedElementFromWindowCatalogIndex = -1,
        thumbElWhenDraggingFromWindow = undefined,
        targetEditor = undefined,
        dummyElementAtMouse = undefined,
        elementDragMap = undefined;
        var iex = 0, iey = 0, ix = 0, iy = 0;

        $(document).off('mousedown', '.sq-element-thumb');
        $(document).on('mousedown', '.sq-element-thumb', function(e) {
            shouldStartDraggingElementToContainer = true;

            iex = e.pageX;
            iey = e.pageY;

            thumbElWhenDraggingFromWindow = $(this);
        });
        $(document).off('mousemove.elementFromWindow');
        $(document).on('mousemove.elementFromWindow', function(e) {
            if (shouldStartDraggingElementToContainer && !didStartDraggingElementToContainer) {
                if (Math.abs(e.pageX - iex) > 5 || Math.abs(e.pageY - iey) > 5) {
                    didStartDraggingElementToContainer = true;

                    // Get contents and position of the element thumb
                    draggedElementFromWindowCatalogIndex = thumbElWhenDraggingFromWindow.data('index');

                    var contents = thumbElWhenDraggingFromWindow.html();

                    ix = thumbElWhenDraggingFromWindow.offset().left;
                    iy = thumbElWhenDraggingFromWindow.offset().top;

                    // Create a copy of the thumb and place it at mouse location
                    $('body').prepend('<div id="sq-dragged-element-clone" class="sq-element-thumb">' + contents + '</div>');
                    dummyElementAtMouse = $('#sq-dragged-element-clone');
                    dummyElementAtMouse.css({
                        left: ix,
                        top: iy,
                        margin: 0
                    });

                    // Create a virtual map of all possible positions of the
                    // dragged element in all editors
                    elementDragMap = new Array();

                    for (var k=0; k<editors.length; k++) {
                        var editor = editors[k];

                        for (var i=0; i<editor.settings.containers.length; i++) {
                            var coords = { x: 0, y: 0 };
                            var c = editor.host.find('.sq-container[data-index='+ i +']');

                            // if the container has no elements, add one dummy element
                            // and move on to next container
                            if (editor.settings.containers[i].settings.elements.length == 0) {
                                c.append('<div id="sq-dummy-element-dragging-from-window-tmp"></div>');
                                var x = $('#sq-dummy-element-dragging-from-window-tmp').offset().left + $('#sq-dummy-element-dragging-from-window-tmp').outerWidth()/2;
                                var y = $('#sq-dummy-element-dragging-from-window-tmp').offset().top + $('#sq-dummy-element-dragging-from-window-tmp').outerHeight()/2;
                                elementDragMap.push({ x: x, y: y, elementIndex: 0, containerIndex: i, editorIndex: k });
                                $('#sq-dummy-element-dragging-from-window-tmp').remove();
                            }

                            for (var j=0; j<editor.settings.containers[i].settings.elements.length; j++) {
                                var el = c.find('.sq-element[data-index='+ j +']');

                                el.before('<div id="sq-dummy-element-dragging-from-window-tmp"></div>');

                                var x = $('#sq-dummy-element-dragging-from-window-tmp').offset().left + $('#sq-dummy-element-dragging-from-window-tmp').outerWidth()/2;
                                var y = $('#sq-dummy-element-dragging-from-window-tmp').offset().top + $('#sq-dummy-element-dragging-from-window-tmp').outerHeight()/2;
                                elementDragMap.push({ x: x, y: y, elementIndex: j, containerIndex: i, editorIndex: k });
                                $('#sq-dummy-element-dragging-from-window-tmp').remove();

                                // When we reach the end of the elements array, add a dummy element after the last element
                                if (j == editor.settings.containers[i].settings.elements.length - 1) {
                                    el.after('<div id="sq-dummy-element-dragging-from-window-tmp"></div>');
                                    var x = $('#sq-dummy-element-dragging-from-window-tmp').offset().left + $('#sq-dummy-element-dragging-from-window-tmp').outerWidth()/2;
                                    var y = $('#sq-dummy-element-dragging-from-window-tmp').offset().top + $('#sq-dummy-element-dragging-from-window-tmp').outerHeight()/2;
                                    elementDragMap.push({ x: x, y: y, elementIndex: j+1, containerIndex: i, editorIndex: k });
                                    $('#sq-dummy-element-dragging-from-window-tmp').remove();
                                }
                            }
                        }
                    }

                    if (elementDragMap.length == 0) {
                        // no valid containers found
                        dummyElementAtMouse.remove();
                        didStartDraggingElementToContainer = false;
                        shouldStartDraggingElementToContainer = false;
                        didStartDraggingElementToContainer = false;
                        draggingElementToContainer = false;
                        virtualIndexOfDraggedElement = -1;
                    }
                }
            }

            if (didStartDraggingElementToContainer) {
                // Update dummy element at mouse position
                dummyElementAtMouse.css({
                    left: ix + e.pageX - iex,
                    top: iy + e.pageY - iey
                });

                // Find the closest virtual position to the mouse position
                var closestIndex = 0;
                var closestDistance = 999999;

                for (var i=0; i<elementDragMap.length; i++) {
                    var d = Math.abs(e.pageX - elementDragMap[i].x) + Math.abs(e.pageY - elementDragMap[i].y);
                    if (d < closestDistance) {
                        closestDistance = d;
                        closestIndex = i;
                    }
                }

                // If the closest index is different than the current index,
                // remove the dummy element and insert a new one and the new index
                if (closestIndex != virtualIndexOfDraggedElement) {
                    virtualIndexOfDraggedElement = closestIndex;

                    // Remove the current dummy element
                    $('#sq-dummy-element-dragging-from-window').remove();

                    // Insert a new dummy element at the container/element index
                    var containerIndex = elementDragMap[virtualIndexOfDraggedElement].containerIndex;
                    var elementIndex = elementDragMap[virtualIndexOfDraggedElement].elementIndex;
                    var editorIndex = elementDragMap[virtualIndexOfDraggedElement].editorIndex;
                    var c = editors[editorIndex].host.find('.sq-container[data-index='+ containerIndex +']');

                    // If the index of the dummy element is bigger than the number
                    // of elements in that container, insert the dummy at the end
                    if (editors[editorIndex].settings.containers[containerIndex].settings.elements.length == 0) {
                        c.prepend('<div id="sq-dummy-element-dragging-from-window"><div id="sq-dummy-element-dragging-from-window-inner"></div></div>');
                    } else if (elementIndex == editors[editorIndex].settings.containers[containerIndex].settings.elements.length) {
                        var lastElementIndex = editors[editorIndex].settings.containers[containerIndex].settings.elements.length - 1;
                        var e = c.find('.sq-element[data-index='+ lastElementIndex +']');
                        e.after('<div id="sq-dummy-element-dragging-from-window"><div id="sq-dummy-element-dragging-from-window-inner"></div></div>');
                    } else {
                        var e = c.find('.sq-element[data-index='+ elementIndex +']');
                        e.before('<div id="sq-dummy-element-dragging-from-window"><div id="sq-dummy-element-dragging-from-window-inner"></div></div>');
                    }
                }
            }

        });
        $(document).off('mouseup.elementFromWindow');
        $(document).on('mouseup.elementFromWindow', function() {
            if (didStartDraggingElementToContainer) {
                // Remove element clone (at mouse position)
                dummyElementAtMouse.remove();

                var containerIndex = elementDragMap[virtualIndexOfDraggedElement].containerIndex;
                var elementIndex = elementDragMap[virtualIndexOfDraggedElement].elementIndex;
                var editorIndex = elementDragMap[virtualIndexOfDraggedElement].editorIndex;

                editors[editorIndex].addElement(containerIndex, elementIndex, draggedElementFromWindowCatalogIndex);
            }

            shouldStartDraggingElementToContainer = false;
            didStartDraggingElementToContainer = false;
            draggingElementToContainer = false;
            virtualIndexOfDraggedElement = -1;
        });

        // [end] Drag elements from window to container functionality
    }
    EditorWindow.prototype.show = function(x, y) {
        this.visible = true;
        this.root.show();

        if (x !== undefined && y !== undefined) {
            this.root.css({
                left: x,
                top: y
            });
        }
    }
    EditorWindow.prototype.hide = function() {
        this.visible = false;
        this.root.hide();
    }
    EditorWindow.prototype.openFirstTab = function() {
        // Open the first tab
        $('.sq-window-main-nav-button').removeClass('active');
        $('#sq-window-main-nav-button-elements').addClass('active');
        $('[data-tab-content][data-tab-group="sq-window-main-tab-group"]').hide();
        $('[data-tab-content][data-tab-group="sq-window-main-tab-group"][data-tab-index="0"]').show();
    }
    EditorWindow.prototype.removeElementSettings = function() {
        $('#sq-window-settings-tab-inner-content').html('<div id="sq-window-settings-tab-no-element">No element selected. To create an element, open the Elements tab and drag an element into a container.</div>');
    }

    function SquaresControl(s, name, group, tabGroup, options, valueUpdated) {
        // The 's' argument is the array coming from the registeredControls array
        // Automatically generated at the time of object creation
        this.id = Math.floor(Math.random() * 9999) + 1;
        this.elementID = 'sq-control-' + this.id;
        this.elementClass = 'sq-element-option-group';

        // Settings coming from the registered controls catalog
        // referenced in the 'this' variable, so 'this' can be accessed within
        // those functions (in case of validate(), HTML(), events(), etc)
        // These settings are also common in all controls
        this.type = s.type;
        this.getValue = s.getValue;
        this.setValue = s.setValue;
        this.HTML = s.HTML;

        // These variables are specific for each individual control
        this.name = name;
        this.options = options;
        this.group = group;
        this.tabGroup = tabGroup;

        // Private property, must be accessed only via setter and getter
        this._value = undefined;

        // Update this.elementClass
        if (this.group !== undefined) {
            this.elementClass = 'sq-element-option-group-' + this.group.toLowerCase().replace(/\s/g, '-');
        }

        // Launch the events provided from the settings
        this.init = s.init;
        this.init();

        // Create a callback function for when the control updates its value
        this.valueUpdated = valueUpdated;

        // Inline label flag
        this.customLabel = s.customLabel;
    }
    SquaresControl.prototype.getVal = function() {
        return this._value;
    }
    SquaresControl.prototype.setVal = function(v) {
        this._value = v;

        try {
            this.setValue(v);
        } catch (err) {

        }
    }
    SquaresControl.prototype.loadVal = function(v) {
        this.setValue(this._value);
    }
    SquaresControl.prototype.valueChanged = function() {
        // Re-sets the control to its stored value
        this._value = this.getValue();
        this.valueUpdated();
    }

    // Utility
    function hexToRgb(hex) {
        var result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hex);
        return result ? {
            r: parseInt(result[1], 16),
            g: parseInt(result[2], 16),
            b: parseInt(result[3], 16)
        } : null;
    }
    function subtract(a, b) {
        var r = {};

        // For each property of 'b'
        // if it's different than the corresponding property of 'a'
        // place it in 'r'
        for (var key in b) {
            if (typeof(b[key]) == 'object') {
                if (!a[key]) a[key] = {};
                r[key] = subtract(a[key], b[key]);
            } else {
                if (b[key] != a[key]) {
                    r[key] = a[key];
                }
            }
        }

        return r;
    }
    function clean(a) {
        var r = undefined;

        // Check if 'a' is an object
        if (typeof(a) == 'object') {
            // If 'a' is an object, check if it's empty and set to undefined if true
            if (isEmpty(a)) {
                r = undefined;
            } else {
                // If 'a' is NOT empty, iterate over each of its properties
                // and recursively clean
                for (var key in a) {
                    var cleaned = clean(a[key]);

                    if (cleaned !== undefined) {
                        if (r === undefined) r = {};

                        r[key] = cleaned;
                    }
                }
            }
        } else {
            r = a;
        }

        return r;
    }
    function isEmpty(obj) {
        for(var prop in obj) {
            if(obj.hasOwnProperty(prop))
            return false;
        }

        return true && JSON.stringify(obj) === JSON.stringify({});
    }
    function getWidthOfElementInGrid(span) {
        var columnWidth = 8.33333333;
        var elementWidth = columnWidth * span;

        return elementWidth + '%';
    }
    function isNumeric(n) {
        return !isNaN(parseFloat(n)) && isFinite(n);
    }

})(jQuery, window, document);
