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
        self.fanart = ko.observableArray([]);
        self.albumArtUrl = ko.observable("/images/albumArt.jpg");
        self.AlbumUrl = ko.observable("");
        self.download = ko.observable("");
        self.updateTrack = function (data) {
            self.albumName(data.track.albumName);
            self.artistName(data.track.artistName);
            self.name(data.track.name);
            self.totalTime(data.track.time);
            self.currentTime(data.currentTime);
            self.fanart(data.fanart);
            // Get all the links figured out
            $.each(data.track.links, function () {
                switch (this.rel) {
                    case "artwork" :
                        self.albumArtUrl(this.href);
                        break;
                    case "album" :
                        self.AlbumUrl(this.href);
                        break;
                    case "download":
                        self.download(this.href);
                        break;
                }
            });
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
