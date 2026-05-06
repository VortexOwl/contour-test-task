using Avalonia.Controls;
using Avalonia.Interactivity;

using contour_test_task.TransformXmlApp;
using contour_test_task.Log;
using contour_test_task.ViewModels;

namespace contour_test_task;

public partial class MainWindow : Window
{
    private readonly MainWindowViewModel _viewModel;

    public MainWindow()
    {
        InitializeComponent();
        _viewModel = new MainWindowViewModel();
        DataContext = _viewModel;
    }

    private void ActivationTransformXml(object? sender, RoutedEventArgs e) 
    {
        Logger.Print("DEBUG", "Start Transform");
        TransformXml.TransformXmlMain();
        _viewModel.Reload();
    }

    private void UpdateData(object? sender, RoutedEventArgs e) 
    {
        Logger.Print("DEBUG", "Start Update");
        string name = AddName.Text ?? "";
        string surname = AddSurname.Text ?? "";
        string mount = AddMount.Text ?? "";
        string amount = AddAmount.Text ?? "";
        bool isAmount = false;

        if (decimal.TryParse(amount, out decimal dAmount) && dAmount >= 0)
        {
            isAmount = true;
        }

        if (name != "" && surname != "" && mount != "" && isAmount) 
        {
            TransformXml.AddRecordData(name, surname, mount, amount);
        } else {
            Logger.Print("INFO", "В запросе было передано поле с некорректным значением. Чтобы добавить запись заполните все поля корректно.");
        }
    }
}