/*jshint strict: true */
/*global jQuery */

(function ($, global, undefined) {
    "use strict";

    $.widget("playr.scrollbar", {
        options: {
            autoRefresh: 0 // time in milliseconds to refresh on.
        },
        _html: "<div class='playr-scrollbar scrollable-overflow'><div class='scrollable-area'></div></div>",
        _propNames: {
            vertical: {
                scrollPosition: "scrollTop",
                scrollSize: "scrollHeight",
                clientSize: "clientHeight",
                page: "pageY",
                position: "top",
                size: "height"
            },
            horizontal: {
                scrollPosition: "scrollLeft",
                scrollSize: "scrollWidth",
                clientSize: "clientWidth",
                page: "pageX",
                position: "left",
                size: "width"
            }
        },
        _pushBy: 40,
        _scrollbarSize: 16,
        _trackOverlap: 10,

        // refresh should be called when new items are added to the scroll area to cause a recalc.
        refresh: function () {
            // We need to remove all the stuff we did to go back to the default browser
            // styles so we can recalc everything... Yes it's fast because we dont actually
            // destory the nodes just remove them from the DOM. 
            this._resetStyles();

            // redo most of _create, we should refactor the common code
            this.original.width = this.element.width();
            this.original.height = this.element.height();
            this.original.isHorizontalScrolling = this._isHorizontalScrolling(this.element.css("overflowX"), this.element);
            this.original.isVerticalScrolling = this._isVerticalScrolling(this.element.css("overflowY"), this.element);
            this.element.wrap(this._html);
            this._scrollableArea = this.element.parent();
            this._container = this._scrollableArea.parent();
            this._savedProperties = this._getAndResetProperties(this.element);
            this._container.width(this.original.width).height(this.original.height);
            this._transferProperties(this._container, this._savedProperties);

            // add back the tracks that were dettached
            this._container.append(this._track.x)
                           .append(this._track.y);
            this._attachScroll();

            // scroll to the right place and show the tracks
            this._scrollableArea
                .scrollLeft(this._scrollableArea.scrollLeft())
                .scrollTop(this._scrollableArea.scrollTop())
                .scroll();

            if (this.original.isHorizontalScrolling) {
                this._track.x.show();
            }

            if (this.original.isVerticalScrolling) {
                this._track.y.show();
            }
        },

        // cleans everything up and leave the dom the way it was before the widget.
        destroy: function () {
            this._resetStyles();

            if (this._resizeHandler !== null) {
                $(global).off("resize", this._resizeHandler);
                this._resizeHandler = null;
            }

            if (this.autoRefresh) {
                global.clearInterval(this.autoRefresh.handler);
            }

            $.Widget.prototype.destroy.call(this);
        },

        _create: function () {
            // Don't accept true, people must specify a number of milliseconds
            if (typeof this.options.autoRefresh !== "number") {
                throw "AutoRefresh must be a number in milliseconds";
            }

            this.original = {};
            this.original.width = this.element.width();
            this.original.height = this.element.height();
            this.original.isHorizontalScrolling = this._isHorizontalScrolling(this.element.css("overflowX"), this.element);
            this.original.isVerticalScrolling = this._isVerticalScrolling(this.element.css("overflowY"), this.element);


            this.element.wrap(this._html);
            this._scrollableArea = this.element.parent();
            this._container = this._scrollableArea.parent();

            this._savedProperties = this._getAndResetProperties(this.element);

            this._container.width(this.original.width).height(this.original.height);

            this._transferProperties(this._container, this._savedProperties);

            this._createTracks();
            this._attachScroll();

            this._scrollableArea
                .scrollLeft(this._scrollableArea.scrollLeft())
                .scrollTop(this._scrollableArea.scrollTop())
                .scroll();

            if (this.original.isHorizontalScrolling) {
                this._track.x.show();
            }

            if (this.original.isVerticalScrolling) {
                this._track.y.show();
            }

            // setup resize handler and auto refresh handler
            var that = this;
            this._resizeHandler = function () { that.refresh.apply(that, arguments); };
            $(global).on("resize", this._resizeHandler);
            if (this.options.autoRefresh > 0) {
                this.autoRefresh = {};
                this.autoRefresh.width = this.element[0].scrollWidth;
                this.autoRefresh.height = this.element[0].scrollHeight;
                this.autoRefresh.handler = global.setInterval(function () { that._autoRefresh.call(that); }, this.options.autoRefresh);
            }

        },

        _createTracks: function () {
            // Huge Tracks of land...
            this._track = {};

            this._track.x = $("<div class='scrollbar-track axis-x'><div class='scrollbar-handle'></div></div>")
                .find(".scrollbar-handle")
                    .click(false)
                    .mousedown(this._trackHandleHandler(this, this._propNames.horizontal))
                .end()
                .click(this._trackHandler(this, this._propNames.horizontal))
                .hide();

            this._track.y = $("<div class='scrollbar-track axis-y'><div class='scrollbar-handle'></div></div>")
                .find(".scrollbar-handle")
                    .click(false).mousedown(this._trackHandleHandler(this, this._propNames.vertical))
                .end()
                .click(this._trackHandler(this, this._propNames.vertical))
                .hide();

            this._container.append(this._track.x)
                           .append(this._track.y);
        },

        _attachScroll: function () {
            var plusWidth = 0,
                plusHeight = 0,
                trackWidth = this._track.y.width(),
                trackHeight = this._track.x.height();

            //TODO: overlay option

            this.element
                .css({ width: "", height: "" })
                .width(
                    Math.max(this.original.width, this.element[0].scrollWidth) -
                    (!this.options.overlay &&
                        this.original.isVerticalScrolling &&
                        !this.original.isHorizontalScrolling ? trackWidth : 0)
                ).height(
                    Math.max(this.original.height, this.element[0].scrollHeight) -
                    (!this.options.overlay &&
                      this.original.isHorizontalScrolling &&
                     !this.original.isVerticalScrolling ? trackHeight : 0));

            if (this.original.isHorizontalScrolling) {
                plusHeight = this._pushBy;
            }

            if (this.original.isVerticalScrolling) {
                plusWidth = this._pushBy;
            }

            if (this.original.isVerticalScrolling && this.original.isHorizontalScrolling) {
                this.element.width(this.element.width() + trackWidth + plusWidth - this._scrollbarSize);
                this.element.height(this.element.height() + trackHeight + plusHeight - this._scrollbarSize);
            }

            this._scrollableArea
                .width(this.original.width + plusWidth)
                .height(this.original.height + plusHeight);

            this._ratio = {};
            this._setHandleSize(this._track.x.find(".scrollbar-handle"), this.original.isVerticalScrolling ? trackWidth : 0, this._propNames.horizontal);
            this._setHandleSize(this._track.y.find(".scrollbar-handle"), this.original.isHorizontalScrolling ? trackHeight : 0, this._propNames.vertical);

            // event
            var that = this;
            this._scrollableArea
                .off("scroll.playrScrollbar")
                .on("scroll.playrScrollbar", function () {
                    that._track.x.find(".scrollbar-handle").css("left", $(this).scrollLeft() / that._ratio.width);
                    that._track.y.find(".scrollbar-handle").css("top", $(this).scrollTop() / that._ratio.height);
                });
        },

        _resetStyles: function () {
            if (this._savedProperties) {
                this._transferProperties(this.element, this._savedProperties);

                this.element.css({
                    overflow: this._savedProperties.overflow ? this._savedProperties.overflow : "auto",
                    width: this._savedProperties.fluidW ? "" : this._savedProperties.cssWidth,
                    height: this._savedProperties.fluidH ? "" : this._savedProperties.cssHeight
                });
            }

            this._detachTrack();
            this.element.unwrap().unwrap();
            this._widget = null;
            this._scrollableArea = null;
        },

        _trackHandler: function (that, propNames) {
            return function (event) {
                if (event[propNames.page] === undefined) {
                    throw "The event doesn't have a proper position";
                }

                // If we click after the handle, we move 1 handle size down, otherwise up - the overlap
                var $this = $(this),
                    handle = $this.find(".scrollbar-handle"),
                    clickPosition = event[propNames.page] - $this.offset()[propNames.position],
                    handleSize = parseInt(handle[propNames.size](), 10),
                    oldScrollPosition = that.element.parent()[propNames.scrollPosition](),
                    newScrollPosition;

                if (clickPosition < handle.position()[propNames.position]) {
                    // Up or left
                    newScrollPosition = oldScrollPosition - handleSize * that._ratio[propNames.size] - that._trackOverlap;
                } else {
                    // Down or right
                    newScrollPosition = oldScrollPosition + handleSize * that._ratio[propNames.size] - that._trackOverlap;
                }

                that.element.parent()[propNames.scrollPosition](newScrollPosition);
            };
        },

        _trackHandleHandler: function (that, propNames) {
            return function (e) {
                // store the position where the mouse was when dragging began
                // so we can calculate the relative difference later...
                var handle = $(this),
                    track = handle.parents(".scrollbar-track:first"),
                    dragStart = e[propNames.page],
                    startPosition = handle.position()[propNames.position];

                // now handle all mouse movements on the page to cause the scrollbar to change...
                $(global.document)
                    .on("mousemove.playrScrollbar", function (event) {
                        // set the scrollTop to the amount moved modified by the height ratio
                        // The scroll handler will take care of moving the handle for us

                        that._scrollableArea[propNames.scrollPosition](that._ratio[propNames.size] * (event[propNames.page] - dragStart + startPosition));
                        return false;
                    })
                    .one("mouseup.playrScrollbar", function () {
                        $(global.document).off("mousemove.playrScrollbar");
                        handle.removeClass("active");
                        track.removeClass("active");

                        return false;
                    });

                handle.addClass("active");
                track.addClass("active");

                return false;
            };
        },

        _setHandleSize: function (handle, oppositeSideTrack, propNames) {
            // TODO Guessing at this, revist with two scrollbars
            var fullSize = this._scrollableArea[0][propNames.scrollSize] + oppositeSideTrack,
                actualSize = this._container[0][propNames.clientSize];

            this._ratio[propNames.size] = fullSize / actualSize;

            handle
                .css(propNames.size, Math.ceil(actualSize / this._ratio[propNames.size]))
                .css(propNames.position, this.element[propNames.scrollPosition]() / this._ratio[propNames.size]);
        },

        _isVerticalScrolling: function (overflowY, element) {
            return this._isScrolling(overflowY, element, this._propNames.vertical);
        },

        _isHorizontalScrolling: function (overflowX, element) {
            return this._isScrolling(overflowX, element, this._propNames.horizontal);
        },

        _isScrolling: function (overflow, element, propNames) {
            if (overflow === "scroll") {
                return true;
            }

            if (overflow === "hidden") {
                return false;
            }

            // Try to move the scroll area one px and see if it moves
            // this works for Auto scroll...
            var initialScroll = element[propNames.scrollPosition]();
            element
                [propNames.scrollPosition](0)
                [propNames.scrollPosition](1);
            if (element[propNames.scrollPosition]() === 1) {
                element[propNames.scrollPosition](initialScroll);
                return true;
            }

            return false;
        },

        _getAndResetProperties: function (from) {
            //TODO: there are propably a shit ton more of these 
            // we should be transfering and erasing I choose my
            // favorite and we'll deal with the rest later.

            var properties = {
                cssWidth: from[0].style.width,
                cssHeight: from[0].style.height,
                fluidW: from[0].style.width === "",
                fluidH: from[0].style.height === "",
                marginTop: from.css("marginTop"),
                marginRight: from.css("marginRight"),
                marginBottom: from.css("marginBottom"),
                marginLeft: from.css("marginLeft"),
                borderTopWidth: from.css("borderTopWidth"),
                borderTopColor: from.css("borderTopColor"),
                borderTopStyle: from.css("borderTopStyle"),
                borderRightWidth: from.css("borderRightWidth"),
                borderRightColor: from.css("borderRightColor"),
                borderRightStyle: from.css("borderRightStyle"),
                borderBottomWidth: from.css("borderBottomWidth"),
                borderBottomColor: from.css("borderBottomColor"),
                borderBottomStyle: from.css("borderBottomStyle"),
                borderLeftWidth: from.css("borderLeftWidth"),
                borderLeftColor: from.css("borderLeftColor"),
                borderLeftStyle: from.css("borderLeftStyle"),
                position: from.css("position"),
                top: from.css("top"),
                right: from.css("right"),
                bottom: from.css("bottom"),
                left: from.css("left"),
                overflowX: from.css("overflowX"),
                overflowY: from.css("overflowY")
            };

            from.css({
                margin: 0,
                borderTopWidth: 0,
                borderTopColor: "",
                borderTopStyle: "",
                borderRightWidth: 0,
                borderRightColor: "",
                borderRightStyle: "",
                borderBottomWidth: 0,
                borderBottomColor: "",
                borderBottomStyle: "",
                borderLeftWidth: 0,
                borderLeftColor: "",
                borderLeftStyle: "",
                position: "static",
                top: "",
                right: "",
                bottom: "",
                left: "",
                overflowX: "hidden",
                overflowY: "hidden"
            });

            return properties;
        },

        _transferProperties: function (to, properties) {
            to.css({
                marginTop: properties.marginTop,
                marginRight: properties.marginRight,
                marginBottom: properties.marginBottom,
                marginLeft: properties.marginLeft,
                borderTopWidth: properties.borderTopWidth,
                borderTopColor: properties.borderTopColor,
                borderTopStyle: properties.borderTopStyle,
                borderRightWidth: properties.borderRightWidth,
                borderRightColor: properties.borderRightColor,
                borderRightStyle: properties.borderRightStyle,
                borderBottomWidth: properties.borderBottomWidth,
                borderBottomColor: properties.borderBottomColor,
                borderBottomStyle: properties.borderBottomStyle,
                borderLeftWidth: properties.borderLeftWidth,
                borderLeftColor: properties.borderLeftColor,
                borderLeftStyle: properties.borderLeftStyle,
                position: properties.position,
                top: properties.top,
                right: properties.right,
                bottom: properties.bottom,
                left: properties.left
            });
            // We don't want static
            if (properties.position === "static") {
                to.css("position", "");
            }
        },

        _detachTrack: function () {
            // like remove but keep jquery info around 
            // this makes it faster when we reattach
            this._track.x.detach();
            this._track.y.detach();
            // Hide them since that is the default style
            this._track.x.hide();
            this._track.y.hide();
        },

        _autoRefresh: function () {
            if (this.element[0].scrollWidth !== this.autoRefresh.width || this.element[0].scrollHeight !== this.autoRefresh.height) {
                this.autoRefresh.width = this.element[0].scrollWidth;
                this.autoRefresh.height = this.element[0].scrollHeight;
                this.refresh();
            }
        }
    });
}(jQuery, this));