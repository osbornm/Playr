![#Playr](https://github.com/osbornm/Playr/blob/master/Media/logo.png?raw=true)

I'm not going to lie this is basically a rip off of [play](https://github.com/play) but writen in .net and using the Microsoft stack. It was a side project to learn new stuff when we, the Azure Portal Team, moved to a team room.

[![Playr Presentation](https://github.com/osbornm/Playr/blob/master/Media/SlidePreview.png?raw=true)
](http://speakerdeck.com/u/osbornm/p/playr)

##API Endpoints

<table>
	<thead>
		<tr><th colspan="4">Music Library Endpoints</td></tr>
	<thead>
	<tr>
		<td></td>
		<td>/api/library</td>
		<td><em>GET</em></td>
		<td>Get Library Information</td>
	</tr>
	<tr>
		<td style="width: 16px"><img src="https://f.cloud.github.com/assets/674284/190162/2196b8be-7ed2-11e2-887f-ea3cdd151a2b.png" alt="Requires API Key"/></td>
		<td>/api/library</td>
		<td><em>POST</em></td>
		<td>Upload tracks to library</td>
	</tr>
	<tr>
		<td></td>
		<td>/api/library/albums</td>
		<td><em>GET</em></td>
		<td>Get all the albums in the library</td>
	</tr>
	<tr>
		<td></td>
		<td>/api/library/albums/{id}</td>
		<td><em>GET</em></td>
		<td>Get an ablum</td>
	</tr>
	<tr>
		<td></td>
		<td>/api/library/albums/{id}/tracks</td>
		<td><em>GET</em></td>
		<td>Get all the tracks for an album</td>
	</tr>
	<tr>
		<td></td>
		<td>/api/library/albums/{id}/download</td>
		<td><em>GET</em></td>
		<td>download album as zip</td>
	</tr>
	<tr>
		<td></td>
		<td>/api/library/albums/{id}/artwork</td>
		<td><em>GET</em></td>
		<td>Get artwork for the album (a.k.a. album art)</td>
	</tr>
	<tr>
		<td></td>
		<td>/api/library/tracks/{id}/download</td>
		<td><em>GET</em></td>
		<td>Download a single track</td>
	</tr>
	<tr>
		<td></td>
		<td>/api/library/artists</td>
		<td><em>GET</em></td>
		<td>Get all the artists in library</td>
	</tr>
	<tr>
		<td></td>
		<td>/api/library/artists/{name}</td>
		<td><em>GET</em></td>
		<td>Get a single artist by name</td>
	</tr>
	<tr>
		<td></td>
		<td>/api/library/artists/{name}/albums</td>
		<td><em>GET</em></td>
		<td>Get all the albums for an artist</td>
	</tr>
	<tr>
		<td></td>
		<td>/api/library/artists/{name}/download</td>
		<td><em>GET</em></td>
		<td>Download all albums for artist as zip file</td>
	</tr>
	<tr>
		<td></td>
		<td>/api/library/artists/{name}/fanart/{id}</td>
		<td><em>GET</em></td>
		<td>Get a single piece of fanart for artist</td>
	</tr>
	<tr>
		<td></td>
		<td>/api/library/genres</td>
		<td><em>GET</em></td>
		<td>Get all the generes in library</td>
	</tr>
	<tr>
		<td></td>
		<td>/api/library/genres/{name}</td>
		<td><em>GET</em></td>
		<td>Get information about a single genere</td>
	</tr>
	<thead>
		<tr><th colspan="4">Control Endpoints</td></tr>
	<thead>
	<tr>
		<td style="width: 16px"><img src="https://f.cloud.github.com/assets/674284/190162/2196b8be-7ed2-11e2-887f-ea3cdd151a2b.png" alt="Requires API Key"/></td>
		<td>/api/control/resume</td>
		<td><em>POST</em></td>
		<td>Resume playing music</td>
	</tr>
	<tr>
		<td style="width: 16px"><img src="https://f.cloud.github.com/assets/674284/190162/2196b8be-7ed2-11e2-887f-ea3cdd151a2b.png" alt="Requires API Key"/></td>
		<td>/api/control/pause</td>
		<td><em>POST</em></td>
		<td>Pause the music</td>
	</tr>
	<tr>
		<td style="width: 16px"><img src="https://f.cloud.github.com/assets/674284/190162/2196b8be-7ed2-11e2-887f-ea3cdd151a2b.png" alt="Requires API Key"/></td>
		<td>/api/control/next</td>
		<td><em>POST</em></td>
		<td>Start playing the next track</td>
	</tr>
	<tr>
		<td style="width: 16px"><img src="https://f.cloud.github.com/assets/674284/190162/2196b8be-7ed2-11e2-887f-ea3cdd151a2b.png" alt="Requires API Key"/></td>
		<td>/api/control/previous</td>
		<td><em>POST</em></td>
		<td>Start playing the previous track</td>
	</tr>
	<thead>
		<tr><th colspan="4">Information Endpoints</td></tr>
	<thead>
	<tr>
		<td></td>
		<td>/api/info/current</td>
		<td><em>GET</em></td>
		<td>Get the currently playing track &amp; related information</td>
	</tr>
	<tr>
		<td></td>
		<td>/api/info/queue</td>
		<td><em>GET</em></td>
		<td>Get the current music queue</td>
	</tr>
</table>
<table>
	<tr>
		<td><img src="https://f.cloud.github.com/assets/674284/190162/2196b8be-7ed2-11e2-887f-ea3cdd151a2b.png" alt="Requires API Key"/></td>
		<td colspan="3">Endpoint Requires API Key.</td>
	</tr>
</table>

## Screenshots
![Main Page](https://github.com/osbornm/Playr/blob/master/Media/Screenshot1.png?raw=true)

###Special Thanks

to [Black Raven](http://blackravenbrewing.com), where most of this app was coded, to my lovely wife to be for understanding my desire to work on side projects, and to all the folks, too numerous to list, who have helped get bits and pieces working. 