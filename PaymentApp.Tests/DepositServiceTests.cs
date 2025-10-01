using System;
using PaymentApp.Models;
using PaymentApp.Services.Impl;
using Xunit;

namespace PaymentApp.Tests;

public class DepositServiceTests
{
	[Fact]
	public void Deposit_Succeeds_IncreasesBalance()
	{
		var account = new Account { Id = 1, Number = "1001", Owner = "A", Balance = 100m, MaxOverdraft = 0m };
		var service = new TransferService();

		var result = service.Deposit(account, 250m);

		Assert.True(result.IsSuccess);
		Assert.Equal("Пополнение выполнено успешно", result.Message);
		Assert.Equal(350m, account.Balance);
	}

	[Fact]
	public void Deposit_Throws_OnNonPositiveAmount()
	{
		var account = new Account { Id = 1, Number = "1001", Owner = "A", Balance = 100m, MaxOverdraft = 0m };
		var service = new TransferService();

		Assert.Throws<ArgumentOutOfRangeException>(() => service.Deposit(account, 0m));
	}
}

