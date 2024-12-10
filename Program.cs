using System;
using System.Collections.Generic;
using System.Linq;

// Клас для товарів
public class Product
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    public double Rating { get; set; }

    public Product(string name, decimal price, string description, string category, double rating)
    {
        Name = name;
        Price = price;
        Description = description;
        Category = category;
        Rating = rating;
    }
}

// Клас для користувачів
public class User
{
    public string Login { get; set; }
    public string Password { get; set; }
    public List<Order> PurchaseHistory { get; set; }

    public User(string login, string password)
    {
        Login = login;
        Password = password;
        PurchaseHistory = new List<Order>();
    }
}

// Клас для замовлень
public class Order
{
    public List<Product> Products { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice => Products.Sum(p => p.Price) * Quantity;
    public string Status { get; set; }

    public Order(List<Product> products, int quantity, string status)
    {
        Products = products;
        Quantity = quantity;
        Status = status;
    }
}

// Інтерфейс для пошуку товарів
public interface ISearchable
{
    List<Product> SearchByPrice(decimal minPrice, decimal maxPrice);
    List<Product> SearchByCategory(string category);
    List<Product> SearchByRating(double minRating);
}

// Клас магазину, що реалізує інтерфейс ISearchable
public class Store : ISearchable
{
    public List<Product> Products { get; set; }
    public List<User> Users { get; set; }

    public Store()
    {
        Products = new List<Product>();
        Users = new List<User>();
    }

    public List<Product> SearchByPrice(decimal minPrice, decimal maxPrice)
    {
        return Products.Where(p => p.Price >= minPrice && p.Price <= maxPrice).ToList();
    }

    public List<Product> SearchByCategory(string category)
    {
        return Products.Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    public List<Product> SearchByRating(double minRating)
    {
        return Products.Where(p => p.Rating >= minRating).ToList();
    }

    public void AddProduct(Product product)
    {
        Products.Add(product);
    }

    public void AddUser(User user)
    {
        Users.Add(user);
    }

    public void CreateOrder(User user, List<Product> products, int quantity)
    {
        if (!Users.Contains(user))
        {
            Console.WriteLine("User not found.");
            return;
        }

        var order = new Order(products, quantity, "In Progress");
        user.PurchaseHistory.Add(order);
        Console.WriteLine("Order created successfully.");
        Console.WriteLine($"Order details: Total Price = ${order.TotalPrice}, Status = {order.Status}");
    }
}

// Основний клас програми для взаємодії з користувачем
public class Program
{
    public static void Main(string[] args)
    {
        // Створення магазину
        Store store = new Store();

        // Додавання товарів
        store.AddProduct(new Product("Laptop", 1500m, "High performance laptop", "Electronics", 4.5));
        store.AddProduct(new Product("Headphones", 100m, "Noise-cancelling headphones", "Electronics", 4.7));
        store.AddProduct(new Product("Smartphone", 800m, "Latest model smartphone", "Electronics", 4.6));
        store.AddProduct(new Product("Coffee Maker", 120m, "Automatic coffee maker", "Home Appliances", 4.3));

        // Додавання користувача з логіном admin і паролем admin
        store.AddUser(new User("admin", "admin"));

        // Аутентифікація користувача
        Console.WriteLine("Enter your login:");
        string login = Console.ReadLine();
        Console.WriteLine("Enter your password:");
        string password = Console.ReadLine();

        User currentUser = store.Users.FirstOrDefault(u => u.Login == login && u.Password == password);
        if (currentUser == null)
        {
            Console.WriteLine("Invalid login or password.");
            return;
        }

        Console.WriteLine("Login successful!");

        // Пошук товарів
        Console.WriteLine("Search for products:");
        Console.WriteLine("1. Search by price range");
        Console.WriteLine("2. Search by category");
        Console.WriteLine("3. Search by rating");
        Console.WriteLine("Enter your choice:");
        string choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                Console.WriteLine("Enter minimum price:");
                decimal minPrice = Convert.ToDecimal(Console.ReadLine());
                Console.WriteLine("Enter maximum price:");
                decimal maxPrice = Convert.ToDecimal(Console.ReadLine());
                var productsByPrice = store.SearchByPrice(minPrice, maxPrice);
                DisplayProducts(productsByPrice);
                break;

            case "2":
                Console.WriteLine("Enter category:");
                string category = Console.ReadLine();
                var productsByCategory = store.SearchByCategory(category);
                DisplayProducts(productsByCategory);
                break;

            case "3":
                Console.WriteLine("Enter minimum rating:");
                double minRating = Convert.ToDouble(Console.ReadLine());
                var productsByRating = store.SearchByRating(minRating);
                DisplayProducts(productsByRating);
                break;

            default:
                Console.WriteLine("Invalid choice.");
                break;
        }

        // Створення замовлення
        Console.WriteLine("Do you want to create an order? (yes/no)");
        string orderChoice = Console.ReadLine();
        if (orderChoice.Equals("yes", StringComparison.OrdinalIgnoreCase))
        {
            CreateOrder(store, currentUser);
        }
    }

    public static void DisplayProducts(List<Product> products)
    {
        if (products.Count == 0)
        {
            Console.WriteLine("No products found.");
            return;
        }

        Console.WriteLine("Products found:");
        for (int i = 0; i < products.Count; i++)
        {
            var product = products[i];
            Console.WriteLine($"{i}. {product.Name} - ${product.Price} (Category: {product.Category}, Rating: {product.Rating})");
        }
    }

    public static void CreateOrder(Store store, User currentUser)
    {
        // Показати доступні продукти для вибору
        DisplayProducts(store.Products);

        Console.WriteLine("Enter product indices (comma-separated):");
        string input = Console.ReadLine();

        // Перевірка введених індексів і їх перетворення на список цілих чисел
        List<int> productIndices = input.Split(',')
            .Select(index => int.TryParse(index, out int result) ? result : -1)
            .Where(index => index >= 0 && index < store.Products.Count)
            .ToList();

        if (productIndices.Count == 0)
        {
            Console.WriteLine("Invalid product indices. No products selected.");
            return;
        }

        // Створення списку вибраних продуктів
        List<Product> selectedProducts = productIndices.Select(index => store.Products[index]).ToList();

        Console.WriteLine("Enter quantity:");
        if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity <= 0)
        {
            Console.WriteLine("Invalid quantity.");
            return;
        }

        // Створення замовлення
        store.CreateOrder(currentUser, selectedProducts, quantity);
    }
}
