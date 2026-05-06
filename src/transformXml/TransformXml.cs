using System;
using System.Globalization;
using System.Xml.Linq;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using Saxon.Api;

using contour_test_task.Log;

namespace contour_test_task.TransformXmlApp;

public class TransformXml
{
    public static void TransformXmlMain()
    {
        const string dataFolder = "data";
        const string styleFolder = "src/transformXml/xsltStyles";
        const string outputFolder = "result";

        Directory.CreateDirectory(outputFolder);

        Processor processor = new Processor();
        XsltCompiler compiler = processor.NewXsltCompiler();

        for (int number = 1; number <= 2; number += 1)
        {
            string xmlPath = Path.Combine(dataFolder, $"Data{number}.xml");
            string xslPath = Path.Combine(styleFolder, $"style{number}.xslt");
            string outputPath = Path.Combine(outputFolder, $"Employees{number}.xml");

            Transform(xmlPath, xslPath, outputPath, processor, compiler);
            
            Logger.Print("INFO", $"Employees{number}.xml создан в папке {outputFolder}.");
        
            AddSalaryAll(outputPath);

            Logger.Print("INFO", $"Employees{number}_v2.xml создан в папке {outputFolder}.");
        
            if (number == 1) {
                PaySalaryAll(xmlPath, outputFolder);

                Logger.Print("INFO", $"Data{number}_v2.xml создан в папке {outputFolder}.");
            }
        }
    }

    static void Transform(string xmlPath, string xslPath, string outputPath, Processor processor, XsltCompiler compiler)
    {
        try
        {
            XsltExecutable executable =
                compiler.Compile(
                    new Uri(Path.GetFullPath(xslPath))
                );

            Xslt30Transformer transformer =
                executable.Load30();

            XdmNode input =
                processor.NewDocumentBuilder()
                    .Build(
                        new Uri(Path.GetFullPath(xmlPath))
                    );

            Serializer serializer =
                processor.NewSerializer();

            serializer.SetOutputFile(outputPath);

            transformer.ApplyTemplates(input, serializer);
        }
        catch (Exception ex)
        {
            Logger.Print("ERROR", $"\n{ex.GetType().Name}: {ex.Message}");
        }
    }

    static void AddSalaryAll(string xmlPath)
    {
        string directory = Path.GetDirectoryName(xmlPath) ?? "";
        string fileName = Path.GetFileNameWithoutExtension(xmlPath);
        string savePath = Path.Combine(directory, $"{fileName}_v2.xml");

        const string tagNameEmployee = "Employee";
        const string tagNameAmount = "amount";
        const string tagNameSalary = "salary";

        try
        {
            var doc = XDocument.Load(xmlPath);

            foreach (var tagEmployee in doc.Descendants(tagNameEmployee))
            {
                var sum = tagEmployee.Elements(tagNameSalary).Sum(s => ParseAmount((string?)s.Attribute(tagNameAmount)));

                tagEmployee.Add(new XAttribute(tagNameAmount, sum.ToString(CultureInfo.InvariantCulture)));
            }
            
            doc.Save(savePath);
        }
        catch (Exception ex)
        {
            Logger.Print("ERROR", $"\n{ex.GetType().Name}: {ex.Message}");
        }
    }

    static void PaySalaryAll(string xmlPath, string outputFolder)
    {
        string outputNameFile = Path.GetFileNameWithoutExtension(xmlPath);
        string outputPath = Path.Combine(outputFolder, $"{outputNameFile}_v2.xml");

        const string tagNamePay = "Pay";
        const string tagNameItem = "item";
        const string tagNameAmount = "amount";
        const string tagNameAllAmount = "all_amount";

        try
        {
            var doc = XDocument.Load(xmlPath);
            var tagPay = doc.Element(tagNamePay);
            var sum = doc.Descendants(tagNameItem).Sum(s => ParseAmount((string?)s.Attribute(tagNameAmount)));
            
            if (tagPay == null)
            {
                Logger.Print("ERROR", $"Корневой элемент {tagNamePay} не найден.");
                return;
            }
            tagPay.Add(new XAttribute(tagNameAllAmount, sum.ToString(CultureInfo.InvariantCulture)));
            doc.Save(outputPath);
        }
        catch (Exception ex)
        {
            Logger.Print("ERROR", $"\n{ex.GetType().Name}: {ex.Message}");
        }
    }

    public static void AddRecordData(string name, string surname, string mount, string amount) 
    {
        const string dataFolder = "data";
        
        const string tagNamePay = "Pay";
        const string tagNameItem = "item";
        const string tagNameName = "name";
        const string tagNameSurname = "surname";
        const string tagNameMount = "mount";
        const string tagNameAmount = "amount";

        try
        {
            string xmlPath = Path.Combine(dataFolder, "Data1.xml");
            var doc = XDocument.Load(xmlPath);
            var tagPay = doc.Element(tagNamePay);
            
            if (tagPay == null)
            {
                Logger.Print("ERROR", $"Корневой элемент {tagNamePay} не найден.");
                return;
            }
            tagPay.Add(new XElement(tagNameItem, new XAttribute(tagNameName, name), new XAttribute(tagNameSurname, surname), new XAttribute(tagNameMount, mount), new XAttribute(tagNameAmount, amount)));
            doc.Save(xmlPath);
        }
        catch (Exception ex)
        {
            Logger.Print("ERROR", $"\n{ex.GetType().Name}: {ex.Message}");
        }
    }

    static decimal ParseAmount(string? amountStr)
    {
        if (string.IsNullOrWhiteSpace(amountStr)) 
        {
            Logger.Print("WARNING", "Передана пустая строка.");
            return 0;
        }

        string amountNormalized = amountStr.Replace(',', '.');

        try
        {
            decimal amount = decimal.Parse(amountNormalized, NumberStyles.Any, CultureInfo.InvariantCulture);
            
            return amount;
        }
        catch (Exception ex)
        {
            Logger.Print("ERROR", $"\n{ex.GetType().Name}: {ex.Message}");
            return 0;
        }
    }

    public class EmployeeRecord
    {
        public string Name { get; set; } = "";
        public string Surname { get; set; } = "";
        public List<SalaryRecord> Salaries { get; set; } = new();
        public decimal AllAmount { get; set; }
    }

    public class SalaryRecord
    {
        public string Mount { get; set; } = "";
        public decimal Amount { get; set; }
    }

    public static List<EmployeeRecord> GetRecordData()
    {
        const string dataFolder = "result";

        const string tagNameEmployee = "Employee";
        const string tagNameName = "name";
        const string tagNameSurname = "surname";
        const string tagNameMount = "mount";
        const string tagNameAmount = "amount";
        const string tagNameSalary = "salary";

        try
        {
            string xmlPath = Path.Combine(dataFolder, "Employees1_v2.xml");
            var doc = XDocument.Load(xmlPath);

            List<EmployeeRecord> employees = new();

            foreach (var tagEmployee in doc.Descendants(tagNameEmployee))
            {
                var employee = new EmployeeRecord
                {
                    Name = tagEmployee.Attribute(tagNameName)?.Value ?? "",
                    Surname = tagEmployee.Attribute(tagNameSurname)?.Value ?? "",
                    AllAmount = ParseAmount(tagEmployee.Attribute(tagNameAmount)?.Value)
                };

                foreach (var tagSalary in tagEmployee.Elements(tagNameSalary))
                {
                    var salary = new SalaryRecord
                    {
                        Mount = tagSalary.Attribute(tagNameMount)?.Value ?? "",
                        Amount = ParseAmount(tagSalary.Attribute(tagNameAmount)?.Value)
                    };
                    employee.Salaries.Add(salary);
                }

                employees.Add(employee);
            }

            return employees;
        }
        catch (Exception ex)
        {
            Logger.Print("ERROR", $"\n{ex.GetType().Name}: {ex.Message}");
            return new List<EmployeeRecord>();
        }
    }
}
