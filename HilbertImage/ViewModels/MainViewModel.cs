using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using AnimatedImage.Avalonia;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HilbertImage.Services;

namespace HilbertImage.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private Bitmap? _sourceImage;
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
                _sourceImage = new Bitmap(stream);
                BitmapImage = ImageConfuserService.Confuse(_sourceImage, IsConfuse);
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

    [RelayCommand]
    private async Task ToggleImage()
    {
        if (_sourceImage is not null)
        {
            await Task.Run(() =>
            {
                IsLoading = true;
                BitmapImage = ImageConfuserService.Confuse(_sourceImage, IsConfuse);
                IsLoading = false;
            });
        }
    }

    [RelayCommand]
    private async Task SaveToAlbum()
    {
        if (BitmapImage is null || PlatformServices.ImageSaver is null)
            return;
        
        var memoryStream = new MemoryStream();
        BitmapImage.Save(memoryStream);
        var fileName = $"HI_{DateTime.Now:yyyy_MM_dd_HH_ss}";
        await PlatformServices.ImageSaver.SaveImageToGalleryAsync(memoryStream.ToArray(), fileName);
    }

    [RelayCommand]
    private void DragOver(DragEventArgs e)
    {
        e.DragEffects = e.DataTransfer.Formats.Contains(DataFormat.File) ? DragDropEffects.Copy : DragDropEffects.None;
    }
    
    [RelayCommand]
    private async Task DragDrop(DragEventArgs e)
    {
        if (!e.DataTransfer.Formats.Contains(DataFormat.File))
            return;
        
        var files = e.DataTransfer.TryGetFiles();
        if (files?[0] is not IStorageFile file)
            return;

        string[] exts = [".png", ".jpg", ".jpeg"];
        if (!exts.Contains(Path.GetExtension(file.Name), StringComparer.OrdinalIgnoreCase))
            return;

        await using var stream = await file.OpenReadAsync();
        await Task.Run(() =>
        {
            IsLoading = true;
            try
            {
                _sourceImage = new Bitmap(stream);
                BitmapImage = ImageConfuserService.Confuse(_sourceImage, IsConfuse);
            }
            catch (ArgumentException)
            {
                return;
            }
            finally
            {
                IsLoading = false;
            }
        });
    }
}