using System.Diagnostics;
using CommunityToolkit.Diagnostics;
using PaymentApp.Models;

namespace PaymentApp.Services.Impl;

/// <summary>
/// Сервис для выполнения банковских операций
/// Тут вся основная логика переводов и операций по счетам
/// </summary>
public class TransferService : ITransferService
{
    /// <summary>
    /// Перевод денег с одного счета на другой
    /// Сначала проверяем предусловия, потом выполняем, потом проверяем постусловия
    /// </summary>
    /// <param name="from">Откуда переводим</param>
    /// <param name="to">Куда переводим</param>
    /// <param name="amount">Сколько переводим</param>
    /// <returns>Результат операции</returns>
    public OperationResult Transfer(Account from, Account to, decimal amount)
    {
        // Проверяем предусловия - если что-то не так, будет исключение
        Guard.IsNotNull(from, nameof(from));
        Guard.IsNotNull(to, nameof(to));
        Guard.IsGreaterThan(amount, 0, nameof(amount));
        Guard.IsNotEqualTo(from.Id, to.Id, nameof(to.Id));

        var result = new OperationResult { Amount = amount };

        // Проверяем хватает ли денег с учетом овердрафта
        if (from.Balance - amount >= -from.MaxOverdraft)
        {
            // Выполняем операцию
            from.Balance -= amount;
            to.Balance += amount;

            result.IsSuccess = true;
            result.Message = "Перевод выполнен успешно";
        }
        else
        {
            result.IsSuccess = false;
            result.Message = "Недостаточно средств для перевода";
        }

        // Проверяем постусловия в дебаге
        Debug.Assert(!result.IsSuccess || from.Balance + from.MaxOverdraft >= 0, "Постусловие нарушено: баланс отправителя ушел в минус больше чем можно");
        Debug.Assert(!result.IsSuccess || to.Balance >= 0, "Постусловие нарушено: баланс получателя стал отрицательным");

        return result;
    }

    /// <summary>
    /// Снятие денег со счета
    /// Похоже на перевод но только в одну сторону
    /// </summary>
    /// <param name="account">Счет с которого снимаем</param>
    /// <param name="amount">Сумма для снятия</param>
    /// <returns>Результат операции</returns>
    public OperationResult Withdraw(Account account, decimal amount)
    {
        // Проверяем что счет существует и сумма положительная
        Guard.IsNotNull(account, nameof(account));
        Guard.IsGreaterThan(amount, 0, nameof(amount));

        var result = new OperationResult { Amount = amount };

        if (account.Balance - amount >= -account.MaxOverdraft)
        {
            account.Balance -= amount;
            result.IsSuccess = true;
            result.Message = "Снятие выполнено успешно";
        }
        else
        {
            result.IsSuccess = false;
            result.Message = "Недостаточно средств для снятия";
        }

        // Проверяем что после снятия баланс не ушел ниже разрешенного
        Debug.Assert(!result.IsSuccess || account.Balance + account.MaxOverdraft >= 0, "Постусловие нарушено: баланс после снятия меньше чем можно");

        return result;
    }

    /// <summary>
    /// Пополнение счета - самая простая операция
    /// Тут практически нечего проверять кроме базовых вещей
    /// </summary>
    /// <param name="account">Счет который пополняем</param>
    /// <param name="amount">Сумма пополнения</param>
    /// <returns>Результат операции</returns>
    public OperationResult Deposit(Account account, decimal amount)
    {
        // Тут предусловия простые - счет должен быть и сумма положительная
        Guard.IsNotNull(account, nameof(account));
        Guard.IsGreaterThan(amount, 0, nameof(amount));

        // Просто увеличиваем баланс
        account.Balance += amount;

        var result = new OperationResult
        {
            IsSuccess = true,
            Amount = amount,
            Message = "Пополнение выполнено успешно"
        };

        // Постусловия для депозита простые - баланс должен увеличиться на нужную сумму
        // но это и так очевидно из кода выше

        return result;
    }
}