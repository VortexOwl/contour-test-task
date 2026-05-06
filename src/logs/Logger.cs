using System;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;


namespace Log;

public class Logger
{
    public static void Print(string level, string message, [CallerMemberName] string method = "Class Program")
    {
        Console.WriteLine($"{level} | {method}: {message}");
    }
}