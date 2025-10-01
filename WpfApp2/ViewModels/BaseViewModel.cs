using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Media;

namespace PaymentApp.ViewModels;

/// <summary>
/// Базовая ViewModel от которой наследуются все остальные
/// Тут общие свойства которые используются во всех операциях
/// </summary>
public partial class BaseViewModel : ObservableObject
{
    /// <summary>
    /// Статус проверки предусловий - показывается в UI
    /// </summary>
    [ObservableProperty]
    private string _preConditionStatus = "Не проверено";

    /// <summary>
    /// Статус проверки постусловий - тоже показывается в интерфейсе
    /// </summary>
    [ObservableProperty]
    private string _postConditionStatus = "Не проверено";

    /// <summary>
    /// Сообщение о результате операции - успех или ошибка
    /// </summary>
    [ObservableProperty]
    private string _operationResultMessage = string.Empty;

    /// <summary>
    /// Текст контракта операции - Pre и Post условия
    /// </summary>
    [ObservableProperty]
    private string _contractText = "Нажмите 'КОНТРАКТ' для просмотра";

    /// <summary>
    /// Цвет для отображения статуса предусловий
    /// Зеленый - выполнено, красный - не выполнено, черный - не проверялось
    /// </summary>
    [ObservableProperty]
    private Brush _preConditionColor = Brushes.Black;

    /// <summary>
    /// Цвет для отображения статуса постусловий
    /// </summary>
    [ObservableProperty]
    private Brush _postConditionColor = Brushes.Black;
}