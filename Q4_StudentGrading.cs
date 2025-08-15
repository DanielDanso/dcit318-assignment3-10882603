using System;
using System.Collections.Generic;
using System.IO;

// Custom Exceptions
public class InvalidScoreFormatException : Exception
{
    public InvalidScoreFormatException(string message) : base(message) { }
}

public class MissingFieldException : Exception
{
    public MissingFieldException(string message) : base(message) { }
}

// Student Class
public class Student
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public int Score { get; set; }

    public string GetGrade()
    {
        if (Score >= 80 && Score <= 100) return "A";
        if (Score >= 70) return "B";
        if (Score >= 60) return "C";
        if (Score >= 50) return "D";
        return "F";
    }
}

// Processor Class
public class StudentResultProcessor
{
    public List<Student> ReadStudentsFromFile(string inputFilePath)
    {
        List<Student> students = new List<Student>();

        using (StreamReader reader = new StreamReader(inputFilePath))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                var parts = line.Split(',');
                if (parts.Length < 3)
                {
                    throw new MissingFieldException($"Missing fields in line: {line}");
                }

                if (!int.TryParse(parts[0], out int id))
                {
                    throw new FormatException($"Invalid ID format: {parts[0]}");
                }

                string fullName = parts[1];

                if (!int.TryParse(parts[2], out int score))
                {
                    throw new InvalidScoreFormatException($"Invalid score format: {parts[2]}");
                }

                students.Add(new Student
                {
                    Id = id,
                    FullName = fullName,
                    Score = score
                });
            }
        }

        return students;
    }

    public void WriteReportToFile(List<Student> students, string outputFilePath)
    {
        using (StreamWriter writer = new StreamWriter(outputFilePath))
        {
            foreach (var student in students)
            {
                writer.WriteLine($"{student.FullName} (ID: {student.Id}): Score = {student.Score}, Grade = {student.GetGrade()}");
            }
        }
    }
}

// Main Program
public class Program
{
    public static void Main()
    {
        StudentResultProcessor processor = new StudentResultProcessor();

        try
        {
            string inputPath = "students.txt";
            string outputPath = "report.txt";

            var students = processor.ReadStudentsFromFile(inputPath);
            processor.WriteReportToFile(students, outputPath);

            Console.WriteLine("Report generated successfully.");
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine("Error: Input file not found. " + ex.Message);
        }
        catch (InvalidScoreFormatException ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
        catch (MissingFieldException ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("An unexpected error occurred: " + ex.Message);
        }
    }
}
