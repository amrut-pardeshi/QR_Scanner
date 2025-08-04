using Google.Cloud.Firestore;
using System.ComponentModel.DataAnnotations;

namespace QR_Scanner.Server.Models
{
    [FirestoreData]
    public class Menu
    {
        [FirestoreProperty]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [FirestoreProperty]
        [Required]
        public string EstablishmentId { get; set; } = string.Empty;

        [FirestoreProperty]
        [Required]
        public string Name { get; set; } = string.Empty;

        [FirestoreProperty]
        public string? Description { get; set; }

        [FirestoreProperty]
        public bool IsActive { get; set; } = true;

        [FirestoreProperty]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [FirestoreProperty]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [FirestoreProperty]
        public List<MenuCategory> Categories { get; set; } = new List<MenuCategory>();
    }

    [FirestoreData]
    public class MenuCategory
    {
        [FirestoreProperty]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [FirestoreProperty]
        [Required]
        public string Name { get; set; } = string.Empty;

        [FirestoreProperty]
        public string? Description { get; set; }

        [FirestoreProperty]
        public int DisplayOrder { get; set; } = 0;

        [FirestoreProperty]
        public bool IsActive { get; set; } = true;

        [FirestoreProperty]
        public List<MenuSubCategory> SubCategories { get; set; } = new List<MenuSubCategory>();
    }

    [FirestoreData]
    public class MenuSubCategory
    {
        [FirestoreProperty]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [FirestoreProperty]
        [Required]
        public string Name { get; set; } = string.Empty;

        [FirestoreProperty]
        public string? Description { get; set; }

        [FirestoreProperty]
        public int DisplayOrder { get; set; } = 0;

        [FirestoreProperty]
        public bool IsActive { get; set; } = true;

        [FirestoreProperty]
        public List<MenuItem> Items { get; set; } = new List<MenuItem>();
    }

    [FirestoreData]
    public class MenuItem
    {
        [FirestoreProperty]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [FirestoreProperty]
        [Required]
        public string Name { get; set; } = string.Empty;

        [FirestoreProperty]
        public string? Description { get; set; }

        [FirestoreProperty]
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [FirestoreProperty]
        public string Currency { get; set; } = "USD";

        [FirestoreProperty]
        public bool IsAvailable { get; set; } = true;

        [FirestoreProperty]
        public bool IsVegetarian { get; set; } = false;

        [FirestoreProperty]
        public bool IsVegan { get; set; } = false;

        [FirestoreProperty]
        public bool IsGlutenFree { get; set; } = false;

        [FirestoreProperty]
        public string? ImageUrl { get; set; }

        [FirestoreProperty]
        public List<string>? Allergens { get; set; }

        [FirestoreProperty]
        public int PreparationTimeMinutes { get; set; } = 0;

        [FirestoreProperty]
        public int DisplayOrder { get; set; } = 0;
    }
}