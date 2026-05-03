using System.Globalization;
using System.Xml.Linq;
using Saxon.Api;

class Program
{
    static void Main()
    {
        for (int number = 1; number <= 2; number += 1)
        {
            string xmlPath = Path.Combine("data", $"Data{number}.xml");
            string xslPath = Path.Combine("src", $"style{number}.xslt");
            string outputPath = Path.Combine("result", $"Result{number}.xml");

            XMLTransform(xmlPath, xslPath, outputPath);
             
            Console.WriteLine($"Result{number}.xml создан в папке result.");
        
            AddSalaryAll(outputPath);

            Console.WriteLine($"Result{number}_v2.xml создан в папке result.");
        
            if (number == 1) {
                PaySalaryAll(xmlPath);

                Console.WriteLine($"Data{number}_v2.xml создан в папке result.");
            }
        }
    }

    static void XMLTransform(string xmlPath, string xslPath, string outputPath)
    {
        try
        {
            Directory.CreateDirectory("result");

            Processor processor = new Processor();

            XsltCompiler compiler = processor.NewXsltCompiler();

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
            Console.WriteLine("Ошибка:");
            Console.WriteLine(ex.Message);
        }
    }

    static void AddSalaryAll(string xmlPath)
    {
        var doc = XDocument.Load(xmlPath);
        foreach (var employee in doc.Descendants("Employee"))
        {
            var sum = employee.Elements("salary").Sum(s => ParseAmount((string)s.Attribute("amount")));

            employee.Add(new XElement("salary_all", new XAttribute("amount", sum.ToString(CultureInfo.InvariantCulture))));
        }

        doc.Save($"{xmlPath.Split('.')[0]}_v2.xml");
    }

    static void PaySalaryAll(string xmlPath)
    {
        var doc = XDocument.Load(xmlPath);
        var pay = doc.Element("Pay");
        var sum = doc.Descendants("item").Sum(s => ParseAmount((string)s.Attribute("amount")));
        pay.Add(new XAttribute("all_amount", sum.ToString(CultureInfo.InvariantCulture)));
        
        var outputPath = Path.GetFileNameWithoutExtension(xmlPath);
        doc.Save($"result/{outputPath}_v2.xml");
    }

    static double ParseAmount(string amount_str)
    {
        if (string.IsNullOrWhiteSpace(amount_str))
            return 0;

        amount_str = amount_str.Replace(',', '.');

        double.TryParse(amount_str, NumberStyles.Any, CultureInfo.InvariantCulture, out double amount);
        
        return amount;
    }
}