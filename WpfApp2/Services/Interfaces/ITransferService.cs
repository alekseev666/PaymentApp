using PaymentApp.Models;

namespace PaymentApp.Services;

/// <summary>
/// Интерфейс для операций перевода денег
/// Описывает какие методы должны быть в TransferService
/// </summary>
public interface ITransferService
{
    /// <summary>
    /// Перевод денег между счетами
    /// </summary>
    /// <param name="from">Счет отправителя</param>
    /// <param name="to">Счет получателя</param>
    /// <param name="amount">Сумма перевода</param>
    /// <returns>Результат операции</returns>
    OperationResult Transfer(Account from, Account to, decimal amount);

    /// <summary>
    /// Снятие денег со счета
    /// </summary>
    /// <param name="account">Счет для снятия</param>
    /// <param name="amount">Сумма снятия</param>
    /// <returns>Результат операции</returns>
    OperationResult Withdraw(Account account, decimal amount);

    /// <summary>
    /// Пополнение счета
    /// </summary>
    /// <param name="account">Счет для пополнения</param>
    /// <param name="amount">Сумма пополнения</param>
    /// <returns>Результат операции</returns>
    OperationResult Deposit(Account account, decimal amount);
}