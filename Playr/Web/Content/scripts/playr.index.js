/// <reference path="jquery-1.9.1.js" />
/// <reference path="playr.js" />
/// <reference path="jquery.signalr-1.0.0.js" />
/// <reference path="knockout-2.2.1.js" />

models.index = {
    ViewModel: function () {
        var self = this;

        self.currentTrack = {
            albumName: ko.observable(""),
            artistName: ko.observable(""),
            year: ko.observable(""),
            name: ko.observable(""),
            totalTime: ko.observable(0),
            currentTime: ko.observable(0),
            fanart: ko.observableArray([]),
            albumArtUrl: ko.observable("/images/albumArt.jpg"),
            albumUrl: ko.observable(""),
            download: ko.observable(""),
        };

        self.currentTrack.albumDisplay = ko.computed(function () {
            return self.currentTrack.albumName() + " ( " + self.currentTrack.year() + " )";
        })

        self.state = ko.observable("Stopped");

        self.controlRequest = null;
        self._abortControlRequest = function () {
            if (self.controlRequest) {
                console.log("Canceling existing request");
                self.controlRequest.abort();
            }
        };

        self.playPause = function () {
            self._abortControlRequest();

            if (self.state() === "Playing") {
                self.controlRequest = $.post("/api/control/pause");
            } else {
                self.controlRequest = $.post("/api/control/resume");
            }
        };
        self.playPauseClass = ko.computed(function () {
            if (self.state() === "Playing") {
                return "playr-icon-pause";
            } else {
                return "playr-icon-play";
            }
        });
        self.playPauseTitle = ko.computed(function () {
            if (self.state() === "Playing") {
                return "Pause";
            } else {
                return "Play";
            }
        });
        self.previous = function () {
            self._abortControlRequest();
            self.controlRequest = $.post("/api/control/previous");
        };
        self.previousTitle = ko.computed(function () {
            return "Previous Song";
        });
        self.skip = function () {
            self._abortControlRequest();
            self.controlRequest = $.post("/api/control/next");
        };
        self.skipTitle = ko.computed(function () {
            return "Skip Song";
        });

        self.toogleQueue = function () {
            $("#tray").stop(true, true).toggle("slide", { direction: "down" });
        };

        self.toogleMenu = function () {
            $("#menu").stop(true, true).toggle("slide");
        };

        self.toogleAllTheThings = function () {
            if ($("#menu").is(":visible")) {
                // Hide Tray

                $("#trayContainer").fadeOut();

                $("#tray").stop(true, true).hide("slide",
                    { duration: 400, direction: "down", easing: "easeInOutCubic" });

                $("#menu").stop(true, true).hide("slide",
                    { duration: 200, direction: "up", easing: "easeInOutCubic" });

                $("#currentTrack").delay(200).fadeIn(400);
            } else {
                // Show Tray

                $("#currentTrack").fadeOut(300);

                $("#trayContainer").delay(300).fadeIn();

                $("#tray").stop(true, true).show("slide",
                    { duration: 300, direction: "down" });

                $("#menu").stop(true, true).show("slide",
                    { duration: 400, direction: "up", easing: "easeInOutCubic" }, function () {
                        $("#trayContainer").scrollbar("refresh");
                    });
            }
        };

        self.stateChanged = function (state, track) {
            self.state(state);
            if (state === "Playing") {
                $("#center-controls").timeline("start", track.currentTime);
            } else {
                $("#center-controls").timeline("pause");
            }
        };

        self.updateCurrentTrack = function (data) {
            self.currentTrack.albumName(data.track.albumName);
            self.currentTrack.artistName(data.track.artistName);
            self.currentTrack.year(data.track.year);
            self.currentTrack.name(data.track.name);
            self.currentTrack.totalTime(data.track.time);
            self.currentTrack.currentTime(data.currentTime);
            self.currentTrack.fanart(data.fanart);
            self.state(data.state);
            // Get all the links figured out
            $.each(data.track.links, function () {
                switch (this.rel) {
                    case "artwork":
                        self.currentTrack.albumArtUrl(this.href);
                        break;
                    case "album":
                        self.currentTrack.albumUrl(this.href);
                        break;
                    case "download":
                        self.currentTrack.download(this.href);
                        break;
                }
            });
        };
    }
};

$(function () {
    var model = new models.index.ViewModel(),
        jqhxr;

    ko.applyBindings(model);

    var hub = $.connection.notificationHub;
    $.extend(hub.client, {
        CurrentTrackChanged: function (currentTrack) {
            model.updateCurrentTrack(currentTrack);
        },
        StateChanged: function (state, currentTrack) {
            model.stateChanged(state, currentTrack);
        }
    });
    $.connection.hub.start();

    jqxhr = $.getJSON("/api/info/current", model.updateCurrentTrack);
});