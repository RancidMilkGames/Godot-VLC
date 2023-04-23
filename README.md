# Godot VLC
This sample project is an example of using a .NET/mono library to extend Godot mono.
Due to licensing constraints, Godot only ships with the ability to play Theora video files.
VLC is a media player that can play almost any video format, and is open source.
This uses libVLCSharp, which is a .NET wrapper for libVLC, what VLC is built on.
This project was made in Godot 4. The process might work in 3 as well though.
Please note, many video files have licensing restrictions on them, and you may not be able to use them in your project.
libVLCSharp does support playing AV1, which is an open video format that's comparable with any of the big popular formats in terms of size and quality.
You might consider using that if you plan to ship something with video.

# Instructions
Before the project will run, you must get the necessary .NET packages.
The project is currently setup to use the Windows implementation of libVLC.
If you want to use a different implementation, you will need to change the VideoLAN.LibVLC.Windows package in the .csproj file to your OS's.
The easiest way of getting the packages is to just open the .sln project in an IDE that supports NuGet packages.
I used Rider, but Visual Studio should work as well.
Once you open the project, it should automatically download the packages.
Remember, if you're not using Windows, you'll need to change the OS-specific package.

# Use
This is a very simple example project. It has two features. Playing back any video you want, and recording your screen.

# Considerations
* Adding libraries like this can have a noticeable increase in the size of your projects and their exports.
* Check the license of any libraries you add and make sure to comply with them.
This project is MIT, but the linked vlc libraries are LGPL 2.1.
Please note that this repository does not actually contain the vlc libraries, it just links to them.
In it's current state, Godot will not package the libraries in the exported .exe(If using Windows), but as separate files that the .exe can access.
The full LGPL 2.1 license is included in the repository under [/Libraries/libVLCLicense/lgpl-2.1.md](Libraries/libVLCLicense/lgpl-2.1.md).
I am not a lawyer and have no qualifications in licensing, so please do your own research or consult a lawyer before releasing a packaged item with these libraries.