![#Playr](https://github.com/osbornm/Playr/blob/master/Media/logo.png?raw=true)

# Look at the `vNext` Branch!

I'm not going to lie this is basically a rip off of [play](https://github.com/play) but writen in .net and using the Microsoft stack. It was a side project to learn new stuff when we, the Azure Portal Team, moved to a team room.

[![Playr Presentation](https://github.com/osbornm/Playr/blob/master/Media/SlidePreview.png?raw=true)
](http://speakerdeck.com/u/osbornm/p/playr)

##Setup

* Install [iTunes](http://itunes.apple.com)
* Make sure iTunes DJ is [enabled & configured](http://support.apple.com/kb/PH1741?viewlocale=en_US)
* In order to help keep your media organized it is highly recommended that you enable the ["keep iTunes Media Folder organized"](http://km.support.apple.com/library/APPLE/APPLECARE_ALLGEOS/HT1364/HT1364_02----003.png) setting. If you choose not to do this uploaded file names with be random GUIDs.
* Build and run Playr.Api Console Application (Needs to be run as an admin)
* Be default Server is running at [http://localhost:5555](http://localhost:5555) and the signalr notification service is running on [http://localhost:5554](http://localhost:5554)
* Navigate to whatever port Playr.Web is running on and start enjoying

##API Endpoints

<table>
	<tr><td>/pause                  </td><td><em>PUT    </em></td></tr>
	<tr><td>/play                   </td><td><em>PUT    </em></td></tr>
	<tr><td>/playpause              </td><td><em>PUT    </em></td></tr>
	<tr><td>/next                   </td><td><em>POST   </em></td></tr>
	<tr><td>/previous               </td><td><em>POST   </em></td></tr>
	<tr><td>/volume/up              </td><td><em>POST   </em></td></tr>
	<tr><td>/volume/down            </td><td><em>POST   </em></td></tr>
	<tr><td>/current                </td><td><em>GET    </em></td></tr>
	<tr><td>/queue                  </td><td><em>GET    </em></td></tr>
	<tr><td>/queue/{id}             </td><td><em>POST   </em></td></tr>
	<tr><td>/songs/{id}/download    </td><td><em>GET    </em></td></tr>
    <tr><td>/songs/{id}/artwork     </td><td><em>GET    </em></td></tr>
    <tr><td>/songs/{id}/favorite    </td><td><em>PUT    </em></td></tr>
    <tr><td>/songs/{id}/favorite    </td><td><em>DELETE </em></td></tr>
    <tr><td>/albums/{name}/download </td><td><em>GET    </em></td></tr>
    <tr><td>/upload                 </td><td><em>POST   </em></td></tr>
    <tr><td>/say                    </td><td><em>POST   </em></td></tr>
    <tr><td>/users/register         </td><td><em>POST   </em></td></tr>
    <tr><td>/users/{email}          </td><td><em>GET    </em></td></tr>
    <tr><td>/users/{email}/token    </td><td><em>PUT    </em></td></tr>
    <tr><td>/artists/{name}         </td><td><em>GET    </em></td></tr>
    <tr><td>/artists/{name}/{file}  </td><td><em>GET    </em></td></tr>
</table>

## Screenshots
![Main Page](https://github.com/osbornm/Playr/blob/master/Media/Screenshot1.png?raw=true)

## License
<a href="http://www.wtfpl.net/"><img src="http://www.wtfpl.net/wp-content/uploads/2012/12/wtfpl-badge-4.png" width="80" height="15" alt="WTFPL" /></a>

###Special Thanks

to [Black Raven](http://blackravenbrewing.com), where most of this app was coded, to my lovely wife to be for understanding my desire to work on side projects, and to all the folks, too numerous to list, who have helped get bits and pieces working. 
