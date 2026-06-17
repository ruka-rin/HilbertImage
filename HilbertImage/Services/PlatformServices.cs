namespace HilbertImage.Services;

public static class PlatformServices
{
    public static IPlatformImageSaver? ImageSaver { get; set; } = null!;
}