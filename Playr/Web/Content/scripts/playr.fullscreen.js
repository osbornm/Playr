/// <reference path="jquery-1.9.1.js" />
/// <reference path="playr.js" />
/// <reference path="knockout-2.2.1.js" />

models.fullscreen = {
    ViewModel: function () {
        var self = this;
        self.albumName = ko.observable("");
        self.artistName = ko.observable("");
        self.name = ko.observable("");
        self.totalTime = ko.observable("");
        self.currentTime = ko.observable("00:00");
        self.fanart = ko.observableArray([]);
        self.albumArtUrl = ko.observable("/images/albumArt.jpg");
        self.updateTrack = function (data) {
            self.albumName(data.track.albumName);
            self.artistName(data.track.artistName);
            self.name(data.track.name);
            self.totalTime(data.track.time);
            self.currentTime(data.currentTime);
            self.fanart(data.fanart);
        };
    }
};

$(function () {
    var model = new models.fullscreen.ViewModel(),
        jqhxr;
    
    ko.applyBindings(model);
    
    jqxhr = $.getJSON("/api/info/current", model.updateTrack);
});
