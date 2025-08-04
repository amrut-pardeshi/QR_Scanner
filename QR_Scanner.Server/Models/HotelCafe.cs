using Google.Cloud.Firestore;
using System.ComponentModel.DataAnnotations;

namespace QR_Scanner.Server.Models
{
    [FirestoreData]
    public class Establishment
    {
        [FirestoreProperty]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [FirestoreProperty]
        [Required]
        public string Name { get; set; } = string.Empty;

        [FirestoreProperty]
        [Required]
        public string Address { get; set; } = string.Empty;

        [FirestoreProperty]
        public string? City { get; set; }

        [FirestoreProperty]
        public string? Country { get; set; }

        [FirestoreProperty]
        public string? PostalCode { get; set; }

        [FirestoreProperty]
        public string? Phone { get; set; }

        [FirestoreProperty]
        public string? Email { get; set; }

        [FirestoreProperty]
        [Required]
        public EstablishmentType Type { get; set; }

        [FirestoreProperty]
        public Owner? Owner { get; set; }

        [FirestoreProperty]
        public bool IsActive { get; set; } = true;

        [FirestoreProperty]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [FirestoreProperty]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [FirestoreProperty]
        public string QRCodeDataUrl { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for creating a new establishment - excludes Id and QRCodeDataUrl
    /// </summary>
    public class CreateEstablishmentDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty;

        public string? City { get; set; }

        public string? Country { get; set; }

        public string? PostalCode { get; set; }

        public string? Phone { get; set; }

        public string? Email { get; set; }

        [Required]
        public EstablishmentType Type { get; set; }

        public Owner? Owner { get; set; }
    }

    /// <summary>
    /// DTO for updating an establishment - excludes Id and QRCodeDataUrl
    /// </summary>
    public class UpdateEstablishmentDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty;

        public string? City { get; set; }

        public string? Country { get; set; }

        public string? PostalCode { get; set; }

        public string? Phone { get; set; }

        public string? Email { get; set; }

        [Required]
        public EstablishmentType Type { get; set; }

        public Owner? Owner { get; set; }
    }

    [FirestoreData]
    public class Owner
    {
        [FirestoreProperty]
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [FirestoreProperty]
        [Required]
        [Phone]
        [StringLength(20, MinimumLength = 10)]
        public string Mobile { get; set; } = string.Empty;
    }

    public enum EstablishmentType
    {
        Hotel,
        Cafe,
        Restaurant,
        FastFood,
        Bar,
        Bakery,
        IceCream,
        TeaHouse
    }

    /// <summary>
    /// Extension methods for mapping between DTOs and Establishment entity
    /// </summary>
    public static class EstablishmentExtensions
    {
        /// <summary>
        /// Maps CreateEstablishmentDto to Establishment entity
        /// </summary>
        public static Establishment ToEstablishment(this CreateEstablishmentDto dto)
        {
            return new Establishment
            {
                Id = Guid.NewGuid().ToString(),
                Name = dto.Name,
                Address = dto.Address,
                City = dto.City,
                Country = dto.Country,
                PostalCode = dto.PostalCode,
                Phone = dto.Phone,
                Email = dto.Email,
                Type = dto.Type,
                Owner = dto.Owner,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                QRCodeDataUrl = string.Empty
            };
        }

        /// <summary>
        /// Maps UpdateEstablishmentDto to existing Establishment entity, preserving Id and QRCodeDataUrl
        /// </summary>
        public static Establishment ToEstablishment(this UpdateEstablishmentDto dto, Establishment existing)
        {
            return new Establishment
            {
                Id = existing.Id,
                Name = dto.Name,
                Address = dto.Address,
                City = dto.City,
                Country = dto.Country,
                PostalCode = dto.PostalCode,
                Phone = dto.Phone,
                Email = dto.Email,
                Type = dto.Type,
                Owner = dto.Owner,
                CreatedAt = existing.CreatedAt,
                UpdatedAt = DateTime.UtcNow,
                QRCodeDataUrl = existing.QRCodeDataUrl
            };
        }
    }

    // Menu Models
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

    // DTOs for Menu Management
    public class CreateMenuDto
    {
        [Required]
        public string EstablishmentId { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
    }

    public class UpdateMenuDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class CreateMenuCategoryDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int DisplayOrder { get; set; } = 0;
    }

    public class UpdateMenuCategoryDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int DisplayOrder { get; set; } = 0;

        public bool IsActive { get; set; } = true;
    }

    public class CreateMenuSubCategoryDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int DisplayOrder { get; set; } = 0;
    }

    public class UpdateMenuSubCategoryDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int DisplayOrder { get; set; } = 0;

        public bool IsActive { get; set; } = true;
    }

    public class CreateMenuItemDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        public string Currency { get; set; } = "USD";

        public bool IsVegetarian { get; set; } = false;

        public bool IsVegan { get; set; } = false;

        public bool IsGlutenFree { get; set; } = false;

        public string? ImageUrl { get; set; }

        public List<string>? Allergens { get; set; }

        public int PreparationTimeMinutes { get; set; } = 0;

        public int DisplayOrder { get; set; } = 0;
    }

    public class UpdateMenuItemDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        public string Currency { get; set; } = "USD";

        public bool IsAvailable { get; set; } = true;

        public bool IsVegetarian { get; set; } = false;

        public bool IsVegan { get; set; } = false;

        public bool IsGlutenFree { get; set; } = false;

        public string? ImageUrl { get; set; }

        public List<string>? Allergens { get; set; }

        public int PreparationTimeMinutes { get; set; } = 0;

        public int DisplayOrder { get; set; } = 0;
    }

    // Extension methods for Menu DTOs
    public static class MenuExtensions
    {
        public static Menu ToMenu(this CreateMenuDto dto)
        {
            return new Menu
            {
                Id = Guid.NewGuid().ToString(),
                EstablishmentId = dto.EstablishmentId,
                Name = dto.Name,
                Description = dto.Description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public static Menu ToMenu(this UpdateMenuDto dto, Menu existing)
        {
            return new Menu
            {
                Id = existing.Id,
                EstablishmentId = existing.EstablishmentId,
                Name = dto.Name,
                Description = dto.Description,
                IsActive = dto.IsActive,
                CreatedAt = existing.CreatedAt,
                UpdatedAt = DateTime.UtcNow,
                Categories = existing.Categories
            };
        }

        public static MenuCategory ToMenuCategory(this CreateMenuCategoryDto dto)
        {
            return new MenuCategory
            {
                Id = Guid.NewGuid().ToString(),
                Name = dto.Name,
                Description = dto.Description,
                DisplayOrder = dto.DisplayOrder
            };
        }

        public static MenuSubCategory ToMenuSubCategory(this CreateMenuSubCategoryDto dto)
        {
            return new MenuSubCategory
            {
                Id = Guid.NewGuid().ToString(),
                Name = dto.Name,
                Description = dto.Description,
                DisplayOrder = dto.DisplayOrder
            };
        }

        public static MenuItem ToMenuItem(this CreateMenuItemDto dto)
        {
            return new MenuItem
            {
                Id = Guid.NewGuid().ToString(),
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Currency = dto.Currency,
                IsVegetarian = dto.IsVegetarian,
                IsVegan = dto.IsVegan,
                IsGlutenFree = dto.IsGlutenFree,
                ImageUrl = dto.ImageUrl,
                Allergens = dto.Allergens,
                PreparationTimeMinutes = dto.PreparationTimeMinutes,
                DisplayOrder = dto.DisplayOrder
            };
        }
    }
}