// Webcraft Plugins Ltd.
// Author: Nikolay Dyankov

/*
Class hierarchy and descriptions:

- WCPEditor
The main class.

- WCPEditorForm
An abstract class, containing a list of controls, grouped in tabs.
It will get/set values for the controls in bulk.
It will generate its own HTML code.

- WCPEditorControl
An object, representing a single control. It will have a getter
and a setter.
*/

;(function ($, window, document, undefined) {
    var wcpEditor = undefined;
    var wcpForms = [];
    var registeredControls = [];

    function WCPEditor() {
        this.host = $('#wcp-editor');
        this.forms = {};

        this.tooltip = undefined;
        this.modal = undefined;
        this.modalTimeout = undefined;

        this.loadingScreen = undefined;
        this.loadingScreenTimeout = undefined;

        // Temp vars
        this.saveToDeleteID = undefined;
    }
    WCPEditor.prototype.init = function(options) {
        this.options = options;

        // Build UI
        var html = '';

        var canvasClass = '';
        if (this.options.canvasFill) {
            canvasClass = 'wcp-editor-canvas-fill';
        }

        var canvasStyle = '';
        if (!this.options.canvasFill) {
            canvasStyle += 'width: ' + this.options.canvasWidth + 'px; height: ' + this.options.canvasHeight + 'px;';
        }

        html += '<div id="wcp-editor-left">';

        // Save, Load, Code, Preview buttons

        html += '   <div id="wcp-editor-main-buttons">';
        //if (this.options.newButton) {
        //    html += '       <div id="wcp-editor-button-new" class="wcp-editor-main-button">';
        //    html += '           <div class="wcp-editor-main-button-icon"><i class="fa fa-file" aria-hidden="true"></i></div>';
        //    html += '           <div class="wcp-editor-main-button-text">New</div>';
        //    html += '       </div>';
        //}
        //html += '       <div id="wcp-editor-button-save" class="wcp-editor-main-button">';
        //html += '           <div class="wcp-editor-main-button-icon"><i class="fa fa-floppy-o" aria-hidden="true"></i></div>';
        //html += '           <div class="wcp-editor-main-button-text">Save</div>';
        //html += '       </div>';
        //html += '       <div id="wcp-editor-button-load" class="wcp-editor-main-button">';
        //html += '           <div class="wcp-editor-main-button-icon"><i class="fa fa-sign-out" aria-hidden="true"></i></div>';
        //html += '           <div class="wcp-editor-main-button-text">Load</div>';
        //html += '       </div>';
        html += '       <div id="wcp-editor-button-undo" class="wcp-editor-main-button">';
        html += '           <div class="wcp-editor-main-button-icon"><i class="fa fa-undo" aria-hidden="true"></i></div>';
        html += '           <div class="wcp-editor-main-button-text">Undo</div>';
        html += '       </div>';
        html += '       <div id="wcp-editor-button-redo" class="wcp-editor-main-button">';
        html += '           <div class="wcp-editor-main-button-icon"><i class="fa fa-repeat" aria-hidden="true"></i></div>';
        html += '           <div class="wcp-editor-main-button-text">Redo</div>';
        html += '       </div>';
        html += '       <div id="wcp-editor-button-preview" class="wcp-editor-main-button">';
        html += '           <div class="wcp-editor-main-button-icon"><i class="fa fa-eye" aria-hidden="true"></i></div>';
        html += '           <div class="wcp-editor-main-button-text">Preview</div>';
        html += '       </div>';

        html += '   </div>';

        // Main toolbar tab buttons
        html += '<div id="wcp-editor-main-tab-buttons">';
        for (var i=0; i<this.options.mainTabs.length; i++) {
            html += '<div class="wcp-editor-main-tab-button" data-wcp-main-tab-button-name="'+ this.options.mainTabs[i].name +'">';
            html += '   <div class="wcp-editor-main-tab-button-icon"><i class="'+ this.options.mainTabs[i].icon +'" aria-hidden="true"></i></div>';
            html += '   <div class="wcp-editor-main-tab-button-text">'+ this.options.mainTabs[i].name +'</div>';
            html += '</div>';
        }
        html += '</div>';

        // Main toolbar tab content
        html += '<div id="wcp-editor-main-tab-contents">';
        for (var i=0; i<this.options.mainTabs.length; i++) {
            html += '<div class="wcp-editor-main-tab-content" data-wcp-main-tab-content-name="'+ this.options.mainTabs[i].name +'">';
            html += '   <div class="wcp-editor-main-tab-content-title">'+ this.options.mainTabs[i].title +'</div>';
            html += '   <div class="wcp-editor-main-tab-content-inner-wrap">'+ $.wcpEditorGetContentForTabWithName(this.options.mainTabs[i].name) +'</div>';
            html += '</div>';
        }
        html += '</div>';

        html += '</div>';
        html += '<div id="wcp-editor-center">';

        // Help button
        if (this.options.helpButton) {
            html += '<div id="wcp-editor-help-button"><i class="fa fa-question" aria-hidden="true"></i></div>';
        }

        // Extra main buttons
        html += '    <div id="wcp-editor-extra-main-buttons">';
        for (var i=0; i<this.options.extraMainButtons.length; i++) {
            var b = this.options.extraMainButtons[i];

            var tooltip = '';

            if (b.tooltip) {
                tooltip = 'data-wcp-tooltip="'+ b.tooltip +'" data-wcp-tooltip-position="bottom"';
            }

            html += '       <div class="wcp-editor-extra-main-button" data-wcp-editor-extra-main-button-name="'+ b.name +'" '+ tooltip +'>';
            html += '           <div class="wcp-editor-extra-main-button-icon"><i class="'+ b.icon +'" aria-hidden="true"></i></div>';
            html += '           <div class="wcp-editor-extra-main-button-title">'+ b.title +'</div>';
            html += '       </div>';
        }
        html += '    </div>';

        // Toolbar

        // Are toolbar buttons grouped?
        if (this.options.toolbarButtons[0].constructor == Array) {
            // Grouped
            html += '   <div id="wcp-editor-toolbar-wrap">';
            for (var i=0; i<this.options.toolbarButtons.length; i++) {
                html += '   <div id="wcp-editor-toolbar" class="wcp-editor-toolbar-grouped">';
                var b = this.options.toolbarButtons[i];
                for (var j=0; j<b.length; j++) {
                    drawToolbarButton(b[j]);
                }
                html += '    </div>';
            }
            html += '    </div>';
        } else {
            // Not grouped
            html += '   <div id="wcp-editor-toolbar">';
            for (var i=0; i<this.options.toolbarButtons.length; i++) {
                var b = this.options.toolbarButtons[i];
                drawToolbarButton(b);
            }
            html += '    </div>';
        }

        function drawToolbarButton(b) {
            var icon = '';

            if (b.customIcon != undefined) {
                icon = b.customIcon;
            } else {
                icon = '<i class="'+ b.icon +'" aria-hidden="true"></i>';
            }

            html += '       <div class="wcp-editor-toolbar-button" data-wcp-editor-toolbar-button-name="'+ b.name +'" data-wcp-tooltip="'+ b.title +'" data-wcp-tooltip-position="right" data-wcp-editor-toolbar-button-kind="'+ b.kind +'">';
            html += '           <div class="wcp-editor-toolbar-button-icon">'+ icon +'</div>';
            html += '           <div class="wcp-editor-toolbar-button-title">'+ b.title +'</div>';
            html += '       </div>';
        }

        // Canvas
        html += '    <div id="wcp-editor-canvas" class="'+ canvasClass +'" style="'+ canvasStyle +'">'+ $.wcpEditorGetContentForCanvas() +'</div>';
        html += '</div>';

        // Editor-right
        html += '<div id="wcp-editor-right">';
        html += '</div>';

        this.host.html(html);

        // Set the list items
        this.setListItems($.wcpEditorGetListItems());

        // Show the first main tab
        this.openMainTabWithName(this.options.mainTabs[0].name);

        this.events();
    };
    WCPEditor.prototype.events = function () {
        var self = this;

        // Main tab functionality
        $('[data-wcp-main-tab-button-name]').on('click', function() {
            var name = $(this).data('wcp-main-tab-button-name');
            self.openMainTabWithName(name);
        });

        // Main buttons events

        // New
        $(document).off('click', '#wcp-editor-button-new');
        $(document).on('click', '#wcp-editor-button-new', function() {
            $.wcpEditorEventNewButtonPressed();
            self.presentCreateNewModal();
        });

        // Save
        $(document).off('click', '#wcp-editor-button-save');
        $(document).on('click', '#wcp-editor-button-save', function() {
            $.wcpEditorEventSaveButtonPressed();
        });

        // Load
        $(document).off('click', '#wcp-editor-button-load');
        $(document).on('click', '#wcp-editor-button-load', function() {
            $.wcpEditorEventLoadButtonPressed();
            self.presentLoadModal();
        });

        // Undo
        $(document).off('click', '#wcp-editor-button-undo');
        $(document).on('click', '#wcp-editor-button-undo', function() {
            $.wcpEditorEventUndoButtonPressed();
        });

        // Redo
        $(document).off('click', '#wcp-editor-button-redo');
        $(document).on('click', '#wcp-editor-button-redo', function() {
            $.wcpEditorEventRedoButtonPressed();
        });

        // Preview
        $(document).off('click', '#wcp-editor-button-preview');
        $(document).on('click', '#wcp-editor-button-preview', function() {
            $.wcpEditorEventPreviewButtonPressed();

            if (self.options.previewToggle) {
                if ($(this).hasClass('wcp-active')) {
                    $(this).removeClass('wcp-active');

                    $.wcpEditorEventExitedPreviewMode();
                } else {
                    $(this).addClass('wcp-active');
                    $.wcpEditorEventEnteredPreviewMode();
                }
            }
        });

        // Extra main buttons events
        $(document).off('click', '.wcp-editor-extra-main-button');
        $(document).on('click', '.wcp-editor-extra-main-button', function(e) {
            var buttonName = $(this).data('wcp-editor-extra-main-button-name');
            $.wcpEditorEventExtraMainButtonClick(buttonName);

            // Import button
            if (buttonName == 'import') {
                self.presentImportModal();
            }

            // Export button
            if (buttonName == 'export') {
                self.presentExportModal();
            }
        });

        // Tools events
        $(document).off('click', '.wcp-editor-toolbar-button');
        $(document).on('click', '.wcp-editor-toolbar-button', function(e) {
            $.wcpEditorEventPressedTool($(this).data('wcp-editor-toolbar-button-name'));

            if ($(this).data('wcp-editor-toolbar-button-kind') == 'button') {
                return;
            }

            $('.wcp-editor-toolbar-button').removeClass('wcp-active');
            $(this).addClass('wcp-active');
            $.wcpEditorEventSelectedTool($(this).data('wcp-editor-toolbar-button-name'));
        });

        // Help button event
        $(document).off('click', '#wcp-editor-help-button');
        $(document).on('click', '#wcp-editor-help-button', function(e) {
            $.wcpEditorEventHelpButtonPressed();
        });


        // List items events
        $(document).off('mouseover', '.wcp-editor-list-item');
        $(document).on('mouseover', '.wcp-editor-list-item', function(e) {
            $.wcpEditorEventListItemMouseover($(this).data('wcp-editor-list-item-id'));
        });

        $(document).off('click', '.wcp-editor-list-item');
        $(document).on('click', '.wcp-editor-list-item', function(e) {
            if ($(e.target).closest('.wcp-editor-list-item-buttons').length == 0) {
                self.selectListItem($(this).data('wcp-editor-list-item-id'));

                $.wcpEditorEventListItemSelected($(this).data('wcp-editor-list-item-id'));
            }
        });
        $(document).off('click', '.wcp-editor-list-item-button');
        $(document).on('click', '.wcp-editor-list-item-button', function() {
            var itemID = $(this).closest('.wcp-editor-list-item').data('wcp-editor-list-item-id');
            var buttonName = $(this).data('wcp-editor-list-item-button-name');
            $.wcpEditorEventListItemButtonPressed(itemID, buttonName);
        });

        // List title buttons
        $(document).off('click', '.wcp-editor-list-item-title-button');
        $(document).on('click', '.wcp-editor-list-item-title-button', function() {
            var buttonName = $(this).data('wcp-editor-list-item-title-button-name');
            $.wcpEditorEventListItemTitleButtonPressed(buttonName);
        });

        // Tooltip functionality
        $(document).off('mouseover', '[data-wcp-tooltip]');
        $(document).on('mouseover', '[data-wcp-tooltip]', function(e) {
            $(this).addClass('wcp-visible-tooltip');
            self.showTooltip($(this), $(this).data('wcp-tooltip'), $(this).data('wcp-tooltip-position'));
        });
        $(document).off('mouseout', '[data-wcp-tooltip]');
        $(document).on('mouseout', '[data-wcp-tooltip]', function(e) {
            self.hideTooltip($(this));
        });

        // Modal events
        $(document).off('click', '#wcp-editor-modal');
        $(document).on('click', '#wcp-editor-modal', function(e) {
            if ($(e.target).attr('id') == 'wcp-editor-modal') {
                self.closeModal();
                var modalName = $('#wcp-editor-modal').data('wcp-editor-modal-name');
                $.wcpEditorEventModalClosed(modalName);
            }
        });
        $(document).off('click', '.wcp-editor-modal-close');
        $(document).on('click', '.wcp-editor-modal-close', function(e) {
            self.closeModal();
            var modalName = $('#wcp-editor-modal').data('wcp-editor-modal-name');
            $.wcpEditorEventModalClosed(modalName);
        });
        $(document).off('click', '.wcp-editor-modal-button');
        $(document).on('click', '.wcp-editor-modal-button', function(e) {
            var modalName = $('#wcp-editor-modal').data('wcp-editor-modal-name');
            var buttonName = $(this).data('wcp-editor-modal-button-name');
            $.wcpEditorEventModalButtonClicked(modalName, buttonName);
        });
        $(document).off('click', '#wcp-editor-confirm-import');
        $(document).on('click', '#wcp-editor-confirm-import', function(e) {
            // Validate JSON
            var json = $('#wcp-editor-textarea-import').val();
            var parsedJSON = undefined;

            try {
                parsedJSON = JSON.parse(json);
            } catch (err) {
                console.log('error decoding JSON!');
            }

            if (parsedJSON === undefined) {
                // Show error text
                $('#wcp-editor-import-error').show();
            } else {
                // No error
                $('#wcp-editor-import-error').hide();

                // Fire event
                $.wcpEditorEventImportedJSON(parsedJSON);

                // Close modal
                self.closeModal();
            }
        });
        $(document).off('click', '#button-loading-screen-close');
        $(document).on('click', '#button-loading-screen-close', function() {
            self.hideLoadingScreen();
        });


        // Create new instance button
        $(document).off('click', '#wcp-editor-button-create-new-instance');
        $(document).on('click', '#wcp-editor-button-create-new-instance', function(e) {
            // validate
            if ($('#wcp-editor-input-create-new').val().length == 0) {
                // show error
                $('#wcp-editor-create-new-error').show();
            } else {
                // hide error, send event and close modal
                $('#wcp-editor-create-new-error').hide();

                var instanceName = $('#wcp-editor-input-create-new').val();
                $.wcpEditorEventCreatedNewInstance(instanceName);
                self.closeModal();
            }
        });
        // Load modal list item
        $(document).off('click', '.wcp-editor-save-list-item');
        $(document).on('click', '.wcp-editor-save-list-item', function() {
            var saveID = $(this).parent().data('wcp-editor-save-list-item-id');
            $.wcpEditorEventLoadSaveWithID(saveID);
            self.closeModal();
        });
        // Load modal delete button
        $(document).off('click', '.wcp-editor-save-list-item-delete-button');
        $(document).on('click', '.wcp-editor-save-list-item-delete-button', function() {
            self.saveToDeleteID = $(this).parent().data('wcp-editor-save-list-item-id');

            self.closeModal();

            // Present delete save confirmation modal
            self.presentDeleteSaveConfirmationModal();
        });
        // Save delete modal cancel
        $(document).off('click', '#wcp-editor-cancel-delete-save');
        $(document).on('click', '#wcp-editor-cancel-delete-save', function() {
            self.presentLoadModal();
        });
        // Save delete modal confirm
        $(document).off('click', '#wcp-editor-confirm-delete-save');
        $(document).on('click', '#wcp-editor-confirm-delete-save', function() {
            $.wcpEditorEventDeleteSaveWithID(self.saveToDeleteID, function() {
                self.presentLoadModal();
            });
        });

        // Press Enter to trigger the primary modal button
        $(document).off('keyup', '#wcp-editor-input-create-new');
        $(document).on('keyup', '#wcp-editor-input-create-new', function(e) {
            if (e.keyCode == 13 && $('#wcp-editor-modal').length > 0 && $('#wcp-editor-modal').hasClass('wcp-editor-modal-visible')) {
                if ($('.wcp-editor-modal-button-primary').length == 1) {
                    $('.wcp-editor-modal-button-primary').click();
                }
                if ($('.wcp-editor-modal-button-danger').length == 1) {
                    $('.wcp-editor-modal-button-danger').click();
                }
            }
        });

        // List items reorder
        var iex = 0, iey = 0, ix = 0, iy = 0;
        var shouldStartDragging = false, didStartDragging = false, dragThreshold = 5;
        var dragMap = [], startingItemIndex = -1, currentItemIndex = -1;
        var draggedListItem = undefined, listItemCopy = undefined;
        var draggedListItemWidth = 0;
        var draggedListItemHeight = 0;
        var listScroll = 0;

        $(document).off('mousedown', '.wcp-editor-list-item');
        $(document).on('mousedown', '.wcp-editor-list-item', function(e) {
            iex = e.pageX;
            iey = e.pageY;

            shouldStartDragging = true;
            draggedListItem = $(this);

            // Set the startingItemIndex
            startingItemIndex = draggedListItem.data('wcp-editor-list-item-index');

            // Cache some variables
            draggedListItemWidth = draggedListItem.outerWidth();
            draggedListItemHeight = draggedListItem.outerHeight();

            // Cache the list scroll
            listScroll = $('#wcp-editor-right').scrollTop();
        });

        $(document).off('mousemove.wcp-editor-list-item-reorder');
        $(document).on('mousemove.wcp-editor-list-item-reorder', function(e) {
            var dx = Math.abs(e.pageX - iex);
            var dy = Math.abs(e.pageY - iey);

            if (!didStartDragging && shouldStartDragging && (dx > dragThreshold || dy > dragThreshold)) {
                didStartDragging = true;

                // Create a copy of the list item at the current mouse position
                listItemCopy = draggedListItem.clone();
                listItemCopy.addClass('wcp-editor-dragged-list-item');
                listItemCopy.css({
                    width: draggedListItemWidth,
                    left: draggedListItem.offset().left,
                    top: draggedListItem.offset().top
                });

                ix = draggedListItem.offset().left;
                iy = draggedListItem.offset().top;

                $('body').prepend(listItemCopy);

                // Wrap the listItemCopy in an element to prevent it from going
                // beyond the boundaries of the document
                listItemCopy.wrap('<div class="wcp-editor-dragged-list-item-wrap"></div>');

                // Create a virtual map of every possible position of the item
                // using an invisible dummy item of the same dimentions
                var tempElHtml = '<div id="wcp-editor-list-item-invisible-tmp" style="width: '+ draggedListItemWidth +'px; height: '+ draggedListItemHeight +'px; position: relative;"></div>';

                var numberOfListItems = $('#wcp-editor-right .wcp-editor-list-item').length;
                for (var i=0; i<numberOfListItems; i++) {
                    // Insert temp el
                    $('#wcp-editor-right .wcp-editor-list-item[data-wcp-editor-list-item-index="'+ i +'"]').before(tempElHtml);

                    // Store its position
                    dragMap.push($('#wcp-editor-list-item-invisible-tmp').offset().top + draggedListItemHeight/2);

                    // Delete it
                    $('#wcp-editor-list-item-invisible-tmp').remove();
                }

                // Hide the draggedListItem
                draggedListItem.hide();
            }

            if (didStartDragging) {
                clearSelection();

                // Update the position of the listItemCopy
                listItemCopy.css({
                    left: ix - (iex - e.pageX),
                    top: iy - (iey - e.pageY)
                });

                // Check which is the closest map point from the virtual map
                var closestIndex = -1;
                var smallestDistance = 99999;
                var listItemCopyOffsetTop = listItemCopy.offset().top + draggedListItemHeight/2;

                for (var i=0; i<dragMap.length; i++) {
                    var distance = Math.abs(listItemCopyOffsetTop - dragMap[i]);

                    if (distance < smallestDistance) {
                        smallestDistance = distance;
                        closestIndex = i;
                    }
                }

                // If the map point has a different index from the currentItemIndex,
                // then insert a visible dummy element at that position
                if (currentItemIndex != closestIndex) {
                    // Remove the current temp element
                    $('#wcp-editor-list-item-visible-tmp').remove();

                    var visibleDummyElementHTML = '<div id="wcp-editor-list-item-visible-tmp" style="width: '+ draggedListItemWidth +'px; height: '+ draggedListItemHeight +'px;"><div id="wcp-editor-list-item-visible-tmp-inner"></div></div>';

                    if (closestIndex < startingItemIndex) {
                        $('#wcp-editor-right .wcp-editor-list-item[data-wcp-editor-list-item-index="'+ closestIndex +'"]').before(visibleDummyElementHTML);
                    } else {
                        $('#wcp-editor-right .wcp-editor-list-item[data-wcp-editor-list-item-index="'+ closestIndex +'"]').after(visibleDummyElementHTML);
                    }

                    // Set the currentItemIndex to the new index
                    currentItemIndex = closestIndex;
                }

                // Preserve the list scroll
                $('#wcp-editor-right').scrollTop(listScroll);
            }
        });

        $(document).off('mouseup.wcp-editor-list-item-reorder');
        $(document).on('mouseup.wcp-editor-list-item-reorder', function() {
            if (didStartDragging) {
                // Delete temporary items
                $('.wcp-editor-dragged-list-item-wrap').remove();
                $('#wcp-editor-list-item-visible-tmp').remove();

                // Show the hidden original list item
                draggedListItem.show();

                // Send an event that the order of the items changed
                $.wcpEditorEventListItemMoved(draggedListItem.attr('id'), startingItemIndex, currentItemIndex);
            }

            // Clean up
            shouldStartDragging = false;
            didStartDragging = false;
            startingItemIndex = -1;
            currentItemIndex = -1;
            dragMap = [];
        });
    };
    WCPEditor.prototype.openMainTabWithName = function(tabName) {
        $('.wcp-editor-main-tab-content').hide();
        $('[data-wcp-main-tab-content-name="'+ tabName +'"]').show();

        $('.wcp-editor-main-tab-button').removeClass('wcp-active');
        $('[data-wcp-main-tab-button-name="'+ tabName +'"]').addClass('wcp-active');
    };
    WCPEditor.prototype.presentModal = function(options) {
        clearTimeout(this.modalTimeout);

        if ($('#wcp-editor-modal').length == 0) {
            var html = '';

            html += '<div id="wcp-editor-modal">';
            html += '   <div class="wcp-editor-modal-body">';
            html += '       <div class="wcp-editor-modal-close"><i class="fa fa-times" aria-hidden="true"></i></div>';
            html += '       <div class="wcp-editor-modal-header"></div>';
            html += '       <div class="wcp-editor-modal-content"></div>';
            html += '       <div class="wcp-editor-modal-footer"></div>';
            html += '       </div>';
            html += '   </div>';
            html += '</div>';

            $('body').append(html);
            this.modal = $('#wcp-editor-modal');
        }
        if (!this.modal) {
            this.modal = $('#wcp-editor-modal');
        }

        // Set the data-name
        this.modal.data('wcp-editor-modal-name', options.name);

        // Set the title
        this.modal.find('.wcp-editor-modal-header').html(options.title);

        // Set the body
        this.modal.find('.wcp-editor-modal-content').html(options.body);

        // Set the buttons
        var buttonHtml = '';
        for (var i=0; i<options.buttons.length; i++) {
            var buttonClass = '';
            var buttonId = '';

            if (options.buttons[i].class == 'primary') {
                buttonClass = 'wcp-editor-modal-button-primary';
            }
            if (options.buttons[i].class == 'danger') {
                buttonClass = 'wcp-editor-modal-button-danger';
            }

            if (options.buttons[i].id) {
                buttonId = options.buttons[i].id;
            }

            buttonHtml += '<div class="wcp-editor-modal-button '+ buttonClass +'" id="'+ buttonId +'" data-wcp-editor-modal-button-name="'+ options.buttons[i].name +'">'+ options.buttons[i].title +'</div>'
        }

        this.modal.find('.wcp-editor-modal-footer').html(buttonHtml);

        // Show modal
        var self = this;
        self.modal.css({ display: 'flex' });
        setTimeout(function() {
            self.modal.addClass('wcp-editor-modal-visible');
        }, 10);
    };
    WCPEditor.prototype.closeModal = function() {
        var self = this;

        this.modal.removeClass('wcp-editor-modal-visible');

        this.modalTimeout = setTimeout(function() {
            self.modal.hide();
        }, 330);
    };
    WCPEditor.prototype.presentCreateNewModal = function() {
        var modalBody = '';
        modalBody += '<div class="wcp-editor-form-control">';
        modalBody += '  <label for="wcp-editor-input-create-new">Name: </label>';
        modalBody += '  <input type="text" id="wcp-editor-input-create-new">';
        modalBody += '  <div id="wcp-editor-create-new-error">Please enter a name!</div>';
        modalBody += '</div>';

        var modalOptions = {
            name: 'create_new',
            title: 'Create New',
            buttons: [
                {
                    name: 'cancel',
                    title: 'Cancel',
                    class: '',
                },
                {
                    name: 'create',
                    title: 'Create',
                    class: 'primary',
                    id: 'wcp-editor-button-create-new-instance'
                },
            ],
            body: modalBody
        };

        this.presentModal(modalOptions);

        // Focus the name input
        $('#wcp-editor-input-create-new').get(0).focus();
    };
    WCPEditor.prototype.presentLoadModal = function() {
        var self = this;

        this.presentLoadingScreenWithText('Loading Saves...');

        $.wcpEditorGetSaves(function(savesList) {
            var modalBody = '';

            for (var i=0; i<savesList.length; i++) {
                modalBody += '  <div class="wcp-editor-save-list-item-wrap" data-wcp-editor-save-list-item-name="'+ savesList[i].name +'" data-wcp-editor-save-list-item-id="'+ savesList[i].id +'">';
                modalBody += '      <div class="wcp-editor-save-list-item">'+ savesList[i].name +'</div>';
                modalBody += '      <div class="wcp-editor-save-list-item-delete-button"><i class="fa fa-trash-o" aria-hidden="true"></i></div>';
                modalBody += '  </div>';
            }

            var modalOptions = {
                name: 'load',
                title: 'Load',
                buttons: [
                    {
                        name: 'cancel',
                        title: 'Cancel',
                        class: '',
                    },
                ],
                body: modalBody
            };

            self.hideLoadingScreen();
            self.presentModal(modalOptions);
        });
    };
    WCPEditor.prototype.presentDeleteSaveConfirmationModal = function() {
        var modalOptions = {
            name: 'confirmation',
            title: 'Delete Save',
            buttons: [
                {
                    name: 'cancel',
                    title: 'Cancel',
                    class: '',
                    id: 'wcp-editor-cancel-delete-save'
                },
                {
                    name: 'delete',
                    title: 'Delete',
                    class: 'danger',
                    id: 'wcp-editor-confirm-delete-save'
                },
            ],
            body: 'Are you sure you want to permanently delete this save?'
        };

        this.presentModal(modalOptions);
    };
    WCPEditor.prototype.presentImportModal = function() {
        var html = '';

        html += '<div class="wcp-editor-form-control">';
        html += '   <label for="wcp-editor-textarea-import">Paste code to import:</label>';
        html += '   <textarea id="wcp-editor-textarea-import"></textarea>';
        html += '  <div id="wcp-editor-import-error">Invalid code!</div>';
        html += '</div>';

        var modalOptions = {
            name: 'import',
            title: 'Import',
            buttons: [
                {
                    name: 'cancel',
                    title: 'Cancel',
                    class: '',
                },
                {
                    name: 'import',
                    title: 'Import',
                    class: 'primary',
                    id: 'wcp-editor-confirm-import'
                },
            ],
            body: html
        };

        this.presentModal(modalOptions);

        // Focus the textarea
        $('#wcp-editor-textarea-import').get(0).focus();
    };
    WCPEditor.prototype.presentExportModal = function() {
        var html = '';

        html += '<div class="wcp-editor-form-control">';
        html += '   <label for="wcp-editor-textarea-export">Copy this code to import it later:</label>';
        html += '   <textarea id="wcp-editor-textarea-export">'+ $.wcpEditorGetExportJSON() +'</textarea>';
        html += '</div>';

        var modalOptions = {
            name: 'export',
            title: 'Export',
            buttons: [
                {
                    name: 'cancel',
                    title: 'Done',
                    class: 'primary',
                }
            ],
            body: html
        };

        this.presentModal(modalOptions);

        // Select the text
        $('#wcp-editor-textarea-export').get(0).select();
    };
    WCPEditor.prototype.setContentForTabWithName = function(tabName, content) {
        $('.wcp-editor-main-tab-content[data-wcp-main-tab-content-name="'+ tabName +'"]').find('.wcp-editor-main-tab-content-inner-wrap').html(content);
    };
    WCPEditor.prototype.setContentForCanvas = function(content) {
        $('#wcp-editor-canvas').html(content);
    };
    WCPEditor.prototype.setListItems = function(listItems) {
        // Preserve scroll
        var s = $('#wcp-editor-list').scrollTop();

        var buttonsHTML = '';

        for (var i=0; i<this.options.listItemButtons.length; i++) {
            var b = this.options.listItemButtons[i];

            buttonsHTML += '<div class="wcp-editor-list-item-button" data-wcp-editor-list-item-button-name="'+ b.name +'" data-wcp-tooltip="'+ b.title +'" data-wcp-tooltip-position="bottom">';
            buttonsHTML += '    <i class="'+ b.icon +'" aria-hidden="true"></i>';
            buttonsHTML += '</div>';
        }

        var html = '';

        // Set the title and title buttons
        if (this.options.listItemTitle && this.options.listItemTitle.length > 0) {
            $('#wcp-editor-right').addClass('wcp-editor-right-with-title');

            html += '<div id="wcp-editor-list-title">'+ this.options.listItemTitle +'</div>';

            if (this.options.listItemTitleButtons.length > 0) {
                $('#wcp-editor-right').addClass('wcp-editor-right-with-title-buttons');

                html += '<div id="wcp-editor-list-item-title-buttons">';

                for (var i=0; i<this.options.listItemTitleButtons.length; i++) {
                    var b = this.options.listItemTitleButtons[i];
                    html += '<div class="wcp-editor-list-item-title-button" data-wcp-editor-list-item-title-button-name="'+ b.name +'" data-wcp-tooltip="'+ b.title +'" data-wcp-tooltip-position="bottom">';
                    html += '    <i class="'+ b.icon +'" aria-hidden="true"></i>';
                    html += '</div>';
                }
                html += '</div>';
            }
        } else {
            $('#wcp-editor-right').removeClass('wcp-editor-right-with-title');
        }

        // Populate the list
        html += '<div id="wcp-editor-list">';
        for (var i=0; i<listItems.length; i++) {
            html += '<div class="wcp-editor-list-item" id="wcp-editor-list-item-'+ listItems[i].id +'" data-wcp-editor-list-item-index="'+ i +'" data-wcp-editor-list-item-id="'+ listItems[i].id +'">';
            html += '   <div class="wcp-editor-list-item-title">'+ listItems[i].title +'</div>';
            html += '   <div class="wcp-editor-list-item-buttons">'+ buttonsHTML +'</div>';
            html += '</div>';
        }
        html += '</div>';

        $('#wcp-editor-right').html(html);

        // Restore scroll
        $('#wcp-editor-list').scrollTop(s);
    };
    WCPEditor.prototype.selectListItem = function(listItemId) {
        $('.wcp-editor-list-item').removeClass('wcp-active');
        $('#wcp-editor-list-item-' + listItemId).addClass('wcp-active');

        // Adjust list scroll position to show the selected list item
        
    };
    WCPEditor.prototype.showTooltip = function(element, text, tooltipPosition) {
        if ($('#wcp-editor-tooltip').length == 0) {
            $('body').append('<div id="wcp-editor-tooltip"></div>');
            this.tooltip = $('#wcp-editor-tooltip');
        }
        if (!this.tooltip) {
            this.tooltip = $('#wcp-editor-tooltip');
        }

        // Set the text
        this.tooltip.html(text);

        // Show (invisible)
        this.tooltip.show();

        // Set the position
        var x = 0;
        var y = 0;
        var tooltipSpacing = 12;

        if (tooltipPosition == 'left') {
            x = element.offset().left - this.tooltip.outerWidth() - tooltipSpacing;
            y = element.offset().top + element.outerHeight()/2 - this.tooltip.outerHeight()/2;
        }
        if (tooltipPosition == 'right') {
            x = element.offset().left + element.outerWidth() + tooltipSpacing;
            y = element.offset().top + element.outerHeight()/2 - this.tooltip.outerHeight()/2;
        }
        if (tooltipPosition == 'top') {
            x = element.offset().left + element.outerWidth()/2 - this.tooltip.outerWidth()/2;
            y = element.offset().top - this.tooltip.outerHeight() - tooltipSpacing;
        }
        if (tooltipPosition == 'bottom') {
            x = element.offset().left + element.outerWidth()/2 - this.tooltip.outerWidth()/2;
            y = element.offset().top + element.outerHeight() + tooltipSpacing;
        }
        
        this.tooltip.css({
            left: x,
            top: y
        });

        // Set tooltip position class
        if (tooltipPosition == 'left') {
            this.tooltip.removeClass('wcp-editor-tooltip-left');
            this.tooltip.removeClass('wcp-editor-tooltip-right');
            this.tooltip.removeClass('wcp-editor-tooltip-top');
            this.tooltip.removeClass('wcp-editor-tooltip-bottom');

            this.tooltip.addClass('wcp-editor-tooltip-left');
        }
        if (tooltipPosition == 'right') {
            this.tooltip.removeClass('wcp-editor-tooltip-left');
            this.tooltip.removeClass('wcp-editor-tooltip-right');
            this.tooltip.removeClass('wcp-editor-tooltip-top');
            this.tooltip.removeClass('wcp-editor-tooltip-bottom');

            this.tooltip.addClass('wcp-editor-tooltip-right');
        }
        if (tooltipPosition == 'top') {
            this.tooltip.removeClass('wcp-editor-tooltip-left');
            this.tooltip.removeClass('wcp-editor-tooltip-right');
            this.tooltip.removeClass('wcp-editor-tooltip-top');
            this.tooltip.removeClass('wcp-editor-tooltip-bottom');

            this.tooltip.addClass('wcp-editor-tooltip-top');
        }
        if (tooltipPosition == 'bottom') {
            this.tooltip.removeClass('wcp-editor-tooltip-left');
            this.tooltip.removeClass('wcp-editor-tooltip-right');
            this.tooltip.removeClass('wcp-editor-tooltip-top');
            this.tooltip.removeClass('wcp-editor-tooltip-bottom');

            this.tooltip.addClass('wcp-editor-tooltip-bottom');
        }

        // Constrain to window
        if (this.tooltip.offset().left + this.tooltip.outerWidth() > window.innerWidth) {
            this.tooltip.css({
                left: window.innerWidth - this.tooltip.outerWidth()
            });
        }
        if (this.tooltip.offset().left < 0) {
            this.tooltip.css({
                left: 0
            });
        }
        if (this.tooltip.offset().top + this.tooltip.outerHeight() > window.innerHeight) {
            this.tooltip.css({
                top: window.innerHeight - this.tooltip.outerHeight()
            });
        }
        if (this.tooltip.offset().top < 0) {
            this.tooltip.css({
                top: 0
            });
        }

        // Show (visible)
        this.tooltip.addClass('wcp-editor-tooltip-visible');
    }
    WCPEditor.prototype.hideTooltip = function() {
        this.tooltip.hide();
        this.tooltip.removeClass('wcp-editor-tooltip-visible');
    }
    WCPEditor.prototype.presentLoadingScreenWithText = function(text) {
        clearTimeout(this.loadingScreenTimeout);

        if ($('#wcp-editor-loading-screen').length == 0) {
            var html = '';

            html += '<div id="wcp-editor-loading-screen">';
            html += '   <div id="wcp-editor-loading-screen-icon"><i class="fa fa-circle-o-notch fa-spin"></i></div>';
            html += '   <div id="wcp-editor-loading-screen-text"></div>';
            html += '</div>';

            $('body').append(html);

            this.loadingScreen = $('#wcp-editor-loading-screen');
        }
        if (!this.loadingScreen) {
            this.loadingScreen = $('#wcp-editor-loading-screen');
        }

        this.loadingScreen.css({ display: 'flex' });

        // Change icon
        $('#wcp-editor-loading-screen-icon').html('<i class="fa fa-circle-o-notch fa-spin"></i>');

        // Change text
        $('#wcp-editor-loading-screen-text').html(text);

        var self = this;
        setTimeout(function() {
            self.loadingScreen.addClass('wcp-editor-loading-screen-visible');
        }, 10);
    }
    WCPEditor.prototype.updateLoadingScreenMessage = function(text) {
        $('#wcp-editor-loading-screen-text').html(text);
    };
    WCPEditor.prototype.hideLoadingScreen = function() {
        if (!this.loadingScreen) {
            this.loadingScreen = $('#wcp-editor-loading-screen');
        }
        this.loadingScreen.removeClass('wcp-editor-loading-screen-visible');

        var self = this;
        this.loadingScreenTimeout = setTimeout(function() {
            self.loadingScreen.hide();
        }, 250);
    }
    WCPEditor.prototype.hideLoadingScreenWithText = function(text, error, manualClose) {
        var self = this;

        // Change text
        if (manualClose) {
            text += '<div class="wcp-editor-control-button" id="button-loading-screen-close">Close</div>';
        }

        $('#wcp-editor-loading-screen-text').html(text);

        // Change icon
        if (error) {
            $('#wcp-editor-loading-screen-icon').html('<i class="fa fa-times"></i>');
        } else {
            $('#wcp-editor-loading-screen-icon').html('<i class="fa fa-check"></i>');
        }

        if (!manualClose) {
            setTimeout(function() {
                self.hideLoadingScreen();
            }, 1000);
        }
    }
    WCPEditor.prototype.selectTool = function(toolName) {
        $('.wcp-editor-toolbar-button').removeClass('wcp-active');
        $('[data-wcp-editor-toolbar-button-name="'+ toolName +'"]').addClass('wcp-active');

        $.wcpEditorEventSelectedTool(toolName);
    }
    WCPEditor.prototype.setPreviewModeOn = function() {
        $('#wcp-editor-button-preview').addClass('wcp-active');
    }
    WCPEditor.prototype.setPreviewModeOff = function() {
        $('#wcp-editor-button-preview').removeClass('wcp-active');
    }
    WCPEditor.prototype.showExtraMainButton = function(buttonName) {
        // Shows an extra main button using the button's name 
        // as specified during initialization

        $('[data-wcp-editor-extra-main-button-name=' + buttonName + ']').show();
    }
    WCPEditor.prototype.hideExtraMainButton = function(buttonName) {
        // Hides an extra main button using the button's name 
        // as specified during initialization

        $('[data-wcp-editor-extra-main-button-name=' + buttonName + ']').hide();
    }

    function WCPEditorForm(options) {
        this.options = options;

        this.id = 'wcp-form-' + (Math.floor(Math.random() * 9999) + 1);

        // Contains a reference to each WCPEditorControl object
        this.controls = [];

        // Callback function for when a control changes its value
        this.formUpdated = undefined;

        // Assoc array of all control values
        this.model = {};

        this.selectedTab = 0;
    };
    WCPEditorForm.prototype.init = function() {
        // Create WCPEditorControl objects
        // Iterate over control groups
        for (var i=0; i<this.options.controlGroups.length; i++) {

            // Iterate over controls in each group
            for (var j=0; j<this.options.controlGroups[i].controls.length; j++) {
                var controlOptions = this.options.controlGroups[i].controls[j];
                var controlRegisteredSettings = $.extend(true, {}, registeredControls[controlOptions.type]);

                var self = this;
                var c = new WCPEditorControl(controlOptions, controlRegisteredSettings, function() {
                    self.controlUpdated(this.name);
                });

                c.setVal(controlOptions.value);

                this.controls[controlOptions.name] = c;
            }
        }

        // Create events
        this.events();
    };
    WCPEditorForm.prototype.events = function(controls) {
        var self = this;

        // Tab functionality
        $(document).on('click', '#' + this.id + ' [data-wcp-form-tab-button-name]', function() {
            var name = $(this).data('wcp-form-tab-button-name');
            self.openFormTabWithName(name);
        });
    }
    WCPEditorForm.prototype.openFormTabWithName = function(tabName) {
        var formRoot = $('#' + this.id);

        formRoot.find('.wcp-editor-form-tab-content').hide();
        formRoot.find('[data-wcp-form-tab-content-name="'+ tabName +'"]').show();

        formRoot.find('.wcp-editor-form-tab-button').removeClass('wcp-active');
        formRoot.find('[data-wcp-form-tab-button-name="'+ tabName +'"]').addClass('wcp-active');

        this.updateForm();

        this.selectedTab = formRoot.find('[data-wcp-form-tab-button-name="'+ tabName +'"]').data('wcp-form-tab-button-index');
    };
    WCPEditorForm.prototype.getFormHTML = function() {
        var html = '';
        var tabsHTML = '';
        var tabsContentsHTML = '';

        tabsHTML += '<div class="wcp-editor-form-tabs-wrap">';
        tabsContentsHTML += '<div class="wcp-editor-form-tabs-content-wrap">';

        // Iterate over control groups
        for (var i=0; i<this.options.controlGroups.length; i++) {
            var controlGroup = this.options.controlGroups[i];
            var buttonClass = '';

            if (i == this.selectedTab) buttonClass = 'wcp-active';

            // Add a tab button
            tabsHTML += '<div class="wcp-editor-form-tab-button '+ buttonClass +'" data-wcp-form-tab-button-name="'+ controlGroup.groupName +'" data-wcp-form-tab-button-index="'+ i +'">';
            tabsHTML += '   <div class="wcp-editor-form-tab-button-icon"><i class="'+ controlGroup.groupIcon +'" aria-hidden="true"></i></div>';
            tabsHTML += '   <div class="wcp-editor-form-tab-button-text">'+ controlGroup.groupTitle +'</div>';
            tabsHTML += '</div>';

            // Create a tab container for the controls
            var contentStyle = 'display: none;';

            if (i == this.selectedTab) contentStyle = '';

            tabsContentsHTML += '<div class="wcp-editor-form-tab-content" data-wcp-form-tab-content-name="'+ controlGroup.groupName +'" style="'+ contentStyle +'">';

            // Iterate over controls in each group
            for (var j=0; j<controlGroup.controls.length; j++) {
                var control = controlGroup.controls[j];

				if (control.render !== false) {
					var tooltipAttributes = '';
	                if (control.tooltip) {
	                    tooltipAttributes = 'data-wcp-tooltip="'+ control.tooltip.text +'" data-wcp-tooltip-position="'+ control.tooltip.position +'"';
	                }

	                tabsContentsHTML += '<div class="wcp-editor-form-control" id="wcp-editor-form-control-'+ control.name +'" '+ tooltipAttributes +'>';

	                if (!this.controls[control.name].customLabel) {
	                    tabsContentsHTML += '   <label>'+ control.title +'</label>';
	                }
	                tabsContentsHTML += this.controls[control.name].HTML();
	                tabsContentsHTML += '</div>';
				}
            }

            // Close the tab container
            tabsContentsHTML += '</div>';
        }

        tabsContentsHTML += '</div>';
        tabsHTML += '</div>';

        html = '<div class="wcp-editor-form-wrap" id="'+ this.id +'">' + tabsHTML + tabsContentsHTML + '</div>';

        return html;
    };
    WCPEditorForm.prototype.controlUpdated = function(controlName) {
        $.wcpEditorEventFormUpdated(this.options.name, controlName);
    }
    WCPEditorForm.prototype.updateForm = function() {
        for (var c in this.controls) {
            this.controls[c].loadVal();
        }
    }
    WCPEditorForm.prototype.getModel = function() {
        var model = {};

        for (var i=0; i<this.options.controlGroups.length; i++) {
            var controlGroupName = this.options.controlGroups[i].groupName;

            model[controlGroupName] = {};

            // Iterate over controls in each group
            for (var j=0; j<this.options.controlGroups[i].controls.length; j++) {
                var controlName = this.options.controlGroups[i].controls[j].name;
                var controlValue = this.controls[controlName].getVal();

                model[controlGroupName][controlName] = controlValue;
            }
        }

        return model;
    }
    WCPEditorForm.prototype.setControlValue = function(controlName, v) {
        if (this.controls[controlName] && this.controls[controlName].getVal() !== v) {
            this.controls[controlName].setVal(v);
        }
    }
    WCPEditorForm.prototype.showControlsGroup = function(groupName) {
        var formRoot = $('#' + this.id);

        $('[data-wcp-form-tab-button-name="'+ groupName +'"]').show();
    }
    WCPEditorForm.prototype.hideControlsGroup = function(groupName) {
        var formRoot = $('#' + this.id);

        formRoot.find('[data-wcp-form-tab-button-name="'+ groupName +'"]').hide();

        if (this.selectedTab == formRoot.find('[data-wcp-form-tab-button-name="'+ groupName +'"]').data('wcp-form-tab-button-index')) {
            this.selectedTab = 0;

            this.openFormTabWithName(formRoot.find('[data-wcp-form-tab-button-index="0"]').data('wcp-form-tab-button-name'));
        }
    }
    WCPEditorForm.prototype.showControl = function(controlName) {
        var formRoot = $('#' + this.id);

        formRoot.find('#wcp-editor-form-control-' + controlName).show();
    }
    WCPEditorForm.prototype.hideControl = function(controlName) {
        var formRoot = $('#' + this.id);

        formRoot.find('#wcp-editor-form-control-' + controlName).hide();
    }
    WCPEditorForm.prototype.addControl = function(controlGroupName, controlOptions) {
        // Add the control to the form's options
        for (var i=0; i<this.options.controlGroups.length; i++) {
            var controlGroup = this.options.controlGroups[i];

            if (controlGroup.groupName == controlGroupName) {
                controlGroup.controls.push(controlOptions);
                break;
            }
        }

        // Create the WCPEditorControl object and add it to this.controls
        var controlRegisteredSettings = $.extend(true, {}, registeredControls[controlOptions.type]);

        var self = this;
        var c = new WCPEditorControl(controlOptions, controlRegisteredSettings, function() {
            self.controlUpdated(this.name);
        });

        c.setVal(controlOptions.value);

        this.controls[controlOptions.name] = c;
    };
    WCPEditorForm.prototype.removeControl = function(controlName) {
        // Delete it from the list of Controls
        delete this.controls[controlName];

        // Delete it from the options array
        for (var i=0; i<this.options.controlGroups.length; i++) {
            var controlGroup = this.options.controlGroups[i];
            var done = false;
            for (var j=0; j<controlGroup.controls.length; j++) {
                var control = controlGroup.controls[j];

                if (control.name == controlName) {
                    controlGroup.controls.splice(j, 1);
                    done = true;
                    break;
                }
            }

            if (done) break;
        }
    };

    function WCPEditorControl(controlOptions, controlRegisteredSettings, valueUpdated) {
        // The 's' argument is the array coming from the registeredControls array
        // Automatically generated at the time of object creation
        this.id = Math.floor(Math.random() * 9999) + 1;
        this.elementID = 'wcp-editor-control-' + this.id;
        this.elementClass = 'sq-element-option-group';

        // Settings coming from the registered controls catalog
        // referenced in the 'this' variable, so 'this' can be accessed within
        // those functions (in case of validate(), HTML(), events(), etc)
        // These settings are also common in all controls
        this.type = controlRegisteredSettings.type;
        this.getValue = controlRegisteredSettings.getValue;
        this.setValue = controlRegisteredSettings.setValue;
        this.HTML = controlRegisteredSettings.HTML;

        // These variables are specific for each individual control
        this.name = controlOptions.name;
        this.title = controlOptions.title;
        this.options = controlOptions.options;

        // Private property, must be accessed only via setter and getter
        this._value = undefined;

        // Launch the events provided from the settings
        this.init = controlRegisteredSettings.init;
        this.init();

        // Create a callback function for when the control updates its value
        this.valueUpdated = valueUpdated;

        // Inline label flag
        this.customLabel = controlRegisteredSettings.customLabel;
    }
    WCPEditorControl.prototype.getVal = function() {
        return this._value;
    }
    WCPEditorControl.prototype.setVal = function(v) {
        this._value = v;

        try {
            this.setValue(v);
        } catch (err) {
            console.log(err);
        }
    }
    WCPEditorControl.prototype.loadVal = function() {
        this.setValue(this._value);
    }
    WCPEditorControl.prototype.valueChanged = function() {
        // Re-sets the control to its stored value
        this._value = this.getValue();
        this.valueUpdated();
    }

    // Utility
    function clearSelection() {
        if (document.selection) {
            document.selection.empty();
        } else if (window.getSelection) {
            window.getSelection().removeAllRanges();
        }
    }

    // API =====================================================================

    // Basic initialization of the editor. Builds UI.
    $.wcpEditorInit = function(options) {
        var defaultOptions = {
            canvasFill: false,
            canvasWidth: 800,
            canvasHeight: 600,
            mainTabs: [], // Objects { name: 'Name', icon: 'fa fa-icon-name', title: 'The Title' }
            toolbarButtons: [], // Objects { name: 'Name', icon: 'fa fa-icon-name', title: 'The Title' }
            extraMainButtons: [], // Objects { name: 'Name', icon: 'fa fa-icon-name', title: 'The Title' }
            listItemButtons: [], // Objects { name: 'Name', icon: 'fa fa-icon-name', title: 'The Title' }
            newButton: true,
            previewToggle: true
        };
        wcpEditor = new WCPEditor();
        wcpEditor.init($.extend(true, {}, defaultOptions, options));
    };

    // Provide a declaration for a control that can later be used in a form
    // as a WCPEditorControl object
    $.wcpEditorRegisterControl = function(options) {
        registeredControls[options.type] = options;
    };

    // A form is created only as an object, does not exist in the DOM
    // It initializes its own WCPEditorControl objects
    $.wcpEditorCreateForm = function(options) {
        wcpForms[options.name] = new WCPEditorForm(options);
        wcpForms[options.name].init();
    };

    // The form will try to re-set the values of all its controls
    $.wcpEditorUpdateForm = function(formName) {
        wcpForms[formName].updateForm();
    };

    // Add/remove controls from a form
    $.wcpEditorFormAddControl = function(formName, controlGroupName, controlOptions) {
        wcpForms[formName].addControl(controlGroupName, controlOptions);
    }
    $.wcpEditorFormRemoveControl = function(formName, controlName) {
        wcpForms[formName].removeControl(controlName);
    }

    // Opens a specific form tab
    $.wcpEditorFormOpenTab = function(formName, tabName) {
        wcpForms[formName].openFormTabWithName(tabName);
    };

    // Generates HTML code for a form with formName
    $.wcpEditorGetHTMLForFormWithName = function(formName) {
        return wcpForms[formName].getFormHTML();
    };

    // Returns an assoc array containing control values
    $.wcpEditorGetModelOfFormWithName = function(formName) {
        return wcpForms[formName].getModel();
    }

    // Sets a new value for a control with controlName in a form with formName
    $.wcpEditorSetControlValue = function(formName, controlName, v) {
        wcpForms[formName].setControlValue(controlName, v);
    }

    // Functions to show/hide controls or control groups(tabs)
    $.wcpEditorFormShowControlsGroup = function(formName, groupName) {
        wcpForms[formName].showControlsGroup(groupName);
    }
    $.wcpEditorFormHideControlsGroup = function(formName, groupName) {
        wcpForms[formName].hideControlsGroup(groupName);
    }
    $.wcpEditorFormShowControl = function(formName, controlName) {
        wcpForms[formName].showControl(controlName);
    }
    $.wcpEditorFormHideControl = function(formName, controlName) {
        wcpForms[formName].hideControl(controlName);
    }

    // Inserts content in a main tab
    $.wcpEditorSetContentForTabWithName = function(tabName, content) {
        wcpEditor.setContentForTabWithName(tabName, content);
    };

    // Opens a main tab with tabName
    $.wcpEditorOpenMainTabWithName = function(tabName) {
        wcpEditor.openMainTabWithName(tabName);
    };

    // Inserts content in the canvas
    $.wcpEditorSetContentForCanvas = function(content) {
        wcpEditor.setContentForCanvas(content);
    };

    // Updates list items
    $.wcpEditorSetListItems = function(listItems) {
        wcpEditor.setListItems(listItems);
    }

    // Selects a list item
    $.wcpEditorSelectListItem = function(listItemId) {
        wcpEditor.selectListItem(listItemId);
    }

    // Selects a tool
    $.wcpEditorSelectTool = function(toolName) {
        wcpEditor.selectTool(toolName);
    }

    // Present loading screen
    $.wcpEditorPresentLoadingScreen = function(text) {
        wcpEditor.presentLoadingScreenWithText(text);
    }
    $.wcpEditorUpdateLoadingScreenMessage = function(text) {
        wcpEditor.updateLoadingScreenMessage(text);
    }
    $.wcpEditorHideLoadingScreen = function() {
        wcpEditor.hideLoadingScreen();
    }
    $.wcpEditorHideLoadingScreenWithMessage = function(text, error, manualClose) {
        wcpEditor.hideLoadingScreenWithText(text, error, manualClose);
    }

    // Present load modal
    $.wcpEditorPresentLoadModal = function() {
        wcpEditor.presentLoadModal();
    }

    // Present modal
    $.wcpEditorPresentModal = function(options) {
        var modalDefaults = {
            title: '',
            buttons: [

            ],
            body: ''
        };

        wcpEditor.presentModal($.extend(true, {}, modalDefaults, options));
    }

    // Close modal
    $.wcpEditorCloseModal = function() {
        wcpEditor.closeModal();
    }

    // Set preview mode
    $.wcpEditorSetPreviewModeOn = function() {
        wcpEditor.setPreviewModeOn();
    }
    $.wcpEditorSetPreviewModeOff = function() {
        wcpEditor.setPreviewModeOff();
    }

    // Show/hide extra main buttons
    $.wcpEditorShowExtraMainButton = function(buttonName) {
        wcpEditor.showExtraMainButton(buttonName);
    }
    $.wcpEditorHideExtraMainButton = function(buttonName) {
        wcpEditor.hideExtraMainButton(buttonName);
    }

    // BOILERPLATE CODE FOR IMPLEMENTING REQUIRED API FUNCTIONS ****************
    // *************************************************************************

    // [data source] Called on initialization:
    $.wcpEditorGetContentForTabWithName = function(tabName) {

    }
    $.wcpEditorGetContentForCanvas = function() {

    }
    $.wcpEditorGetListItems = function() {
        // Returns an array of objects in the format { id: 'id', title: 'title' }
    }
    // [data source] Get a list of saves
    $.wcpEditorGetSaves = function(callback) {
        // Format: [ { name: 'name', id: 'id' }, ... ]

    }
    // [data source] Provide encoded JSON for export
    $.wcpEditorGetExportJSON = function() {
        return '{}';
    }

    // Form events
    $.wcpEditorEventFormUpdated = function(formName, controlName) {

    }

    // Main button events
    $.wcpEditorEventNewButtonPressed = function() {

    }
    $.wcpEditorEventSaveButtonPressed = function() {

    }
    $.wcpEditorEventLoadButtonPressed = function() {

    }
    $.wcpEditorEventUndoButtonPressed = function() {

    }
    $.wcpEditorEventRedoButtonPressed = function() {

    }
    $.wcpEditorEventPreviewButtonPressed = function() {

    }
    $.wcpEditorEventEnteredPreviewMode = function() {

    }
    $.wcpEditorEventExitedPreviewMode = function() {

    }

    // List events
    $.wcpEditorEventListItemMouseover = function(itemID) {

    }
    $.wcpEditorEventListItemSelected = function(itemID) {

    }
    $.wcpEditorEventListItemMoved = function(itemID, oldIndex, newIndex) {

    }
    $.wcpEditorEventListItemButtonPressed = function(itemID, buttonName) {

    }
    $.wcpEditorEventListItemTitleButtonPressed = function(buttonName) {

    }

    // Tool events
    $.wcpEditorEventSelectedTool = function(toolName) {

    }
    $.wcpEditorEventPressedTool = function(toolName) {

    }

    // Extra main button events
    $.wcpEditorEventExtraMainButtonClick = function(buttonName) {

    }

    // Modal events
    $.wcpEditorEventModalButtonClicked = function(modalName, buttonName) {

    }
    $.wcpEditorEventModalClosed = function(modalName) {

    }

    // Create new event
    $.wcpEditorEventCreatedNewInstance = function(instanceName) {

    }

    // Event for loading a save
    $.wcpEditorEventLoadSaveWithID = function(saveID) {

    }

    // Event for deleting a save
    $.wcpEditorEventDeleteSaveWithID = function(saveID) {

    }

    // Event for importing
    $.wcpEditorEventImportedJSON = function(parsedJSON) {

    }

    // Event for help button
    $.wcpEditorEventHelpButtonPressed = function() {

    }

})(jQuery, window, document);
