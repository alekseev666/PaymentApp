using CommunityToolkit.Mvvm.DependencyInjection;
using PaymentApp.ViewModels;
using System.Windows.Controls;
namespace PaymentApp.Views;
public partial class WithdrawView : UserControl
{
    public WithdrawView()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetService<WithdrawViewModel>();
    }
}