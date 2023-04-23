using System;
using System.Threading.Tasks;
using Godot;
using LibVLCSharp.Shared;

namespace libVLCsolution.Scripts;

public partial class App : Node
{
	private Media _media;
	public string VidPath = ProjectSettings.GlobalizePath("res://record.mp4");
	private IntPtr windowHandle;
	private MediaPlayer mediaPlayer;
	private LibVLC libVLC;
	private Window newWindow;
	[Export] private LineEdit VidPathEdit;
	[Export] private CheckBox newWindowCheck;
	[Export] private RichTextLabel infoLabel;

	public override void _Ready()
	{
		base._Ready();
		infoLabel.ParseBbcode(infoLabel.Text);
	}


	// This is the method that starts the video playback
	public void StartVideo()
	{
		// Do we have the play in new window checkbox checked?
		if (newWindowCheck.ButtonPressed)
		{
			// If so, we'll create a new window
			newWindow = new Window();
			// Add the new window to the scene tree
			AddChild(newWindow);
			// Set the name of the window
			newWindow.Name = "Video Player";
			// Subscribe to the CloseRequested event so we can clean up
			newWindow.CloseRequested += NewWindowOnCloseRequested;
			// Set the size and position of the window
			newWindow.PopupCentered(new Vector2I(700, 500));
			// Get the window handle of the new window
			windowHandle = (IntPtr)DisplayServer.WindowGetNativeHandle(DisplayServer.HandleType.WindowHandle, 1);
		}
		else
		{
			// If not, we'll use the Godot window
			windowHandle = (IntPtr)DisplayServer.WindowGetNativeHandle(DisplayServer.HandleType.WindowHandle, 0);
		}

		// Initialize the libVLC library
		libVLC = new LibVLC();
		// Create a new MediaPlayer instance
		mediaPlayer = new MediaPlayer(libVLC);
		
		// We need something to play, so we'll choose the path to a video file
		string playPath;
		// First we check if the user has entered a path in the LineEdit
		if (FileAccess.FileExists(VidPathEdit.Text)) playPath = VidPathEdit.Text;
		// If not, we'll check if there's a recorded video
		else if (FileAccess.FileExists(VidPath)) playPath = VidPath;
		// If not, we'll play a sample video
		else playPath = "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4";
		
		// Create a new Media instance with the path to the video file
		_media = new Media(libVLC, new Uri(playPath));
		// Set the window handle of the video player to the Godot window handle
		mediaPlayer.Hwnd = windowHandle;
		// Play the video
		mediaPlayer.Play(_media);
		// Subscribe to the EndReached event so we can clean up
		mediaPlayer.EndReached += MediaPlayerOnEndReached;
		// We can clean up the Media instance now
		_media.Dispose();
	}

	private async void MediaPlayerOnEndReached(object sender, EventArgs e)
	{
		// There will be issues if you try to clean up exactly when the video ends
		// so we'll wait a tiny bit before cleaning up
		await Task.Delay(100);
		// Clean up
		mediaPlayer.Dispose();
		libVLC.Dispose();
		if (newWindowCheck.ButtonPressed)
		{
			newWindow.QueueFree();
		}
	}
	
	private void NewWindowOnCloseRequested()
	{
		// Clean up
		newWindow.QueueFree();
		mediaPlayer.Dispose();
		libVLC.Dispose();
	}
	

	// A Godot signal is connected to this method, which starts the recording process
	// A Godot signal can't call a Task function, which we need for recording
	// So Godot calls this one, which then calls the Task function
	public async void Record()
	{
		await RecordVideo();
	}

	public async Task RecordVideo()
	{
		// Initialize the libVLC library
		Core.Initialize();

		// Make a new instance of libVLC
		var libVlcRec = new LibVLC();
		// Make a new instance of MediaPlayer, which controls the recording
		var mediaPlayer = new MediaPlayer(libVlcRec);
		// Make a new instance of Media, which is the source of the recording
		var recMedia = new Media(libVlcRec, "screen://", FromType.FromLocation);
		
		// Set the options for the recording
		recMedia.AddOption(":screen-fps=24");
		recMedia.AddOption(":sout=#transcode{vcodec=h264,vb=0,scale=0,acodec=mp4a,ab=128,channels=2,samplerate=44100}:file{dst=" + VidPath + "}");
		recMedia.AddOption(":sout-keep");

		// start recording
		mediaPlayer.Play(recMedia);	
		// record for 5 seconds
		await Task.Delay(5000);
		// stop recording and saves the file
		mediaPlayer.Stop();
		
		// Clean up
		recMedia.Dispose();
		mediaPlayer.Dispose();
		libVlcRec.Dispose();
	}
	
	// Clean up if there's an early close
	public override void _ExitTree()
	{
		base._ExitTree();
		_media?.Dispose();
	}
}
