using System.Runtime.Versioning;
using Android.App;
using Android.Runtime;
using Avalonia;
using Avalonia.Android;
using HilbertImage.Services;

namespace HilbertImage.Android
{
    [Application]
    [SupportedOSPlatform("android29.0")]
    public class Application : AvaloniaAndroidApplication<App>
    {
        protected Application(nint javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
        {
            PlatformServices.ImageSaver = new AndroidImageSaver(Context);
            
            return base.CustomizeAppBuilder(builder)
                .WithInterFont().With(new AndroidPlatformOptions 
                { 
                    RenderingMode = [AndroidRenderingMode.Software],
                });
        }
    }
}