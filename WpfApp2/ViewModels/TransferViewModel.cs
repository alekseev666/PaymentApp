using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PaymentApp.Models;
using PaymentApp.Services;
using System.Diagnostics;

namespace PaymentApp.ViewModels;

public partial class TransferViewModel : BaseViewModel
{
    private readonly ITransferService _transferService;
    private readonly IAccountService _accountService;

    public List<Account> Accounts => _accountService.GetAccounts();

    [ObservableProperty]
    private Account? _selectedFromAccount;

    [ObservableProperty]
    private Account? _selectedToAccount;

    [ObservableProperty]
    private decimal _transferAmount;

    public TransferViewModel(ITransferService transferService, IAccountService accountService)
    {
        _transferService = transferService;
        _accountService = accountService;
    }

    [RelayCommand]
    private void ExecuteTransfer()
    {
        // Сбрасываем статусы
        PreConditionStatus = "Проверяется...";
        PostConditionStatus = "Не проверено";
        OperationResultMessage = string.Empty;

        try
        {
            // ПРЕДУСЛОВИЯ - проверяем вручную для отображения в UI
            var preConditions = new List<string>();

            if (SelectedFromAccount == null) preConditions.Add("Не выбран счет отправителя");
            if (SelectedToAccount == null) preConditions.Add("Не выбран счет получателя");
            if (SelectedFromAccount != null && SelectedToAccount != null && SelectedFromAccount.Id == SelectedToAccount.Id)
                preConditions.Add("Нельзя переводить на тот же счет");
            if (TransferAmount <= 0) preConditions.Add("Сумма перевода должна быть больше 0");

            if (preConditions.Any())
            {
                PreConditionStatus = "НЕ ВЫПОЛНЕНО";
                OperationResultMessage = $"Ошибка предусловий: {string.Join(", ", preConditions)}";
                return;
            }

            PreConditionStatus = "ВЫПОЛНЕНО";

            // ВЫПОЛНЕНИЕ ОПЕРАЦИИ
            var result = _transferService.Transfer(SelectedFromAccount!, SelectedToAccount!, TransferAmount);
            OperationResultMessage = result.Message;

            // ПОСТУСЛОВИЯ - упрощенная проверка для UI
            if (result.IsSuccess)
            {
                var postConditions = new List<string>();

                if (SelectedFromAccount!.Balance + SelectedFromAccount.MaxOverdraft < 0)
                    postConditions.Add("Баланс отправителя ниже овердрафта");
                if (SelectedToAccount!.Balance < 0)
                    postConditions.Add("Баланс получателя отрицательный");

                PostConditionStatus = postConditions.Any() ? "НЕ ВЫПОЛНЕНО" : "ВЫПОЛНЕНО";
            }
            else
            {
                PostConditionStatus = "Не применялось (операция не выполнена)";
            }
        }
        catch (Exception ex)
        {
            PreConditionStatus = "ОШИБКА ПРОВЕРКИ";
            PostConditionStatus = "ОШИБКА ПРОВЕРКИ";
            OperationResultMessage = $"Ошибка: {ex.Message}";
        }
    }
}