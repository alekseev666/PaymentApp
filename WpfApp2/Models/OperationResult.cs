namespace PaymentApp.Models;

public class OperationResult
{
    public bool IsSuccess { get; set; }               // Успех/неудача
    public string Message { get; set; } = string.Empty; // Сообщение для пользователя
    public decimal Amount { get; set; }               // Сумма операции
}