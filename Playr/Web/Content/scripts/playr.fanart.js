/// <reference path="jquery-ui-1.10.1.js" />
/// <reference path="jquery-1.9.1.js" />
/// <reference path="knockout-2.2.1.js" />

models.widgets.fanart = function (urls) {
    var self = this;
    self.artwork = ko.isObservable(urls) ? urls : ko.observableArray(urls);
    self.destory = function () {

    };
};

(function ($, undefined) {
    "use strict";

    $.widget("playr.fanart", {
        html: "<section data-bind='foreach: artwork' class='fanart'>" +
                  "<div data-bind=\"style: { backgroundImage: ('url('+ $data + ')') }\" />" +
              "</section>",
        model: null,
        _create: function () {
            var $element = $(this.element),
                $html = $(this.html);

            self.model = new models.widgets.fanart(this.options.urls);
            
            $element.html($html);
            ko.applyBindings(model, $html[0]);
        },
        destory: function () {
            var $element = $(this.element);
            this.model.destory();
            ko.cleanNode($element);
            base.destroy.call(this);
        }
    });
})(jQuery);