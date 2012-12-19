var playr = {
    pad: function (number, width) {
        width -= number.toString().length;
        if (width > 0) {
            return new Array(width + (/\./.test(number) ? 2 : 1)).join('0') + number;
        }
        return number + "";
    },

    ConvetToMinSec: function (secs) {
        var hours = Math.floor(secs / (60 * 60));

        var divisor_for_minutes = secs % (60 * 60);
        var minutes = Math.floor(divisor_for_minutes / 60);

        var divisor_for_seconds = divisor_for_minutes % 60;
        var seconds = Math.ceil(divisor_for_seconds);

        return playr.pad(minutes, 2) + ":" + playr.pad(seconds, 2);
    },

    Song: function (data) {
        var self = this;
        self.Id = ko.observable(data.Id);
        self.Artist = ko.observable(data.Artist);
        self.Album = ko.observable(data.Album);
        self.Title = ko.observable(data.Title);
        self.Rating = ko.observable(data.Rating);
        self.ArtworkUrl = ko.observable(data.ArtworkUrl);
        self.IsFavorite = ko.observable(data.IsFavorite);
        self.SongDownloadUrl = ko.observable(data.DownloadUrl);
        self.AlbumDownloadUrl = ko.observable(data.AlbumDownloadUrl);
        self.Duration = ko.observable(data.Duration);
        self.DurationFormated = ko.observable(playr.ConvetToMinSec(data.Duration));
        self.Poisition = ko.observable(data.Poisition);

        self.TimeRemaining = ko.computed(function () {
            var time = self.Duration() - self.Poisition();
            return playr.ConvetToMinSec(time >= 1 ? time : 0);
        });

        self.Favorite = function () {
            var url = "/songs/" + self.Id() + "/favorite";
            if (self.IsFavorite()) {
                $.ajax({ url: url, type: "DELETE" });
            }
            else {
                $.ajax({ url: url, type: "PUT" });
            }
            self.IsFavorite(!self.IsFavorite());
        };
    },
    initNewPage: function (data, hubUrl) {
        function PageViewModel(queue) {
            var self = this;
            self.CurrentTrack = ko.observable(new playr.Song(queue.CurrentTrack));
            self.History = ko.observableArray();
            self.Queue = ko.observableArray();
            self.RotateFanartTimer = null;
            self.ProgressTimer = null;

            $.each(queue.History, function (idx, item) {
                self.History.push(new playr.Song(item));
            });
            $.each(queue.Queue, function (idx, item) {
                self.Queue.push(new playr.Song(item));
            });
        }

        function SetupFanart() {
            $(".fanart").cycle("stop").fadeOut(500, function () {
                $(this).cycle("destroy").empty().fadeIn(100);
                $.getJSON("/home/Fanart?artist=" + encodeURIComponent(viewModel.CurrentTrack().Artist()), function (art) {
                    fisherYates(art);
                    $.each(art, function (idx, item) {
                        $(".fanart").append($("<div/>").css("background-image", "url(" + item + ")"));
                    });
                    if (art.length > 1) {
                        $(".fanart").cycle({
                            fx: 'fade',
                            speed: 2000,
                            timeout: 10000
                        });
                    }
                });
            });
        }

        function fisherYates(myArray) {
            var i = myArray.length;
            if (i == 0) return false;
            while (--i) {
                var j = Math.floor(Math.random() * (i + 1));
                var tempi = myArray[i];
                var tempj = myArray[j];
                myArray[i] = tempj;
                myArray[j] = tempi;
            }
        }

        function UpdateProgress() {
            var position = viewModel.CurrentTrack().Poisition(),
                length = viewModel.CurrentTrack().Duration();
            if(position <= length){
                viewModel.ProgressTimer = setTimeout(UpdateProgress, 1000);
                $("#currentPosition").text(playr.ConvetToMinSec(position >= 1 ? position : 0));

                viewModel.CurrentTrack().Poisition(position + 1);
            } else {
                clearTimeout(viewModel.ProgressTimer);
            }
        }

        function SetProgressBar(position, duration) {
            var timeLeft = (duration - position) * 1000;
            $("#progressBar #progress").stop(true, true).width(((position / duration) * 100) + "%").animate({ width: "100%" }, timeLeft, "linear");
        }

        var viewModel = new PageViewModel(data),
           hub = $.connection.playr;

        ko.bindingHandlers['flipAttr'] = {
            update: function (element, valueAccessor, allBindingsAccessor) {
                var value = ko.utils.unwrapObservable(valueAccessor()) || {};

                var updateAttr = function () {
                    for (var attrName in value) {
                        if (typeof attrName == "string") {
                            var attrValue = ko.utils.unwrapObservable(value[attrName]);
                            var toRemove = (attrValue === false) || (attrValue === null) || (attrValue === undefined);
                            if (toRemove)
                                element.removeAttribute(attrName);
                            if (ko.utils.ieVersion <= 8 && attrName in attrHtmlToJavascriptMap) {
                                attrName = attrHtmlToJavascriptMap[attrName];
                                if (toRemove)
                                    element.removeAttribute(attrName);
                                else
                                    element[attrName] = attrValue;
                            } else if (!toRemove) {
                                element.setAttribute(attrName, attrValue.toString());
                            }
                        }
                    }
                };
                $("#albumArt").transition({
                    perspective: '1000',
                    rotateX: '+=180'
                }, 500, "in", function () {
                    updateAttr();
                    $(this).transition({
                        perspective: '1000',
                        rotateX: '+=180'
                    }, 500, "ease");
                });
            }
        };

        ko.applyBindings(viewModel);

        hub.DjInfoUpdated = function () {
            $.getJSON("/home/GetQueue", function (data) {
                clearTimeout(viewModel.ProgressTimer);

                viewModel.CurrentTrack(new playr.Song(data.CurrentTrack));
                viewModel.History.removeAll();
                viewModel.Queue.removeAll();
                // TODO: This is pretty bad each push redraws, fix this...
                $.each(data.History, function (idx, item) {
                    viewModel.History.push(new playr.Song(item));
                });
                $.each(data.Queue, function (idx, item) {
                    viewModel.Queue.push(new playr.Song(item));
                });

                UpdateProgress();
                SetProgressBar(viewModel.CurrentTrack().Poisition(), viewModel.CurrentTrack().Duration());
                SetupFanart();
            });
        };

        UpdateProgress();
        SetProgressBar(viewModel.CurrentTrack().Poisition(), viewModel.CurrentTrack().Duration());
        SetupFanart();

        $.connection.hub.url = hubUrl;
        $.connection.hub.start();
    }
};
