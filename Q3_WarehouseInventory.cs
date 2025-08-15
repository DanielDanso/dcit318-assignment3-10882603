using System;
using System.Collections.Generic;

namespace WarehouseInventoryQ3
{
    // a) Marker interface
    public interface IInventoryItem
    {
        int Id { get; }
        string Name { get; }
        int Quantity { get; set; }
    }

    // e) Custom exceptions
    public class DuplicateItemException : Exception
    {
        public DuplicateItemException(string message) : base(message) { }
    }

    public class ItemNotFoundException : Exception
    {
        public ItemNotFoundException(string message) : base(message) { }
    }

    public class InvalidQuantityException : Exception
    {
        public InvalidQuantityException(string message) : base(message) { }
    }

    // b) ElectronicItem
    public class ElectronicItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public string Brand { get; }
        public int WarrantyMonths { get; }

        public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            Brand = brand;
            WarrantyMonths = warrantyMonths;
        }

        public override string ToString() =>
            $"ElectronicItem {{ Id={Id}, Name={Name}, Qty={Quantity}, Brand={Brand}, Warranty={WarrantyMonths}m }}";
    }

    // c) GroceryItem
    public class GroceryItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public DateTime ExpiryDate { get; }

        public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            ExpiryDate = expiryDate;
        }

        public override string ToString() =>
            $"GroceryItem {{ Id={Id}, Name={Name}, Qty={Quantity}, Expiry={ExpiryDate:d} }}";
    }

    // d) Generic InventoryRepository<T>
    public class InventoryRepository<T> where T : class, IInventoryItem
    {
        private readonly Dictionary<int, T> _items = new Dictionary<int, T>();

        public void AddItem(T item)
        {
            if (_items.ContainsKey(item.Id))
                throw new DuplicateItemException($"Item with Id {item.Id} already exists.");
            _items[item.Id] = item;
        }

        public T GetItemById(int id)
        {
            if (!_items.TryGetValue(id, out var item))
                throw new ItemNotFoundException($"Item with Id {id} not found.");
            return item;
        }

        public void RemoveItem(int id)
        {
            if (!_items.Remove(id))
                throw new ItemNotFoundException($"Item with Id {id} not found.");
        }

        public List<T> GetAllItems() => new List<T>(_items.Values);

        public void UpdateQuantity(int id, int newQuantity)
        {
            if (newQuantity < 0)
                throw new InvalidQuantityException("Quantity cannot be negative.");

            var item = GetItemById(id);
            item.Quantity = newQuantity; // valid because Quantity has a setter
        }
    }

    // f) WareHouseManager
    public class WareHouseManager
    {
        private readonly InventoryRepository<ElectronicItem> _electronics = new InventoryRepository<ElectronicItem>();
        private readonly InventoryRepository<GroceryItem> _groceries = new InventoryRepository<GroceryItem>();

        public void SeedData()
        {
            // Electronics
            _electronics.AddItem(new ElectronicItem(1, "Laptop", 10, "Lenovo", 24));
            _electronics.AddItem(new ElectronicItem(2, "Smartphone", 25, "Samsung", 12));
            _electronics.AddItem(new ElectronicItem(3, "Router", 15, "TP-Link", 18));

            // Groceries
            _groceries.AddItem(new GroceryItem(101, "Rice 5kg", 40, DateTime.Today.AddMonths(12)));
            _groceries.AddItem(new GroceryItem(102, "Milk 1L", 60, DateTime.Today.AddDays(20)));
            _groceries.AddItem(new GroceryItem(103, "Eggs 12-pack", 30, DateTime.Today.AddDays(14)));
        }

        public void PrintAllItems<T>(InventoryRepository<T> repo) where T : class, IInventoryItem
        {
            foreach (var item in repo.GetAllItems())
                Console.WriteLine(item);
        }

        public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : class, IInventoryItem
        {
            try
            {
                var current = repo.GetItemById(id);
                repo.UpdateQuantity(id, checked(current.Quantity + quantity));
                Console.WriteLine($"Stock increased: Id={id}, NewQty={current.Quantity}");
            }
            catch (Exception ex) when (
                ex is ItemNotFoundException ||
                ex is InvalidQuantityException ||
                ex is OverflowException)
            {
                Console.WriteLine($"[IncreaseStock Error] {ex.Message}");
            }
        }

        public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : class, IInventoryItem
        {
            try
            {
                repo.RemoveItem(id);
                Console.WriteLine($"Item removed: Id={id}");
            }
            catch (ItemNotFoundException ex)
            {
                Console.WriteLine($"[RemoveItem Error] {ex.Message}");
            }
        }

        // Convenience accessors for printing
        public InventoryRepository<ElectronicItem> ElectronicsRepo => _electronics;
        public InventoryRepository<GroceryItem> GroceriesRepo => _groceries;
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var manager = new WareHouseManager();

            // i–ii. Seed
            manager.SeedData();

            // iii. Print all grocery items
            Console.WriteLine("=== Groceries ===");
            manager.PrintAllItems(manager.GroceriesRepo);

            // iv. Print all electronic items
            Console.WriteLine("\n=== Electronics ===");
            manager.PrintAllItems(manager.ElectronicsRepo);

            // v. Try error scenarios
            Console.WriteLine("\n=== Error Scenarios ===");

            // Add a duplicate item
            try
            {
                manager.ElectronicsRepo.AddItem(new ElectronicItem(1, "Tablet", 5, "Apple", 12));
            }
            catch (DuplicateItemException ex)
            {
                Console.WriteLine($"[Duplicate Add Error] {ex.Message}");
            }

            // Remove a non-existent item
            manager.RemoveItemById(manager.GroceriesRepo, 999);

            // Update with invalid quantity
            try
            {
                manager.ElectronicsRepo.UpdateQuantity(2, -5);
            }
            catch (InvalidQuantityException ex)
            {
                Console.WriteLine($"[Invalid Quantity Error] {ex.Message}");
            }
            catch (ItemNotFoundException ex)
            {
                Console.WriteLine($"[Update Error] {ex.Message}");
            }

            // A valid stock increase (happy path)
            manager.IncreaseStock(manager.GroceriesRepo, 101, 10);

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
