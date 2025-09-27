using CommunityToolkit.Mvvm.ComponentModel;

namespace PaymentApp.Models;

public class Account
{
    public int Id { get; set; }
    public string Number { get; set; } = string.Empty; // Номер счета
    public string Owner { get; set; } = string.Empty;  // Владелец
    public decimal Balance { get; set; }               // Баланс
    public decimal MaxOverdraft { get; set; } = 0;     // Макс. разрешенный овердрафт
}