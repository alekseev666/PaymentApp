using CommunityToolkit.Mvvm.DependencyInjection;
using PaymentApp.ViewModels;
using System.Windows.Controls;

namespace PaymentApp.Views;

public partial class HelpView : UserControl
{
    public HelpView()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetService<HelpViewModel>();
    }
}