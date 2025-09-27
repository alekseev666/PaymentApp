using CommunityToolkit.Mvvm.ComponentModel;

namespace PaymentApp.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    private string _preConditionStatus = "Не проверено";

    [ObservableProperty]
    private string _postConditionStatus = "Не проверено";

    [ObservableProperty]
    private string _operationResultMessage = string.Empty;
}