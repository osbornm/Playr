var playr = {

    Song: function(data) {
        this.Id = ko.observable(data.Id);
        this.Artist = ko.observable(data.Artist);
        this.Album = ko.observable(data.Album);
        this.Title = ko.observable(data.Title);
        this.Rating = ko.observable(data.Rating);
        this.ArtworkUrl = ko.observable(data.ArtworkUrl);

        this.songDownloadUrl = ko.computed(function () {
            return "http://localhost:5555/songs/" + this.Id() + "/download";
        }, this);

        this.albumDownloadUrl = ko.computed(function () {
            return "http://localhost:5555/albums/" + this.Album() + "/download";
        }, this);
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
            $.getJSON("http://localhost:5555/queue", function (data) {
                viewModel.CurrentTrack(new playr.Song(data.CurrentTrack));
                viewModel.History.removeAll();
                viewModel.Queue.removeAll();
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