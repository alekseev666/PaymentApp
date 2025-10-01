using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PaymentApp.Models;
using PaymentApp.Services;
using System.Windows.Media;

namespace PaymentApp.ViewModels;

/// <summary>
/// ViewModel для операции снятия денег со счета
/// </summary>
public partial class WithdrawViewModel : BaseViewModel
{
    private readonly ITransferService _transferService;
    private readonly IAccountService _accountService;

    /// <summary>
    /// Список всех доступных счетов для выбора
    /// </summary>
    public List<Account> Accounts => _accountService.GetAccounts();

    /// <summary>
    /// Выбранный счет для снятия денег
    /// </summary>
    [ObservableProperty]
    private Account? _selectedAccount;

    /// <summary>
    /// Сумма для снятия со счета
    /// </summary>
    [ObservableProperty]
    private decimal _withdrawAmount;

    /// <summary>
    /// Конструктор, принимает сервисы через DI
    /// </summary>
    /// <param name="transferService">Сервис для операций</param>
    /// <param name="accountService">Сервис для работы со счетами</param>
    public WithdrawViewModel(ITransferService transferService, IAccountService accountService)
    {
        _transferService = transferService;
        _accountService = accountService;
        // Тут могла бы быть какая-то инициализация но пока не нужно
    }

    /// <summary>
    /// Команда для выполнения снятия денег со счета
    /// Вызывается когда пользователь нажимает кнопку
    /// Проверяет условия, выполняет операцию, проверяет результат
    /// </summary>
    [RelayCommand]
    private void ExecuteWithdraw()
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
            if (WithdrawAmount <= 0) preConditions.Add("Сумма снятия должна быть больше 0");

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

            // ОПЕРАЦИЯ - выполняем снятие денег
            var result = _transferService.Withdraw(SelectedAccount!, WithdrawAmount);
            OperationResultMessage = result.Message;

            // ПОСТУСЛОВИЯ - проверяем после операции
            if (result.IsSuccess)
            {
                // Проверяем что баланс не ушел ниже разрешенного овердрафта
                PostConditionStatus = SelectedAccount!.Balance + SelectedAccount.MaxOverdraft >= 0
                    ? "ВЫПОЛНЕНО"
                    : "НЕ ВЫПОЛНЕНО";
                PostConditionColor = SelectedAccount!.Balance + SelectedAccount.MaxOverdraft >= 0
                    ? Brushes.Green
                    : Brushes.Red;
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
            // Если что-то пошло не так - показываем ошибку
            PreConditionStatus = "ОШИБКА ПРОВЕРКИ";
            PreConditionColor = Brushes.Red;
            OperationResultMessage = $"Ошибка: {ex.Message}";
        }
    }

    /// <summary>
    /// Команда для показа контракта операции снятия
    /// Просто устанавливает текст который потом показывается в UI
    /// </summary>
    [RelayCommand]
    private void ShowContract()
    {
        ContractText = @"
КОНТРАКТ ОПЕРАЦИИ 'СНЯТИЕ'

ПРЕДУСЛОВИЯ (Pre):
1. Счет не null
2. Сумма снятия > 0
3. Баланс - сумма >= -овердрафт

ПОСТУСЛОВИЯ (Post):
1. Баланс уменьшился на сумму снятия
2. Баланс >= -овердрафт";
    }
}