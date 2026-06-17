using System.Runtime.Versioning;
using System.Threading.Tasks;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Provider;
using HilbertImage.Services;
using Java.IO;

namespace HilbertImage.Android;

[SupportedOSPlatform("android29.0")]
public class AndroidImageSaver(Context context) : IPlatformImageSaver
{
    public async Task<bool> SaveImageToGalleryAsync(byte[] imageBytes, string fileName)
    {
        var resolver = context.ContentResolver;
        if (resolver is null)
            return false;

        if (Build.VERSION.SdkInt >= BuildVersionCodes.Q)
        {
            var values = new ContentValues();
            values.Put(MediaStore.Images.Media.InterfaceConsts.DisplayName, fileName);
            values.Put(MediaStore.Images.Media.InterfaceConsts.MimeType, "image/png");
            values.Put(MediaStore.Images.Media.InterfaceConsts.RelativePath, "Pictures/HilbertImage");
            values.Put(MediaStore.Images.Media.InterfaceConsts.IsPending, 1);

            var collection = MediaStore.Images.Media.GetContentUri(MediaStore.VolumeExternalPrimary);
            if (collection is null)
                return false;
            
            var uri = resolver.Insert(collection, values);
            if (uri == null)
                return false;

            try
            {
                await using var stream = resolver.OpenOutputStream(uri);
                await stream!.WriteAsync(imageBytes);
                await stream.FlushAsync();

                values.Clear();
                values.Put(MediaStore.Images.Media.InterfaceConsts.IsPending, 0);
                resolver.Update(uri, values, null, null);
                return true;
            }
            catch
            {
                resolver.Delete(uri, null, null);
                return false;
            }
        }
        
        // Android 9 及以下

        var picturesDir = Environment.GetExternalStoragePublicDirectory(Environment.DirectoryPictures);

        var directory = new File(picturesDir, "HilbertImage");
        if (!directory.Exists())
            directory.Mkdirs();

        var file = new File(directory, fileName);

        try
        {
            using var stream = new FileOutputStream(file);
            await stream.WriteAsync(imageBytes);
            stream.Flush();
        }
        catch
        {
            file.Delete();
            return false;
        }

        MediaScannerConnection.ScanFile(context, [file.AbsolutePath], ["image/png"], null);
        return true;
    }
}