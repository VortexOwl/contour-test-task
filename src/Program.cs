using System.Globalization;
using System.Xml.Linq;
using System.Runtime.CompilerServices;
using Saxon.Api;

class Program
{
    static void Main()
    {
        const string outputFolder = "result";

        Directory.CreateDirectory(outputFolder);

        Processor processor = new Processor();
        XsltCompiler compiler = processor.NewXsltCompiler();

        for (int number = 1; number <= 2; number += 1)
        {
            string xmlPath = Path.Combine("data", $"Data{number}.xml");
            string xslPath = Path.Combine("src", $"style{number}.xslt");
            string outputPath = Path.Combine(outputFolder, $"Result{number}.xml");

            TransformXml(xmlPath, xslPath, outputPath, processor, compiler);
             
            Log("INFO", $"Result{number}.xml создан в папке {outputFolder}.");
        
            AddSalaryAll(outputPath);

            Log("INFO", $"Result{number}_v2.xml создан в папке {outputFolder}.");
        
            if (number == 1) {
                PaySalaryAll(xmlPath, outputFolder);

                Log("INFO", $"Data{number}_v2.xml создан в папке {outputFolder}.");
            }
        }
    }

    static void TransformXml(string xmlPath, string xslPath, string outputPath, Processor processor, XsltCompiler compiler)
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
            Log("ERROR", $"\n{ex.GetType().Name}: {ex.Message}");
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
        const string tagNameAllSalary = "all_salary";

        try
        {
            var doc = XDocument.Load(xmlPath);

            foreach (var employee in doc.Descendants(tagNameEmployee))
            {
                var sum = employee.Elements(tagNameSalary).Sum(s => ParseAmount((string?)s.Attribute(tagNameAmount)));

                employee.Add(new XElement(tagNameAllSalary, new XAttribute(tagNameAmount, sum.ToString(CultureInfo.InvariantCulture))));
            }
            
            doc.Save(savePath);
        }
        catch (Exception ex)
        {
            Log("ERROR", $"\n{ex.GetType().Name}: {ex.Message}");
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
                Log("ERROR", $"Корневой элемент {tagNamePay} не найден.");
                return;
            }
            tagPay.Add(new XAttribute(tagNameAllAmount, sum.ToString(CultureInfo.InvariantCulture)));
            doc.Save(outputPath);
        }
        catch (Exception ex)
        {
            Log("ERROR", $"\n{ex.GetType().Name}: {ex.Message}");
        }
    }

    static double ParseAmount(string? amountStr)
    {
        if (string.IsNullOrWhiteSpace(amountStr)) 
        {
            Log("WARNING", "Передана пустая строка.");
            return 0;
        }

        string amountNormalized = amountStr.Replace(',', '.');

        try
        {
            double amount = double.Parse(amountNormalized, NumberStyles.Any, CultureInfo.InvariantCulture);
            
            return amount;
        }
        catch (Exception ex)
        {
            Log("ERROR", $"\n{ex.GetType().Name}: {ex.Message}");
            return 0;
        }
    }

    static void Log(string level, string message, [CallerMemberName] string method = "Class Program")
    {
        Console.WriteLine($"{level} | {method}: {message}");
    }
}