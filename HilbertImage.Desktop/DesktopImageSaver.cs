using System;
using System.IO;
using System.Threading.Tasks;
using HilbertImage.Services;

namespace HilbertImage.Desktop;

public class DesktopImageSaver: IPlatformImageSaver
{
    public async Task<bool> SaveImageToGalleryAsync(byte[] imageBytes, string fileName)
    {
         var path = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
         await File.WriteAllBytesAsync(Path.Combine(path, $"{fileName}.png"), imageBytes);
         return true;
    }
}