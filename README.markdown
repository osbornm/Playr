![#Playr](https://github.com/osbornm/Playr/blob/master/Media/logo.png?raw=true)

I'm not going to lie this is basically a rip off of [play](https://github.com/play) but writen in .net and using the Microsoft stack. It was a side project to learn new stuff when we, the Azure Portal Team, moved to a team room.

[![Playr Presentation](https://github.com/osbornm/Playr/blob/master/Media/SlidePreview.png?raw=true)
](http://speakerdeck.com/u/osbornm/p/playr)

##API Endpoints

<table>
	<thead>
		<tr><th colspan="3">Music Library Endpoints</td></tr>
	<thead>
	<tr>
		<td></td>
		<td>/api/library</td>
		<td><em>GET</em></td>
		<td>Get Library Information</td>
	</tr>
	<tr>
		<td style="width: 16px">
			<svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" version="1.1" id="Calque_1" x="0px" y="0px" width="16px" height="16px" viewBox="0 0 100 100" enable-background="new 0 0 100 100" xml:space="preserve">
				<path d="M76.344,55.214h4.724v-9.887c0-1.399-1.135-2.536-2.536-2.536h-9.889V32.633c0-9.983-8.123-18.108-18.107-18.108  c-9.986,0-18.109,8.125-18.109,18.108v10.158h-9.892c-1.398,0-2.534,1.137-2.534,2.536v9.887h42.224  c1.302,0,2.352,1.054,2.352,2.354c0,1.299-1.05,2.352-2.352,2.352H20v8.097h42.224c1.302,0,2.352,1.055,2.352,2.352  c0,1.302-1.05,2.355-2.352,2.355H20v10.083c0,1.4,1.136,2.536,2.534,2.536h55.997c1.401,0,2.536-1.136,2.536-2.536V72.723h-4.724  c-1.302,0-2.354-1.054-2.354-2.355c0-1.297,1.052-2.352,2.354-2.352h4.724v-8.097h-4.724c-1.302,0-2.354-1.053-2.354-2.352  C73.99,56.268,75.042,55.214,76.344,55.214z M63.572,42.791H37.494V32.633c0-7.188,5.85-13.036,13.041-13.036  c7.188,0,13.037,5.848,13.037,13.036V42.791z"/>
			</svg>
		</td>
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
		<tr><th colspan="3">Control Endpoints</td></tr>
	<thead>
	<tr>
		<td style="width: 16px">
			<svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" version="1.1" id="Calque_1" x="0px" y="0px" width="16px" height="16px" viewBox="0 0 100 100" enable-background="new 0 0 100 100" xml:space="preserve">
				<path d="M76.344,55.214h4.724v-9.887c0-1.399-1.135-2.536-2.536-2.536h-9.889V32.633c0-9.983-8.123-18.108-18.107-18.108  c-9.986,0-18.109,8.125-18.109,18.108v10.158h-9.892c-1.398,0-2.534,1.137-2.534,2.536v9.887h42.224  c1.302,0,2.352,1.054,2.352,2.354c0,1.299-1.05,2.352-2.352,2.352H20v8.097h42.224c1.302,0,2.352,1.055,2.352,2.352  c0,1.302-1.05,2.355-2.352,2.355H20v10.083c0,1.4,1.136,2.536,2.534,2.536h55.997c1.401,0,2.536-1.136,2.536-2.536V72.723h-4.724  c-1.302,0-2.354-1.054-2.354-2.355c0-1.297,1.052-2.352,2.354-2.352h4.724v-8.097h-4.724c-1.302,0-2.354-1.053-2.354-2.352  C73.99,56.268,75.042,55.214,76.344,55.214z M63.572,42.791H37.494V32.633c0-7.188,5.85-13.036,13.041-13.036  c7.188,0,13.037,5.848,13.037,13.036V42.791z"/>
			</svg>
		</td>
		<td>/api/control/resume</td>
		<td><em>POST</em></td>
		<td>Resume playing music</td>
	</tr>
	<tr>
		<td style="width: 16px">
			<svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" version="1.1" id="Calque_1" x="0px" y="0px" width="16px" height="16px" viewBox="0 0 100 100" enable-background="new 0 0 100 100" xml:space="preserve">
				<path d="M76.344,55.214h4.724v-9.887c0-1.399-1.135-2.536-2.536-2.536h-9.889V32.633c0-9.983-8.123-18.108-18.107-18.108  c-9.986,0-18.109,8.125-18.109,18.108v10.158h-9.892c-1.398,0-2.534,1.137-2.534,2.536v9.887h42.224  c1.302,0,2.352,1.054,2.352,2.354c0,1.299-1.05,2.352-2.352,2.352H20v8.097h42.224c1.302,0,2.352,1.055,2.352,2.352  c0,1.302-1.05,2.355-2.352,2.355H20v10.083c0,1.4,1.136,2.536,2.534,2.536h55.997c1.401,0,2.536-1.136,2.536-2.536V72.723h-4.724  c-1.302,0-2.354-1.054-2.354-2.355c0-1.297,1.052-2.352,2.354-2.352h4.724v-8.097h-4.724c-1.302,0-2.354-1.053-2.354-2.352  C73.99,56.268,75.042,55.214,76.344,55.214z M63.572,42.791H37.494V32.633c0-7.188,5.85-13.036,13.041-13.036  c7.188,0,13.037,5.848,13.037,13.036V42.791z"/>
			</svg>
		</td>
		<td>/api/control/pause</td>
		<td><em>POST</em></td>
		<td>Pause the music</td>
	</tr>
	<tr>
		<td style="width: 16px">
			<svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" version="1.1" id="Calque_1" x="0px" y="0px" width="16px" height="16px" viewBox="0 0 100 100" enable-background="new 0 0 100 100" xml:space="preserve">
				<path d="M76.344,55.214h4.724v-9.887c0-1.399-1.135-2.536-2.536-2.536h-9.889V32.633c0-9.983-8.123-18.108-18.107-18.108  c-9.986,0-18.109,8.125-18.109,18.108v10.158h-9.892c-1.398,0-2.534,1.137-2.534,2.536v9.887h42.224  c1.302,0,2.352,1.054,2.352,2.354c0,1.299-1.05,2.352-2.352,2.352H20v8.097h42.224c1.302,0,2.352,1.055,2.352,2.352  c0,1.302-1.05,2.355-2.352,2.355H20v10.083c0,1.4,1.136,2.536,2.534,2.536h55.997c1.401,0,2.536-1.136,2.536-2.536V72.723h-4.724  c-1.302,0-2.354-1.054-2.354-2.355c0-1.297,1.052-2.352,2.354-2.352h4.724v-8.097h-4.724c-1.302,0-2.354-1.053-2.354-2.352  C73.99,56.268,75.042,55.214,76.344,55.214z M63.572,42.791H37.494V32.633c0-7.188,5.85-13.036,13.041-13.036  c7.188,0,13.037,5.848,13.037,13.036V42.791z"/>
			</svg>
		</td>
		<td>/api/control/next</td>
		<td><em>POST</em></td>
		<td>Start playing the next track</td>
	</tr>
	<tr>
		<td style="width: 16px">
			<svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" version="1.1" id="Calque_1" x="0px" y="0px" width="16px" height="16px" viewBox="0 0 100 100" enable-background="new 0 0 100 100" xml:space="preserve">
				<path d="M76.344,55.214h4.724v-9.887c0-1.399-1.135-2.536-2.536-2.536h-9.889V32.633c0-9.983-8.123-18.108-18.107-18.108  c-9.986,0-18.109,8.125-18.109,18.108v10.158h-9.892c-1.398,0-2.534,1.137-2.534,2.536v9.887h42.224  c1.302,0,2.352,1.054,2.352,2.354c0,1.299-1.05,2.352-2.352,2.352H20v8.097h42.224c1.302,0,2.352,1.055,2.352,2.352  c0,1.302-1.05,2.355-2.352,2.355H20v10.083c0,1.4,1.136,2.536,2.534,2.536h55.997c1.401,0,2.536-1.136,2.536-2.536V72.723h-4.724  c-1.302,0-2.354-1.054-2.354-2.355c0-1.297,1.052-2.352,2.354-2.352h4.724v-8.097h-4.724c-1.302,0-2.354-1.053-2.354-2.352  C73.99,56.268,75.042,55.214,76.344,55.214z M63.572,42.791H37.494V32.633c0-7.188,5.85-13.036,13.041-13.036  c7.188,0,13.037,5.848,13.037,13.036V42.791z"/>
			</svg>
		</td>
		<td>/api/control/previous</td>
		<td><em>POST</em></td>
		<td>Start playing the previous track</td>
	</tr>
	<thead>
		<tr><th colspan="3">Information Endpoints</td></tr>
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

## Screenshots
![Main Page](https://github.com/osbornm/Playr/blob/master/Media/Screenshot1.png?raw=true)

###Special Thanks

to [Black Raven](http://blackravenbrewing.com), where most of this app was coded, to my lovely wife to be for understanding my desire to work on side projects, and to all the folks, too numerous to list, who have helped get bits and pieces working. 