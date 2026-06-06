using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using HilbertImage.Services;
using HilbertImage.ViewModels;
using HilbertImage.Views;

namespace HilbertImage;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow { DataContext = new MainViewModel() };
            StorageProviderService.MainVisual = desktop.MainWindow;
        }
        else if (ApplicationLifetime is IActivityApplicationLifetime singleViewFactoryApplicationLifetime)
        {
            singleViewFactoryApplicationLifetime.MainViewFactory =
                () =>
                {
                    var mainView = new MainView { DataContext = new MainViewModel() };
                    StorageProviderService.MainVisual = mainView;
                    return mainView;
                };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView() { DataContext = new MainViewModel() };
            StorageProviderService.MainVisual = singleViewPlatform.MainView;
        }

        base.OnFrameworkInitializationCompleted();
    }
    
}