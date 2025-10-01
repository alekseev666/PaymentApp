using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PaymentApp.Models;
using PaymentApp.Services;
using System.Windows.Media;

namespace PaymentApp.ViewModels;

/// <summary>
/// ViewModel для операции пополнения счета
/// Отвечает за логику пополнения и отображение статусов в UI
/// </summary>
public partial class DepositViewModel : BaseViewModel
{
    private readonly ITransferService _transferService;
    private readonly IAccountService _accountService;

    /// <summary>
    /// Список всех доступных счетов для выбора
    /// Берется из сервиса, обновляется автоматически
    /// </summary>
    public List<Account> Accounts => _accountService.GetAccounts();

    /// <summary>
    /// Выбранный счет для пополнения
    /// Может быть null если пользователь еще не выбрал
    /// </summary>
    [ObservableProperty]
    private Account? _selectedAccount;

    /// <summary>
    /// Сумма для пополнения
    /// Должна быть больше 0 иначе будет ошибка
    /// </summary>
    [ObservableProperty]
    private decimal _depositAmount;

    /// <summary>
    /// Конструктор, принимает сервисы через DI
    /// </summary>
    /// <param name="transferService">Сервис для операций</param>
    /// <param name="accountService">Сервис для работы со счетами</param>
    public DepositViewModel(ITransferService transferService, IAccountService accountService)
    {
        _transferService = transferService;
        _accountService = accountService;
    }

    /// <summary>
    /// Команда для выполнения пополнения счета
    /// Вызывается когда пользователь нажимает кнопку
    /// Сначала проверяет Pre условия, потом выполняет, потом проверяет Post условия
    /// </summary>
    [RelayCommand]
    private void ExecuteDeposit()
    {
        // Сбрасываем статусы перед выполнением
        PreConditionStatus = "Проверяется...";
        PostConditionStatus = "Не проверено";
        PreConditionColor = Brushes.Black;
        PostConditionColor = Brushes.Black;
        OperationResultMessage = string.Empty;

        try
        {
            // ПРЕДУСЛОВИЯ - проверяем перед операцией
            var preConditions = new List<string>();

            if (SelectedAccount == null) preConditions.Add("Не выбран счет");
            if (DepositAmount <= 0) preConditions.Add("Сумма пополнения должна быть больше 0");

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

            // ОПЕРАЦИЯ - выполняем пополнение
            var oldBalance = SelectedAccount!.Balance; // запоминаем старый баланс для проверки постусловий
            var result = _transferService.Deposit(SelectedAccount!, DepositAmount);
            OperationResultMessage = result.Message;

            // ПОСТУСЛОВИЯ - проверяем после операции
            PostConditionStatus = SelectedAccount.Balance == oldBalance + DepositAmount
                ? "ВЫПОЛНЕНО"
                : "НЕ ВЫПОЛНЕНО";
            PostConditionColor = SelectedAccount.Balance == oldBalance + DepositAmount
                ? Brushes.Green
                : Brushes.Red;
        }
        catch (Exception ex)
        {
            // Если что-то пошло не так - показываем ошибку
            PreConditionStatus = "ОШИБКА ПРОВЕРКИ";
            PreConditionColor = Brushes.Red;
            OperationResultMessage = $"Ошибка: {ex.Message}";
        }
    }

    /// <summary>
    /// Команда для показа контракта операции
    /// </summary>
    [RelayCommand]
    private void ShowContract()
    {
        ContractText = @"
КОНТРАКТ ОПЕРАЦИИ 'ПОПОЛНЕНИЕ'

ПРЕДУСЛОВИЯ (Pre):
1. Счет не null
2. Сумма пополнения > 0

ПОСТУСЛОВИЯ (Post):
1. Баланс увеличился на сумму пополнения";
    }
}