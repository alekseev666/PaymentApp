using PaymentApp.Models;

namespace PaymentApp.Services;

public interface IAccountService
{
    List<Account> GetAccounts();
    Account? GetAccountById(int id);
}