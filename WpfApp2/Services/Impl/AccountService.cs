using PaymentApp.Models;

namespace PaymentApp.Services;

public class AccountService : IAccountService
{
    private readonly List<Account> _accounts = new()
    {
        new Account { Id = 1, Number = "1001", Owner = "Иван Иванов", Balance = 5000, MaxOverdraft = 1000 },
        new Account { Id = 2, Number = "1002", Owner = "Петр Петров", Balance = 3000, MaxOverdraft = 500 },
        new Account { Id = 3, Number = "1003", Owner = "Сидор Сидоров", Balance = 10000, MaxOverdraft = 0 }
    };

    public List<Account> GetAccounts() => _accounts;

    public Account? GetAccountById(int id) => _accounts.FirstOrDefault(a => a.Id == id);
}