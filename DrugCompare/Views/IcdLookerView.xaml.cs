using System.Windows;
using System.Windows.Controls;
using DrugCompare.ViewModels;

namespace DrugCompare.Views;

public partial class IcdLookerView : UserControl
{
    public IcdLookerView()
    {
        InitializeComponent();
        IsVisibleChanged += IcdLookerView_IsVisibleChanged;
    }

    private void IcdLookerView_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is not true)
        {
            return;
        }

        if (DataContext is not MainViewModel viewModel)
        {
            return;
        }

        if (viewModel.LoadIcdCategoriesCommand.CanExecute(null))
        {
            viewModel.LoadIcdCategoriesCommand.Execute(null);
        }
    }
}