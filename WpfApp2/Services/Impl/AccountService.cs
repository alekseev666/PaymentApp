using PaymentApp.Models;

namespace PaymentApp.Services.Impl;

/// <summary>
/// Сервис для работы со счетами, тут хранятся все наши счета
/// </summary>
public class AccountService : IAccountService
{
    // тествые данные, три счета для примеров
    // в идеале надо бы вынести в конфиг или базу
    private readonly List<Account> _accounts = new()
    {
        new Account { Id = 1, Number = "1001", Owner = "Иван Иванов", Balance = 5000, MaxOverdraft = 1000 },
        new Account { Id = 2, Number = "1002", Owner = "Петр Петров", Balance = 3000, MaxOverdraft = 500 },
        new Account { Id = 3, Number = "1003", Owner = "Сидор Сидоров", Balance = 10000, MaxOverdraft = 0 }
    };

    /// <summary>
    /// Получить все счета какие есть
    /// </summary>
    /// <returns>Список всех счетов</returns>
    public List<Account> GetAccounts() => _accounts;

    /// <summary>
    /// Найти счет по айдишнику
    /// Если не найдет - вернет null, надо не забыть проверить на null когда вызываешь
    /// </summary>
    /// <param name="id">айди счета который ищем</param>
    /// <returns>Найденый счет или null если нету</returns>
    public Account? GetAccountById(int id) => _accounts.FirstOrDefault(a => a.Id == id);
}