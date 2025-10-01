using PaymentApp.Models;

namespace PaymentApp.Services;

/// <summary>
/// Интерфейс для работы со счетами
/// Тут только методы без реализации, реализация в AccountService
/// </summary>
public interface IAccountService
{
    /// <summary>
    /// Получить все счета
    /// </summary>
    /// <returns>Список счетов</returns>
    List<Account> GetAccounts();

    /// <summary>
    /// Найти счет по айди
    /// </summary>
    /// <param name="id">айди счета</param>
    /// <returns>Найденный счет или null</returns>
    Account? GetAccountById(int id);
}