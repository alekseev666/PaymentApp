using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PaymentApp.Models;
using PaymentApp.Services;

namespace PaymentApp.ViewModels;

public partial class DepositViewModel : BaseViewModel
{
    private readonly ITransferService _transferService;
    private readonly IAccountService _accountService;

    public List<Account> Accounts => _accountService.GetAccounts();

    [ObservableProperty]
    private Account? _selectedAccount;

    [ObservableProperty]
    private decimal _depositAmount;

    public DepositViewModel(ITransferService transferService, IAccountService accountService)
    {
        _transferService = transferService;
        _accountService = accountService;
    }

    [RelayCommand]
    private void ExecuteDeposit()
    {
        PreConditionStatus = "Проверяется...";
        PostConditionStatus = "Не проверено";
        OperationResultMessage = string.Empty;

        try
        {
            // ПРЕДУСЛОВИЯ
            var preConditions = new List<string>();

            if (SelectedAccount == null) preConditions.Add("Не выбран счет");
            if (DepositAmount <= 0) preConditions.Add("Сумма пополнения должна быть больше 0");

            if (preConditions.Any())
            {
                PreConditionStatus = "НЕ ВЫПОЛНЕНО";
                OperationResultMessage = $"Ошибка предусловий: {string.Join(", ", preConditions)}";
                return;
            }

            PreConditionStatus = "ВЫПОЛНЕНО";

            // ОПЕРАЦИЯ
            var oldBalance = SelectedAccount!.Balance;
            var result = _transferService.Deposit(SelectedAccount!, DepositAmount);
            OperationResultMessage = result.Message;

            // ПОСТУСЛОВИЯ
            PostConditionStatus = SelectedAccount.Balance == oldBalance + DepositAmount
                ? "ВЫПОЛНЕНО"
                : "НЕ ВЫПОЛНЕНО";
        }
        catch (Exception ex)
        {
            PreConditionStatus = "ОШИБКА ПРОВЕРКИ";
            OperationResultMessage = $"Ошибка: {ex.Message}";
        }
    }
}