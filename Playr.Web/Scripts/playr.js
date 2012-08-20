var playr = {

    Song: function (data) {
        var self = this;
        self.Id = ko.observable(data.Id);
        self.Artist = ko.observable(data.Artist);
        self.Album = ko.observable(data.Album);
        self.Title = ko.observable(data.Title);
        self.Rating = ko.observable(data.Rating);
        self.ArtworkUrl = ko.observable(data.ArtworkUrl);
        self.IsFavorite = ko.observable(data.IsFavorite);

        self.songDownloadUrl = ko.computed(function () {
            return "http://localhost:5555/songs/" + self.Id() + "/download";
        });

        self.albumDownloadUrl = ko.computed(function () {
            return "http://localhost:5555/albums/" + self.Album() + "/download";
        });

        self.Favorite = function () {
            var url = "/songs/" + self.Id() + "/favorite";
            if (self.IsFavorite()) {
                $.ajax({url: url, type: "DELETE"  });
            }
            else {
                $.ajax({ url: url, type: "PUT" });
            }
            self.IsFavorite(!self.IsFavorite());
        };
    },

    initMainPage: function(data) {
        function PageViewModel(djInfo) {
            var self = this;
            self.CurrentTrack = ko.observable(new playr.Song(djInfo.CurrentTrack));
            self.History = ko.observableArray();
            self.Queue = ko.observableArray();

            $.each(djInfo.History, function (idx, item) {
                self.History.push(new playr.Song(item));
            });
            $.each(djInfo.Queue, function (idx, item) {
                self.Queue.push(new playr.Song(item));
            });

        }

        var viewModel = new PageViewModel(data),
            hub = $.connection.playr;

        ko.applyBindings(viewModel);

        hub.DjInfoUpdated = function () {
            $.getJSON("/home/GetQueue", function (data) {
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
            });
        };

        $.connection.hub.url = "http://localhost:5554/signalr"
        $.connection.hub.start();
    }
};

// General Setup for everything
$(function () {
    $("#commandBar").hover(function () {
        $(this).find(".expanded").slideDown();
    }, function () {
        $(this).find(".expanded").slideUp();
    });




});
