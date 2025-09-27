using CommunityToolkit.Mvvm.ComponentModel;
using PaymentApp.Services;

namespace PaymentApp.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private TransferViewModel _transferVM;

    [ObservableProperty]
    private WithdrawViewModel _withdrawVM;

    [ObservableProperty]
    private DepositViewModel _depositVM;

    public MainViewModel(ITransferService transferService, IAccountService accountService)
    {
        _transferVM = new TransferViewModel(transferService, accountService);
        _withdrawVM = new WithdrawViewModel(transferService, accountService);
        _depositVM = new DepositViewModel(transferService, accountService);
    }
}