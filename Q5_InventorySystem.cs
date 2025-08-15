using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

// Marker Interface
public interface IInventoryEntity
{
    int Id { get; }
}

// Immutable Inventory Record
public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

// Generic Inventory Logger
public class InventoryLogger<T> where T : IInventoryEntity
{
    private List<T> _log = new List<T>();
    private string _filePath;

    public InventoryLogger(string filePath)
    {
        _filePath = filePath;
    }

    public void Add(T item) => _log.Add(item);

    public List<T> GetAll() => new List<T>(_log);

    public void SaveToFile()
    {
        try
        {
            string json = JsonSerializer.Serialize(_log, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving file: " + ex.Message);
        }
    }

    public void LoadFromFile()
    {
        try
        {
            if (File.Exists(_filePath))
            {
                string json = File.ReadAllText(_filePath);
                _log = JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
            }
            else
            {
                Console.WriteLine("File not found. Starting with empty log.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading file: " + ex.Message);
        }
    }
}

// Inventory Application
public class InventoryApp
{
    private InventoryLogger<InventoryItem> _logger;

    public InventoryApp(string filePath)
    {
        _logger = new InventoryLogger<InventoryItem>(filePath);
    }

    public void SeedSampleData()
    {
        _logger.Add(new InventoryItem(1, "Laptop", 10, DateTime.Now));
        _logger.Add(new InventoryItem(2, "Keyboard", 25, DateTime.Now));
        _logger.Add(new InventoryItem(3, "Mouse", 30, DateTime.Now));
    }

    public void SaveData() => _logger.SaveToFile();

    public void LoadData() => _logger.LoadFromFile();

    public void PrintAllItems()
    {
        foreach (var item in _logger.GetAll())
        {
            Console.WriteLine($"{item.Name} (ID: {item.Id}) - Qty: {item.Quantity}, Added: {item.DateAdded}");
        }
    }
}

// Main Program
public class Program
{
    public static void Main()
    {
        string filePath = "inventory.json";

        InventoryApp app = new InventoryApp(filePath);

        app.SeedSampleData();
        app.SaveData();

        Console.WriteLine("Data saved. Simulating new session...\n");

        InventoryApp newApp = new InventoryApp(filePath);
        newApp.LoadData();
        newApp.PrintAllItems();
    }
}
