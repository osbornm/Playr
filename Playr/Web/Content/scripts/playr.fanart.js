/// <reference path="jquery-1.9.1.js" />
/// <reference path="jquery-ui-1.10.1.js" />
/// <reference path="knockout-2.2.1.js" />

models.widgets.fanart = function (urls, element) {
    var self = this;
    self.artwork = ko.isObservable(urls) ? urls : ko.observableArray(urls);
    if (self.artwork().length < 1) {
        self.artwork().push("/images/defaultFanart.jpg");
    }
    self.element = $(element);
    self.currentIndex = 0;
    self.tickIterval = 5000;
    self.tick = function () {
        if (self.artwork().length > 1) {
            var current = self.element.find("div:visible"),
                next = self.element.find("div:visible + div");
            if (next.length < 1) {
                next = self.element.children().first();
            }

            var animation = Math.floor(Math.random() * (3 + 1));
            // for now we like crossfade only
            animation = 3;
            switch (animation) {
                // Slide Up
                case 0:
                    current.css("zIndex", -1).slideUp({ duration: 700, queue: false }).fadeOut({ duration: 1000, queue: false });
                    next.css("zIndex", -2).fadeIn(1000);
                    break;
                // Slide Right
                case 1:
                    current.css("zIndex", -2).fadeOut(2000);
                    next.css("zIndex", -1).fadeIn({ duration: 1000, queue: false }).effect("slide", { duration: 1000, queue: false });
                    break;
                // Clip
                case 2:
                    current.css("zIndex", -1).fadeOut({ duration: 1500, queue: false }).effect("clip", { duration: 1000, queue: false });
                    next.css("zIndex", -2).fadeIn({ duration: 1500, queue: false })
                    break;
                // Cross Fade
                default:
                    current.css("zIndex", -1).fadeOut({ duration: 2000, queue: false });
                    next.css("zIndex", -2).fadeIn({ duration: 2000, queue: false });
            }
        }
    }
    self.destory = function () {
        clearInterval(self.timer);
    };

    self.timer = setInterval(self.tick, self.tickIterval);
};

(function ($, undefined) {
    "use strict";

    $.widget("playr.fanart", {
        html: "<section data-bind='foreach: artwork' class='fanart'>" +
                  "<div data-bind=\"visible: ($index() === 0), style: { backgroundImage: ('url('+ $data + ')') }\" />" +
              "</section>" +
              "<div class='gradient'></div>",
        model: null,
        _create: function () {
            var $element = $(this.element),
                $html = $(this.html);

            this.model = new models.widgets.fanart(this.options.urls, $html);

            $element.html($html);
            ko.applyBindings(this.model, $html[0]);
        },
        destory: function () {
            var $element = $(this.element);
            this.model.destory();
            ko.cleanNode($element);
            base.destroy.call(this);
        }
    });
})(jQuery);