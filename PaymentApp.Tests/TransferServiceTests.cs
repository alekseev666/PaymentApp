using PaymentApp.Models;
using PaymentApp.Services.Impl;
using Xunit;

namespace PaymentApp.Tests;

public class TransferServiceTests
{
	[Fact]
	public void Transfer_Succeeds_WhenSufficientFundsOrOverdraft()
	{
		var from = new Account { Id = 1, Number = "1001", Owner = "A", Balance = 1000m, MaxOverdraft = 200m };
		var to = new Account { Id = 2, Number = "1002", Owner = "B", Balance = 300m, MaxOverdraft = 0m };
		var service = new TransferService();

		var result = service.Transfer(from, to, 1100m);

		Assert.True(result.IsSuccess);
		Assert.Equal("Перевод выполнен успешно", result.Message);
		Assert.Equal(-100m, from.Balance);
		Assert.Equal(1400m, to.Balance);
	}

	[Fact]
	public void Transfer_Fails_WhenInsufficientFundsBeyondOverdraft()
	{
		var from = new Account { Id = 1, Number = "1001", Owner = "A", Balance = 500m, MaxOverdraft = 100m };
		var to = new Account { Id = 2, Number = "1002", Owner = "B", Balance = 0m, MaxOverdraft = 0m };
		var service = new TransferService();

		var result = service.Transfer(from, to, 700m);

		Assert.False(result.IsSuccess);
		Assert.Equal("Недостаточно средств для перевода", result.Message);
		Assert.Equal(500m, from.Balance);
		Assert.Equal(0m, to.Balance);
	}
}

