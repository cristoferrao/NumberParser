using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using System.Xml.Linq;

// Define a data format interface
public interface IDataFormat
{
    void Persist(int[] data, string filePath);
}

// Implement text file format
public class TextFormat : IDataFormat
{
    public void Persist(int[] data, string filePath)
    {
        File.WriteAllText(filePath, string.Join(",", data));
    }
}

// Implement JSON format
public class JsonFormat : IDataFormat
{
    public void Persist(int[] data, string filePath)
    {
        string json = JsonConvert.SerializeObject(data);
        File.WriteAllText(filePath, json);
    }
}

// Implement XML format
public class XmlFormat : IDataFormat
{
    public void Persist(int[] data, string filePath)
    {
        XElement xml = new XElement("Numbers",
                            data.Select(num => new XElement("Number", num)));
        xml.Save(filePath);
    }
}

// Factory class to create IDataFormat instances
public static class FormatFactory
{
    public static IDataFormat CreateFormat(string format)
    {
        switch (format.ToLower())
        {
            case "text":
                return new TextFormat();
            case "json":
                return new JsonFormat();
            case "xml":
                return new XmlFormat();
            default:
                throw new ArgumentException("Unsupported format.");
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        string input;
        string[] inputArgs;

        // If arguments are provided when starting the program, use them
        if (args.Length >= 2)
        {
            inputArgs = args;
        }
        else
        {
            Console.WriteLine("Enter numbers separated by commas:");
            input = Console.ReadLine();
            inputArgs = input.Split(' ');
        }

        if (inputArgs.Length < 2)
        {
            Console.WriteLine("Usage: NumberParser <numbers> <format>");
            return;
        }

        // Parse numbers from the input
        string[] numbersStr = inputArgs[0].Split(',');
        int[] numbers = numbersStr.Select(int.Parse).ToArray();

        // Sort numbers in descending order
        Array.Sort(numbers);
        Array.Reverse(numbers);

        // Get format from input
        string format = inputArgs[1];

        // Get file path
        string filePath = $"output.{format.ToLower()}";

        // Create corresponding IDataFormat instance
        IDataFormat dataFormat = FormatFactory.CreateFormat(format);

        // Persist the sorted numbers
        dataFormat.Persist(numbers, filePath);

        Console.WriteLine($"Numbers sorted and persisted in {format} format at {filePath}");
    }
}