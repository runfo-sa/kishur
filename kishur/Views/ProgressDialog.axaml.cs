using Avalonia.Controls;

namespace kishur.Views;

public partial class ProgressDialog : Window
{
    public ProgressDialog()
    {
        InitializeComponent();
    }

    private void CancelCommand(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close();
    }
}