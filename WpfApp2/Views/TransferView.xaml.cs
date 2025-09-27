using CommunityToolkit.Mvvm.DependencyInjection;
using PaymentApp.ViewModels;
using System.Windows.Controls;

namespace PaymentApp.Views;

public partial class TransferView : UserControl
{
    public TransferView()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetService<TransferViewModel>();
    }
}