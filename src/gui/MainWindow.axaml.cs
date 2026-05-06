using Avalonia.Controls;
using Avalonia.Interactivity;

using TransformXmlApp;
using Log;

namespace contour_test_task;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void ActivationTransformXml(object? sender, RoutedEventArgs e) 
    {
        Logger.Print("DEBUG", "Start Transform");
        TransformXml.TransformXmlMain();
    }

    private void UpdateData(object? sender, RoutedEventArgs e) 
    {
        Logger.Print("DEBUG", "Start Update");
        string name = AddName.Text ?? "";
        string surname = AddSurname.Text ?? "";
        string mount = AddMount.Text ?? "";
        string amount = AddAmount.Text ?? "";

        if (name != "" && surname != "" && mount != "" && amount != "") 
        {  
            TransformXml.AddRecordData(name, surname, mount, amount);
        } else {
            Logger.Print("INFO", "В запросе было передано пустое поле. Чтобы добавить запись заполните все поля.");
        }
    }
}