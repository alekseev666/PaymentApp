using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using PaymentApp.Services;
using PaymentApp.ViewModels;
using PaymentApp.Views;
using System.Windows;
using PaymentApp.Services.Impl;
namespace PaymentApp;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Настройка DI
        var services = new ServiceCollection();
        services.AddSingleton<IAccountService, AccountService>();
        services.AddSingleton<ITransferService, TransferService>();
        services.AddTransient<MainViewModel>();
        services.AddTransient<TransferViewModel>();
        services.AddTransient<WithdrawViewModel>();
        services.AddTransient<DepositViewModel>();

        var provider = services.BuildServiceProvider();
        Ioc.Default.ConfigureServices(provider);

        // Запуск главного окна
        var mainWindow = new MainWindow(provider.GetService<MainViewModel>()!);
        mainWindow.Show();
    }
}