using System;
using PaymentApp.Models;
using PaymentApp.Services.Impl;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PaymentApp.Tests;

[TestClass]
public class DepositServiceTests
{
	[TestMethod]
	public void Deposit_Succeeds_IncreasesBalance()
	{
		var account = new Account { Id = 1, Number = "1001", Owner = "A", Balance = 100m, MaxOverdraft = 0m };
		var service = new TransferService();

		var result = service.Deposit(account, 250m);

		Assert.IsTrue(result.IsSuccess);
		Assert.AreEqual("Пополнение выполнено успешно", result.Message);
		Assert.AreEqual(350m, account.Balance);
	}

	[TestMethod]
	public void Deposit_Throws_OnNonPositiveAmount()
	{
		var account = new Account { Id = 1, Number = "1001", Owner = "A", Balance = 100m, MaxOverdraft = 0m };
		var service = new TransferService();

		Assert.ThrowsException<ArgumentOutOfRangeException>(() => service.Deposit(account, 0m));
	}
}

