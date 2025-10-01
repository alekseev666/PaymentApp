using PaymentApp.Models;
using PaymentApp.Services;
using PaymentApp.Services.Impl;
using Xunit;

namespace PaymentApp.Tests;

/// <summary>
/// Содержит модульные тесты для операций снятия средств со счета.
/// Проверяет корректность работы метода Withdraw в различных сценариях.
/// </summary>
public class WithdrawTests
{
    /// <summary>
    /// Проверяет успешное снятие средств при использовании овердрафта в пределах лимита.
    /// Ожидается, что операция завершится успешно и баланс станет отрицательным в разрешенных пределах.
    /// </summary>
    /// <remarks>
    /// Тестовый сценарий:
    /// - Начальный баланс: 200
    /// - Лимит овердрафта: 150
    /// - Сумма снятия: 300
    /// - Ожидаемый баланс: -100 (в пределах лимита)
    /// </remarks>
    [Fact]
    public void Withdraw_Succeeds_WithOverdraftWithinLimit()
    {
        var account = new Account
        {
            Id = 1,
            Number = "1001",
            Owner = "A",
            Balance = 200m,
            MaxOverdraft = 150m
        };
        var service = new TransferService();
        var result = service.Withdraw(account, 300m);
        Assert.True(result.IsSuccess);
        Assert.Equal("Снятие выполнено успешно", result.Message);
        Assert.Equal(-100m, account.Balance);
    }

    /// <summary>
    /// Проверяет неудачное снятие средств при превышении лимита овердрафта.
    /// Ожидается, что операция завершится ошибкой и баланс останется неизменным.
    /// </summary>
    /// <remarks>
    /// Тестовый сценарий:
    /// - Начальный баланс: 100
    /// - Лимит овердрафта: 50
    /// - Сумма снятия: 200
    /// - Доступно средств: 150 (100 + 50)
    /// - Ожидается отказ в операции
    /// </remarks>
    [Fact]
    public void Withdraw_Fails_WhenExceedsOverdraftLimit()
    {
        var account = new Account
        {
            Id = 1,
            Number = "1001",
            Owner = "A",
            Balance = 100m,
            MaxOverdraft = 50m
        };
        var service = new TransferService();
        var result = service.Withdraw(account, 200m);
        Assert.False(result.IsSuccess);
        Assert.Equal("Недостаточно средств для снятия", result.Message);
        Assert.Equal(100m, account.Balance);
    }
}

/// <summary>
/// Содержит модульные тесты для операций перевода средств между счетами.
/// Проверяет корректность работы метода Transfer в различных сценариях.
/// </summary>
public class TransferTests
{
    /// <summary>
    /// Проверяет успешный перевод средств при достаточном балансе на счете отправителя.
    /// Ожидается, что операция завершится успешно и балансы обоих счетов будут корректно обновлены.
    /// </summary>
    /// <remarks>
    /// Тестовый сценарий:
    /// - Счет отправителя: 1000 → 700
    /// - Счет получателя: 500 → 800
    /// - Сумма перевода: 300
    /// </remarks>
    [Fact]
    public void Transfer_Succeeds_WhenSufficientFunds()
    {
        var fromAccount = new Account { Id = 1, Balance = 1000m, MaxOverdraft = 0 };
        var toAccount = new Account { Id = 2, Balance = 500m, MaxOverdraft = 0 };
        var service = new TransferService();
        var result = service.Transfer(fromAccount, toAccount, 300m);
        Assert.True(result.IsSuccess);
        Assert.Equal(700m, fromAccount.Balance);
        Assert.Equal(800m, toAccount.Balance);
    }

    /// <summary>
    /// Проверяет неудачный перевод средств при недостаточном балансе на счете отправителя.
    /// Ожидается, что операция завершится ошибкой и балансы обоих счетов останутся неизменными.
    /// </summary>
    /// <remarks>
    /// Тестовый сценарий:
    /// - Счет отправителя: 100 (недостаточно для перевода 200)
    /// - Ожидается отказ в операции
    /// - Балансы не должны измениться
    /// </remarks>
    [Fact]
    public void Transfer_Fails_WhenInsufficientFunds()
    {
        var fromAccount = new Account { Id = 1, Balance = 100m, MaxOverdraft = 0 };
        var toAccount = new Account { Id = 2, Balance = 500m, MaxOverdraft = 0 };
        var service = new TransferService();
        var result = service.Transfer(fromAccount, toAccount, 200m);
        Assert.False(result.IsSuccess);
        Assert.Equal(100m, fromAccount.Balance);
        Assert.Equal(500m, toAccount.Balance);
    }
}

/// <summary>
/// Содержит модульные тесты для операций пополнения счета.
/// Проверяет корректность работы метода Deposit в различных сценариях.
/// </summary>
public class DepositTests
{
    /// <summary>
    /// Проверяет успешное пополнение счета положительной суммой.
    /// Ожидается, что операция завершится успешно и баланс увеличится на указанную сумму.
    /// </summary>
    /// <remarks>
    /// Тестовый сценарий:
    /// - Начальный баланс: 1000
    /// - Сумма пополнения: 500
    /// - Ожидаемый баланс: 1500
    /// </remarks>
    [Fact]
    public void Deposit_Succeeds_WhenPositiveAmount()
    {
        var account = new Account { Id = 1, Balance = 1000m, MaxOverdraft = 0 };
        var service = new TransferService();
        var result = service.Deposit(account, 500m);
        Assert.True(result.IsSuccess);
        Assert.Equal(1500m, account.Balance);
        Assert.Equal("Пополнение выполнено успешно", result.Message);
    }

    /// <summary>
    /// Проверяет успешное пополнение счета, находящегося в овердрафте.
    /// Ожидается, что операция завершится успешно и баланс станет положительным после погашения овердрафта.
    /// </summary>
    /// <remarks>
    /// Тестовый сценарий:
    /// - Начальный баланс: -100 (овердрафт)
    /// - Сумма пополнения: 300
    /// - Ожидаемый баланс: 200 (300 - 100 = 200)
    /// </remarks>
    [Fact]
    public void Deposit_Succeeds_WhenAccountHasOverdraft()
    {
        var account = new Account { Id = 1, Balance = -100m, MaxOverdraft = 200m };
        var service = new TransferService();
        var result = service.Deposit(account, 300m);
        Assert.True(result.IsSuccess);
        Assert.Equal(200m, account.Balance);
    }
}