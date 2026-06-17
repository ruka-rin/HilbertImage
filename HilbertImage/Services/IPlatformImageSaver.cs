using System.Threading.Tasks;

namespace HilbertImage.Services;

public interface IPlatformImageSaver
{
    Task<bool> SaveImageToGalleryAsync(byte[] imageBytes, string fileName);
}