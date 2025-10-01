using CommunityToolkit.Mvvm.ComponentModel;
using PaymentApp.Services;

namespace PaymentApp.ViewModels;

/// <summary>
/// Главная ViewModel которая содержит все остальные ViewModel
/// </summary>
public partial class MainViewModel : ObservableObject
{
    /// <summary>
    /// ViewModel для операции перевода между счетами
    /// </summary>
    [ObservableProperty]
    private TransferViewModel _transferVM;

    /// <summary>
    /// ViewModel для операции снятия денег
    /// </summary>
    [ObservableProperty]
    private WithdrawViewModel _withdrawVM;

    /// <summary>
    /// ViewModel для операции пополнения счета
    /// </summary>
    [ObservableProperty]
    private DepositViewModel _depositVM;

    [ObservableProperty]
    private HelpViewModel _HelpVM;

    /// <summary>
    /// Конструктор главной ViewModel
    /// Принимает сервисы и создает все дочерние ViewModel
    /// dependency injection 
    /// </summary>
    /// <param name="transferService">сервис для операций перевода</param>
    /// <param name="accountService">сервис для работы со счетами</param>
    public MainViewModel(ITransferService transferService, IAccountService accountService)
    {
        // Создаем все ViewModel и передаем им сервисы
        _transferVM = new TransferViewModel(transferService, accountService);
        _withdrawVM = new WithdrawViewModel(transferService, accountService);
        _depositVM = new DepositViewModel(transferService, accountService);
        _HelpVM = new HelpViewModel();
    }
}