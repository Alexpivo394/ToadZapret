using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ToadZapret.Services;

namespace ToadZapret.ViewModels;

public partial class MainViewModel : ObservableObject
{
    IZapretService  _zapretService;

    public MainViewModel(IZapretService zapretService)
    {
        _zapretService = zapretService;
    }
    
    [ObservableProperty] private string? _buttonContent = "Старт";
    
    [ObservableProperty] private bool _isWorked = false;

    [ObservableProperty] private string? _statusText = "Не работает";
    

    [RelayCommand]
    public void StartZapret()
    {
        if (!IsWorked)
        {
            // Запускаем сервис
            _zapretService.Start();
            
            ButtonContent = "Стоп";
            IsWorked = true;
            StatusText = "Работает";
        }
        else
        {
            //Останавливаем сервис
            _zapretService.Stop();
            ButtonContent = "Старт";
            IsWorked = false;
            StatusText = "Не работает";
        }
    }
}