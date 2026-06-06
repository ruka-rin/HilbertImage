using System;
using System.IO;
using System.Threading.Tasks;
using AnimatedImage.Avalonia;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HilbertImage.Services;

namespace HilbertImage.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty] private Bitmap? _bitmapImage;
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private bool _isConfuse;

    [RelayCommand]
    private async Task SelectImage()
    {
        if (StorageProviderService.StorageProvider is null)
            return;
        
        var files = await StorageProviderService.StorageProvider.OpenFilePickerAsync(
            new FilePickerOpenOptions
            {
                Title = "选择图片",
                AllowMultiple = false,
                FileTypeFilter =
                [
                    new FilePickerFileType("图像文件") { Patterns = ["*.png", "*.jpg", "*.jpeg"] },
                ]
            }
        );

        if (files.Count == 1)
        {
            using var file = files[0];
            await using var stream = await file.OpenReadAsync();
            await Task.Run(() =>
            {
                IsLoading = true;
                BitmapImage = ImageConfuserService.Confuse(new Bitmap(stream), IsConfuse);
                IsLoading = false;
            });
        }
    }

    [RelayCommand]
    private async Task SaveImage()
    {
        if (BitmapImage is null || StorageProviderService.StorageProvider is null)
            return;
        
        var pngFileType = new FilePickerFileType("PNG") { Patterns = ["*.png"] };
        using var file = await StorageProviderService.StorageProvider.SaveFilePickerAsync(
            new FilePickerSaveOptions
            {
                Title = "保存图片",
                SuggestedFileName = "output.png",
                SuggestedFileType = pngFileType,
                FileTypeChoices = [pngFileType]
            }
        );

        if (file is not null)
        {
            await using var stream = await file.OpenWriteAsync();
            await Task.Run(() => BitmapImage?.Save(stream));
        }
    }

}