using PaymentApp.Models;
using PaymentApp.Services.Impl;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PaymentApp.Tests;

[TestClass]
public class WithdrawServiceTests
{
	[TestMethod]
	public void Withdraw_Succeeds_WithOverdraftWithinLimit()
	{
		var account = new Account { Id = 1, Number = "1001", Owner = "A", Balance = 200m, MaxOverdraft = 150m };
		var service = new TransferService();

		var result = service.Withdraw(account, 300m);

		Assert.IsTrue(result.IsSuccess);
		Assert.AreEqual("Снятие выполнено успешно", result.Message);
		Assert.AreEqual(-100m, account.Balance);
	}

	[TestMethod]
	public void Withdraw_Fails_WhenExceedsOverdraftLimit()
	{
		var account = new Account { Id = 1, Number = "1001", Owner = "A", Balance = 100m, MaxOverdraft = 50m };
		var service = new TransferService();

		var result = service.Withdraw(account, 200m);

		Assert.IsFalse(result.IsSuccess);
		Assert.AreEqual("Недостаточно средств для снятия", result.Message);
		Assert.AreEqual(100m, account.Balance);
	}
}

