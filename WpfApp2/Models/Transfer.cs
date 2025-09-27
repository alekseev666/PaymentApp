namespace PaymentApp.Models;

public class Transfer
{
    public Account FromAccount { get; set; }  // Счет отправителя
    public Account ToAccount { get; set; }    // Счет получателя
    public decimal Amount { get; set; }       // Сумма перевода
}