using PaymentApp.Models;
using PaymentApp.Services.Impl;
using Xunit;

namespace PaymentApp.Tests;

public class WithdrawServiceTests
{
	[Fact]
	public void Withdraw_Succeeds_WithOverdraftWithinLimit()
	{
		var account = new Account { Id = 1, Number = "1001", Owner = "A", Balance = 200m, MaxOverdraft = 150m };
		var service = new TransferService();

		var result = service.Withdraw(account, 300m);

		Assert.True(result.IsSuccess);
		Assert.Equal("Снятие выполнено успешно", result.Message);
		Assert.Equal(-100m, account.Balance);
	}

	[Fact]
	public void Withdraw_Fails_WhenExceedsOverdraftLimit()
	{
		var account = new Account { Id = 1, Number = "1001", Owner = "A", Balance = 100m, MaxOverdraft = 50m };
		var service = new TransferService();

		var result = service.Withdraw(account, 200m);

		Assert.False(result.IsSuccess);
		Assert.Equal("Недостаточно средств для снятия", result.Message);
		Assert.Equal(100m, account.Balance);
	}
}

