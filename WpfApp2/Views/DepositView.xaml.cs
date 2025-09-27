using CommunityToolkit.Mvvm.DependencyInjection;
using PaymentApp.ViewModels;
using System.Windows.Controls;

namespace PaymentApp.Views;

public partial class DepositView : UserControl
{
    public DepositView()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetService<DepositViewModel>();
    }
}