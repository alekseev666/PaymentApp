using CommunityToolkit.Diagnostics;
using PaymentApp.Models;
using System;
using System.Diagnostics;

namespace PaymentApp.Services;

public class TransferService : ITransferService
{
    public OperationResult Transfer(Account from, Account to, decimal amount)
    {
        // ПРЕДУСЛОВИЯ (Pre-conditions)
        Guard.IsNotNull(from, nameof(from));
        Guard.IsNotNull(to, nameof(to));
        Guard.IsGreaterThan(amount, 0, nameof(amount));
        Guard.IsNotEqualTo(from.Id, to.Id, nameof(to.Id)); // Нельзя переводить на тот же счет

        // Логика операции
        var result = new OperationResult { Amount = amount };

        if (from.Balance - amount >= -from.MaxOverdraft)
        {
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

        // ПОСТУСЛОВИЯ (Post-conditions)
        Debug.Assert(!result.IsSuccess || from.Balance + from.MaxOverdraft >= 0,
            "Постусловие нарушено: баланс отправителя ниже разрешенного овердрафта");
        Debug.Assert(!result.IsSuccess || to.Balance >= 0,
            "Постусловие нарушено: баланс получателя отрицательный");

        return result;
    }

    public OperationResult Withdraw(Account account, decimal amount)
    {
        // ПРЕДУСЛОВИЯ
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

        // ПОСТУСЛОВИЯ
        Debug.Assert(!result.IsSuccess || account.Balance + account.MaxOverdraft >= 0,
            "Постусловие нарушено: баланс после снятия ниже разрешенного овердрафта");

        return result;
    }

    public OperationResult Deposit(Account account, decimal amount)
    {
        // ПРЕДУСЛОВИЯ
        Guard.IsNotNull(account, nameof(account));
        Guard.IsGreaterThan(amount, 0, nameof(amount));

        var oldBalance = account.Balance;
        account.Balance += amount;

        var result = new OperationResult
        {
            IsSuccess = true,
            Amount = amount,
            Message = "Пополнение выполнено успешно"
        };

        // ПОСТУСЛОВИЯ
        Debug.Assert(account.Balance == oldBalance + amount,
            "Постусловие нарушено: баланс увеличился не на сумму пополнения");

        return result;
    }
}