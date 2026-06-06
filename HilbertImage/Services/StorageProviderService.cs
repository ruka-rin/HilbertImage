using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform.Storage;

namespace HilbertImage.Services;

internal static class StorageProviderService
{
    public static IStorageProvider? StorageProvider => TopLevel.GetTopLevel(MainVisual)?.StorageProvider;

    public static Visual? MainVisual { get; set; }
}