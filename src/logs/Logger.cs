using System;
using System.Runtime.CompilerServices;


namespace contour_test_task.Log;

public class Logger
{
    public static void Print(string level, string message, [CallerMemberName] string method = "Class Program")
    {
        Console.WriteLine($"{level} | {method}: {message}");
    }
}