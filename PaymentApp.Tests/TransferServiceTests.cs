using PaymentApp.Models;
using PaymentApp.Services.Impl;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PaymentApp.Tests;

[TestClass]
public class TransferServiceTests
{
	[TestMethod]
	public void Transfer_Succeeds_WhenSufficientFundsOrOverdraft()
	{
		var from = new Account { Id = 1, Number = "1001", Owner = "A", Balance = 1000m, MaxOverdraft = 200m };
		var to = new Account { Id = 2, Number = "1002", Owner = "B", Balance = 300m, MaxOverdraft = 0m };
		var service = new TransferService();

		var result = service.Transfer(from, to, 1100m);

		Assert.IsTrue(result.IsSuccess);
		Assert.AreEqual("Перевод выполнен успешно", result.Message);
		Assert.AreEqual(-100m, from.Balance);
		Assert.AreEqual(1400m, to.Balance);
	}

	[TestMethod]
	public void Transfer_Fails_WhenInsufficientFundsBeyondOverdraft()
	{
		var from = new Account { Id = 1, Number = "1001", Owner = "A", Balance = 500m, MaxOverdraft = 100m };
		var to = new Account { Id = 2, Number = "1002", Owner = "B", Balance = 0m, MaxOverdraft = 0m };
		var service = new TransferService();

		var result = service.Transfer(from, to, 700m);

		Assert.IsFalse(result.IsSuccess);
		Assert.AreEqual("Недостаточно средств для перевода", result.Message);
		Assert.AreEqual(500m, from.Balance);
		Assert.AreEqual(0m, to.Balance);
	}
}

