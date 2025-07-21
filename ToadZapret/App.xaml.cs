using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Extensions.DependencyInjection;
using ToadZapret.Services;
using ToadZapret.ViewModels;
using ToadZapret.Views;
using Wpf.Ui.Tray;

namespace ToadZapret;

public partial class App
{
    private INotifyIconService _tray;
    private readonly IServiceProvider _serviceProvider;

    public App()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<MainViewModel>();
        services.AddSingleton<IZapretService, ZapretService>();
        services.AddSingleton<INotifyIconService, NotifyIconService>();
        
        services.AddSingleton<MainWindow>();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        
        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        var mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        mainWindow.Show();
        base.OnStartup(e);
        _tray = _serviceProvider.GetRequiredService<INotifyIconService>();

        _tray.Icon = new BitmapImage(new Uri("pack://application:,,,/ToadZapret;component/Assets/icon.ico"));
        _tray.TooltipText = "ToadZapret";
        
        // Добавляем контекстное меню
        _tray.ContextMenu = new System.Windows.Controls.ContextMenu();
        
        // Статус
        var statusMenuItem = new System.Windows.Controls.MenuItem { Header = "Статус: Не работает" };
        statusMenuItem.IsEnabled = false;
        
        // Показать окно
        var showMenuItem = new System.Windows.Controls.MenuItem { Header = "Показать окно" };
        showMenuItem.Click += (sender, args) => 
        {
            mainWindow.Show();
            mainWindow.WindowState = WindowState.Normal;
            mainWindow.Activate();
        };
        
        // Кнопка Старт/Стоп
        var toggleMenuItem = new System.Windows.Controls.MenuItem { Header = "Старт" };
        toggleMenuItem.Click += (sender, args) => 
        {
            mainViewModel.StartZapretCommand.Execute(null);
        };
        
        // Выход
        var exitMenuItem = new System.Windows.Controls.MenuItem { Header = "Выход" };
        exitMenuItem.Click += (sender, args) => 
        {
            // Останавливаем сервис перед выходом
            var zapretService = _serviceProvider.GetService<IZapretService>();
            zapretService?.Dispose();
            
            _tray.Unregister();
            Shutdown();
        };
        
        _tray.ContextMenu.Items.Add(statusMenuItem);
        _tray.ContextMenu.Items.Add(new System.Windows.Controls.Separator());
        _tray.ContextMenu.Items.Add(toggleMenuItem);
        _tray.ContextMenu.Items.Add(showMenuItem);
        _tray.ContextMenu.Items.Add(new System.Windows.Controls.Separator());
        _tray.ContextMenu.Items.Add(exitMenuItem);
        
        // Подписываемся на изменения в ViewModel для обновления меню
        mainViewModel.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(MainViewModel.StatusText))
            {
                statusMenuItem.Header = $"Статус: {mainViewModel.StatusText}";
            }
            if (args.PropertyName == nameof(MainViewModel.ButtonContent))
            {
                toggleMenuItem.Header = mainViewModel.ButtonContent;
            }
        };
        
        _tray.Register();
    }
    
    protected override void OnExit(ExitEventArgs e)
    {
        // Останавливаем сервис zapret перед выходом
        var zapretService = _serviceProvider.GetService<IZapretService>();
        zapretService?.Dispose();
        
        _tray?.Unregister();
        base.OnExit(e);
    }
}