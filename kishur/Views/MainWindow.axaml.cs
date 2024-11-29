using Avalonia.Controls;
using kishur.ViewModels;
using System;
using System.Threading.Tasks;

namespace kishur.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    // Stores a reference to the disposable in order to clean it up if needed
    private IDisposable? _interactionDisposable;

    protected override void OnDataContextChanged(EventArgs e)
    {
        // Dispose any old handler
        _interactionDisposable?.Dispose();

        if (DataContext is MainViewModel vm)
        {
            // register the interaction handler
            _interactionDisposable =
                vm.ProgressDialog.RegisterHandler(InteractionHandler);
        }

        base.OnDataContextChanged(e);
    }

    public async Task<int> InteractionHandler(ProgressViewModel input)
    {
        var dialog = new ProgressDialog
        {
            DataContext = input
        };

        return await dialog.ShowDialog<int>(this);
    }
}