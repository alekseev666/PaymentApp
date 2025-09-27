using PaymentApp.Models;

namespace PaymentApp.Services;

public interface ITransferService
{
    OperationResult Transfer(Account from, Account to, decimal amount);
    OperationResult Withdraw(Account account, decimal amount);
    OperationResult Deposit(Account account, decimal amount);
}