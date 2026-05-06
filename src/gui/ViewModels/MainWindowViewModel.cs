using contour_test_task.Models;
using System.Collections.ObjectModel;
using TransformXmlApp;

namespace contour_test_task.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        // Сотрудники для TreeView
        public ObservableCollection<EmployeeNode> Employees { get; } = new();

        public MainWindowViewModel()
        {
            LoadEmployees();
        }

        public void Reload()
        {
            LoadEmployees();
        }

        private void LoadEmployees()
        {
            Employees.Clear();

            var records = TransformXml.GetRecordData();

            foreach (var emp in records)
            {
                string title = $"{emp.Name} {emp.Surname}";

                var salaryNodes = new ObservableCollection<SalaryNode>();

                foreach (var s in emp.Salaries)
                {
                    string salaryTitle = $"{s.Mount}: {s.Amount.ToString(System.Globalization.CultureInfo.InvariantCulture)}";
                    salaryNodes.Add(new SalaryNode(salaryTitle));
                }

                string allTitle = $"all_amount: {emp.AllAmount.ToString(System.Globalization.CultureInfo.InvariantCulture)}";
                salaryNodes.Add(new SalaryNode(allTitle));

                Employees.Add(new EmployeeNode(title, salaryNodes));
            }
        }
    }
}