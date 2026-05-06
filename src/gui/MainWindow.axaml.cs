using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Globalization;

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
        Logger.Print("DEBUG", "Start transform XML");
        TransformXml.TransformXmlMain();
        _viewModel.Reload();
    }

    private void AddData(object? sender, RoutedEventArgs e) 
    {
        Logger.Print("DEBUG", "Start add data");
        string name = AddName.Text ?? "";
        string surname = AddSurname.Text ?? "";
        string mount = AddMount.Text ?? "";
        string amount = AddAmount.Text ?? "";

        string normalized = amount.Replace(',', '.');
        bool isAmount = decimal.TryParse(normalized, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal dAmount) && dAmount >= 0;

        if (name != "" && surname != "" && mount != "" && isAmount) 
        {
            TransformXml.AddRecordData(name, surname, mount, amount);
        } else {
            Logger.Print("INFO", "В запросе было передано поле с некорректным значением. Чтобы добавить запись заполните все поля корректно.");
        }
    }
}