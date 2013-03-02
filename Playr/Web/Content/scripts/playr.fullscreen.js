/// <reference path="jquery-1.9.1.js" />
/// <reference path="playr.js" />
/// <reference path="jquery.signalr-1.0.0.js" />
/// <reference path="knockout-2.2.1.js" />

models.fullscreen = {
    ViewModel: function () {
        var self = this;
        self.albumName = ko.observable("");
        self.artistName = ko.observable("");
        self.name = ko.observable("");
        self.totalTime = ko.observable(0);
        self.currentTime = ko.observable(0);
        self.TimeRemaining = ko.computed(function () {
            var time = self.totalTime() - self.currentTime();
            return time < 0 ? 0 : time;
        });
        self.fanart = ko.observableArray([]);
        self.albumArtUrl = ko.observable("/images/albumArt.jpg");
        self.updateTrack = function (data) {
            self.albumName(data.track.albumName);
            self.artistName(data.track.artistName);
            self.name(data.track.name);
            self.totalTime(data.track.time);
            self.currentTime(data.currentTime);
            // Crazy awesome fanart stuff
            helpers.SortRandom(data.fanart);
            self.fanart(data.fanart);
            if (data.track.links) {
                $.each(data.track.links, function () {
                    if(this.rel === "artwork")
                        self.albumArtUrl(this.href);
                });
            }
            if ($(".fanart").cycle("widget")) {
                $(".fanart").cycle("destory");
            }
            if (self.fanart && self.fanart.length > 1) {
                $(".fanart").cycle({
                    fx: 'fade',
                    speed: 3000,
                    timeout: 15000
                });
            }
            // Animate the progress bard
            $("#progress").stop(true, true)
                .width(((self.currentTime() / self.totalTime()) * 100) + "%")
                .animate({ width: "100%" }, self.TimeRemaining(), "linear");
            // Kick off time countdown
            if (self.timer) {
                clearInterval(self.timer);
            }
            self.timer = setInterval(function () {
                var newTime = self.currentTime() + 1000;
                if (newTime > self.totalTime()) {
                    newTime = self.totalTime();
                    clearInterval(self.timer);
                }
                self.currentTime(newTime);
            }, 1000);
        };
        
    }
};

$(function () {
    var model = new models.fullscreen.ViewModel(),
        jqhxr;
    
    ko.applyBindings(model);
      
    var hub = $.connection.notificationHub;
    $.extend(hub.client, {
        CurrentTrackChanged: function (track) {
            model.updateTrack(track);
        }
    });
    $.connection.hub.start();

    jqxhr = $.getJSON("/api/info/current", model.updateTrack);
});
