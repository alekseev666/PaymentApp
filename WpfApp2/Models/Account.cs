using CommunityToolkit.Mvvm.ComponentModel;

namespace PaymentApp.Models;

// Модель банковского счета для хранения данных о счете
// Наследуемся от ObservableObject чтобы уведомлять UI об изменениях
public partial class Account : ObservableObject
{
    // Уникальный идентификатор счета
    [ObservableProperty]
    private int _id;

    // номер счета например "40817810099910004312"
    // по хорошему нужно бы добавить валидацию но и так сойдет
    [ObservableProperty]
    private string _number = string.Empty;

    // ФИО владельца счета
    [ObservableProperty]
    private string _owner = string.Empty;

    // Текущий баланс счета в рублях
    // Должен быть не меньше -MaxOverdraft (инвариант)
    [ObservableProperty]
    private decimal _balance;

    // Максимальный разрешенный овердрафт (отрицательный баланс)
    // Если 0 - то овердрафт не разрешен
    [ObservableProperty]
    private decimal _maxOverdraft;
}