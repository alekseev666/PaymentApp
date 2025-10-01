using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PaymentApp.Models;
using PaymentApp.Services;
using System.Diagnostics;
using System.Windows.Media;

namespace PaymentApp.ViewModels;

/// <summary>
/// ViewModel для операции перевода между счетами
/// </summary>
public partial class TransferViewModel : BaseViewModel
{
    private readonly ITransferService _transferService;
    private readonly IAccountService _accountService;

    /// <summary>
    /// Список всех доступных счетов
    /// </summary>
    public List<Account> Accounts => _accountService.GetAccounts();

    /// <summary>
    /// Выбранный счет отправителя
    /// </summary>
    [ObservableProperty]
    private Account? _selectedFromAccount;
    
    /// <summary>
    /// Выбранный счет получателя  
    /// </summary>
    [ObservableProperty]
    private Account? _selectedToAccount;

    /// <summary>
    /// Сумма для перевода
    /// </summary>
    [ObservableProperty]
    private decimal _transferAmount;

    /// <summary>
    /// Конструктор, принимает сервисы через DI
    /// </summary>
    /// <param name="transferService">сервис для операций перевода</param>
    /// <param name="accountService">сервис для работы со счетами</param>
    public TransferViewModel(ITransferService transferService, IAccountService accountService)
    {
        _transferService = transferService;
        _accountService = accountService;
        // Тут могла бы быть какая-то инициализация но пока не придумал что
    }

    /// <summary>
    /// Команда для выполнения перевода между счетами
    /// Сначала Pre условия, потом операция, потом Post условия
    /// </summary>
    [RelayCommand]
    private void ExecuteTransfer()
    {
        // Сбрасываем статусы и цвета перед выполнением
        PreConditionStatus = "Проверяется...";
        PostConditionStatus = "Не проверено";
        PreConditionColor = Brushes.Black;
        PostConditionColor = Brushes.Black;
        OperationResultMessage = string.Empty;

        try
        {
            // ПРЕДУСЛОВИЯ - проверяем перед операцией
            var preConditions = new List<string>();

            if (SelectedFromAccount == null) preConditions.Add("Не выбран счет отправителя");
            if (SelectedToAccount == null) preConditions.Add("Не выбран счет получателя");
            if (SelectedFromAccount != null && SelectedToAccount != null && SelectedFromAccount.Id == SelectedToAccount.Id)
                preConditions.Add("Нельзя переводить на тот же счет");
            if (TransferAmount <= 0) preConditions.Add("Сумма перевода должна быть больше 0");

            // Если есть ошибки в предусловиях - показываем и выходим
            if (preConditions.Any())
            {
                PreConditionStatus = "НЕ ВЫПОЛНЕНО";
                PreConditionColor = Brushes.Red;
                OperationResultMessage = $"Ошибка предусловий: {string.Join(", ", preConditions)}";
                return;
            }

            // Предусловия выполнены - показываем зеленый
            PreConditionStatus = "ВЫПОЛНЕНО";
            PreConditionColor = Brushes.Green;

            // ВЫПОЛНЕНИЕ ОПЕРАЦИИ - собственно перевод денег
            var result = _transferService.Transfer(SelectedFromAccount!, SelectedToAccount!, TransferAmount);
            OperationResultMessage = result.Message;

            // ПОСТУСЛОВИЯ - проверяем после выполнения операции
            if (result.IsSuccess)
            {
                var postConditions = new List<string>();

                if (SelectedFromAccount!.Balance + SelectedFromAccount.MaxOverdraft < 0)
                    postConditions.Add("Баланс отправителя ниже овердрафта");
                if (SelectedToAccount!.Balance < 0)
                    postConditions.Add("Баланс получателя отрицательный");

                // Если есть нарушения постусловий - показываем красный
                if (postConditions.Any())
                {
                    PostConditionStatus = "НЕ ВЫПОЛНЕНО";
                    PostConditionColor = Brushes.Red;
                }
                else
                {
                    PostConditionStatus = "ВЫПОЛНЕНО";
                    PostConditionColor = Brushes.Green;
                }
            }
            else
            {
                // Если операция не выполнена - постусловия не проверяем
                PostConditionStatus = "Не применялось (операция не выполнена)";
                PostConditionColor = Brushes.Gray;
            }
        }
        catch (Exception ex)
        {
            // Если что-то пошло не так - показываем ошибки везде
            PreConditionStatus = "ОШИБКА ПРОВЕРКИ";
            PreConditionColor = Brushes.Red;
            PostConditionStatus = "ОШИБКА ПРОВЕРКИ";
            PostConditionColor = Brushes.Red;
            OperationResultMessage = $"Ошибка: {ex.Message}";
        }
    }

    /// <summary>
    /// Команда для показа контракта операции перевода
    /// Просто устанавливает текст который потом показывается в UI
    /// </summary>
    [RelayCommand]
    private void ShowContract()
    {
        ContractText = @"
КОНТРАКТ ОПЕРАЦИИ 'ПЕРЕВОД'

ПРЕДУСЛОВИЯ (Pre):
1. Счет отправителя не null
2. Счет получателя не null  
3. Сумма перевода > 0
4. Счета разные
5. Баланс отправителя - сумма >= -овердрафт

ПОСТУСЛОВИЯ (Post):
1. Баланс отправителя уменьшился на сумму перевода
2. Баланс получателя увеличился на сумму перевода
3. Баланс отправителя >= -овердрафт
4. Баланс получателя >= 0";
    }
}