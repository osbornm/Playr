![#Playr](https://github.com/osbornm/Playr/blob/master/Media/logo.png?raw=true)

I'm not going to lie this is basically a rip off of [play](https://github.com/play) but writen in .net and using the Microsoft stack. It was a side project to learn new stuff when we, the Azure Portal Team, moved to a team room.

##Setup

* Install [iTunes](http://itunes.apple.com)
* Make sure iTunes DJ is [enabled & configured](http://support.apple.com/kb/PH1741?viewlocale=en_US)
* In order to help keep your media organized it is highly recommended that you enable the ["keep iTunes Media Folder organized"](http://km.support.apple.com/library/APPLE/APPLECARE_ALLGEOS/HT1364/HT1364_02----003.png) setting. If you choose not to do this uploaded file names with be random GUIDs.
* Build and run Playr.Api Console Application (Needs to be run as an admin)
* Be default Server is running at [http://localhost:5555](http://localhost:5555) and the signalr notification service is running on [http://localhost:5554](http://localhost:5554)
* Navigate to whatever port Playr.Web is running on and start enjoying

##Usage
* /pause                  [*PUT*]
* /play                   [*PUT*]
* /playpause              [*PUT*]
* /next                   [*POST*]
* /previous               [*POST*]
* /volume/up              [*POST*]
* /volume/down            [*POST*]
* /current                [*GET*]
* /queue                  [*GET*]
* /queue/{id}             [*POST*]
* /songs/{id}/download    [*GET*]
* /songs/{id}/artwork     [*GET*]
* /songs/{id}/favorite    [*PUT*]
* /songs/{id}/favorite    [*DELETE*]
* /albums/{name}/download [*GET*]
* /upload                 [*POST*]
* /say                    [*POST*]
* /users/register         [*POST*]
* /users/{email}          [*GET*]
* /users/{email}/token    [*PUT*]

###Speacial Thanks
to [Black Raven](http://blackravenbrewing.com), where most of this app was coded, to my lovely wife to be for understanding my desire to work on side projects, and to all the folks, too numerous to list, who have helped get bits and pieces working. 