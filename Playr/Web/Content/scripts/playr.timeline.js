﻿/// <reference path="jquery-ui-1.10.1.js" />
/// <reference path="jquery-1.9.1.js" />
/// <reference path="knockout-2.2.1.js" />

models.widgets.timeline = function (currentPosition, total) {
    var self = this;
    self.currentPosition = ko.isObservable(currentPosition) ? currentPosition : ko.observable(currentPosition);
    self.total = ko.isObservable(total) ? total : ko.observable(total);
    self.progressWidth = ko.computed(function () {
        return ((self.currentPosition() / self.total()) * 100) + "%"
    });

    // Timer Logic
    if (self.timer) {
        clearInterval(self.timer);
    }
    self.tickInterval = 100;
    self.tick = function () {
        var newTime = self.currentPosition() + self.tickInterval;
        if (newTime > self.total()) {
            newTime = self.total();
            clearInterval(self.timer);
        }
        self.currentPosition(newTime);
    };
    self.timer = setInterval(function () {
        self.tick.call(this);
    }, self.tickInterval);

    self.destory = function () {
        clearInterval(self.timer);
    }
};

(function ($, undefined) {
    "use strict";

    $.widget("playr.timeline", {
        html: "<section class='playr-timeline'>" +
                  "<div class='timelineText'>" +
                       "<h3 class='currentPosition' data-bind='time: currentPosition'></h3>" +
                       "<h3 class='trackLength' data-bind='time: total'></h3>" +
                  "</div>" +
                  "<div class='progressBar'>" +
                       "<div class='progress' data-bind='style:{ width: progressWidth }'></div>" +
                  "</div>" +
              "</section>",
        model: null,
        _create: function () {
            var $element = $(this.element),
                $html = $(this.html);

            this.model = this._createModel(this.options);

            $element.html($html);
            ko.applyBindings(this.model, $html[0]);
        },
        _createModel: function (options) {
            return new models.widgets.timeline(options.currentPosition, options.total);
        },
        destory: function () {
            var $element = $(this.element);
            this.model.destory();
            ko.cleanNode($element);
            $element.empty();
            base.destroy.call(this);
        }
    });
})(jQuery);