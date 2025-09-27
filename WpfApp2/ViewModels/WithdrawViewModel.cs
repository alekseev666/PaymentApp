using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PaymentApp.Models;
using PaymentApp.Services;

namespace PaymentApp.ViewModels;

public partial class WithdrawViewModel : BaseViewModel
{
    private readonly ITransferService _transferService;
    private readonly IAccountService _accountService;

    public List<Account> Accounts => _accountService.GetAccounts();

    [ObservableProperty]
    private Account? _selectedAccount;

    [ObservableProperty]
    private decimal _withdrawAmount;

    public WithdrawViewModel(ITransferService transferService, IAccountService accountService)
    {
        _transferService = transferService;
        _accountService = accountService;
    }

    [RelayCommand]
    private void ExecuteWithdraw()
    {
        PreConditionStatus = "Проверяется...";
        PostConditionStatus = "Не проверено";
        OperationResultMessage = string.Empty;

        try
        {
            // ПРЕДУСЛОВИЯ
            var preConditions = new List<string>();

            if (SelectedAccount == null) preConditions.Add("Не выбран счет");
            if (WithdrawAmount <= 0) preConditions.Add("Сумма снятия должна быть больше 0");

            if (preConditions.Any())
            {
                PreConditionStatus = "НЕ ВЫПОЛНЕНО";
                OperationResultMessage = $"Ошибка предусловий: {string.Join(", ", preConditions)}";
                return;
            }

            PreConditionStatus = "ВЫПОЛНЕНО";

            // ОПЕРАЦИЯ
            var result = _transferService.Withdraw(SelectedAccount!, WithdrawAmount);
            OperationResultMessage = result.Message;

            // ПОСТУСЛОВИЯ
            if (result.IsSuccess)
            {
                PostConditionStatus = SelectedAccount!.Balance + SelectedAccount.MaxOverdraft >= 0
                    ? "ВЫПОЛНЕНО"
                    : "НЕ ВЫПОЛНЕНО";
            }
            else
            {
                PostConditionStatus = "Не применялось (операция не выполнена)";
            }
        }
        catch (Exception ex)
        {
            PreConditionStatus = "ОШИБКА ПРОВЕРКИ";
            OperationResultMessage = $"Ошибка: {ex.Message}";
        }
    }
}