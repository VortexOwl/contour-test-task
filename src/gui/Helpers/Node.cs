using System.Collections.ObjectModel;

namespace contour_test_task.Models
{
    public class EmployeeNode
    {
        public string Title { get; }              // "Lena Ivanova"
        public ObservableCollection<SalaryNode> Salaries { get; }

        public EmployeeNode(string title, ObservableCollection<SalaryNode> salaries)
        {
            Title = title;
            Salaries = salaries;
        }
    }

    public class SalaryNode
    {
        public string Title { get; }             // "march: 1001.1", "all_amount: 6003.20"

        public SalaryNode(string title)
        {
            Title = title;
        }

        public override string ToString() => Title;
    }
}