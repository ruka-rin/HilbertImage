using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Avalonia.Android;

namespace HilbertImage.Android;

[Activity(
    Label = "HilbertImage.Android",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@drawable/icon",
    MainLauncher =  true,
    LaunchMode = LaunchMode.SingleTask,
    NoHistory = false,
    ConfigurationChanges =
        ConfigChanges.Orientation |
        ConfigChanges.ScreenSize |
        ConfigChanges.UiMode |
        ConfigChanges.SmallestScreenSize |
        ConfigChanges.ScreenLayout)]
public class MainActivity : AvaloniaMainActivity
{

}